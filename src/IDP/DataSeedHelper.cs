using IDP.Data;
using IDP.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
public static class DataSeedHelper
{
    public static void ConfigureClientsData(this IServiceCollection services,IConfiguration configuration)
    {
        services.Configure<IdentityDataConfig>(configuration.GetSection(nameof(IdentityDataConfig)));
    }

    public static async Task SeedDemoData(IServiceProvider serviceProvider,CancellationToken cancellationToken)
    {
        var dataConfig = serviceProvider.GetRequiredService<IOptions<IdentityDataConfig>>().Value.GuardAgainstNull("dataConfig");

        var context = serviceProvider.GetRequiredService<OpenIddictAppDbContext>();
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync();
        await RegisterScopesAsync(serviceProvider,dataConfig);

        var manager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // API
        if ((await manager.FindByClientIdAsync(dataConfig.BackendClientId)).IsNull())
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = dataConfig.BackendClientId,
                ClientSecret = dataConfig.BackendClientSecret,
                Permissions =
                    {
                        Permissions.Endpoints.Introspection
                    }
            };

            await manager.CreateAsync(descriptor);
        }

        if ((await manager.FindByClientIdAsync(dataConfig.WebPortalClientId)).IsNull())
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {                
                ClientId = dataConfig.WebPortalClientId,
                ClientSecret = dataConfig.WebPortalClientSecret,
                ConsentType = ConsentTypes.Implicit,
                DisplayName = "The Bff Web web host client application",
                RedirectUris =
                {
                    new Uri(dataConfig.PortalClientRedirectUrl)
                },
                PostLogoutRedirectUris =
                {
                    new Uri(dataConfig.PortalClientPostLogoutUrl)
                },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Logout,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.ResponseTypes.Code,
                    Permissions.GrantTypes.RefreshToken,                    
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope+dataConfig.BackendClientId,
                    Permissions.Prefixes.Scope+"offline_access"

                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                },
            });
        }
        await RegisterDemoUser(serviceProvider,dataConfig);
    }

    static async Task RegisterScopesAsync(IServiceProvider provider,IdentityDataConfig dataConfig)
    {
        var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

        if (await manager.FindByNameAsync(dataConfig.BackendClientId) is null)
        {
            await manager.CreateAsync(new OpenIddictScopeDescriptor
            {
                DisplayName = "Backend API access",              
                Name = dataConfig.BackendClientId,
                Resources =
                    {
                        dataConfig.BackendClientId
                    }
            });
        }
    }

    static async Task RegisterDemoUser(IServiceProvider serviceProvider,IdentityDataConfig dataConfig)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<UserManager<ApplicationUser>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync(dataConfig.DemoUserName);
        if(user.IsNotNull())
        {
            logger.LogInformation("Demo user {0} already exists",dataConfig.DemoUserName);
            return;
        }

        user = new ApplicationUser
        {
            Id = $"user-{Guid.NewGuid().ToString()}",
            Email = dataConfig.DemoUserName,
            UserName = dataConfig.DemoUserName,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(user);


        if (!result.Succeeded)
        {
            var msg = string.Join(",", result.Errors.Select(e => e.Description));
            logger.LogError("Test user could not be created {msg}", msg);
            return;
        }

        result = await userManager.AddPasswordAsync(user, dataConfig.DemoUserPassword);

        if (!result.Succeeded)
        {
            var msg = string.Join(",", result.Errors.Select(e => e.Description));
            logger.LogError("Test user password could not be set {msg}", msg);
            return;
        }

        logger.LogInformation("Test User Added");
    }
}
