using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.Parameters.Flow;
using EFFC.Frame.Net.Flow.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Flow.Proxy
{
    /// <summary>
    /// 多实例多线程业务模块请求代理
    /// </summary>
    public class FlowBusinessProxy:LocalModuleProxy<FlowParameter,FlowData>
    {
        FlowBusinessModule _module = null;
        protected override BaseModule<FlowParameter, FlowData> GetModule(FlowParameter p, FlowData data)
        {
            if (_module == null)
            {
                _module = new FlowBusinessModule();
            }
            return _module;
        }

        public override void OnError(Exception ex, FlowParameter p, FlowData data)
        {
            throw ex;
        }
    }
    /// <summary>
    /// 单实例多线程业务模块请求代理
    /// </summary>
    public class FlowBusinessSingletonProxy : LocalModuleProxy<FlowParameter, FlowData>
    {
        FlowBusinessSingletonModule _module = null;
        protected override BaseModule<FlowParameter, FlowData> GetModule(FlowParameter p, FlowData data)
        {
            if (_module == null)
            {
                _module = new FlowBusinessSingletonModule();
            }
            return _module;
        }

        public override void OnError(Exception ex, FlowParameter p, FlowData data)
        {
            throw ex;
        }
    }
}
