using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;

namespace EFFC.Frame.Net.WebControlLib
{
    /// <summary>
    /// Validator Delegate On Ajax
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void OnAjaxValidatorDelegate(object sender, AjaxValidatorEventArgs e);

    /// <summary>
    /// AJAX CustomValidator
    /// </summary>
    [ToolboxBitmapAttribute(typeof(CustomValidator))]
    public class AjaxValidator : CustomValidator
    {
        /// <summary>
        /// method
        /// </summary>
        public AjaxValidator()
        {
            base.ClientValidationFunction = "AjaxValidatorFunction";

        }

        /// <summary>
        /// event
        /// </summary>
        public event OnAjaxValidatorDelegate OnAjaxValidatorQuest = default(OnAjaxValidatorDelegate);

        /// <summary>
        /// On Init
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            if (!base.Page.IsPostBack)
            {
                string QueryCtrl = base.Page.Request.Form["QueryCtrl"];
                string ff = base.Page.Request.Form.ToString();
                if (QueryCtrl != null && QueryCtrl == this.UniqueID.Replace("$", "_"))
                {
                    if (OnAjaxValidatorQuest != default(OnAjaxValidatorDelegate))
                    {
                        string EventField = base.Page.Request.Form["QueryData"];
                        AjaxValidatorEventArgs newEventArg = new AjaxValidatorEventArgs(EventField);

                        string requestInfo = base.Page.Request.Form["QueryInfo"];
                        if (requestInfo != null)
                        {
                            newEventArg.QueryInfo = Page.Server.HtmlDecode(requestInfo);
                        }
                        OnAjaxValidatorQuest(this, newEventArg);
                        if (newEventArg.IsAllowSubmit)
                        {
                            base.Page.Response.Write("Y");
                        }
                        else
                        {
                            base.Page.Response.Write("N");
                        }
                    }
                    base.Page.Response.End();
                }
            }
            base.OnInit(e);
        }
        /// <summary>
        /// On Load
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AjaxValidator", Framework.WebControls.Properties.Resources.AjaxValidatorScript, true);
        }

        /// <summary>
        /// Query Info Attribute
        /// </summary>
        [DefaultValue(null)]
        public string QueryInfo { get { return queryInfo; } set { queryInfo = value; } }
        private string queryInfo = null;

        /// <summary>
        /// Render
        /// </summary>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (!DesignMode)
            {
                base.Attributes.Add("AjaxUrl", "http://" + Page.Request.Url.Authority + Page.Request.Url.AbsolutePath);
                if (queryInfo != null)
                {
                    base.Attributes.Add("QueryInfo", Page.Server.HtmlEncode(queryInfo));
                }
            }
            base.Render(writer);
        }
    }

    /// <summary>
    /// AJAX Validator Eventargs
    /// </summary>
    public class AjaxValidatorEventArgs : EventArgs
    {
        /// <summary>
        /// AJAX Validator Eventargs
        /// </summary>
        /// <param name="_queryData"></param>
        public AjaxValidatorEventArgs(string _queryData)
            : base()
        {
            queryData = _queryData;
        }
        /// <summary>
        /// Is or isn't Allow Submit Attribute
        /// </summary>
        [DefaultValue(false)]
        public bool IsAllowSubmit { get { return isAllowSubmit; } set { isAllowSubmit = value; } }
        private bool isAllowSubmit = false;

        /// <summary>
        /// Info Attribute
        /// </summary>
        [DefaultValue("")]
        public string Info { get { return info; } set { info = value; } }
        private string info = "";

        /// <summary>
        /// Query Data Attribute
        /// </summary>
        [DefaultValue(null)]
        public string QueryData { get { return queryData; } }
        private string queryData = null;

        /// <summary>
        /// QueryInfo Attribute
        /// </summary>
        [DefaultValue(null)]
        public string QueryInfo { get { return queryInfo; } set { queryInfo = value; } }
        private string queryInfo = null;
    }
}
