using EFFC.Frame.Net.Business.Logic;
using EFFC.Frame.Net.Business.Logic.WebForm;
using EFFC.Frame.Net.Business.Module;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Web.Module
{
    public class WebFormBusinessModule : AssemblyLoadBusinessModule<WebParameter, WebFormData, WebFormLogic>
    {
        public override string LogicAssemblyPath
        {
            get { return GlobalCommon.WebFormCommon.LogicAssemblyPath; }
        }

        public override string LogicName
        {
            get { return this.Parameter.RequestResourceName; }
        }

        public override string Description
        {
            get { return "業務處理模塊"; }
        }

        public override string Name
        {
            get { return "WebFormBusiness"; }
        }

        public override string Version
        {
            get { return "0.0.1"; }
        }

        protected override void OnError(Exception ex, WebParameter p, WebFormData d)
        {
            throw ex;
        }
    }
}
