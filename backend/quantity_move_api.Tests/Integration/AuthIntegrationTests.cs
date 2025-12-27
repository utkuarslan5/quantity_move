using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace quantity_move_api.Tests.Integration;

public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ConnectionStrings:DefaultConnection", "Server=test;Database=test;Trusted_Connection=true;" },
                    { "Jwt:SecretKey", "TestSecretKeyForJwtTokenGeneration12345678901234567890" },
                    { "Jwt:Issuer", "test-issuer" },
                    { "Jwt:Audience", "test-audience" },
                    { "Jwt:ExpirationInHours", "24" }
                });
            });

            builder.ConfigureServices(services =>
            {
                // Mock AuthService for integration tests
                var authServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAuthService));
                if (authServiceDescriptor != null)
                {
                    services.Remove(authServiceDescriptor);
                }

                var mockAuthService = new Mock<IAuthService>();
                mockAuthService.Setup(x => x.ValidateUserAsync("testuser", "testpass"))
                    .ReturnsAsync(new User
                    {
                        UserId = 1,
                        Username = "testuser",
                        Password = "testpass",
                        FullName = "Test User",
                        Email = "test@example.com"
                    });

                services.AddSingleton(mockAuthService.Object);
            });
        });

        _client = _factory.CreateClient();
        _client.BaseAddress = new Uri(_client.BaseAddress!, "/api/");
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "testpass"
        };

        // Act
        var response = await _client.PostAsJsonAsync("Auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeEmpty();

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Token.Should().NotBeEmpty();
        apiResponse.Data.User.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "invaliduser",
            Password = "invalidpass"
        };

        // Act
        var response = await _client.PostAsJsonAsync("Auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithMissingUsername_ReturnsBadRequest()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "",
            Password = "testpass"
        };

        // Act
        var response = await _client.PostAsJsonAsync("Auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithMissingPassword_ReturnsBadRequest()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("Auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

