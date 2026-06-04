using CMDProject.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CMDProject.Infrastructure.Security;

public class JwtTokenService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(
        string subject,
        IEnumerable<Claim>? claims = null)
    {
        var jwtSection =
            _configuration.GetSection("Jwt");

        var issuer =
            jwtSection["Issuer"]
            ?? throw new InvalidOperationException(
                "Jwt:Issuer not found");

        var audience =
            jwtSection["Audience"]
            ?? throw new InvalidOperationException(
                "Jwt:Audience not found");

        var key =
            jwtSection["Key"]
            ?? throw new InvalidOperationException(
                "Jwt:Key not found");

        var expireMinutes =
            int.TryParse(
                jwtSection["ExpireMinutes"],
                out var minutes)
                ? minutes
                : 60;

        var tokenClaims =
            new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    subject),

                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString("N")),

                new(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow
                        .ToUnixTimeSeconds()
                        .ToString(),
                    ClaimValueTypes.Integer64)
            };

        if (claims != null)
        {
            tokenClaims.AddRange(claims);
        }

        var signingKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

        var creds =
            new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer,
                audience,
                tokenClaims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(
                    expireMinutes),
                creds);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public string CreateRefreshToken(
        string subject)
    {
        var jwtSection =
            _configuration.GetSection("Jwt");

        var claims =
            new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    subject),

                new(
                    "token_type",
                    "refresh"),

                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString("N"))
            };

        var signingKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtSection["Key"]!));

        var creds =
            new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

        var expireDays =
            int.TryParse(
                jwtSection["RefreshExpireDays"],
                out var days)
                ? days
                : 7;

        var token =
            new JwtSecurityToken(
                jwtSection["Issuer"],
                jwtSection["Audience"],
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(
                    expireDays),
                creds);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public ClaimsPrincipal ValidateRefreshToken(
        string refreshToken)
    {
        var jwtSection =
            _configuration.GetSection("Jwt");

        var validationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    jwtSection["Issuer"],

                ValidAudience =
                    jwtSection["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSection["Key"]!)),

                ClockSkew =
                    TimeSpan.Zero
            };

        var handler =
            new JwtSecurityTokenHandler();

        var principal =
            handler.ValidateToken(
                refreshToken,
                validationParameters,
                out _);

        var tokenType =
            principal.Claims
                .FirstOrDefault(x =>
                    x.Type == "token_type")
                ?.Value;

        if (tokenType != "refresh")
        {
            throw new SecurityTokenException(
                "Invalid refresh token");
        }

        return principal;
    }
}