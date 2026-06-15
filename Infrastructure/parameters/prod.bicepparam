using '../main.bicep'

// ── Prod environment ──────────────────────────────────────────────────────────
// Production-grade SKUs with SLA backing.
// sqlAdminPassword is NOT stored here; inject from a Key Vault secret or CI secret:
//   --parameters sqlAdminPassword=$env:FLIGHTDEX_SQL_PASSWORD

param environmentName   = 'prod'
param location          = 'eastasia'
param sqlAdminUsername  = 'flightdexadmin'
param sqlAdminPassword  = readEnvironmentVariable('FLIGHTDEX_SQL_PASSWORD')

// App Service: S2 (Standard, 2 cores / 3.5 GB, custom domains + SSL) — ~$100/month
param appServiceSkuName = 'S2'

// SQL Database: S2 Standard (50 DTU) — ~$150/month
param sqlSkuName        = 'S2'
param sqlSkuTier        = 'Standard'

// Service Bus: Standard (queues + topics, 10 M ops/month included) — ~$10/month
param serviceBusSkuName = 'Standard'
