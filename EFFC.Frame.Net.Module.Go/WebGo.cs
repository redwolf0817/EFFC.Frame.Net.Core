using EFFC.Frame.Net.Module.Web.Modules;
using EFFC.Frame.Net.Module.Web.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Business.Parameters;
using EFFC.Frame.Net.Module.Business.Datas;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Common;
using Frame.Net.Base.Exceptions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Razor;
using static EFFC.Frame.Net.Module.Razor.RazorViewModule;

namespace EFFC.Frame.Net.Module.Extend.WebGo
{
    public class WebGo : WebBaseModule<WebParameter, GoData>
    {
        static string startpage = "index";
        //static RazorViewToStringRenderer _render = null;
        //static string _excutefilepath = "";
        public override string Name => "webgo";

        public override string Description => "go请求处理";

        protected override void DoAddProxy(ProxyManager ma, dynamic options)
        {
            //_render = options.RazorEngine;
            //_excutefilepath = ComFunc.nvl(options.ExcuteFilePath);
            ma.UseProxy<GoBusiProxy>("gobusi", options);
            ma.UseProxy<RazorProxy>("razor", options);
        }

        protected override void InvokeAction(WebParameter p, GoData d)
        {
            object od = d;

            GlobalCommon.Proxys["gobusi"].CallModule(ref od, p);


        }

        protected override void BeforeProcess(WebParameter p, GoData d)
        {
            LoadConfig(p, d);
            base.BeforeProcess(p, d);
        }

