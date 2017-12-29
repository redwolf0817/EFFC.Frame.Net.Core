using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Web.UI.Design;
using EFFC.Frame.Net.WebControlLib.Interfaces;

[assembly: TagPrefix("EFFC.Frame.Net.WebControlLib.Input", "uc1")]
namespace EFFC.Frame.Net.WebControlLib.Input
{
    [ValidationProperty("Text")]
    public class CalendarInput : WebControl, IPostBackEventHandler, IPostBackDataHandler, INamingContainer, IEditableTextControl,IWebDataControl
    {
        string _textWidth = "80";
        string _textHeight = "17";
        string _pickWidth = "16";
        string _pickHeight = "16";
        bool _visible = true;
        bool _disable = false;
        string _imgpath = "~/image/cal.gif";
        DateTime _seletedDate = DateTime.MinValue;
        string _clientstriptclick = "";
        private static readonly object EventTextChanged = new object();

        [Category("Action")]
        public event EventHandler TextChanged
        {
            add
            {
                base.Events.AddHandler(EventTextChanged, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventTextChanged, value);
            }
        }
        /// <summary>
        /// 前次设定的值
        /// </summary>
        private DateTime PreDateTime
        {
            get
            {
                if (ViewState[ClientID + "_PreSelectedDate"] == null)
                {
                    ViewState[ClientID + "_PreSelectedDate"] = SelectedDate;
                }
                return DateTimeStd.ParseStd(ViewState[ClientID + "_PreSelectedDate"]);
            }
            set
            {
                ViewState[ClientID + "_PreSelectedDate"] = value.ToString("yyyy/MM/dd");
            }
        }

        public override bool Enabled
        {
            get
            {
                return !this._disable;
            }
            set
            {
                this._disable = !value;
            }
        }

        [Browsable(true)]
        [Category("DataInfo")]
        [Description("输入框值设定，输入的必须为日期格式，否则设置为空")]
        [Bindable(true)]
        public string Text
        {
            get
            {
                return SelectedDate == DateTime.MinValue ? "" : SelectedDate.ToString("yyyy/MM/dd");
            }
            set
            {
                if (DateTimeStd.IsDateTime(value))
                {
                    SelectedDate =DateTimeStd.ParseStd(value);
                }
                else
                {
                    SelectedDate = DateTime.MinValue;
                }
            }
        }
        [Browsable(true)]
        [Category("BaseInfo")]
        [Description("输入框值的宽度")]
        [DefaultValue("80px")]
        public string TextWidth
        {
            get
            {
                return _textWidth;
            }
            set
            {
                _textWidth = value;
            }
        }

        [Browsable(true)]
        [Category("BaseInfo")]
        [Description("输入框值的高度")]
        public string TextHeight
        {
            get
            {
                return _textHeight;
            }
            set
            {
                _textHeight = value;
            }
        }
        [Browsable(true)]
        [Category("BaseInfo")]
        [Description("图形按钮的宽度")]
        public string ImgWidth
        {
            get
            {
                return _pickWidth;
            }
            set
            {
                _pickWidth = value;
            }
        }
        [Browsable(true)]
        [Category("BaseInfo")]
        [Description("图形按钮的高度")]
        public string ImgHeight
        {
            get
            {
                return _pickHeight;
            }
            set
            {
                _pickHeight = value;
            }
        }
        [Browsable(true)]
        [Category("BaseInfo")]
        [Description("控件Disable属性")]
        public bool Disabled
        {
            get
            {
                return _disable;
            }

            set
            {
                _disable = value;
            }
        }
        [Browsable(true)]
        [Category("BaseInfo")]
        [Description("图形按钮的图像路径")]
        [Editor(typeof(ImageUrlEditor), typeof(UITypeEditor))]
        public string ImgPath
        {
            get
            {
                return _imgpath;
            }
            set
            {
                _imgpath = value;
            }
        }
        [Browsable(true)]
        [Category("DataInfo")]
        [Description("选中的日期值")]
        [Bindable(true)]
        public DateTime SelectedDate
        {
            get
            {
                if (ViewState[ID + "_SelectedDate"] != null)
                    _seletedDate = DateTimeStd.ParseStd(ViewState[ID + "_SelectedDate"]);
                return _seletedDate;
            }
            set
            {
                ViewState[ID + "_SelectedDate"] = value;
                _seletedDate = value;
            }
        }
        [Browsable(true)]
        [Category("ActionInfo")]
        [Description("日期控件脚本调用事件,需要用到控件ID的就使用#ClientID#来表示,如果需要用到ImagebuttonID的则使用#ImageClientID#")]
        [Bindable(true)]
        public string OnClientClick
        {
            get
            {
                return _clientstriptclick;
            }
            set
            {
                _clientstriptclick = value;
            }
        }

        protected override object SaveViewState()
        {
            return base.SaveViewState();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresPostBack(this);
        }

        public override void RenderBeginTag(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write("");
        }

        public override void RenderEndTag(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write("");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new System.Web.UI.AttributeCollection Attributes
        {
            get
            {
                return base.Attributes;
            }
        }

        [Description("AutoPostBackControl_CausesValidation")]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Themeable(false)]
        public virtual bool CausesValidation
        {
            get
            {
                object obj2 = this.ViewState["CausesValidation"];
                if (obj2 == null)
                {
                    return true;
                }
                else
                {
                    return ((bool)obj2);
                }
            }
            set
            {
                this.ViewState["CausesValidation"] = value;
            }
        }

