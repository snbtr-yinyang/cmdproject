using FluentValidation;
using CMDProject.Domain.Common;
using System.Net;
using System.Text.Json;

namespace CMDProject.API.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation error occurred.");
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ApiResponse<object>.FailResponse("Validation failed", ex.Errors.Select(x => x.ErrorMessage)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred.");
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, ApiResponse<object>.FailResponse("An unexpected error occurred"));
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, ApiResponse<object> response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
