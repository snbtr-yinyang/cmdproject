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
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new UnauthorizedAccessException("Username is required");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new UnauthorizedAccessException("Password is required");

        var normalizedUsername = request.Username.Trim();

        var user = await _db.AuthUsers
            .AsNoTracking()
            .Where(x => x.AuthUserName == normalizedUsername)
            .Select(x => new
            {
                x.UserId,
                Username = x.AuthUserName ?? string.Empty,
                Password = x.AuthPassword ?? string.Empty
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
                user.UserId.ToString(),
                new[]
                {
                    new Claim("username", user.Username),
                    new Claim("auth_provider", "db")
                });

        var refreshToken =
            _jwtService.CreateRefreshToken(
                user.UserId.ToString());

        await _tokenRepository.AddAsync(
            new UserToken
            {
                UserId = user.UserId,
                TokenName = refreshToken,
                ActiveStatus = true,
                CreatedDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddDays(7)
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

        if (storedToken.ExpiredDate < DateTime.UtcNow)
            throw new UnauthorizedAccessException();

        storedToken.ActiveStatus = false;
        storedToken.RevokedDate = DateTime.UtcNow;

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
                CreatedDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddDays(7)
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