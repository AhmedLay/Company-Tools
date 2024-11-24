using Eventuous.Postgresql.Subscriptions;
using Eventuous.Projections.MongoDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

public static class Registrations
{
    public static IServiceCollection RegisterJWTBearerAuthNService(this IServiceCollection services, IConfiguration config)
    {
        var identityOptions = config.GetSection(nameof(IdentityOptions))
                                    .Get<IdentityOptions>()
                                    .GuardAgainstNull("identityOptions");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer("Bearer", jwtOptions =>
        {
            jwtOptions.RequireHttpsMetadata = identityOptions!.RequireHttpsMetadata;
            jwtOptions.Authority = identityOptions.Authority;
            jwtOptions.Audience = identityOptions.Audience;
            jwtOptions.MetadataAddress = identityOptions.MetadataAddress;

            jwtOptions.TokenValidationParameters =
            new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidAudiences = [identityOptions.Audience],
                ValidIssuers = [identityOptions.Authority],
            };
        });

        return services;
    }

    /// <summary>
    /// Registers eventuous Read and Event stores.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterEventuousStores(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddEventuousPostgres(configuration.GetConnectionString("ctbx-events-db")!,"ctbx");
        
        services.AddCheckpointStore<MongoCheckpointStore>();

        services.AddSubscription<PostgresAllStreamSubscription, PostgresAllStreamSubscriptionOptions>(
            "CTBXProjections",
            builder => { 
                  //builder
                
                // TODO [AL] register projections here
                //.AddEventHandler<BookingStateProjection>()
                //.AddEventHandler<MyBookingsProjection>()
                //.WithPartitioningByStream(2)
                }
        );

        return services;
    }
}
