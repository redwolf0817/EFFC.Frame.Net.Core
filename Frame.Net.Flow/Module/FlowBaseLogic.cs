using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Business.Logic;
using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters.Flow;
using EFFC.Frame.Net.Flow.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Flow.Module
{
    public abstract class FlowBaseLogic:BaseLogic<FlowParameter,FlowData>
    {
        protected abstract Action<FlowLogicData> GetAction(string actionName);
        protected override void DoProcess(FlowParameter p, FlowData d)
        {
            string actionanme = ComFunc.nvl(p.GetValue(ParameterKey.ACTION));
            FlowLogicData ldata = new FlowLogicData();
            ldata.JumpStep = p.JumpStep;
            foreach (var val in p.Domain(DomainKey.INPUT_PARAMETER))
            {
                ldata.SetValue(val.Key, val.Value);
            }
            foreach (var val in p.Domain(DomainKey.CUSTOMER_PARAMETER))
            {
                ldata.SetValue(val.Key, val.Value);
            }
            GetAction(actionanme).Invoke(ldata);
        }

    }

    public static class FlowLogicExtention
    {
        public static bool CallSingletonFLowLogic(this FlowBaseLogic.LogicCallHelper helper, string logicname, string logicaction)
        {
            var copyp = helper.Parameter.Clone<FlowParameter>();
            copyp.CalledLogicName = logicname;
            copyp.SetValue(ParameterKey.ACTION, logicaction);
            return ModuleProxyManager.Call<FlowBusinessSingletonProxy, FlowParameter, FlowData>(copyp, helper.Data);
        }
    }
}
