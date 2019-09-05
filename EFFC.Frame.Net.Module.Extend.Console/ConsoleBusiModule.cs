using EFFC.Frame.Net.Module.Business;
using EFFC.Frame.Net.Module.Extend.EConsole.DataCollections;
using EFFC.Frame.Net.Module.Extend.EConsole.Logic;
using EFFC.Frame.Net.Module.Extend.EConsole.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EConsole
{
    public class ConsoleBusiModule : BusiModule<ConsoleLogic, ConsoleParameter, ConsoleData>
    {
        public override string Name => "ConsoleModule";

        public override string Description => "用于普通批次程式使用的业务逻辑模型";

        protected override void AfterProcess(ConsoleParameter p, ConsoleData d)
        {
            p.Resources.CommitTransaction(p.CurrentTransToken);
        }

        protected override bool DoCheckMyParametersAndConfig(ConsoleParameter p, ConsoleData d)
        {
            return true;
        }

    }
}
