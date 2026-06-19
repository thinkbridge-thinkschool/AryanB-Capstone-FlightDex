targetScope = 'resourceGroup'

// ── Parameters ────────────────────────────────────────────────────────────────

@description('Short environment identifier used in every resource name.')
@allowed(['dev', 'prod'])
param environmentName string

@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('Entra ID admin login name (user UPN or group display name) for SQL.')
param sqlAadAdminLogin string

@description('Object ID (SID) of the Entra ID admin user or group for SQL.')
param sqlAadAdminObjectId string

@description('Principal type of the SQL Entra ID admin.')
@allowed(['User', 'Group', 'Application'])
param sqlAadAdminPrincipalType string = 'Group'

@description('External flight-feed API key — stored in Key Vault, never in app settings.')
@secure()
param externalFeedApiKey string

@description('Entra ID app-registration (client) ID for App Service Easy Auth. Empty leaves auth off.')
param entraAuthClientId string = ''

@description('App Service Plan SKU (B1 for dev, S2 for prod).')
@allowed(['F1', 'B1', 'B2', 'S1', 'S2', 'S3', 'P1v3', 'P2v3'])
param appServiceSkuName string = 'B1'

@description('Azure SQL Database SKU name (Basic / S2 …).')
param sqlSkuName string = 'Basic'

@description('Azure SQL Database pricing tier (Basic / Standard / Premium).')
param sqlSkuTier string = 'Basic'

@description('Service Bus namespace SKU (Basic / Standard / Premium).')
@allowed(['Basic', 'Standard', 'Premium'])
param serviceBusSkuName string = 'Basic'

@description('Public network access on the data tier (SQL + Key Vault). Disabled puts them behind private endpoints only.')
@allowed(['Enabled', 'Disabled'])
param dataPublicNetworkAccess string = 'Disabled'

// ── Variables ─────────────────────────────────────────────────────────────────

// Six-char suffix derived from the resource group so every name is globally unique.
var suffix = take(uniqueString(resourceGroup().id), 6)

var tags = {
  Application: 'FlightDex'
  Environment: environmentName
  ManagedBy:   'Bicep'
}

// ── Modules ───────────────────────────────────────────────────────────────────

// The private network the data tier sits behind. Created first so the data
// services and apps can reference its subnets and DNS zones.
module network 'modules/network.bicep' = {
  name: 'deploy-network-${environmentName}'
  params: {
    vnetName: 'flightdex-vnet-${environmentName}-${suffix}'
    location: location
    tags:     tags
  }
}

module sql 'modules/sql.bicep' = {
  name: 'deploy-sql-${environmentName}'
  params: {
    serverName:           'flightdex-sql-${environmentName}-${suffix}'
    databaseName:         'FlightDex'
    aadAdminLogin:        sqlAadAdminLogin
    aadAdminObjectId:     sqlAadAdminObjectId
    aadAdminPrincipalType: sqlAadAdminPrincipalType
    location:             location
    skuName:              sqlSkuName
    skuTier:              sqlSkuTier
    publicNetworkAccess:  dataPublicNetworkAccess
    tags:                 tags
  }
}

module bus 'modules/serviceBus.bicep' = {
  name: 'deploy-bus-${environmentName}'
  params: {
    namespaceName: 'flightdex-bus-${environmentName}-${suffix}'
    location:      location
    skuName:       serviceBusSkuName
    tags:          tags
  }
}

module kv 'modules/keyVault.bicep' = {
  name: 'deploy-kv-${environmentName}'
  params: {
    vaultName:         'flightdex-kv-${environmentName}-${suffix}'
    location:          location
    externalFeedApiKey: externalFeedApiKey
    publicNetworkAccess: dataPublicNetworkAccess
    tags:              tags
  }
}

// Private endpoints for the data tier (SQL + Key Vault), placed in snet-data and
// wired to the private DNS zones. With dataPublicNetworkAccess = 'Disabled' these
// are the only network path to the data.
module privateEndpoints 'modules/privateEndpoints.bicep' = {
  name: 'deploy-pe-${environmentName}'
  params: {
    location:          location
    dataSubnetId:      network.outputs.dataSubnetId
    sqlServerId:       sql.outputs.serverId
    keyVaultId:        kv.outputs.vaultId
    sqlDnsZoneId:      network.outputs.sqlDnsZoneId
    keyVaultDnsZoneId: network.outputs.keyVaultDnsZoneId
    tags:              tags
  }
}

// Workspace-based Application Insights — telemetry sink for the API and worker.
module monitor 'modules/appInsights.bicep' = {
  name: 'deploy-ai-${environmentName}'
  params: {
    name:          'flightdex-ai-${environmentName}-${suffix}'
    workspaceName: 'flightdex-law-${environmentName}-${suffix}'
    location:      location
    tags:          tags
  }
}

module app 'modules/appService.bicep' = {
  name: 'deploy-app-${environmentName}'
  params: {
    appName:                     'flightdex-api-${environmentName}-${suffix}'
    planName:                    'flightdex-plan-${environmentName}-${suffix}'
    location:                    location
    skuName:                     appServiceSkuName
    sqlServerFqdn:               sql.outputs.serverFqdn
    sqlDatabaseName:             sql.outputs.databaseName
    serviceBusFqdn:              bus.outputs.fullyQualifiedNamespace
    keyVaultSecretUri:           kv.outputs.secretUri
    entraAuthClientId:           entraAuthClientId
    appInsightsConnectionString: monitor.outputs.connectionString
    vnetIntegrationSubnetId:     network.outputs.appSubnetId
    tags:                        tags
  }
}

// The Service Bus consumer — shares the API's plan, reaches SQL/Service Bus by identity.
module worker 'modules/workerService.bicep' = {
  name: 'deploy-worker-${environmentName}'
  params: {
    appName:                     'flightdex-worker-${environmentName}-${suffix}'
    serverFarmId:                app.outputs.planId
    location:                    location
    sqlServerFqdn:               sql.outputs.serverFqdn
    sqlDatabaseName:             sql.outputs.databaseName
    serviceBusFqdn:              bus.outputs.fullyQualifiedNamespace
    appInsightsConnectionString: monitor.outputs.connectionString
    vnetIntegrationSubnetId:     network.outputs.workerSubnetId
    tags:                        tags
  }
}

// Grants Send (API) / Receive (worker) on Service Bus and read on Key Vault (API).
// Runs last because it needs both apps' principal IDs.
module rbac 'modules/rbac.bicep' = {
  name: 'deploy-rbac-${environmentName}'
  params: {
    apiPrincipalId:          app.outputs.principalId
    workerPrincipalId:       worker.outputs.principalId
    serviceBusNamespaceName: bus.outputs.namespaceName
    keyVaultName:            kv.outputs.vaultName
  }
}

// ── Outputs ───────────────────────────────────────────────────────────────────

output apiUrl             string = 'https://${app.outputs.defaultHostName}'
output workerUrl          string = 'https://${worker.outputs.defaultHostName}'
output sqlServerFqdn      string = sql.outputs.serverFqdn
output serviceBusEndpoint string = bus.outputs.endpoint
output keyVaultUri        string = kv.outputs.vaultUri
output apiPrincipalId     string = app.outputs.principalId
output workerPrincipalId  string = worker.outputs.principalId
output appInsightsName    string = monitor.outputs.componentName
