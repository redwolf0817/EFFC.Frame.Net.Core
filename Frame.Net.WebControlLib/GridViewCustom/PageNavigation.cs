using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.WebControlLib.Interfaces;
using System.Text.RegularExpressions;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.WebControlLib.TypeConvert;
[assembly: TagPrefix("EFFC.Frame.Net.WebControlLib.GridViewCustom", "clgvc")]
namespace EFFC.Frame.Net.WebControlLib.GridViewCustom
{
    public class PageChangedEventArgs : EventArgs
    {
        private int _go_to = 0;
        private int _current_page = 0;
        private int _page_size = 0;
        private int _total_page = 0;
        private int _total_size = 0;
        private string _order_by = "";

        public int GotoPage
        {
            get { return _go_to; }
            set { _go_to = value; }
        }
        public int CurrentPage
        {
            get { return _current_page; }
            set { _current_page = value; }
        }
        public int PageSize
        {
            get { return _page_size; }
            set { _page_size = value; }
        }
        public int TotalPage
        {
            get { return _total_page; }
            set { _total_page = value; }
        }

        public int TotalSize
        {
            get { return _total_size; }
            set { _total_size = value; }
        }

        public string OrderBy
        {
            get { return _order_by; }
            set { _order_by = value; }
        }

        public PageChangedEventArgs(int toPage, int curPage, int pageSize)
        {
            GotoPage = toPage;
            CurrentPage = curPage;
            PageSize = pageSize;
        }
    }
    /// <summary>
    /// PageNavigation控件翻頁時間執行處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PageChangedEvent(object sender, PageChangedEventArgs e); 

