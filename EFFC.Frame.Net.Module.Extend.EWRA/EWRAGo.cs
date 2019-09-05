using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using EFFC.Frame.Net.Module.Web.Modules;
using EFFC.Frame.Net.Module.Web.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.EWRA.Constants;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using Frame.Net.Base.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;

namespace EFFC.Frame.Net.Module.Extend.EWRA
{
    public class EWRAGo : WebBaseRestAPI<EWRAParameter, EWRAData>
    {
        static string restroothome = "~/";
        static string page_404 = "";
        static string page_403 = "";
        static string page_500 = "";
        public override string Name => "EWRA";

        public override string Description => "EFFC Web Rest API";

        public override void Dispose()
        {
            
        }
        protected override void OnUsed(ProxyManager ma, dynamic options)
        {
            if (options != null)
            {
                if (ComFunc.nvl(options.RestAPIRootHome) != "")
                {
                    restroothome = options.RestAPIRootHome;
                }
                if(ComFunc.nvl(options.PagePath4Forbidden) != "")
                {
                    page_403 = ComFunc.nvl(options.PagePath4Forbidden);
                }
                if (ComFunc.nvl(options.PagePath4NotFound) != "")
                {
                    page_404 = ComFunc.nvl(options.PagePath4NotFound);
                }
                if (ComFunc.nvl(options.PagePath4NotFound) != "")
                {
                    page_500 = ComFunc.nvl(options.PagePath4Error);
                }
            }

            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO,
                        string.Format("EWRAGo的RootHome设定为{0},如要调整，请在ProxyManager.UseProxy中的options参数设定RestAPIRootHome的值", restroothome));
            //构建路由表
            ma.UseProxy<EWRABusiProxy>("busi", options);
        }

