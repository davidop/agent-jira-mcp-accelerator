# Project Milestone: Release v1.0

## Objective

Ship the first release of the Enterprise Agent Accelerator with a stable read-only Jira demo path, starting from the existing local MVP and finishing with Jira Cloud-backed execution for the core demo flows.

## Release Summary

- Current foundation: local MVP with mock Jira data, Aspire composition, MCP tools, Agent API, and Blazor UI.
- Release focus: Jira Cloud integration, cloud-safe configuration, deterministic query behavior, and demo readiness.
- Product promise: turn Jira delivery data into executive answers without leaving read-only territory.

## In Scope

- Jira Cloud read access through the existing Jira.Client abstraction.
- Basic Auth demo path using email and API token.
- Deterministic JQL templates for the core demo prompts.
- Improved status mapping and resilient search behavior.
- Updated demo documentation and verification guidance.

## Out of Scope

- Jira write actions.
- Full Atlassian OAuth production rollout.
- Azure AI / Foundry orchestration.
- Cross-system connectors beyond Jira.

## Success Criteria

- The demo works end to end against Jira Cloud in read-only mode.
- The core prompts still produce executive-quality answers.
- Local development remains simple with Aspire and mock mode available.
- The release can be explained as a clear step from MVP to enterprise-ready integration.
