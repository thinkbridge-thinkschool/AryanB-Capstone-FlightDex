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

// ── Variables ─────────────────────────────────────────────────────────────────

// Six-char suffix derived from the resource group so every name is globally unique.
var suffix = take(uniqueString(resourceGroup().id), 6)

var tags = {
  Application: 'FlightDex'
  Environment: environmentName
  ManagedBy:   'Bicep'
}

// ── Modules ───────────────────────────────────────────────────────────────────

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
    tags:              tags
  }
}

module app 'modules/appService.bicep' = {
  name: 'deploy-app-${environmentName}'
  params: {
    appName:           'flightdex-api-${environmentName}-${suffix}'
    planName:          'flightdex-plan-${environmentName}-${suffix}'
    location:          location
    skuName:           appServiceSkuName
    sqlServerFqdn:     sql.outputs.serverFqdn
    sqlDatabaseName:   sql.outputs.databaseName
    serviceBusFqdn:    bus.outputs.fullyQualifiedNamespace
    keyVaultSecretUri: kv.outputs.secretUri
    entraAuthClientId: entraAuthClientId
    tags:              tags
  }
}

// Grants the app's managed identity Send/Receive on Service Bus and read on
// Key Vault. Runs last because it needs the app's principal ID.
module rbac 'modules/rbac.bicep' = {
  name: 'deploy-rbac-${environmentName}'
  params: {
    principalId:             app.outputs.principalId
    serviceBusNamespaceName: bus.outputs.namespaceName
    keyVaultName:            kv.outputs.vaultName
  }
}

// ── Outputs ───────────────────────────────────────────────────────────────────

output apiUrl             string = 'https://${app.outputs.defaultHostName}'
output sqlServerFqdn      string = sql.outputs.serverFqdn
output serviceBusEndpoint string = bus.outputs.endpoint
output keyVaultUri        string = kv.outputs.vaultUri
output apiPrincipalId     string = app.outputs.principalId
