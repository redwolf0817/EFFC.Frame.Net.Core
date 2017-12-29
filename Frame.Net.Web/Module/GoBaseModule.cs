using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using EFFC.Frame.Net.Business.Engine;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Global;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;

namespace Frame.Net.Web.Module
{
    /// <summary>
    /// Go请求的基类模块
    /// </summary>
    /// <typeparam name="WebParameter"></typeparam>
    /// <typeparam name="GoData"></typeparam>
    public abstract class GoBaseModule : WebBaseModule<WebParameter, GoData>
    {
        bool isSuccess = true;
        protected override void InvokeAction(WebParameter p, GoData d)
        {
            isSuccess = RunMe(p, d);

        }

        protected abstract bool RunMe(WebParameter p, GoData d);

        protected override void SetResponseContent(WebParameter p, GoData d)
        {
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

        protected virtual void SetContent4WebSocket(WebParameter p, GoData d)
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
            var obj = CurrentContext.Request.Headers;
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

                CurrentContext.Response.Headers.Add("Content-Length", b.Length + "");
                CurrentContext.Response.ContentType = "image/jpeg";
                CurrentContext.Response.Body.Write(b, 0, b.Length);

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

                CurrentContext.Response.Headers.Add("Content-Length", b.Length + "");
                CurrentContext.Response.ContentType = "image/gif";
                CurrentContext.Response.Body.Write(b, 0, b.Length);
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

                CurrentContext.Response.Headers.Add("Content-Length", b.Length + "");
                CurrentContext.Response.ContentType = "image/bmp";
                CurrentContext.Response.Body.Write(b, 0, b.Length);
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

                CurrentContext.Response.Headers.Add("Content-Length", b.Length + "");
                CurrentContext.Response.ContentType = "image/png";
                CurrentContext.Response.Body.Write(b, 0, b.Length);
            }
            else if (d.ContentType == GoResponseDataType.Excel)
            {
                CurrentContext.Response.Headers.Add("Content-Disposition", "attachment;filename=" + ComFunc.UrlEncode(ComFunc.nvl(d["__download_filename__"])));
                //CurrentContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.xls;
                if (d.ResponseData is byte[])
                {
                    CurrentContext.Response.Body.Write((byte[])d.ResponseData, 0, ((byte[])d.ResponseData).Length);
                }
                else if (d.ResponseData is Stream)
                {
                    var arr = StreamToBytes((Stream)d.ResponseData);
                    CurrentContext.Response.Body.Write(arr, 0, arr.Length);
                }
                else
                {
                    CurrentContext.Response.WriteAsync(ComFunc.nvl(d.ResponseData)).Wait();
                }
                CurrentContext.Response.Body.Flush();
            }
            else if (d.ContentType == GoResponseDataType.Word)
            {
                CurrentContext.Response.Headers.Add("Content-Disposition", "attachment;filename=" + ComFunc.UrlEncode(ComFunc.nvl(d["__download_filename__"])));
                //CurrentContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.doc;
                if (d.ResponseData is byte[])
                {
                    CurrentContext.Response.Body.Write((byte[])d.ResponseData, 0, ((byte[])d.ResponseData).Length);
                }
                else if (d.ResponseData is Stream)
                {
                    var arr = StreamToBytes((Stream)d.ResponseData);
                    CurrentContext.Response.Body.Write(arr, 0, arr.Length);
                }
                else
                {
                    CurrentContext.Response.WriteAsync(ComFunc.nvl(d.ResponseData)).Wait();
                }
                CurrentContext.Response.Body.Flush();
            }
            else if (d.ContentType == GoResponseDataType.PDF)
            {
                if (agent != null && agent.IndexOf("MSIE") == -1 && agent.IndexOf("Chrome") == -1 && agent.IndexOf("Opera") == -1)
                {
                    //非IE非Chrom
                    CurrentContext.Response.Headers.Add("Content-Disposition", "attachment;filename=" + ComFunc.nvl(d["__download_filename__"]));
                }
                else
                {
                    CurrentContext.Response.Headers.Add("Content-Disposition", "attachment;filename=" + ComFunc.UrlEncode(ComFunc.nvl(d["__download_filename__"])));
                }

                //CurrentContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.pdf;
                if (d.ResponseData is byte[])
                {
                    CurrentContext.Response.Body.Write((byte[])d.ResponseData, 0, ((byte[])d.ResponseData).Length);
                }
                else if (d.ResponseData is Stream)
                {
                    var arr = StreamToBytes((Stream)d.ResponseData);
                    CurrentContext.Response.Body.Write(arr, 0, arr.Length);
                }
                else
                {
                    CurrentContext.Response.WriteAsync(ComFunc.nvl(d.ResponseData)).Wait();
                }
                CurrentContext.Response.Body.Flush();
            }
            else if (d.ContentType == GoResponseDataType.Json)
            {
                //CurrentContext.Response.Charset = "UTF-8";
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.json + ";charset=utf-8";
                if (d.ResponseData is FrameDLRObject)
                {
                    CurrentContext.Response.WriteAsync(ComFunc.FormatJSON((FrameDLRObject)d.ResponseData).ToJSONString()).Wait();
                }
                else
                {
                    CurrentContext.Response.WriteAsync(ComFunc.FormatJSON(ComFunc.nvl(d.ResponseData)).ToJSONString()).Wait();
                }

            }
            else if (d.ContentType == GoResponseDataType.HostView)
            {
                if (d.ResponseData is FrameDLRObject)
                {
                    var dobj = (FrameDLRObject)d.ResponseData;
                    //获取view路径
                    string vieWebParameterath = ComFunc.nvl(d.ExtentionObj.hostvieWebParameterath);
                    vieWebParameterath = vieWebParameterath.Replace("~", GlobalCommon.HostCommon.RootPath + HostJsConstants.COMPILED_VIEW_PATH);
                    if (File.Exists(vieWebParameterath))
                    {
                        //调用hostview引擎进行渲染
                        HostJsView hjv = (HostJsView)p.ExtentionObj.hostviewengine;
                        hjv.CurrentContext.SetDataModel(dobj.ToDictionary());
                        var html = hjv.Render(File.ReadAllText(vieWebParameterath, Encoding.UTF8));

                        //CurrentContext.Response.Charset = "UTF-8";
                        CurrentContext.Response.ContentType = ResponseHeader_ContentType.html + ";charset=utf-8";
                        CurrentContext.Response.WriteAsync(html).Wait();
                    }
                    else
                    {
                        CurrentContext.Response.WriteAsync("File Not Found!").Wait();
                    }
                }
            }
            else if (d.ContentType == GoResponseDataType.RazorView)
            {
                //Mvc进行视图展示
                StringWriter sw = new StringWriter();
                var htmlstr = WMvcView.RenderView(p, d, CurrentContext);
                d.SetValue("ViewHtmlCode", sw.ToString());
                if (!IsWebSocket)
                {
                    CurrentContext.Response.ContentType = ResponseHeader_ContentType.html + ";charset=utf-8";
                    CurrentContext.Response.WriteAsync(htmlstr).Wait();
                }
                else
                {
                    CurrentContext.Response.ContentType = ResponseHeader_ContentType.json + ";charset=utf-8";
                    var v = ComFunc.FormatJSON(sw.ToString());
                    var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(v.ToJSONString()));
                    CurrentSocket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
                }
            }
            else
            {
                if (d is IJSONable)
                {
                    CurrentContext.Response.ContentType = ResponseHeader_ContentType.json + ";charset=utf-8";
                    CurrentContext.Response.WriteAsync(((IJSONable)d.ResponseData).ToJSONString());
                }
                else
                {
                    CurrentContext.Response.ContentType = ResponseHeader_ContentType.html + ";charset=utf-8";
                    FrameDLRObject dobj = FrameDLRObject.CreateInstance(d.ResponseData, FrameDLRFlags.SensitiveCase);
                    CurrentContext.Response.WriteAsync(dobj.ToJSONString());
                }

            }
        }

        private byte[] StreamToBytes(Stream stream)
        {
            return ComFunc.StreamToBytes(stream);
        }
    }
}
