// Azure SQL — the cheapest always-on option: a single Basic-tier database (5 DTU / 2 GB, no
// auto-pause). The server is Entra-only (no SQL password); its Entra admin IS the app's managed
// identity, so the container connects as admin and EF migrations create the schema on startup —
// no post-deploy CREATE USER step. Both modules share this one database.
@description('Azure region for the SQL server.')
param location string

@description('SQL logical server name (globally unique).')
param serverName string

@description('Database name.')
param databaseName string = 'flightdex'

@description('Object (principal) id of the managed identity to set as the Entra-only admin.')
param adminPrincipalId string

@description('Display name (login) for the Entra admin — the managed identity name.')
param adminLogin string

@description('Tags applied to the resources.')
param tags object = {}

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: serverName
  location: location
  tags: tags
  properties: {
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    // Entra-only authentication; the managed identity is the sole admin.
    administrators: {
      administratorType: 'ActiveDirectory'
      principalType: 'Application' // a managed identity is a service principal
      login: adminLogin
      sid: adminPrincipalId
      tenantId: subscription().tenantId
      azureADOnlyAuthentication: true
    }
  }
}

resource database 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  tags: tags
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  properties: {
    maxSizeBytes: 2147483648 // 2 GB — the Basic-tier maximum
  }
}

// Allow access from Azure services (the Container App's egress). 0.0.0.0/0.0.0.0 is the
// "Allow Azure services and resources to access this server" rule, not the public internet.
resource allowAzure 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAllAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

output serverFqdn string = sqlServer.properties.fullyQualifiedDomainName
output databaseName string = database.name
output serverName string = sqlServer.name
