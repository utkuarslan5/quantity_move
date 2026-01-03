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

            // Query TRM_EDIUSER table (legacy authentication table)
            var userQuery = $@"SELECT TOP 1 
                Kullanici AS Username,
                Sifre AS Password,
                Ambar AS Warehouse
              FROM {TableNames.UserMaster}
              WHERE Kullanici = @Username AND Sifre = @Password";

            var user = await _queryService.QueryFirstOrDefaultAsync<User>(
                userQuery,
                new { Username = username, Password = password }
            ).ConfigureAwait(false);

            if (user == null)
            {
                Logger.LogWarning("User {Username} not found or invalid password", username);
                return null;
            }

            // Validate employee exists (matches legacy app behavior)
            var defaultSite = ConfigurationService.GetDefaultSite();
            var siteRef = string.IsNullOrEmpty(defaultSite) || defaultSite == "Default" ? "faz" : defaultSite;
            var employeeQuery = @"SELECT TOP 1 
                emp_num,
                site_Ref,
                dept,
                full_name
              FROM employee_mst
              WHERE LTRIM(emp_num) = @Username AND site_Ref = @SiteRef";

            var employee = await _queryService.QueryFirstOrDefaultAsync<dynamic>(
                employeeQuery,
                new { Username = username, SiteRef = siteRef }
            ).ConfigureAwait(false);

            if (employee == null)
            {
                Logger.LogWarning("User {Username} employee record not found", username);
                // Still return user but log warning (legacy app shows error but we'll be more lenient)
            }
            else
            {
                // Set full name from employee record if available
                if (user.FullName == null && employee.full_name != null)
                {
                    user.FullName = employee.full_name.ToString();
                }
            }

            // Set UserId from username (since TRM_EDIUSER doesn't have a numeric ID)
            // Use FNV-1a hash algorithm for deterministic, collision-resistant integer ID
            user.UserId = ComputeUserIdHash(username);

            Logger.LogInformation("User {Username} validated successfully", username);
            return user;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating user {Username}", username);
            return null;
        }
    }

    /// <summary>
    /// Computes a deterministic integer hash from a username using FNV-1a algorithm.
    /// This provides better collision resistance than GetHashCode() while maintaining determinism.
    /// The hash is guaranteed to be a positive integer (using bitwise AND with 0x7FFFFFFF).
    /// </summary>
    /// <param name="username">The username to hash</param>
    /// <returns>A deterministic positive integer hash of the username</returns>
    private static int ComputeUserIdHash(string username)
    {
        if (string.IsNullOrEmpty(username))
            return 0;

        const uint FNV_OFFSET_BASIS = 2166136261u;
        const uint FNV_PRIME = 16777619u;
        
        uint hash = FNV_OFFSET_BASIS;
        foreach (byte b in System.Text.Encoding.UTF8.GetBytes(username))
        {
            hash ^= b;
            hash *= FNV_PRIME;
        }
        
        // Convert to signed int, ensuring positive value
        return (int)(hash & 0x7FFFFFFF);
    }
}