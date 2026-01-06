using Dapper;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using quantity_move_api.Common;

namespace quantity_move_api.Services;

public interface IDatabaseService
{
    Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null);
    Task<T?> ExecuteStoredProcedureSingleAsync<T>(string procedureName, object? parameters = null);
    Task<int> ExecuteStoredProcedureNonQueryAsync(string procedureName, object? parameters = null);
    Task<IDictionary<string, object>> ExecuteStoredProcedureWithOutputAsync(string procedureName, object? parameters = null);
    
    // Query execution methods
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null);
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null);
    Task<T> QuerySingleAsync<T>(string sql, object? parameters = null);
    Task<int> ExecuteAsync(string sql, object? parameters = null);
}

public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseService> _logger;
    private readonly IMetricsService? _metricsService;

    public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger, IMetricsService? metricsService = null)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
        _metricsService = metricsService;
    }

    private SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    /// <summary>
    /// Executes a database operation with metrics tracking, logging, and error handling.
    /// This is a helper method that encapsulates the common pattern used by all database operations.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the operation</typeparam>
    /// <param name="operation">The database operation to execute</param>
    /// <param name="operationName">The name of the operation for logging and metrics</param>
    /// <param name="sqlPreview">Optional SQL preview for logging (truncated if too long)</param>
    /// <returns>The result of the database operation</returns>
    private async Task<TResult> ExecuteWithMetricsAsync<TResult>(
        Func<SqlConnection, Task<TResult>> operation,
        string operationName,
        string? sqlPreview = null)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync().ConfigureAwait(false);
            var result = await operation(connection).ConfigureAwait(false);
            
            stopwatch.Stop();
            var durationMs = stopwatch.ElapsedMilliseconds;
            
            _logger.LogDebug(
                string.IsNullOrEmpty(sqlPreview)
                    ? "{OperationName} executed successfully in {Duration}ms"
                    : "{OperationName} executed successfully in {Duration}ms. SQL: {Sql}",
                operationName,
                durationMs,
                sqlPreview ?? string.Empty);
            
            _metricsService?.RecordDatabaseOperation(operationName, durationMs, true);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            var durationMs = stopwatch.ElapsedMilliseconds;
            
            _metricsService?.RecordDatabaseOperation(operationName, durationMs, false);
            _metricsService?.IncrementError($"DatabaseError_{operationName}");
            
            _logger.LogDatabaseException(ex, operationName, _connectionString);
            _logger.LogError(ex,
                string.IsNullOrEmpty(sqlPreview)
                    ? "Error executing {OperationName} after {Duration}ms"
                    : "Error executing {OperationName} after {Duration}ms. SQL: {Sql}",
                operationName,
                durationMs,
                sqlPreview ?? string.Empty);
            throw;
        }
    }

    /// <summary>
    /// Executes a database operation with metrics tracking, logging, and error handling.
    /// Includes parameter logging for operations that accept parameters.
    /// This is a helper method that encapsulates the common pattern used by all database operations.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the operation</typeparam>
    /// <param name="operation">The database operation to execute</param>
    /// <param name="operationName">The name of the operation for logging and metrics</param>
    /// <param name="parameters">The parameters for the operation (will be sanitized in logs)</param>
    /// <param name="sqlPreview">Optional SQL preview for logging (truncated if too long)</param>
    /// <returns>The result of the database operation</returns>
    private async Task<TResult> ExecuteWithMetricsAsync<TResult>(
        Func<SqlConnection, Task<TResult>> operation,
        string operationName,
        object? parameters,
        string? sqlPreview = null)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync().ConfigureAwait(false);
            var result = await operation(connection).ConfigureAwait(false);
            
            stopwatch.Stop();
            var durationMs = stopwatch.ElapsedMilliseconds;
            
            _logger.LogDebug(
                string.IsNullOrEmpty(sqlPreview)
                    ? "{OperationName} executed successfully in {Duration}ms"
                    : "{OperationName} executed successfully in {Duration}ms. SQL: {Sql}",
                operationName,
                durationMs,
                sqlPreview ?? string.Empty);
            
            _metricsService?.RecordDatabaseOperation(operationName, durationMs, true);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            var durationMs = stopwatch.ElapsedMilliseconds;
            
            _metricsService?.RecordDatabaseOperation(operationName, durationMs, false);
            _metricsService?.IncrementError($"DatabaseError_{operationName}");
            
            _logger.LogDatabaseException(ex, operationName, _connectionString);
            _logger.LogError(ex,
                "Error executing {OperationName} after {Duration}ms. Parameters: {Parameters}",
                operationName,
                durationMs,
                LoggingExtensions.SanitizeParameters(parameters));
            throw;
        }
    }

    public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null)
    {
        return await ExecuteWithMetricsAsync(
            async connection => await connection.QueryAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false),
            procedureName,
            parameters).ConfigureAwait(false);
    }

    public async Task<T?> ExecuteStoredProcedureSingleAsync<T>(string procedureName, object? parameters = null)
    {
        return await ExecuteWithMetricsAsync(
            async connection => await connection.QueryFirstOrDefaultAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false),
            procedureName,
            parameters).ConfigureAwait(false);
    }

    public async Task<int> ExecuteStoredProcedureNonQueryAsync(string procedureName, object? parameters = null)
    {
        return await ExecuteWithMetricsAsync(
            async connection =>
            {
                // Handle DynamicParameters for output parameters
                if (parameters is Dapper.SqlMapper.IDynamicParameters dynamicParams)
                {
                    await connection.ExecuteAsync(procedureName, dynamicParams, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                    return 1; // Success
                }
                else
                {
                    return await connection.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                }
            },
            procedureName,
            parameters).ConfigureAwait(false);
    }

    public async Task<IDictionary<string, object>> ExecuteStoredProcedureWithOutputAsync(string procedureName, object? parameters = null)
    {
        return await ExecuteWithMetricsAsync(
            async connection =>
            {
                var dynamicParameters = new DynamicParameters(parameters);
                await connection.ExecuteAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                
                var result = new Dictionary<string, object>();
                foreach (var paramName in dynamicParameters.ParameterNames)
                {
                    if (paramName.StartsWith("@"))
                    {
                        // Safely retrieve parameter value with null checking
                        try
                        {
                            var paramValue = dynamicParameters.Get<object>(paramName);
                            result[paramName] = paramValue ?? string.Empty;
                        }
                        catch
                        {
                            // If parameter retrieval fails, use empty string
                            result[paramName] = string.Empty;
                        }
                    }
                }
                
                return result;
            },
            procedureName,
            parameters).ConfigureAwait(false);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        var sqlPreview = sql.Length > 200 ? sql.Substring(0, 200) + "..." : sql;
        return await ExecuteWithMetricsAsync(
            async connection => await connection.QueryAsync<T>(sql, parameters).ConfigureAwait(false),
            "Query",
            parameters,
            sqlPreview).ConfigureAwait(false);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        var sqlPreview = sql.Length > 200 ? sql.Substring(0, 200) + "..." : sql;
        return await ExecuteWithMetricsAsync(
            async connection => await connection.QueryFirstOrDefaultAsync<T>(sql, parameters).ConfigureAwait(false),
            "Query",
            parameters,
            sqlPreview).ConfigureAwait(false);
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        var sqlPreview = sql.Length > 200 ? sql.Substring(0, 200) + "..." : sql;
        return await ExecuteWithMetricsAsync(
            async connection => await connection.QuerySingleAsync<T>(sql, parameters).ConfigureAwait(false),
            "Query",
            parameters,
            sqlPreview).ConfigureAwait(false);
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        var sqlPreview = sql.Length > 200 ? sql.Substring(0, 200) + "..." : sql;
        return await ExecuteWithMetricsAsync(
            async connection => await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false),
            "Query",
            parameters,
            sqlPreview).ConfigureAwait(false);
    }
}
