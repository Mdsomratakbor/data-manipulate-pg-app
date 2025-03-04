using Npgsql;
using data_manipulate_pg.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_manipulate_pg
{
    public partial class DataRepository : IDataAsyncRepository
    {
        private readonly string _databaseConnection;

        // Constructor to initialize the repository with the connection string
        public DataRepository(string connectionString)
        {
            this._databaseConnection = connectionString;
        }

        /// <summary>
        /// Retrieves a single column value from the database as a specific type (T).
        /// </summary>
        /// <typeparam name="T">The type to which the result should be cast.</typeparam>
        /// <param name="query">SQL query string.</param>
        /// <param name="parameter">Optional dictionary of parameters for the query.</param>
        /// <returns>The value of the first column in the first row, cast to type T.</returns>
        public virtual async Task<T> GetDataOneRowColumAsync<T>(string query, Dictionary<string, string> parameter = null)
        {
            try
            {
                var data = "";  // Variable to hold the retrieved data
                using (NpgsqlConnection connection = new NpgsqlConnection(_databaseConnection))
                {
                    await connection.OpenAsync();  // Open connection asynchronously
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                    {
                        // Add parameters to the command if provided
                        if (parameter != null && parameter.Count > 0)
                        {
                            foreach (var item in parameter)
                            {
                                cmd.Parameters.AddWithValue(item.Key, item.Value);
                            }
                        }

                        // Execute the query and read the result
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())  // Read data from the result set
                                {
                                    data = Convert.ToString(reader[0]);  // Get the first column
                                }
                            }
                        }
                    }
                }
                return (T)Convert.ChangeType(data, typeof(T));  // Convert the result to the specified type
            }
            catch (Exception ex)
            {
                throw ex;  // Rethrow the exception for handling at a higher level
            }
        }

        /// <summary>
        /// Retrieves a single row from the database as a DataRow.
        /// </summary>
        /// <param name="query">SQL query string.</param>
        /// <param name="parameter">Optional dictionary of parameters for the query.</param>
        /// <returns>A DataRow containing the result of the query.</returns>
        public virtual async Task<DataRow> GetDataRowAsync(string query, Dictionary<string, string> parameter = null)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_databaseConnection))
                {
                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(query, connection))
                    {
                        // Add parameters to the data adapter if provided
                        if (parameter != null && parameter.Count > 0)
                        {
                            foreach (var item in parameter)
                            {
                                dataAdapter.SelectCommand.Parameters.AddWithValue(item.Key, item.Value);
                            }
                        }

                        // Fill the DataTable with the query results
                        DataTable dataTable = new DataTable();
                        await Task.Run(() => dataAdapter.Fill(dataTable));

                        // Return the first row of the DataTable
                        if (dataTable.Rows.Count > 0)
                            return dataTable.Rows[0];

                        return null;  // Return null if no rows were found
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;  // Rethrow the exception for handling at a higher level
            }
        }

        /// <summary>
        /// Retrieves a DataSet from the database.
        /// </summary>
        /// <param name="query">SQL query string.</param>
        /// <param name="parameter">Optional dictionary of parameters for the query.</param>
        /// <returns>A DataSet containing the query results.</returns>
        public virtual async Task<DataSet> GetDataSetAsync(string query, Dictionary<string, string> parameter = null)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_databaseConnection))
                {
                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(query, connection))
                    {
                        // Add parameters to the data adapter if provided
                        if (parameter != null && parameter.Count > 0)
                        {
                            foreach (var item in parameter)
                            {
                                dataAdapter.SelectCommand.Parameters.AddWithValue(item.Key, item.Value);
                            }
                        }

                        // Fill the DataSet with the query results
                        DataSet dataSet = new DataSet();
                        await Task.Run(() => dataAdapter.Fill(dataSet));
                        return dataSet;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;  // Rethrow the exception for handling at a higher level
            }
        }

        /// <summary>
        /// Retrieves a DataTable from the database.
        /// </summary>
        /// <param name="query">SQL query string.</param>
        /// <param name="parameter">Optional dictionary of parameters for the query.</param>
        /// <returns>A DataTable containing the query results.</returns>
        public virtual async Task<DataTable> GetDataTableAsync(string query, Dictionary<string, string> parameter = null)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_databaseConnection))
                {
                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(query, connection))
                    {
                        // Add parameters to the data adapter if provided
                        if (parameter != null && parameter.Count > 0)
                        {
                            foreach (var item in parameter)
                            {
                                dataAdapter.SelectCommand.Parameters.AddWithValue(item.Key, item.Value);
                            }
                        }

                        // Fill the DataTable with the query results
                        DataTable dt = new DataTable();
                        await Task.Run(() => dataAdapter.Fill(dt));
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;  // Rethrow the exception for handling at a higher level
            }
        }

        /// <summary>
        /// Executes a series of database queries within a transaction and commits them.
        /// </summary>
        /// <param name="queryPatterns">A list of queries to be executed.</param>
        /// <returns>True if all queries were successfully executed and committed.</returns>
        public async Task<bool> SaveChangesAsync(List<IQueryPattern> queryPatterns)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_databaseConnection))
                {
                    await connection.OpenAsync();  // Open connection asynchronously
                    var transaction = await connection.BeginTransactionAsync();  // Begin a transaction

                    try
                    {
                        using (NpgsqlCommand cmd = connection.CreateCommand())
                        {
                            cmd.Transaction = transaction;  // Assign the transaction to the command

                            // Loop through each query pattern and execute it
                            foreach (var data in queryPatterns)
                            {
                                cmd.CommandText = data.Query;  // Set the query text

                                // Add parameters to the command if provided
                                if (data.Parameters.Count > 0 && data.Parameters != null)
                                {
                                    cmd.Parameters.Clear();  // Clear any previous parameters
                                    foreach (var parameter in data.Parameters)
                                    {
                                        foreach (var item in parameter)
                                        {
                                            cmd.Parameters.AddWithValue(item.Key, item.Value);  // Add new parameters
                                        }
                                    }
                                }

                                // Execute the query asynchronously
                                await cmd.ExecuteNonQueryAsync();
                            }

                            await transaction.CommitAsync();  // Commit the transaction if all queries were successful
                        }
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();  // Rollback the transaction if there was an error
                        throw;
                    }
                }

                return true;  // Return true if all queries were executed successfully
            }
            catch (Exception)
            {
                throw;  // Rethrow the exception for handling at a higher level
            }
        }
    }
}
