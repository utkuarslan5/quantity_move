using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Services;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("api/metrics")]
[Authorize]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _metricsService;
    private readonly ILogger<MetricsController> _logger;

    public MetricsController(IMetricsService metricsService, ILogger<MetricsController> logger)
    {
        _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
        _logger = logger;
    }

    /// <summary>
    /// Get current metrics snapshot
    /// Returns request statistics, database performance, business operation stats, and error counts
    /// </summary>
    [HttpGet]
    public ActionResult<MetricsSnapshot> GetMetrics()
    {
        try
        {
            var snapshot = _metricsService.GetSnapshot();
            return Ok(snapshot);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metrics");
            return StatusCode(500, new { error = "Failed to retrieve metrics" });
        }
    }

    /// <summary>
    /// Reset all metrics counters
    /// </summary>
    [HttpPost("reset")]
    public IActionResult ResetMetrics()
    {
        try
        {
            _metricsService.Reset();
            _logger.LogInformation("Metrics reset requested by user {Username}", User?.Identity?.Name ?? "Unknown");
            return Ok(new { message = "Metrics reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting metrics");
            return StatusCode(500, new { error = "Failed to reset metrics" });
        }
    }
}

