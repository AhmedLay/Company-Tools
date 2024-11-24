using System.Security.Principal;

var builder = WebApplication.CreateBuilder(args);
builder.AddMongoDBClient("ctbx-read-db");
builder.AddServiceDefaults();
builder.Services.RegisterJWTBearerAuthNService(builder.Configuration);
builder.Services.RegisterEventuousStores(builder.Configuration);
var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();
