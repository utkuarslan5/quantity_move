using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Controllers;
using quantity_move_api.Tests.Helpers;
using System.Security.Claims;

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

    protected void AddModelError(ModelStateDictionary modelState, string key, string errorMessage)
    {
        modelState.AddModelError(key, errorMessage);
    }

    protected void SetupHttpContext(ControllerBase controller, string? correlationId = null)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Correlation-ID"] = correlationId ?? Guid.NewGuid().ToString();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }
}

