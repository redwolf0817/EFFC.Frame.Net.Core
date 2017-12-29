using EFFC.Frame.Net.Module.Business;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Business.Parameters;
using EFFC.Frame.Net.Module.Extend.WebGo.Logic;
using EFFC.Frame.Net.Module.Business.Datas;
using EFFC.Frame.Net.Module.Web.Parameters;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Extend.WebGo.Parameters;

namespace EFFC.Frame.Net.Module.Extend.WebGo
{
    public class GoBusiModule : BusiModule<GoLogic,GoBusiParameter,GoBusiData>
    {
        public override string Name => "GoBusiModule";

        public override string Description => "Go请求处理的业务模块";

        protected override void AfterProcess(GoBusiParameter p, GoBusiData d)
        {
            
        }

        protected override bool DoCheckMyParametersAndConfig(GoBusiParameter p, GoBusiData d)
        {
            return true;
        }
    }
}
