// ── sql.bicep ─────────────────────────────────────────────────────────────────
// Azure SQL Server + Database with Entra-ID-ONLY authentication.
// There is no SQL admin login/password: azureADOnlyAuthentication = true makes
// the server reject password auth outright, so no SQL secret exists anywhere.
// The App Service connects with its managed identity (see appService.bicep).

param serverName string
param databaseName string
param location string

@description('Entra ID admin login name (user UPN or group display name).')
param aadAdminLogin string

@description('Object ID (SID) of the Entra ID admin user or group.')
param aadAdminObjectId string

@description('Principal type of the Entra ID admin.')
@allowed(['User', 'Group', 'Application'])
param aadAdminPrincipalType string = 'Group'

@description('SQL Database SKU name — Basic (dev) or S2 (prod).')
param skuName string = 'Basic'

@description('SQL Database pricing tier — Basic or Standard.')
param skuTier string = 'Basic'

param tags object = {}

// ── SQL Server (Entra-only) ─────────────────────────────────────────────────────

resource server 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: serverName
  location: location
  tags: tags
  properties: {
    version:             '12.0'
    minimalTlsVersion:   '1.2'
    publicNetworkAccess: 'Enabled'
    // No administratorLogin / administratorLoginPassword: the server is
    // provisioned Entra-only, so there is no SQL password to store or leak.
    administrators: {
      administratorType:         'ActiveDirectory'
      principalType:             aadAdminPrincipalType
      login:                     aadAdminLogin
      sid:                       aadAdminObjectId
      tenantId:                  subscription().tenantId
      azureADOnlyAuthentication: true
    }
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
