# 10-minute demo script

## 1. Business context — 60 seconds

"Jira already contains the project truth, but managers still depend on dashboards, JQL, meetings and manual reporting. This demo turns Jira into a governed enterprise agent tool."

## 2. Technical architecture — 90 seconds

Show the architecture diagram:

- Blazor Web UI.
- Agent API in .NET.
- Jira MCP Server in .NET.
- Jira data source.
- Future Azure AI Search and Foundry.

## 3. Live query: blocked work — 2 minutes

Prompt:

```text
¿Qué issues están bloqueadas en el proyecto KM?
```

Explain that the agent is not guessing. It calls a Jira MCP tool.

## 4. Live query: sprint executive summary — 2 minutes

Prompt:

```text
Resume el estado del sprint actual para comité de dirección.
```

Explain the value: no JQL, no manual dashboard reading, direct executive interpretation.

## 5. Live query: risk narrative — 2 minutes

Prompt:

```text
Dame un informe ejecutivo del proyecto KM con riesgos, bloqueos y próximos pasos.
```

Explain how this can later cross Jira with contract docs, minutes, architecture docs and delivery standards.

## 6. Close — 90 seconds

Position as accelerator:

- Same pattern works for Jira, Azure DevOps, GitHub, ServiceNow, Confluence and SharePoint.
- Start read-only; then add governed actions.
- Package as MVP + enterprise hardening.
