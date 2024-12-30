using DnsClient.Internal;
using Eventuous.Postgresql;
using Eventuous.Postgresql.Subscriptions;
using Eventuous.Subscriptions.Registrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using MicrosoftLoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
namespace MinimalApiArchitecture.Application;
public static class AbsenceManagementFeatureRegistration
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration, Action<PostgresStoreOptions> configure)
    {
        var options = new PostgresStoreOptions();
        services.AddHostedService<AbsenceManagementDataSeeder>();
        services.Configure<PostgresCheckpointStoreOptions>(opt => opt.Schema = options.Schema);
        services.AddEventuousPostgres(options.ConnectionString, options.Schema, options.InitializeDatabase);
        services.AddEventStore<PostgresStore>();
        services.AddCheckpointStore(sp => new PostgresCheckpointStore(
            sp.GetRequiredService<NpgsqlDataSource>(),
            options.Schema,
            sp.GetRequiredService<MicrosoftLoggerFactory>() 
));

        services.AddCommandService<AbsenceManagementApplicationService, AbsenceState>();
    }
    public static SubscriptionBuilder RegisterSubscriptions(this SubscriptionBuilder<PostgresAllStreamSubscription, PostgresAllStreamSubscriptionOptions> builder)
    {
        builder
            .AddEventHandler<AbsenceManagementProjection>(); 
        return builder;
    }
}


