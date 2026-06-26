// Container Apps managed environment (Consumption — serverless, pay-per-use) wired to Log
// Analytics. Data now lives in Azure SQL, so no Azure Files storage is attached here.
@description('Azure region for the environment.')
param location string

@description('Environment name.')
param name string

@description('Log Analytics workspace customer id (GUID).')
param logAnalyticsCustomerId string

@description('Log Analytics workspace resource id (for the shared key lookup).')
param logAnalyticsWorkspaceId string

@description('Tags applied to the environment.')
param tags object = {}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' existing = {
  name: last(split(logAnalyticsWorkspaceId, '/'))
}

resource environment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: name
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsCustomerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

output id string = environment.id
output name string = environment.name
output defaultDomain string = environment.properties.defaultDomain
