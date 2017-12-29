using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;

namespace EFFC.Frame.Net.Base.Interfaces.System
{
    public interface IExceptionProcess
    {
        void ProcessException(object sender, Exception ex, ParameterStd p, DataCollection d);
    }
}
