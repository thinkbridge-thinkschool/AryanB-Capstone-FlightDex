using '../main.bicep'

// ── Dev environment ───────────────────────────────────────────────────────────
// Cheapest SKUs — suitable for CI and developer testing.
// Identity end-to-end: there is no SQL password here (or anywhere). Provide the
// Entra admin and the one external secret via environment variables:
//   $env:FLIGHTDEX_SQL_AAD_ADMIN_LOGIN     = '<user-upn-or-group-name>'
//   $env:FLIGHTDEX_SQL_AAD_ADMIN_OBJECT_ID = '<objectId>'
//   $env:FLIGHTDEX_EXTERNAL_FEED_API_KEY   = '<api-key>'

param environmentName        = 'dev'
param location               = 'eastasia'

// SQL is Entra-only: the deploying admin/group becomes the SQL AAD admin.
param sqlAadAdminLogin        = readEnvironmentVariable('FLIGHTDEX_SQL_AAD_ADMIN_LOGIN')
param sqlAadAdminObjectId     = readEnvironmentVariable('FLIGHTDEX_SQL_AAD_ADMIN_OBJECT_ID')
param sqlAadAdminPrincipalType = 'Group'

// The single remaining secret lives in Key Vault, surfaced as a KV reference.
param externalFeedApiKey      = readEnvironmentVariable('FLIGHTDEX_EXTERNAL_FEED_API_KEY')

// Easy Auth client ID is optional; leave blank to deploy before the app reg exists.
param entraAuthClientId       = ''

// App Service: B1 (Basic, 1 core / 1.75 GB) — ~$13/month
param appServiceSkuName = 'B1'

// SQL Database: Basic (5 DTU) — ~$5/month
param sqlSkuName        = 'Basic'
param sqlSkuTier        = 'Basic'

// Service Bus: Basic (queues only) — pay-per-operation
param serviceBusSkuName = 'Basic'
