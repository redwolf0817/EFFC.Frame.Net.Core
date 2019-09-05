using EFFC.Frame.Net.Module.Web.Options;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPISample
{
    public class MyWebOptions:WebMiddleWareProcessOptions
    {
        public override string ConvertExtTo(HttpContext context, string requestExtType)
        {
            if(new string[] { "html", "htm" }.Contains(requestExtType))
            {
                return "go";
            }
            else
            {
                return base.ConvertExtTo(context, requestExtType);
            }
        }

        public override string PagePath4Forbidden => "~/views/admissions.html";
        public override string PagePath4NotFound => "~/views/contact.html";
    }
}
