using Dapper;
using quantity_move_api.Common.Constants;
using quantity_move_api.Models;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Query;
using System.Text.Json;

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

            var allQueriesThrewExceptions = true;
            var anyQueryReturnedNull = false;
            Exception? criticalException = null;
            var allExceptionsWereCritical = true;
            // #region agent log
            System.IO.File.AppendAllText("/home/r00t/code/ekip/quantity_move/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { id = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), location = "AuthService.cs:73", message = "Variables initialized", data = new { anyQueryReturnedNull = anyQueryReturnedNull, allQueriesThrewExceptions = allQueriesThrewExceptions, username = username }, sessionId = "debug-session", runId = "run1", hypothesisId = "E" }) + "\n");
            // #endregion

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
                    allQueriesThrewExceptions = false;
                    anyQueryReturnedNull = true;
                    // #region agent log
                    System.IO.File.AppendAllText("/home/r00t/code/ekip/quantity_move/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { id = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), location = "AuthService.cs:89", message = "anyQueryReturnedNull set to true", data = new { anyQueryReturnedNull = true, allQueriesThrewExceptions = allQueriesThrewExceptions, queryIndex = Array.IndexOf(queries, query) }, sessionId = "debug-session", runId = "run1", hypothesisId = "A" }) + "\n");
                    // #endregion
                }
                catch (Exception ex)
                {
                    if (ex is InvalidOperationException)
                    {
                        criticalException = ex;
                    }
                    else
                    {
                        allExceptionsWereCritical = false;
                    }
                    Logger.LogDebug(ex, "Query failed, trying next option");
                    // Continue to next query
                }
            }

            // If all queries threw exceptions and they were all critical (InvalidOperationException), re-throw the last one
            if (allQueriesThrewExceptions && allExceptionsWereCritical && criticalException != null)
            {
                throw criticalException;
            }

            // Only log warning if queries returned null (not if they all threw exceptions)
            // #region agent log
            System.IO.File.AppendAllText("/home/r00t/code/ekip/quantity_move/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { id = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), location = "AuthService.cs:113", message = "Before condition check", data = new { anyQueryReturnedNull = anyQueryReturnedNull, allQueriesThrewExceptions = allQueriesThrewExceptions, conditionValue = !allQueriesThrewExceptions, wouldUseAnyQueryReturnedNull = anyQueryReturnedNull }, sessionId = "debug-session", runId = "run1", hypothesisId = "A" }) + "\n");
            // #endregion
            if (!allQueriesThrewExceptions)
            {
                Logger.LogWarning("User {Username} not found or invalid password", username);
                // #region agent log
                System.IO.File.AppendAllText("/home/r00t/code/ekip/quantity_move/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { id = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), location = "AuthService.cs:115", message = "Warning logged", data = new { anyQueryReturnedNull = anyQueryReturnedNull, allQueriesThrewExceptions = allQueriesThrewExceptions }, sessionId = "debug-session", runId = "run1", hypothesisId = "A" }) + "\n");
                // #endregion
            }
            else
            {
                // #region agent log
                System.IO.File.AppendAllText("/home/r00t/code/ekip/quantity_move/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { id = $"log_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), location = "AuthService.cs:122", message = "Warning NOT logged - all queries threw exceptions", data = new { anyQueryReturnedNull = anyQueryReturnedNull, allQueriesThrewExceptions = allQueriesThrewExceptions }, sessionId = "debug-session", runId = "run1", hypothesisId = "B" }) + "\n");
                // #endregion
            }
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating user {Username}", username);
            return null;
        }
    }
}
