// ── appService.bicep ─────────────────────────────────────────────────────────
// Deploys an App Service Plan and a Linux Web App pre-wired with the FlightDex
// SQL connection string and Service Bus connection string as app settings.

@description('Web app name — must be globally unique.')
param appName string

@description('App Service Plan name.')
param planName string

@description('Azure region.')
param location string

@description('App Service Plan SKU.')
@allowed(['F1', 'B1', 'B2', 'S1', 'S2', 'S3', 'P1v3', 'P2v3'])
param skuName string = 'B1'

@description('SQL Server fully-qualified domain name.')
param sqlServerFqdn string

@description('SQL Database name.')
param sqlDatabaseName string

@description('SQL Server administrator login.')
param sqlAdminUsername string

@description('SQL Server administrator password.')
@secure()
param sqlAdminPassword string

@description('Service Bus primary connection string (Send + Listen).')
@secure()
param serviceBusConnectionString string

param tags object = {}

// Built here so the password never surfaces in an output.
var sqlConnectionString = 'Server=tcp:${sqlServerFqdn},1433;Database=${sqlDatabaseName};User Id=${sqlAdminUsername};Password=${sqlAdminPassword};Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;'

// ── App Service Plan ──────────────────────────────────────────────────────────

resource plan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: planName
  location: location
  tags: tags
  sku: {
    name: skuName
  }
  kind: 'linux'
  properties: {
    reserved: true   // required for Linux plans
  }
}

// ── Web App ───────────────────────────────────────────────────────────────────

resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: appName
  location: location
  tags: tags
  kind: 'app,linux'
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      http20Enabled: true
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'ServiceBus__ConnectionString'
          value: serviceBusConnectionString
        }
      ]
      connectionStrings: [
        {
          name: 'FlightDex'
          connectionString: sqlConnectionString
          type: 'SQLAzure'
        }
      ]
    }
  }
}

// ── Outputs ───────────────────────────────────────────────────────────────────

output defaultHostName string = webApp.properties.defaultHostName
output webAppId        string = webApp.id
