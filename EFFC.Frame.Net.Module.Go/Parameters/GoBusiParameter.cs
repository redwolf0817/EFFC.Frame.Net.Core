using EFFC.Frame.Net.Module.Business.Parameters;
using EFFC.Frame.Net.Module.Web.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.WebGo.Parameters
{
    public class GoBusiParameter:BusiModuleParameter
    {
        /// <summary>
        /// 从WebModule中传过来的parameter
        /// </summary>
        public WebParameter WebParam
        {
            get;
            set;
        }
    }
}
