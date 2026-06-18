// ── rbac.bicep ────────────────────────────────────────────────────────────────
// Data-plane role assignments that replace every connection-string secret with
// identity. Runs after the apps exist so their system-assigned principal IDs are
// available. The API publishes (Send) and reads the feed key (Key Vault); the
// worker consumes (Receive).

@description('System-assigned principal ID of the API web app (publisher).')
param apiPrincipalId string

@description('System-assigned principal ID of the worker web app (consumer).')
param workerPrincipalId string

param serviceBusNamespaceName string
param keyVaultName string

// Built-in role definition IDs (stable GUIDs).
var sbDataSenderRoleId   = '69a216fc-b8fb-44d8-bc22-1f3c2cd27a39' // Azure Service Bus Data Sender
var sbDataReceiverRoleId = '4f6d3b9b-027b-4f4c-9142-0e5a2a2247e0' // Azure Service Bus Data Receiver
var kvSecretsUserRoleId  = '4633458b-17de-408a-b874-0445c86b69e6' // Key Vault Secrets User

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' existing = {
  name: serviceBusNamespaceName
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

// ── API → Service Bus: Send ──────────────────────────────────────────────────────
resource apiSbSender 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: serviceBusNamespace
  name: guid(serviceBusNamespace.id, apiPrincipalId, sbDataSenderRoleId)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', sbDataSenderRoleId)
    principalId:      apiPrincipalId
    principalType:    'ServicePrincipal'
  }
}

// ── Worker → Service Bus: Receive ────────────────────────────────────────────────
resource workerSbReceiver 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: serviceBusNamespace
  name: guid(serviceBusNamespace.id, workerPrincipalId, sbDataReceiverRoleId)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', sbDataReceiverRoleId)
    principalId:      workerPrincipalId
    principalType:    'ServicePrincipal'
  }
}

// ── API → Key Vault: read secrets ────────────────────────────────────────────────
resource kvSecretsUser 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: keyVault
  name: guid(keyVault.id, apiPrincipalId, kvSecretsUserRoleId)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', kvSecretsUserRoleId)
    principalId:      apiPrincipalId
    principalType:    'ServicePrincipal'
  }
}
