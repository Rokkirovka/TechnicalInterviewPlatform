using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

/// <summary>
/// 
/// </summary>
/// <param name="logger"></param>
public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger = logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        LogException(exception);
        
        var problemDetails = new ProblemDetails
        {
            Status = GetStatusCode(exception),
            Title = GetTitle(exception),
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
    
    private static int GetStatusCode(Exception exception) => exception switch
    {
        KeyNotFoundException => StatusCodes.Status404NotFound,
        ArgumentException => StatusCodes.Status400BadRequest,
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        InvalidOperationException => StatusCodes.Status400BadRequest,
        BusinessRuleConflictException => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetTitle(Exception exception) => exception switch
    {
        KeyNotFoundException => "Not Found",
        ArgumentException => "Bad Request",
        UnauthorizedAccessException => "Unauthorized",
        InvalidOperationException => "Bad Request",
        BusinessRuleConflictException => "Conflict", 
        _ => "Server Error"
    };

    private void LogException(Exception exception)
    {
        if (exception is KeyNotFoundException or InvalidOperationException or ArgumentException)
        {
            _logger.LogInformation(
                "Client error: {ExceptionType}: {Message}", 
                exception.GetType().Name, 
                exception.Message);
            return;
        }

        if (exception is BusinessRuleConflictException or UnauthorizedAccessException)
        {
            return;
        }

        _logger.LogError(exception, "Unhandled exception occurred");
    }
}

/// <summary>
/// 
/// </summary>
public static class CustomExceptionHandlerExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    public static void AddExceptionHandler(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<CustomExceptionHandler>();
        builder.Services.AddProblemDetails();
    }
}