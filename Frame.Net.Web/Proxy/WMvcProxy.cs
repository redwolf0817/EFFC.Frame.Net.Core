using EFFC.Frame.Net.Base.Module;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;

namespace Frame.Net.Web.Proxy
{
    public class WMvcProxy<TModule> : LocalModuleProxy<WebParameter,WMvcData>
        where TModule: BaseModule<WebParameter, WMvcData>
    {
        public override void OnError(Exception ex, WebParameter p, WMvcData data)
        {
            throw ex;
        }
        protected override BaseModule<WebParameter, WMvcData> GetModule(WebParameter p, WMvcData data)
        {
            return Activator.CreateInstance<TModule>();
        }
    }
}
