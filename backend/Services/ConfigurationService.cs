namespace quantity_move_api.Services;

public interface IConfigurationService
{
    string GetDefaultWarehouse();
    string GetDefaultSite();
    bool GetQualityControlEnabled();
    string GetConnectionString();
    string GetStoredProcedureName(string key);
    string GetTableName(string key);
}

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;

    public ConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetDefaultWarehouse()
    {
        return _configuration["Defaults:DefaultWarehouse"] ?? "MAIN";
    }

    public string GetDefaultSite()
    {
        return _configuration["Defaults:DefaultSite"] ?? "Default";
    }

    public bool GetQualityControlEnabled()
    {
        var value = _configuration["Defaults:QualityControlEnabled"];
        return value != null && bool.TryParse(value, out var result) && result;
    }

    public string GetConnectionString()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string 'DefaultConnection' not found.");
        }
        return connectionString;
    }

    public string GetStoredProcedureName(string key)
    {
        var procedureName = _configuration[$"StoredProcedures:{key}"];
        if (string.IsNullOrEmpty(procedureName))
        {
            throw new InvalidOperationException($"Stored procedure name for key '{key}' not found in configuration.");
        }
        return procedureName;
    }

    public string GetTableName(string key)
    {
        var tableName = _configuration[$"Database:Tables:{key}"];
        if (string.IsNullOrEmpty(tableName))
        {
            throw new InvalidOperationException($"Table name for key '{key}' not found in configuration.");
        }
        return tableName;
    }
}

