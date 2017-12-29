using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Business.Engine;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace Frame.Net.Web.Module
{
    /// <summary>
    /// 基于Razor的view请求处理（该模块为保留模块，以便兼容原来的程式，其功能已被Go处理模块替代）
    /// </summary>
    /// <typeparam name="WP"></typeparam>
    /// <typeparam name="WD"></typeparam>
    public abstract class WMvcBaseModule : WebBaseModule<WebParameter, WMvcData>
    {
        bool isSuccess = true;
        protected abstract bool RunMe(WebParameter p, WMvcData d);

        protected override void InvokeAction(WebParameter p, WMvcData d)
        {
            isSuccess = RunMe(p, d);
        }

        protected override void SetResponseContent(WebParameter p, WMvcData d)
        {
            //Mvc进行视图展示
            if (string.IsNullOrEmpty(d.RedirectUri))
            {
                var htmlstr = WMvcView.RenderView(p, d, CurrentContext);
                d.SetValue("ViewHtmlCode", htmlstr);
                
                if (!IsWebSocket)
                {
                    CurrentContext.Response.ContentType = ResponseHeader_ContentType.html + ";charset=utf-8";
                    CurrentContext.Response.WriteAsync(htmlstr).Wait();
                }
                else
                {
                    var v = ComFunc.FormatJSON(htmlstr);
                    var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(v.ToJSONString()));
                    CurrentSocket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
                }
            }
            else
            {
                AfterProcess(p, d);
                CurrentContext.Response.Redirect(d.RedirectUri, false);
            }
        }
    }
}
