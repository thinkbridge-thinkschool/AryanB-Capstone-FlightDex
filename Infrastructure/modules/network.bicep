// ── network.bicep ─────────────────────────────────────────────────────────────
// The private network the data tier sits behind. Creates one VNet with three
// subnets and the private DNS zones that resolve the data services to their private
// IPs:
//   • snet-data    — holds the private endpoints (NICs into the data services).
//   • snet-app     — delegated to App Service so the API can route outbound through
//                    the VNet (regional VNet integration).
//   • snet-worker  — same, for the worker.
// The private DNS zones are linked to the VNet so that inside it, names like
// <server>.database.windows.net resolve to the private endpoint's 10.x address
// instead of the public one.

param vnetName string
param location string
param tags object = {}

@description('Private DNS zone for Azure SQL private endpoints.')
var sqlPrivateDnsZoneName = 'privatelink${environment().suffixes.sqlServerHostname}'

@description('Private DNS zone for Key Vault private endpoints.')
var keyVaultPrivateDnsZoneName = 'privatelink.vaultcore.azure.net'

// ── Virtual network + subnets ─────────────────────────────────────────────────
resource vnet 'Microsoft.Network/virtualNetworks@2023-09-01' = {
  name: vnetName
  location: location
  tags: tags
  properties: {
    addressSpace: {
      addressPrefixes: ['10.20.0.0/16']
    }
    subnets: [
      {
        name: 'snet-data'
        properties: {
          addressPrefix: '10.20.0.0/24'
          // Required so a private endpoint NIC can be placed in this subnet.
          privateEndpointNetworkPolicies: 'Disabled'
        }
      }
      {
        name: 'snet-app'
        properties: {
          addressPrefix: '10.20.1.0/24'
          delegations: [
            {
              name: 'webapp'
              properties: { serviceName: 'Microsoft.Web/serverFarms' }
            }
          ]
        }
      }
      {
        name: 'snet-worker'
        properties: {
          addressPrefix: '10.20.2.0/24'
          delegations: [
            {
              name: 'webapp'
              properties: { serviceName: 'Microsoft.Web/serverFarms' }
            }
          ]
        }
      }
    ]
  }
}

// ── Private DNS zones + VNet links ─────────────────────────────────────────────
resource sqlZone 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: sqlPrivateDnsZoneName
  location: 'global'
  tags: tags
}

resource sqlZoneLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2020-06-01' = {
  parent: sqlZone
  name: '${vnetName}-sql-link'
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: { id: vnet.id }
  }
}

resource kvZone 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: keyVaultPrivateDnsZoneName
  location: 'global'
  tags: tags
}

resource kvZoneLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2020-06-01' = {
  parent: kvZone
  name: '${vnetName}-kv-link'
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: { id: vnet.id }
  }
}

// ── Outputs ───────────────────────────────────────────────────────────────────
output dataSubnetId      string = vnet.properties.subnets[0].id
output appSubnetId       string = vnet.properties.subnets[1].id
output workerSubnetId    string = vnet.properties.subnets[2].id
output sqlDnsZoneId      string = sqlZone.id
output keyVaultDnsZoneId string = kvZone.id