        [Description("PostBackControl_ValidationGroup")]
        [Category("Behavior")]
        [Themeable(false)]
        [DefaultValue("")]
        public virtual string ValidationGroup
        {
            get
            {
                string str = (string)this.ViewState["ValidationGroup"];
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["ValidationGroup"] = value;
            }
        }

        [Description("")]
        [Category("Behavior")]
        [Themeable(false)]
        [DefaultValue(false)]
        [Browsable(false)]
        public virtual bool IsTextChanged
        {
            get
            {
                object str = this.ViewState[this.ClientID + "_IsTextChanged"];
                if (str != null)
                {
                    return bool.Parse(str.ToString());
                }
                else
                {
                    return false;
                }
            }
            set
            {
                this.ViewState[this.ClientID + "_IsTextChanged"] = value;
            }
        }


        protected override void RenderContents(HtmlTextWriter writer)
        {
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (Visible)
            {
                if (this.Page != null)
                {
                    this.Page.VerifyRenderingInServerForm(this);
                }

                //输入框
                writer.AddAttribute(HtmlTextWriterAttribute.Id, base.ClientID);
                writer.AddAttribute(HtmlTextWriterAttribute.Name, base.UniqueID);
                writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "true");
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "Text");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, this._seletedDate == DateTime.MinValue ? "" : this._seletedDate.ToString("yyyy/MM/dd"));
                if (!this.Disabled)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Onclick, OnClientClick.Replace("#ClientID#", "this").Replace("#ImageClientID#", "document.getElementById('" + base.ClientID + "_Picker')"), false);
                    //string strclick = OnClientClick.Replace("#ClientID#", "this").Replace("#ImageClientID#", "document.getElementById('" + base.ClientID + "_Picker')");

                    if (!DesignMode)
                    {
                        string str4 = "";
                        if (base.HasAttributes)
                        {
                            str4 = base.Attributes["onchange"];
                            if (str4 != null)
                            {
                                str4 = ComFunc.EnsureEndWithSemiColon(str4);
                                base.Attributes.Remove("onchange");
                            }
                        }

                        EventHandler handler = (EventHandler)base.Events[EventTextChanged];
                        if (handler != null)
                        {
                            PostBackOptions options = new PostBackOptions(this, string.Empty);
                            if (this.CausesValidation)
                            {
                                options.PerformValidation = true;
                                options.ValidationGroup = this.ValidationGroup;
                            }
                            if (Page.Form != null)
                            {
                                options.AutoPostBack = true;
                            }
                            str4 = ComFunc.MergeScript(str4, Page.ClientScript.GetPostBackEventReference(options, true));
                            writer.AddAttribute("onpropertychange", str4);
                        }
                    }
                }
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.TextWidth.ToString() + "px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.TextHeight.ToString() + "px");

                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();
                //添加空格
                writer.Write("&nbsp;");
                //image
                writer.AddAttribute(HtmlTextWriterAttribute.Id, base.ClientID + "_Picker");
                writer.AddAttribute(HtmlTextWriterAttribute.Name, base.UniqueID + "$Picker");
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, OnClientClick.Replace("#ClientID#", "document.getElementById('" + base.ClientID + "')").Replace("#ImageClientID#", "document.getElementById('" + base.ClientID + "_Picker')"));

                if (DesignMode)
                {
                    //string directoryPath = ComFunc.GetProjectPathInDesignMode();
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, ResolveClientUrl(this.ImgPath));
                }
                else
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, ResolveClientUrl(this.ImgPath));
                }

                if (this.Disabled)
                    writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "true");

                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.ImgWidth.ToString() + "px");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.ImgHeight.ToString() + "px");

                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
        }

        #region IPostBackEventHandler 成員

        public void RaisePostBackEvent(string eventArgument)
        {
            if (IsTextChanged)
            {
                OnTextChanged(EventArgs.Empty);
                IsTextChanged = false;
            }
        }

        #endregion

        #region IPostBackDataHandler 成員

        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            string postbackstr = postCollection[base.UniqueID];
            DateTime d = string.IsNullOrEmpty(postbackstr) ? DateTime.MinValue : DateTimeStd.ParseStd(postbackstr).Value;
            this.SelectedDate = d;
            if (d.CompareTo(PreDateTime) == 0)
            {
                IsTextChanged = false;
                return false;
            }
            else
            {
                IsTextChanged = true;
                return true;
            }
        }

        public void RaisePostDataChangedEvent()
        {
            if (!this.Page.IsPostBackEventControlRegistered)
            {
                this.Page.AutoPostBackControl = this;
                if (this.CausesValidation)
                {
                    this.Page.Validate(this.ValidationGroup);
                }
            }

            //将当前的值设定成历史记录
            PreDateTime = this.SelectedDate;
        }

        #endregion

        protected virtual void OnTextChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)base.Events[EventTextChanged];
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        string _mapdatafield = "";
        [Browsable(true)]
        [Category("DataInfo")]
        [Description("映射栏位名称，用于转化成module")]
        [Bindable(false)]
         public string MapDataField
        {
            get
            {
                return _mapdatafield;
            }
            set
            {
                _mapdatafield = value;
            }
        }
    }
}
