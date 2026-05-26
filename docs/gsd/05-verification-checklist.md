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

## Jira Cloud

- [ ] `Jira:Mode=Cloud` works with `Jira:BaseUrl`, `Jira:Email`, and `Jira:ApiToken` from user-secrets.
- [ ] Cloud search handles pagination without truncating expected demo data.
- [ ] Rate-limit and transient errors are retried and logged.
- [ ] API returns a safe fallback message when Jira Cloud is temporarily unavailable.

## Security

- [ ] No Jira token in repository.
- [ ] `.env` is ignored.
- [ ] Key Vault planned for Azure deployment.
- [ ] Write tools are not enabled in MVP.
- [ ] Jira Cloud path remains read-only.

## Demo

- [ ] Demo can be completed in 10 minutes.
- [ ] Narrative explains value beyond Jira dashboards.
- [ ] Roadmap explains enterprise hardening.
