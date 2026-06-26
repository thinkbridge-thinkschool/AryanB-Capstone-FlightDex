// Reads the image currently running on the API container app, if it already exists. This lets a
// re-provision keep the image that `azd deploy` pushed, instead of reverting to the placeholder.
@description('Name of the container app to inspect.')
param name string

@description('Whether the container app already exists.')
param exists bool

resource existingApp 'Microsoft.App/containerApps@2024-03-01' existing = if (exists) {
  name: name
}

output containers array = exists ? existingApp.properties.template.containers : []
