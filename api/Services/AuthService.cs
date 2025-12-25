using Dapper;
using System.Data;
using System.Data.SqlClient;
using quantity_move_api.Models;

namespace quantity_move_api.Services;

public interface IAuthService
{
    Task<User?> ValidateUserAsync(string username, string password);
}

public class AuthService : IAuthService
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IDatabaseService databaseService, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _databaseService = databaseService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        try
        {
            // Try to query user table directly
            // Common table names in Infor: user_mst, employees, users
            // Adjust based on actual database schema
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Connection string not found");
                return null;
            }

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Try multiple possible table/column combinations
            // This will need adjustment based on actual schema
            var queries = new[]
            {
                // Option 1: user_mst table (common in Infor)
                @"SELECT TOP 1 
                    user_id AS UserId,
                    user_name AS Username,
                    password AS Password,
                    full_name AS FullName,
                    email AS Email
                  FROM user_mst
                  WHERE user_name = @Username AND password = @Password",
                
                // Option 2: employees table
                @"SELECT TOP 1 
                    emp_id AS UserId,
                    emp_name AS Username,
                    password AS Password,
                    full_name AS FullName,
                    email AS Email
                  FROM employees
                  WHERE emp_name = @Username AND password = @Password",
                
                // Option 3: users table
                @"SELECT TOP 1 
                    id AS UserId,
                    username AS Username,
                    password AS Password,
                    full_name AS FullName,
                    email AS Email
                  FROM users
                  WHERE username = @Username AND password = @Password"
            };

            foreach (var query in queries)
            {
                try
                {
                    var user = await connection.QueryFirstOrDefaultAsync<User>(
                        query,
                        new { Username = username, Password = password }
                    );

                    if (user != null)
                    {
                        _logger.LogInformation("User {Username} validated successfully", username);
                        return user;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Query failed, trying next option");
                    // Continue to next query
                }
            }

            _logger.LogWarning("User {Username} not found or invalid password", username);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user {Username}", username);
            return null;
        }
    }
}

