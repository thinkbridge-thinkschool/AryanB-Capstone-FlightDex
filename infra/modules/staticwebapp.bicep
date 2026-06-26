// Azure Static Web App (Free SKU, $0) that serves the Angular UI from a global CDN. azd builds
// the Angular app and uploads the static bundle here; it calls the API cross-origin (CORS).
@description('Azure region for the Static Web App (SWA-supported region).')
param location string

@description('Static Web App name.')
param name string

@description('Tags applied to the Static Web App (must include azd-service-name).')
param tags object = {}

resource staticSite 'Microsoft.Web/staticSites@2023-12-01' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    // azd pushes the prebuilt bundle via the deployment token; no repo-linked CI build.
    allowConfigFileUpdates: true
  }
}

output name string = staticSite.name
output uri string = 'https://${staticSite.properties.defaultHostname}'
output defaultHostname string = staticSite.properties.defaultHostname
