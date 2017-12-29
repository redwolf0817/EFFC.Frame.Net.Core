using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Web.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Web.Proxy
{
    public class WebFormBusinessProxy:LocalModuleProxy<WebParameter,WebFormData>
    {
        protected override BaseModule<WebParameter, WebFormData> GetModule(WebParameter p, WebFormData data)
        {
            return new WebFormBusinessModule();
        }

        public override void OnError(Exception ex, WebParameter p, WebFormData data)
        {
            throw ex;
        }
    }
}
