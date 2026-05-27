# Phase 4.1 plan

## Goal

Ensure application code from service projects is published as container images to Azure Container Registry during deployment.

## Scope

- Pipeline changes only.
- No Container Apps runtime deployment in this phase.

## Build tasks

1. Add .NET SDK setup step in deploy workflow.
2. Resolve ACR login server dynamically from the provisioned registry.
3. Build and publish container images for:
   - `src/Agent.Api/Agent.Api.csproj`
   - `src/Jira.McpServer/Jira.McpServer.csproj`
   - `src/Web/Web.csproj`
4. Tag images with commit SHA.
5. Verify repositories and tags exist in ACR after publish.

## Verification

- Deploy workflow completes successfully after provision.
- ACR contains repositories:
  - `agent-api`
  - `jira-mcp-server`
  - `web`
- Each repository contains at least one tag (current run SHA).
- Existing infra validation (App Insights, Key Vault, ACR, Storage) remains green.

## Out of scope

- Container Apps environment creation.
- Service release to Container Apps.
- Runtime smoke tests against public endpoints.
