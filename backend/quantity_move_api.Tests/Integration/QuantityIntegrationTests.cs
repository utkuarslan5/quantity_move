using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace quantity_move_api.Tests.Integration;

/// <summary>
/// Integration tests for quantity move operations using real database connection.
/// </summary>
public class QuantityIntegrationTests : DatabaseIntegrationTestBase
{
    private readonly string _authToken;
    private const string TestItemCode = "TEST_ITEM_INT";
    private const string TestLotNumber = "TEST_LOT_INT";
    private const string TestSourceLocation = "TEST_LOC_SRC";
    private const string TestTargetLocation = "TEST_LOC_TGT";
    private const string TestWarehouse = "MAIN";
    private const string TestSite = "faz";

    public QuantityIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        // Ensure database connection before proceeding
        EnsureDatabaseConnectionAsync().GetAwaiter().GetResult();

        // Setup test data
        SetupTestDataAsync().GetAwaiter().GetResult();

        // Get auth token (requires valid user in database)
        // Set TEST_USERNAME and TEST_PASSWORD environment variables if needed
        try
        {
            _authToken = GetAuthTokenAsync().GetAwaiter().GetResult();
            Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
        }
        catch
        {
            // If auth fails, tests requiring auth will fail with 401
            // This is expected - ensure test database has valid users
            _authToken = string.Empty;
        }
    }

    private async Task SetupTestDataAsync()
    {
        // Create test data using helpers
        await DatabaseTestHelpers.CreateCompleteTestDataAsync(
            DatabaseService,
            TestItemCode,
            TestLotNumber,
            TestSourceLocation,
            TestTargetLocation,
            sourceQuantity: 100.0m,
            targetQuantity: 0,
            TestWarehouse,
            TestSite);
    }

    private async Task<string> GetAuthTokenAsync()
    {
        // Try to get token from real auth service
        // Note: This requires a valid user in TRM_EDIUSER table
        // Set TEST_USERNAME and TEST_PASSWORD environment variables, or use defaults
        var testUsername = Environment.GetEnvironmentVariable("TEST_USERNAME") ?? "testuser";
        var testPassword = Environment.GetEnvironmentVariable("TEST_PASSWORD") ?? "testpass";

        try
        {
            var loginRequest = new LoginRequest
            {
                Username = testUsername,
                Password = testPassword
            };

            var response = await Client.PostAsJsonAsync("auth/login", loginRequest);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse?.Data?.Token != null)
                {
                    return apiResponse.Data.Token;
                }
            }
        }
        catch (Exception ex)
        {
            // Log but don't fail - some tests can run without auth
            System.Diagnostics.Debug.WriteLine($"Authentication failed: {ex.Message}");
        }

        // If authentication fails, tests that require auth will fail with 401
        // This is expected behavior - ensure test database has valid users
        throw new InvalidOperationException(
            $"Authentication failed for user '{testUsername}'. " +
            "Ensure test database has a valid user in TRM_EDIUSER table, " +
            "or set TEST_USERNAME and TEST_PASSWORD environment variables.");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Move_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new MoveQuantityRequest
        {
            ItemCode = TestItemCode,
            SourceLocation = TestSourceLocation,
            SourceLotNumber = TestLotNumber,
            TargetLocation = TestTargetLocation,
            Quantity = 10.0m,
            WarehouseCode = TestWarehouse,
            SiteReference = TestSite
        };

        // Get initial quantities
        var initialSourceQty = await DatabaseTestHelpers.GetStockQuantityAsync(
            DatabaseService, TestItemCode, TestLotNumber, TestSourceLocation, TestWarehouse);
        var initialTargetQty = await DatabaseTestHelpers.GetStockQuantityAsync(
            DatabaseService, TestItemCode, TestLotNumber, TestTargetLocation, TestWarehouse);

        // Act
        var response = await Client.PostAsJsonAsync("move", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<MoveQuantityResponse>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Success.Should().BeTrue();
        apiResponse.Data.TransactionId.Should().BeGreaterThan(0);

        // Verify database state changed
        var finalSourceQty = await DatabaseTestHelpers.GetStockQuantityAsync(
            DatabaseService, TestItemCode, TestLotNumber, TestSourceLocation, TestWarehouse);
        var finalTargetQty = await DatabaseTestHelpers.GetStockQuantityAsync(
            DatabaseService, TestItemCode, TestLotNumber, TestTargetLocation, TestWarehouse);

        finalSourceQty.Should().Be(initialSourceQty - 10.0m);
        finalTargetQty.Should().Be((initialTargetQty ?? 0) + 10.0m);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Move_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        var clientWithoutAuth = Factory.CreateClient();
        clientWithoutAuth.BaseAddress = new Uri(clientWithoutAuth.BaseAddress!, "/api/");

        var request = new MoveQuantityRequest
        {
            ItemCode = TestItemCode,
            SourceLocation = TestSourceLocation,
            SourceLotNumber = TestLotNumber,
            TargetLocation = TestTargetLocation,
            Quantity = 10.0m,
            WarehouseCode = TestWarehouse
        };

        // Act
        var response = await clientWithoutAuth.PostAsJsonAsync("move", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Move_WithInsufficientStock_ReturnsBadRequest()
    {
        // Arrange
        var request = new MoveQuantityRequest
        {
            ItemCode = TestItemCode,
            SourceLocation = TestSourceLocation,
            SourceLotNumber = TestLotNumber,
            TargetLocation = TestTargetLocation,
            Quantity = 10000.0m, // More than available
            WarehouseCode = TestWarehouse,
            SiteReference = TestSite
        };

        // Act
        var response = await Client.PostAsJsonAsync("move", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<MoveQuantityResponse>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Errors.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Move_WithInvalidLocation_ReturnsBadRequest()
    {
        // Arrange
        var request = new MoveQuantityRequest
        {
            ItemCode = TestItemCode,
            SourceLocation = TestSourceLocation,
            SourceLotNumber = TestLotNumber,
            TargetLocation = "INVALID_LOCATION_XYZ",
            Quantity = 10.0m,
            WarehouseCode = TestWarehouse,
            SiteReference = TestSite
        };

        // Act
        var response = await Client.PostAsJsonAsync("move", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Move_WithZeroQuantity_ReturnsBadRequest()
    {
        // Arrange
        var request = new MoveQuantityRequest
        {
            ItemCode = TestItemCode,
            SourceLocation = TestSourceLocation,
            SourceLotNumber = TestLotNumber,
            TargetLocation = TestTargetLocation,
            Quantity = 0, // Invalid
            WarehouseCode = TestWarehouse,
            SiteReference = TestSite
        };

        // Act
        var response = await Client.PostAsJsonAsync("move", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Cleanup test data
            CleanupTestDataAsync().GetAwaiter().GetResult();
        }
        base.Dispose(disposing);
    }

    private async Task CleanupTestDataAsync()
    {
        try
        {
            await DatabaseTestHelpers.CleanupTestItemAsync(DatabaseService, TestItemCode, TestSite);
            await DatabaseTestHelpers.CleanupTestLocationAsync(DatabaseService, TestSourceLocation, TestSite);
            await DatabaseTestHelpers.CleanupTestLocationAsync(DatabaseService, TestTargetLocation, TestSite);
        }
        catch
        {
            // Ignore cleanup errors in tests
        }
    }
}
