using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
                ValidAudiences = new[] { identityOptions.Audience },
                ValidIssuers = new[] { identityOptions.Authority },
            };
        });

        return services;
    }

}
