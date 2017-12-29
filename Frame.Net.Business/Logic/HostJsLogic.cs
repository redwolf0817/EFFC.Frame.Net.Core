using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Business.Engine;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Logic
{
    public abstract partial class HostJsLogic : WebBaseLogic<WebParameter, GoData>
    {
        public override string Name
        {
            get { return "hostjslogic"; }
        }
        /// <summary>
        /// 获取logic js文件的根路径
        /// </summary>
        /// <returns></returns>
        protected abstract string GetLogicRootPath();

        protected override void DoInvoke(WebParameter p, GoData d, Data.LogicData.LogicData ld)
        {
            var logicfile = p.RequestResourceName + (ComFunc.nvl(p.Action) != "" ? "." + p.Action : "") + ".hjs";
            var path = GetLogicRootPath() + @"/" + logicfile;
            HostLogicEngine hle = GetHostLogicEngine();
            var js = File.ReadAllText(path);
            var result = hle.RunJs(js);
            d.ResponseData = result;
        }

        protected virtual HostLogicEngine GetHostLogicEngine()
        {
            var context = new HostLogicContext();
            return new HostLogicEngine(context, this);
        }
    }
}
