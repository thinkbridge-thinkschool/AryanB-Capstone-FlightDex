// ── privateEndpoints.bicep ────────────────────────────────────────────────────
// The actual private endpoints for the data tier — one NIC per service, placed in
// snet-data, each wired to its private DNS zone so in-VNet callers resolve the
// service to its private IP. With public network access disabled on the services
// themselves (see sql.bicep / keyVault.bicep), these endpoints become the *only*
// network path to the data, reachable solely from the VNet-integrated apps.
//
// Service Bus is intentionally not included: private endpoints require the Premium
// SKU, which the dev/prod plans don't use. It stays identity-only (disableLocalAuth)
// — documented as the cost-bounded exception in D27P1_Solution.md.

param location string
param tags object = {}

@description('Subnet that will hold the private endpoint NICs (snet-data).')
param dataSubnetId string

@description('Resource ID of the SQL logical server.')
param sqlServerId string

@description('Resource ID of the Key Vault.')
param keyVaultId string

@description('Private DNS zone ID for Azure SQL.')
param sqlDnsZoneId string

@description('Private DNS zone ID for Key Vault.')
param keyVaultDnsZoneId string

// ── SQL private endpoint ───────────────────────────────────────────────────────
resource sqlPe 'Microsoft.Network/privateEndpoints@2023-09-01' = {
  name: 'pe-sql'
  location: location
  tags: tags
  properties: {
    subnet: { id: dataSubnetId }
    privateLinkServiceConnections: [
      {
        name: 'sql'
        properties: {
          privateLinkServiceId: sqlServerId
          groupIds: ['sqlServer']
        }
      }
    ]
  }
}

resource sqlPeDns 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2023-09-01' = {
  parent: sqlPe
  name: 'default'
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'sql'
        properties: { privateDnsZoneId: sqlDnsZoneId }
      }
    ]
  }
}

// ── Key Vault private endpoint ─────────────────────────────────────────────────
resource kvPe 'Microsoft.Network/privateEndpoints@2023-09-01' = {
  name: 'pe-kv'
  location: location
  tags: tags
  properties: {
    subnet: { id: dataSubnetId }
    privateLinkServiceConnections: [
      {
        name: 'kv'
        properties: {
          privateLinkServiceId: keyVaultId
          groupIds: ['vault']
        }
      }
    ]
  }
}

resource kvPeDns 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2023-09-01' = {
  parent: kvPe
  name: 'default'
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'kv'
        properties: { privateDnsZoneId: keyVaultDnsZoneId }
      }
    ]
  }
}
