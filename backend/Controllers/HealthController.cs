using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("api/health")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthController> _logger;

    public HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger)
    {
        _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint - indicates if the service is running
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Readiness check endpoint - indicates if the service is ready to accept requests
    /// Includes database connection and other dependency checks
    /// </summary>
    [HttpGet("ready")]
    public async Task<ActionResult> Ready()
    {
        try
        {
            var healthReport = await _healthCheckService.CheckHealthAsync();
            
            var status = healthReport.Status == HealthStatus.Healthy ? "ready" : "not ready";
            var statusCode = healthReport.Status == HealthStatus.Healthy 
                ? HttpStatusCode.OK 
                : HttpStatusCode.ServiceUnavailable;

            return StatusCode((int)statusCode, new
            {
                status = status,
                timestamp = DateTime.UtcNow,
                checks = healthReport.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString().ToLower(),
                    description = e.Value.Description,
                    exception = e.Value.Exception?.Message
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during readiness check");
            return StatusCode((int)HttpStatusCode.ServiceUnavailable, new
            {
                status = "not ready",
                timestamp = DateTime.UtcNow,
                error = "Health check failed"
            });
        }
    }
}
