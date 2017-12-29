using EFFC.Frame.Net.Base.Module;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using EFFC.Frame.Net.Base.Common;
using System.Linq;
using EFFC.Frame.Net.Module.Web.Parameters;
using EFFC.Frame.Net.Module.Web.Datas;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Module.Web.Options;
using System.Threading;
using System.Threading.Tasks;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using Frame.Net.Base.Common;
using System.IO;
using EFFC.Frame.Net.Module.Web.Extentions;
using Microsoft.AspNetCore.Hosting;

namespace EFFC.Frame.Net.Module.Web.Modules
{
    /// <summary>
    /// Web请求处理的基类模块
    /// </summary>
    public abstract class WebBaseModule<TParameter, TData> : BaseModule
        where TParameter:WebParameter
        where TData:WebBaseData
    {
        static WebOptions Settings = new WebOptions();
        protected override void OnUsed(ProxyManager ma, dynamic options)
        {
            BeforeOnUsed(ma, options);
            if (options != null)
            {
                if(ComFunc.nvl(options.StartPage) != null)
                {
                    Settings.StartPage = options.StartPage;
                }
                if (ComFunc.nvl(options.RootHome) != "")
                {
                    Settings.RootHome = options.RootHome;
                }
            }

            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO,
                        string.Format("{1}起始页StartPage设定为{0},如要调整，请在ProxyManager.UseProxy中的options参数设定StartPage的值", Settings.StartPage,this.GetType().Name));
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO,
                        string.Format("{1}中RootHome设定为{0},如要调整，请在ProxyManager.UseProxy中的options参数设定RootHome的值", Settings.RootHome, this.GetType().Name));
            //加载模块内需要的代理器
            DoAddProxy(ma, options);
        }
        /// <summary>
        /// 添加模块内需要用到的代理器
        /// </summary>
        /// <param name="ma"></param>
        protected virtual void DoAddProxy(ProxyManager ma,dynamic options)
        {
            //do nothing
        }
        protected virtual void BeforeOnUsed(ProxyManager ma, dynamic options)
        {
            //do nothing
        }

        #region Static Define
        static object lockobj = new object();
        #endregion
        HttpContext _context;
        Uri _requesturi = null;
        /// <summary>
        /// 当前的HttpContext
        /// </summary>
        protected HttpContext CurrentContext
        {
            get
            {
                return _context;
            }
        }
        /// <summary>
        /// 判断是否为ajax异步调用
        /// </summary>
        protected bool IsAjaxAsync
        {
            get
            {
                IHeaderDictionary headersDictionary = _context.Request.Headers;
                if (ComFunc.nvl(_context.Request.Headers["x-requested-with"].FirstOrDefault()) == "XMLHttpRequest")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 是否为websocket请求
        /// </summary>
        protected bool IsWebSocket
        {
            get
            {
                return _context.WebSockets.IsWebSocketRequest;
            }
        }
        #region ImplementModule
        public override bool CheckParametersAndConfig(ParameterStd p, DataCollection d)
        {
            if (!(p is TParameter)) return false;
            if (!(d is TData)) return false;

            var tp = (TParameter)p;
            var td = (TData)d;

            return true;
        }

        protected override void Run(ParameterStd p, DataCollection d)
        {
            var tp = (TParameter)p;
            var td = (TData)d;

            _context = tp.CurrentHttpContext;
            //var startdt = DateTime.Now;
            //GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, string.Format("Before {0} Process Request Memory:{1}", _requesturi.AbsoluteUri, ComFunc.GetProcessUsedMemory()));
            var dt = DateTime.Now;
            var dtstart = DateTime.Now;
            //进行参数初始化
            BeforeProcess(tp, td);
            //GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"excute:{_context.Request.Method} \"{tp.RequestUri}\" BeforeProcess cast time:{(DateTime.Now - dt).TotalMilliseconds}ms "); 
            dt = DateTime.Now;
            //业务逻辑操作
            InvokeAction(tp, td);
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"excute:{_context.Request.Method} \"{tp.RequestUri}\" InvokeAction cast time:{(DateTime.Now - dt).TotalMilliseconds}ms "); dt = DateTime.Now;
            //session和cookie等的设置必须在response回写之前处理，否则会报异常
            AfterProcess(tp, td);
            //GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"excute:{_context.Request.Method} \"{tp.RequestUri}\" AfterProcess cast time:{(DateTime.Now - dt).TotalMilliseconds}ms "); 
            dt = DateTime.Now;
            //进行response的回写
            SetResponseContent(tp, td);
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"excute:{_context.Request.Method} \"{tp.RequestUri}\" SetResponseContent cast time:{(DateTime.Now - dt).TotalMilliseconds}ms "); dt = DateTime.Now;
            //收尾作业
            FinishedProcess(tp, td);
            //GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"excute:{_context.Request.Method} \"{tp.RequestUri}\" FinishedProcess cast time:{(DateTime.Now - dt).TotalMilliseconds}ms "); dt = DateTime.Now;

            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"excute:{_context.Request.Method} \"{tp.RequestUri}\" cast time:{(DateTime.Now - dtstart).TotalMilliseconds}ms ");

            //GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, string.Format("After {0} Process Request Memory:{1}", _requesturi.AbsoluteUri, ComFunc.GetProcessUsedMemory()));
            //GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, string.Format("Time cost {0}ms", (DateTime.Now - startdt).TotalMilliseconds));
        }
        public override void Dispose()
        {
            _context = null;
            //释放hostjsview
            //var hjv = (HostJsView)p.ExtentionObj.hostviewengine;
            //hjv.Release();
        }
        #endregion

        #region SelfMethod



        protected virtual void BeforeProcess(TParameter p, TData d)
        {
            
            //获取请求的资源和参数
            ResourceManage rema = new ResourceManage();
            p.SetValue(ParameterKey.RESOURCE_MANAGER, rema);
            var defaulttoken = TransactionToken.NewToken();
            p.TransTokenList.Add(defaulttoken);
            p.SetValue(ParameterKey.TOKEN, defaulttoken);
            p.SetValue("IsAjaxAsync", IsAjaxAsync);
            p.RequestUri = new Uri($"{p.CurrentHttpContext.Request.Scheme}://{p.CurrentHttpContext.Request.Host}{p.CurrentHttpContext.Request.Path}{p.CurrentHttpContext.Request.QueryString}");

            ProcessRequestPath(p, d);
            ProcessRequestHeader(p, d);
            ProcessRequestInfo(p, d);
            ProcessRequestSession(p, d);
            ProcessRequestCookie(p, d);
            ////hostview的引擎
            //p.ExtentionObj.hostviewengine = new HostJsView();

        }
        protected virtual void AfterProcess(TParameter p, TData d)
        {
            ProcessResponseSeesion(p, d);
            ProcessResponseCookie(p, d);
        }

        protected virtual void FinishedProcess(TParameter p, TData d)
        {

        }
        #region Request
        protected virtual void ProcessRequestPath(TParameter p,TData d)
        {
            var context = p.CurrentHttpContext;
            //抓取请求资源
            if(Settings.RootHome.Replace("~", "") != "/")
            {
                p.RequestResourcePath = ComFunc.nvl(context.Request.Path).Replace("\\", "/").Replace(Settings.RootHome.Replace("~", ""), "");
            }
            else
            {
                p.RequestResourcePath = ComFunc.nvl(context.Request.Path).Replace("\\", "/");
            }
            
            if (p.RequestResourceName == "" && p.RequestResourcePath == "/")
            {
                p.RequestResourcePath = p.RequestResourcePath + Settings.StartPage;
            }

            var ext = Path.GetExtension(p.RequestResourcePath);
            if (ext != "")
            {
                string reqpath = Path.GetFileNameWithoutExtension(p.RequestResourcePath);
                string[] ss = reqpath.Split('.');
                p.Action = ss.Length > 1 ? ss[1] : "";
                p.RequestResourceName = ss.Length > 0 ? ss[0] : "";
                p.RequestResources = ss;
            }
            else
            {
                var turl = p.RequestResourcePath.Replace("~", "");
                turl = turl.StartsWith("/") ? turl.Substring(1) : turl;
                string[] ss = turl.Split('/');
                //action取最后一个
                p.Action = ss.Length > 1 ? ss[ss.Length-1] : "";
                //login取倒数第二个
                p.RequestResourceName = ss.Length > 0 ? ss[0] : "";
                p.RequestResources = ss;
            }
        }
        /// <summary>
        /// 进行header的处理
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void ProcessRequestHeader(TParameter p, TData d)
        {
            var header = p.CurrentHttpContext.Request.Headers;
            foreach(var item in header)
            {
                p[DomainKey.HEADER, item.Key] = item.Value.ToString();
            }
        }
        /// <summary>
        /// 处理Request中的基本数据信息
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void ProcessRequestInfo(TParameter p,TData d)
        {
            var context = p.CurrentHttpContext;

            p.IsNeedSessionAbandon = false;
            p.IsWebSocket = CurrentContext.WebSockets.IsWebSocketRequest;
            //设置serverinfo
            p[DomainKey.APPLICATION_ENVIRONMENT, "server_servername"] = Environment.MachineName;
            p[DomainKey.APPLICATION_ENVIRONMENT, "serverinfo_ip"] = CurrentContext.Connection.LocalIpAddress;
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath", ((IHostingEnvironment)context.RequestServices.GetService(typeof(IHostingEnvironment))).ContentRootPath);
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath_URL", $"{context.Request.Scheme}://{context.Request.Host}");
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "WebPath", $"{context.Request.Scheme}://{context.Request.Host}");
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "Domain", p.RequestUri.Host);
            //设置clientinfo
            var ip = CurrentContext.Connection.RemoteIpAddress.ToString();

            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_ip"] = ip;
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_browserversion"] = CurrentContext.Request.GetBrowserType();
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_platform"] = CurrentContext.Request.GetBrowserType();
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_userhostname"] = CurrentContext.Connection.RemoteIpAddress.ToString();

            //设置框架信息
            var fai = FrameAssemblyInfo.From(typeof(ComFunc));
            p[DomainKey.APPLICATION_ENVIRONMENT, "effcinfo_base_version"] = fai.Version;
            p[DomainKey.APPLICATION_ENVIRONMENT, "effcinfo_base_product_version"] = fai.ProductVersion;

            //解析请求body数据
            context.Request.LoadEFFCParameters<TParameter>(ref p);
        }
        /// <summary>
        /// 处理request中的session信息
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void ProcessRequestSession(TParameter p,TData d)
        {
            var context = p.CurrentHttpContext;
            var sessionid = ComFunc.nvl(context.Request.Cookies["ASP.NET_SessionId"]);
            lock (lockobj)
            {
                //websocket下无session
                //因websocket无session需要将cache中的数据域与session中同步一下
                if (sessionid != "" && GlobalCommon.ApplicationCache.Get("__frame_session__" + sessionid) != null)
                {
                    var sessionobj = (FrameDLRObject)GlobalCommon.ApplicationCache.Get("__frame_session__" + sessionid);
                    foreach (var key in sessionobj.Keys)
                    {
                        context.Session.SetObject(key, sessionobj.GetValue(key));
                    }
                    var sessionkeys = context.Session.Keys.Cast<string>().ToArray(); ;
                    var removekeys = new List<string>();
                    foreach (string s in sessionkeys)
                    {
                        if (sessionobj.GetValue(s) == null)
                        {
                            removekeys.Add(s);
                        }
                    }
                    foreach (var s in removekeys)
                    {
                        context.Session.Remove(s);
                    }
                }
                var skeys = context.Session.Keys.Cast<string>().ToArray();
                foreach (string s in skeys)
                {
                    //深度复制session中的对象，防止p在最后释放时导致session对象丢失
                    p[DomainKey.SESSION, s] = ComFunc.CloneObject(context.Session.GetObject(s));
                }
            }

            

            p[DomainKey.SESSION, "SessionID"] = sessionid;
        }
        /// <summary>
        /// 处理Request中的cookie数据
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void ProcessRequestCookie(TParameter p,TData d)
        {
            var context = p.CurrentHttpContext;
            if (p.ExtentionObj.cookie == null)p.ExtentionObj.cookie = FrameDLRObject.CreateInstance();
            if (context.Request.Cookies != null && context.Request.Cookies.Count > 0)
            {
                foreach (var key in context.Request.Cookies.Keys)
                {
                    if (key != "ASP.NET_SessionId")
                    {
                        FrameDLRObject item = FrameDLRObject.CreateInstance();
                        item.SetValue("name", key);
                        item.SetValue("value", context.Request.Cookies[key]);

                        ((FrameDLRObject)p.ExtentionObj.cookie).SetValue(ComFunc.nvl(item.GetValue("name")), item);
                    }
                }
            }
        }
        #endregion
        /// <summary>
        /// 处理Response的session数据
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void ProcessResponseSeesion(TParameter p, TData d)
        {
            lock (lockobj)
            {
                var sessionid = ComFunc.nvl(CurrentContext.Request.Cookies["ASP.NET_SessionId"]);
                if (p.IsNeedSessionAbandon)
                {
                    CurrentContext.Session.Clear();
                    GlobalCommon.ApplicationCache.Remove("__frame_session__" + sessionid);
                }
                else
                {
                    IEnumerable<string> keys = p.Domain(DomainKey.SESSION).Keys;
                    //Websocket下无session，将session对象写入ApplicationCache中用于同步

                    if (!string.IsNullOrEmpty(sessionid))
                    {
                        var removeobj = GlobalCommon.ApplicationCache.Get("__frame_session__" + sessionid);
                        if (removeobj != null)
                        {
                            ((FrameDLRObject)removeobj).Dispose();
                        }
                        GlobalCommon.ApplicationCache.Remove("__frame_session__" + sessionid);
                        FrameDLRObject sessionobj = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                        foreach (string s in keys)
                        {
                            sessionobj.SetValue(s, ComFunc.CloneObject(p[DomainKey.SESSION, s]));
                        }
                        GlobalCommon.ApplicationCache.Set("__frame_session__" + sessionid, sessionobj, TimeSpan.FromMinutes(20));
                    }
                    //websocket下没有session
                    if (CurrentContext.Session != null)
                    {
                        foreach (string s in keys)
                        {
                            CurrentContext.Session.SetObject(s, ComFunc.CloneObject(p[DomainKey.SESSION, s]));
                        }
                    }


                    if (!IsWebSocket)
                    {
                        keys = CurrentContext.Session.Keys.Cast<string>().Except(keys).ToArray();
                        foreach (string s in keys)
                        {
                            CurrentContext.Session.Remove(s);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 处理Response中的cookie信息
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void ProcessResponseCookie(TParameter p, TData d)
        {
            //处理cookie
            var cookies = (FrameDLRObject)p.ExtentionObj.cookie;
            if (cookies != null)
            {
                var addcookies = p.ExtentionObj.cookie.add == null ? (FrameDLRObject)FrameDLRObject.CreateInstance() : (FrameDLRObject)p.ExtentionObj.cookie.add;
                var removecookies = p.ExtentionObj.cookie.remove == null ? (FrameDLRObject)FrameDLRObject.CreateInstance() : (FrameDLRObject)p.ExtentionObj.cookie.remove;
                foreach (var key in addcookies.Keys)
                {
                    var item = (FrameDLRObject)addcookies.GetValue(key);
                    var name = ComFunc.nvl(item.GetValue("name"));
                    var value = ComFunc.nvl(item.GetValue("value"));
                    var domain = ComFunc.nvl(item.GetValue("domain"));
                    var expire = (DateTime)item.GetValue("expire");

                    CurrentContext.Response.Cookies.Append(name, value, new CookieOptions() { Expires = expire, Domain = domain });
                }

                foreach (var key in removecookies.Keys)
                {
                    var item = (FrameDLRObject)removecookies.GetValue(key);
                    var name = ComFunc.nvl(item.GetValue("name"));
                    CurrentContext.Response.Cookies.Delete(name);
                }
            }
        }
        #endregion

        #region AbstractMethod
        protected abstract void InvokeAction(TParameter p, TData d);
        protected abstract void SetResponseContent(TParameter p, TData d);
        #endregion
    }
}
