using System.Security.Principal;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.RegisterJWTBearerAuthNService(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();
