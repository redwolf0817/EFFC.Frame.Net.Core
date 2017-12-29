using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Business.Module;
using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.Parameters.Flow;
using EFFC.Frame.Net.Flow.Module;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Flow.Module
{
    public class FlowBusinessModule : AssemblyLoadBusinessModule<FlowParameter, FlowData, FlowBaseLogic>
    {
        public override string LogicAssemblyPath
        {
            get { return GlobalCommon.FlowCommon.LogicAssemblyPath; }
        }

        public override string LogicName
        {
            get { return this.Parameter.CalledLogicName; }
        }

        public override string Description
        {
            get { return "交易業務處理模塊"; }
        }

        public override string Name
        {
            get { return "TradeBusiness"; }
        }

        public override string Version
        {
            get { return "0.0.1"; }
        }

        protected override void OnError(Exception ex, FlowParameter p, FlowData d)
        {
            throw ex;
        }
    }

    public class FlowBusinessSingletonModule : AssemblyLoadBusinessSingletonModule<FlowParameter, FlowData, FlowBaseLogic>
    {
        public override string LogicAssemblyPath
        {
            get { return GlobalCommon.FlowCommon.LogicAssemblyPath; }
        }

        public override string LogicName
        {
            get { return this.Parameter.CalledLogicName; }
        }

        public override string Description
        {
            get { return "交易業務處理模塊-单态"; }
        }

        public override string Name
        {
            get { return "TradeBusinessSington"; }
        }

        public override string Version
        {
            get { return "0.0.1"; }
        }

        protected override void OnError(Exception ex, FlowParameter p, FlowData d)
        {
            throw ex;
        }
    }
}
