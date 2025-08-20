using ServiceRegistry.Dashboard.Components;

var builder = WebApplication.CreateBuilder(args);

var monitorUrl = builder.Configuration["MonitorApiBaseUrl"] ?? "http://localhost:5000";
builder.Services.AddHttpClient("monitor", client => client.BaseAddress = new Uri(monitorUrl));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
