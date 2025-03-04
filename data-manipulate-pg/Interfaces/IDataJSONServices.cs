using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_manipulate_pg.Interfaces
{
    public interface IDataJSONServices
    {
        /// <summary>
        /// This Method used DataTable to convert  Single JSON object 
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <returns>Single JSON Object</returns>
        string DataTableToJSON(DataTable dataTable);

        /// <summary>
        /// This Method used DataSet to convert Multiple JSON object 
        /// </summary>
        /// <param name="dataTable">DataSet</param>
        /// <returns>Multiple JSON Object</returns>
        string DataSetToJSON(DataSet dataset);
    }
}
