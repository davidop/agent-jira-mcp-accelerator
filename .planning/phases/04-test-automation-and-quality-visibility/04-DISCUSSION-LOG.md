# Phase 4: Test automation and quality visibility - Discussion Log

> **Audit trail only.** Do not use as input to planning, research, or execution agents.
> Decisions are captured in CONTEXT.md - this log preserves the alternatives considered.

**Date:** 2026-05-26
**Phase:** 04-test-automation-and-quality-visibility
**Areas discussed:** Prioridad de flujos E2E, Topologia de ejecucion Playwright, Estrategia xUnit por capa, Mermaid y evidencia de calidad

---

## Prioridad de flujos E2E

| Option | Description | Selected |
|--------|-------------|----------|
| 3 flujos criticos | Bloqueadas + Sprint summary + Informe ejecutivo | ✓ |
| 5 flujos completos | Add workload by assignee + epic/project drilldown | |
| Solo 1 flujo | Minimum baseline | |

**User's choice:** 3 flujos criticos
**Notes:** Chose happy-path-only for phase 4 and deterministic mock fixture strategy.

| Option | Description | Selected |
|--------|-------------|----------|
| Si, 2 negativos por flujo critico | API timeout/error and empty responses | |
| Solo casos felices | Negative paths deferred | ✓ |
| Solo un flujo con negativos | Pilot resiliency coverage | |

**User's choice:** Solo casos felices
**Notes:** Negative-path E2E coverage deferred.

| Option | Description | Selected |
|--------|-------------|----------|
| Fixtures deterministas en Mock | Stable PR signal | ✓ |
| Sandbox Cloud real | More realism, more flakiness | |
| Mixto | PR in Mock, nightly in Cloud | |

**User's choice:** Fixtures deterministas en Mock
**Notes:** Deterministic data preferred for merge confidence.

| Option | Description | Selected |
|--------|-------------|----------|
| 100% smoke verde | No quarantined tests baseline | ✓ |
| Permitir cuarentena temporal | Allow temporary flaky bucket | |
| Solo advisory | Non-blocking signal | |

**User's choice:** 100% smoke verde
**Notes:** Strong merge gate required for smoke suite.

---

## Topologia de ejecucion Playwright

| Option | Description | Selected |
|--------|-------------|----------|
| Solo Mock en PR | Fast and stable PR validation | ✓ |
| Cloud sandbox en PR | Realistic but brittle | |
| Ambos en PR | Maximum coverage, slower pipeline | |

**User's choice:** Solo Mock en PR
**Notes:** Prefer stable PR cycle.

| Option | Description | Selected |
|--------|-------------|----------|
| Si, nightly Cloud | Detect drift without blocking PR | |
| No por ahora | Keep scope tight for phase 4 | ✓ |

**User's choice:** No por ahora
**Notes:** Nightly cloud run deferred.

| Option | Description | Selected |
|--------|-------------|----------|
| GitHub Environments + secrets | Structured secret governance | |
| Repository secrets simples | Lower setup, lower control | |
| Sin Cloud tests | No cloud secret path required | ✓ |

**User's choice:** Sin Cloud tests
**Notes:** Consistent with no nightly cloud run in this phase.

| Option | Description | Selected |
|--------|-------------|----------|
| Timeout moderado + 1 retry | Balance signal and stability | ✓ |
| Sin retry | Fail fast, more noise | |
| 2+ retries | More tolerance, less regression signal | |

**User's choice:** Timeout moderado + 1 retry
**Notes:** Baseline anti-flaky strategy accepted.

---

## Estrategia xUnit por capa

| Option | Description | Selected |
|--------|-------------|----------|
| Jira.Client + DemoAgentService primero | Highest business impact | ✓ |
| MCP tools primero | Tool contract first | |
| Balanceado en las 3 capas | Spread effort thinly | |

**User's choice:** Jira.Client + DemoAgentService primero
**Notes:** Prioritize critical backend behavior.

| Option | Description | Selected |
|--------|-------------|----------|
| Si, integracion in-memory | Validate real DI and endpoint wiring | ✓ |
| No, solo unit tests | Fast but less integration confidence | |
| Mixto | A few in-memory + many unit tests | |

**User's choice:** Si, integracion in-memory
**Notes:** Integration confidence required for Agent.Api and MCP.

| Option | Description | Selected |
|--------|-------------|----------|
| Fake HttpMessageHandler | Deterministic resilience tests | ✓ |
| Sandbox real | Real network behavior | |
| No cubrir resiliencia ahora | Defer resilience tests | |

**User's choice:** Tests deterministas con HttpMessageHandler fake
**Notes:** Network-free deterministic resilience testing.

| Option | Description | Selected |
|--------|-------------|----------|
| Sin % fijo, foco en rutas criticas | Outcome-focused quality | ✓ |
| 70% lineas | Numeric gate | |
| 80% lineas + branch | Strict numeric gate | |

**User's choice:** Sin % fijo, foco en rutas criticas
**Notes:** Avoid vanity metrics in phase 4.

---

## Mermaid y evidencia de calidad

| Option | Description | Selected |
|--------|-------------|----------|
| docs/testing/ | Dedicated testing documentation area | ✓ |
| docs/gsd/ | Keep under planning docs | |
| .planning/phases/04-.../ | Phase-local only | |

**User's choice:** docs/testing/
**Notes:** Team-visible docs location selected.

| Option | Description | Selected |
|--------|-------------|----------|
| Cobertura por capa + flujo CI | Two required diagrams | ✓ |
| Solo flujo CI | Minimal diagram set | |
| 3+ diagramas detallados | Expanded artifact scope | |

**User's choice:** Cobertura por capa + flujo CI
**Notes:** Two-diagram baseline accepted.

| Option | Description | Selected |
|--------|-------------|----------|
| Checklist en PR + actualizacion manual | Practical baseline process | ✓ |
| Generacion automatica en CI | Automated synchronization | |
| No gatearlos | Informal docs only | |

**User's choice:** Checklist en PR + actualizacion manual
**Notes:** Manual update model for initial phase.

| Option | Description | Selected |
|--------|-------------|----------|
| No bloquea, warning | Gradual enforcement | ✓ |
| Si, bloquea merge | Strict enforcement from day one | |
| Ignorar por ahora | No enforcement | |

**User's choice:** No bloquea, warning
**Notes:** Warning-level policy selected for Mermaid freshness.

---

## the agent's Discretion

- Test project naming and helper structure.
- Exact fixture file layout.
- Assertion helper patterns.

## Deferred Ideas

- E2E negative-path suites.
- Nightly cloud-sandbox Playwright runs.
- Hard merge blocking for Mermaid freshness checks.
