using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frame.Net.Web.Proxy
{
    public class GoProxy<TModule> : LocalModuleProxy<WebParameter, GoData>
        where TModule: BaseModule<WebParameter, GoData>
    {
        public override void OnError(Exception ex, WebParameter p, GoData data)
        {
            throw ex;
        }

        protected override BaseModule<WebParameter, GoData> GetModule(WebParameter p, GoData data)
        {
            return Activator.CreateInstance<TModule>();
        }
    }
}
