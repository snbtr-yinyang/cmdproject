using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CMDProject.API.Security;

public static class MicrosoftEntraBearerSetup
{
    public const string Scheme = "EntraBearer";

    public static Microsoft.AspNetCore.Authentication.AuthenticationBuilder AddMicrosoftEntraBearer(
        this Microsoft.AspNetCore.Authentication.AuthenticationBuilder builder,
        IConfiguration configuration)
    {
        var tenantId = configuration["ExternalAuth:Microsoft:TenantId"] ?? "common";
        var instance = configuration["ExternalAuth:Microsoft:Instance"] ?? "https://login.microsoftonline.com/";
        var authority = $"{instance.TrimEnd('/')}/{tenantId}/v2.0";
        var audience = configuration["ExternalAuth:Microsoft:Audience"];

        if (string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException("ExternalAuth:Microsoft:Audience is not configured.");

        return builder.AddJwtBearer(Scheme, options =>
        {
            options.Authority = authority;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = audience
            };
        });
    }
}
