using System.Net.Http.Headers;
using CTBX.WebPortal.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Yarp.ReverseProxy.Transforms;
using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;
namespace CTBX.WebPortal;
public static class Registrations
{
    const string SI_OIDC_SCHEME = "SmartInspectOidc";
    const string SI_OIDC_ACCESS_TOKEN_NAME = "access_token";
    const string SI_OIDC_ACCESS_TOKEN_BEARER = "Bearer";

    public static WebApplication ForwardBackendApiRequest(this WebApplication app, string webAPIBackEndRootUrl)
    {
        app.MapForwarder("api/ctbx/{**catch-all}/", webAPIBackEndRootUrl, transformBuilder =>
        {
            transformBuilder.AddRequestTransform(async transformContext =>
            {
                var accessToken = await transformContext.HttpContext.GetTokenAsync(SI_OIDC_ACCESS_TOKEN_NAME);
                transformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue
                (
                    SI_OIDC_ACCESS_TOKEN_BEARER,
                    accessToken
                );
            });
        })
            .RequireAuthorization()
            ;

        return app;
    }

    public static IServiceCollection RegisterAuthNServices(this IServiceCollection services, IConfiguration config)
    {
        var idpOpenIddictOptions = config.GetSection(nameof(IdentityOptions))
                                          .Get<IdentityOptions>();

        var scopes = config.GetSection(nameof(PortalOptions))
                                          .Get<PortalOptions>()
                                          !.GuardAgainstNull(nameof(PortalOptions))
                                          .ResourcesScopes ?? string.Empty;

        // Add services to the container.
        services.AddAuthentication(SI_OIDC_SCHEME)
            .AddOpenIdConnect(SI_OIDC_SCHEME, oidcOptions =>
            {
                // For the following OIDC settings, any line that's commented out
                // represents a DEFAULT setting. If you adopt the default, you can
                // remove the line if you wish.

                // ........................................................................
                // The OIDC handler must use a sign-in scheme capable of persisting 
                // user credentials across requests.

                oidcOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // ........................................................................

                // ........................................................................
                // The "openid" and "profile" scopes are required for the OIDC handler 
                // and included by default. You should enable these scopes here if scopes 
                // are provided by "Authentication:Schemes:MicrosoftOidc:Scope" 
                // configuration because configuration may overwrite the scopes collection.

                //oidcOptions.Scope.Add(OpenIdConnectScope.OpenIdProfile);
                // ........................................................................

                // ........................................................................
                // The following paths must match the redirect and post logout redirect 
                // paths configured when registering the application with the OIDC provider. 
                // For Microsoft Entra ID, this is accomplished through the "Authentication" 
                // blade of the application's registration in the Azure portal. Both the
                // signin and signout paths must be registered as Redirect URIs. The default 
                // values are "/signin-oidc" and "/signout-callback-oidc".
                // Microsoft Identity currently only redirects back to the 
                // SignedOutCallbackPath if authority is 
                // https://login.microsoftonline.com/{TENANT ID}/v2.0/ as it is below. 
                // You can use the "common" authority instead, and logout redirects back to 
                // the Blazor app. For more information, see 
                // https://github.com/AzureAD/microsoft-authentication-library-for-js/issues/5783

                //oidcOptions.CallbackPath = new PathString("/signin-oidc");
                //oidcOptions.SignedOutCallbackPath = new PathString("/signout-callback-oidc");
                // ........................................................................

                // ........................................................................
                // The RemoteSignOutPath is the "Front-channel logout URL" for remote single 
                // sign-out. The default value is "/signout-oidc".

                //oidcOptions.RemoteSignOutPath = new PathString("/signout-oidc");
                // ........................................................................

                // ........................................................................
                // The "Weather.Get" scope is configured in the Azure or Entra portal under 
                // "Expose an API". This is necessary for backend web API (MinimalApiJwt)
                // to validate the access token with AddBearerJwt. The following code example
                // uses a scope format of the App ID URI for an AAD B2C tenant type. If your
                // tenant is an ME-ID tenant, the App ID URI format is different:
                // api://{CLIENT ID}, so the full scope with an API name of "Weather.Get" is:
                // api://{CLIENT ID}/Weather.Get
                // The {CLIENT ID} is the application (client) ID of the MinimalApiJwt app 
                // registration.

                //oidcOptions.Scope.Add("https://{DIRECTORY NAME}.onmicrosoft.com/{CLIENT ID}/Weather.Get");
                oidcOptions.Scope.Add(OpenIdConnectScope.OpenId);
                oidcOptions.Scope.Add(OpenIdConnectScope.OpenIdProfile);
                oidcOptions.Scope.Add(OpenIdConnectScope.Email);
                oidcOptions.Scope.Add(ClaimConstants.Roles);
                oidcOptions.Scope.AddConfiguredScopes(scopes);
                // ........................................................................

                // ........................................................................
                // The following example Authority is configured for Microsoft Entra ID
                // and a single-tenant application registration. Set the {TENANT ID} 
                // placeholder to the Tenant ID. The "common" Authority 
                // https://login.microsoftonline.com/common/v2.0/ should be used 
                // for multi-tenant apps. You can also use the "common" Authority for 
                // single-tenant apps, but it requires a custom IssuerValidator as shown 
                // in the comments below. 

                oidcOptions.Authority = idpOpenIddictOptions!.Authority;
                // ........................................................................

                // ........................................................................
                // Set the Client ID for the app. Set the {CLIENT ID} placeholder to
                // the Client ID.

                oidcOptions.ClientId = idpOpenIddictOptions.ClientId;
                // ........................................................................

                // ........................................................................
                // ClientSecret shouldn't be compiled into the application assembly or 
                // checked into source control. Adopt User Secrets, Azure KeyVault, 
                // or an environment variable to supply the value. Authentication scheme
                // configuration is automatically read from 
                // "Authentication:Schemes:{SchemeName}:{PropertyName}", so ClientSecret is 
                // for OIDC configuration is automatically read from 
                // "Authentication:Schemes:MicrosoftOidc:ClientSecret" configuration.
                oidcOptions.MetadataAddress = $"{idpOpenIddictOptions.Authority}/.well-known/openid-configuration";
                oidcOptions.ClientSecret = idpOpenIddictOptions.ClientSecret;
                // ........................................................................

                // ........................................................................
                // Setting ResponseType to "code" configures the OIDC handler to use 
                // authorization code flow. Implicit grants and hybrid flows are unnecessary
                // in this mode. In a Microsoft Entra ID app registration, you don't need to 
                // select either box for the authorization endpoint to return access tokens 
                // or ID tokens. The OIDC handler automatically requests the appropriate 
                // tokens using the code returned from the authorization endpoint.

                oidcOptions.ResponseType = OpenIdConnectResponseType.Code;
                // ........................................................................

                // ........................................................................
                // // Set MapInboundClaims to "false" to obtain the original claim types from 
                // the token. Many OIDC servers use "name" and "role"/"roles" rather than 
                // the SOAP/WS-Fed defaults in ClaimTypes. Adjust these values if your 
                // identity provider uses different claim types.

                oidcOptions.MapInboundClaims = false;
                oidcOptions.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
                oidcOptions.TokenValidationParameters.RoleClaimType = ClaimConstants.Roles;
                // ........................................................................

                // ........................................................................
                // Many OIDC providers work with the default issuer validator, but the
                // configuration must account for the issuer parameterized with "{TENANT ID}" 
                // returned by the "common" endpoint's /.well-known/openid-configuration
                // For more information, see
                // https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/1731

                //var microsoftIssuerValidator = AadIssuerValidator.GetAadIssuerValidator(oidcOptions.Authority);
                //oidcOptions.TokenValidationParameters.IssuerValidator = microsoftIssuerValidator.Validate;
                // ........................................................................

                // ........................................................................
                // OIDC connect options set later via ConfigureCookieOidcRefresh
                //
                // (1) The "offline_access" scope is required for the refresh token.
                //
                // (2) SaveTokens is set to true, which saves the access and refresh tokens
                // in the cookie, so the app can authenticate requests for weather data and
                // cookie, so the app can authenticate requests for weather data and
                // use the refresh token to obtain a new access token on access token
                // expiration.
                // ........................................................................
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

        // ConfigureCookieOidcRefresh attaches a cookie OnValidatePrincipal callback to get
        // a new access token when the current one expires, and reissue a cookie with the
        // new access token saved inside. If the refresh fails, the user will be signed
        // out. OIDC connect options are set for saving tokens and the offline access
        // scope.
        services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, SI_OIDC_SCHEME);

        services.AddAuthorization();

        services.AddCascadingAuthenticationState();

        services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
        services.AddHttpContextAccessor();

        return services;
    }

    private static void AddConfiguredScopes(this ICollection<string> @this, string scopesCsv)
    {
        var scopes = scopesCsv.Split(',');

        foreach (var scope in scopes)
        {
            @this.Add(scopes[0]);
        }
    }

}
