# Verification checklist

## Local

- [ ] `dotnet restore` succeeds.
- [ ] `dotnet build` succeeds.
- [ ] `Jira.McpServer` starts.
- [ ] `Agent.Api` starts.
- [ ] `Web` starts.
- [ ] `/health` returns `ok` for API and MCP server.
- [ ] Blocked issues prompt returns KM-102 and KM-105.
- [ ] Sprint summary prompt returns risks.

## Security

- [ ] No Jira token in repository.
- [ ] `.env` is ignored.
- [ ] Key Vault planned for Azure deployment.
- [ ] Write tools are not enabled in MVP.

## Demo

- [ ] Demo can be completed in 10 minutes.
- [ ] Narrative explains value beyond Jira dashboards.
- [ ] Roadmap explains enterprise hardening.
