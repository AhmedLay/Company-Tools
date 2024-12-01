using System.Security.Principal;
using Carter;
using CTBX.AbsenceManagement;
using CTBX.Backend;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter();
builder.AddMongoDBClient("ctbx-read-db");
builder.AddServiceDefaults();
builder.Services.RegisterJWTBearerAuthNService(builder.Configuration);
builder.Services.RegisterEventuousStores(builder.Configuration);
var app = builder.Build();

app.MapDefaultEndpoints();

app.MapCarter();
app.Run();
