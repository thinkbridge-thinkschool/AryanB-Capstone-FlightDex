// Azure Container Registry (Basic — cheapest SKU) that holds the API image, plus an AcrPull
// role assignment so the managed identity can pull without admin credentials.
@description('Azure region for the registry.')
param location string

@description('Registry name (alphanumeric, globally unique).')
param name string

@description('Principal id of the managed identity that pulls images.')
param pullPrincipalId string

@description('Tags applied to the registry.')
param tags object = {}

resource registry 'Microsoft.ContainerRegistry/registries@2023-11-01-preview' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: 'Basic'
  }
  properties: {
    // Identity-based pulls only; the admin user stays disabled.
    adminUserEnabled: false
  }
}

// 7f951dda-4ed3-4680-a7ca-43fe172d538d = AcrPull
resource acrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(registry.id, pullPrincipalId, 'AcrPull')
  scope: registry
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
    principalId: pullPrincipalId
    principalType: 'ServicePrincipal'
  }
}

output loginServer string = registry.properties.loginServer
output name string = registry.name
