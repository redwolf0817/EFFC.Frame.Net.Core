//******************************************************************
//*  作    者：未知
//*  功能說明：FileUpload公共方法類
//*  創建日期：未知
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Reflection;
using System.Web.UI;
using System.IO;

namespace EFFC.Frame.Net.WebControlLib
{
    /// <summary>
    /// FileUpload公共方法類
    /// </summary>
    internal static class FileUploadUtility
    {
        private static MethodInfo encodeStringMethodInfo;
        /// <summary>
        /// 建構程式
        /// </summary>
        static FileUploadUtility()
        {
            Assembly assembly = typeof(ScriptManager).Assembly;
            Type pageRequestManagerType = assembly.GetType("System.Web.UI.PageRequestManager");
            encodeStringMethodInfo = pageRequestManagerType.GetMethod(
                "EncodeString", BindingFlags.Static | BindingFlags.NonPublic);
        }
        /// <summary>
        /// JavaScript的開始或結束語句
        /// </summary>
        /// <param name="response">HttpResponse二進制流</param>
        /// <param name="begin">是否爲開始</param>
        internal static void WriteScriptBlock(HttpResponse response, bool begin)
        {
            string scriptBegin = "<script type='text/javascript' language='javascript'>window.__f__=function(){/*";
            string scriptEnd = "*/}</script>";

            response.Write(begin ? scriptBegin : scriptEnd);
        }
        /// <summary>
        /// 以指定Encode
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="content"></param>
        internal static void EncodeString(TextWriter writer, string type, string id, string content)
        {
            encodeStringMethodInfo.Invoke(null, new object[] { writer, type, id, content });
        }
        /// <summary>
        /// 判斷請求是否爲__IsInAjaxFileUploading__回傳的請求
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        internal static bool IsInIFrameAsyncPostBack(NameValueCollection requestBody)
        {
            string[] values = requestBody.GetValues("__AjaxFileUploading__");

            if (values == null) return false;

            foreach (string value in values)
            {
                if (value == "__IsInAjaxFileUploading__")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
