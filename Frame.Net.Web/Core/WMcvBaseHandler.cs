using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using System.Web.WebPages;
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
using System.Threading.Tasks;
using EFFC.Frame.Net.Business.Engine;
using EFFC.Frame.Net.Global;

namespace EFFC.Frame.Net.Web.Core
{
    /// <summary>
    /// WITS.Frame2中的MVC框架处理器，用于处理后缀为view的请求。请求格式为Logic.action.view
    /// </summary>
    public abstract class WMcvBaseHandler<WP, WD> : WebBaseHandler<WP,WD>
        where WP : WebParameter
        where WD : WMvcData
    {
        public override bool IsReusable
        {
            get { return false; }
        }

        protected override void Run(WP p, WD d)
        {
            StringWriter sw = new StringWriter();
            try
            {
                bool isSuccess = RunMe(p, d);
                //Mvc进行视图展示
                
                if (string.IsNullOrEmpty(d.RedirectUri))
                {
                    WMvcView.RenderView(p, d, CurrentContext, sw);
                    d.SetValue("ViewHtmlCode", sw.ToString());
                    if (!IsWebSocket)
                    {
                        CurrentContext.Response.Write(sw.ToString());
                    }
                    else
                    {
                        var v = ComFunc.FormatJSON(sw.ToString());
                        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(v.ToJSONString()));
                        CurrentSocket.SendAsync(buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
                    }
                }
                else
                {
                    AfterProcess(CurrentContext, p, d);
                    CurrentContext.Response.Redirect(d.RedirectUri, false);
                }
            }
            finally
            {
                sw.Flush();
                sw.Close();
                sw.Dispose();
                sw = null;
                p.Resources.ReleaseAll();
            }
        }

        protected abstract bool RunMe(WP p, WD d);
    }
}
