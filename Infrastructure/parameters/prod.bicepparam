using '../main.bicep'

// ── Prod environment ──────────────────────────────────────────────────────────
// Production-grade SKUs with SLA backing.
// Identity end-to-end: there is no SQL password here (or anywhere). Provide the
// Entra admin and the one external secret via environment variables:
//   $env:FLIGHTDEX_SQL_AAD_ADMIN_LOGIN     = '<user-upn-or-group-name>'
//   $env:FLIGHTDEX_SQL_AAD_ADMIN_OBJECT_ID = '<objectId>'
//   $env:FLIGHTDEX_EXTERNAL_FEED_API_KEY   = '<api-key>'

param environmentName        = 'prod'
param location               = 'eastasia'

// SQL is Entra-only: the deploying admin/group becomes the SQL AAD admin.
param sqlAadAdminLogin        = readEnvironmentVariable('FLIGHTDEX_SQL_AAD_ADMIN_LOGIN')
param sqlAadAdminObjectId     = readEnvironmentVariable('FLIGHTDEX_SQL_AAD_ADMIN_OBJECT_ID')
param sqlAadAdminPrincipalType = 'Group'

// The single remaining secret lives in Key Vault, surfaced as a KV reference.
param externalFeedApiKey      = readEnvironmentVariable('FLIGHTDEX_EXTERNAL_FEED_API_KEY')

// Easy Auth client ID is optional; leave blank to deploy before the app reg exists.
param entraAuthClientId       = ''

// App Service: S2 (Standard, 2 cores / 3.5 GB, custom domains + SSL) — ~$100/month
param appServiceSkuName = 'S2'

// SQL Database: S2 Standard (50 DTU) — ~$150/month
param sqlSkuName        = 'S2'
param sqlSkuTier        = 'Standard'

// Service Bus: Standard (queues + topics, 10 M ops/month included) — ~$10/month
param serviceBusSkuName = 'Standard'
