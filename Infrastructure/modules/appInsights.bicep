// ── appInsights.bicep ─────────────────────────────────────────────────────────
// Workspace-based Application Insights: a Log Analytics workspace plus the AI
// component that ingests into it. The connection string is handed to the API and
// worker as an app setting (APPLICATIONINSIGHTS_CONNECTION_STRING) — telemetry auth
// is the ingestion key embedded in that string, so no RBAC grant is needed.

@description('Application Insights component name.')
param name string

@description('Log Analytics workspace name backing the AI component.')
param workspaceName string

@description('Azure region.')
param location string

@description('Daily ingestion cap in GB (dev keeps costs predictable).')
param dailyQuotaGb int = 1

@description('Telemetry retention in days.')
param retentionInDays int = 30

param tags object = {}

resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: workspaceName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: retentionInDays
    workspaceCapping: {
      dailyQuotaGb: dailyQuotaGb
    }
  }
}

resource component 'Microsoft.Insights/components@2020-02-02' = {
  name: name
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type:    'web'
    WorkspaceResourceId: workspace.id
    IngestionMode:       'LogAnalytics'
  }
}

// ── Outputs ───────────────────────────────────────────────────────────────────

output connectionString string = component.properties.ConnectionString
output workspaceId      string = workspace.id
output componentName    string = component.name
