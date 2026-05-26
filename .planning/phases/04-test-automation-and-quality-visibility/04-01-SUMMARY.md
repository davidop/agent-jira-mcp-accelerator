---
phase: 04-test-automation-and-quality-visibility
plan: 01
subsystem: testing
tags: [xunit, aspnetcore-testing, playwright, deterministic-fixtures, jira-client]
requires:
  - phase: 02-jira-cloud-integration-and-hardening
    provides: JiraCloudReader paging, retry, and status normalization behavior used by resilience tests
provides:
  - Deterministic fixture-backed test suites for Jira.Client, Agent.Api, Jira.McpServer, and smoke journeys
  - Contract tests for ask endpoint and MCP demo read-only behavior
  - Bounded smoke coverage for blocked issues, sprint summary, and executive prompt flows
affects: [ci-testing, quality-visibility, verification]
tech-stack:
  added: [xunit, Microsoft.NET.Test.Sdk, xunit.runner.visualstudio, coverlet.collector, Microsoft.AspNetCore.Mvc.Testing, Microsoft.Playwright.Xunit]
  patterns: [fixture-driven deterministic testing, in-memory integration hosts via WebApplicationFactory, happy-path smoke boundaries]
key-files:
  created:
    - tests/Fixtures/jira-mock-stable.json
    - tests/Jira.Client.Tests/JiraCloudReaderResilienceTests.cs
    - tests/Agent.Api.Tests/AgentAskEndpointTests.cs
    - tests/Jira.McpServer.Tests/DemoEndpointsTests.cs
    - tests/Web.E2E/SmokeJourneysTests.cs
  modified:
    - src/Agent.Api/Program.cs
    - src/Jira.McpServer/Program.cs
key-decisions:
  - "Use a shared immutable fixture file to keep all test layers deterministic in mock mode."
  - "Use WebApplicationFactory-based in-memory integration tests for API and MCP contract validation."
  - "Bound smoke scope to exactly three happy-path journeys aligned to D-01 and REQ-04."
patterns-established:
  - "Deterministic fixture seam: tests/Fixtures/jira-mock-stable.json is injected via Jira:MockDataPath."
  - "Read-only contract checks include negative write-attempt assertions on demo routes."
requirements-completed: [REQ-04, REQ-06, REQ-07]
duration: 25min
completed: 2026-05-26
---

# Phase 04 Plan 01: Test Automation and Quality Visibility Summary

**Deterministic xUnit and smoke validation now cover blocked issues, sprint summary, executive prompt flows, and Jira.Client retry/paging/status normalization in mock mode.**

## Performance

- **Duration:** 25 min
- **Started:** 2026-05-26T18:08:00+02:00
- **Completed:** 2026-05-26T18:33:00+02:00
- **Tasks:** 1/1
- **Files modified:** 11

## Accomplishments
- Added four dedicated test projects under tests/ with deterministic fixture-driven data.
- Implemented Jira.Client resilience and mapping tests for Normalize, Paging boundaries, and Retry-After handling.
- Implemented integration coverage for Agent.Api ask endpoint behavior and Jira.McpServer read-only demo contracts.
- Added exactly three smoke journey tests for blocked issues, sprint summary, and executive report happy paths.

## Task Commits

Each task was committed atomically:

1. **Task 2: Scaffold xUnit and Playwright test projects with deterministic fixture seam** - `d94969f` (test)
2. **Task 2 support seam: in-memory app host visibility** - `78fc982` (feat)

**Plan metadata:** pending

## Files Created/Modified
- `tests/Fixtures/jira-mock-stable.json` - Shared deterministic fixture corpus for all test layers.
- `tests/Jira.Client.Tests/Jira.Client.Tests.csproj` - Jira.Client test project package and project references.
- `tests/Jira.Client.Tests/JiraCloudReaderResilienceTests.cs` - Normalize, paging, and retry resilience matrix tests.
- `tests/Agent.Api.Tests/Agent.Api.Tests.csproj` - Agent.Api integration test project dependencies.
- `tests/Agent.Api.Tests/AgentAskEndpointTests.cs` - Ask endpoint prompt-family and schema contract coverage.
- `tests/Jira.McpServer.Tests/Jira.McpServer.Tests.csproj` - MCP server integration test project dependencies.
- `tests/Jira.McpServer.Tests/DemoEndpointsTests.cs` - Demo endpoint contract and read-only behavior tests.
- `tests/Web.E2E/Web.E2E.csproj` - Smoke project dependencies including Playwright test package.
- `tests/Web.E2E/SmokeJourneysTests.cs` - Three bounded smoke journey tests for critical happy paths.
- `src/Agent.Api/Program.cs` - Added partial Program marker for in-memory host bootstrapping in tests.
- `src/Jira.McpServer/Program.cs` - Added partial Program marker for in-memory host bootstrapping in tests.

## Decisions Made
- Shared deterministic fixture injection through `Jira:MockDataPath` in all integration/smoke factories.
- Smoke scope intentionally constrained to three happy-path journeys (no negative scenarios in this phase).
- No fixed coverage-percentage quality gates were introduced in this phase.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Test scaffold compile failures from missing xUnit imports and fixture typing**
- **Found during:** Task 2 verification (Jira.Client and Agent.Api test runs)
- **Issue:** New test files failed to compile due missing `using Xunit;`, missing `IWebHostBuilder` imports, and one unresolved nested fixture type reference.
- **Fix:** Added required imports and corrected fixture generic reference to nested type.
- **Files modified:** `tests/Jira.Client.Tests/JiraCloudReaderResilienceTests.cs`, `tests/Agent.Api.Tests/AgentAskEndpointTests.cs`, `tests/Jira.McpServer.Tests/DemoEndpointsTests.cs`, `tests/Web.E2E/SmokeJourneysTests.cs`
- **Verification:** All required `dotnet test` commands passed.
- **Committed in:** `d94969f`

**2. [Rule 3 - Blocking] Jira client test harness compile issues in disposable usage and delegate inference**
- **Found during:** Task 2 verification (filtered Jira.Client test run)
- **Issue:** `using` declarations targeted non-disposable type and delegate inference failed for fake response factory.
- **Fix:** Removed invalid `using` declarations for reader instances and made delegate factory typing explicit.
- **Files modified:** `tests/Jira.Client.Tests/JiraCloudReaderResilienceTests.cs`
- **Verification:** Filtered Normalize/Retry/Paging test command passed with 3/3 tests.
- **Committed in:** `d94969f`

---

**Total deviations:** 2 auto-fixed (2 blocking)
**Impact on plan:** Both fixes were required to complete planned verification; no scope expansion.

## Issues Encountered
- None beyond expected scaffold-to-green compile iteration during TDD-style test setup.

## Auth Gates
- Task 1 package legitimacy checkpoint approved by human before package references were added.

## Known Stubs
- None.

## User Setup Required
- None - no external service configuration required.

## Next Phase Readiness
- Phase now has deterministic quality gates for REQ-04, REQ-06, and REQ-07 in mock mode.
- Test suites are ready for CI invocation and future expansion without cloud credentials.

## Self-Check
- Pending

---
*Phase: 04-test-automation-and-quality-visibility*
*Completed: 2026-05-26*
