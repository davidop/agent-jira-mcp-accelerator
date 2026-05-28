# Release Notes v1.0

## Summary

v1.0 delivers a production-minded Jira MCP accelerator with Cloud read-only support, deterministic quality gates, and Azure deployment/runtime flow for demo and presales scenarios.

## Delivered in v1.0

- Jira Cloud integration with resilient read path (pagination, retry, status normalization).
- Mock mode preserved for deterministic local/demo execution.
- xUnit coverage across Jira.Client, Agent.Api, and Jira.McpServer.
- Playwright smoke coverage for core demo journeys.
- CI pipeline with smoke gate enforcement and diagnostics artifact upload.
- Testing quality visibility documentation under `docs/testing`.
- Azure deploy workflow with:
  - Infra provisioning
  - Container image publication to ACR
  - Runtime deployment stages for Container Apps
  - URL verification step for API, MCP, and Web

## Operational Constraints

- Jira write actions are out of scope for v1.0.
- Governance hardening (advanced RBAC, write approvals, private networking) remains for subsequent phases.
- Mermaid freshness checks are warning-only in this phase.

## Known Next Steps

- Phase 3 verification closure and release sign-off checklist completion.
- Governance enterprise backlog: Entra ID deep integration, approval workflow, private networking, and multi-system expansion.

## References

- `README.md`
- `docs/demo-script.md`
- `docs/gsd/05-verification-checklist.md`
- `docs/gsd/06-release-checklist.md`
- `.planning/ROADMAP.md`
