

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Eventuous.Postgresql;
using MongoDB.Driver.Core.Configuration;

namespace CTBX.ReportSickLeave.Backend;

public static class ReportSickLeaveFeatureRegistration
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ctbx-events-db")!.GuardAgainstNullOrEmpty("connectionstring");
        services.AddEventuousPostgres(connectionString, "ctbx", true);
        services.AddEventStore<PostgresStore>();
        
    }
    

}
