using CMDProject.API.Extensions;
using CMDProject.API.Security;
using CMDProject.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMDProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(
    ApplicationDbContext db,
    IJwtTokenService jwt,
    IGoogleIdTokenValidator googleValidator) : ControllerBase
{
    public sealed record DbLoginRequest(string Username, string Password);
    public sealed record GoogleLoginRequest(string IdToken);

    [HttpPost("login/db")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginByDb([FromBody] DbLoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Username and password are required" });

        // Project only needed fields to avoid materializing null columns into non-nullable CLR properties.
        var user = await db.AuthUsers
            .AsNoTracking()
            .Where(x => x.Auth_UserName == request.Username)
            .Select(x => new
            {
                x.Users_Id,
                Auth_UserName = x.Auth_UserName ?? string.Empty,
                Auth_Password = x.Auth_Password ?? string.Empty
            })
            .FirstOrDefaultAsync(ct);

        if (user is null)
            return Unauthorized(new { message = "Invalid username/password" });

        // NOTE: This matches your current schema (plain text). Replace with hashed verification ASAP.
        if (!string.Equals(user.Auth_Password, request.Password, StringComparison.Ordinal))
            return Unauthorized(new { message = "Invalid username/password" });

        var token = jwt.CreateToken(
            subject: user.Auth_UserName,
            extraClaims:
            [
                new Claim("auth_provider", "db"),
                new Claim("users_id", user.Users_Id.ToString())
            ]);

        return Ok(new { access_token = token, token_type = "Bearer" });
    }

    [HttpPost("login/google")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginByGoogle([FromBody] GoogleLoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            return BadRequest(new { message = "IdToken is required" });

        var principal = await googleValidator.ValidateAsync(request.IdToken, ct);

        var email = principal.FindFirstValue(ClaimTypes.Email)
                    ?? principal.FindFirstValue("email");

        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized(new { message = "Google token has no email claim." });

        var token = jwt.CreateToken(
            subject: email,
            extraClaims:
            [
                new Claim("auth_provider", "google"),
                new Claim(ClaimTypes.Email, email)
            ]);

        return Ok(new { access_token = token, token_type = "Bearer" });
    }

    // Example endpoint: requires Microsoft Entra token (SSO/AD)
    [HttpGet("me/entra")]
    [Authorize(AuthenticationSchemes = MicrosoftEntraBearerSetup.Scheme)]
    public IActionResult MeFromEntra()
    {
        return Ok(new
        {
            name = User.Identity?.Name,
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    // Example endpoint: requires API internal JWT (from DB/Google login)
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = ServiceCollectionExtensions.ApiJwtScheme)]
    public IActionResult Me()
    {
        return Ok(new
        {
            sub = User.FindFirstValue("sub"),
            provider = User.FindFirstValue("auth_provider"),
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
}
