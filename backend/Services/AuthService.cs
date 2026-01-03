using Dapper;
using quantity_move_api.Common.Constants;
using quantity_move_api.Models;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Query;

namespace quantity_move_api.Services;

public interface IAuthService
{
    Task<User?> ValidateUserAsync(string username, string password);
}

public class AuthService : BaseService<AuthService>, IAuthService
{
    private readonly IQueryService _queryService;

    public AuthService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IQueryService queryService,
        ILogger<AuthService> logger)
        : base(databaseService, configurationService, logger)
    {
        _queryService = queryService;
    }

    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        try
        {
            Logger.LogInformation("Validating user {Username}", username);

            // Try multiple possible table/column combinations
            // This will need adjustment based on actual schema
            var queries = new[]
            {
                // Option 1: user_mst table (common in Infor)
                $@"SELECT TOP 1 
                    user_id AS UserId,
                    user_name AS Username,
                    password AS Password,
                    full_name AS FullName,
                    email AS Email
                  FROM {TableNames.UserMaster}
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
                    var user = await _queryService.QueryFirstOrDefaultAsync<User>(
                        query,
                        new { Username = username, Password = password }
                    ).ConfigureAwait(false);

                    if (user != null)
                    {
                        Logger.LogInformation("User {Username} validated successfully", username);
                        return user;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex, "Query failed, trying next option");
                    // Continue to next query
                }
            }

            Logger.LogWarning("User {Username} not found or invalid password", username);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating user {Username}", username);
            return null;
        }
    }
}