using System.Security.Claims;

namespace CMDProject.Application.Services;
public interface IJwtService
{
    string CreateToken(
        string subject,
        IEnumerable<Claim>? claims = null);

    string CreateRefreshToken(
        string subject);

    ClaimsPrincipal ValidateRefreshToken(
        string refreshToken);
}
