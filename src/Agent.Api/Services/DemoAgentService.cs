using EnterpriseAgentAccelerator.Jira.Client;
using EnterpriseAgentAccelerator.Jira.Client.Models;
using EnterpriseAgentAccelerator.Shared.Contracts;

namespace EnterpriseAgentAccelerator.Agent.Api.Services;

public sealed class DemoAgentService
{
    private readonly IJiraReader _jira;

    public DemoAgentService(IJiraReader jira) => _jira = jira;

    public async Task<AgentResponse> AnswerAsync(AgentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var q = request.Question.ToLowerInvariant();
            var projectKey = string.IsNullOrWhiteSpace(request.ProjectKey) ? "KM" : request.ProjectKey;

            if (q.Contains("bloque") || q.Contains("blocked"))
            {
                var data = await _jira.GetBlockedIssuesAsync(projectKey, cancellationToken);
                return BuildBlockedResponse(data, request.IncludeRawToolData);
            }

            if (q.Contains("sprint") || q.Contains("comité") || q.Contains("comite") || q.Contains("executive") || q.Contains("ejecutivo"))
            {
                var data = await _jira.GetSprintSummaryAsync(projectKey, null, cancellationToken);
                return BuildSprintResponse(data, request.IncludeRawToolData);
            }

            if (q.Contains("asign") || q.Contains("assigned"))
            {
                var assignee = request.UserName ?? "David";
                var data = await _jira.GetIssuesByAssigneeAsync(assignee, cancellationToken);
                return BuildAssigneeResponse(assignee, data, request.IncludeRawToolData);
            }

            var issues = await _jira.GetProjectIssuesAsync(projectKey, cancellationToken);
            return BuildProjectResponse(projectKey, issues, request.IncludeRawToolData);
        }
        catch (Exception)
        {
            return BuildCloudUnavailableResponse();
        }
    }

    private static AgentResponse BuildCloudUnavailableResponse() =>
        new(
            "No he podido consultar Jira en este momento. Revisa conectividad, configuración y límites de Jira, y vuelve a intentarlo.",
            [],
            null,
            ["¿Quieres que te ayude a validar la configuración Jira:BaseUrl, Jira:Email y Jira:ApiToken?", "¿Quieres que probemos el modo Mock mientras se recupera el acceso a Jira?"]);

    private static AgentResponse BuildBlockedResponse(IReadOnlyList<JiraIssue> issues, bool includeRaw)
    {
        var lines = issues.Select(i => $"- {i.Key}: {i.Summary} ({i.Priority}) — {i.BlockReason ?? "sin motivo informado"}");
        var answer = issues.Count == 0
            ? "No he encontrado issues bloqueadas en el proyecto."
            : $"Hay {issues.Count} issue(s) bloqueadas:\n" + string.Join("\n", lines) + "\n\nRecomendación: revisar owner, dependencia y fecha objetivo de desbloqueo para cada una.";
        return new AgentResponse(answer, ["get_blocked_issues"], includeRaw ? issues : null, ["¿Quieres que genere un resumen ejecutivo?", "¿Quieres crear acciones de desbloqueo?"]);
    }

    private static AgentResponse BuildSprintResponse(JiraSprintSummary summary, bool includeRaw)
    {
        var progress = summary.Total == 0 ? 0 : Math.Round((decimal)summary.Done / summary.Total * 100, 1);
        var risks = summary.Risks.Length == 0 ? "No hay riesgos relevantes detectados." : string.Join("\n", summary.Risks.Select(r => $"- {r}"));
        var answer = $"Resumen del {summary.Sprint} para {summary.ProjectKey}:\n\n" +
                     $"- Avance: {progress}% completado ({summary.Done}/{summary.Total}).\n" +
                     $"- En progreso: {summary.InProgress}.\n" +
                     $"- Bloqueadas: {summary.Blocked}.\n" +
                     $"- Alta/crítica prioridad: {summary.HighPriority}.\n\n" +
                     $"Riesgos:\n{risks}\n\n" +
                     "Próximo paso recomendado: revisar bloqueos y confirmar si el alcance del sprint sigue siendo realista.";
        return new AgentResponse(answer, ["get_sprint_summary"], includeRaw ? summary : null, ["¿Quieres versión para comité?", "¿Quieres que lo convierta en acta de seguimiento?"]);
    }

    private static AgentResponse BuildAssigneeResponse(string assignee, IReadOnlyList<JiraIssue> issues, bool includeRaw)
    {
        var lines = issues.Select(i => $"- {i.Key}: {i.Summary} — {i.Status} / {i.Priority}");
        var answer = issues.Count == 0
            ? $"No he encontrado issues asignadas a {assignee}."
            : $"Issues asignadas a {assignee}:\n" + string.Join("\n", lines);
        return new AgentResponse(answer, ["get_issues_by_assignee"], includeRaw ? issues : null, ["¿Quieres priorizarlas?", "¿Quieres detectar bloqueos?"]);
    }

    private static AgentResponse BuildProjectResponse(string projectKey, IReadOnlyList<JiraIssue> issues, bool includeRaw)
    {
        var blocked = issues.Count(i => i.Blocked);
        var high = issues.Count(i => i.Priority is "High" or "Critical");
        var done = issues.Count(i => i.Status.Equals("Done", StringComparison.OrdinalIgnoreCase));
        var answer = $"Estado general del proyecto {projectKey}:\n\n" +
                     $"- Total issues: {issues.Count}.\n" +
                     $"- Cerradas: {done}.\n" +
                     $"- Bloqueadas: {blocked}.\n" +
                     $"- Alta/crítica prioridad: {high}.\n\n" +
                     "Puedo profundizar en sprint, épica, bloqueos, riesgos o asignaciones.";
        return new AgentResponse(answer, ["get_project_issues"], includeRaw ? issues : null, ["Resume el sprint actual", "Dime las issues bloqueadas", "Genera informe ejecutivo"]);
    }
}
