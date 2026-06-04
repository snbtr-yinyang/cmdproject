using CMDProject.Application.DTOs.Request;
using CMDProject.Application.DTOs.Response;
using CMDProject.Application.Interfaces.Repository;
using CMDProject.Domain.Entities;
using CMDProject.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CMDProject.Application.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _tokenRepository;

    public AuthService(
        ApplicationDbContext db,
        IJwtService jwtService,
        IRefreshTokenRepository tokenRepository)
    {
        _db = db;
        _jwtService = jwtService;
        _tokenRepository = tokenRepository;
    }

    public async Task<LoginResponse> LoginByDbAsync(
        LoginRequest request,
        CancellationToken ct)
    {
        var user = await _db.AuthUsers
            .AsNoTracking()
            .Where(x => x.Auth_UserName == request.Username)
            .Select(x => new
            {
                x.Users_Id,
                Username = x.Auth_UserName ?? string.Empty,
                Password = x.Auth_Password ?? string.Empty
            })
            .FirstOrDefaultAsync(ct);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid username");

        if (!string.Equals(
                user.Password,
                request.Password,
                StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("Invalid password");
        }

        var accessToken =
            _jwtService.CreateToken(
                user.Users_Id.ToString(),
                new[]
                {
                    new Claim("username", user.Username),
                    new Claim("auth_provider", "db")
                });

        var refreshToken =
            _jwtService.CreateRefreshToken(
                user.Users_Id.ToString());

        await _tokenRepository.AddAsync(
            new UserToken
            {
                UserId = user.Users_Id,
                TokenName = refreshToken,
                ActiveStatus = true,
                Created_Date = DateTime.UtcNow,
                Expired_Date = DateTime.UtcNow.AddDays(7)
            });

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(
        string refreshToken)
    {
        var principal =
            _jwtService.ValidateRefreshToken(
                refreshToken);

        var storedToken =
            await _tokenRepository.GetByTokenAsync(
                refreshToken);

        if (storedToken == null)
            throw new UnauthorizedAccessException();

        if (!storedToken.ActiveStatus)
            throw new UnauthorizedAccessException();

        if (storedToken.Expired_Date < DateTime.UtcNow)
            throw new UnauthorizedAccessException();

        storedToken.ActiveStatus = false;
        storedToken.Revoked_Date = DateTime.UtcNow;

        await _tokenRepository.UpdateAsync(
            storedToken);

        var userId =
            principal.FindFirst(
                JwtRegisteredClaimNames.Sub)!
            .Value;

        var newAccessToken =
            _jwtService.CreateToken(userId);

        var newRefreshToken =
            _jwtService.CreateRefreshToken(userId);

        await _tokenRepository.AddAsync(
            new UserToken
            {
                UserId = storedToken.UserId,
                TokenName = newRefreshToken,
                ActiveStatus = true,
                Created_Date = DateTime.UtcNow,
                Expired_Date = DateTime.UtcNow.AddDays(7)
            });

        return new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            TokenType = "Bearer",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}