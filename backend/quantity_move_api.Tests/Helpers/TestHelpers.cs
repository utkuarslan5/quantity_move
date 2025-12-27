using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;

namespace quantity_move_api.Tests.Helpers;

public static class TestHelpers
{
    public static IConfiguration CreateTestConfiguration(Dictionary<string, string>? settings = null)
    {
        var defaultSettings = new Dictionary<string, string>
        {
            { "ConnectionStrings:DefaultConnection", "Server=test;Database=test;Trusted_Connection=true;" },
            { "Jwt:SecretKey", "TestSecretKeyForJwtTokenGeneration12345678901234567890" },
            { "Jwt:Issuer", "test-issuer" },
            { "Jwt:Audience", "test-audience" },
            { "Jwt:ExpirationInHours", "24" },
            { "StoredProcedures:MoveQuantity", "TR_Miktar_Ilerlet" }
        };

        if (settings != null)
        {
            foreach (var setting in settings)
            {
                defaultSettings[setting.Key] = setting.Value;
            }
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(defaultSettings)
            .Build();
    }

    public static Mock<ILogger<T>> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    public static User CreateTestUser(int userId = 1, string username = "testuser", string password = "testpass")
    {
        return new User
        {
            UserId = userId,
            Username = username,
            Password = password,
            FullName = "Test User",
            Email = "test@example.com"
        };
    }

    public static LoginRequest CreateLoginRequest(string username = "testuser", string password = "testpass")
    {
        return new LoginRequest
        {
            Username = username,
            Password = password
        };
    }

    public static MoveQuantityRequest CreateMoveQuantityRequest(
        string itemCode = "ITEM001",
        string sourceLocation = "LOC001",
        string targetLocation = "LOC002",
        decimal quantity = 10.0m)
    {
        return new MoveQuantityRequest
        {
            ItemCode = itemCode,
            SourceLocation = sourceLocation,
            TargetLocation = targetLocation,
            Quantity = quantity,
            Lot = "LOT001",
            Warehouse = "WH001",
            Site = "SITE001",
            UserId = "USER001"
        };
    }

    public static ValidateLocationRequest CreateValidateLocationRequest(string locationCode = "LOC001")
    {
        return new ValidateLocationRequest
        {
            LocationCode = locationCode,
            Warehouse = "WH001",
            Site = "SITE001"
        };
    }

    public static StockLookupRequest CreateStockLookupRequest(
        string? itemCode = "ITEM001",
        string? location = "LOC001")
    {
        return new StockLookupRequest
        {
            ItemCode = itemCode,
            Location = location,
            Warehouse = "WH001",
            Site = "SITE001",
            Lot = "LOT001"
        };
    }

    public static MoveQuantityResponse CreateMoveQuantityResponse(
        bool success = true,
        int returnCode = 0,
        string? transactionId = "TXN001")
    {
        return new MoveQuantityResponse
        {
            Success = success,
            ReturnCode = returnCode,
            TransactionId = transactionId,
            Message = success ? "Success" : "Failed",
            RowsAffected = 1
        };
    }

    public static ValidateLocationResponse CreateValidateLocationResponse(
        bool isValid = true,
        string locationCode = "LOC001")
    {
        return new ValidateLocationResponse
        {
            IsValid = isValid,
            LocationCode = locationCode,
            LocationDescription = isValid ? "Test Location" : null,
            Message = isValid ? null : "Location not found"
        };
    }

    public static StockLookupResponse CreateStockLookupResponse()
    {
        return new StockLookupResponse
        {
            Items = new List<StockItem>
            {
                new StockItem
                {
                    ItemCode = "ITEM001",
                    Location = "LOC001",
                    Warehouse = "WH001",
                    Site = "SITE001",
                    Lot = "LOT001",
                    Quantity = 100.0m,
                    ExpiryDate = DateTime.Now.AddDays(30),
                    ProductionDate = DateTime.Now.AddDays(-10),
                    Priority = 1
                }
            }
        };
    }
}

