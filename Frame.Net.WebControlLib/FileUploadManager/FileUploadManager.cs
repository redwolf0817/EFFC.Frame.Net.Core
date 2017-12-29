//******************************************************************
//*      ゼ
//*  弧AJAX肚北摸
//*  承ら戳ゼ
//*  э癘魁
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
//******************************************************************
//*      ゼ
//*  弧FileUploadそよ猭摸
//*  承ら戳ゼ
//*  э癘魁
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.IO;
using System.Web;
using System.Globalization;

namespace EFFC.Frame.Net.WebControlLib
{
    /// <summary>
    /// ゅン肚北摸
    /// </summary>
    [PersistChildren(false)]
    [ParseChildren(true)]
    [NonVisualControl]
    public class FileUploadManager : Control
    {
        // ScriptManager members;
		private static FieldInfo isInAsyncPostBackFieldInfo;
		private static PropertyInfo pageRequestManagerPropertyInfo;

		// PageRequestManager members;
		private static MethodInfo onPageErrorMethodInfo;
		private static MethodInfo renderPageCallbackMethodInfo;

        /// <summary>
        /// 篶祘Α.﹍て
        /// </summary>
        static FileUploadManager()
		{
            Type scriptManagerType = typeof(ClientScriptManager);
			isInAsyncPostBackFieldInfo = scriptManagerType.GetField(
				"_isInAsyncPostBack",
				BindingFlags.Instance | BindingFlags.NonPublic);
			pageRequestManagerPropertyInfo = scriptManagerType.GetProperty(
				"PageRequestManager",
				BindingFlags.Instance | BindingFlags.NonPublic);

			Assembly assembly = scriptManagerType.Assembly;
			Type pageRequestManagerType = assembly.GetType("System.Web.UI.PageRequestManager");
			onPageErrorMethodInfo = pageRequestManagerType.GetMethod(
				"OnPageError", BindingFlags.Instance | BindingFlags.NonPublic);
			renderPageCallbackMethodInfo = pageRequestManagerType.GetMethod(
				"RenderPageCallback", BindingFlags.Instance | BindingFlags.NonPublic);
		}
        /// <summary>
        /// 眔讽玡いFileUploadManager
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static FileUploadManager GetCurrent(Page page)
		{
            return page.Items[typeof(FileUploadManager)] as FileUploadManager;
		}
        /// <summary>
        /// 琌FileUploadManager
        /// </summary>
		private bool isInAjaxUploading = false;

		private bool _SupportAjaxUpload = true;
        /// <summary>
        /// 琌やAjaxUpload
        /// </summary>
		public bool SupportAjaxUpload
		{
			get { return _SupportAjaxUpload; }
			set { _SupportAjaxUpload = value; }
		}
        /// <summary>
        /// 糶﹍て
        /// </summary>
        /// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
            //* 狦硂竒ΤFileUploadManager玥厨岿
            if (this.Page.Items.Contains(typeof(FileUploadManager)))
			{
                throw new InvalidOperationException("One FileUploadManager per page.");
			}

            this.Page.Items[typeof(FileUploadManager)] = this;

			this.EnsureIsInAjaxFileUploading();
		}
        /// <summary>
        /// 糶北ン酶礶玡ㄆン
        /// </summary>
        /// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (this.isInAjaxUploading)
			{
				this.Page.SetRenderMethodDelegate(new RenderMethod(this.RenderPageCallback));
			}

			if (this.Page.IsPostBack || !this.SupportAjaxUpload) return;

            if (!ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
			{
				 ScriptReference script = new ScriptReference(
                    "EFFC.Frame.Net.WebControlLib.FileUploadManager.FileUploadManager.js", this.GetType().Assembly.FullName);
				ScriptManager.GetCurrent(this.Page).Scripts.Add(script);
			}
		}
        /// <summary>
        /// ネΘCallBack
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="pageControl"></param>
		private void RenderPageCallback(HtmlTextWriter writer, Control pageControl)
		{
            FileUploadUtility.WriteScriptBlock(this.Page.Response, true);

			StringBuilder sb = new StringBuilder();
			HtmlTextWriter innerWriter = new HtmlTextWriter(new StringWriter(sb));
			renderPageCallbackMethodInfo.Invoke(this.PageRequestManager, new object[] { innerWriter, pageControl });

			writer.Write(sb.Replace("*/", "*//*").Replace("</script", "</scriptt").ToString());

            FileUploadUtility.WriteScriptBlock(this.Page.Response, false);
		}
        /// <summary>
        /// 絋玂琌AjaxFileUploadじン肚
        /// </summary>
		private void EnsureIsInAjaxFileUploading()
		{
            this.isInAjaxUploading = FileUploadUtility.IsInIFrameAsyncPostBack(this.Page.Request.Params);

			if (this.isInAjaxUploading)
			{
				isInAsyncPostBackFieldInfo.SetValue(
					ScriptManager.GetCurrent(this.Page),
					true);

				this.Page.Error += new EventHandler(Page_Error);
			}
		}
        /// <summary>
        /// 矪瞶岿Hander
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Page_Error(object sender, EventArgs e)
		{
            FileUploadUtility.WriteScriptBlock(this.Page.Response, true);

			onPageErrorMethodInfo.Invoke(this.PageRequestManager, new object[] { sender, e });

            FileUploadUtility.WriteScriptBlock(this.Page.Response, false);
		}
        /// <summary>
        /// 
        /// </summary>
		private object _PageRequestManager;
        /// <summary>
        /// 弄㎝糶PageRequestManager妮┦
        /// </summary>
		private object PageRequestManager
		{
			get
			{
				if (this._PageRequestManager == null)
				{
					this._PageRequestManager = pageRequestManagerPropertyInfo.GetValue(
						ScriptManager.GetCurrent(this.Page), null);
				}

				return this._PageRequestManager;
			}
		}
    }
}
