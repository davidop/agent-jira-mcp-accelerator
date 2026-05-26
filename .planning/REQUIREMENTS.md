# Release v1.0 Requirements

## Functional Requirements

1. The system must support Jira Cloud read access using the existing Jira.Client abstraction.
2. The system must keep the mock data mode available for local development and demos.
3. The system must accept Jira Cloud configuration through email, API token, and base URL.
4. The system must return answers for the current demo flows: blocked issues, sprint summary, assignee workload, epic or project drilldowns, and executive summaries.
5. The system must use deterministic JQL templates for the supported queries.
6. The system must normalize Jira status information into consistent internal states for reporting.
7. The system must handle pagination and rate-limit conditions without breaking the demo flow.
8. The system must preserve read-only behavior in the MCP tool surface.

## Non-Functional Requirements

1. The cloud path must be safe to configure through user-secrets or environment variables.
2. The cloud reader must provide useful logs for auth, query, pagination, and failure diagnosis.
3. The release must remain easy to run locally with Aspire.
4. The implementation should degrade gracefully if Jira Cloud is slow or partially unavailable.
5. The release should preserve the current Spanish demo narrative and executive tone.

## Acceptance Criteria

- A Jira Cloud sandbox can be used without changing the demo shape.
- The main demo prompts still work after switching from mock to cloud mode.
- The project remains read-only at release time.
- The release documentation clearly explains how to switch modes.
