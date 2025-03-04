using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_manipulate_pg.Interfaces
{
    public interface IDataRepository: IDataJSONServices
    {
        /// <summary>
        /// This Method Return  DataTable. If you need work with  DataTable then use this method
        /// </summary>
        /// <param name="query">Pass SQL Query</param>
        /// <param name="parameter">SQL Query all parameter pass</param>
        /// <returns>Return DataTable</returns>
        DataTable GetDataTable(string query, Dictionary<string, string> parameter=null);
        /// <summary>
        /// This Method Return Multiple DataTable. If you need work with multiple data table on one method then use this method
        /// </summary>
        /// <param name="query">Pass SQL Query</param>
        /// <param name="parameter">SQL Query all parameter pass</param>
        /// <returns>Return DataSet</returns>
        DataSet GetDataSet(string query, Dictionary<string, string> parameter);

        /// <summary>
        /// This Method Return Only One row
        /// </summary>
        /// <param name="query">Pass SQL Query</param>
        /// <param name="parameter">SQL Query all parameter pass</param>
        /// <returns>This Method Return Only One row</returns>
        DataRow GetDataRow(string query, Dictionary<string, string> parameter = null);
   
        /// <summary>
        /// This Method Return only one colume and one row 
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="query">Pass SQL Query</param>
        /// <param name="parameter">SQL Query all parameter pass</param>
        /// <returns>This Method Return only one colume and one row</returns>
        T GetDataOneRowColum<T>(string query, Dictionary<string, string> parameter = null);

        /// <summary>
        /// A method all query and parameter pass this method
        /// </summary>
        /// <param name="query">SQL all  Query pass this parameter</param>
        /// <param name="parameters">Passing Query all parameter pass this dictionary</param>
        /// <returns></returns>
        IQueryPattern AddQuery(string query, Dictionary<string,string> parameter);
        /// <summary>
        /// this method return dictionary of parameter, default parameter is KeyValuePair<"@param1","com_code">, others parameter start name @param2 and end parameter name defendend you. how many parameter pass in array parameter 
        /// </summary>
        /// <param name="values">array of parameter values</param>
        /// <returns>Dictonary string with key value pair</returns>
        Dictionary<string, string> AddParameter(string[] values = null);


        /// <summary>
        /// Multiple SQL Command Query Execueteue this method. All Query Bind on AddQuery() method then pass  List of List<IQueryPattern>. If All Query execute successfully then return true, else return false. If any query is not excute List<IQueryPattern> queries then all query execution will be rollback.
        /// </summary>
        /// <param name="queryPatterns">All Query Bind on AddQuery() method then pass  List of List<IQueryPattern></param>
        /// <returns>Return bool value</returns>
        bool SaveChanges(List<IQueryPattern> queryPatterns);
    }
}
