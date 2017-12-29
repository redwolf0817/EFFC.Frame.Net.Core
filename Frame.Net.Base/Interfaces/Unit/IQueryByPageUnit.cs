using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;

namespace EFFC.Frame.Net.Base.Interfaces.Unit
{
    public interface IQueryByPageUnit<T> where T : ParameterStd
    {
        Func<T, string> PreSQLFunc(string flag);
        Func<T, string> QuerySQLByPageFunc(string flag);
        Func<T, string> AfterSQLFunc(string flag);
        Func<T, string> OrderByFunc(string flag);
    }
}
