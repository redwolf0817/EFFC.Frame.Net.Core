using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Business.Engine;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Web.Extentions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Frame.Net.Web.Module
{
    /// <summary>
    /// Web请求处理的基类模块
    /// </summary>
    /// <typeparam name="WP"></typeparam>
    /// <typeparam name="WD"></typeparam>
    public abstract class WebBaseModule<WP, WD> : BaseModule<WP, WD>, IDisposable
        where WP : WebParameter
        where WD : WebBaseData
    {
        static object lockobj = new object();
        protected HttpContext CurrentContext
        {
            get
            {
                return _context;
            }
        }

        protected WebSocket CurrentSocket
        {
            get
            {
                return _socket;
            }
        }
        /// <summary>
        /// 根据User-agent判定浏览器类型
        /// </summary>
        /// <param name="useragent"></param>
        /// <returns></returns>
        public static string GetBrowserType(string useragent)
        {
            if (string.IsNullOrEmpty(useragent)) return "";

            if (useragent.Contains("MSIE") || useragent.Contains("Trident"))
            {
                return "IE";
            }
            if (useragent.Contains("Edge"))
            {
                return "Edge";
            }
            if (useragent.Contains("Chrome"))
            {
                return "Chrome";
            }
            if (useragent.Contains("Firefox"))
            {
                return "Firefox";
            }
            if (useragent.Contains("Safari") && !useragent.Contains("Chrome"))
            {
                return "Safari";
            }
            if (useragent.Contains("Opera"))
            {
                return "Opera";
            }
            return "Unknown";
        }
        /// <summary>
        /// 根据User-agent获取浏览器的版本号
        /// </summary>
        /// <param name="useragent"></param>
        /// <returns></returns>
        public static string GetBrowserVersion(string useragent)
        {
            if (string.IsNullOrEmpty(useragent)) return "";
            var browser = GetBrowserType(useragent);

            var regStr_ie = new Regex(@"(?<=MSIE )[\d.]+", RegexOptions.IgnoreCase);
            var regStr_ie2 = new Regex(@"(?<=rv:)[\d.]+", RegexOptions.IgnoreCase);
            var regStr_edge = new Regex(@"(?<=Edge/)[\d.]+", RegexOptions.IgnoreCase);
            var regStr_ff = new Regex(@"(?<=Firefox/)[\d.]+", RegexOptions.IgnoreCase);
            var regStr_chrome = new Regex(@"(?<=Chrome/)[\d.]+", RegexOptions.IgnoreCase);
            var regStr_saf = new Regex(@"(?<=Version/)[\d.]+", RegexOptions.IgnoreCase);
            var regStr_opr = new Regex(@"(?<=Version/)[\d.]+", RegexOptions.IgnoreCase);

            if (browser == "IE")
            {
                if (useragent.Contains("MSIE"))
                {
                    return regStr_ie.IsMatch(useragent) ? regStr_ie.Match(useragent).Value : "";
                }
                else
                {
                    return regStr_ie2.IsMatch(useragent) ? regStr_ie2.Match(useragent).Value : "";
                }

            }
            if (browser == "Edge")
            {
                return regStr_edge.IsMatch(useragent) ? regStr_edge.Match(useragent).Value : "";
            }
            if (browser == "Chrome")
            {
                return regStr_chrome.IsMatch(useragent) ? regStr_chrome.Match(useragent).Value : "";
            }
            if (browser == "Firefox")
            {
                return regStr_ff.IsMatch(useragent) ? regStr_ff.Match(useragent).Value : "";
            }
            if (browser == "Safari")
            {
                return regStr_saf.IsMatch(useragent) ? regStr_saf.Match(useragent).Value : "";
            }
            if (browser == "Opera")
            {
                return regStr_opr.IsMatch(useragent) ? regStr_opr.Match(useragent).Value : "";
            }
            return "";
        }
        HttpContext _context;
        WebSocket _socket;
        Uri _requesturi = null;
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

        protected abstract void InvokeAction(WP p, WD d);
        protected abstract void SetResponseContent(WP p, WD d);
        protected override void Run(WP p, WD d)
        {
            _context = p.CurrentHttpContext;
            _requesturi = new Uri($"{CurrentContext.Request.Scheme}://{CurrentContext.Request.Host}{CurrentContext.Request.Path}{CurrentContext.Request.QueryString}");
            var startdt = DateTime.Now;
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, string.Format("Before {0} Process Request Memory:{1}", _requesturi.AbsoluteUri, ComFunc.GetProcessUsedMemory()));
            
            if (IsWebSocket)
            {
                var socket = CurrentContext.WebSockets.AcceptWebSocketAsync().GetAwaiter().GetResult();
                ProcessWebSocketAsync(socket);
            }
            else
            {
                try
                {
                    //进行参数初始化
                    Init(p, d);
                    //业务逻辑操作
                    InvokeAction(p, d);
                    //session和cookie等的设置必须在response回写之前处理，否则会报异常
                    AfterProcess(p,d);
                    //进行response的回写
                    SetResponseContent(p, d);
                }catch(Exception ex)
                {
                    OnError(ex, p, d);
                }
                finally
                {
                    //释放hostjsview
                    //var hjv = (HostJsView)p.ExtentionObj.hostviewengine;
                    //hjv.Release();
                    p.Resources.ReleaseAll();
                    p.Dispose();
                    d.Dispose();
                    GC.Collect();
                }
            }
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, string.Format("After {0} Process Request Memory:{1}", _requesturi.AbsoluteUri, ComFunc.GetProcessUsedMemory()));
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, string.Format("Time cost {0}ms", (DateTime.Now-startdt).TotalMilliseconds));
        }

        private void ProcessWebSocketAsync(WebSocket arg)
        {
            WP p = Activator.CreateInstance<WP>();
            WD d = Activator.CreateInstance<WD>();
            p.ExtentionObj.websocket_uid = Guid.NewGuid().ToString();
            _socket = arg;
            try
            {

                DateTime expirationtime = DateTime.Now.AddMinutes(GlobalCommon.WebSocketCommon.MaxConnectionMinutes);
                Init(p, d);
                if (GlobalCommon.ApplicationCache.Get(p.ExtentionObj.websocket_uid + "websocket_expiration") == null)
                {
                    GlobalCommon.ApplicationCache.Set(p.ExtentionObj.websocket_uid + "websocket_expiration", expirationtime, DateTime.Now.AddDays(1));
                }
                CancellationTokenSource ct = new CancellationTokenSource();

                //Task.Factory.StartNew(() =>
                //{
                //    try
                //    {
                //        var isend = false;
                //        while (!isend)
                //        {
                //            expirationtime = (DateTime)GlobalCommon.ApplicationCache.Get(p.ExtentionObj.websocket_uid + "websocket_expiration");
                //            if (DateTime.Now > expirationtime)
                //            {
                //                ct.Cancel();
                //                isend = true;
                //                AfterProcess(_context, p, d);
                //                break;
                //            }

                //            Thread.Sleep(30 * 1000);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        OnError(ex, p, d);
                //    }
                //});
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
                        expirationtime = DateTime.Now.AddMinutes(GlobalCommon.WebSocketCommon.MaxConnectionMinutes);
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string userMsg = Encoding.UTF8.GetString(bl.ToArray());
                            if (FrameDLRObject.IsJson(userMsg))
                            {
                                FrameDLRObject jsondata = FrameDLRObject.CreateInstance(userMsg);
                                foreach (var k in jsondata.Keys)
                                {
                                    p[DomainKey.POST_DATA, k] = jsondata.GetValue(k);
                                }
                            }
                            else
                            {
                                p[DomainKey.POST_DATA, "ws_data"] = userMsg;
                            }
                        }
                        else
                        {
                            p[DomainKey.POST_DATA, "ws_data"] = bl.ToArray();
                        }

                        d.SetValue("websocket", _socket);
                        StepStart(p, d);
                        AfterProcess(p, d);
                    }
                    else
                    {
                        break;
                    }
                }

                if (_socket.State == WebSocketState.Closed)
                {
                    AfterProcess(p, d);
                }

            }
            catch (Exception ex)
            {
                OnError(ex, p, d);
            }
            finally
            {
                _socket.Abort();
            }
        }


        protected virtual void Init(WP p, WD d)
        {
            //获取请求的资源和参数
            ResourceManage rema = new ResourceManage();
            p.SetValue<ResourceManage>(ParameterKey.RESOURCE_MANAGER, rema);
            var defaulttoken = TransactionToken.NewToken();
            p.TransTokenList.Add(defaulttoken);
            p.SetValue<TransactionToken>(ParameterKey.TOKEN, defaulttoken);
            p.SetValue("IsAjaxAsync", IsAjaxAsync);
            //设置serverinfo
            p[DomainKey.APPLICATION_ENVIRONMENT, "server_servername"] = Environment.MachineName;
            p[DomainKey.APPLICATION_ENVIRONMENT, "serverinfo_ip"] = CurrentContext.Connection.LocalIpAddress;
            //设置clientinfo
            var ip = CurrentContext.Connection.RemoteIpAddress.ToString();

            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_ip"] = ip;
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_browserversion"] = GetBrowserType(ComFunc.nvl(CurrentContext.Request.Headers["user-agent"]));
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_platform"] = GetBrowserType(ComFunc.nvl(CurrentContext.Request.Headers["user-agent"]));
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_userhostname"] = CurrentContext.Connection.RemoteIpAddress.ToString();
            p.IsNeedSessionAbandon = false;
            p.IsWebSocket = CurrentContext.WebSockets.IsWebSocketRequest;

            //设置框架信息
            p[DomainKey.APPLICATION_ENVIRONMENT, "effcinfo_base_version"] = Base.Common.EFFCAssemblyInfo.Version;
            p[DomainKey.APPLICATION_ENVIRONMENT, "effcinfo_base_product_version"] = Base.Common.EFFCAssemblyInfo.ProductVersion;
            //hostview的引擎
            p.ExtentionObj.hostviewengine = new HostJsView();
            lock (lockobj)
            {
                Prepare(ref p, ref d);
            }
        }
        protected virtual void AfterProcess(WP p,WD d)
        {
            ProcessResponseSeesion(p,d);
            ProcessResponseCookie(p, d);
        }
        /// <summary>
        /// 处理Response的session数据
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void ProcessResponseSeesion(WP p,WD d)
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
        protected virtual void ProcessResponseCookie(WP p, WD d)
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

        

        public void Dispose()
        {
            if (!IsWebSocket)
            {
                _context = null;
                _socket = null;
            }
            else
            {
                if (_socket != null && _socket.State == WebSocketState.Closed)
                {
                    _context = null;
                    _socket = null;
                }
            }
        }
        #region 静态方法
        public static void Prepare(ref WP p, ref WD d)
        {
            var context = p.CurrentHttpContext;
            var location = new Uri($"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");

            var url = location.AbsoluteUri.Replace("\\", "/");
            if (url == "" && url == "/")
            {
                url = GlobalCommon.WebCommon.StartPage;
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


            context.Request.LoadEFFCParameters<WP>(ref p);
            lock (lockobj)
            {
                //websocket下无session
                if (!context.WebSockets.IsWebSocketRequest)
                {

                    //因websocket无session需要将cache中的数据域与session中同步一下
                    var sessionid = ComFunc.nvl(context.Request.Cookies["ASP.NET_SessionId"]);
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
                else
                {
                    var sessionid = ComFunc.nvl(context.Request.Cookies["ASP.NET_SessionId"]);
                    if (sessionid != "" && GlobalCommon.ApplicationCache.Get("__frame_session__" + sessionid) != null)
                    {
                        var sessionobj = (FrameDLRObject)GlobalCommon.ApplicationCache.Get("__frame_session__" + sessionid);
                        foreach (var key in sessionobj.Keys)
                        {
                            //深度复制session中的对象，防止p在最后释放时导致session对象丢失
                            p[DomainKey.SESSION, key] = ComFunc.CloneObject(sessionobj.GetValue(key));
                        }
                    }
                }


                //foreach (var s in context.Application.Keys)
                //{
                //    //深度复制application中的对象，防止p在最后释放时导致application对象丢失
                //    p[DomainKey.APPLICATION, s.ToString()] = ComFunc.CloneObject(context.Application[s.ToString()]);
                //}
            }

            p.ExtentionObj.cookie = FrameDLRObject.CreateInstance();
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

            p[DomainKey.SESSION, "SessionID"] = context.WebSockets.IsWebSocketRequest ? p[DomainKey.POST_DATA, "ASP.NET_SessionId"] : ComFunc.nvl(context.Request.Cookies["ASP.NET_SessionId"]);
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath", ((IHostingEnvironment)context.RequestServices.GetService(typeof(IHostingEnvironment))).ContentRootPath);
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath_URL", $"{context.Request.Scheme}://{context.Request.Host}");
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "WebPath", $"{context.Request.Scheme}://{context.Request.Host}");
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "Domain", location.Host);
        }
        
        #endregion
    }
}
