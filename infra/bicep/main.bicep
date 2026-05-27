@description('Azure region')
param location string = resourceGroup().location

@description('Base name for resources')
param name string = 'ent-agent-accel'

@description('Deploy Container Apps runtime resources')
param deployApps bool = false

@description('Container image tag used by Container Apps when deployApps is true')
param imageTag string = 'latest'

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${name}-appi'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: '${name}-kv'
  location: location
  properties: {
    tenantId: tenant().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableRbacAuthorization: true
  }
}

resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: replace('${name}st', '-', '')
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: replace('${name}acr', '-', '')
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
  }
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: '${name}-law'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

resource containerAppsIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${name}-ca-id'
  location: location
}

var acrPullRoleDefinitionId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')

resource acrPullAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = if (deployApps) {
  name: guid(acr.id, containerAppsIdentity.id, 'acrpull')
  scope: acr
  properties: {
    principalId: containerAppsIdentity.properties.principalId!
    principalType: 'ServicePrincipal'
    roleDefinitionId: acrPullRoleDefinitionId
  }
}

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = if (deployApps) {
  name: '${name}-cae'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId!
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

resource agentApiApp 'Microsoft.App/containerApps@2024-03-01' = if (deployApps) {
  name: '${name}-api'
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${containerAppsIdentity.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironment.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
      }
      registries: [
        {
          server: acr.properties.loginServer
          identity: containerAppsIdentity.id
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'agent-api'
          image: '${acr.properties.loginServer}/agent-api:${imageTag}'
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 2
      }
    }
  }
  dependsOn: [
    acrPullAssignment
  ]
}

resource jiraMcpServerApp 'Microsoft.App/containerApps@2024-03-01' = if (deployApps) {
  name: '${name}-mcp'
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${containerAppsIdentity.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironment.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
      }
      registries: [
        {
          server: acr.properties.loginServer
          identity: containerAppsIdentity.id
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'jira-mcp-server'
          image: '${acr.properties.loginServer}/jira-mcp-server:${imageTag}'
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 2
      }
    }
  }
  dependsOn: [
    acrPullAssignment
  ]
}

resource webApp 'Microsoft.App/containerApps@2024-03-01' = if (deployApps) {
  name: '${name}-web'
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${containerAppsIdentity.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironment.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
      }
      registries: [
        {
          server: acr.properties.loginServer
          identity: containerAppsIdentity.id
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'web'
          image: '${acr.properties.loginServer}/web:${imageTag}'
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 2
      }
    }
  }
  dependsOn: [
    acrPullAssignment
  ]
}

output applicationInsightsName string = appInsights.name
output keyVaultName string = keyVault.name
output storageAccountName string = storage.name
output containerRegistryName string = acr.name
output agentApiUrl string = deployApps ? 'https://${agentApiApp!.properties.configuration.ingress.fqdn}' : ''
output jiraMcpServerUrl string = deployApps ? 'https://${jiraMcpServerApp!.properties.configuration.ingress.fqdn}' : ''
output webUrl string = deployApps ? 'https://${webApp!.properties.configuration.ingress.fqdn}' : ''
