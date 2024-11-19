namespace IDP;

using IDP.Data;
using IDP.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using static OpenIddict.Abstractions.OpenIddictConstants;
public static class DIExtensions
{
    /// <summary>
    /// Registers all the needed OpenIddict services and configurations.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder RegisterOpenIddict(this WebApplicationBuilder builder)
    {
        // the used connection string section name is already configured via aspire AppHost.
        builder.AddNpgsqlDbContext<OpenIddictAppDbContext>("idpDb");

        // makes sure that the exception details are only showen in dev mode.
        if (builder.Environment.IsDevelopment())
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<OpenIddictAppDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

        builder.Services.RegisterQuartzService(builder.Configuration);

        builder.Services.RegisterOpenIddictService(builder.Environment.IsDevelopment());

        return builder;
    }

    private static IServiceCollection RegisterQuartzService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DbInitOptions>(options =>
        {
            options.QuartzDbConnectionString = configuration.GetConnectionString("idpDb") ?? string.Empty;
        });

        services.AddQuartz(options =>
        {
            options.SchedulerName = "Quartz Schedular for Idp";
            options.UseSimpleTypeLoader();

            options.UsePersistentStore(storeOptions =>
            {
                storeOptions.UsePostgres(options =>
                {
                    options.ConnectionString = configuration.GetConnectionString("idpDb") ?? string.Empty;
                });

                storeOptions.UseNewtonsoftJsonSerializer();
            });

            options.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });
        });

        // regsiters the db schema initializer that executes the migration scripts
        services.AddHostedService<DbSchemaInitializer>();

        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }

    private static IServiceCollection RegisterOpenIddictService(this IServiceCollection services, bool isDevEnvironment)
    {
        services.AddOpenIddict()
            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                options.UseEntityFrameworkCore()
                       .UseDbContext<OpenIddictAppDbContext>();
                //.ReplaceDefaultEntities<string>();

                // Enable Quartz.NET integration.
                options.UseQuartz();
            })


            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                // Enable the authorization, logout, token and userinfo endpoints.
                options.SetAuthorizationEndpointUris("connect/authorize")
                               .SetLogoutEndpointUris("connect/logout")
                               .SetIntrospectionEndpointUris("connect/introspect")
                               .SetTokenEndpointUris("connect/token")
                               .SetUserinfoEndpointUris("connect/userinfo")
                               .SetVerificationEndpointUris("connect/verify");

                // Mark the "email", "profile" and "roles" scopes as supported scopes.
                options.RegisterScopes(Scopes.Profile, Scopes.Roles, Scopes.Email);

                // Note: this sample only uses the authorization code flow but you can enable
                // the other flows if you need to support implicit, password or client credentials.
                options.AllowAuthorizationCodeFlow()
                       .AllowRefreshTokenFlow();

                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableLogoutEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .EnableUserinfoEndpointPassthrough()
                       .EnableStatusCodePagesIntegration();

                if (isDevEnvironment)
                {
                    options.AddEphemeralEncryptionKey()
                        .AddEphemeralSigningKey()
                        .DisableAccessTokenEncryption();
                }
            })

            // Register the OpenIddict validation components.
            .AddValidation(options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });

        return services;
    }
}
