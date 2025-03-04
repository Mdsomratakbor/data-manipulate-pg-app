using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace data_manipulate_pg.Interfaces
{
    public interface IDataAsyncRepository
    {
        /// <summary>
        /// This Method Returns Asynchronous Single DataTable.
        /// </summary>
        /// <param name="query">Pass SQL Query</param>
        /// <param name="parameter">SQL Query parameters</param>
        /// <returns>Returns DataTable</returns>
        Task<DataTable> GetDataTableAsync(string query, Dictionary<string, string> parameter = null);

        /// <summary>
        /// This Method Returns Asynchronous Multiple DataTables. If you need to work with multiple data tables in one method, use this method.
        /// </summary>
        /// <param name="query">Pass SQL Query</param>
        /// <param name="parameter">SQL Query parameters</param>
        /// <returns>Returns DataSet</returns>
        Task<DataSet> GetDataSetAsync(string query, Dictionary<string, string> parameter = null);

        /// <summary>
        /// This Method Returns Asynchronous Single DataTable Row. If you need to work with a single row, use this method.
        /// </summary>
        /// <param name="query">Pass SQL Query</param>
        /// <param name="parameter">SQL Query parameters</param>
        /// <returns>Returns DataRow</returns>
        Task<DataRow> GetDataRowAsync(string query, Dictionary<string, string> parameter = null);

        /// <summary>
        /// This Method Returns Asynchronous One Row and One Column.
        /// </summary>
        /// <param name="query">Pass SQL Query</param>
        /// <param name="parameter">SQL Query parameters</param>
        /// <returns>Returns value of type T</returns>
        Task<T> GetDataOneRowColumAsync<T>(string query, Dictionary<string, string> parameter = null);

        /// <summary>
        /// All SQL command queries can be passed through this method.
        /// </summary>
        /// <param name="queryPatterns">List of queries and their parameters</param>
        /// <returns>Returns boolean value indicating success or failure</returns>
        Task<bool> SaveChangesAsync(List<IQueryPattern> queryPatterns);
    }
}