        protected override void InvokeAction(EWRAParameter p, EWRAData d)
        {
            var dt = DateTime.Now;
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"{ComFunc.nvl(p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_ip"])} excute:{p.MethodName} \"{p.RequestRoute}\" InvokeAction begin");
            object obj = d;
            if (p.MethodName.ToUpper() == "OPTIONS")
            {
                //
                d.Result = "";
                d.StatusCode = RestStatusCode.OK;
            }
            else
            {
                GlobalCommon.Proxys["busi"].CallModule(ref obj, new object[] { p });
            }
            
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"{ComFunc.nvl(p[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_ip"])} excute:{p.MethodName} \"{p.RequestRoute}\" InvokeAction cast time:{(DateTime.Now - dt).TotalMilliseconds}ms "); dt = DateTime.Now;
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            var ep = (EWRAParameter)p;
            var ed = (EWRAData)d;
            ed.StatusCode = Constants.RestStatusCode.INVALID_REQUEST;
            var isdebug = p[DomainKey.CONFIG, "DebugMode"] == null ? false : (bool)p[DomainKey.CONFIG, "DebugMode"];

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

            var contentbody = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            var errormsg = "";
            if (isdebug)
            {
                errormsg = string.Format("出错了，{0}", errlog); ;
            }
            else
            {
                errormsg = string.Format("系统错误!!!（错误编号：{0}）", errorCode);
            }

            contentbody.error = errormsg;
            var jsonmsg = ((FrameDLRObject)contentbody).ToJSONString(true);
            var msgbytelength = Encoding.UTF8.GetByteCount(jsonmsg);

            SetHeaders(ep, ed);
            if (page_500 == "")
            {
                ep.CurrentHttpContext.Response.StatusCode = (int)ed.StatusCode;
                ep.CurrentHttpContext.Response.ContentType = ResponseHeader_ContentType.Map(ComFunc.Enum2String<RestContentType>(ed.ContentType).ToLower()) + ";charset=utf-8";
                ep.CurrentHttpContext.Response.Headers.Add("Content-Length", msgbytelength + "");
                ep.CurrentHttpContext.Response.WriteAsync(jsonmsg);
                ep.CurrentHttpContext.Response.Body.FlushAsync();
            }
            else
            {
                ResponsePageContent(page_500, ep, ed);
            }
        }

        protected override void SetResponseContent(EWRAParameter p, EWRAData d)
        {
            if (!string.IsNullOrEmpty(d.RedirectUri))
            {
                p.CurrentHttpContext.Response.Redirect(d.RedirectUri);
                return;
            }

            var obj = p.CurrentHttpContext.Request.Headers;
            String agent = ComFunc.nvl(obj["USER-AGENT"]);

            var contentbody = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            SetHeaders(p, d);
            var arr = new string[] { "put", "post", "patch", "options" };
            var msgbytelength = 0;
            var resultmsg = "";
            if (d.ContentType == RestContentType.JSON)
            {
                if (d.StatusCode == RestStatusCode.NONE)
                {
                    if (p.MethodName == "get")
                    {
                        if (d.Result != null)
                        {
                            contentbody.result = d.Result;
                            d.StatusCode = RestStatusCode.OK;
                        }
                        else
                        {
                            contentbody.error = "资源未找到或非法请求";
                            d.StatusCode = RestStatusCode.NOT_FOUND;
                        }
                    }
                    else if (p.MethodName == "delete")
                    {
                        if (d.Result != null && d.Result is bool)
                        {
                            if ((bool)d.Result)
                                d.StatusCode = RestStatusCode.NO_CONTENT;
                            else
                            {
                                contentbody.error = "操作失败";
                                d.StatusCode = RestStatusCode.NOT_FOUND;
                            }
                        }
                        else
                        {
                            if (d.Result != null)
                            {
                                contentbody.result = d.Result;
                                d.StatusCode = RestStatusCode.OK;
                            }
                            else
                            {
                                contentbody.error = "资源未找到或非法请求";
                                d.StatusCode = RestStatusCode.NOT_FOUND;
                            }
                        }
                    }
                    else if (arr.Contains(p.MethodName))
                    {
                        if (d.Result != null)
                        {
                            contentbody.result = d.Result;
                            d.StatusCode = RestStatusCode.CREATED;
                        }
                        else
                        {
                            contentbody.error = "资源未找到或非法请求";
                            d.StatusCode = RestStatusCode.NOT_FOUND;
                        }
                    }
                    else
                    {
                        contentbody.error = "非法请求";
                        d.StatusCode = RestStatusCode.FORBIDDEN;
                    }
                }
                else
                {
                    if ((int)d.StatusCode >= 400)
                    {
                        contentbody.error = d.Error;
                    }
                    else
                    {
                        contentbody.result = d.Result;
                    }

                }

                resultmsg = ((FrameDLRObject)contentbody).ToJSONString(true);
                msgbytelength = Encoding.UTF8.GetByteCount(resultmsg);
                p.CurrentHttpContext.Response.StatusCode = (int)d.StatusCode;
                if (p.CurrentHttpContext.Response.StatusCode != 204)
                {
                    p.CurrentHttpContext.Response.ContentType = ResponseHeader_ContentType.json + ";charset=utf-8";
                    p.CurrentHttpContext.Response.Headers.Add("Content-Length", msgbytelength + "");
                    p.CurrentHttpContext.Response.WriteAsync(resultmsg);
                }
                p.CurrentHttpContext.Response.Body.FlushAsync();
            }
            else if (d.ContentType == RestContentType.Binary)
            {
                p.CurrentHttpContext.Response.Headers.Add("Content-Disposition", "attachment;filename=" + ComFunc.UrlEncode(ComFunc.nvl(d.Download_File_Name)));
                p.CurrentHttpContext.Response.ContentType = d.File_ContentType;
                if (d.Result is byte[])
                {
                    p.CurrentHttpContext.Response.Headers.Add("Content-Length", ((byte[])d.Result).Length + "");
                    p.CurrentHttpContext.Response.Body.Write((byte[])d.Result, 0, ((byte[])d.Result).Length);
                }
                else if (d.Result is Stream)
                {
                    var bytes = ComFunc.StreamToBytes((Stream)d.Result);
                    p.CurrentHttpContext.Response.Headers.Add("Content-Length", bytes.Length + "");
                    p.CurrentHttpContext.Response.Body.Write(bytes, 0, bytes.Length);
                }
                else
                {
                    p.CurrentHttpContext.Response.WriteAsync(ComFunc.nvl(d.Result)).Wait();
                }
                p.CurrentHttpContext.Response.Body.Flush();
            }
            else if (d.ContentType == RestContentType.HTML)
            {
                resultmsg = ComFunc.nvl(d.Result);
                msgbytelength = Encoding.UTF8.GetByteCount(resultmsg);
                p.CurrentHttpContext.Response.StatusCode = (int)d.StatusCode;
                if (p.CurrentHttpContext.Response.StatusCode == 200)
                {
                    p.CurrentHttpContext.Response.ContentType = ResponseHeader_ContentType.html + ";charset=utf-8";
                    p.CurrentHttpContext.Response.Headers.Add("Content-Length", msgbytelength + "");
                    p.CurrentHttpContext.Response.WriteAsync(resultmsg).Wait();

                    p.CurrentHttpContext.Response.Body.Flush();
                }
                else if(p.CurrentHttpContext.Response.StatusCode == 403)
                {
                    if(page_403 != "")
                    {
                        ResponsePageContent(page_403, p, d);
                    }
                }
                else if (p.CurrentHttpContext.Response.StatusCode == 404)
                {
                    if (page_404 != "")
                    {
                        ResponsePageContent(page_404, p, d);
                    }
                }
            }
            else
            {
                resultmsg = ComFunc.nvl(d.Result);
                msgbytelength = Encoding.UTF8.GetByteCount(resultmsg);
                p.CurrentHttpContext.Response.StatusCode = (int)d.StatusCode;
                if (p.CurrentHttpContext.Response.StatusCode != 204)
                {
                    p.CurrentHttpContext.Response.ContentType = ResponseHeader_ContentType.txt + ";charset=utf-8";
                    p.CurrentHttpContext.Response.Headers.Add("Content-Length", msgbytelength + "");
                    p.CurrentHttpContext.Response.WriteAsync(resultmsg);
                    p.CurrentHttpContext.Response.Body.FlushAsync();
                    
                }
                else if (p.CurrentHttpContext.Response.StatusCode == 403)
                {
                    if (page_403 != "")
                    {
                        ResponsePageContent(page_403, p, d);
                    }
                }
                else if (p.CurrentHttpContext.Response.StatusCode == 404)
                {
                    if (page_404 != "")
                    {
                        ResponsePageContent(page_404, p, d);
                    }
                }
            }
        }

        protected override void BeforeProcess(EWRAParameter p, EWRAData d)
        {
            LoadConfig(p, d);
            base.BeforeProcess(p, d);
            //抓取验证token
            p.AuthorizedToken = ComFunc.nvl(p[DomainKey.HEADER, "Authorization"]).Replace("Bearer ","");
            //滤掉RootHome
            if(restroothome != "~/")
            {
                p.RequestRoute = (p.RequestRoute + "/").Replace("//","/").ToLower().Replace(restroothome.ToLower().Replace("~", ""), "");
            }
            
        }

        protected virtual void LoadConfig(EWRAParameter p, EWRAData d)
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
            foreach (var item in MyConfig.GetConfigurationList("CORS"))
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

        private void SetHeaders(EWRAParameter p, EWRAData d)
        {
            var allow_origin = "*";
            var allow_methods = "GET,POST,PATCH,PUT,DELETE,OPTIONS";
            var allow_headers = "Content-Type,Content-Length, Authorization, Accept,X-Requested-With,Authorization";
            var x_frame_options = "DENY";
            var x_content_type_options = "nosniff";
            var x_xss_protection = "1; mode=block";

            if(ComFunc.nvl(p[DomainKey.CONFIG,"Allow_Origin"]) != "")
            {
                allow_origin = ComFunc.nvl(p[DomainKey.CONFIG, "Allow_Origin"]);
            }
            if (ComFunc.nvl(p[DomainKey.CONFIG, "Allow_Methods"]) != "")
            {
                allow_methods = ComFunc.nvl(p[DomainKey.CONFIG, "Allow_Methods"]);
            }
            if (ComFunc.nvl(p[DomainKey.CONFIG, "Allow_Headers"]) != "")
            {
                allow_headers = ComFunc.nvl(p[DomainKey.CONFIG, "Allow_Headers"]);
            }
            if (ComFunc.nvl(p[DomainKey.CONFIG, "X_Frame_Options"]) != "")
            {
                x_frame_options = ComFunc.nvl(p[DomainKey.CONFIG, "X_Frame_Options"]);
            }
            if (ComFunc.nvl(p[DomainKey.CONFIG, "X_Content_Type_Options"]) != "")
            {
                x_content_type_options = ComFunc.nvl(p[DomainKey.CONFIG, "X_Content_Type_Options"]);
            }
            if (ComFunc.nvl(p[DomainKey.CONFIG, "X_XSS_Protection"]) != "")
            {
                x_xss_protection = ComFunc.nvl(p[DomainKey.CONFIG, "X_XSS_Protection"]);
            }

            p.CurrentHttpContext.Response.Headers.TryAdd("Access-Control-Allow-Origin", allow_origin);
            //CORS的Options验证处理
            p.CurrentHttpContext.Response.Headers.TryAdd("Access-Control-Allow-Methods", allow_methods);
            p.CurrentHttpContext.Response.Headers.TryAdd("Access-Control-Allow-Headers", allow_headers);
            p.CurrentHttpContext.Response.Headers.TryAdd("X-Frame-Options", x_frame_options);
            p.CurrentHttpContext.Response.Headers.TryAdd("X-Content-Type-Options", x_content_type_options);
            p.CurrentHttpContext.Response.Headers.TryAdd("X-XSS-Protection", x_xss_protection);
            if (p.CurrentHttpContext.Request.IsHttps)
            {
                p.CurrentHttpContext.Response.Headers.TryAdd("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
            }
        }
    }
}
