using '../main.bicep'

// ── Dev environment ───────────────────────────────────────────────────────────
// Cheapest SKUs — suitable for CI and developer testing.
// sqlAdminPassword is NOT stored here; pass via:
//   --parameters sqlAdminPassword=$env:FLIGHTDEX_SQL_PASSWORD

param environmentName   = 'dev'
param location          = 'eastasia'
param sqlAdminUsername  = 'flightdexadmin'
param sqlAdminPassword  = readEnvironmentVariable('FLIGHTDEX_SQL_PASSWORD')

// App Service: B1 (Basic, 1 core / 1.75 GB) — ~$13/month
param appServiceSkuName = 'B1'

// SQL Database: Basic (5 DTU) — ~$5/month
param sqlSkuName        = 'Basic'
param sqlSkuTier        = 'Basic'

// Service Bus: Basic (queues only) — pay-per-operation
param serviceBusSkuName = 'Basic'
