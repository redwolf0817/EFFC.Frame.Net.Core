using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Base.Data.Base;
using System.Net.WebSockets;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using EFFC.Frame.Net.Global;
using System.Text.RegularExpressions;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Business.Engine;
namespace EFFC.Frame.Net.Web.Core
{
    public abstract class WebBaseHandler<WP, WD> : BaseModule<WP, WD>, IHttpHandler, IRequiresSessionState,IDisposable
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

        HttpContext _context;
        WebSocket _socket;
        /// <summary>
        /// 判断是否为ajax异步调用
        /// </summary>
        protected bool IsAjaxAsync
        {
            get
            {
                if (_context.Request.Headers["x-requested-with"] != null && _context.Request.Headers["x-requested-with"] == "XMLHttpRequest")
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
                return _context.IsWebSocketRequest;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, string.Format("Before {0} Process Request Memory:{1}" , context.Request.Url, ComFunc.GetProcessUsedMemory()));
            _context = context;
            if (IsWebSocket)
            {
                context.AcceptWebSocketRequest(ProcessWebSocket);
            }
            else
            {
                WP p = Activator.CreateInstance<WP>();
                WD d = Activator.CreateInstance<WD>();
                try
                {
                    Init(context, p, d);
                    this.StepStart(p, d);
                    AfterProcess(context, p, d);
                }
                catch (Exception ex)
                {
                    OnError(ex, p, d);
                }
                finally
                {
                    //释放hostjsview
                    //var hjv = (HostJsView)p.ExtentionObj.hostviewengine;
                    //hjv.Release();
                    p.Dispose();
                    d.Dispose();
                    GC.Collect();
                }
            }
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, string.Format("After {0} Process Request Memory:{1}", context.Request.Url, ComFunc.GetProcessUsedMemory()));
        }

        private async Task ProcessWebSocket(System.Web.WebSockets.AspNetWebSocketContext arg)
        {
            WP p = Activator.CreateInstance<WP>();
            WD d = Activator.CreateInstance<WD>();
            p.ExtentionObj.websocket_uid = Guid.NewGuid().ToString();
            WebSocket socket = arg.WebSocket;
            _socket = socket;
            try
            {

                DateTime expirationtime = DateTime.Now.AddMinutes(GlobalCommon.WebSocketCommon.MaxConnectionMinutes);
                Init(_context, p, d);
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
                    if (socket.State == WebSocketState.Open)
                    {
                        //设置websockect能接受的数据大小不受限制，但单次接收的数据最多只有4088个byte，websocket对于大数据会分多段传送为，因此buffer定为4K
                        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
                        List<byte> bl = new List<byte>();
                        WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, ct.Token);
                        if (socket.State == WebSocketState.CloseReceived)
                        {
                            break;
                        }
                        bl.AddRange(buffer.Array.Take(result.Count));
                        while (!result.EndOfMessage)
                        {
                            result = await socket.ReceiveAsync(buffer, ct.Token);
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
                        
                        d.SetValue("websocket", socket);
                        StepStart(p, d);
                        AfterProcess(_context, p, d);
                    }
                    else
                    {
                        break;
                    }
                }

                if (socket.State == WebSocketState.Closed)
                {
                    AfterProcess(_context, p, d);
                }

            }
            catch (Exception ex)
            {
                OnError(ex, p, d);
            }
            finally
            {
                socket.Abort();
            }
        }
        

        protected virtual void Init(HttpContext context, WP p, WD d)
        {
            //获取请求的资源和参数
            ResourceManage rema = new ResourceManage();
            p.SetValue<ResourceManage>(ParameterKey.RESOURCE_MANAGER, rema);
            var defaulttoken = TransactionToken.NewToken();
            p.TransTokenList.Add(defaulttoken);
            p.SetValue<TransactionToken>(ParameterKey.TOKEN, defaulttoken);
            p.SetValue("IsAjaxAsync", IsAjaxAsync);
            //设置serverinfo
            p[DomainKey.APPLICATION_ENVIRONMENT, "server_servername"] = context.Server.MachineName;
            p[DomainKey.APPLICATION_ENVIRONMENT, "serverinfo_ip"] = context.Request.ServerVariables["LOCAl_ADDR"];
            //设置clientinfo
            var ip = "";
            if (context.Request.ServerVariables["HTTP_VIA"] != null) // using proxy
            {
                ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];  // Return real client IP.
            }
            else// not using proxy or can't get the Client IP
            {
                ip = context.Request.ServerVariables["REMOTE_ADDR"]; //While it can't get the Client IP, it will return proxy IP.
            }
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_ip"] = ip;
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_browserversion"] = context.Request.Browser.Version;
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_platform"] = context.Request.Browser.Platform;
            p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_userhostname"] = context.Request.UserHostName;
            p.IsNeedSessionAbandon = false;
            p.IsWebSocket = context.IsWebSocketRequest;
            //hostview的引擎
            p.ExtentionObj.hostviewengine = new HostJsView();
            lock (lockobj)
            {
                Prepare(context, ref p, ref d);
            }
        }

        public static void Prepare(HttpContext context, ref WP p, ref WD d)
        {
            var url = context.Request.Path.Replace("\\", "/");
            if (url == "" && url == "/")
            {
                url = GlobalCommon.WebCommon.StartPage;
            }

            p.RequestResourcePath = ComFunc.nvl(url);
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


            foreach (string s in context.Request.QueryString.Keys)
            {
                p[DomainKey.QUERY_STRING, s] = context.Request.QueryString[s];
            }

            foreach (string s in context.Request.Form.AllKeys)
            {
                p[DomainKey.POST_DATA, s] = context.Request.Form[s];
            }

            //获取上传文件的二进制流
            foreach (string s in context.Request.Files.AllKeys)
            {
                p[DomainKey.UPDATE_FILE, s] = context.Request.Files[s];
            }
            lock (lockobj)
            {
                //websocket下无session
                if (!context.IsWebSocketRequest)
                {

                    //因websocket无session需要将cache中的数据域与session中同步一下
                    var sessionid = context.Session.SessionID;
                    if (sessionid != "" && GlobalCommon.ApplicationCache.Get("__frame_session__" + sessionid) != null)
                    {
                        var sessionobj = (FrameDLRObject)GlobalCommon.ApplicationCache.Get("__frame_session__" + sessionid);
                        foreach (var key in sessionobj.Keys)
                        {
                            context.Session[key] = sessionobj.GetValue(key);
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
                        p[DomainKey.SESSION, s] = ComFunc.CloneObject(context.Session[s]);
                    }
                }
                else
                {
                    var sessionid = context.Request.Params["ASP.NET_SessionId"];
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


                foreach (var s in context.Application.Keys)
                {
                    //深度复制application中的对象，防止p在最后释放时导致application对象丢失
                    p[DomainKey.APPLICATION, s.ToString()] = ComFunc.CloneObject(context.Application[s.ToString()]);
                }
            }

            if (context.Request.ContentType.ToLower().IndexOf("/json") > 0)
            {
                var sr = new StreamReader(context.Request.InputStream);
                var str = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(str))
                {
                    FrameDLRObject o = null;
                    FrameDLRObject.TryParse(str, FrameDLRFlags.SensitiveCase, out o);
                    if (o != null)
                    {
                        foreach (var k in o.Keys)
                        {
                            p[DomainKey.POST_DATA, k] = o.GetValue(k);
                        }
                    }
                }
                p.RequestContent = str;
            }
            //xml
            if (context.Request.ContentType.ToLower().IndexOf("/xml") > 0)
            {
                var sr = new StreamReader(context.Request.InputStream);
                var str = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(str))
                {
                    FrameDLRObject o = FrameDLRObject.CreateInstance(str, FrameDLRFlags.SensitiveCase);
                    if (o != null)
                    {
                        foreach (var k in o.Keys)
                        {
                            p[DomainKey.POST_DATA, k] = o.GetValue(k);
                        }
                    }
                }
                p.RequestContent = str;
            }
            //multipart/form-data
            if (context.Request.ContentType.ToLower().IndexOf("multipart/form-data") >= 0)
            {
                var mp = ParseMultipartFormData(context.Request.InputStream, context.Request.ContentEncoding);
                foreach (var k in mp.Keys)
                {
                    if (mp.GetValue(k) is FrameUploadFile)
                    {
                        p[DomainKey.UPDATE_FILE, k] = mp.GetValue(k);
                    }
                    else
                    {
                        p[DomainKey.POST_DATA, k] = mp.GetValue(k);
                    }
                }
            }

            p.ExtentionObj.cookie = FrameDLRObject.CreateInstance();
            if (context.Request.Cookies != null && context.Request.Cookies.Count > 0)
            {
                foreach (var key in context.Request.Cookies.AllKeys)
                {
                    if (key != "ASP.NET_SessionId")
                    {
                        FrameDLRObject item = FrameDLRObject.CreateInstance();
                        item.SetValue("name", context.Request.Cookies[key].Name);
                        item.SetValue("value", context.Request.Cookies[key].Value);
                        item.SetValue("domain", context.Request.Cookies[key].Domain);
                        item.SetValue("expire", context.Request.Cookies[key].Expires);

                        ((FrameDLRObject)p.ExtentionObj.cookie).SetValue(ComFunc.nvl(item.GetValue("name")), item);
                    }
                }
            }

            p[DomainKey.SESSION, "SessionID"] = context.IsWebSocketRequest ? context.Request.Params["ASP.NET_SessionId"] : context.Session.SessionID;
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath", context.Server.MapPath("~"));
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath_URL", context.Request.Url.AbsoluteUri.Replace(context.Request.FilePath, ""));
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "WebPath", context.Request.Url.AbsoluteUri.Replace(context.Request.RawUrl, ""));
            p.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "Domain", context.Request.Url.Host);
        }
        /// <summary>
        /// 对multipart/form-data数据流进行解析
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static FrameDLRObject ParseMultipartFormData(Stream stream, Encoding encoding)
        {
            FrameDLRObject rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);

            stream.Seek(0, SeekOrigin.Begin);
            // Read the stream into a byte array
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            string str = encoding.GetString(data);
            // Copy to a string for header parsing
            string content = encoding.GetString(data);

            // The first line should contain the delimiter
            int delimiterEndIndex = content.IndexOf("\r\n");

            if (delimiterEndIndex > -1)
            {
                string delimiter = content.Substring(0, content.IndexOf("\r\n"));
                int delimiterByteLength = encoding.GetByteCount(delimiter);

                string[] sections = content.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

                var splitindexs = ComFunc.BytesSplit(data, delimiter, Encoding.UTF8);
                var i = 0;

                foreach (string s in sections)
                {
                    //搜索分隔喘在byte[]中所在的位置
                   
                    if (s.Contains("Content-Disposition"))
                    {
                        // If we find "Content-Disposition", this is a valid multi-part section
                        // Now, look for the "name" parameter
                        Match nameMatch = new Regex(@"(?<=name\=\"")(.*?)(?=\"")").Match(s);
                        string name = nameMatch.Value.Trim().ToLower();

                        // Look for Content-Type
                        Regex re = new Regex(@"(?<=Content\-Type:)(.*?)(?=\r\n\r\n)");
                        Match contentTypeMatch = re.Match(s);

                        // Look for filename
                        re = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")");
                        Match filenameMatch = re.Match(s);

                        if (contentTypeMatch.Success || filenameMatch.Success)
                        {
                            
                            // Set properties
                            var contentype = contentTypeMatch.Value.Trim();
                            var filename = filenameMatch.Value.Trim();
                            
                            // Get the start & end indexes of the file contents
                            int startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;
                            var prestr = s.Substring(0, startIndex);
                            startIndex = encoding.GetByteCount(prestr) + splitindexs[i] + delimiterByteLength;
                            var endIndex = (i + 1) < splitindexs.Length ? splitindexs[i + 1] : data.Length - 1;
                            //清除首位的换行符
                            for (int j = 0; j < 2; j++)
                            {
                                if (data[startIndex] == 10 || data[startIndex] == 13)
                                {
                                    startIndex += 1;
                                }
                                if (data[endIndex-1] == 10 || data[endIndex-1] == 13)
                                {
                                    endIndex -= 1;
                                }
                            }

                            var contentLength = endIndex - startIndex;
                            var fileData = new byte[contentLength];
                            Array.Copy(data, startIndex, fileData, 0, fileData.Length);
                            
                            

                            var ms = new MemoryStream();
                            ms.Write(fileData,0,fileData.Length);

                            var fuf = new FrameUploadFile(filename, contentype, ms, encoding);
                            rtn.SetValue(name, fuf);
                        }
                        else if (!string.IsNullOrWhiteSpace(name))
                        {
                            // Get the start & end indexes of the file contents
                            int startIndex = nameMatch.Index + nameMatch.Length + "\r\n\r\n".Length;
                            rtn.SetValue(name, s.Substring(startIndex).TrimEnd(new char[] { '\r', '\n' }).Trim());
                        }
                    }
                    i++;
                }
            }

            return rtn;
        }

        protected virtual void AfterProcess(HttpContext context, WP p, WD d)
        {
            lock (lockobj)
            {
                var sessionid = context.Request.Params["ASP.NET_SessionId"];
                if (p.IsNeedSessionAbandon)
                {
                    context.Session.Abandon();
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
                    if (context.Session != null)
                    {
                        foreach (string s in keys)
                        {
                            context.Session[s] = ComFunc.CloneObject(p[DomainKey.SESSION, s]);
                        }
                    }


                    if (!IsWebSocket)
                    {
                        keys = context.Session.Keys.Cast<string>().Except(keys).ToArray();
                        foreach (string s in keys)
                        {
                            context.Session.Remove(s);
                        }
                    }

                    keys = p.Domain(DomainKey.APPLICATION).Keys;
                    foreach (string s in keys)
                    {
                        context.Application[s] = ComFunc.CloneObject(p[DomainKey.APPLICATION, s]);
                    }

                    keys = context.Application.Keys.Cast<string>().Except(keys).ToArray();
                    foreach (string s in keys)
                    {
                        context.Application.Remove(s);
                    }

                }
            }
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
                HttpCookie hc = new HttpCookie(name);
                hc.Value = value;
                hc.Domain = domain;
                if (expire != DateTime.MinValue)
                    hc.Expires = expire;
                HttpContext.Current.Response.Cookies.Add(hc);
            }

            foreach (var key in removecookies.Keys)
            {
                var item = (FrameDLRObject)removecookies.GetValue(key);
                var name = ComFunc.nvl(item.GetValue("name"));
                HttpCookie currentUserCookie = HttpContext.Current.Request.Cookies[name];
                if (currentUserCookie != null)
                {
                    HttpContext.Current.Response.Cookies.Remove(name);
                    currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                    currentUserCookie.Value = null;
                    HttpContext.Current.Response.SetCookie(currentUserCookie);
                }
            }
        }

        public abstract bool IsReusable
        {
            get;
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
    }
}
