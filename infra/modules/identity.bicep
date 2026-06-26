// User-assigned managed identity (an Entra ID identity). The Container App uses it to pull
// images from ACR and read secrets from Key Vault — no passwords or connection secrets anywhere.
@description('Azure region for the identity.')
param location string

@description('Managed identity name.')
param name string

@description('Tags applied to the identity.')
param tags object = {}

resource uami 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: name
  location: location
  tags: tags
}

output id string = uami.id
output name string = uami.name
output principalId string = uami.properties.principalId
output clientId string = uami.properties.clientId
