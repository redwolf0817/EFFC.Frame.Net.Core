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
using EFFC.Frame.Net.Business.Engine;

namespace EFFC.Frame.Net.Web.Core
{
    public abstract class GoBaseHandler<WP,WD> :WebBaseHandler<WP,WD>
        where WP:WebParameter
        where WD:GoData
    {
        public override bool IsReusable
        {
            get { return false; ; }
        }

        protected override void Run(WP p, WD d)
        {
            try
            {
                bool isSuccess = RunMe(p, d);

                if (isSuccess)
                {
                    if (IsAjaxAsync == false && !string.IsNullOrEmpty(d.RedirectUri))
                    {
                        CurrentContext.Response.Redirect(d.RedirectUri, false);
                    }
                    else
                    {
                        if (!IsWebSocket)
                            SetContent(p, d);
                        else
                            SetContent4WebSocket(p, d);
                    }
                }
            }
            finally
            {
                p.Resources.ReleaseAll();
            }
        }

       

        protected abstract bool RunMe(WP p, WD d);

        protected virtual void SetContent4WebSocket(WP p, WD d)
        {
            var socket = CurrentSocket;
            if (d.ResponseData is FrameDLRObject)
            {
                var v = ComFunc.FormatJSON((FrameDLRObject)d.ResponseData);
                var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(v.ToJSONString()));
                socket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
            }
            else if (d.ResponseData is byte[])
            {
                var v = (byte[])d.ResponseData;
                var buffer = new ArraySegment<byte>(v);
                socket.SendAsync(buffer, WebSocketMessageType.Binary, true, System.Threading.CancellationToken.None);
            }
            else if (d.ResponseData is Stream)
            {
                byte[] b;
                if (d.ResponseData is MemoryStream)
                {
                    b = ((MemoryStream)d.ResponseData).ToArray();
                }
                else
                {
                    var v = (Stream)d.ResponseData;
                    MemoryStream ms = new MemoryStream();
                    v.CopyTo(ms);

                    b = ms.ToArray();
                }

                var buffer = new ArraySegment<byte>(b);
                socket.SendAsync(buffer, WebSocketMessageType.Binary, true, System.Threading.CancellationToken.None);
            }
            else
            {
                var v = ComFunc.FormatJSON(ComFunc.nvl(d.ResponseData));
                var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(v.ToJSONString()));
                socket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
            }
        }
        protected virtual void SetContent(WebParameter p, GoData d)
        {
            System.Collections.Specialized.NameValueCollection obj = CurrentContext.Request.Headers;
            String agent = ComFunc.nvl(obj["USER-AGENT"]);

            if (d.ResponseData == null)
            {
                throw new Exception("ResponseData is null!");
            }
            if (d.ContentType == GoResponseDataType.Pic_Jpg)
            {
                byte[] b = null;
                if (d.ResponseData is Stream)
                {
                    b = StreamToBytes((Stream)d.ResponseData);
                }
                else
                {
                    b = ((byte[])d.ResponseData);
                }

                CurrentContext.Response.AppendHeader("Content-Length", b.Length + "");
                CurrentContext.Response.ContentType = "image/jpeg";
                CurrentContext.Response.BinaryWrite(b);

            }
            else if (d.ContentType == GoResponseDataType.Pic_Gif)
            {
                byte[] b = null;
                if (d.ResponseData is Stream)
                {
                    b = StreamToBytes((Stream)d.ResponseData);
                }
                else
                {
                    b = ((byte[])d.ResponseData);
                }

                CurrentContext.Response.AppendHeader("Content-Length", b.Length + "");
                CurrentContext.Response.ContentType = "image/gif";
                CurrentContext.Response.BinaryWrite(b);
            }
            else if (d.ContentType == GoResponseDataType.Pic_Bmp)
            {
                byte[] b = null;
                if (d.ResponseData is Stream)
                {
                    b = StreamToBytes((Stream)d.ResponseData);
                }
                else
                {
                    b = ((byte[])d.ResponseData);
                }

                CurrentContext.Response.AppendHeader("Content-Length", b.Length + "");
                CurrentContext.Response.ContentType = "image/bmp";
                CurrentContext.Response.BinaryWrite(b);
            }
            else if (d.ContentType == GoResponseDataType.Pic_png)
            {
                byte[] b = null;
                if (d.ResponseData is Stream)
                {
                    b = StreamToBytes((Stream)d.ResponseData);
                }
                else
                {
                    b = ((byte[])d.ResponseData);
                }

                CurrentContext.Response.AppendHeader("Content-Length", b.Length + "");
                CurrentContext.Response.ContentType = "image/png";
                CurrentContext.Response.BinaryWrite(b);
            }
            else if (d.ContentType == GoResponseDataType.Excel)
            {
                CurrentContext.Response.Buffer = true;
                CurrentContext.Response.Charset = "UTF-8";
                //CurrentContext.Response.ClearHeaders();
                CurrentContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(ComFunc.nvl(d["__download_filename__"]), Encoding.UTF8));
                CurrentContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.xls;
                if (d.ResponseData is byte[])
                {
                    CurrentContext.Response.BinaryWrite((byte[])d.ResponseData);
                }
                else if (d.ResponseData is Stream)
                {
                    CurrentContext.Response.BinaryWrite(StreamToBytes((Stream)d.ResponseData));
                }
                else
                {
                    CurrentContext.Response.Write(d.ResponseData);
                }
                CurrentContext.Response.Flush();
            }
            else if (d.ContentType == GoResponseDataType.Word)
            {
                CurrentContext.Response.Buffer = true;
                CurrentContext.Response.Charset = "UTF-8";
                //CurrentContext.Response.ClearHeaders();
                CurrentContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(ComFunc.nvl(d["__download_filename__"]), Encoding.UTF8));
                CurrentContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.doc;
                if (d.ResponseData is byte[])
                {
                    CurrentContext.Response.BinaryWrite((byte[])d.ResponseData);
                }
                else if (d.ResponseData is Stream)
                {
                    CurrentContext.Response.BinaryWrite(StreamToBytes((Stream)d.ResponseData));
                }
                else
                {
                    CurrentContext.Response.Write(d.ResponseData);
                }
                CurrentContext.Response.Flush();
            }
            else if (d.ContentType == GoResponseDataType.PDF)
            {
                CurrentContext.Response.Buffer = true;
                CurrentContext.Response.Charset = "UTF-8";
                //CurrentContext.Response.ClearHeaders();
                if (agent != null && agent.IndexOf("MSIE") == -1 && agent.IndexOf("Chrome") == -1 && agent.IndexOf("Opera") == -1)
                {
                    //非IE非Chrom
                    CurrentContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + ComFunc.nvl(d["__download_filename__"]));
                }
                else
                {
                    CurrentContext.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(ComFunc.nvl(d["__download_filename__"]), Encoding.UTF8));
                }

                CurrentContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.pdf;
                if (d.ResponseData is byte[])
                {
                    CurrentContext.Response.BinaryWrite((byte[])d.ResponseData);
                }
                else if (d.ResponseData is Stream)
                {
                    CurrentContext.Response.BinaryWrite(StreamToBytes((Stream)d.ResponseData));
                }
                else
                {
                    CurrentContext.Response.Write(d.ResponseData);
                }
                CurrentContext.Response.Flush();
            }
            else if (d.ContentType == GoResponseDataType.Json)
            {
                CurrentContext.Response.Charset = "UTF-8";
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.json;
                if (d.ResponseData is FrameDLRObject)
                {
                    CurrentContext.Response.Write(ComFunc.FormatJSON((FrameDLRObject)d.ResponseData).ToJSONString());
                }
                else
                {
                    CurrentContext.Response.Write(ComFunc.FormatJSON(ComFunc.nvl(d.ResponseData)).ToJSONString());
                }

            }
            else if (d.ContentType == GoResponseDataType.HostView)
            {
                if (d.ResponseData is FrameDLRObject)
                {
                    var dobj = (FrameDLRObject)d.ResponseData;
                    //获取view路径
                    string viewpath = ComFunc.nvl(d.ExtentionObj.hostviewpath);
                    viewpath = viewpath.Replace("~", GlobalCommon.HostCommon.RootPath + HostJsConstants.COMPILED_VIEW_PATH);
                    if (File.Exists(viewpath))
                    {
                        //调用hostview引擎进行渲染
                        HostJsView hjv = (HostJsView)p.ExtentionObj.hostviewengine;
                        hjv.CurrentContext.SetDataModel(dobj.ToDictionary());
                        var html = hjv.Render(File.ReadAllText(viewpath, Encoding.UTF8));

                        CurrentContext.Response.Charset = "UTF-8";
                        CurrentContext.Response.ContentType = ResponseHeader_ContentType.html;
                        CurrentContext.Response.Write(html);
                    }
                    else
                    {
                        CurrentContext.Response.Write("File Not Found!");
                    }
                }
            }
            else if (d.ContentType == GoResponseDataType.RazorView)
            {
                //Mvc进行视图展示
                StringWriter sw = new StringWriter();
                WMvcView.RenderView(p, d, CurrentContext, sw);
                d.SetValue("ViewHtmlCode", sw.ToString());
                if (!IsWebSocket)
                {
                    CurrentContext.Response.Charset = "UTF-8";
                    CurrentContext.Response.ContentType = ResponseHeader_ContentType.html;
                    CurrentContext.Response.Write(sw.ToString());
                }
                else
                {
                    CurrentContext.Response.Charset = "UTF-8";
                    CurrentContext.Response.ContentType = ResponseHeader_ContentType.json;
                    var v = ComFunc.FormatJSON(sw.ToString());
                    var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(v.ToJSONString()));
                    CurrentSocket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
                }
            }
            else
            {
                CurrentContext.Response.Write(d.ResponseData);
            }
        }

        private byte[] StreamToBytes(Stream stream)
        {
            if (stream is MemoryStream)
            {
                return ((MemoryStream)stream).ToArray();
            }
            else
            {
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);

                return ms.ToArray();
            }
        }
    }
}