    public class PageNavigation : Control, IPostBackEventHandler, INamingContainer
    {
        bool _enabled = true;
        string _width = "100%";
        string _height = "20px";
        string _firstText = "首頁";
        string _preText = "上一頁";
        string _nextText = "下一頁";
        string _lastText = "尾頁";
        string _totalsizeText = "共{0}筆記錄";
        string _currentshowText = "目前顯示{0}筆";
        string _currentshowPage = "第{0}頁";
        bool _iserror = false;
        int _page_show_size = 5;
        int _goto = 0;
        int _totolSize = 0;
        GridView _bindgv = null;
        string _linkactPn = null;
        string _listSizeOption = "每頁10筆,每頁25筆,每頁50筆,每頁100筆";
        string _orderby = "";
        string _cssClass = "";

        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定翻页器是否可操作")]
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定翻页器的宽度")]
        public string Width
        {
            get { return _width; }
            set { _width = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定翻页器的高度")]
        public string Height
        {
            get { return _height; }
            set { _height = value; }
        }
        [Category("PageProperties")]
        [Browsable(true)]
        [Description("设定翻页器的首页文字")]
        [Localizable(true)]
        public string FirstText
        {
            get { return _firstText; }
            set { _firstText = value; }
        }
        [Category("PageProperties")]
        [Browsable(true)]
        [Description("设定翻页器的上一页文字")]
        [Localizable(true)]
        public string PreText
        {
            get { return _preText; }
            set { _preText = value; }
        }
        [Category("PageProperties")]
        [Browsable(true)]
        [Description("设定翻页器的下一页文字")]
        [Localizable(true)]
        public string NextText
        {
            get { return _nextText; }
            set { _nextText = value; }
        }
        [Category("PageProperties")]
        [Browsable(true)]
        [Description("设定翻页器的尾页文字")]
        [Localizable(true)]
        public string LastText
        {
            get { return _lastText; }
            set { _lastText = value; }
        }

        [Category("PageProperties")]
        [Browsable(true)]
        [Description("设定翻页器的TotalSize的顯示文字")]
        [Localizable(true)]
        public string TotalSizeText
        {
            get { return _totalsizeText; }
            set { _totalsizeText = value; }
        }

        [Category("PageProperties")]
        [Browsable(true)]
        [Description("设定翻页器的顯示n~m筆的顯示文字")]
        [Localizable(true)]
        public string SizeShowPerPageText
        {
            get { return _currentshowText; }
            set { _currentshowText = value; }
        }

        [Category("PageProperties")]
        [Browsable(true)]
        [Description("设定翻页器的第N/N頁顯示文字")]
        [Localizable(true)]
        public string ShowCurrentPageText
        {
            get { return _currentshowPage; }
            set { _currentshowPage = value; }
        }

        [Category("DataProperties")]
        [Browsable(true)]
        [Description("设定需要绑定的GridView控件")]
        [Localizable(true)]
        public GridView GridViewControl
        {
            get { return _bindgv; }
            set { _bindgv = value; }
        }
        [Category("PageProperties")]
        [Browsable(true)]
        [Description("设定每頁顯示的筆數")]
        [Localizable(true)]
        public int ShowPageSize
        {
            get
            {
                if (ViewState[this.ClientID + "_PageSize"] == null)
                {
                    ViewState[this.ClientID + "_PageSize"] = 10;
                }
                return IntStd.ParseStd(ViewState[this.ClientID + "_PageSize"]);
            }
            set { ViewState[this.ClientID + "_PageSize"] = value; }
        }

        [Category("PageProperties")]
        [Browsable(true)]
        [Description("设定可以直接点选的页面数量")]
        [Localizable(true)]
        public string ShowPageSizeList
        {
            get { return _listSizeOption; }
            set { _listSizeOption = value; }
        }

        [Category("LinkAct")]
        [Browsable(true)]
        [Description("设定需要联动的翻页器，如果不需要则可以设空")]
        [Localizable(false)]
        [TypeConverter(typeof(PageNavigationControlConverter))]
        public string LinkActPageNavigation
        {
            get { return _linkactPn; }
            set { _linkactPn = value; }
        }

        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定Css样式")]
        [Localizable(false)]
        [TypeConverter(typeof(PageNavigationControlConverter))]
        public string CssClass
        {
            get { return _cssClass; }
            set { _cssClass = value; }
        }

        /// <summary>
        /// 總頁數
        /// </summary>
        [Browsable(false)]
        public int TotalPage
        {
            get { return IntStd.ParseStd(ViewState[UniqueID + "$TotalPage"]); }
        }
        /// <summary>
        /// 寫入總頁數
        /// </summary>
        /// <param name="o"></param>
        protected void SetTotalPage(int o)
        {
            ViewState[UniqueID + "$TotalPage"] = o;
        }

        /// <summary>
        /// 當前頁
        /// </summary>
        [Browsable(false)]
        public int CurrentPage
        {
            get { return IntStd.ParseStd(ViewState[UniqueID + "$CurrentPage"]); }
        }

        public event PageChangedEvent PageChanged;


        /// <summary>
        /// 寫入當前頁
        /// </summary>
        /// <param name="o"></param>
        protected void SetCurrentPage(int o)
        {
            ViewState[UniqueID + "$CurrentPage"] = o;
        }

        /// <summary>
        /// 写入總資料筆數
        /// </summary>
        /// <param name="o"></param>
        protected void SetTotalSize(int o)
        {
            _totolSize = o;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!DesignMode)
            {
                SetCurrentPage(0);
                SetTotalPage(0);
                int initPageSize = 0;
                string[] strsizes = ShowPageSizeList.Split(',');
                Regex rs = new Regex(@"\d+");
                if (strsizes.Length > 0)
                {
                    initPageSize = IntStd.ParseStd(rs.Match(strsizes[0])).Value;
                }
                else
                {
                    initPageSize = 10;
                }
                ShowPageSize = initPageSize;
            }
        }

        

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            //base.RenderControl(writer);
            Render(writer);
        }

        private bool _hasData = false;

        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
            {

                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width);
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.Height);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
                writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write(string.Format(TotalSizeText, _totolSize));
                writer.RenderEndTag();
                writer.Write("&nbsp;");

                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write(string.Format(SizeShowPerPageText, "1~5"));
                writer.RenderEndTag();
                writer.Write("&nbsp;");

                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100px");
                writer.RenderBeginTag(HtmlTextWriterTag.Select);
                writer.RenderBeginTag(HtmlTextWriterTag.Option);
                writer.Write("每頁10筆");
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.Write("&nbsp;");

                writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(PreText);
                writer.RenderEndTag();
                writer.Write("&nbsp;");

                writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(NextText);
                writer.RenderEndTag();
                writer.Write("&nbsp;");

                for (int i = 1; i <= 10; i++)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write("[" + i.ToString() + "]");
                    writer.RenderEndTag();

                    writer.Write("&nbsp;");
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(FirstText);
                writer.RenderEndTag();
                writer.Write("&nbsp;");

                writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(LastText);
                writer.RenderEndTag();
                writer.Write("&nbsp;");

                //writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height);
                //writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "50px");
                //writer.AddAttribute(HtmlTextWriterAttribute.Value, "");
                //writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                //writer.RenderBeginTag(HtmlTextWriterTag.Input);
                //writer.RenderEndTag();
                //writer.Write("&nbsp;");

                //writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height);
                //writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "50px");
                //writer.AddAttribute(HtmlTextWriterAttribute.Value, "GO");
                //writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
                //writer.RenderBeginTag(HtmlTextWriterTag.Input);
                //writer.RenderEndTag();

                writer.RenderEndTag();
            }
            else
            {
                if (Visible && _hasData)
                {

                    writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width);
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.Height);
                    if (CssClass != "")
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
                    }
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    //出錯處理
                    if (_iserror)
                    {
                        writer.AddStyleAttribute(HtmlTextWriterStyle.FontStyle, "font-color:red;");
                        writer.RenderBeginTag(HtmlTextWriterTag.P);
                        writer.RenderEndTag();
                    }
                    else
                    {
                        //添加__dopoastback脚本
                        writer.Write("<a style='display:none' onclick=" + Page.ClientScript.GetPostBackEventReference(this,"") + "></a>");

                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        writer.Write(string.Format(TotalSizeText, _totolSize));
                        writer.RenderEndTag();
                        writer.Write("&nbsp;");

                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        writer.Write(string.Format(ShowCurrentPageText, CurrentPage + "/" + TotalPage));
                        writer.RenderEndTag();
                        writer.Write("&nbsp;");

                        //當資料總數大於一頁設定數的時候
                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        //本頁起始資料序號
                        int start = (CurrentPage - 1) * ShowPageSize + 1;
                        //本頁結束資料序號
                        int end = CurrentPage * ShowPageSize;
                        end = end <= _totolSize ? end : _totolSize;

                        writer.Write(string.Format(SizeShowPerPageText, start + "~" + end));
                        writer.RenderEndTag();
                        writer.Write("&nbsp;");

                        writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100px");
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_PageSize");
                        writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID + "$PageSize");
                        writer.AddAttribute(HtmlTextWriterAttribute.Onchange,  "javascript:__doPostBack('" + this.UniqueID + "','PageSize='+document.getElementById('" + this.ClientID + "_PageSize').value)");
                        writer.RenderBeginTag(HtmlTextWriterTag.Select);
                        string[] strsizes = ShowPageSizeList.Split(',');
                        Regex rs = new Regex(@"\d+");
                        foreach (string s in strsizes)
                        {
                            string sizevalue = rs.Match(s).Value;
                            writer.AddAttribute(HtmlTextWriterAttribute.Value, sizevalue);
                            if (sizevalue == ShowPageSize.ToString())
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Selected, "true");
                            }
                            writer.RenderBeginTag(HtmlTextWriterTag.Option);
                            writer.Write(s);
                            writer.RenderEndTag();
                        }
                        writer.RenderEndTag();
                        writer.Write("&nbsp;");

                        if (_totolSize > ShowPageSize)
                        {
                            

                            //Add By TangbaoPeng On 2112/12/14
                            int prevPage = this.CurrentPage - 1, nextPage = this.CurrentPage + 1;

                            if (prevPage < 1)
                            {
                                ////首頁
                                //writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:");
                                //writer.RenderBeginTag(HtmlTextWriterTag.A);
                                //writer.Write(FirstText);
                                //writer.RenderEndTag();
                                //writer.Write("&nbsp;&nbsp;");
                                ////上一頁
                                //writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:");
                                //writer.RenderBeginTag(HtmlTextWriterTag.A);
                                //writer.Write(PreText);
                                //writer.RenderEndTag();
                                //writer.Write("&nbsp;&nbsp;");
                            }
                            else
                            {
                                //首頁
                                writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, 1 + ""));
                                writer.RenderBeginTag(HtmlTextWriterTag.A);
                                writer.Write(FirstText);
                                writer.RenderEndTag();
                                writer.Write("&nbsp;");
                                //上一頁
                                writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, prevPage + ""));
                                writer.RenderBeginTag(HtmlTextWriterTag.A);
                                writer.Write(PreText);
                                writer.RenderEndTag();
                                writer.Write("&nbsp;");
                            }

                            if (CurrentPage != 1)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, 1 + ""));
                                writer.RenderBeginTag(HtmlTextWriterTag.A);
                                writer.Write("[1]");
                                writer.RenderEndTag();
                                //writer.Write("&nbsp;");
                            }

                            if (CurrentPage >= 5)
                            {
                                writer.Write("<span>...</span>");
                            }

                            int startPage=0;
                            int endPage = 0;

                            startPage = CurrentPage - 2;

                            if (TotalPage > CurrentPage + 2)
                            {
                                endPage = CurrentPage + 2;

                                //if (TotalPage>4&&CurrentPage <= 4)
                                //    endPage = 5;
                            }
                            else
                            {
                                endPage = TotalPage;
                            }

                            //if (CurrentPage > TotalPage - 4)
                            //    startPage = TotalPage - 4;


                            for (int i = startPage; i <= endPage; i++)
                            {
                                if (i > 0)
                                {
                                    if (i == CurrentPage)
                                    {
                                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                                        writer.Write("[<b>" + i.ToString() + "</b>]");
                                        writer.RenderEndTag();
                                    }
                                    else
                                    {
                                        if (i != 1 && i != TotalPage)
                                        {
                                            writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, i + ""));
                                            writer.RenderBeginTag(HtmlTextWriterTag.A);
                                            writer.Write("[" + i.ToString() + "]");
                                            writer.RenderEndTag();
                                        }
                                    }
                                   // writer.Write("&nbsp;");
                                }
                            }

                            if (CurrentPage + 3 < TotalPage)
                            {
                                writer.Write("<span>...</span>");
                            }

                            if (CurrentPage != TotalPage)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, TotalPage + ""));
                                writer.RenderBeginTag(HtmlTextWriterTag.A);
                                writer.Write("[" + TotalPage + "]");
                                writer.RenderEndTag();
                               // writer.Write("&nbsp;");
                            }

                            if (nextPage > TotalPage)
                            {
                                ////下一頁
                                //writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:");
                                //writer.RenderBeginTag(HtmlTextWriterTag.A);
                                //writer.Write(NextText);
                                //writer.RenderEndTag();
                                //writer.Write("&nbsp;&nbsp;");
                                ////尾頁
                                //writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:");
                                //writer.RenderBeginTag(HtmlTextWriterTag.A);
                                //writer.Write(LastText);
                                //writer.RenderEndTag();
                                //writer.Write("&nbsp;&nbsp;");
                            }
                            else
                            {
                                //下一頁
                                writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, nextPage + ""));
                                writer.RenderBeginTag(HtmlTextWriterTag.A);
                                writer.Write(NextText);
                                writer.RenderEndTag();
                                writer.Write("&nbsp;");
                                //尾頁
                                writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, TotalPage + ""));
                                writer.RenderBeginTag(HtmlTextWriterTag.A);
                                writer.Write(LastText);
                                writer.RenderEndTag();

                            }
                        }
                        //End Add By TangbaoPeng On 2112/12/14




                        //    //如果當前頁不為第一頁
                        //    if (CurrentPage > 1)
                        //    {
                        //        writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, CurrentPage - 1 + ""));
                        //        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        //        writer.Write(PreText);
                        //        writer.RenderEndTag();
                        //        writer.Write("&nbsp;&nbsp;");
                        //    }
                        //    if (CurrentPage < TotalPage)
                        //    {

                        //        writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, CurrentPage + 1 + ""));
                        //        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        //        writer.Write(NextText);
                        //        writer.RenderEndTag();
                        //        writer.Write("&nbsp;&nbsp;");
                        //    }

                        //    //可直接點擊的頁面的起始頁數
                        //    //int startPage = CurrentPage > ShowPageSize ? CurrentPage - ShowPageSize + 1 : 1;

                        //    //int endPage = TotalPage > ShowPageSize ? (startPage + ShowPageSize - 1) : TotalPage;

                        //    int tmp = _page_show_size / 2;
                        //    int startPage = 0;
                        //    if (CurrentPage > TotalPage - tmp + 1)
                        //    {
                        //        startPage = TotalPage - _page_show_size + 1 > 0 ? TotalPage - _page_show_size + 1 : 1;
                        //    }
                        //    else
                        //    {
                        //        startPage = CurrentPage - tmp > 0 ? CurrentPage - tmp : 1;
                        //    }


                        //    int endPage = 0;
                        //    if (tmp * 2 == _page_show_size)
                        //    {
                        //        if (TotalPage > CurrentPage + (tmp - 1))
                        //        {
                        //            if (_page_show_size > CurrentPage + (tmp - 1))
                        //            {
                        //                endPage = _page_show_size;
                        //            }
                        //            else
                        //            {
                        //                endPage = CurrentPage + (tmp - 1);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            endPage = TotalPage;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (TotalPage > CurrentPage + tmp)
                        //        {
                        //            if (_page_show_size > CurrentPage + tmp)
                        //            {
                        //                endPage = _page_show_size;
                        //            }
                        //            else
                        //            {
                        //                endPage = CurrentPage + tmp;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            endPage = TotalPage;
                        //        }
                        //    }


                        //    //總顯示第一頁
                        //    if (startPage > 1)
                        //    {

                        //        writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, 1 + ""));
                        //        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        //        writer.Write("[1]");
                        //        writer.RenderEndTag();
                        //        writer.Write("&nbsp;");
                        //    }
                        //    //與第一頁數相差2個以上，則加上……
                        //    if (startPage > 2)
                        //    {
                        //        writer.Write("......");
                        //    }

                        //    for (int i = startPage; i <= endPage; i++)
                        //    {

                        //        //如果為當前頁
                        //        if (i == CurrentPage)
                        //        {
                        //            writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        //            writer.Write("[<b>" + i.ToString() + "</b>]");
                        //            writer.RenderEndTag();

                        //        }
                        //        else
                        //        {
                        //            writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, i + ""));
                        //            writer.RenderBeginTag(HtmlTextWriterTag.A);
                        //            writer.Write("[" + i.ToString() + "]");
                        //            writer.RenderEndTag();

                        //        }

                        //        writer.Write("&nbsp;");
                        //    }
                        //    //與最後頁數相差2個以上，則加上……
                        //    if (endPage < TotalPage - 1)
                        //    {
                        //        writer.Write("......");
                        //    }
                        //    //總顯示最後一頁
                        //    if (endPage < TotalPage )
                        //    {

                        //        writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, TotalPage + ""));
                        //        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        //        writer.Write("["+TotalPage+"]");
                        //        writer.RenderEndTag();
                        //        writer.Write("&nbsp;");
                        //    }

                        //    //如果當前頁不為第一頁
                        //    if (CurrentPage > 1)
                        //    {
                        //        writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, "1"));
                        //        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        //        writer.Write(FirstText);
                        //        writer.RenderEndTag();
                        //        writer.Write("&nbsp;");
                        //    }

                        //    if (CurrentPage < TotalPage)
                        //    {
                        //        writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + Page.ClientScript.GetPostBackEventReference(this, TotalPage + ""));
                        //        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        //        writer.Write(LastText);
                        //        writer.RenderEndTag();
                        //        writer.Write("&nbsp;");
                        //    }
                        //}

                        //writer.AddStyleAttribute(HtmlTextWriterStyle.Height,"15px");
                        //writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "50px");
                        //writer.AddAttribute(HtmlTextWriterAttribute.Value, "");
                        //writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                        //writer.AddAttribute(HtmlTextWriterAttribute.Id,ClientID+"_GoTo");
                        //writer.AddAttribute(HtmlTextWriterAttribute.Name,UniqueID+"$GoTo");
                        //writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        //writer.RenderEndTag();
                        //writer.Write("&nbsp;");

                        //writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height);
                        //writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "50px");
                        //writer.AddAttribute(HtmlTextWriterAttribute.Value, "GO");
                        //writer.AddAttribute(HtmlTextWriterAttribute.Type, "submit");
                        //writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "javascript:__doPostBack('" + this.UniqueID + "',document.getElementById('" + this.ClientID + "_GoTo').value)");
                        //writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        //writer.RenderEndTag();

                        writer.RenderEndTag();
                    }
                }
            }
        }

        public PageChangedEventArgs GetPageChangedEventArgs(int gotoPage)
        {
            PageChangedEventArgs e = new PageChangedEventArgs(gotoPage, CurrentPage, ShowPageSize);
            e.OrderBy = _orderby;
            return e;
        }

        protected virtual void OnPageChanged(int gotoPage)
        {
            try
            {
                PageChangedEventArgs e = GetPageChangedEventArgs(gotoPage);
                e.OrderBy = _orderby;
                if (this.PageChanged != null)
                {
                    PageChanged(this, e);
                    SetTotalPage(e.TotalPage);
                    SetCurrentPage(e.CurrentPage);
                    SetTotalSize(e.TotalSize);
                 
                    //如果有需要聯動的翻頁器
                    if (LinkActPageNavigation != null)
                    {
                        PageNavigation _pn = this.GetPNByID(LinkActPageNavigation);
                        _pn.SetTotalPage(e.TotalPage);
                        _pn.SetCurrentPage(e.CurrentPage);
                        _pn.SetTotalSize(e.TotalSize);
                    }
                }
                if (TotalPage <= 0)
                    _hasData = false;
                else
                    _hasData = true;

                //如果有需要聯動的翻頁器
                if (LinkActPageNavigation != null)
                {
                    PageNavigation _pn = this.GetPNByID(LinkActPageNavigation);
                    _pn._hasData = _hasData;
                }
            }
            catch (Exception ex)
            {                
                _iserror = true;
                throw ex;
            }
        }
        /// <summary>
        /// 根據ID查找控件
        /// 只從本控件往上找，一直到Page層，如果還沒找到就返回空
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private PageNavigation GetPNByID(string id)
        {
            Control container = this.Parent;
            while (true)
            {
                if (container.FindControl(id) != null)
                {
                    return (PageNavigation)container.FindControl(id);
                }

                if (container is Page)
                {
                    return null;
                }
                else
                {
                    container = container.Parent;
                }
            }
        }


        #region IPostBackEventHandler Members

        public void RaisePostBackEvent(string eventArgument)
        {
            int gotopage = IntStd.IsInt(eventArgument) ? IntStd.ParseStd(eventArgument).Value : CurrentPage;
            
            if (eventArgument.StartsWith("PageSize="))
            {
                ShowPageSize = IntStd.ParseStd(eventArgument.Replace("PageSize=", "")).Value;
            }
            OnPageChanged(gotopage);
        }

        #endregion

        public void StartShowPage()
        {
            OnPageChanged(1);
        }

        public void Refresh()
        {
            OnPageChanged(CurrentPage);
        }

        public void SortBy(string orderby)
        {
            _orderby = orderby;
            OnPageChanged(CurrentPage);
        }

        public void StartShowPage(int toPage)
        {
            OnPageChanged(toPage);
        }
    }
}
