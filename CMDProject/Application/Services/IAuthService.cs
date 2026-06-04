using CMDProject.Application.DTOs.Request;
using CMDProject.Application.DTOs.Response;

namespace CMDProject.Application.Services;
public interface IAuthService
{
    Task<LoginResponse> LoginByDbAsync(
        LoginRequest request,
        CancellationToken ct);

    Task<LoginResponse> RefreshTokenAsync(
        string refreshToken);
}