// ── workerService.bicep ──────────────────────────────────────────────────────
// The Service Bus consumer host. A headless background worker, deployed as a Linux
// Web App so App Service keeps it running (Always On) and probes its /healthz route.
// Identity end-to-end, same as the API:
//   • System-assigned managed identity (Service Bus receive + SQL via AAD token).
//   • SQL connection string uses Active Directory auth — no User Id / Password.
//   • Service Bus is addressed by namespace hostname only — no SAS key.
//   • App Insights connection string passed in for OpenTelemetry export.
// Shares the API's App Service Plan to avoid a second plan's cost.

@description('Worker web app name — must be globally unique.')
param appName string

@description('Resource ID of the (shared) App Service Plan.')
param serverFarmId string

@description('Azure region.')
param location string

@description('SQL Server fully-qualified domain name.')
param sqlServerFqdn string

@description('SQL Database name.')
param sqlDatabaseName string

@description('Service Bus fully-qualified namespace (e.g. ns.servicebus.windows.net).')
param serviceBusFqdn string

@description('Application Insights connection string for telemetry export.')
param appInsightsConnectionString string

param tags object = {}

// AAD-token auth — note there is no password in this string.
var sqlConnectionString = 'Server=tcp:${sqlServerFqdn},1433;Database=${sqlDatabaseName};Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;'

resource workerApp 'Microsoft.Web/sites@2023-01-01' = {
  name: appName
  location: location
  tags: tags
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: serverFarmId
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      alwaysOn:       true   // keep the background consumer resident
      http20Enabled:  true
      minTlsVersion:  '1.2'
      ftpsState:      'Disabled'
      healthCheckPath: '/healthz'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          // Hostname only — the SDK uses DefaultAzureCredential against it.
          name: 'ServiceBus__FullyQualifiedNamespace'
          value: serviceBusFqdn
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
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

output defaultHostName string = workerApp.properties.defaultHostName
output principalId     string = workerApp.identity.principalId
