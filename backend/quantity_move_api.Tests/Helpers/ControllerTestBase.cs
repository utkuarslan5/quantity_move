using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Controllers;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Helpers;

public abstract class ControllerTestBase
{
    protected Mock<ILogger<TController>> CreateMockLogger<TController>()
        where TController : ControllerBase
    {
        return TestHelpers.CreateMockLogger<TController>();
    }

    protected IConfiguration CreateTestConfiguration(Dictionary<string, string>? settings = null)
    {
        return TestHelpers.CreateTestConfiguration(settings);
    }

    protected void AddModelError(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState, string key, string errorMessage)
    {
        modelState.AddModelError(key, errorMessage);
    }
}

