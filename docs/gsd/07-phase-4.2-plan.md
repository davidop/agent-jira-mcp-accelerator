# Phase 4.2 plan

## Goal

Deploy runnable Azure Container Apps for the three service projects and expose concrete URLs for validation.

## Scope

- Extend Bicep infra to include Container Apps runtime resources.
- Update deploy workflow to create runtime resources after image publish.

## Build tasks

1. Add Log Analytics Workspace for Container Apps diagnostics.
2. Add Container Apps Environment.
3. Add a user-assigned managed identity and grant AcrPull on ACR.
4. Add three Container Apps:
   - `ent-agent-accel-api`
   - `ent-agent-accel-mcp`
   - `ent-agent-accel-web`
5. Parameterize runtime image tag.
6. Update deploy workflow to run what-if + deployment for runtime stage.
7. Add workflow verification for Container App FQDN URLs.

## Verification

- `az deployment group what-if` succeeds for runtime stage.
- `az deployment group create` succeeds for runtime stage.
- `az containerapp show` returns ingress FQDN for all three apps.
- URLs are printed in workflow logs.

## Out of scope

- Private ingress/networking.
- Custom domains and certificates.
- WAF/API gateway fronting.
