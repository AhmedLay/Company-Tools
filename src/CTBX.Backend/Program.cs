using Carter;
using CTBX.Backend;
using Eventuous.Postgresql.Subscriptions;
using Hangfire;
using Hangfire.PostgreSql;
using Hellang.Middleware.ProblemDetails;

using MinimalApiArchitecture.Application;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSubscription<PostgresAllStreamSubscription, PostgresAllStreamSubscriptionOptions>(
    "AbsenceManagementSubscription",
    subscriptionBuilder => AbsenceManagementFeatureRegistration.RegisterSubscriptions(subscriptionBuilder)
);
EmployeesImportFeatureRegistration.RegisterServices(builder.Services, builder.Configuration);
AbsenceManagementFeatureRegistration.RegisterServices(
    builder.Services,
    builder.Configuration,
    options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("ctbx-events-db")
    ?? "DefaultConnectionString";
        options.Schema = "absencemanagement"; 
        options.InitializeDatabase = true;  
    }
);
CommonUtilRegistration.RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();
app.UseProblemDetails();
app.UseCors("all");
app.MapDefaultEndpoints();
app.MapCarter();
app.UseHangfireDashboard();


app.Run();


