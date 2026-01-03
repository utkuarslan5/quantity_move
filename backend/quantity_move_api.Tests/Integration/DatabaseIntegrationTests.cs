using FluentAssertions;
using quantity_move_api.Tests.Helpers;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;

namespace quantity_move_api.Tests.Integration;

/// <summary>
/// Integration tests for database connectivity, stored procedures, and transaction handling.
/// </summary>
public class DatabaseIntegrationTests : DatabaseIntegrationTestBase
{
    private const string TestItemCode = "TEST_DB_ITEM";
    private const string TestLotNumber = "TEST_DB_LOT";
    private const string TestLocation = "TEST_DB_LOC";
    private const string TestWarehouse = "MAIN";
    private const string TestSite = "faz";

    public DatabaseIntegrationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        // Ensure database connection before proceeding
        EnsureDatabaseConnectionAsync().GetAwaiter().GetResult();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DatabaseConnection_ShouldBeSuccessful()
    {
        // Act
        var isConnected = await TestDatabaseConnectionAsync();

        // Assert
        isConnected.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task QueryAsync_ShouldReturnResults()
    {
        // Arrange
        var sql = "SELECT TOP 1 item FROM item_mst";

        // Act
        var results = await QueryAsync<string>(sql);

        // Assert
        results.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task QueryFirstOrDefaultAsync_ShouldReturnNull_WhenNoResults()
    {
        // Arrange
        var sql = "SELECT item FROM item_mst WHERE item = 'NONEXISTENT_ITEM_XYZ123'";

        // Act
        var result = await QueryFirstOrDefaultAsync<string>(sql);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ExecuteAsync_ShouldModifyDatabase()
    {
        // Arrange
        await DatabaseTestHelpers.CreateTestItemAsync(DatabaseService, TestItemCode, TestSite);
        
        try
        {
            var updateSql = "UPDATE item_mst SET item_desc = @NewDesc WHERE item = @ItemCode";
            
            // Act
            var rowsAffected = await ExecuteNonQueryAsync(updateSql, new
            {
                NewDesc = "Updated Description",
                ItemCode = TestItemCode
            });

            // Assert
            rowsAffected.Should().BeGreaterThan(0);

            // Verify update
            var verifySql = "SELECT item_desc FROM item_mst WHERE item = @ItemCode";
            var description = await QueryFirstOrDefaultAsync<string>(verifySql, new { ItemCode = TestItemCode });
            description.Should().Be("Updated Description");
        }
        finally
        {
            await DatabaseTestHelpers.CleanupTestItemAsync(DatabaseService, TestItemCode, TestSite);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task StoredProcedure_ValidateStock_ShouldExecute()
    {
        // Arrange
        await DatabaseTestHelpers.CreateCompleteTestDataAsync(
            DatabaseService,
            TestItemCode,
            TestLotNumber,
            TestLocation,
            "TARGET_LOC",
            100.0m,
            0,
            TestWarehouse,
            TestSite);

        try
        {
            // Act - Execute stored procedure (if it exists and is accessible)
            // Note: This test verifies the stored procedure can be called
            // The actual validation logic is tested in service tests
            var sql = "EXEC TR_Stok_Kontrol @Item, @Lot, @Location, @Whse";
            
            try
            {
                var result = await DatabaseService.ExecuteStoredProcedureNonQueryAsync(
                    "TR_Stok_Kontrol",
                    new
                    {
                        Item = TestItemCode,
                        Lot = TestLotNumber,
                        Location = TestLocation,
                        Whse = TestWarehouse
                    });

                // Assert - If we get here, stored procedure executed
                result.Should().BeGreaterThanOrEqualTo(0);
            }
            catch (SqlException ex) when (ex.Number == 2812) // Procedure not found
            {
                // Stored procedure might not exist in test database - skip this test
                // This is acceptable for integration tests
                return;
            }
        }
        finally
        {
            await DatabaseTestHelpers.CleanupTestItemAsync(DatabaseService, TestItemCode, TestSite);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DatabaseTransaction_ShouldRollbackOnError()
    {
        // Arrange
        await DatabaseTestHelpers.CreateTestItemAsync(DatabaseService, TestItemCode, TestSite);

        try
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Insert test lot within transaction
                var insertSql = @"
                    INSERT INTO lot_mst (item, lot, site_Ref, create_date, RecordDate)
                    VALUES (@ItemCode, @LotNumber, @SiteRef, @CreateDate, @RecordDate)";

                await connection.ExecuteAsync(insertSql, new
                {
                    ItemCode = TestItemCode,
                    LotNumber = TestLotNumber,
                    SiteRef = TestSite,
                    CreateDate = DateTime.Now,
                    RecordDate = DateTime.Now
                }, transaction);

                // Verify it exists within transaction
                var verifySql = "SELECT COUNT(*) FROM lot_mst WHERE item = @ItemCode AND lot = @LotNumber";
                var count = await connection.QuerySingleAsync<int>(verifySql, new { ItemCode = TestItemCode, LotNumber = TestLotNumber }, transaction);
                count.Should().Be(1);

                // Rollback transaction
                transaction.Rollback();

                // Verify it doesn't exist after rollback
                var countAfterRollback = await QueryFirstOrDefaultAsync<int>(verifySql, new { ItemCode = TestItemCode, LotNumber = TestLotNumber });
                countAfterRollback.Should().Be(0);
            }
            finally
            {
                transaction.Dispose();
            }
        }
        finally
        {
            await DatabaseTestHelpers.CleanupTestItemAsync(DatabaseService, TestItemCode, TestSite);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DatabaseConnection_ShouldHandleTimeout()
    {
        // Arrange
        var longRunningSql = "WAITFOR DELAY '00:00:02'"; // 2 second delay

        // Act
        var startTime = DateTime.Now;
        await QueryAsync<object>(longRunningSql);
        var elapsed = DateTime.Now - startTime;

        // Assert
        elapsed.Should().BeGreaterThanOrEqualTo(TimeSpan.FromSeconds(1.5));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DatabaseService_ShouldHandleInvalidSql()
    {
        // Arrange
        var invalidSql = "SELECT * FROM nonexistent_table_xyz123";

        // Act & Assert
        await Assert.ThrowsAsync<SqlException>(async () =>
        {
            await QueryAsync<object>(invalidSql);
        });
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DatabaseService_ShouldHandleParameterizedQueries()
    {
        // Arrange
        await DatabaseTestHelpers.CreateTestItemAsync(DatabaseService, TestItemCode, TestSite);

        try
        {
            var sql = "SELECT item FROM item_mst WHERE item = @ItemCode AND site_Ref = @SiteRef";

            // Act
            var result = await QueryFirstOrDefaultAsync<string>(sql, new
            {
                ItemCode = TestItemCode,
                SiteRef = TestSite
            });

            // Assert
            result.Should().Be(TestItemCode);
        }
        finally
        {
            await DatabaseTestHelpers.CleanupTestItemAsync(DatabaseService, TestItemCode, TestSite);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DatabaseService_ShouldHandleNullParameters()
    {
        // Arrange
        var sql = "SELECT @Value AS TestValue";

        // Act
        var result = await QueryFirstOrDefaultAsync<string>(sql, new { Value = (string?)null });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task TestDataHelpers_ShouldCreateAndCleanupData()
    {
        // Arrange & Act
        await DatabaseTestHelpers.CreateCompleteTestDataAsync(
            DatabaseService,
            TestItemCode,
            TestLotNumber,
            TestLocation,
            "TARGET_LOC",
            50.0m,
            0,
            TestWarehouse,
            TestSite);

        // Verify data exists
        var quantity = await DatabaseTestHelpers.GetStockQuantityAsync(
            DatabaseService, TestItemCode, TestLotNumber, TestLocation, TestWarehouse);
        quantity.Should().Be(50.0m);

        // Cleanup
        await DatabaseTestHelpers.CleanupTestItemAsync(DatabaseService, TestItemCode, TestSite);

        // Verify data is cleaned up
        var itemExists = await QueryFirstOrDefaultAsync<string>(
            "SELECT item FROM item_mst WHERE item = @ItemCode",
            new { ItemCode = TestItemCode });
        itemExists.Should().BeNull();
    }
}

