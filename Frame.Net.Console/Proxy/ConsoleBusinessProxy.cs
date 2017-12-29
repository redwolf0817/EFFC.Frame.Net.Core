using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Web.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Console.Proxy
{
    /// <summary>
    /// 多实例多线程业务模块请求代理
    /// </summary>
    public class ConsoleBusinessProxy:LocalModuleProxy<ConsoleParameter,DataCollection>
    {
        protected override BaseModule<ConsoleParameter, DataCollection> GetModule(ConsoleParameter p, DataCollection data)
        {
            return new ConsoleBusinessModule();
        }

        public override void OnError(Exception ex, ConsoleParameter p, DataCollection data)
        {
            throw ex;
        }
    }
    /// <summary>
    /// 单实例多线程业务模块请求代理
    /// </summary>
    public class ConsoleBusinessSingletonProxy : LocalModuleProxy<ConsoleParameter, DataCollection>
    {
        protected override BaseModule<ConsoleParameter, DataCollection> GetModule(ConsoleParameter p, DataCollection data)
        {
            return new ConsoleBusinessSingletonModule();
        }

        public override void OnError(Exception ex, ConsoleParameter p, DataCollection data)
        {
            throw ex;
        }
    }
}
