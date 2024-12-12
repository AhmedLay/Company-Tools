using Carter;
using CTBX.Backend;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.Configuration;
using MinimalApiArchitecture.Application;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();
app.UseProblemDetails();
app.UseCors("all");
app.MapDefaultEndpoints();
app.MapCarter();
app.Run();


