using Npgsql;
using data_manipulate_pg;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using data_manipulate_pg.Interfaces;

public class DataAsyncRepositoryTests
{
     string _testConnectionString = "Host=localhost;Username=postgres;Password=fraser_admin;Database=frasersdb;Port=5433";
    private readonly DataRepository _dataRepository;

    public DataAsyncRepositoryTests()
    {
        _dataRepository = new DataRepository(_testConnectionString);
    }

    #region Test: GetDataOneRowColumnAsync
    [Fact]
    public async Task GetDataOneRowColumnAsync_ReturnsValue()
    {
        // Arrange
        var query = "SELECT * FROM bms.measurment_points LIMIT 1"; 
        //var parameters = new Dictionary<string, string>();  

    
        //await InsertTestDataAsync();

        // Act
        var result = await _dataRepository.GetDataOneRowColumAsync<string>(query);

        // Assert
        Assert.Equal("CHPL_raw_load_building", result); 
    }
    #endregion

    #region Helper: Insert Test Data
    private async Task InsertTestDataAsync()
    {
        using (var connection = new NpgsqlConnection(_testConnectionString))
        {
            await connection.OpenAsync();
            using (var command = new NpgsqlCommand("INSERT INTO bms.measurment_points (point_name) VALUES ('TestPoint')", connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }
    }
    #endregion

    #region Test: GetDataRowAsync
    [Fact]
    public async Task GetDataRowAsync_ReturnsDataRow()
    {
        // Arrange
        var query = "SELECT * FROM bms.measurment_points LIMIT 1";
       // var parameters = new Dictionary<string, string>(); 

        //await InsertTestDataAsync();

        // Act
        var result = await _dataRepository.GetDataRowAsync(query);

        // Assert
        Assert.NotNull(result); 
        Assert.Equal("TestPoint", result["point_name"]); 
    }
    #endregion

    #region Test: GetDataSetAsync
    [Fact]
    public async Task GetDataSetAsync_ReturnsDataSet()
    {
        // Arrange
        var query = "SELECT * FROM bms.measurment_points";  
        //var parameters = new Dictionary<string, string>();  

        //await InsertTestDataAsync();

        // Act
        var result = await _dataRepository.GetDataSetAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Tables[0].Rows.Count > 0);
    }
    #endregion

    #region Test: GetDataTableAsync
    [Fact]
    public async Task GetDataTableAsync_ReturnsDataTable()
    {
        // Arrange
        var query = "SELECT * FROM bms.measurment_points";
        // var parameters = new Dictionary<string, string>(); 

        // await InsertTestDataAsync();

        // Act
        var result = await _dataRepository.GetDataTableAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Rows.Count > 0);
    }
    #endregion


    #region Test: SaveChangesAsync
    [Fact]
    public async Task SaveChangesAsync_SavesChangesSuccessfully()
    {
        // Arrange
        var queryPatterns = new List<IQueryPattern>
    {
        new QueryPattern
        {
            Query = "INSERT INTO bms.measurment_points (point_name) VALUES ('TestPoint2')",
            Parameters = new List<Dictionary<string, string>>()
        },
        new QueryPattern
        {
            Query = "INSERT INTO bms.measurment_points (point_name) VALUES ('TestPoint3')",
            Parameters = new List<Dictionary<string, string>>()
        }
    };

        // Act
        var result = await _dataRepository.SaveChangesAsync(queryPatterns);

        // Assert
        Assert.True(result); 
    }
    #endregion


    #region Test: GetDataOneRowColumnAsync_WithParameters
    [Fact]
    public async Task GetDataOneRowColumnAsync_WithParameters_ReturnsValue()
    {
        // Arrange
        var query = "SELECT * FROM bms.measurment_points WHERE point_name = @pointName LIMIT 1";
        var parameters = new Dictionary<string, string> { { "@pointName", "TestPoint" } };

        // Act
        var result = await _dataRepository.GetDataOneRowColumAsync<string>(query, parameters);

        // Assert
        Assert.Equal("TestPoint", result); 
    }
    #endregion

    #region Test: GetDataRowAsync_WithParameters
    [Fact]
    public async Task GetDataRowAsync_WithParameters_ReturnsDataRow()
    {
        // Arrange
        var query = "SELECT * FROM bms.measurment_points WHERE point_name = @pointName LIMIT 1";
        var parameters = new Dictionary<string, string> { { "@pointName", "TestPoint" } };

        // Act
        var result = await _dataRepository.GetDataRowAsync(query, parameters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestPoint", result["point_name"]); 
    }
    #endregion

    #region Test: GetDataSetAsync_WithParameters
    [Fact]
    public async Task GetDataSetAsync_WithParameters_ReturnsDataSet()
    {
        // Arrange
        var query = "SELECT * FROM bms.measurment_points WHERE point_name = @pointName";
        var parameters = new Dictionary<string, string> { { "@pointName", "TestPoint" } };

        // Act
        var result = await _dataRepository.GetDataSetAsync(query, parameters);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Tables[0].Rows.Count > 0);
    }
    #endregion


    #region Test: SaveChangesAsync_WithInvalidData
    [Fact]
    public async Task SaveChangesAsync_WithInvalidData_ReturnsFalseAndRollsBack()
    {
        // Arrange
        var queryPatterns = new List<IQueryPattern>
    {
        new QueryPattern
        {
            Query = "INSERT INTO bms.measurment_points (point_name) VALUES ('TestPoint4')",
            Parameters = new List<Dictionary<string, string>>()
        },
        new QueryPattern
        {
            Query = "INSERT INTO bms.invalid_table (point_name) VALUES ('InvalidPoint')",  
            Parameters = new List<Dictionary<string, string>>()
        }
    };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await _dataRepository.SaveChangesAsync(queryPatterns); 
        });
    }
    #endregion


}
