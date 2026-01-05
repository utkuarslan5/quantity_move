using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using System.Net;
using quantity_move_api.Services;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("health")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthController> _logger;
    private readonly IMetricsService? _metricsService;
    private static readonly DateTime _startTime = DateTime.UtcNow;

    public HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger, IMetricsService? metricsService = null)
    {
        _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
        _logger = logger;
        _metricsService = metricsService;
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

            var uptime = DateTime.UtcNow - _startTime;
            var metricsSummary = _metricsService != null ? GetMetricsSummary() : null;

            return StatusCode((int)statusCode, new
            {
                status = status,
                timestamp = DateTime.UtcNow,
                uptimeSeconds = uptime.TotalSeconds,
                checks = healthReport.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString().ToLower(),
                    description = e.Value.Description,
                    exception = e.Value.Exception?.Message
                }),
                metrics = metricsSummary
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

    /// <summary>
    /// Detailed health information endpoint with metrics summary
    /// </summary>
    [HttpGet("detailed")]
    [AllowAnonymous]
    public async Task<ActionResult> Detailed()
    {
        try
        {
            var healthReport = await _healthCheckService.CheckHealthAsync();
            var uptime = DateTime.UtcNow - _startTime;
            var process = Process.GetCurrentProcess();
            
            var metricsSnapshot = _metricsService?.GetSnapshot();
            var recentErrorCount = metricsSnapshot?.ErrorCounts.Values.Sum() ?? 0;

            return Ok(new
            {
                status = healthReport.Status.ToString(),
                timestamp = DateTime.UtcNow,
                uptime = new
                {
                    totalSeconds = uptime.TotalSeconds,
                    days = uptime.Days,
                    hours = uptime.Hours,
                    minutes = uptime.Minutes
                },
                process = new
                {
                    id = process.Id,
                    memoryMB = process.WorkingSet64 / 1024 / 1024,
                    threads = process.Threads.Count,
                    startTime = process.StartTime
                },
                checks = healthReport.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration.TotalMilliseconds,
                    exception = e.Value.Exception?.Message,
                    data = e.Value.Data
                }),
                metrics = metricsSnapshot != null ? new
                {
                    uptimeSeconds = metricsSnapshot.UptimeSeconds,
                    totalRequests = metricsSnapshot.RequestMetrics.Values.Sum(r => r.Count),
                    totalDatabaseOperations = metricsSnapshot.DatabaseMetrics.Values.Sum(d => d.Count),
                    totalBusinessOperations = metricsSnapshot.BusinessMetrics.Values.Sum(b => b.TotalCount),
                    recentErrorCount = recentErrorCount,
                    requestMetrics = metricsSnapshot.RequestMetrics.Take(10).ToDictionary(kvp => kvp.Key, kvp => new
                    {
                        count = kvp.Value.Count,
                        averageDurationMs = kvp.Value.AverageDurationMs,
                        minDurationMs = kvp.Value.MinDurationMs,
                        maxDurationMs = kvp.Value.MaxDurationMs
                    }),
                    databaseMetrics = metricsSnapshot.DatabaseMetrics.Take(10).ToDictionary(kvp => kvp.Key, kvp => new
                    {
                        count = kvp.Value.Count,
                        successCount = kvp.Value.SuccessCount,
                        successRate = kvp.Value.SuccessRate,
                        averageDurationMs = kvp.Value.AverageDurationMs
                    }),
                    errorCounts = metricsSnapshot.ErrorCounts
                } : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during detailed health check");
            return StatusCode((int)HttpStatusCode.ServiceUnavailable, new
            {
                status = "error",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }

    private object? GetMetricsSummary()
    {
        if (_metricsService == null)
            return null;

        var snapshot = _metricsService.GetSnapshot();
        return new
        {
            totalRequests = snapshot.RequestMetrics.Values.Sum(r => r.Count),
            totalErrors = snapshot.ErrorCounts.Values.Sum(),
            databaseOperations = snapshot.DatabaseMetrics.Values.Sum(d => d.Count)
        };
    }
}
