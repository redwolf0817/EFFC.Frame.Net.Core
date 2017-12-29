using EFFC.Frame.Net.Base.Module;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Web.Parameters;
using EFFC.Frame.Net.Module.Web.Datas;
using EFFC.Frame.Net.Module.Web.Options;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Common;
using System.IO;
using EFFC.Frame.Net.Module.Web.Extentions;
using EFFC.Frame.Net.Base.Data.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using Frame.Net.Base.Common;

namespace EFFC.Frame.Net.Module.Web.Modules
{
    /// <summary>
    /// WebSocket请求处理基类
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public abstract class WebSocketBaseModule<TParameter, TData> : BaseModule
        where TParameter : WebParameter
        where TData : WebBaseData
    {
        static WebSocketOptions Settings = new WebSocketOptions();
        protected override void OnUsed(ProxyManager ma, dynamic options)
        {
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "Websocket服务启动设定...");
            if (options != null)
            {
                if(options.WebSocket_Max_Connection_Live_Minute != null)
                {
                    Settings.WebSocket_Max_Connection_Live_Minute = options.WebSocket_Max_Connection_Live_Minute;
                }
                if (options.StartPage != null)
                {
                    Settings.StartPage = options.StartPage;
                }
            }
            if (Settings == null) Settings = new WebSocketOptions();
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO,
                        string.Format("Websocket的链接最大存活时间设定为{0}分钟，如要调整，请在ProxyManager.UseProxy中的options参数设定WebSocket_Max_Connection_Live_Minute的值", Settings.WebSocket_Max_Connection_Live_Minute));
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO,
                        string.Format("Web服务StartPage设定为{0},如要调整，请在ProxyManager.UseProxy中的options参数设定StartPage的值", Settings.StartPage));
            //加载模块内需要的代理器
            DoAddProxy(ma);
        }
        /// <summary>
        /// 添加模块内需要用到的代理器
        /// </summary>
        /// <param name="ma"></param>
        protected abstract void DoAddProxy(ProxyManager ma);
        #region Static Define
        static object lockobj = new object();
        #endregion
        HttpContext _context;
        WebSocket _socket;
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
        /// 当前的websocket
        /// </summary>
        protected WebSocket CurrentSocket
        {
            get
            {
                return _socket;
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

            var socket = CurrentContext.WebSockets.AcceptWebSocketAsync().GetAwaiter().GetResult();
            ProcessWebSocketAsync(socket);
        }

        private void ProcessWebSocketAsync(WebSocket arg)
        {
            TParameter tp = Activator.CreateInstance<TParameter>();
            TData td = Activator.CreateInstance<TData>();
            tp.ExtentionObj.websocket_uid = Guid.NewGuid().ToString();
            _socket = arg;
            try
            {

                DateTime expirationtime = DateTime.Now.AddMinutes(Settings.WebSocket_Max_Connection_Live_Minute);
                BeforeProcess(tp, td);
                if (GlobalCommon.ApplicationCache.Get(tp.ExtentionObj.websocket_uid + "websocket_expiration") == null)
                {
                    GlobalCommon.ApplicationCache.Set(tp.ExtentionObj.websocket_uid + "websocket_expiration", expirationtime, DateTime.Now.AddDays(1));
                }
                CancellationTokenSource ct = new CancellationTokenSource();

                while (true)
                {
                    if (_socket.State == WebSocketState.Open)
                    {
                        //设置websockect能接受的数据大小不受限制，但单次接收的数据最多只有4088个byte，websocket对于大数据会分多段传送为，因此buffer定为4K
                        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
                        List<byte> bl = new List<byte>();
                        var t = _socket.ReceiveAsync(buffer, ct.Token);
                        Task.WaitAll(t);
                        WebSocketReceiveResult result = t.Result;
                        if (_socket.State == WebSocketState.CloseReceived)
                        {
                            break;
                        }
                        bl.AddRange(buffer.Array.Take(result.Count));
                        while (!result.EndOfMessage)
                        {
                            t = _socket.ReceiveAsync(buffer, ct.Token);
                            Task.WaitAll(t);
                            result = t.Result;
                            bl.AddRange(buffer.Array.Take(result.Count));
                        }
                        //重置超时时间
                        expirationtime = DateTime.Now.AddMinutes(Settings.WebSocket_Max_Connection_Live_Minute);
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string userMsg = Encoding.UTF8.GetString(bl.ToArray());
                            if (FrameDLRObject.IsJson(userMsg))
                            {
                                FrameDLRObject jsondata = FrameDLRObject.CreateInstance(userMsg);
                                foreach (var k in jsondata.Keys)
                                {
                                    tp[DomainKey.POST_DATA, k] = jsondata.GetValue(k);
                                }
                            }
                            else
                            {
                                tp[DomainKey.POST_DATA, "ws_data"] = userMsg;
                            }
                        }
                        else
                        {
                            tp[DomainKey.POST_DATA, "ws_data"] = bl.ToArray();
                        }

                        td.SetValue("websocket", _socket);
                        InvokeAction(tp, td);
                        AfterProcess(tp, td);
                    }
                    else
                    {
                        break;
                    }
                }

                if (_socket.State == WebSocketState.Closed)
                {
                    AfterProcess(tp, td);
                }

            }
            catch (Exception ex)
            {
                OnError(ex, tp, td);
            }
            finally
            {
                _socket.Abort();
            }
        }
        #region SelfMethod
        protected virtual void BeforeProcess(TParameter p, TData d)
        {
            //获取请求的资源和参数
            ResourceManage rema = new ResourceManage();
            p.SetValue(ParameterKey.RESOURCE_MANAGER, rema);
            var defaulttoken = TransactionToken.NewToken();
            p.TransTokenList.Add(defaulttoken);
            p.SetValue(ParameterKey.TOKEN, defaulttoken);
            p.SetValue("IsAjaxAsync", false);

            ProcessRequestInfo(p, d);
            ProcessRequestSession(p, d);
            ProcessResponseCookie(p, d);
        }
        protected virtual void AfterProcess(TParameter p, TData d)
        {
            ProcessResponseSeesion(p, d);
            ProcessResponseCookie(p, d);
        }
        #region Request
        /// <summary>
        /// 处理Request中的基本数据信息
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void ProcessRequestInfo(TParameter p, TData d)
        {
            var context = p.CurrentHttpContext;
            p.RequestUri = new Uri($"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
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
            p.IsNeedSessionAbandon = false;
            p.IsWebSocket = CurrentContext.WebSockets.IsWebSocketRequest;

            //设置框架信息
            var fai = FrameAssemblyInfo.From(typeof(ComFunc));
            p[DomainKey.APPLICATION_ENVIRONMENT, "effcinfo_base_version"] = fai.Version;
            p[DomainKey.APPLICATION_ENVIRONMENT, "effcinfo_base_product_version"] = fai.ProductVersion;
            //抓取请求资源
            var url = p.RequestUri.AbsoluteUri.Replace("\\", "/");
            if (url == "" && url == "/")
            {
                url = Settings.StartPage;
            }

            p.RequestResourcePath = ComFunc.nvl(context.Request.Path);
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
                p.Action = ss.Length > 1 ? ss[1] : "";
                p.RequestResourceName = ss.Length > 0 ? ss[0] : "";
                p.RequestResources = ss;
            }

            //解析请求body数据
            context.Request.LoadEFFCParameters<TParameter>(ref p);
        }
        /// <summary>
        /// 处理request中的session信息
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void ProcessRequestSession(TParameter p, TData d)
        {
            var sessionid = ComFunc.nvl(p.CurrentHttpContext.Request.Cookies["ASP.NET_SessionId"]);
            if (sessionid != "" && GlobalCommon.ApplicationCache.Get("__frame_session__" + sessionid) != null)
            {
                var sessionobj = (FrameDLRObject)GlobalCommon.ApplicationCache.Get("__frame_session__" + sessionid);
                foreach (var key in sessionobj.Keys)
                {
                    //深度复制session中的对象，防止p在最后释放时导致session对象丢失
                    p[DomainKey.SESSION, key] = ComFunc.CloneObject(sessionobj.GetValue(key));
                }
            }

            p[DomainKey.SESSION, "SessionID"] = sessionid;
        }
        /// <summary>
        /// 处理Request中的cookie数据
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void ProcessRequestCookie(TParameter p, TData d)
        {
            var context = p.CurrentHttpContext;
            if (p.ExtentionObj.cookie != null) p.ExtentionObj.cookie = FrameDLRObject.CreateInstance();
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

        #region Response
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
        #endregion

        #endregion
        #region AbstractMethod
        protected abstract void InvokeAction(TParameter p, TData d);
        protected abstract void SetResponseContent(TParameter p, TData d);
        #endregion
    }
}
