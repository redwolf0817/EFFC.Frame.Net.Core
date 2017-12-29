using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Business.Logic.Consoles;
using EFFC.Frame.Net.Console.Proxy;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Console.Module
{
    public abstract class ConsoleLogic:ConsoleBaseLogic<ConsoleParameter,DataCollection>
    {
        public abstract Action<ConsoleLogicData> GetAction(string actionName);
        private DataCollection _d;
        private ConsoleParameter _p;

        protected override void DoProcess(ConsoleParameter p, DataCollection d)
        {
            _p = p;
            _d = d;
            ConsoleLogicData pd = new ConsoleLogicData();
            pd.Args = p.ExtentionObj.args;
            GetAction(_p.Action)(pd);
        }

        

    }

    public static class ConsoleLogicExtention
    {
        public static bool CallSingletonLogic(this ConsoleLogic.LogicCallHelper helper,string logicname, string logicaction)
        {
            var copyp = helper.Parameter.Clone<ConsoleParameter>();
            copyp.SetValue(ParameterKey.LOGIC, logicname);
            copyp.SetValue(ParameterKey.ACTION, logicaction);
            return ModuleProxyManager.Call<ConsoleBusinessSingletonProxy, ConsoleParameter, DataCollection>(copyp, helper.Data);
        }
    }
}
