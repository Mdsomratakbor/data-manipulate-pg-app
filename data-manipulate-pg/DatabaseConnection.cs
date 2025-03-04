using data_manipulate_pg.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_manipulate_pg
{
    public class DatabaseConnection : IDatabaseConnection
    {
        public string ConnectionString { get; set; }
    }
}
