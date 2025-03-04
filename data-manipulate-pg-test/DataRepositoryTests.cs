using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_manipulate_pg;
using data_manipulate_pg.Interfaces;
using Moq;
using Npgsql;

namespace data_manipulate_pg_test
{
    public class DataRepositoryTests
    {
        private readonly Mock<NpgsqlConnection> _mockConnection;
        private readonly Mock<NpgsqlCommand> _mockCommand;
        private readonly Mock<NpgsqlDataReader> _mockReader;
        private readonly Mock<NpgsqlDataAdapter> _mockDataAdapter;
        private readonly IDataRepository _dataRepository;

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
        public  void GetDataOneRowColumnAsync_ReturnsValue()
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
            var result =  _dataRepository.GetDataOneRowColum<string>(query, parameters);

            // Assert
            Assert.Equal("TestValue", result);
        }

        [Fact]
        public void GetDataOneRowColumnAsync_ReturnsNull_IfNoRows()
        {
            // Arrange
            var query = "SELECT column_name FROM table_name LIMIT 1";
            var parameters = new Dictionary<string, string> { { "param1", "value1" } };

            _mockCommand.Setup(cmd => cmd.ExecuteReaderAsync(It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(_mockReader.Object);
            _mockReader.Setup(reader => reader.HasRows)
                       .Returns(false);

            // Act
            var result =  _dataRepository.GetDataOneRowColum<string>(query, parameters);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region Test: GetDataRowAsync
        [Fact]
        public void GetDataRowAsync_ReturnsRow()
        {
            // Arrange
            var query = "SELECT * FROM table_name LIMIT 1";
            var parameters = new Dictionary<string, string> { { "param1", "value1" } };
            var mockDataTable = new DataTable();
            mockDataTable.Columns.Add("Column1");
            mockDataTable.Rows.Add("TestValue");

            _mockDataAdapter.Setup(x => x.Fill(It.IsAny<DataTable>())).Returns(1);

            // Act
            var result =  _dataRepository.GetDataRow(query, parameters);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestValue", result["Column1"]);
        }

        [Fact]
        public void GetDataRowAsync_ReturnsNull_IfNoRows()
        {
            // Arrange
            var query = "SELECT * FROM table_name LIMIT 1";
            var parameters = new Dictionary<string, string> { { "param1", "value1" } };
            var mockDataTable = new DataTable();

            _mockDataAdapter.Setup(x => x.Fill(It.IsAny<DataTable>())).Returns(0);

            // Act
            var result =  _dataRepository.GetDataRow(query, parameters);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region Test: GetDataSetAsync
        [Fact]
        public void GetDataSetAsync_ReturnsDataSet()
        {
            // Arrange
            var query = "SELECT * FROM table_name";
            var parameters = new Dictionary<string, string> { { "param1", "value1" } };
            var mockDataSet = new DataSet();

            _mockDataAdapter.Setup(x => x.Fill(It.IsAny<DataSet>())).Returns(1);

            // Act
            var result = _dataRepository.GetDataSet(query, parameters);

            // Assert
            Assert.NotNull(result);
        }
        #endregion

        #region Test: GetDataTableAsync
        [Fact]
        public void GetDataTableAsync_ReturnsDataTable()
        {
            // Arrange
            var query = "SELECT * FROM table_name";
            var parameters = new Dictionary<string, string> { { "param1", "value1" } };
            var mockDataTable = new DataTable();

            _mockDataAdapter.Setup(x => x.Fill(It.IsAny<DataTable>())).Returns(1);

            // Act
            var result = _dataRepository.GetDataTable(query, parameters);

            // Assert
            Assert.NotNull(result);
        }
        #endregion

        #region Test: SaveChangesAsync
        [Fact]
        public void SaveChangesAsync_Success()
        {
            // Arrange
            var queryPatterns = new List<IQueryPattern>
            {
                new Mock<IQueryPattern>().Object
            };

            _mockCommand.Setup(x => x.ExecuteNonQueryAsync(It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(1);

            // Act
            var result =  _dataRepository.SaveChanges(queryPatterns);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void SaveChangesAsync_Failure()
        {
            // Arrange
            var queryPatterns = new List<IQueryPattern>
            {
                new Mock<IQueryPattern>().Object
            };

            _mockCommand.Setup(x => x.ExecuteNonQueryAsync(It.IsAny<System.Threading.CancellationToken>()))
                        .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
             Assert.Throws<Exception>(() => _dataRepository.SaveChanges(queryPatterns));
        }
        #endregion
    }
}
