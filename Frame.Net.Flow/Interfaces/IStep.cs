using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.Parameters.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Flow.Interfaces
{
    public interface IStep<P,D>
        where P:FlowParameter
        where D:FlowData
    {
        string StepUID { get; }
        FlowStateType Run(P p, D d);
    }
}
