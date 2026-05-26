# Release v1.0 Roadmap

## Phases

- [x] **Phase 2: Jira Cloud integration** - Move demo data reads from mock to Jira Cloud with resilient, read-only behavior.
- [ ] **Phase 3: Release hardening** - Finalize docs, verification, and release readiness for v1.0.
- [ ] **Phase 4: Test automation and quality visibility** - Add automated Playwright + xUnit coverage and Mermaid quality artifacts.

## Phase Details

### Phase 2: Jira Cloud integration
**Goal**: Replace mock-backed demo flows with resilient Jira Cloud read access while preserving read-only behavior.
**Depends on**: Phase 1
**Requirements**: REQ-01, REQ-02, REQ-03, REQ-04, REQ-05, REQ-06, REQ-07, REQ-08
**Success Criteria** (what must be TRUE):
	1. Core demo prompts return Jira Cloud-backed answers.
	2. Cloud pagination/retries/status normalization behave deterministically.
	3. MCP surface remains read-only.
**Plans**: 1 plan

Plans:
- [x] 02-01: Implement Jira Cloud integration and hardening in client and demo flow.

### Phase 3: Release hardening
**Goal**: Align docs, verification flow, and release packaging for v1.0 handoff.
**Depends on**: Phase 2
**Requirements**: REQ-04, REQ-08
**Success Criteria** (what must be TRUE):
	1. Demo guidance and README reflect shipped cloud behavior.
	2. Verification checklist is up to date and executable locally.
	3. Release checklist and notes are complete.
**Plans**: TBD

Plans:
- [ ] 03-01: Update release docs and verification scripts.

### Phase 4: Test automation and quality visibility
**Goal**: Establish automated test gates for UI and backend with traceable quality documentation.
**Depends on**: Phase 3
**Requirements**: REQ-04, REQ-06, REQ-07
**Success Criteria** (what must be TRUE):
	1. Playwright E2E tests validate the core demo journeys.
	2. xUnit tests validate API, MCP tools, and Jira client seams.
	3. Mermaid diagrams document coverage scope and test execution flows.
	4. CI runs and reports these tests deterministically.
**Plans**: 2 plans

Plans:
- [x] 04-01: Add Playwright and xUnit test automation baseline with CI integration.
- [ ] 04-02: Enforce CI test gates and publish quality visibility artifacts/documentation.

## Phase 2 — Jira Cloud integration

- Harden Jira Cloud reads in `Jira.Client`.
- Keep Basic Auth for demos and preserve an OAuth seam for enterprise.
- Add deterministic JQL templates for the core demo prompts.
- Improve status mapping, pagination, and rate-limit handling.
- Keep the MCP surface read-only and stable.

## Phase 3 — Release hardening

- Update documentation and quick start guidance for Cloud mode.
- Verify the demo prompts against Jira Cloud.
- Confirm the release stays aligned with Aspire local composition.
- Capture release notes and shipping checklist.

## Phase 4 — Test automation and quality visibility

- Add end-to-end UI coverage with Playwright for core demo flows.
- Add xUnit integration tests for API, MCP tools, and Jira client seams.
- Generate Mermaid diagrams to document test coverage and execution flows.
- Wire test execution into local and CI verification steps.
- Publish deterministic test artifacts for release validation.

## Release Exit Criteria

- Phase 2 is implemented and validated.
- The main demo flows work against Jira Cloud.
- Phase 4 automated tests pass (Playwright and xUnit).
- Test coverage and execution flow diagrams are documented with Mermaid.
- The README and demo guidance match the shipped behavior.
- The release is ready to be tagged and shared as v1.0.
