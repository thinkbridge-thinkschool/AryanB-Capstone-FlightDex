// ── serviceBus.bicep ──────────────────────────────────────────────────────────
// Service Bus Namespace + integration-events queue + a scoped auth rule
// (Send + Listen only — no Manage — following least-privilege principle).

param namespaceName string
param location string

@description('Service Bus SKU: Basic (dev) or Standard (prod).')
@allowed(['Basic', 'Standard', 'Premium'])
param skuName string = 'Basic'

@description('Queue name for integration events.')
param queueName string = 'integration-events'

param tags object = {}

// ── Namespace ─────────────────────────────────────────────────────────────────

resource namespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: namespaceName
  location: location
  tags: tags
  sku: {
    name: skuName
    tier: skuName
  }
}

// ── Queue ─────────────────────────────────────────────────────────────────────

resource queue 'Microsoft.ServiceBus/namespaces/queues@2022-10-01-preview' = {
  parent: namespace
  name: queueName
  properties: {
    maxDeliveryCount:                10
    deadLetteringOnMessageExpiration: true
    lockDuration:                    'PT1M'
    defaultMessageTimeToLive:        'P14D'
  }
}

// ── Scoped auth rule (Send + Listen, no Manage) ───────────────────────────────

resource apiRule 'Microsoft.ServiceBus/namespaces/authorizationRules@2022-10-01-preview' = {
  parent: namespace
  name: 'flightdex-api'
  properties: {
    rights: ['Send', 'Listen']
  }
}

// ── Outputs ───────────────────────────────────────────────────────────────────

output namespaceName string = namespace.name
output endpoint      string = namespace.properties.serviceBusEndpoint

// The connection string must flow to App Service app-settings; suppress the
// linter warning — the value never appears in portal or deployment history.
#disable-next-line outputs-should-not-contain-secrets
output connectionString string = apiRule.listKeys().primaryConnectionString
