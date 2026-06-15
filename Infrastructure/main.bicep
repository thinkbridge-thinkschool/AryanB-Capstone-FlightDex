targetScope = 'resourceGroup'

// ── Parameters ────────────────────────────────────────────────────────────────

@description('Short environment identifier used in every resource name.')
@allowed(['dev', 'prod'])
param environmentName string

@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('SQL Server administrator login name.')
param sqlAdminUsername string = 'flightdexadmin'

@description('SQL Server administrator password.')
@secure()
param sqlAdminPassword string

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
    serverName:    'flightdex-sql-${environmentName}-${suffix}'
    databaseName:  'FlightDex'
    adminUsername: sqlAdminUsername
    adminPassword: sqlAdminPassword
    location:      location
    skuName:       sqlSkuName
    skuTier:       sqlSkuTier
    tags:          tags
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

module app 'modules/appService.bicep' = {
  name: 'deploy-app-${environmentName}'
  params: {
    appName:                    'flightdex-api-${environmentName}-${suffix}'
    planName:                   'flightdex-plan-${environmentName}-${suffix}'
    location:                   location
    skuName:                    appServiceSkuName
    sqlServerFqdn:              sql.outputs.serverFqdn
    sqlDatabaseName:            sql.outputs.databaseName
    sqlAdminUsername:           sqlAdminUsername
    sqlAdminPassword:           sqlAdminPassword
    serviceBusConnectionString: bus.outputs.connectionString
    tags:                       tags
  }
}

// ── Outputs ───────────────────────────────────────────────────────────────────

output apiUrl             string = 'https://${app.outputs.defaultHostName}'
output sqlServerFqdn      string = sql.outputs.serverFqdn
output serviceBusEndpoint string = bus.outputs.endpoint
