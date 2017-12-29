using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.Parameters.Flow;
using EFFC.Frame.Net.Flow.Exceptions;
using EFFC.Frame.Net.Flow.Interfaces;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EFFC.Frame.Net.Flow.Core
{
    public abstract class FlowBaseDefine : IFlowDefine<FlowParameter, FlowData>
    {
        public virtual void Prepare(FlowParameter p, FlowData d)
        {

        }
        protected abstract ConditionBaseDriver NextDriver(FlowParameter p, FlowData d);
        public IConditionDriver<FlowParameter, FlowData> GetDriver(FlowParameter p, FlowData d)
        {
            Prepare(p, d);

            return NextDriver(p, d);
        }

        public abstract string FlowName
        {
            get;
        }

        public abstract FlowVersion Version
        {
            get;
        }
    }
}
