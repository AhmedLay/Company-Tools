using DnsClient.Internal;
using Eventuous.Postgresql;
using Eventuous.Postgresql.Subscriptions;
using Eventuous.Projections.MongoDB;
using Eventuous.Subscriptions.Registrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
namespace MinimalApiArchitecture.Application;
public static class AbsenceManagementFeatureRegistration
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ctbx-events-db")!.GuardAgainstNullOrEmpty("connectionstring");
        services.AddEventuousPostgres(connectionString, "ctbx",true);
        services.AddEventStore<PostgresStore>();
        services.AddCommandService<AbsenceManagementApplicationService, AbsenceState>();
        services.AddCheckpointStore<MongoCheckpointStore>();
        services.AddSubscription<PostgresAllStreamSubscription, PostgresAllStreamSubscriptionOptions>(
            "AbsenceProjections",
            builder => builder
                .AddEventHandler<AbsenceManagementProjection>()
                );
    }
    public static SubscriptionBuilder RegisterSubscriptions(this SubscriptionBuilder<PostgresAllStreamSubscription, PostgresAllStreamSubscriptionOptions> builder)
    {
        return builder;
    }

}


