using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using System.Threading;

namespace EFFC.Frame.Net.Base.Interfaces.Core
{
    public interface IDriver
    {
        void StepStart(ParameterStd p, DataCollection d);
    }

    public interface IDriver<P, D>
        where P : ParameterStd
        where D : DataCollection
    {
        void StepStart(P p, D d);
    }
}
