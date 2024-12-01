using Eventuous.Postgresql.Subscriptions;
using Eventuous.Subscriptions.Registrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace MinimalApiArchitecture.Application;

public static class AbsenceManagementFeatureRegistration
{
    public static void RegisterServices(IServiceCollection services,IConfiguration configuration)
    {
        // add service registrations here
    }

    public static SubscriptionBuilder RegisterSubscriptions(this SubscriptionBuilder<PostgresAllStreamSubscription, PostgresAllStreamSubscriptionOptions> builder)
    {
        //add all projections and subscriptions here
        throw new NotImplementedException();
    }
}

