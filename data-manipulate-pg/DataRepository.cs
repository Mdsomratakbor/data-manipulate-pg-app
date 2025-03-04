using data_manipulate_pg.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using Npgsql; // Import PostgreSQL namespace
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_manipulate_pg
{
    public partial class DataRepository : DataJSONServices, IDataRepository
    {
        public IQueryPattern _queryPattern;

        /// <summary>
        /// Adds parameters for SQL queries.
        /// </summary>
        /// <param name="values">Array of values to be added as parameters.</param>
        /// <returns>A dictionary of parameters with their corresponding values.</returns>
        public virtual Dictionary<string, string> AddParameter(string[] values = null)
        {
            var parameter = new Dictionary<string, string>();
            int i = 1;
            if (values.Length > 0)
            {
                foreach (var data in values)
                {
                    parameter.Add($"@param{i}", data); // PostgreSQL uses '@' for parameters
                    i++;
                }
            }

            return parameter;
        }

        /// <summary>
        /// Adds a query pattern to be executed with parameters.
        /// </summary>
        /// <param name="query">The SQL query to be executed.</param>
        /// <param name="parameters">Dictionary of parameters to be used in the query.</param>
        /// <returns>Query pattern with the query and parameters.</returns>
        public virtual IQueryPattern AddQuery(string query, Dictionary<string, string> parameters)
        {
            _queryPattern = new QueryPattern();
            _queryPattern.Query = query;
            _queryPattern.Parameters.Add(parameters);
            return _queryPattern;
        }

        /// <summary>
        /// Gets a single value from the database query.
        /// </summary>
        /// <typeparam name="T">The type of data to return.</typeparam>
        /// <param name="query">The SQL query.</param>
        /// <param name="parameter">Parameters for the query.</param>
        /// <returns>A single value of type T.</returns>
        public virtual T GetDataOneRowColum<T>(string query, Dictionary<string, string> parameter = null)
        {
            try
            {
                var data = "";
                using (NpgsqlConnection connection = new NpgsqlConnection(_databaseConnection)) // PostgreSQL connection
                {
                    connection.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection)) // PostgreSQL command
                    {
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                data = Convert.ToString(reader[0]);
                            }
                            reader.Close();
                            connection.Close();
                        }
                        return (T)Convert.ChangeType(data, typeof(T));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a single row of data from the database.
        /// </summary>
        /// <param name="query">The SQL query.</param>
        /// <param name="parameter">Parameters for the query.</param>
        /// <returns>A DataRow object representing the retrieved row.</returns>
        public virtual DataRow GetDataRow(string query, Dictionary<string, string> parameter = null)
        {
            try
            {
                using (NpgsqlConnection obcon = new NpgsqlConnection(_databaseConnection)) // PostgreSQL connection
                {
                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(query, obcon)) // PostgreSQL data adapter
                    {
                        if (parameter != null && parameter.Count > 0)
                        {
                            foreach (var item in parameter)
                            {
                                dataAdapter.SelectCommand.Parameters.AddWithValue(item.Key, item.Value);
                            }
                        }
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        DataRow row = dataTable.Rows[0];
                        return row;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a DataSet from the database for the given query.
        /// </summary>
        /// <param name="query">The SQL query.</param>
        /// <param name="parameter">Parameters for the query.</param>
        /// <returns>A DataSet object containing the data.</returns>
        public virtual DataSet GetDataSet(string query, Dictionary<string, string> parameter)
        {
            try
            {
                using (NpgsqlConnection obcon = new NpgsqlConnection(_databaseConnection)) // PostgreSQL connection
                {
                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(query, obcon)) // PostgreSQL data adapter
                    {
                        if (parameter != null && parameter.Count > 0)
                        {
                            foreach (var item in parameter)
                            {
                                dataAdapter.SelectCommand.Parameters.AddWithValue(item.Key, item.Value);
                            }
                        }
                        DataSet dataSet = new DataSet();
                        dataAdapter.Fill(dataSet);
                        return dataSet;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a DataTable from the database for the given query.
        /// </summary>
        /// <param name="query">The SQL query.</param>
        /// <param name="parameter">Parameters for the query.</param>
        /// <returns>A DataTable object containing the data.</returns>
        public virtual DataTable GetDataTable(string query, Dictionary<string, string> parameter = null)
        {
            try
            {
                using (NpgsqlConnection obcon = new NpgsqlConnection(_databaseConnection)) // PostgreSQL connection
                {
                    using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(query, obcon)) // PostgreSQL data adapter
                    {

                        if (parameter != null && parameter.Count > 0)
                        {
                            foreach (var item in parameter)
                            {
                                dataAdapter.SelectCommand.Parameters.AddWithValue(item.Key, item.Value);
                            }
                        }
                        DataTable dt = new DataTable();
                        dataAdapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Saves changes to the database using a list of query patterns.
        /// </summary>
        /// <param name="queryPatterns">List of query patterns to be executed.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public bool SaveChanges(List<IQueryPattern> queryPatterns)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(_databaseConnection)) // PostgreSQL connection
                {
                    connection.Open();
                    NpgsqlTransaction transaction;
                    transaction = connection.BeginTransaction(); // Begin transaction
                    try
                    {
                        using (NpgsqlCommand cmd = connection.CreateCommand()) // PostgreSQL command
                        {
                            cmd.Transaction = transaction;
                            foreach (var data in queryPatterns)
                            {
                                cmd.CommandText = data.Query;
                                if (data.Parameters.Count > 0 && data.Parameters != null)
                                {
                                    cmd.Parameters.Clear();
                                    foreach (var parameter in data.Parameters)
                                    {
                                        foreach (var item in parameter)
                                        {
                                            cmd.Parameters.AddWithValue(item.Key, item.Value); // Add parameters to the command
                                        }
                                    }
                                }

                                cmd.ExecuteNonQuery();
                            }
                            transaction.Commit(); // Commit the transaction
                            connection.Close();
                        }
                    }
                    catch (Exception ex1)
                    {
                        transaction.Rollback(); // Rollback in case of an error
                        throw ex1;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
