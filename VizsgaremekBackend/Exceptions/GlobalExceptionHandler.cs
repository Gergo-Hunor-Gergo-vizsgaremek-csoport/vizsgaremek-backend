using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace VizsgaremekBackend.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = MapException(exception);

        if (exception is BadHttpRequestException)
        {
            logger.LogDebug(exception, "Bad Request");
            
        }
        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            NotLoggedInException => (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
            ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden", exception.Message),
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found", exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict", exception.Message),
            BadRequestException => (StatusCodes.Status400BadRequest, "Bad Request", exception.Message),
            BadHttpRequestException ex => (ex.StatusCode, "Bad Request", "The request could not be procesed."),
            DbUpdateConcurrencyException
                => (StatusCodes.Status409Conflict, "Conflict", "The resource was modified by another request."),
            DbUpdateException dbEx when GetPostgresSqlState(dbEx) == "23505"
                => (StatusCodes.Status409Conflict, "Conflict", "A resource with the same unique value already exists."),
            DbUpdateException dbEx when GetPostgresSqlState(dbEx) == "23503"
                => (StatusCodes.Status400BadRequest, "Bad Request", "The request references a related resource that does not exist."),
            DbUpdateException dbEx when GetPostgresSqlState(dbEx) == "23502"
                => (StatusCodes.Status400BadRequest, "Bad Request", "A required value is missing."),
#if !DEBUG
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
#endif
        };
    }

    private static string? GetPostgresSqlState(DbUpdateException ex)
    {
        // Npgsql surfaces constraint violations as PostgresException with a SqlState.
        Exception? inner = ex.InnerException;
        while (inner != null)
        {
            if (inner.GetType().Name == nameof(PostgresException))
            {
                return inner.GetType().GetProperty(nameof(PostgresException.SqlState))?.GetValue(inner) as string;
            }
            inner = inner.InnerException;
        }
        return null;
    }
}
