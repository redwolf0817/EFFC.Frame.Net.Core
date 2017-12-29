//******************************************************************
//*      ゼ
//*  弧FileUploadそよ猭摸
//*  承ら戳ゼ
//*  э癘魁
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;

namespace EFFC.Frame.Net.WebControlLib
{
    /// <summary>
    /// FileUploadModule
    /// </summary>
    public class FileUploadModule : IHttpModule
    {
        /// <summary>
        /// ﹍て
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += new EventHandler(PreSendRequestHeadersHandler);
        }
        /// <summary>
        /// эResponseHeader磞瓃?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreSendRequestHeadersHandler(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpResponse response = application.Response;
            //* 狦琌Τfileupload北ン肚叫―
            if (response.StatusCode == 302 &&
                FileUploadUtility.IsInIFrameAsyncPostBack(application.Request.Params))
            {
                string redirectLocation = response.RedirectLocation;
                List<HttpCookie> cookies = new List<HttpCookie>(response.Cookies.Count);

                for (int i = 0; i < response.Cookies.Count; i++)
                {
                    cookies.Add(response.Cookies[i]);
                }

                response.ClearContent();
                response.ClearHeaders();

                for (int i = 0; i < cookies.Count; i++)
                {
                    response.AppendCookie(cookies[i]);
                }

                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.ContentType = "text/plain";

                FileUploadUtility.WriteScriptBlock(response, true);

                StringBuilder sb = new StringBuilder();
                TextWriter writer = new StringWriter(sb);
                FileUploadUtility.EncodeString(writer, "pageRedirect", String.Empty, redirectLocation);
                response.Write(sb.Replace("*/", "*//*").ToString());

                FileUploadUtility.WriteScriptBlock(response, false);

                response.End();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose() { }
    }
}
