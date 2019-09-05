using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Module.Extend.EWRA;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using EFFC.Frame.Net.Module.Extend.WeixinWeb;
using EFFC.Frame.Net.Module.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPISample
{
    public class MyWebModule : EWRAGoExtend
    {
        protected override void OnUsed(ProxyManager ma, dynamic options)
        {
            base.OnUsed(ma, (object)options);
            ma.UseProxy<TagSimpleProxy>("tag", (object)options);
        }
        protected override void InvokeAction(EWRAParameter p, EWRAData d)
        {
            base.InvokeAction(p, d);
        }
    }
}
