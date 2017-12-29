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
        void Start(ParameterStd p, DataCollection d);
    }
}
