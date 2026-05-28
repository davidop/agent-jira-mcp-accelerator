# Milestone v1.0 - Project Summary

**Generated:** 2026-05-28
**Purpose:** Team onboarding and project review

---

## 1. Project Overview

This project delivers an enterprise-oriented accelerator that turns Jira delivery data into actionable agent tooling via MCP and .NET services, with a path from local deterministic demo mode to Azure-hosted runtime.

For v1.0, the milestone focused on making the cloud read path reliable, adding deterministic quality gates, and hardening release artifacts so the solution is handoff-ready for presales and engineering teams.

## 2. Architecture and Technical Decisions

- **Decision:** Preserve dual data modes (`Mock` and Jira Cloud) behind `Jira.Client` abstraction.
  - **Why:** Enables deterministic demos while supporting real Jira Cloud reads without interface churn.
  - **Phase:** 2

- **Decision:** Keep Jira and MCP behavior read-only for release scope.
  - **Why:** Reduces risk and governance complexity for first release and enterprise demos.
  - **Phase:** 2, 3

- **Decision:** Adopt deterministic test strategy using shared fixture plus in-memory integration hosts.
  - **Why:** Stable CI outcomes and reproducible validation for core journeys.
  - **Phase:** 4 (04-01)

- **Decision:** Enforce smoke gate with retry and fail-fast merge readiness policy.
  - **Why:** Ensure critical demo journeys stay healthy without introducing noisy hard coverage thresholds.
  - **Phase:** 4 (04-02)

- **Decision:** Publish .NET SDK containers directly to ACR in deploy pipeline.
  - **Why:** Avoid Dockerfile overhead for this phase while producing versioned deployable artifacts.
  - **Phase:** 4.1

- **Decision:** Deploy runtime with Bicep-controlled Container Apps and URL verification gate.
  - **Why:** Provide concrete hosted endpoints and deterministic runtime release checks.
  - **Phase:** 4.2

## 3. Phases Delivered

| Phase | Name | Status | Outcome |
|-------|------|--------|---------|
| 2 | Jira Cloud integration | Complete | Cloud read path hardened with pagination, retry/backoff, and status normalization. |
| 3 | Release hardening | Complete | Release docs, verification hardening, and sign-off artifacts finalized. |
| 4 | Test automation and quality visibility | Complete | xUnit + Playwright CI gates and Mermaid quality visibility artifacts delivered. |
| 4.1 | App image publication to ACR | Complete | Deploy workflow publishes and validates ACR tags for all service images. |
| 4.2 | Container Apps runtime deployment and URLs | Complete | Runtime deployment and endpoint URL verification added to Azure deploy flow. |

## 4. Requirements Coverage

- ✅ REQ-01 to REQ-08 roadmap intent covered by delivered phases and verified artifacts.
- ✅ REQ-04, REQ-06, REQ-07 enforced by CI gates, smoke validation, and testing visibility assets.
- ✅ REQ-08 covered by release hardening artifacts (`release-checklist`, `release-notes`, updated verification baseline).
- ⚠️ Governance expansion items (advanced RBAC/write approvals/private networking depth) are explicitly deferred beyond v1.0.

## 5. Key Decisions Log

- Cloud mode remains read-only and deterministic for demo safety.
- CI quality model prioritizes critical-path confidence over fixed coverage percentage gates.
- Mermaid freshness is warning-level (non-blocking) in this milestone.
- Azure deploy flow chains provision -> publish images -> runtime deploy -> URL verification.
- Release handoff requires explicit checklist and notes artifacts under `docs/gsd`.

## 6. Tech Debt and Deferred Items

- Advanced enterprise governance controls are deferred to subsequent milestones.
- Multi-system MCP expansion (Azure DevOps, GitHub, ServiceNow, Confluence/SharePoint) remains backlog scope.
- Some runtime checks depend on external Azure/GitHub execution confirmation rather than local-only validation.

## 7. Getting Started

- **Run locally (manual):**
  - `dotnet restore`
  - `dotnet run --project src/Jira.McpServer/Jira.McpServer.csproj`
  - `dotnet run --project src/Agent.Api/Agent.Api.csproj`
  - `dotnet run --project src/Web/Web.csproj`

- **Run with Aspire:**
  - `dotnet run --project src/AppHost/AppHost.csproj`

- **Key workflows:**
  - CI: `.github/workflows/ci.yml`
  - Deploy: `.github/workflows/deploy.yml`

- **Primary docs:**
  - `README.md`
  - `docs/demo-script.md`
  - `docs/gsd/05-verification-checklist.md`
  - `docs/gsd/06-release-checklist.md`
  - `docs/gsd/07-release-notes-v1.0.md`

---

## Stats

- **Timeline:** 2026-05-26 -> 2026-05-28
- **Phases:** 5 / 5 complete
- **Commits:** 18
- **Files changed:** 56 (+244 / -166)
- **Contributors:** David Oliva Paredes
