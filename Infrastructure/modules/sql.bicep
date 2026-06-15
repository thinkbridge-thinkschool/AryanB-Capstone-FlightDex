// ── sql.bicep ─────────────────────────────────────────────────────────────────
// Azure SQL Server + Database.  Firewall rule 0.0.0.0/0.0.0.0 allows all
// Azure-internal services to connect (standard Azure platform behaviour).

param serverName string
param databaseName string
param adminUsername string
@secure()
param adminPassword string
param location string

@description('SQL Database SKU name — Basic (dev) or S2 (prod).')
param skuName string = 'Basic'

@description('SQL Database pricing tier — Basic or Standard.')
param skuTier string = 'Basic'

param tags object = {}

// ── SQL Server ────────────────────────────────────────────────────────────────

resource server 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: serverName
  location: location
  tags: tags
  properties: {
    administratorLogin:         adminUsername
    administratorLoginPassword: adminPassword
    version:                    '12.0'
    minimalTlsVersion:          '1.2'
    publicNetworkAccess:        'Enabled'
  }
}

// Allows Azure-hosted services (including App Service) to reach the server.
resource allowAzureServices 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: server
  name: 'AllowAllAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress:   '0.0.0.0'
  }
}

// ── Database ──────────────────────────────────────────────────────────────────

resource database 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: server
  name: databaseName
  location: location
  tags: tags
  sku: {
    name: skuName
    tier: skuTier
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
  }
}

// ── Outputs ───────────────────────────────────────────────────────────────────

output serverFqdn   string = server.properties.fullyQualifiedDomainName
output databaseName string = database.name
