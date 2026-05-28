# Testing Quality Visibility

This folder contains Mermaid diagrams that summarize testing coverage and CI execution flow.

## Files

- coverage-by-layer.mmd: Maps REQ-04, REQ-06, and REQ-07 to test layers and CI wiring.
- ci-execution-flow.mmd: Shows CI stage order, smoke gate logic, and artifact publication.

## Freshness policy

- Diagram freshness is warning-only in this phase.
- Test-impacting changes should update docs/testing diagrams in the same PR when behavior or flow changes.
- Missing updates should trigger reviewer attention but must not block merge automatically.

## Coverage stance

- Coverage is advisory-only in this phase.
- No fixed percentage gate is required; confidence is based on deterministic critical-path tests.
