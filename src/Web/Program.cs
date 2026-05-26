using EnterpriseAgentAccelerator.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient("agent", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("AgentApi:BaseUrl") ?? "https://localhost:7041");
});

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