        protected virtual void LoadConfig(WebParameter p, GoData d)
        {
            foreach (var item in MyConfig.GetConfigurationList("ConnectionStrings"))
            {
                p[DomainKey.CONFIG, item.Key] = ComFunc.nvl(item.Value);
            }
            p.DBConnectionString = ComFunc.nvl(p[DomainKey.CONFIG, "DefaultConnection"]);
            bool bvalue = true;
            foreach (var item in MyConfig.GetConfigurationList("EFFC"))
            {
                if (bool.TryParse(ComFunc.nvl(item.Value), out bvalue))
                {
                    p[DomainKey.CONFIG, item.Key] = bool.Parse(ComFunc.nvl(item.Value));
                }
                else if (DateTimeStd.IsDateTime(item.Value))
                {
                    p[DomainKey.CONFIG, item.Key] = DateTimeStd.ParseStd(item.Value).Value;
                }
                else
                {
                    p[DomainKey.CONFIG, item.Key] = ComFunc.nvl(item.Value);
                }
            }
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            var wp = (WebParameter)p;
            var wd = (GoData)d;

            GlobalCommon.ExceptionProcessor.ProcessException(this, ex, p, d);
            string errorCode = "E-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string errlog = "";
            if (ex is HostJsException)
            {
                var jex = (HostJsException)ex;
                if (ex.InnerException != null)
                {
                    if (ex.InnerException is HostJsException)
                    {
                        var ijex = (HostJsException)ex.InnerException;
                        errlog = string.Format("错误编号：{0}，\n{1}\n{2}\n出错代码行数{3}\n出错代码列数{4}\n出错代码位置{5}\nInnerException:{6}\n{7}\n出错代码行数{8}\n出错代码列数{9}\n出错代码位置{10}", errorCode, ex.Message, ex.StackTrace,
                            jex.Line, jex.Column, jex.SourceCode.Replace("\"", "'"),
                            ex.InnerException.Message, ex.InnerException.StackTrace,
                            ijex.Line, ijex.Column, ijex.SourceCode.Replace("\"", "'"));
                    }
                    else
                    {
                        errlog = string.Format("错误编号：{0}，\n{1}\n{2}\n出错代码行数{3}\n出错代码列数{4}\n出错代码位置{5}\nInnerException:{6}\n{7}", errorCode, ex.Message, ex.StackTrace, jex.Line, jex.Column, jex.SourceCode, ex.InnerException.Message, ex.InnerException.StackTrace);
                    }
                }
                else
                {
                    errlog = string.Format("错误编号：{0}，\n{1}\n{2}\n出错代码行数{3}\n出错代码列数{4}\n出错代码位置{5}", errorCode, ex.Message, ex.StackTrace,
                        jex.Line, jex.Column, jex.SourceCode.Replace("\"", "'"));
                }
            }
            else
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException is HostJsException)
                    {
                        var ijex = (HostJsException)ex.InnerException;
                        errlog = string.Format("错误编号：{0}，\n{1}\n{2}\nInnerException:{3}\n{4}\n\n出错代码行数{5}\n出错代码列数{6}\n出错代码位置{7}", errorCode, ex.Message, ex.StackTrace,
                            ex.InnerException.Message, ex.InnerException.StackTrace,
                            ijex.Line, ijex.Column, ijex.SourceCode.Replace("\"", "'"));
                    }
                    else
                    {
                        errlog = string.Format("错误编号：{0}，\n{1}\n{2}\nInnerException:{3}\n{4}", errorCode, ex.Message, ex.StackTrace, ex.InnerException.Message, ex.InnerException.StackTrace);
                    }
                }
                else
                {
                    errlog = string.Format("错误编号：{0}，\n{1}\n{2}", errorCode, ex.Message, ex.StackTrace);
                }
            }

            GlobalCommon.Logger.WriteLog(LoggerLevel.ERROR, errlog);

            var errormsg = "";
            var isdebug = p[DomainKey.CONFIG, "DebugMode"] == null ? false : (bool)p[DomainKey.CONFIG, "DebugMode"];
            if (isdebug)
            {
                errormsg = string.Format("出错了，{0}", errlog); ;
            }
            else
            {
                errormsg = string.Format("系统出错了，请联系相关人员帮助处理，并告知其错误编号。谢谢！（错误编号：{0}）", errorCode);
            }

            CurrentContext.Response.StatusCode = 200;
            var jsonmsg = ComFunc.FormatJSON(errorCode, errormsg, "").ToJSONString();
            var msgbytelength = Encoding.UTF8.GetByteCount(jsonmsg);
            CurrentContext.Response.Headers.Add("Content-Length", msgbytelength + "");
            if (wd.ContentType == GoResponseDataType.Json)
            {
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.json + ";charset=utf-8";
                CurrentContext.Response.WriteAsync(jsonmsg).Wait();

            }
            else if (wd.ContentType == GoResponseDataType.RazorView)
            {
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.json + ";charset=utf-8";
                CurrentContext.Response.WriteAsync(jsonmsg).Wait();

            }
            else if (wd.ContentType == GoResponseDataType.HostView)
            {
                //var viewpath = "~/error.hjs".Replace("~", GlobalCommon.HostCommon.RootPath + HostJsConstants.COMPILED_VIEW_PATH);
                //if (File.Exists(viewpath))
                //{
                //    //调用hostview引擎进行渲染
                //    HostJsView hjv = (HostJsView)p.ExtentionObj.hostviewengine;
                //    hjv.CurrentContext.SetDataModel(FrameDLRObject.CreateInstanceFromat(@"{ErrorTitle:{0},ErrorMsg:{1}}", "系统出错了", errormsg).ToDictionary());
                //    var html = hjv.Render(File.ReadAllText(viewpath, Encoding.UTF8));

                //    CurrentContext.Response.ContentType = ResponseHeader_ContentType.html + ";charset=utf-8";
                //    CurrentContext.Response.WriteAsync(html).Wait();
                //}
                //else
                //{
                //    CurrentContext.Response.ContentType = ResponseHeader_ContentType.json + ";charset=utf-8";
                //    CurrentContext.Response.WriteAsync(ComFunc.FormatJSON(errorCode, errlog, "").ToJSONString()).Wait();
                //}
            }
            else
            {
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.json + ";charset=utf-8";
                CurrentContext.Response.WriteAsync(jsonmsg).Wait();
            }

            CurrentContext.Response.Body.FlushAsync().Wait();
        }

        protected override void SetResponseContent(WebParameter p, GoData d)
        {
            var dt = DateTime.Now;
            if (!string.IsNullOrEmpty(d.RedirectUri))
            {
                p.CurrentHttpContext.Response.Redirect(d.RedirectUri);
                return;
            }

            var obj = CurrentContext.Request.Headers;
            String agent = ComFunc.nvl(obj["USER-AGENT"]);

            if (d.ResponseData == null)
            {
                throw new Exception("ResponseData is null!");
            }
            CurrentContext.Response.StatusCode = 200;

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
                    var jsonstr = ComFunc.FormatJSON((FrameDLRObject)d.ResponseData).ToJSONString();
                    var msgbytelength = Encoding.UTF8.GetByteCount(jsonstr);
                    CurrentContext.Response.Headers.Add("Content-Length", msgbytelength + "");
                    CurrentContext.Response.WriteAsync(jsonstr).Wait();
                }
                else if (d.ResponseData is string)
                {
                    var jsonstr = ComFunc.FormatJSON(ComFunc.nvl(d.ResponseData)).ToJSONString();
                    var msgbytelength = Encoding.UTF8.GetByteCount(jsonstr);
                    CurrentContext.Response.Headers.Add("Content-Length", msgbytelength + "");
                    CurrentContext.Response.WriteAsync(jsonstr).Wait();
                }
                else
                {
                    FrameDLRObject dobj = FrameDLRObject.CreateInstance(d.ResponseData);
                    var jsonstr = ComFunc.FormatJSON(dobj).ToJSONString();
                    var msgbytelength = Encoding.UTF8.GetByteCount(jsonstr);
                    CurrentContext.Response.Headers.Add("Content-Length", msgbytelength + "");
                    CurrentContext.Response.WriteAsync(ComFunc.FormatJSON(dobj).ToJSONString()).Wait();
                }

            }
            else if (d.ContentType == GoResponseDataType.HostView)
            {
                //if (d.ResponseData is FrameDLRObject)
                //{
                //    var dobj = (FrameDLRObject)d.ResponseData;
                //    //获取view路径
                //    string vieWebParameterath = ComFunc.nvl(d.ExtentionObj.hostvieWebParameterath);
                //    vieWebParameterath = vieWebParameterath.Replace("~", GlobalCommon.HostCommon.RootPath + HostJsConstants.COMPILED_VIEW_PATH);
                //    if (File.Exists(vieWebParameterath))
                //    {
                //        //调用hostview引擎进行渲染
                //        HostJsView hjv = (HostJsView)p.ExtentionObj.hostviewengine;
                //        hjv.CurrentContext.SetDataModel(dobj.ToDictionary());
                //        var html = hjv.Render(File.ReadAllText(vieWebParameterath, Encoding.UTF8));

                //        //CurrentContext.Response.Charset = "UTF-8";
                //        CurrentContext.Response.ContentType = ResponseHeader_ContentType.html + ";charset=utf-8";
                //        CurrentContext.Response.WriteAsync(html).Wait();
                //    }
                //    else
                //    {
                //        CurrentContext.Response.WriteAsync("File Not Found!").Wait();
                //    }
                //}
            }
            else if (d.ContentType == GoResponseDataType.RazorView)
            {

                //Mvc进行视图展示
                var htmlstr = GlobalCommon.Proxys["razor"].CallModule<string>(new
                {
                    ViewPath = d.ViewPath,
                    Model = d.MvcModuleData,
                    ViewList = d.Domain(DomainKey.VIEW_LIST),
                    HttpContext = p.CurrentHttpContext
                }).GetAwaiter().GetResult();
                //var htmlstr =  _render.RenderViewToString(_excutefilepath, d.ViewPath, p.CurrentHttpContext, d.MvcModuleData, d.Domain(DomainKey.VIEW_LIST)).GetAwaiter().GetResult();
                GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"Razor render cast time:{(DateTime.Now - dt).TotalMilliseconds}ms "); dt = DateTime.Now;
                var msgbytelength = Encoding.UTF8.GetByteCount(htmlstr);
                CurrentContext.Response.Headers.Add("Content-Length", msgbytelength + "");
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.html + ";charset=utf-8";
                CurrentContext.Response.WriteAsync(htmlstr);
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
