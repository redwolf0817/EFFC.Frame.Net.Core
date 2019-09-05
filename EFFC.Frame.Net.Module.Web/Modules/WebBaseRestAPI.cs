using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Module.Web.Datas;
using EFFC.Frame.Net.Module.Web.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using Microsoft.AspNetCore.Http;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Token;
using Microsoft.AspNetCore.Hosting;
using EFFC.Frame.Net.Module.Web.Extentions;
using Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Common;
using System.IO;

namespace EFFC.Frame.Net.Module.Web.Modules
{
    /// <summary>
    /// Rest API的基础框架模块
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public abstract class WebBaseRestAPI<TParameter, TData> : BaseModule
        where TParameter : RestParameter
        where TData : RestDataCollection
    {
        public override bool CheckParametersAndConfig(ParameterStd p, DataCollection d)
        {
            if (!(p is TParameter)) return false;
            if (!(d is TData)) return false;

            var rp = (TParameter)p;
            if (rp.CurrentHttpContext == null) return false;

            return true;
        }

        protected override void Run(ParameterStd p, DataCollection d)
        {
            var tp = (TParameter)p;
            var td = (TData)d;

            BeforeProcess(tp, td);
            InvokeAction(tp, td);
            AfterProcess(tp, td);
            SetResponseContent(tp, td);
            FinishedProcess(tp, td);

        }

        #region 定义生命周期流程
        protected virtual void BeforeProcess(TParameter p, TData d)
        {
            //获取请求的资源和参数
            ResourceManage rema = new ResourceManage();
            p.SetValue(ParameterKey.RESOURCE_MANAGER, rema);
            var defaulttoken = TransactionToken.NewToken();
            p.TransTokenList.Add(defaulttoken);
            p.SetValue(ParameterKey.TOKEN, defaulttoken);
            p.RequestUri = new Uri($"{p.CurrentHttpContext.Request.Scheme}://{p.CurrentHttpContext.Request.Host}{p.CurrentHttpContext.Request.Path}{p.CurrentHttpContext.Request.QueryString}");
            p.RequestRoute = p.CurrentHttpContext.Request.Path.Value;
            p.RestResourcesArray = p.CurrentHttpContext.Request.Path.Value.Split('/');

            ProcessRequestInfo(p, d);
            ProcessRequestHeader(p, d);

        }
        protected virtual void AfterProcess(TParameter p, TData d)
        {

        }

        protected virtual void FinishedProcess(TParameter p, TData d)
        {

        }
        #region AbstractMethod
        protected abstract void InvokeAction(TParameter p, TData d);
        protected abstract void SetResponseContent(TParameter p, TData d);
        #endregion
        #endregion

        #region Request
        /// <summary>
        /// 进行header的处理
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void ProcessRequestHeader(TParameter p, TData d)
        {
            var header = p.CurrentHttpContext.Request.Headers;
            foreach (var item in header)
            {
                p[DomainKey.HEADER, item.Key] = item.Value.ToString();
            }
        }
        /// <summary>
        /// 处理Request中的基本数据信息
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void ProcessRequestInfo(TParameter p, TData d)
        {
            var context = p.CurrentHttpContext;
            //设置serverinfo
            p[DomainKey.APPLICATION_ENVIRONMENT, "server_servername"] = Environment.MachineName;
            p[DomainKey.APPLICATION_ENVIRONMENT, "serverinfo_ip"] = p.CurrentHttpContext.Connection.LocalIpAddress;
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath", ((IHostingEnvironment)context.RequestServices.GetService(typeof(IHostingEnvironment))).ContentRootPath);
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath_URL", $"{context.Request.Scheme}://{context.Request.Host}");
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "WebPath", $"{context.Request.Scheme}://{context.Request.Host}");
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "Domain", p.RequestUri.Host);
            //设置clientinfo
            var ip = p.CurrentHttpContext.Connection.RemoteIpAddress.ToString();

            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_ip"] = ip;
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_browserversion"] = p.CurrentHttpContext.Request.GetBrowserType();
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_platform"] = p.CurrentHttpContext.Request.GetBrowserType();
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_userhostname"] = p.CurrentHttpContext.Connection.RemoteIpAddress.ToString();

            //设置框架信息
            var fai = FrameAssemblyInfo.From(typeof(ComFunc));
            p[DomainKey.APPLICATION_ENVIRONMENT, "effcinfo_base_version"] = fai.Version;
            p[DomainKey.APPLICATION_ENVIRONMENT, "effcinfo_base_product_version"] = fai.ProductVersion;

            //解析请求body数据
            context.Request.LoadEFFCRestParameters<TParameter>(ref p);
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 往response流中写入page内容
        /// </summary>
        /// <param name="pagepath">page的路径</param>
        /// <param name="p">参数集</param>
        /// <param name="d">结果集</param>
        protected virtual void ResponsePageContent(string pagepath, TParameter p, TData d)
        {
            var path = pagepath.Replace("~", ComFunc.nvl(p.GetValue(DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath")));
            if (File.Exists(path))
            {
                var resultmsg = File.ReadAllText(path);
                var msgbytelength = Encoding.UTF8.GetByteCount(resultmsg);
                p.CurrentHttpContext.Response.ContentType = ResponseHeader_ContentType.html + ";charset=utf-8";
                p.CurrentHttpContext.Response.Headers.Add("Content-Length", msgbytelength + "");
                p.CurrentHttpContext.Response.WriteAsync(resultmsg).Wait();
                p.CurrentHttpContext.Response.Body.Flush();
            }
        }
        #endregion
    }
}
