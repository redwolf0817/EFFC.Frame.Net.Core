using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Business.Logic;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Module
{
    public abstract class LocalLoadHostBusinessModule<L, P, D> : BaseModule<P, D>
        where L : HostJsLogic
        where P : ParameterStd
        where D : DataCollection
    {
        protected override void Run(P p, D d)
        {
            var l = Activator.CreateInstance<L>();
            l.process(p, d);
        }

    }
}
