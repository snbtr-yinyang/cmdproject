using CMDProject.API.Security;
using CMDProject.Application.Interfaces.Repository;
using CMDProject.Application.Services;
using CMDProject.Infrastructure.Persistence.DBContext;
using CMDProject.Infrastructure.Persistence.Repositories;
using CMDProject.Infrastructure.Security;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace CMDProject.API.Extensions;

public static class ServiceCollectionExtensions
{
    public const string ApiJwtScheme = "ApiJwt";

    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "CMDProject API",
                    Version = "v1"
                });

            options.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "Input JWT Bearer token"
                });

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference =
                                new OpenApiReference
                                {
                                    Type =
                                        ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                        },
                        Array.Empty<string>()
                    }
                });
        });

        services.AddFluentValidationAutoValidation();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString(
                    "DefaultConnection")));

        /*
         * ============================
         * SERVICES
         * ============================
         */

        services.AddScoped<IJwtService,JwtTokenService>();
        services.AddScoped<IAuthService,AuthService>();
        services.AddScoped<IRefreshTokenRepository,TokenRepository>();

        /*
         * GOOGLE LOGIN
         */

        services.AddSingleton<IGoogleIdTokenValidator,GoogleIdTokenValidator>();

        /*
         * JWT CONFIGURATION
         */

        var jwtSettings =
            configuration.GetSection("Jwt");

        var key =
            jwtSettings["Key"]
            ?? throw new InvalidOperationException(
                "JWT Key is not configured.");

        services.AddAuthentication()
            .AddJwtBearer(ApiJwtScheme,
                options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer =
                                jwtSettings["Issuer"],

                            ValidAudience =
                                jwtSettings["Audience"],

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(key)),

                            ClockSkew =
                                TimeSpan.Zero
                        };
                })
            .AddMicrosoftEntraBearer(configuration);

        services.AddAuthorization();

        return services;
    }
}