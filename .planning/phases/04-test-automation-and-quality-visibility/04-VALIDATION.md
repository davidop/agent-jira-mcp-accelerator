---
phase: 04
slug: test-automation-and-quality-visibility
status: draft
nyquist_compliant: false
wave_0_complete: false
created: 2026-05-26
---

# Phase 04 - Validation Strategy

> Per-phase validation contract for feedback sampling during execution.

---

## Test Infrastructure

| Property | Value |
|----------|-------|
| **Framework** | xUnit + Playwright (.NET) |
| **Config file** | none - Wave 1 creates test projects and deterministic fixtures |
| **Quick run command** | `dotnet test tests/Jira.Client.Tests/Jira.Client.Tests.csproj -c Release` |
| **Full suite command** | `dotnet test AgentJiraMCPAccelerator.slnx -c Release --logger "trx;LogFilePrefix=full" --collect:"XPlat Code Coverage"` |
| **Estimated runtime** | ~240 seconds |

---

## Sampling Rate

- **After every task commit:** Run `dotnet test tests/Jira.Client.Tests/Jira.Client.Tests.csproj -c Release`
- **After every plan wave:** Run `dotnet test AgentJiraMCPAccelerator.slnx -c Release --logger "trx;LogFilePrefix=wave" --collect:"XPlat Code Coverage"`
- **Before `/gsd-verify-work`:** Full suite must be green
- **Max feedback latency:** 300 seconds

---

## Per-Task Verification Map

| Task ID | Plan | Wave | Requirement | Threat Ref | Secure Behavior | Test Type | Automated Command | File Exists | Status |
|---------|------|------|-------------|------------|-----------------|-----------|-------------------|-------------|--------|
| 04-01-01 | 01 | 1 | REQ-04, REQ-06, REQ-07 | T-04-SC | Package legitimacy checkpoint completed before dependency install | checkpoint | `pwsh -NoProfile -Command "Select-String -Path '.planning/phases/04-test-automation-and-quality-visibility/04-RESEARCH.md' -Pattern 'xunit|Microsoft.NET.Test.Sdk|xunit.runner.visualstudio|coverlet.collector|Microsoft.AspNetCore.Mvc.Testing|Microsoft.Playwright.Xunit'"` | ✅ | ⬜ pending |
| 04-01-02 | 01 | 1 | REQ-04, REQ-06, REQ-07 | T-04-01/T-04-02/T-04-03 | Deterministic test suites enforce read-only and resilience behavior | unit+integration+e2e | `dotnet test tests/Jira.Client.Tests/Jira.Client.Tests.csproj -c Release --filter "FullyQualifiedName~Normalize|FullyQualifiedName~Retry|FullyQualifiedName~Paging"` | ❌ W0 | ⬜ pending |
| 04-02-01 | 02 | 2 | REQ-04, REQ-06, REQ-07 | T-04-05/T-04-06 | CI gates run xUnit + Mock-only smoke and publish artifacts | ci | `dotnet test AgentJiraMCPAccelerator.slnx -c Release --logger "trx;LogFilePrefix=ci" --collect:"XPlat Code Coverage"` | ❌ W0 | ⬜ pending |
| 04-02-02 | 02 | 2 | REQ-04, REQ-06, REQ-07 | T-04-04 | Mermaid quality docs remain synchronized via warning policy | docs | `rg "REQ-04|REQ-06|REQ-07" docs/testing/coverage-by-layer.mmd` | ❌ W0 | ⬜ pending |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠ flaky*

---

## Wave 0 Requirements

- [ ] `tests/Jira.Client.Tests/JiraCloudReaderResilienceTests.cs` - deterministic REQ-06/REQ-07 scenarios
- [ ] `tests/Agent.Api.Tests/AgentAskEndpointTests.cs` - REQ-04 prompt routing checks
- [ ] `tests/Jira.McpServer.Tests/DemoEndpointsTests.cs` - read-only contract checks
- [ ] `tests/Web.E2E/SmokeJourneysTests.cs` - 3 happy-path smoke journeys
- [ ] `pwsh tests/Web.E2E/bin/Release/net10.0/playwright.ps1 install --with-deps` - browser bootstrap in CI

---

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|----------|-------------|------------|-------------------|
| NuGet legitimacy validation for ASSUMED packages | REQ-04, REQ-06, REQ-07 | slopcheck unavailable in environment | Confirm package IDs/owners on nuget.org before install and record checkpoint approval in summary notes |

---

## Validation Sign-Off

- [ ] All tasks have `<automated>` verify or Wave 0 dependencies
- [ ] Sampling continuity: no 3 consecutive tasks without automated verify
- [ ] Wave 0 covers all MISSING references
- [ ] No watch-mode flags
- [ ] Feedback latency < 300s
- [ ] `nyquist_compliant: true` set in frontmatter

**Approval:** pending
