# Architecture

## Runtime view

```mermaid
sequenceDiagram
    actor User
    participant Web as Blazor Web
    participant Api as Agent.Api
    participant Mcp as Jira.McpServer
    participant Jira as Jira Client / Jira Cloud
    participant Search as Azure AI Search

    User->>Web: Pregunta en lenguaje natural
    Web->>Api: POST /api/agent/ask
    Api->>Mcp: Tool call via MCP
    Mcp->>Jira: Query issues/sprint/epic
    Jira-->>Mcp: Operational data
    Mcp-->>Api: Tool result
    Api-->>Web: Executive answer
    Note over Api,Search: Phase 2: cross Jira data with documents
```

## Services

- `Jira.Client`: domain model and data access abstraction.
- `Jira.McpServer`: exposes Jira tools to MCP-compatible clients.
- `Agent.Api`: demo orchestration facade; future Azure OpenAI / Foundry integration point.
- `Web`: Blazor UI for executive live demos.
- `AppHost`: Aspire composition scaffold.

## Security design

MVP:

- Mock data by default.
- Jira token only via user-secrets or Key Vault.
- Read-only operations first.

Enterprise:

- OAuth 2.0 for Atlassian.
- Entra ID authentication on the Web/API.
- RBAC by Jira project.
- Tool approval gates for write operations.
- Audit log for every tool call.
- Managed identity and Key Vault.
