using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Controllers;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Tests.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace quantity_move_api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly IConfiguration _configuration;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        
        // Use real IConfiguration instead of mock for extension methods
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Jwt:SecretKey", "TestSecretKeyForJwtTokenGeneration12345678901234567890" },
            { "Jwt:Issuer", "test-issuer" },
            { "Jwt:Audience", "test-audience" },
            { "Jwt:ExpirationInHours", "24" }
        });
        _configuration = configBuilder.Build();
        _controller = new AuthController(_mockAuthService.Object, _configuration, _mockLogger.Object);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var request = TestHelpers.CreateLoginRequest();
        var user = TestHelpers.CreateTestUser();
        _mockAuthService.Setup(x => x.ValidateUserAsync(request.Username, request.Password))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<ApiResponse<LoginResponse>>();
        
        var response = okResult.Value as ApiResponse<LoginResponse>;
        response!.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.Token.Should().NotBeEmpty();
        response.Data.User.Should().NotBeNull();
        response.Data.User!.UserId.Should().Be(user.UserId);
        response.Data.User.Username.Should().Be(user.Username);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = TestHelpers.CreateLoginRequest();
        _mockAuthService.Setup(x => x.ValidateUserAsync(request.Username, request.Password))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        unauthorizedResult!.Value.Should().BeOfType<ApiResponse<LoginResponse>>();
        
        var response = unauthorizedResult.Value as ApiResponse<LoginResponse>;
        response!.Success.Should().BeFalse();
        response.Message.Should().Contain("Invalid username or password");
    }

    [Fact]
    public async Task Login_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var request = TestHelpers.CreateLoginRequest();
        _controller.ModelState.AddModelError("Username", "Username is required");

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<ApiResponse<LoginResponse>>();
        
        var response = badRequestResult.Value as ApiResponse<LoginResponse>;
        response!.Success.Should().BeFalse();
        response.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Login_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = TestHelpers.CreateLoginRequest();
        _mockAuthService.Setup(x => x.ValidateUserAsync(request.Username, request.Password))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeOfType<ApiResponse<LoginResponse>>();
        
        var response = objectResult.Value as ApiResponse<LoginResponse>;
        response!.Success.Should().BeFalse();
        response.Message.Should().Contain("error occurred");
    }

    [Fact]
    public async Task Login_GeneratesValidJwtToken()
    {
        // Arrange
        var request = TestHelpers.CreateLoginRequest();
        var user = TestHelpers.CreateTestUser(userId: 123, username: "testuser", password: "pass");
        _mockAuthService.Setup(x => x.ValidateUserAsync(request.Username, request.Password))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as ApiResponse<LoginResponse>;
        var token = response!.Data!.Token;

        token.Should().NotBeEmpty();
        
        // Verify token can be decoded
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        jsonToken.Should().NotBeNull();
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "123");
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "testuser");
    }

    [Fact]
    public async Task Login_IncludesUserInfoInResponse()
    {
        // Arrange
        var request = TestHelpers.CreateLoginRequest();
        var user = TestHelpers.CreateTestUser(
            userId: 456,
            username: "john.doe",
            password: "password123");
        user.FullName = "John Doe";
        user.Email = "john.doe@example.com";
        
        _mockAuthService.Setup(x => x.ValidateUserAsync(request.Username, request.Password))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as ApiResponse<LoginResponse>;
        
        response!.Data!.User.Should().NotBeNull();
        response.Data.User!.UserId.Should().Be(456);
        response.Data.User.Username.Should().Be("john.doe");
        response.Data.User.FullName.Should().Be("John Doe");
        response.Data.User.Email.Should().Be("john.doe@example.com");
        response.Data.ExpiresIn.Should().Be(24 * 3600); // 24 hours in seconds
    }
}

