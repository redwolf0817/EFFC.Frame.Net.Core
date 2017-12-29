using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Data;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Base.Constants;
using System.Net.WebSockets;
using EFFC.Frame.Net.Base.Data.Base;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EFFC.Frame.Net.Global;

namespace EFFC.Frame.Net.Business.Logic
{
    public partial class WebBaseLogic<P, D>
    {
        private WebSocketHelper _ws;
        /// <summary>
        /// websocket操作
        /// </summary>
        public virtual WebSocketHelper WS
        {
            get
            {
                if (_ws == null)
                {
                    _ws = new WebSocketHelper(this);
                }

                   
                return _ws;
            }
        }

        public class WebSocketHelper
        {
            WebSocket socket = null;
            bool iswebsocket = false;
            WebBaseLogic<P, D> _logic = null;
            public WebSocketHelper(WebBaseLogic<P, D> logic)
            {
                _logic = logic;
                if (_logic.CallContext_DataCollection["websocket"] != null)
                {
                    socket = (WebSocket)_logic.CallContext_DataCollection["websocket"];
                }
                iswebsocket = _logic.IsWebSocket;
            }
            /// <summary>
            /// 即时回传信息
            /// </summary>
            /// <param name="value"></param>
            public virtual void Send(object value)
            {
                if (iswebsocket && socket.State == WebSocketState.Open)
                {
                    if (value is FrameDLRObject)
                    {
                        var v = ComFunc.FormatJSON((FrameDLRObject)value);
                        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(v.ToJSONString()));
                        socket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
                    }
                    else if (value is byte[])
                    {
                        var v = (byte[])value;
                        var buffer = new ArraySegment<byte>(v);
                        socket.SendAsync(buffer, WebSocketMessageType.Binary, true, System.Threading.CancellationToken.None);
                    }
                    else if (value is Stream)
                    {
                        byte[] b;
                        if (value is MemoryStream)
                        {
                            b = ((MemoryStream)value).ToArray();
                        }
                        else
                        {
                            var v = (Stream)value;
                            MemoryStream ms = new MemoryStream();
                            v.CopyTo(ms);

                            b = ms.ToArray();
                        }

                        var buffer = new ArraySegment<byte>(b);
                        socket.SendAsync(buffer, WebSocketMessageType.Binary, true, System.Threading.CancellationToken.None);
                    }
                    else
                    {
                        var v = ComFunc.FormatJSON(ComFunc.nvl(value));
                        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(v.ToJSONString()));
                        socket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
                    }
                }
            }
            /// <summary>
            /// 接收websockect发来的消息，该方法会阻断当前线程，直到消息发来或因超时导致框架自动关闭连接
            /// 若因超时连接自动关闭则后续程序将无法处理
            /// </summary>
            /// <returns></returns>
            public virtual object Recieve()
            {
                string wsguid = ComFunc.nvl(_logic.CallContext_Parameter.ExtentionObj.websocket_uid);
                object outobj = null;
                if (socket.State == WebSocketState.Open)
                {
                    
                    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
                    var t = socket.ReceiveAsync(buffer, CancellationToken.None);
                    t.Wait();
                    var result = t.Result;
                    if (socket.State != WebSocketState.CloseReceived)
                    {
                        var bl = new List<byte>();
                        bl.AddRange(buffer.Array.Take(result.Count));
                        while (!result.EndOfMessage && result.MessageType != WebSocketMessageType.Close)
                        {
                            t = socket.ReceiveAsync(buffer, CancellationToken.None);
                            t.Wait();
                            result = t.Result;
                            bl.AddRange(buffer.Array.Take(result.Count));
                        }
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string userMsg = Encoding.UTF8.GetString(bl.ToArray());
                            if (GlobalCommon.ApplicationCache.Get(wsguid + "websocket_expiration") != null)
                            {
                                var expirationtime = (DateTime)GlobalCommon.ApplicationCache.Get(wsguid + "websocket_expiration");
                                expirationtime = DateTime.Now.AddMinutes(GlobalCommon.WebSocketCommon.MaxConnectionMinutes);
                            }
                            if (FrameDLRObject.IsJson(userMsg))
                            {
                                outobj = FrameDLRObject.CreateInstance(userMsg);
                            }
                            else
                            {
                                outobj = userMsg;
                            }
                        }
                        else
                        {
                            outobj = bl.ToArray();
                        }
                    }
                    
                }

                return outobj;
            }
            /// <summary>
            /// 异步接收信息或因超时导致框架自动关闭连接
            /// </summary>
            /// <param name="func"></param>
            /// <param name="whenerror"></param>
            public virtual void RecieveAsync(Action<object> func, Action whenerror)
            {
                if (iswebsocket && socket.State == WebSocketState.Open)
                {
                    string wsguid = ComFunc.nvl(_logic.CallContext_Parameter.ExtentionObj.websocket_uid);

                    Task.Run(() =>
                    {
                        try
                        {
                            if (socket.State == WebSocketState.Open)
                            {
                                object outobj = null;
                                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[2048]);
                                var t = socket.ReceiveAsync(buffer, CancellationToken.None);
                                t.Wait();
                                string userMsg = Encoding.UTF8.GetString(buffer.Array, 0, t.Result.Count);
                                if (GlobalCommon.ApplicationCache.Get(wsguid + "websocket_expiration") != null)
                                {
                                    var expirationtime = (DateTime)GlobalCommon.ApplicationCache.Get(wsguid + "websocket_expiration");
                                    expirationtime = DateTime.Now.AddMinutes(GlobalCommon.WebSocketCommon.MaxConnectionMinutes);
                                }
                                if (FrameDLRObject.IsJson(userMsg))
                                {
                                    outobj = FrameDLRObject.CreateInstance(userMsg);
                                }
                                else
                                {
                                    outobj = userMsg;
                                }

                                if (func != null)
                                {
                                    func.Invoke(outobj);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobalCommon.Logger.WriteLog(LoggerLevel.ERROR, "Exception happend in WebSockect RecieveSync Task Run:" + ex.Message + "\n" + ex.StackTrace);
                            socket.CloseAsync(WebSocketCloseStatus.InternalServerError, "error", CancellationToken.None);
                            if (whenerror != null)
                            {
                                whenerror.Invoke();
                            }
                        }
                    });

                }
            }

            /// <summary>
            /// 即时关闭websocket连接
            /// </summary>
            /// <param name="closedesc"></param>
            public virtual void CloseConnection(string closedesc)
            {
                if (iswebsocket)
                {
                    socket.CloseAsync(WebSocketCloseStatus.NormalClosure, closedesc, System.Threading.CancellationToken.None);
                }
            }
            /// <summary>
            /// 判断是否已经关闭
            /// </summary>
            public bool IsClose
            {
                get
                {
                    if (socket == null || socket.State == WebSocketState.Closed
                        || socket.State == WebSocketState.CloseReceived
                        || socket.State == WebSocketState.CloseSent)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}
