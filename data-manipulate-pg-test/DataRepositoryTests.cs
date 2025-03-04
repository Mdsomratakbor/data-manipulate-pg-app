using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace data_manipulate_pg_test
{
    public class DataRepositoryTests
    {
        private readonly Mock<NpgsqlConnection> _mockConnection;
        private readonly Mock<NpgsqlCommand> _mockCommand;
        private readonly Mock<NpgsqlDataReader> _mockReader;
        private readonly Mock<NpgsqlDataAdapter> _mockDataAdapter;
        private readonly DataRepository _dataRepository;

        public DataRepositoryTests()
        {
            _mockConnection = new Mock<NpgsqlConnection>("YourConnectionStringHere");
            _mockCommand = new Mock<NpgsqlCommand>();
            _mockReader = new Mock<NpgsqlDataReader>();
            _mockDataAdapter = new Mock<NpgsqlDataAdapter>();

            // Initialize DataRepository with a mock connection string
            _dataRepository = new DataRepository("YourConnectionStringHere");
        }

        #region Test: GetDataOneRowColumnAsync
        [Fact]
        public async Task GetDataOneRowColumnAsync_ReturnsValue()
        {
            // Arrange
            var query = "SELECT column_name FROM table_name LIMIT 1";
            var parameters = new Dictionary<string, string> { { "param1", "value1" } };

            _mockCommand.Setup(cmd => cmd.ExecuteReaderAsync(It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(_mockReader.Object);

            _mockReader.Setup(reader => reader.ReadAsync(It.IsAny<System.Threading.CancellationToken>()))
                       .ReturnsAsync(true);
            _mockReader.Setup(reader => reader[0])
                       .Returns("TestValue");

            // Act
            var result = await _dataRepository.GetDataOneRowColumAsync<string>(query, parameters);

            // Assert
            Assert.Equal("TestValue", result);
        }

        [Fact]
        public async Task GetDataOneRowColumnAsync_ReturnsNull_IfNoRows()
        {
            // Arrange
            var query = "SELECT column_name FROM table_name LIMIT 1";
            var parameters = new Dictionary<string, string> { { "param1", "value1" } };

            _mockCommand.Setup(cmd => cmd.ExecuteReaderAsync(It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(_mockReader.Object);
            _mockReader.Setup(reader => reader.HasRows)
                       .Returns(false);

            // Act
            var result = await _dataRepository.GetDataOneRowColumAsync<string>(query, parameters);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region Test: GetDataRowAsync
        [Fact]
        public async Task GetDataRowAsync_ReturnsRow()
        {
            // Arrange
            var query = "SELECT * FROM table_name LIMIT 1";
            var parameters = new Dictionary<string, string> { { "param1", "value1" } };
            var mockDataTable = new DataTable();
            mockDataTable.Columns.Add("Column1");
            mockDataTable.Rows.Add("TestValue");

            _mockDataAdapter.Setup(x => x.Fill(It.IsAny<DataTable>())).Returns(1);

            // Act
            var result = await _dataRepository.GetDataRowAsync(query, parameters);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestValue", result["Column1"]);
        }

        [Fact]
        public async Task GetDataRowAsync_ReturnsNull_IfNoRows()
        {
            // Arrange
            var query = "SELECT * FROM table_name LIMIT 1";
            var parameters = new Dictionary<string, string> { { "param1", "value1" } };
            var mockDataTable = new DataTable();

            _mockDataAdapter.Setup(x => x.Fill(It.IsAny<DataTable>())).Returns(0);

            // Act
            var result = await _dataRepository.GetDataRowAsync(query, parameters);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region Test: GetDataSetAsync
        [Fact]
        public async Task GetDataSetAsync_ReturnsDataSet()
        {
            // Arrange
            var query = "SELECT * FROM table_name";
            var parameters = new Dictionary<string, string> { { "param1", "value1" } };
            var mockDataSet = new DataSet();

            _mockDataAdapter.Setup(x => x.Fill(It.IsAny<DataSet>())).Returns(1);

            // Act
            var result = await _dataRepository.GetDataSetAsync(query, parameters);

            // Assert
            Assert.NotNull(result);
        }
        #endregion

        #region Test: GetDataTableAsync
        [Fact]
        public async Task GetDataTableAsync_ReturnsDataTable()
        {
            // Arrange
            var query = "SELECT * FROM table_name";
            var parameters = new Dictionary<string, string> { { "param1", "value1" } };
            var mockDataTable = new DataTable();

            _mockDataAdapter.Setup(x => x.Fill(It.IsAny<DataTable>())).Returns(1);

            // Act
            var result = await _dataRepository.GetDataTableAsync(query, parameters);

            // Assert
            Assert.NotNull(result);
        }
        #endregion

        #region Test: SaveChangesAsync
        [Fact]
        public async Task SaveChangesAsync_Success()
        {
            // Arrange
            var queryPatterns = new List<IQueryPattern>
            {
                new Mock<IQueryPattern>().Object
            };

            _mockCommand.Setup(x => x.ExecuteNonQueryAsync(It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(1);

            // Act
            var result = await _dataRepository.SaveChangesAsync(queryPatterns);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SaveChangesAsync_Failure()
        {
            // Arrange
            var queryPatterns = new List<IQueryPattern>
            {
                new Mock<IQueryPattern>().Object
            };

            _mockCommand.Setup(x => x.ExecuteNonQueryAsync(It.IsAny<System.Threading.CancellationToken>()))
                        .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _dataRepository.SaveChangesAsync(queryPatterns));
        }
        #endregion
    }
}
