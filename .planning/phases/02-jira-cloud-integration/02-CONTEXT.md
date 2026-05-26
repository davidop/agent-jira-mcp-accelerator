# Phase 2: Jira Cloud Integration - Context

**Gathered:** 2026-05-26
**Status:** Ready for planning
**Source:** Roadmap, requirements, architecture, demo script, and the shipped phase 1 implementation

<domain>
## Phase Boundary

This phase replaces the mock Jira data source with live Jira Cloud read access while preserving the demo-first experience, the Aspire local composition model, and the read-only operating stance.

The phase must keep the current user flows working for blocked issues, sprint summaries, assignee workload, and executive narration, but the answers should come from Jira Cloud instead of local mock data.

</domain>

<decisions>
## Implementation Decisions

### Cloud read path
- Jira Cloud is accessed through the existing `Jira.Client` abstraction.
- Phase 2 keeps the current `IJiraReader` contract and adds the cloud-hardening needed around it.

### Demo authentication
- Basic Auth with Jira email and API token is the supported demo path.
- Credentials must stay in configuration only and continue to be supplied through user-secrets or environment variables.

### Enterprise path
- OAuth 2.0 is the enterprise extension point.
- The phase should preserve a clear seam for a later Atlassian OAuth flow without forcing the demo path to depend on it.

### Query behavior
- The phase must standardize JQL templates for blocked issues, sprint summaries, assignee workloads, and epic or project drilldowns.
- Search behavior must remain deterministic and explainable to the user.

### Result shaping
- Jira Cloud responses must map into the same internal models used by the demo today.
- Status mapping must be improved so the executive summaries can distinguish done, in progress, blocked, and risk states consistently.

### Resilience
- Pagination, rate-limit handling, timeout handling, and retry behavior must be part of the cloud reader.
- The API should keep returning useful fallback messages if Jira Cloud is slow or partially unavailable.

### Observability
- Jira Cloud calls should emit structured logs and preserve enough detail to diagnose auth, search, pagination, and rate-limit failures.

### the agent's Discretion
- The exact internal shape of pagination helpers, retry policy placement, and status normalization tables is left to implementation, as long as the read-only user flows stay intact.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Product and roadmap
- `docs/gsd/02-requirements.md` — functional and non-functional constraints for the accelerator.
- `docs/gsd/03-roadmap.md` — phase 2 scope and enterprise trajectory.
- `docs/gsd/05-verification-checklist.md` — local, security, and demo acceptance checks.

### Current shipped behavior
- `README.md` — demo positioning, quick start, and current mock-to-cloud configuration guidance.
- `docs/architecture.md` — runtime view and the enterprise security design.
- `docs/demo-script.md` — the 10-minute business narrative that must keep working.
- `docs/gsd/04-phase-1-plan.md` — the local MVP plan that phase 2 builds on.

### Existing implementation seams
- `src/Jira.Client/JiraOptions.cs` — current Jira configuration surface.
- `src/Jira.Client/ServiceCollectionExtensions.cs` — current cloud/mock registration seam.
- `src/Jira.Client/JiraCloudReader.cs` — current cloud reader baseline.
- `src/Agent.Api/Services/DemoAgentService.cs` — current user-facing responses that must remain stable.

</canonical_refs>

<specifics>
## Specific Ideas

- Keep `Jira:Mode=Mock|Cloud` as the switch, but make Cloud mode production-safe.
- Prefer direct JQL templates for the current demo questions rather than a generic query builder.
- Preserve the current Spanish demo narrative and output style.
- Extend the current cloud reader instead of introducing a second Jira client layer.

</specifics>

<deferred>
## Deferred Ideas

- Write actions in Jira.
- Approval gates for tool execution.
- Atlassian OAuth production rollout.
- Cross-system reasoning with Azure DevOps, GitHub, ServiceNow, Confluence, or SharePoint.
- Azure AI / Foundry orchestration.

</deferred>

---

*Phase: 02-jira-cloud-integration*
*Context gathered: 2026-05-26 via roadmap, requirements, architecture, demo script, and phase 1 implementation*
