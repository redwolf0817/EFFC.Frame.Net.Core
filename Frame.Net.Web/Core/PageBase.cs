using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Web.Proxy;
using EFFC.Frame.Net.WebControlLib.GridViewCustom;
using EFFC.Frame.Net.WebControlLib.Input;
using EFFC.Frame.Net.WebControlLib.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EFFC.Frame.Net.Web.Core
{
    public abstract partial class PageBase:Page
    {
        protected Dictionary<string, Control> _controls_DataMap = new Dictionary<string, Control>();
        protected Dictionary<string, Control> _controls_Permission = new Dictionary<string, Control>();
        protected Dictionary<string, Control> _controls_Special = new Dictionary<string, Control>();
        protected Dictionary<string, Control> _controls_global = new Dictionary<string, Control>();
        protected Dictionary<string, Control> _controls_auto_process = new Dictionary<string, Control>();
        private WebParameter _wp = null;
        private WebFormData _d = new WebFormData();
        List<string> s_systemPostFields = new List<string>();
        private static readonly string UniqueFilePathSuffixID = "__ufps";
        string pagename = "";

        protected void CallLogic()
        {
            ModuleProxyManager<WebParameter, WebFormData>.Call<WebFormBusinessProxy>(_wp, _d);
        }

        public virtual void OnError(Exception ex)
        {
            _wp.Resources.RollbackTransaction(_wp.CurrentTransToken);
            
            GlobalCommon.ExceptionProcessor.ProcessException(this, ex, _wp, _d);
        }

        public virtual void FinalyProcess()
        {
            _wp.Resources.CommitTransaction(_wp.CurrentTransToken);
            _wp.Resources.ReleaseAll();
        }
        /// <summary>
        /// 页面参数
        /// </summary>
        public WebParameter WebParameters
        {
            get
            {
                return _wp;
            }
        }
        /// <summary>
        /// 页面进过处理后的数据
        /// </summary>
        public WebFormData WebData
        {
            get
            {
                return _d;
            }
        }

        /// <summary>
        /// 当前页面的名称
        /// </summary>
        public string PageName
        {
            get
            {
                if (pagename == "")
                {
                    //获取本页面的名称
                    string url = HttpContext.Current.Request.Url.PathAndQuery.ToString();
                    if (url.Contains('?'))
                    {
                        url = url.Substring(0, url.IndexOf('?'));
                    }
                    int tag = url.LastIndexOf("/") + 1;
                    int mm = url.IndexOf(".aspx") - url.LastIndexOf("/") - 1;
                    pagename = url.Substring(tag, mm);
                }
                return pagename;
            }
        }

        protected sealed override void OnLoad(EventArgs e)
        {
            try
            {
                FindControls(this);
                _d.PageModule = GetModule();
                
                if (!IsPostBack)
                {
                    _wp.RequestResourceName = this.PageName;
                    _wp.Action = PostBackActionKey.LOAD;
                    //进行加载的logic预调用
                    CallLogic();
                    SetPageDataBind();
                    SetPageModelValue();
                }
                OnPageLoad();
                base.OnLoad(e);
                AutoProcessPostDataChange();
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            finally
            {
                FinalyProcess();
            }
        }

        private void AutoProcessPostDataChange()
        {
            if (IsPostBack)
            {
                List<Control> l = new List<Control>();
                //为postdatachanged事件做logic的预调用
                if (base.Request.Form["__EVENTTARGET"] != null)
                {
                    var s = base.Request.Form["__EVENTTARGET"];
                    var c = FindControl(s);
                    if (c != null && c is IPostBackDataHandler && _controls_auto_process.ContainsKey(c.ID))
                    {
                        if (((IPostBackDataHandler)c).LoadPostData(s, base.Request.Form))
                        {
                            l.Add(c);
                        }
                    }
                }
                else
                {
                    foreach (string s in base.Request.Form)
                    {
                        if (!s_systemPostFields.Contains(s))
                        {
                            var c = FindControl(s);
                            if (c != null && c is IPostBackDataHandler && _controls_auto_process.ContainsKey(c.ID))
                            {

                                if (((IPostBackDataHandler)c).LoadPostData(s, base.Request.Form))
                                {
                                    l.Add(c);
                                }
                            }
                        }
                    }
                }

                foreach (var c in l)
                {
                    _wp.RequestResourceName = this.PageName;
                    _wp.Action =  PostBackActionKey.POST_BACK_DATA_CHANGE + "." + c.ID;
                    CallLogic();
                    //回写页面数据
                    SetPageDataBind();
                    SetPageModelValue();
                }
            }
        }



        protected virtual bool IsRaiseEvent()
        {
            return true;
        }

        protected sealed override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            try
            {
                if (IsRaiseEvent())
                {
                    BeforeEventRaise(sourceControl, eventArgument);
                    base.RaisePostBackEvent(sourceControl, eventArgument);
                    AfterEventRaise(sourceControl, eventArgument);
                }
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException)
                {

                }
                else
                {
                    OnError(ex);
                }
            }
            finally
            {
                FinalyProcess();
            }
        }
        /// <summary>
        /// 当页面事件发生时
        /// </summary>
        /// <param name="sourceControl"></param>
        /// <param name="eventArgument"></param>
        protected virtual void BeforeEventRaise(IPostBackEventHandler sourceControl, string eventArgument)
        {
            if (!(sourceControl is PageNavigation) && _controls_auto_process.ContainsKey(((Control)sourceControl).ID))
            {
                //执行预调用logic
                WebParameters.SetValue("__CurrentEventArgs", ComFunc.nvl(eventArgument));
                WebParameters.RequestResourceName = this.PageName;
                WebParameters.Action = PostBackActionKey.POST_BACK_EVENT + "." + ((Control)sourceControl).ID;
                CallLogic();
                //回写页面数据
                SetPageDataBind();
                SetPageModelValue();
            }
        }

        /// <summary>
        /// 当页面事件发生之後
        /// </summary>
        /// <param name="sourceControl"></param>
        /// <param name="eventArgument"></param>
        protected virtual void AfterEventRaise(IPostBackEventHandler sourceControl, string eventArgument)
        {

        }
        /// <summary>
        /// 注册自动执行控件
        /// </summary>
        /// <param name="c"></param>
        public void RegisterAutoProcess(Control c)
        {
            if (c is IPostBackDataHandler || c is IPostBackEventHandler)
            {
                if (!_controls_auto_process.ContainsKey(c.ID))
                    _controls_auto_process.Add(c.ID, c);
            }
        }



        /// <summary>
        /// 提供给控件进行注册使用
        /// </summary>
        /// <typeparam name="T">Control,IWebControl</typeparam>
        /// <param name="c"></param>
        public void RegisterWebControlDataMap(Control c)
        {
            if (c is IWebDataControl)
            {
                if (_controls_DataMap.ContainsKey(((IWebDataControl)c).MapDataField))
                {
                    _controls_DataMap[((IWebDataControl)c).MapDataField] = c;
                }
                else
                {
                    _controls_DataMap.Add(((IWebDataControl)c).MapDataField, c);
                }
            }
            else
            {
                if (_controls_DataMap.ContainsKey(c.ID))
                {
                    _controls_DataMap[c.ID] = c;
                }
                else
                {
                    _controls_DataMap.Add(c.ID, c);
                }
            }
        }
        /// <summary>
        /// 提供给需要全球化控件进行注册使用
        /// </summary>
        /// <typeparam name="T">Control,IWebControl</typeparam>
        /// <param name="c"></param>
        public void RegisterWebControlGlobalizationMap(Control c)
        {
            if (c is Label
                || c is GridView
                || c is Button
                || c is LinkButton
                || c is ImageButton
                || c is Image)
            {
                if (_controls_global.ContainsKey(c.ID))
                {
                    _controls_global[c.ID] = c;
                }
                else
                {
                    _controls_global.Add(c.ID, c);
                }
            }
        }
        /// <summary>
        /// 提供给特殊控件进行注册使用
        /// </summary>
        /// <typeparam name="T">Control,IWebControl</typeparam>
        /// <param name="c"></param>
        public void RegisterSpecialControl(Control c)
        {
            if (_controls_Special.ContainsKey(c.ID))
            {
                _controls_Special[c.ID] = c;
            }
            else
            {
                _controls_Special.Add(c.ID, c);
            }
        }

        /// <summary>
        /// 提供给控件进行注册使用
        /// </summary>
        /// <param name="c">Control,IActionPermission</param>
        public void RegisterWebControlPermission(Control c)
        {
            if (c is IActionPermission)
            {
                if (_controls_Permission.ContainsKey(c.ID))
                {
                    _controls_Permission[c.ID] = c;
                }
                else
                {
                    _controls_Permission.Add(c.ID, c);
                }
            }
        }

        public abstract void OnPageLoad();

        protected sealed override void OnInit(EventArgs e)
        {
            try
            {
                BeforeInit(e);
                base.OnInit(e);
                AfterInit(e);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            finally
            {
                FinalyProcess();
            }
        }

        public virtual void BeforeInit(EventArgs e)
        {
            //System key
            s_systemPostFields.Add("__EVENTTARGET");
            s_systemPostFields.Add("__EVENTARGUMENT");
            s_systemPostFields.Add("__VIEWSTATEFIELDCOUNT");
            s_systemPostFields.Add("__VIEWSTATE");
            s_systemPostFields.Add("__VIEWSTATEENCRYPTED");
            s_systemPostFields.Add("__PREVIOUSPAGE");
            s_systemPostFields.Add("__CALLBACKID");
            s_systemPostFields.Add("__CALLBACKPARAM");
            s_systemPostFields.Add("__LASTFOCUS");
            s_systemPostFields.Add(UniqueFilePathSuffixID);
            s_systemPostFields.Add("__redir");
            s_systemPostFields.Add("__EVENTVALIDATION");

            //获取请求的资源和参数
            ResourceManage rema = new ResourceManage();
            _wp.SetValue<ResourceManage>(ParameterKey.RESOURCE_MANAGER, rema);
            _wp[DomainKey.SESSION, "SessionID"] = Context.Session.SessionID;
            _wp.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath", Context.Server.MapPath("~"));
            _wp.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath_URL", Context.Request.Url.AbsoluteUri.Replace(Context.Request.FilePath, ""));

            _wp.SetValue(DomainKey.APPLICATION_ENVIRONMENT, "WebPath", Context.Request.Url.AbsoluteUri.Replace(Context.Request.RawUrl, ""));
            //设置serverinfo
            _wp[DomainKey.APPLICATION_ENVIRONMENT, "server_servername"] = Context.Server.MachineName;
            _wp[DomainKey.APPLICATION_ENVIRONMENT, "serverinfo_ip"] = Context.Request.ServerVariables["LOCAl_ADDR"];
            //设置clientinfo
            _wp[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_ip"] = Context.Request.UserHostAddress;
            _wp[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_browserversion"] = Context.Request.Browser.Version;
            _wp[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_platform"] = Context.Request.Browser.Platform;
            _wp[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_userhostname"] = Context.Request.UserHostName;


            if (Context.Request.Path != "/")
            {
                _wp.RequestResourcePath = Context.Request.PhysicalPath;
                string reqpath = Path.GetFileNameWithoutExtension(_wp.RequestResourcePath);
                string[] ss = reqpath.Split('.');
                _wp.Action = ss.Length > 1 ? ss[1] : "";
                _wp.RequestResourceName = ss[0];
            }

            foreach (string s in Context.Request.QueryString.Keys)
            {
                _wp[DomainKey.QUERY_STRING, s] = Context.Request.QueryString[s];
            }
        }
        public virtual void AfterInit(EventArgs e)
        {
        }

        public sealed override void ProcessRequest(System.Web.HttpContext context)
        {
            try
            {
                base.ProcessRequest(context);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            finally
            {
                FinalyProcess();
            }
        }


        /// <summary>
        /// 搜索頁面裏面的控件并添加到字典中
        /// </summary>
        /// <param name="contrainer"></param>
        public virtual void FindControls(Control contrainer)
        {
            if (contrainer is ContentPlaceHolder
                || contrainer.Parent is UpdatePanel
                || contrainer is Panel
                || contrainer is System.Web.UI.HtmlControls.HtmlGenericControl
                || contrainer is System.Web.UI.HtmlControls.HtmlForm)
            {
                foreach (Control c in contrainer.Controls)
                {

                    //可能会有某个控件即属于IActionPermission，又属于IWebDataControl
                    if (c is IActionPermission)
                    {
                        RegisterWebControlPermission(c);
                        RegisterWebControlGlobalizationMap(c);
                    }

                    if (c is PageNavigation)
                    {
                        RegisterSpecialControl(c);
                        RegisterWebControlGlobalizationMap(c);
                    }

                    if (c is IWebDataControl)
                    {
                        RegisterWebControlDataMap(c);
                        RegisterWebControlGlobalizationMap(c);
                    }
                    else if (c is TextBox)
                    {
                        RegisterWebControlDataMap(c);
                    }
                    if (c is Label)
                    {
                        RegisterWebControlDataMap(c);
                        RegisterWebControlGlobalizationMap(c);
                    }
                    if (c is DropDownList)
                    {
                        RegisterWebControlDataMap(c);
                    }
                    if (c is RadioButton)
                    {
                        RegisterWebControlDataMap(c);
                    }
                    if (c is RadioButtonList)
                    {
                        RegisterWebControlDataMap(c);
                    }
                    if (c is CalendarInput)
                    {
                        RegisterWebControlDataMap(c);
                    }
                    if (c is CheckBox)
                    {
                        RegisterWebControlDataMap(c);
                    }
                    if (c is CheckBoxList)
                    {
                        RegisterWebControlDataMap(c);
                    }
                    if (c is HiddenField)
                    {
                        RegisterWebControlDataMap(c);
                    }
                    if (c is GridView)
                    {
                        RegisterWebControlDataMap(c);
                        RegisterWebControlGlobalizationMap(c);
                    }

                    if (c is GridView
                        || c is Button
                        || c is LinkButton
                        || c is ImageButton
                        || c is Image)
                    {
                        RegisterWebControlGlobalizationMap(c);
                    }
                    if (c is UpdatePanel)
                    {
                        FindControls(c.Controls[0]);
                    }
                    if (c is Panel)
                    {
                        FindControls(c);
                    }
                    if (c is ContentPlaceHolder)
                    {
                        FindControls(c);
                    }
                    if (c is System.Web.UI.HtmlControls.HtmlGenericControl)
                    {
                        if (((System.Web.UI.HtmlControls.HtmlGenericControl)c).TagName.ToUpper() == "DIV")
                        {
                            FindControls(c);
                        }
                    }

                }
            }
            else
            {
                foreach (Control c in contrainer.Controls)
                {
                    FindControls(c);
                }
            }
        }



        /// <summary>
        /// 获取本页面的数据Module，1.0.0.1版本中使用
        /// </summary>
        /// <returns></returns>
        public virtual WebFormPageModel GetModule()
        {
            WebFormPageModel rtn = new WebFormPageModel();
            foreach (Control c in _controls_DataMap.Values)
            {
                if (c is IWebDataControl)
                {
                    rtn.SetValue(((IWebDataControl)c).MapDataField, GetControlValue(c));
                }
                else
                {
                    rtn.SetValue(c.ID, GetControlValue(c));
                }
            }
            return rtn;
        }
        /// <summary>
        /// 根据module的值往页面写数据
        /// </summary>
        /// <param name="module"></param>
        public virtual void SetPageModelValue(WebFormPageModel model)
        {
            foreach (Control c in _controls_DataMap.Values)
            {
                SetControlValue(c, model.GetValue(c.ID));
            }
        }
        /// <summary>
        /// 根据module的值往页面写数据
        /// </summary>
        public void SetPageModelValue()
        {
            SetPageModelValue(_d.PageModule);
        }
        /// <summary>
        /// 根据module的值进行数据绑定
        /// </summary>
        /// <param name="model"></param>
        public virtual void SetPageDataBind(WebFormPageModel model)
        {
            foreach (Control c in _controls_DataMap.Values)
            {
                SetControlDataBind(c, model.GetValue(c.ID));
            }
        }
        /// <summary>
        /// 根据module的值进行数据绑定
        /// </summary>
        public void SetPageDataBind()
        {
            SetPageDataBind(_d.DataBindModule);
        }
        /// <summary>
        /// 获取控件的值
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected object GetControlValue(Control c)
        {
            object rtn = null;

            if (c is TextBox)
            {
                rtn = ((TextBox)c).Text;
            }
            else if (c is Label)
            {
                rtn = ((Label)c).Text;
            }
            else if (c is DropDownList)
            {
                rtn = ((DropDownList)c).SelectedValue;
            }
            else if (c is RadioButton)
            {
                rtn = ((RadioButton)c).Checked;
            }
            else if (c is RadioButtonList)
            {
                rtn = ((RadioButtonList)c).SelectedValue;
            }
            else if (c is CalendarInput)
            {
                rtn = ((CalendarInput)c).Text;
            }
            else if (c is CheckBox)
            {
                rtn = ((CheckBox)c).Checked;
            }
            else if (c is CheckBoxList)
            {
                string stempvalue = "";
                foreach (ListItem item in ((CheckBoxList)c).Items)
                {
                    if (item.Selected)
                    {
                        stempvalue += stempvalue == "" ? item.Value : "," + item.Value;
                    }
                }
                rtn = stempvalue;
            }
            else if (c is HiddenField)
            {
                rtn = ((HiddenField)c).Value;
            }

            if (rtn is string)
                rtn = rtn.ToString().Trim();

            return rtn;
        }

        protected void SetControlDataBind(Control c, object value)
        {
            if (value == null) return;
            if (!(value is IListSource) && !(value is IDataSource) && !(value is IEnumerable) && !(value is DataTableStd)) return;

            if (c is DropDownList)
            {
                ((DropDownList)c).DataSource = value is DataTableStd ? ((DataTableStd)value).Value : value;
                if (string.IsNullOrEmpty(((DropDownList)c).DataTextField))
                {
                    ((DropDownList)c).DataTextField = "Text";
                }
                if (string.IsNullOrEmpty(((DropDownList)c).DataValueField))
                {
                    ((DropDownList)c).DataValueField = "Value";
                }
                ((DropDownList)c).DataBind();
            }
            else if (c is RadioButtonList)
            {
                ((RadioButtonList)c).DataSource = value is DataTableStd ? ((DataTableStd)value).Value : value;
                if (string.IsNullOrEmpty(((RadioButtonList)c).DataTextField))
                {
                    ((RadioButtonList)c).DataTextField = "Text";
                }
                if (string.IsNullOrEmpty(((RadioButtonList)c).DataValueField))
                {
                    ((RadioButtonList)c).DataValueField = "Value";
                }
                ((RadioButtonList)c).DataBind();
            }
            else if (c is CheckBoxList)
            {
                ((CheckBoxList)c).DataSource = value is DataTableStd ? ((DataTableStd)value).Value : value;
                if (string.IsNullOrEmpty(((CheckBoxList)c).DataTextField))
                {
                    ((CheckBoxList)c).DataTextField = "Text";
                }
                if (string.IsNullOrEmpty(((CheckBoxList)c).DataValueField))
                {
                    ((CheckBoxList)c).DataValueField = "Value";
                }
                ((CheckBoxList)c).DataBind();
            }
            else if (c is GridView)
            {
                ((GridView)c).DataSource = value is DataTableStd ? ((DataTableStd)value).Value : value;
                ((GridView)c).DataBind();
            }
        }

        protected void SetControlValue(Control c, object value)
        {
            if (value is string)
                value = value.ToString().Trim();

            if (c is TextBox)
            {
                ((TextBox)c).Text = ComFunc.nvl(value);
            }
            else if (c is Label)
            {
                ((Label)c).Text = ComFunc.nvl(value);
            }
            else if (c is DropDownList)
            {
                if (value != null)
                {
                    if (value is string)
                    {

                        string v = ComFunc.nvl(value);
                        if (((DropDownList)c).Items.FindByValue(v) != null)
                            ((DropDownList)c).SelectedValue = ComFunc.nvl(value);
                    }
                }
            }
            else if (c is RadioButton)
            {
                ((RadioButton)c).Checked = value != null ? bool.Parse(ComFunc.nvl(value)) : false;
            }
            else if (c is RadioButtonList)
            {
                ((RadioButtonList)c).SelectedValue = ComFunc.nvl(value);
            }
            else if (c is CalendarInput)
            {
                if (value is DateTime)
                {
                    ((CalendarInput)c).SelectedDate = DateTimeStd.ParseStd(value).Value;
                }
                else if (value is DateTimeStd)
                {
                    ((CalendarInput)c).SelectedDate = DateTimeStd.ParseStd(value);
                }
                else
                {
                    ((CalendarInput)c).Text = ComFunc.nvl(value);
                }
            }
            else if (c is CheckBox)
            {
                ((CheckBox)c).Checked = value != null ? bool.Parse(ComFunc.nvl(value)) : false;
            }
            else if (c is CheckBoxList)
            {
                string stempvalue = (string)value;
                if (stempvalue != null)
                {
                    List<string> ltemp = new List<string>();
                    string[] splitvalues = stempvalue.Split(',');
                    foreach (string v in splitvalues)
                    {
                        ltemp.Add(v);
                    }
                    foreach (ListItem item in ((CheckBoxList)c).Items)
                    {
                        if (ltemp.Contains(item.Value))
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
            else if (c is HiddenField)
            {
                ((HiddenField)c).Value = ComFunc.nvl(value);
            }
        }
    }
}
