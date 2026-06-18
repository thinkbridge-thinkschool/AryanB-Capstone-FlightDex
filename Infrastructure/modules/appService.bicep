// ── appService.bicep ─────────────────────────────────────────────────────────
// App Service Plan + Linux Web App wired for identity end-to-end:
//   • System-assigned managed identity (used for SQL + Service Bus).
//   • SQL connection string uses Active Directory auth — no User Id / Password.
//   • Service Bus is addressed by namespace hostname only — no SAS key.
//   • The one remaining secret is a Key Vault reference, not a plaintext value.
//   • Optional Entra ID (Easy Auth) front door when a client ID is supplied.

@description('Web app name — must be globally unique.')
param appName string

@description('App Service Plan name.')
param planName string

@description('Azure region.')
param location string

@description('App Service Plan SKU.')
@allowed(['F1', 'B1', 'B2', 'S1', 'S2', 'S3', 'P1v3', 'P2v3'])
param skuName string = 'B1'

@description('SQL Server fully-qualified domain name.')
param sqlServerFqdn string

@description('SQL Database name.')
param sqlDatabaseName string

@description('Service Bus fully-qualified namespace (e.g. ns.servicebus.windows.net).')
param serviceBusFqdn string

@description('Versionless Key Vault secret URI for the external feed API key.')
param keyVaultSecretUri string

@description('Entra ID app-registration (client) ID for Easy Auth. Empty leaves auth off.')
param entraAuthClientId string = ''

@description('Application Insights connection string for telemetry export.')
param appInsightsConnectionString string = ''

param tags object = {}

// AAD-token auth — note there is no password in this string.
var sqlConnectionString = 'Server=tcp:${sqlServerFqdn},1433;Database=${sqlDatabaseName};Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;'

// ── App Service Plan ──────────────────────────────────────────────────────────

resource plan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: planName
  location: location
  tags: tags
  sku: {
    name: skuName
  }
  kind: 'linux'
  properties: {
    reserved: true   // required for Linux plans
  }
}

// ── Web App ───────────────────────────────────────────────────────────────────

resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: appName
  location: location
  tags: tags
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      http20Enabled:  true
      minTlsVersion:  '1.2'
      ftpsState:      'Disabled'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          // Hostname only — the SDK uses DefaultAzureCredential against it.
          name: 'ServiceBus__FullyQualifiedNamespace'
          value: serviceBusFqdn
        }
        {
          // Key Vault reference — App Service resolves it via the MI at runtime.
          // The literal secret value is never stored here.
          name: 'ExternalFlightFeed__ApiKey'
          value: '@Microsoft.KeyVault(SecretUri=${keyVaultSecretUri})'
        }
        {
          // OpenTelemetry (Azure Monitor distro) reads this on startup.
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
      ]
      connectionStrings: [
        {
          name: 'FlightDex'
          connectionString: sqlConnectionString
          type: 'SQLAzure'
        }
      ]
    }
  }
}

// ── Entra ID app auth (Easy Auth) ───────────────────────────────────────────────
// Enabled only when a client ID is provided, so the template still deploys
// before the app registration exists.
resource authSettings 'Microsoft.Web/sites/config@2023-01-01' = if (!empty(entraAuthClientId)) {
  parent: webApp
  name: 'authsettingsV2'
  properties: {
    globalValidation: {
      requireAuthentication:       true
      unauthenticatedClientAction: 'Return401'
    }
    identityProviders: {
      azureActiveDirectory: {
        enabled: true
        registration: {
          openIdIssuer: '${environment().authentication.loginEndpoint}${subscription().tenantId}/v2.0'
          clientId:     entraAuthClientId
        }
        validation: {
          allowedAudiences: [
            'api://${entraAuthClientId}'
          ]
        }
      }
    }
  }
}

// ── Outputs ───────────────────────────────────────────────────────────────────

output defaultHostName string = webApp.properties.defaultHostName
output webAppId        string = webApp.id
output principalId     string = webApp.identity.principalId
output planId          string = plan.id
