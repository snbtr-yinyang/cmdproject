using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CMDProject.API.Security;

public interface IGoogleIdTokenValidator
{
    Task<ClaimsPrincipal> ValidateAsync(string idToken, CancellationToken ct = default);
}

public sealed class GoogleIdTokenValidator(IConfiguration configuration) : IGoogleIdTokenValidator
{
    private readonly IConfigurationManager<OpenIdConnectConfiguration> _configManager =
        new ConfigurationManager<OpenIdConnectConfiguration>(
            "https://accounts.google.com/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever());

    public async Task<ClaimsPrincipal> ValidateAsync(string idToken, CancellationToken ct = default)
    {
        var clientId = configuration["ExternalAuth:Google:ClientId"];
        if (string.IsNullOrWhiteSpace(clientId))
            throw new InvalidOperationException("ExternalAuth:Google:ClientId is not configured.");

        var oidc = await _configManager.GetConfigurationAsync(ct);

        var handler = new JwtSecurityTokenHandler();
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuers = ["accounts.google.com", "https://accounts.google.com"],

            ValidateAudience = true,
            ValidAudience = clientId,

            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = oidc.SigningKeys,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };

        var principal = handler.ValidateToken(idToken, parameters, out _);
        return principal;
    }
}
