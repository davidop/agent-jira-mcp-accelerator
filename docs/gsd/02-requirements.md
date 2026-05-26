# Requirements

## Functional requirements

- Read issues from mock Jira data.
- Expose Jira tools through an MCP server.
- Provide an Agent API with natural-language questions.
- Return executive-friendly summaries.
- Include demo prompts and script.
- Prepare Jira Cloud implementation.

## Non-functional requirements

- No secrets in source control.
- Read-only tools by default.
- Simple local run path.
- Clear extension points.
- Azure-ready architecture.

## Acceptance criteria

- A user can ask for blocked issues.
- A user can ask for a sprint summary.
- A user can ask for assignee workload.
- The answer explains risk and next steps.
- The repository explains how to move from mock to Jira Cloud.
