// The FlightDex API as an always-on serverless Container App. External ingress (the Static Web
// App calls it cross-origin), one fixed replica so it never scales to zero, managed-identity
// pulls + Key Vault secret, and an Azure SQL connection (the identity is the SQL admin, so EF
// migrations run on startup).
@description('Azure region for the container app.')
param location string

@description('Container app name.')
param name string

@description('Container Apps environment resource id.')
param environmentId string

@description('Resource id of the user-assigned managed identity.')
param identityId string

@description('ACR login server (e.g. acrxxxx.azurecr.io).')
param registryLoginServer string

@description('Versionless Key Vault secret URI for the JWT signing key.')
param jwtSecretUri string

@description('Allowed browser origin for CORS (the Static Web App URL).')
param corsOrigin string

@description('Azure SQL connection string (Entra managed-identity auth). Used for both modules.')
param sqlConnectionString string

@description('Container image to run. A placeholder until azd builds and pushes the real image.')
param image string

@description('Tags applied to the container app (must include azd-service-name).')
param tags object = {}

var targetPort = 8080

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: name
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identityId}': {}
    }
  }
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: targetPort
        transport: 'auto'
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      registries: [
        {
          server: registryLoginServer
          identity: identityId
        }
      ]
      secrets: [
        {
          name: 'jwt-key'
          keyVaultUrl: jwtSecretUri
          identity: identityId
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'api'
          image: image
          resources: {
            // Smallest Consumption sizing — economical, sufficient for this API.
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Production'
            }
            {
              name: 'Database__Provider'
              value: 'SqlServer'
            }
            {
              name: 'ConnectionStrings__FlightDex'
              value: sqlConnectionString
            }
            {
              name: 'ConnectionStrings__Booking'
              value: sqlConnectionString
            }
            {
              name: 'Cors__AngularOrigins__0'
              value: corsOrigin
            }
            {
              name: 'Jwt__Key'
              secretRef: 'jwt-key'
            }
          ]
        }
      ]
      scale: {
        // Always-on: never scale to zero. maxReplicas 2 allows a rolling (zero-downtime) revision
        // swap and headroom under load; at idle it stays at 1 replica, so no extra cost. Safe on
        // Azure SQL (multi-replica), unlike the earlier single-writer SQLite design.
        minReplicas: 1
        maxReplicas: 2
      }
    }
  }
}

output name string = containerApp.name
output fqdn string = containerApp.properties.configuration.ingress.fqdn
output uri string = 'https://${containerApp.properties.configuration.ingress.fqdn}'
