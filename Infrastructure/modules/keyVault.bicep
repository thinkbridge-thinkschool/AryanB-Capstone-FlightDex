// ── keyVault.bicep ────────────────────────────────────────────────────────────
// RBAC-enabled Key Vault holding the only remaining piece of sensitive config
// (an external flight-feed API key). The API never sees the value directly —
// App Service resolves it at runtime through a Key Vault reference, and access
// is granted to the app's managed identity via "Key Vault Secrets User"
// (see rbac.bicep). No secret value lands in app settings.

param vaultName string
param location string

@description('External flight-feed API key — the one secret that cannot be replaced by identity.')
@secure()
param externalFeedApiKey string

@description('Public network access. Disabled once the vault sits behind a private endpoint.')
@allowed(['Enabled', 'Disabled'])
param publicNetworkAccess string = 'Disabled'

param tags object = {}

resource vault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: vaultName
  location: location
  tags: tags
  properties: {
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name:   'standard'
    }
    enableRbacAuthorization: true   // RBAC, not access policies
    enableSoftDelete:        true
    publicNetworkAccess:     publicNetworkAccess
    // Deny the public internet but let the ARM/azd deployment write the secret
    // through the trusted-Azure-services bypass; the app reads it over the
    // private endpoint at runtime.
    networkAcls: {
      defaultAction: publicNetworkAccess == 'Disabled' ? 'Deny' : 'Allow'
      bypass:        'AzureServices'
    }
  }
}

resource secret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: vault
  name: 'ExternalFlightFeed-ApiKey'
  properties: {
    value: externalFeedApiKey
  }
}

// ── Outputs ───────────────────────────────────────────────────────────────────

output vaultName  string = vault.name
output vaultUri   string = vault.properties.vaultUri
output vaultId    string = vault.id
// Versionless secret URI so rotation flows through without redeploying the app.
output secretUri  string = '${vault.properties.vaultUri}secrets/${secret.name}'
