using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using System.Net;
using System.Text.Json;

namespace quantity_move_api.Controllers;

/// <summary>
/// Controller for handling application-level exceptions via UseExceptionHandler middleware.
/// This endpoint is called when an unhandled exception occurs and is not caught by ExceptionHandlingMiddleware.
/// </summary>
[ApiController]
[Route("error")]
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles application-level exceptions that bypass the ExceptionHandlingMiddleware.
    /// This is typically called for exceptions that occur during request pipeline setup,
    /// model binding, or other early-stage processing.
    /// </summary>
    [HttpGet]
    [HttpPost]
    [HttpPut]
    [HttpDelete]
    [HttpPatch]
    [HttpHead]
    [HttpOptions]
    public IActionResult Error()
    {
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        if (exception == null)
        {
            // No exception found, return generic error
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                ApiResponse<object>.ErrorResponse(
                    "An error occurred while processing your request.",
                    new List<string> { "No exception details available" }));
        }

        // Log the exception
        _logger.LogError(exception,
            "Application-level exception occurred. Path: {Path}, Method: {Method}, ExceptionType: {ExceptionType}",
            exceptionHandlerPathFeature?.Path ?? "Unknown",
            HttpContext.Request.Method,
            exception.GetType().Name);

        // Determine status code based on exception type
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An error occurred while processing your request.";

        if (exception is UnauthorizedAccessException)
        {
            statusCode = HttpStatusCode.Unauthorized;
            message = "Unauthorized access.";
        }
        else if (exception is ArgumentException || exception is ArgumentNullException)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = exception.Message;
        }
        else if (exception is KeyNotFoundException)
        {
            statusCode = HttpStatusCode.NotFound;
            message = exception.Message;
        }
        else if (exception is InvalidOperationException)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = exception.Message;
        }

        var errors = new List<string> { exception.Message };
        if (exception.InnerException != null)
        {
            errors.Add($"Inner exception: {exception.InnerException.Message}");
        }

        return StatusCode(
            (int)statusCode,
            ApiResponse<object>.ErrorResponse(message, errors));
    }
}

