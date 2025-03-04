using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_manipulate_pg.Interfaces
{
    public interface IDataCommonRepository:IDataRepository, IDataAsyncRepository
    {
    }
}
