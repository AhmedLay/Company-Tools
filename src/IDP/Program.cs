using IDP;
using IDP.Common;

const string CorsPolicyName = "ctbxCorsPolicy";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddRazorPages();
builder.AddServiceDefaults();

// gets the clients urls from the settings (settings are configured via aspire AppHost)
var clientsUrls = builder
                 .Configuration
                 .GetSection("ClientsUrls")
                 .Get<ClientsUrls>()
                 .GuardAgainstNull("clientsUrls");

builder.Services.ConfigureClientsData(builder.Configuration);

// this is only for demo and meant for production
builder.Services.AddHostedService<DataSeedHostedService>();

// registers a polly policy for resilient connections
builder.Services.RegisterResiliencePipeline();

// registers all the required opendict sevices and configurations
builder.RegisterOpenIddict();

builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsPolicyName,
                      policy =>
                      {
                          policy.WithOrigins(clientsUrls!.WebPortal);
                          policy.AllowCredentials();
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                      });
});


var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapDefaultEndpoints();

app.Run();
