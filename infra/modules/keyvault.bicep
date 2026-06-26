// Key Vault holding the JWT signing key. The Container App reads it via a Key Vault reference
// resolved with the managed identity — the secret never appears in app settings or git.
@description('Azure region for the vault.')
param location string

@description('Key Vault name (3-24 chars, globally unique).')
param name string

@description('Principal id of the managed identity that reads secrets.')
param readerPrincipalId string

@description('The JWT signing key to store as a secret.')
@secure()
param jwtKey string

@description('Tags applied to the vault.')
param tags object = {}

resource vault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: name
  location: location
  tags: tags
  properties: {
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    // RBAC authorization (not access policies) — pairs with the role assignment below.
    enableRbacAuthorization: true
    publicNetworkAccess: 'Enabled'
  }
}

resource jwtSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: vault
  name: 'jwt-key'
  properties: {
    value: jwtKey
  }
}

// 4633458b-17de-408a-b874-0445c86b69e6 = Key Vault Secrets User
resource secretsUser 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(vault.id, readerPrincipalId, 'KeyVaultSecretsUser')
  scope: vault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')
    principalId: readerPrincipalId
    principalType: 'ServicePrincipal'
  }
}

// Versionless secret URI — Container Apps re-resolves it on rotation.
output jwtSecretUri string = '${vault.properties.vaultUri}secrets/jwt-key'
output name string = vault.name
