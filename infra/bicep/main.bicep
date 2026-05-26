@description('Azure region')
param location string = resourceGroup().location

@description('Base name for resources')
param name string = 'ent-agent-accel'

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

// Add Container Apps Environment, apps, Azure AI Search and Foundry/OpenAI resources in Phase 3.

output applicationInsightsName string = appInsights.name
output keyVaultName string = keyVault.name
output storageAccountName string = storage.name
output containerRegistryName string = acr.name
