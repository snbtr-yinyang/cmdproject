using CMDProject.Application.DTOs.Request;
using CMDProject.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMDProject.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login/db")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var result =
            await _authService
                .LoginByDbAsync(
                    request,
                    ct);

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request)
    {
        var result =
            await _authService
                .RefreshTokenAsync(
                    request.RefreshToken);

        return Ok(result);
    }
}