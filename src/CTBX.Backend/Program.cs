using Carter;
using CTBX.Backend;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter();
builder.AddMongoDBClient("ctbx-read-db");
builder.AddServiceDefaults();
builder.Services.AddCors(opts=>opts.AddPolicy("all",p=> p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
builder.Services.RegisterJWTBearerAuthNService(builder.Configuration);
builder.Services.RegisterEventuousStores(builder.Configuration);
builder.Services.AddHostedService<DbInitService>(); // added by duy 
var app = builder.Build();
app.UseCors("all");
app.MapDefaultEndpoints();
app.MapCarter();
app.Run();

