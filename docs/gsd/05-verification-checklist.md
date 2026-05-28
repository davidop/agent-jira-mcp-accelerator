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

## Testing Quality Visibility

- [ ] `docs/testing/coverage-by-layer.mmd` maps REQ-04, REQ-06, and REQ-07.
- [ ] `docs/testing/ci-execution-flow.mmd` reflects current CI stage order and smoke gate behavior.
- [ ] Mermaid freshness policy is documented as warning-only (non-blocking) for this phase.

## Release Hardening (v1.0)

- [ ] `README.md` reflects current shipped scope (Cloud read-only, CI test gates, runtime deploy path).
- [ ] `docs/demo-script.md` matches the shipped demo posture and current release caveats.
- [ ] `docs/gsd/06-release-checklist.md` is complete with evidence links.
- [ ] `docs/gsd/07-release-notes-v1.0.md` exists and lists capabilities, constraints, and next steps.
