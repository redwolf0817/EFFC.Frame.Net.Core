using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Unit.DB.Unit
{
    public interface IDBUnit<T> where T : ParameterStd
    {
        Func<T, dynamic> GetSqlFunc(string flag);
    }
}
