using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;

namespace EFFC.Frame.Net.Business.Logic
{
    public interface ILogic
    {
        string Name { get; }
        void process(ParameterStd p, DataCollection d);
    }
}
