# Phase 4: Test automation and quality visibility - Context

**Gathered:** 2026-05-26
**Status:** Ready for planning

<domain>
## Phase Boundary

This phase establishes automated quality gates for the existing Jira Cloud demo behavior by adding Playwright end-to-end tests, xUnit tests for core backend seams, and Mermaid documentation for coverage and execution flow visibility.

The phase must validate already-shipped functionality without expanding product scope.

</domain>

<decisions>
## Implementation Decisions

### E2E scope and quality gate
- **D-01:** Playwright smoke coverage must include 3 critical journeys: blocked issues, sprint summary, and executive report flow.
- **D-02:** Phase 4 E2E scope is happy-path focused; negative-path scenarios are deferred.
- **D-03:** E2E data must use deterministic Mock fixtures for stable PR validation.
- **D-04:** E2E gate for merge requires 100% passing smoke tests (no quarantine baseline in this phase).

### Playwright execution topology
- **D-05:** Playwright runs in PR CI against Mock mode only.
- **D-06:** No nightly Jira Cloud E2E run in this phase.
- **D-07:** Baseline anti-flaky strategy is moderate timeouts plus 1 retry.

### xUnit strategy by layer
- **D-08:** xUnit priority is Jira.Client and DemoAgentService coverage first.
- **D-09:** Agent.Api and Jira.McpServer should include in-memory integration tests for wiring and endpoint behavior.
- **D-10:** Jira.Client resilience behavior (retry, paging, mapping) should be tested deterministically using fake HttpMessageHandler rather than live network.
- **D-11:** No fixed line-percentage target for this phase; coverage is measured by critical-path confidence.

### Mermaid quality visibility
- **D-12:** Mermaid artifacts live under `docs/testing/`.
- **D-13:** Required Mermaid outputs in this phase: (1) coverage-by-layer diagram and (2) CI execution flow diagram.
- **D-14:** Mermaid updates are enforced by PR checklist and reviewer checks, not auto-generation.
- **D-15:** Missing Mermaid updates produce a warning signal, not a hard merge block, in this phase.

### Carry-forward decisions from prior phases
- **D-16:** Preserve read-only behavior in MCP and API surfaces while adding tests.
- **D-17:** Preserve deterministic and explainable behavior for core demo prompts.

### the agent's Discretion
- Test project folder naming and exact test file granularity.
- Fixture composition details as long as deterministic behavior is preserved.
- Assertion style and helper abstraction choices for readability and maintainability.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Milestone and phase control
- `.planning/PROJECT.md` - milestone objective and success criteria.
- `.planning/REQUIREMENTS.md` - release functional and non-functional constraints.
- `.planning/ROADMAP.md` - phase boundary, goal, and success criteria for phase 4.
- `.planning/STATE.md` - active milestone status and execution context.

### Prior phase implementation intent
- `.planning/phases/02-jira-cloud-integration/02-CONTEXT.md` - locked decisions from cloud integration.
- `.planning/phases/02-jira-cloud-integration/02-PLAN.md` - implementation scope and verification approach from phase 2.
- `.planning/phases/02-jira-cloud-integration/02-SUMMARY.md` - shipped behavior and validated outcomes from phase 2.

### Product and architecture references
- `README.md` - quick-start flows and user-facing demo expectations.
- `docs/architecture.md` - system topology and service responsibilities.
- `docs/demo-script.md` - critical demo narrative and expected user journeys.
- `docs/gsd/05-verification-checklist.md` - verification baseline for release quality.

### Runtime and integration points for tests
- `.github/workflows/ci.yml` - current CI baseline (restore/build only).
- `src/AppHost/Program.cs` - local distributed composition entry point.
- `src/Web/Program.cs` - web runtime setup and Agent.Api client path.
- `src/Agent.Api/Program.cs` - API host and ask endpoint wiring.
- `src/Agent.Api/Services/DemoAgentService.cs` - deterministic prompt routing and response shaping.
- `src/Jira.McpServer/Program.cs` - MCP host and demo HTTP endpoints.
- `src/Jira.McpServer/Tools/JiraTools.cs` - MCP tool surface to validate.
- `src/Jira.Client/ServiceCollectionExtensions.cs` - mode selection and cloud/mock DI seam.
- `src/Jira.Client/JiraCloudReader.cs` - cloud query, retry, and mapping logic.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- `IJiraReader` abstraction already centralizes test seams across Agent.Api and Jira.McpServer.
- `DemoAgentService` has deterministic branching by question keyword, suitable for scenario-driven tests.
- `JiraCloudReader` already isolates transport and mapping logic, suitable for fake handler-based tests.

### Established Patterns
- Service wiring is done through minimal-hosting Program.cs files with explicit DI registration.
- Solution currently emphasizes deterministic local execution (Mock mode) for demos.
- CI currently performs restore/build only, so test stages can be added cleanly as new steps.

### Integration Points
- Add xUnit projects that reference `Jira.Client`, `Agent.Api`, and `Jira.McpServer` integration seams.
- Add Playwright project aligned to Web -> Agent.Api path in local runtime.
- Extend `.github/workflows/ci.yml` with test jobs and artifact publishing.
- Add `docs/testing/` for Mermaid diagrams and test evidence docs.

</code_context>

<specifics>
## Specific Ideas

- Keep phase 4 focused on confidence for currently demonstrated journeys, not new product features.
- Use Mock-mode fixtures as source-of-truth for PR reliability.
- Keep quality communication visible with two Mermaid diagrams that map to CI behavior and layer coverage.

</specifics>

<deferred>
## Deferred Ideas

- Add E2E negative-path suites for API error and timeout behavior.
- Add nightly Cloud-sandbox E2E validation.
- Promote Mermaid freshness checks from warning to blocking gate once test process stabilizes.

</deferred>

---

*Phase: 04-test-automation-and-quality-visibility*
*Context gathered: 2026-05-26*
