
using Cyberpunk_Campaign.Components;
using Cyberpunk_Campaign.DataAcess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("CyberpunkDB")
    ?? throw new InvalidOperationException("Connection string 'CyberpunkDB' not found in appsettings.json.");

builder.Services.AddSingleton(new SqlConnectionFactory(connectionString));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();