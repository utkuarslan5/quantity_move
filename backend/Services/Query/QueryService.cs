using quantity_move_api.Services;

namespace quantity_move_api.Services.Query;

public class QueryService : IQueryService
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<QueryService> _logger;

    public QueryService(IDatabaseService databaseService, ILogger<QueryService> logger)
    {
        _databaseService = databaseService;
        _logger = logger;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        return await _databaseService.QueryAsync<T>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        return await _databaseService.QueryFirstOrDefaultAsync<T>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        return await _databaseService.QuerySingleAsync<T>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        return await _databaseService.ExecuteAsync(sql, parameters).ConfigureAwait(false);
    }
}

