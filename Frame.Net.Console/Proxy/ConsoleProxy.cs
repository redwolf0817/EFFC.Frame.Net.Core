using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Data.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFFC.Frame.Net.Consoles.Core;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Global;

namespace EFFC.Frame.Net.Consoles.Proxy
{
    public class ConsoleProxy
    {
        public static void Call<C,P,D>(params string[] args)
            where C : ConsoleBaseHandler<P, D>
            where P : ConsoleParameter
            where D : DataCollection
        {
            try
            {
                P p = Activator.CreateInstance<P>();
                D d = Activator.CreateInstance<D>();
                p.ExtentionObj.args = args;
                var proxy = (C)Activator.CreateInstance(typeof(C), true);
                proxy.StepStart(p, d);
            }
            catch (Exception ex)
            {
                GlobalCommon.Logger.WriteLog(LoggerLevel.ERROR, ex.Message);
            }
        }
    }
}
