using Carter;
using CTBX.Backend;
using CTBX.EmployeesImport.Backend;
using CTBX.SkillManagment.Backend;
using Hangfire;
using Hangfire.PostgreSql;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiArchitecture.Application;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.AddHangfire(x => x.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("ctbx-common-db"))));
builder.Services.AddHangfireServer();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (ctx, ex) =>
    {
        return builder.Environment.IsDevelopment();
    };
    options.Map<UnauthorizedAccessException>(ex => new StatusCodeProblemDetails(StatusCodes.Status401Unauthorized));
    options.Map<NotImplementedException>(ex => new StatusCodeProblemDetails(StatusCodes.Status501NotImplemented));
    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});

builder.Services.AddCarter();

builder.AddMongoDBClient("ctbx-read-db");
builder.AddServiceDefaults();
builder.Services.AddCors(opts=>opts.AddPolicy("all",p=> p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
builder.Services.RegisterJWTBearerAuthNService(builder.Configuration);
builder.Services.RegisterEventuousStores(builder.Configuration);
EmployeesImportFeatureRegistration.RegisterServices(builder.Services, builder.Configuration);
AbsenceManagementFeatureRegistration.RegisterServices(builder.Services, builder.Configuration);
CommonUtilRegistration.RegisterServices(builder.Services, builder.Configuration);
SkillManagmentFeatureRegistration.RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}
app.UseProblemDetails();
app.UseCors("all");
app.MapDefaultEndpoints();
app.MapCarter();
app.UseHangfireDashboard();


app.Run();


