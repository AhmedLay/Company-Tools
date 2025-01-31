using CTBX.WebPortal;
using CTBX.WebPortal.Auth;
using CTBX.WebPortal.Client.Pages;
using CTBX.WebPortal.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.RegisterAuthNServices(builder.Configuration);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddServiceDiscovery();
builder.Services.AddHttpForwarderWithServiceDiscovery();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CTBX.WebPortal.Client._Imports).Assembly);

var backendUrl = app.Configuration
                         .GetSection(nameof(PortalOptions))
                         !.GuardAgainstNull(nameof(PortalOptions))
                         !.Get<PortalOptions>()
                         !.BackendUrl;


app.ForwardBackendApiRequest(backendUrl);
app.MapGroup("/authentication").MapLoginAndLogout();
app.Run();
