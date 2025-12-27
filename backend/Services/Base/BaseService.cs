using quantity_move_api.Services;

namespace quantity_move_api.Services.Base;

public abstract class BaseService<T>
{
    protected readonly IDatabaseService DatabaseService;
    protected readonly IConfigurationService ConfigurationService;
    protected readonly ILogger<T> Logger;

    protected BaseService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<T> logger)
    {
        DatabaseService = databaseService;
        ConfigurationService = configurationService;
        Logger = logger;
    }

    protected string GetDefaultWarehouse(string? provided) => 
        !string.IsNullOrWhiteSpace(provided) ? provided : ConfigurationService.GetDefaultWarehouse();
    
    protected string GetDefaultSite(string? provided) => 
        !string.IsNullOrWhiteSpace(provided) ? provided : ConfigurationService.GetDefaultSite();
}

