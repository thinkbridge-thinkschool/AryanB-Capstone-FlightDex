// Log Analytics workspace — the diagnostic backing store for the Container Apps environment.
@description('Azure region for the workspace.')
param location string

@description('Workspace name.')
param name string

@description('Tags applied to the workspace.')
param tags object = {}

resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: name
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    // Cheapest sensible retention; the env only needs short-lived operational logs.
    retentionInDays: 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
}

output id string = workspace.id
output customerId string = workspace.properties.customerId
output name string = workspace.name
