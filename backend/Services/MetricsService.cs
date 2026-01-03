using System.Collections.Concurrent;

namespace quantity_move_api.Services;

/// <summary>
/// In-memory metrics collection service for monitoring
/// Thread-safe counters and histograms
/// </summary>
public interface IMetricsService
{
    void RecordRequest(string endpoint, int statusCode, long durationMs);
    void RecordDatabaseOperation(string procedureName, long durationMs, bool success);
    void RecordBusinessOperation(string operationType, bool success);
    void IncrementError(string errorType);
    MetricsSnapshot GetSnapshot();
    void Reset();
}

public class MetricsService : IMetricsService
{
    private readonly ConcurrentDictionary<string, RequestMetrics> _requestMetrics = new();
    private readonly ConcurrentDictionary<string, DatabaseMetrics> _databaseMetrics = new();
    private readonly ConcurrentDictionary<string, BusinessOperationMetrics> _businessMetrics = new();
    private readonly ConcurrentDictionary<string, long> _errorCounts = new();
    private readonly DateTime _startTime = DateTime.UtcNow;

    public void RecordRequest(string endpoint, int statusCode, long durationMs)
    {
        var key = $"{endpoint}:{statusCode}";
        _requestMetrics.AddOrUpdate(key,
            new RequestMetrics { Count = 1, TotalDurationMs = durationMs, MinDurationMs = durationMs, MaxDurationMs = durationMs },
            (k, existing) => existing with
            {
                Count = existing.Count + 1,
                TotalDurationMs = existing.TotalDurationMs + durationMs,
                MinDurationMs = Math.Min(existing.MinDurationMs, durationMs),
                MaxDurationMs = Math.Max(existing.MaxDurationMs, durationMs)
            });
    }

    public void RecordDatabaseOperation(string procedureName, long durationMs, bool success)
    {
        _databaseMetrics.AddOrUpdate(procedureName,
            new DatabaseMetrics { Count = 1, TotalDurationMs = durationMs, SuccessCount = success ? 1 : 0, MinDurationMs = durationMs, MaxDurationMs = durationMs },
            (k, existing) => existing with
            {
                Count = existing.Count + 1,
                TotalDurationMs = existing.TotalDurationMs + durationMs,
                SuccessCount = success ? existing.SuccessCount + 1 : existing.SuccessCount,
                MinDurationMs = Math.Min(existing.MinDurationMs, durationMs),
                MaxDurationMs = Math.Max(existing.MaxDurationMs, durationMs)
            });
    }

    public void RecordBusinessOperation(string operationType, bool success)
    {
        _businessMetrics.AddOrUpdate(operationType,
            new BusinessOperationMetrics { TotalCount = 1, SuccessCount = success ? 1 : 0 },
            (k, existing) => existing with
            {
                TotalCount = existing.TotalCount + 1,
                SuccessCount = success ? existing.SuccessCount + 1 : existing.SuccessCount
            });
    }

    public void IncrementError(string errorType)
    {
        _errorCounts.AddOrUpdate(errorType, 1, (k, v) => v + 1);
    }

    public MetricsSnapshot GetSnapshot()
    {
        return new MetricsSnapshot
        {
            UptimeSeconds = (DateTime.UtcNow - _startTime).TotalSeconds,
            RequestMetrics = _requestMetrics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            DatabaseMetrics = _databaseMetrics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            BusinessMetrics = _businessMetrics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            ErrorCounts = _errorCounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        };
    }

    public void Reset()
    {
        _requestMetrics.Clear();
        _databaseMetrics.Clear();
        _businessMetrics.Clear();
        _errorCounts.Clear();
    }
}

/// <summary>
/// Immutable metrics for request tracking
/// </summary>
public record RequestMetrics
{
    public long Count { get; init; }
    public long TotalDurationMs { get; init; }
    public long MinDurationMs { get; init; }
    public long MaxDurationMs { get; init; }
    public double AverageDurationMs => Count > 0 ? (double)TotalDurationMs / Count : 0;
}

/// <summary>
/// Immutable metrics for database operation tracking
/// </summary>
public record DatabaseMetrics
{
    public long Count { get; init; }
    public long SuccessCount { get; init; }
    public long TotalDurationMs { get; init; }
    public long MinDurationMs { get; init; }
    public long MaxDurationMs { get; init; }
    public double AverageDurationMs => Count > 0 ? (double)TotalDurationMs / Count : 0;
    public double SuccessRate => Count > 0 ? (double)SuccessCount / Count : 0;
}

/// <summary>
/// Immutable metrics for business operation tracking
/// </summary>
public record BusinessOperationMetrics
{
    public long TotalCount { get; init; }
    public long SuccessCount { get; init; }
    public double SuccessRate => TotalCount > 0 ? (double)SuccessCount / TotalCount : 0;
}

public class MetricsSnapshot
{
    public double UptimeSeconds { get; set; }
    public Dictionary<string, RequestMetrics> RequestMetrics { get; set; } = new();
    public Dictionary<string, DatabaseMetrics> DatabaseMetrics { get; set; } = new();
    public Dictionary<string, BusinessOperationMetrics> BusinessMetrics { get; set; } = new();
    public Dictionary<string, long> ErrorCounts { get; set; } = new();
}

