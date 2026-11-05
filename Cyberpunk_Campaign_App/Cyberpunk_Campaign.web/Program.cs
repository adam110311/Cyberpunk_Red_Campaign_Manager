using Cyberpunk_Campaign.DataAccess;
using Cyberpunk_Campaign.web.Components;
using Cyberpunk_Campaign.Logic.Interfaces;
using Cyberpunk_Campaign.DataAccess.Repositories;
using Cyberpunk_Campaign.Logic.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("CyberpunkDB")
    ?? throw new InvalidOperationException("Connection string 'CyberpunkDB' not found in appsettings.json.");

builder.Services.AddSingleton(new SqlConnectionFactory(connectionString));

// Register services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddScoped<AccountService>();

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