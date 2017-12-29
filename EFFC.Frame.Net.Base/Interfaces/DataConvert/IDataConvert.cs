using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Base.Interfaces.DataConvert
{
    public interface IDataConvert<T>
    {
        T ConvertTo(object obj);
    }

    public interface IDataConvert
    {
        object ConvertTo(object obj);
    }

    public interface IDataConvert<From, To>
    {
        To ConvertTo(From obj);
    }
}
