using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json; 

namespace data_manipulate_pg.Interfaces
{
    public abstract class DataJSONServices : IDataJSONServices
    {
        public virtual string DataSetToJSON(DataSet dataset)
        {
            try
            {
                // Creating a dictionary to hold the DataTable name as the key and its data as value
                var datasetDict = new Dictionary<string, object>();

                // Iterating through each DataTable in the DataSet
                foreach (DataTable table in dataset.Tables)
                {
                    // Serializing each DataTable to JSON using DataTableToJSON method
                    var tableJson = DataTableToJSON(table);

                    // Adding the table's name and its serialized data to the dictionary
                    datasetDict[table.TableName] = JsonConvert.DeserializeObject(tableJson);
                }

                // Serializing the entire DataSet dictionary to JSON
                return JsonConvert.SerializeObject(datasetDict, Formatting.Indented);
            }
            catch (Exception ex)
            {
                // Handling any exceptions that might occur during the process
                throw new InvalidOperationException("Error converting DataSet to JSON", ex);
            }
        }

        public virtual string DataTableToJSON(DataTable dataTable)
        {
            try
            {
                // Creating a list of dictionaries where each dictionary represents a row.
                var rows = new List<Dictionary<string, object>>();

                // Iterating through each row in the DataTable
                foreach (DataRow row in dataTable.Rows)
                {
                    var rowDict = new Dictionary<string, object>();

                    // Iterating through each column in the row
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        rowDict[column.ColumnName] = row[column];
                    }

                    // Adding the row dictionary to the list of rows
                    rows.Add(rowDict);
                }

                // Serializing the list of rows to JSON using Newtonsoft.Json
                return JsonConvert.SerializeObject(rows);
            }
            catch (Exception ex)
            {
                // Handling any exceptions that might occur during the process
                throw new InvalidOperationException("Error converting DataTable to JSON", ex);
            }
        }
    }
}
