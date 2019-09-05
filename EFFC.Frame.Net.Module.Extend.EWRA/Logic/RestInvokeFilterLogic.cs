using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using EFFC.Frame.Net.Module.Web.Logic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Logic
{
    /// <summary>
    /// Rest可执行过滤器，用于控制是否执行接口方法，并返回对应的提示信息和statuscode
    /// </summary>
    public partial class RestInvokeFilterLogic : WebBaseLogic<EWRAParameter, EWRAData>
    {
        protected override void DoProcess(EWRAParameter p, EWRAData d)
        {
            Constants.RestStatusCode statuscode = Constants.RestStatusCode.NONE;
            var msg = "";
            d.IsInvoke = IsInvoke(p.RequestRoute, p.MethodName, p.ExtentionObj.invokelist,ref statuscode, ref msg);
            d.StatusCode = statuscode;
            d.Error = msg;
        }
        /// <summary>
        /// 根据路由、执行谓词和要执行的列表来判定是否需要执行
        /// </summary>
        /// <param name="requestRoute">请求的路由</param>
        /// <param name="methodverb">执行谓词</param>
        /// <param name="invokeList">根据路由和谓词找出的要执行的方法清单</param>
        /// <param name="statuscode">返回的状态码</param>
        /// <param name="msg">返回的提示信息</param>
        /// <returns></returns>
        protected virtual bool IsInvoke(string requestRoute,string methodverb, List<RouteInvokeEntity> invokeList,ref Constants.RestStatusCode statuscode, ref string errormsg)
        {
            return true;
        }
    }
}
