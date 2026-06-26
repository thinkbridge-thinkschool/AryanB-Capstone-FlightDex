// FlightDex Azure deployment — subscription-scoped entry point. Creates the resource group and
// wires every resource: Log Analytics, managed identity, ACR, Key Vault, Azure SQL (Basic), the
// Container Apps environment, the API container app, and the Static Web App for the UI.
// Provisioned as a single Azure Deployment Stack via `azd` (deployment-stacks alpha feature).
targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('azd environment name — used to derive resource names and the resource group.')
param environmentName string

@minLength(1)
@description('Azure region for all resources (must support Container Apps + Static Web Apps, e.g. eastasia).')
param location string

@secure()
@description('JWT signing key, stored as a Key Vault secret and referenced by the API.')
param jwtKey string

@description('Whether the API container app already exists (set by azd to preserve the deployed image).')
param apiExists bool = false

@description('Optional extra tags applied to every resource.')
param tags object = {}

// Deterministic short suffix for globally-unique names; stable across redeploys of the same env.
var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))
var defaultTags = union(tags, { 'azd-env-name': environmentName })

var apiAppName = 'ca-api-${resourceToken}'
var placeholderImage = 'mcr.microsoft.com/k8se/quickstart:latest'
var sqlDatabaseName = 'flightdex'

resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: 'rg-flightdex-${environmentName}'
  location: location
  tags: defaultTags
}

module monitoring 'modules/monitoring.bicep' = {
  scope: rg
  name: 'monitoring'
  params: {
    location: location
    name: 'log-${resourceToken}'
    tags: defaultTags
  }
}

module identity 'modules/identity.bicep' = {
  scope: rg
  name: 'identity'
  params: {
    location: location
    name: 'id-${resourceToken}'
    tags: defaultTags
  }
}

module registry 'modules/registry.bicep' = {
  scope: rg
  name: 'registry'
  params: {
    location: location
    name: 'acr${resourceToken}'
    pullPrincipalId: identity.outputs.principalId
    tags: defaultTags
  }
}

module keyVault 'modules/keyvault.bicep' = {
  scope: rg
  name: 'keyvault'
  params: {
    location: location
    name: 'kv-${resourceToken}'
    readerPrincipalId: identity.outputs.principalId
    jwtKey: jwtKey
    tags: defaultTags
  }
}

module sql 'modules/sql.bicep' = {
  scope: rg
  name: 'sql'
  params: {
    location: location
    serverName: 'sql-${resourceToken}'
    databaseName: sqlDatabaseName
    adminPrincipalId: identity.outputs.principalId
    adminLogin: identity.outputs.name
    tags: defaultTags
  }
}

module caenv 'modules/containerAppsEnv.bicep' = {
  scope: rg
  name: 'caenv'
  params: {
    location: location
    name: 'cae-${resourceToken}'
    logAnalyticsCustomerId: monitoring.outputs.customerId
    logAnalyticsWorkspaceId: monitoring.outputs.id
    tags: defaultTags
  }
}

module web 'modules/staticwebapp.bicep' = {
  scope: rg
  name: 'web'
  params: {
    location: location
    name: 'swa-${resourceToken}'
    tags: union(defaultTags, { 'azd-service-name': 'web' })
  }
}

// Preserve the image azd pushed across re-provisions; fall back to the placeholder on first run.
module fetchApiImage 'modules/fetch-container-image.bicep' = {
  scope: rg
  name: 'fetch-api-image'
  params: {
    name: apiAppName
    exists: apiExists
  }
}

var apiImage = (apiExists && length(fetchApiImage.outputs.containers) > 0)
  ? fetchApiImage.outputs.containers[0].image
  : placeholderImage

// Entra managed-identity auth to Azure SQL — no password. The identity is the SQL admin.
var sqlConnectionString = 'Server=tcp:${sql.outputs.serverFqdn},1433;Database=${sql.outputs.databaseName};Authentication=Active Directory Managed Identity;User Id=${identity.outputs.clientId};Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;'

module api 'modules/api.bicep' = {
  scope: rg
  name: 'api'
  params: {
    location: location
    name: apiAppName
    environmentId: caenv.outputs.id
    identityId: identity.outputs.id
    registryLoginServer: registry.outputs.loginServer
    jwtSecretUri: keyVault.outputs.jwtSecretUri
    corsOrigin: web.outputs.uri
    sqlConnectionString: sqlConnectionString
    image: apiImage
    tags: union(defaultTags, { 'azd-service-name': 'api' })
  }
}

// azd reads these as environment outputs.
output AZURE_LOCATION string = location
output AZURE_RESOURCE_GROUP string = rg.name
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = registry.outputs.loginServer
output AZURE_KEY_VAULT_NAME string = keyVault.outputs.name
output AZURE_SQL_SERVER string = sql.outputs.serverFqdn
output AZURE_SQL_DATABASE string = sql.outputs.databaseName
output API_BASE_URL string = api.outputs.uri
output WEB_BASE_URL string = web.outputs.uri
output SERVICE_API_NAME string = api.outputs.name
output SERVICE_WEB_NAME string = web.outputs.name
