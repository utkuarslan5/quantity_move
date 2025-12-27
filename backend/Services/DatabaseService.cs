using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;

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

    public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }

    private SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object? parameters = null)
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();
            return await connection.QueryAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure {ProcedureName}", procedureName);
            throw;
        }
    }

    public async Task<T?> ExecuteStoredProcedureSingleAsync<T>(string procedureName, object? parameters = null)
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync().ConfigureAwait(false);
            return await connection.QueryFirstOrDefaultAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure {ProcedureName}", procedureName);
            throw;
        }
    }

    public async Task<int> ExecuteStoredProcedureNonQueryAsync(string procedureName, object? parameters = null)
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync().ConfigureAwait(false);
            
            // Handle DynamicParameters for output parameters
            if (parameters is Dapper.SqlMapper.IDynamicParameters dynamicParams)
            {
                await connection.ExecuteAsync(procedureName, dynamicParams, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                return 1; // Success
            }
            
            return await connection.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure {ProcedureName}", procedureName);
            throw;
        }
    }

    public async Task<IDictionary<string, object>> ExecuteStoredProcedureWithOutputAsync(string procedureName, object? parameters = null)
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync().ConfigureAwait(false);
            
            var dynamicParameters = new DynamicParameters(parameters);
            await connection.ExecuteAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            
            var result = new Dictionary<string, object>();
            foreach (var paramName in dynamicParameters.ParameterNames)
            {
                if (paramName.StartsWith("@"))
                {
                    result[paramName] = dynamicParameters.Get<object>(paramName) ?? string.Empty;
                }
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure {ProcedureName}", procedureName);
            throw;
        }
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync().ConfigureAwait(false);
            return await connection.QueryAsync<T>(sql, parameters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query: {Sql}", sql);
            throw;
        }
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync().ConfigureAwait(false);
            return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query: {Sql}", sql);
            throw;
        }
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync().ConfigureAwait(false);
            return await connection.QuerySingleAsync<T>(sql, parameters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query: {Sql}", sql);
            throw;
        }
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        try
        {
            using var connection = CreateConnection();
            await connection.OpenAsync().ConfigureAwait(false);
            return await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query: {Sql}", sql);
            throw;
        }
    }
}
