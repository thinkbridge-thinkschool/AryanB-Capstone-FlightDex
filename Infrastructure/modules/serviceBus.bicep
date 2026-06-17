// ── serviceBus.bicep ──────────────────────────────────────────────────────────
// Service Bus Namespace + integration-events queue.
// No SAS authorization rule and no connection string are emitted: the API
// authenticates with its managed identity over RBAC (granted in rbac.bicep),
// so there is no Service Bus key anywhere in app settings.

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
  // Local SAS-key auth disabled — identity (Entra ID + RBAC) is the only path in.
  properties: {
    disableLocalAuth: true
  }
}

// ── Queue ─────────────────────────────────────────────────────────────────────

resource queue 'Microsoft.ServiceBus/namespaces/queues@2022-10-01-preview' = {
  parent: namespace
  name: queueName
  properties: {
    maxDeliveryCount:                 10
    deadLetteringOnMessageExpiration: true
    lockDuration:                     'PT1M'
    defaultMessageTimeToLive:         'P14D'
  }
}

// ── Outputs ───────────────────────────────────────────────────────────────────

output namespaceName           string = namespace.name
output endpoint                string = namespace.properties.serviceBusEndpoint
output fullyQualifiedNamespace string = '${namespace.name}.servicebus.windows.net'
