# Phase 2: Jira Cloud Integration - Plan

## Goal

Replace the mock Jira source with Jira Cloud read access while keeping the MVP read-only, preserving the current demo flows, and leaving a clean enterprise path for OAuth 2.0.

## Scope

- Add production-safe Jira Cloud configuration for demo use with email + API token.
- Keep the current `Jira.Client` abstraction and harden the cloud reader.
- Normalize Jira statuses into the internal summary model.
- Add deterministic JQL templates for blocked issues, sprint summary, assignee workload, and epic or project drilldowns.
- Add pagination and rate-limit handling to cloud search.
- Add structured observability around Jira Cloud calls.
- Update docs and demo guidance for the cloud switch.

## Out of Scope

- Jira write operations.
- Approval workflows for writes.
- Full Atlassian OAuth delivery beyond the extension seam and design contract.
- Azure AI / Foundry orchestration.
- Cross-system integrations.

## File-by-File Changes

- `src/Jira.Client/JiraOptions.cs`
  - Add cloud-specific settings for pagination, retry/backoff tuning, and optional OAuth placeholders if needed.
- `src/Jira.Client/ServiceCollectionExtensions.cs`
  - Register cloud helpers and any resilience dependencies required by the cloud reader.
- `src/Jira.Client/JiraCloudReader.cs`
  - Add pagination handling, rate-limit awareness, better status mapping, and explicit JQL templates.
- `src/Jira.Client/Models/JiraIssue.cs`
  - Extend or normalize fields if the cloud payload requires richer status or sprint metadata.
- `src/Jira.Client/Models/JiraSprintSummary.cs`
  - Improve derived risk/output fields if the cloud data exposes more nuance.
- `src/Agent.Api/Services/DemoAgentService.cs`
  - Preserve the current answers while adapting phrasing or fallbacks for cloud-specific errors.
- `src/Jira.McpServer/Program.cs`
  - Keep the MCP tool surface read-only and ensure cloud mode remains safe in local composition.
- `src/Agent.Api/Program.cs`
  - Ensure cloud mode is wired through the same configuration surface used by the demo.
- `src/AppHost/Program.cs`
  - Keep local composition defaults sane for cloud mode and developer use.
- `README.md`
  - Document the cloud switch, required secrets, and the read-only constraint.
- `docs/demo-script.md`
  - Keep the current 10-minute demo narrative aligned with the cloud-backed flow.
- `docs/gsd/05-verification-checklist.md`
  - Add or update cloud-specific verification items.

## Risks

- Jira Cloud search shape can differ from the mock data shape, which can break existing summary assumptions.
- Rate limiting or slow responses may degrade the demo if retries and timeouts are not explicit.
- Status mapping can drift if Jira workflows are customized per project.
- OAuth scope creep could delay the demo if it is not kept as an extension seam only.
- Query templates can become brittle if they are not kept close to the current demo prompts.

## Verification Commands

- `dotnet restore`
- `dotnet build EnterpriseAgentAccelerator.slnx -v minimal`
- `dotnet run --project src/Jira.McpServer/Jira.McpServer.csproj`
- `dotnet run --project src/Agent.Api/Agent.Api.csproj`
- `dotnet run --project src/Web/Web.csproj`
- `aspire run src/AppHost/AppHost.csproj --non-interactive --nologo`
- Run the blocked issues and sprint summary prompts against Jira Cloud configuration.
- Confirm the app stays read-only and no write tools are exposed.

## Demo Impact

- The demo moves from local mock data to real Jira Cloud data while keeping the same user story and executive narrative.
- The same blocked issues and sprint summary prompts should still be the core live demo moments.
- The cloud mode should make the demo more credible without changing the pace or structure of the 10-minute walkthrough.

## Implementation Order

1. Harden Jira Cloud configuration and resilience in `Jira.Client`.
2. Normalize Jira Cloud status and sprint mapping.
3. Keep the Agent API responses stable against the new cloud-backed data.
4. Update docs and demo guidance.
5. Verify the read-only demo flow end to end.

