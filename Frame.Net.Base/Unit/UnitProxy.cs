using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;

namespace EFFC.Frame.Net.Base.Unit
{
    public class UnitProxy
    {
        public static DataCollection Call<T>(ParameterStd p) where T : IUnit
        {
            T t = (T)Activator.CreateInstance(typeof(T), true);

            return t.DoOperate(p);
        }
    }

    public class UnitProxy<P> where P : ParameterStd
    {
        public static DataCollection Call<T>(P p) where T : IUnit<P>
        {
            T t = (T)Activator.CreateInstance(typeof(T), true);

            return t.DoOperate(p);
        }
    }
}
