using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.Data.SqlClient;
using quantity_move_api.Services;

namespace quantity_move_api.Tests.Integration;

/// <summary>
/// Base class for integration tests that use real database connections.
/// Loads connection string from appsettings.Test.json or environment variables.
/// </summary>
public abstract class DatabaseIntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly string ConnectionString;
    protected readonly IDatabaseService DatabaseService;
    protected readonly IServiceScope ServiceScope;
    private bool _disposed = false;

    protected DatabaseIntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Load test configuration
                config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: false);
                config.AddEnvironmentVariables("TEST_");
                
                // Override with environment variables if provided
                var testConnectionString = Environment.GetEnvironmentVariable("TEST_CONNECTION_STRING");
                if (!string.IsNullOrWhiteSpace(testConnectionString))
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { "ConnectionStrings:DefaultConnection", testConnectionString }
                    });
                }
            });
        });

        Client = Factory.CreateClient();
        Client.BaseAddress = new Uri(Client.BaseAddress!, "/api/");

        // Get connection string from configuration
        var configuration = Factory.Services.GetRequiredService<IConfiguration>();
        ConnectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Test database connection string not configured. Set TEST_CONNECTION_STRING environment variable or configure in appsettings.Test.json");

        // Get database service for test data operations
        ServiceScope = Factory.Services.CreateScope();
        DatabaseService = ServiceScope.ServiceProvider.GetRequiredService<IDatabaseService>();
    }

    /// <summary>
    /// Executes a SQL command that doesn't return results (INSERT, UPDATE, DELETE)
    /// </summary>
    protected async Task<int> ExecuteNonQueryAsync(string sql, object? parameters = null)
    {
        return await DatabaseService.ExecuteAsync(sql, parameters);
    }

    /// <summary>
    /// Executes a SQL query and returns results
    /// </summary>
    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        return await DatabaseService.QueryAsync<T>(sql, parameters);
    }

    /// <summary>
    /// Executes a SQL query and returns a single result
    /// </summary>
    protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        return await DatabaseService.QueryFirstOrDefaultAsync<T>(sql, parameters);
    }

    /// <summary>
    /// Tests database connectivity
    /// </summary>
    protected async Task<bool> TestDatabaseConnectionAsync()
    {
        try
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            return connection.State == ConnectionState.Open;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Verifies that the database connection is available before running tests
    /// </summary>
    protected async Task EnsureDatabaseConnectionAsync()
    {
        var isConnected = await TestDatabaseConnectionAsync();
        if (!isConnected)
        {
            throw new InvalidOperationException(
                "Database connection failed. Ensure the test database is accessible. " +
                "Set TEST_CONNECTION_STRING environment variable or configure in appsettings.Test.json");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                ServiceScope?.Dispose();
                Client?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

