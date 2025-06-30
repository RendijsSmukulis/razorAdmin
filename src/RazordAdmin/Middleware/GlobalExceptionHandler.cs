using System.Net;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using RazorAdmin.Models;

namespace RazorAdmin.Middleware;

/// <summary>
/// Global exception handling middleware for consistent error responses.
/// </summary>
public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        context.Response.ContentType = "application/json";
        
        var response = new ApiResponse<object>
        {
            Success = false,
            Message = _environment.IsDevelopment() 
                ? exception.Message 
                : "An error occurred while processing your request."
        };

        if (_environment.IsDevelopment())
        {
            response.Errors = new List<string> 
            { 
                exception.StackTrace ?? "No stack trace available" 
            };
        }

        switch (exception)
        {
            case ValidationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
} 