using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = DistributedApplication.CreateBuilder(args);

var webPortalclientSecret = builder.AddParameter("WebCockpitClientSecret", secret: true);
var webPortalClientId = builder.AddParameter("WebCockpitClientId", secret: true);
var backendClientId = builder.AddParameter("BackendClientId", secret: true);
var backendClientSecret = builder.AddParameter("BackendClientSecret", secret: true);
var demoUserName = builder.AddParameter("DemoUserName", secret: true);
var demoUserPassword = builder.AddParameter("DemoUserPassword", secret: true);

// we are using the same container for IDP and backend
// this is only to make it easy to manage the dbs in one PgAdmin
// in prod each service will have its own db container 
var postgresDbResource = builder.AddPostgres("ctbx-db")
                                 .WithPgAdmin(options =>
                                 {
                                     options.WithHostPort(port: 49100);                                     
                                 });

var idpDb = postgresDbResource
            .AddDatabase("idpDb");

var idpService = builder.AddProject<Projects.IDP>("idp")
             .WithEnvironment("IdentityDataConfig__BackendClientId", backendClientId)
             .WithEnvironment("IdentityDataConfig__BackendClientSecret", backendClientSecret)
             .WithEnvironment("IdentityDataConfig__WebPortalClientId", webPortalClientId)
             .WithEnvironment("IdentityDataConfig__WebPortalclientSecret", webPortalclientSecret)
             .WithEnvironment("IdentityDataConfig__DemoUserName", demoUserName)
             .WithEnvironment("IdentityDataConfig__DemoUserPassword", demoUserPassword)
             .WithReference(idpDb)
             .WaitFor(idpDb);

var backend = builder.AddProject<Projects.CTBX_Backend>("ctbx-backend");


// gets the url from the https launchsettings profile
var idpUrl = idpService.GetEndpoint("https");

var portal = builder.AddProject<Projects.CTBX_WebPortal>("ctbx-webportal")
                    .WithEnvironment("Identity__Authority", idpUrl)
                    .WithEnvironment("Identity__ClientId", webPortalClientId)
                    .WithEnvironment("Identity__ClientSecret", webPortalclientSecret)
                    .WithEnvironment("CockpitOptions__BackendUrl", backend.GetEndpoint("https"))
                    .WithEnvironment("CockpitOptions__ResourcesScopes", backendClientId)
                    .WithReference(idpService)
                    .WaitFor(idpService);

var portalUrl = portal.GetEndpoint("https");

idpService
    .WithEnvironment("ClientsUrls__Portal", portalUrl)
    .WithEnvironment("IdentityDataConfig__PortalClientRedirectUrl", $"{portalUrl}/signin-oidc")
    .WithEnvironment("IdentityDataConfig__PortalClientPostLogoutUrl", $"{portalUrl}/signout-callback-oidc");


builder.Build().Run();
