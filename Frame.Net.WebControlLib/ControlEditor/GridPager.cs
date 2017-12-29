using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;


namespace EFFC.Frame.Net.WebControlLib
{
    /// <summary>
    /// Expend Pager
    /// </summary>
    [DefaultProperty("PageSize")]
    [DefaultEvent("PageChanged")]
    [ParseChildren(false)]
    [PersistChildren(false)]
    [Description("Pager Control")]
    [Designer(typeof(PagerDesigner))]
    [ToolboxData("<{0}:GridPager runat=server></{0}:GridPager>")]
    public class GridPager : Panel, INamingContainer, IPostBackEventHandler, IPostBackDataHandler
    {
        private string cssClassName = "divPager";
        private string urlPageIndexName = "page";
        private bool urlPaging = false;
        private string inputPageIndex;
        private string inputPageSize;
        private string currentUrl = null;
        private NameValueCollection urlParams = null;
        private const string NO_RESULTS = "No data";

        #region Properties

        #region Navigation Buttons

        /// <summary>
        /// ShowNavigationToolTip
        /// </summary>
        [Browsable(true),
        Category("NavigationTool"),
        DefaultValue(true),
        Description("Tool tip of navigation button")]
        public bool ShowNavigationToolTip
        {
            get
            {
                object obj = ViewState["ShowNavigationToolTip"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["ShowNavigationToolTip"] = value;
            }
        }

        /// <summary>
        /// NavigationToolTipTextFormatString
        /// </summary>
        [Browsable(true),
        Category("NavigationTool"),
        DefaultValue("Go to page {0}"),
        Description("")]
        public string NavigationToolTipTextFormatString
        {
            get
            {
                object obj = ViewState["NavigationToolTipTextFormatString"];
                return (obj == null) ? "Go to page {0}" : (string)obj;
            }
            set
            {
                string tip = value;
                if (tip.Trim().Length < 1 && tip.IndexOf("{0}") < 0)
                    tip = "{0}";
                ViewState["NavigationToolTipTextFormatString"] = tip;
            }
        }

        /// <summary>
        /// Chinese index
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Browsable(true),
       Category("NavigationTool"),
        DefaultValue(false),
        Description("")]
        public bool ChinesePageIndex
        {
            get
            {
                object obj = ViewState["ChinesePageIndex"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["ChinesePageIndex"] = value;
            }
        }

        /// <summary>
        /// Format of Index
        /// </summary>
        /// <remarks>
        /// if you set this to [{0}],the index would show like this [1] [2] [3] ... if set to -{0}- it may show -1- -2- -3- ...
        /// </remarks>
        [Browsable(true),
        DefaultValue(""),
       Category("NavigationTool"),
        Description("Format the index number")]
        public string NumericButtonTextFormatString
        {
            get
            {
                object obj = ViewState["NumericButtonTextFormatString"];
                return (obj == null) ? String.Empty : (string)obj;
            }
            set
            {
                ViewState["NumericButtonTextFormatString"] = value;
            }
        }

        /// <summary>
        /// Paging button type
        /// </summary>
        [Browsable(true),
        DefaultValue(PagingButtonType.Text),
           Category("NavigationTool"),
        Description("Paging button type")]
        public PagingButtonType PagingButtonType
        {
            get
            {
                object obj = ViewState["PagingButtonType"];
                return (obj == null) ? PagingButtonType.Text : (PagingButtonType)obj;
            }
            set
            {
                ViewState["PagingButtonType"] = value;
            }
        }

        /// <summary>
        /// It will only be valid when PagingButtonType is set to Image
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Browsable(true),
        DefaultValue(PagingButtonType.Text),
        Category("NavigationTool"),
        Description("NumericButtonType of page index")]
        public PagingButtonType NumericButtonType
        {
            get
            {
                object obj = ViewState["NumericButtonType"];
                return (obj == null) ? PagingButtonType : (PagingButtonType)obj;
            }
            set
            {
                ViewState["NumericButtonType"] = value;
            }
        }

        /// <summary>
        /// It will only be valid when PagingButtonType is set to Image
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Browsable(true),
        Category("NavigationTool"),
        DefaultValue(PagingButtonType.Text),
        Description("Button type for first,previous,next,last")]
        public PagingButtonType NavigationButtonType
        {
            get
            {
                object obj = ViewState["NavigationButtonType"];
                return (obj == null) ? PagingButtonType : (PagingButtonType)obj;
            }
            set
            {
                ViewState["NavigationButtonType"] = value;
            }
        }

        /// <summary>
        /// more(...)
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Browsable(true),
        Category("NavigationTool"),
        DefaultValue(PagingButtonType.Text),
        Description("Button type of more(...)")]
        public PagingButtonType MoreButtonType
        {
            get
            {
                object obj = ViewState["MoreButtonType"];
                return (obj == null) ? PagingButtonType : (PagingButtonType)obj;
            }
            set
            {
                ViewState["MoreButtonType"] = value;
            }
        }

        /// <summary>
        /// Get or set gap among paging buttons
        /// </summary>
        [Browsable(true),
        Category("NavigationTool"),
        DefaultValue(typeof(Unit), "5px"),
        Description("Gap among paging buttons")]
        public Unit PagingButtonSpacing
        {
            get
            {
                object obj = ViewState["PagingButtonSpacing"];
                return (obj == null) ? Unit.Pixel(5) : (Unit.Parse(obj.ToString()));
            }
            set
            {
                ViewState["PagingButtonSpacing"] = value;
            }
        }

        /// <summary>
        /// Show First and Last
        /// </summary>
        [Browsable(true),
        Description("Show first and last"),
        Category("NavigationTool"),
        DefaultValue(true)]
        public bool ShowFirstLast
        {
            get
            {
                object obj = ViewState["ShowFirstLast"];
                return (obj == null) ? true : (bool)obj;
            }
            set { ViewState["ShowFirstLast"] = value; }
        }

        /// <summary>
        /// Show previous and next
        /// </summary>
        [Browsable(true),
        Description("Show previous and next"),
        Category("NavigationTool"),
        DefaultValue(true)]
        public bool ShowPrevNext
        {
            get
            {
                object obj = ViewState["ShowPrevNext"];
                return (obj == null) ? true : (bool)obj;
            }
            set { ViewState["ShowPrevNext"] = value; }
        }

        /// <summary>
        /// Show page index
        /// </summary>
        [Browsable(true),
        Description("Show page index"),
        Category("NavigationTool"),
        DefaultValue(false)]
        public bool ShowPageIndex
        {
            get
            {
                object obj = ViewState["ShowPageIndex"];
                return (obj == null) ? false : (bool)obj;
            }
            set { ViewState["ShowPageIndex"] = value; }
        }

        /// <summary>
        /// The text of first button
        /// </summary>
        [Browsable(true),
        Description("Text of first button"),
        Category("NavigationTool"),
       DefaultValue("")]
        public string FirstPageText
        {
            get
            {
                object obj = ViewState["FirstPageText"];
                return (obj == null) ? "" : (string)obj;
            }
            set { ViewState["FirstPageText"] = value; }
        }

        /// <summary>
        /// The text of previous button
        /// </summary>
        [Browsable(true),
        Description("Text of previous button"),
        Category("NavigationTool"),
       DefaultValue("Last page")]
        public string PrevPageText
        {
            get
            {
                object obj = ViewState["PrevPageText"];
                return (obj == null) ? "<Last Page" : (string)obj;
            }
            set { ViewState["PrevPageText"] = value; }
        }

        /// <summary>
        /// The text of next button
        /// </summary>
        [Browsable(true),
        Description("Text of next button"),
        Category("NavigationTool"),
       DefaultValue("")]
        public string NextPageText
        {
            get
            {
                object obj = ViewState["NextPageText"];
                return (obj == null) ? "Next Page>" : (string)obj;
            }
            set { ViewState["NextPageText"] = value; }
        }

        /// <summary>
        /// The text of last button
        /// </summary>
        [Browsable(true),
        Description("Text of last button"),
        Category("NavigationTool"),
        DefaultValue("Ю")]
        public string LastPageText
        {
            get
            {
                object obj = ViewState["LastPageText"];
                return (obj == null) ? "Ю" : (string)obj;
            }
            set { ViewState["LastPageText"] = value; }
        }

        /// <summary>
        /// Navigation button counts
        /// </summary>
        [Browsable(true),
        Description("Navigation button counts"),
        Category("NavigationTool"),
        DefaultValue(10)]
        public int NumericButtonCount
        {
            get
            {
                object obj = ViewState["NumericButtonCount"];
                return (obj == null) ? 10 : (int)obj;
            }
            set { ViewState["NumericButtonCount"] = value; }
        }

        /// <summary>
        /// Show disabled buttons
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Browsable(true),
        Category("NavigationTool"),
        Description("Show disabled buttons"),
        DefaultValue(true)]
        public bool ShowDisabledButtons
        {
            get
            {
                object obj = ViewState["ShowDisabledButtons"];
                return (obj == null) ? true : (bool)obj;
            }
            set
            {
                ViewState["ShowDisabledButtons"] = value;
            }
        }

        #endregion

        #region Image Buttons

        /// <summary>
        /// the Image's URL of Image button
        /// </summary>
        [Browsable(true),
       Category("Image Button"),
       Description("the Image's URL of Image button"),
        DefaultValue(null)]
        public string ImagePath
        {
            get
            {
                string imgPath = (string)ViewState["ImagePath"];
                if (imgPath != null)
                    imgPath = this.ResolveUrl(imgPath);
                return imgPath;
            }
            set
            {
                string imgPath = value.Trim().Replace("\\", "/");
                ViewState["ImagePath"] = (imgPath.EndsWith("/")) ? imgPath : imgPath + "/";
            }
        }

        /// <summary>
        /// Image button's filename extension 
        /// </summary>
        [Browsable(true),
        Category("Image Button"),
        DefaultValue(".gif"),
        Description("Image button's filename extension ")]
        public string ButtonImageExtension
        {
            get
            {
                object obj = ViewState["ButtonImageExtension"];
                return (obj == null) ? ".gif" : (string)obj;
            }
            set
            {
                string ext = value.Trim();
                ViewState["ButtonImageExtension"] = (ext.StartsWith(".")) ? ext : ("." + ext);
            }
        }

        /// <summary>
        /// Get or set type of Image button's filename extension  
        /// </summary>
        /// <remarks><note>note:</note>this isn't filename extensio,is type of filename extension,example:1f.gif,1n.gif,f and n is type of filename extension</remarks>
        [Browsable(true),
        DefaultValue(null),
        Category("Image Button"),
        Description("this isn't filename extensio,is type of filename extension,example:1f.gif,1n.gif,f and n is type of filename extension")]
        public string ButtonImageNameExtension
        {
            get
            {
                return (string)ViewState["ButtonImageNameExtension"];
            }
            set
            {
                ViewState["ButtonImageNameExtension"] = value;
            }
        }

        /// <summary>
        /// Get or set index of current page can be used image button
        /// </summary>
        /// <remarks>
        /// When<see cref="PagingButtonType"/>is images index of current page or index of other page can be used image button else<see cref="ButtonImageNameExtension"/>used same image button
        /// </remarks>
        [Browsable(true),
        DefaultValue(null),
        Category("Image Button"),
        Description("")]
        public string CpiButtonImageNameExtension
        {
            get
            {
                object obj = ViewState["CpiButtonImageNameExtension"];
                return (obj == null) ? ButtonImageNameExtension : (string)obj;
            }
            set
            {
                ViewState["CpiButtonImageNameExtension"] = value;
            }
        }

        /// <summary>
        /// Get or set disabled NavigationTool can be used image's extensio
        /// </summary>
        /// <remarks>
        /// When<see cref="PagingButtonType"/>Set Image can be allow set disable <see cref="ButtonImageNameExtension"/>
        /// </remarks>
        [Browsable(true),
        DefaultValue(null),
        Category("Image Button"),
        Description("")]
        public string DisabledButtonImageNameExtension
        {
            get
            {
                object obj = ViewState["DisabledButtonImageNameExtension"];
                return (obj == null) ? ButtonImageNameExtension : (string)obj;
            }
            set
            {
                ViewState["DisabledButtonImageNameExtension"] = value;
            }
        }

        [Browsable(true),
        Description(""),
        DefaultValue(ImageAlign.Baseline),
        Category("Image button")]
        public ImageAlign ButtonImageAlign
        {
            get
            {
                object obj = ViewState["ButtonImageAlign"];
                return (obj == null) ? ImageAlign.Baseline : (ImageAlign)obj;
            }
            set { ViewState["ButtonImageAlign"] = value; }
        }


        #endregion

        #region Paging

        /// <summary>
        /// Start using url to pass paging info
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        ///<%@Register TagPrefix="Webdiyer" Namespace="Wuqi.Webdiyer" Assembly="aspnetpager"%>
        ///<%@Import Namespace="System.Data.OleDb"%>
        ///<%@ Import Namespace="System.Data"%>
        ///<%@ Page Language="C#" debug=true%>
        ///<HTML>
        ///	<HEAD>
        ///		<TITLE>Welcome to Webdiyer.com </TITLE>
        ///		<script runat="server">
        ///		OleDbConnection conn;
        ///		OleDbCommand cmd;
        ///		void Page_Load(object src,EventArgs e){
        ///		conn=new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+Server.MapPath("access/aspnetpager.mdb"));
        ///		if(!Page.IsPostBack){
        ///		cmd=new OleDbCommand("select count(newsid) from wqnews",conn);
        ///		conn.Open();
        ///		pager.RecordCount=(int)cmd.ExecuteScalar();
        ///		conn.Close();
        ///		BindData();
        ///		}
        ///		}
        ///
        ///		void BindData(){
        ///		cmd=new OleDbCommand("select newsid,heading,source,addtime from wqnews order by addtime desc",conn);
        ///		OleDbDataAdapter adapter=new OleDbDataAdapter(cmd);
        ///		DataSet ds=new DataSet();
        ///		adapter.Fill(ds,pager.PageSize*(pager.CurrentPageIndex-1),pager.PageSize,"news");
        ///		dg.DataSource=ds.Tables["news"];
        ///		dg.DataBind();
        ///		}
        ///
        ///		void ChangePage(object src,PageChangedEventArgs e){
        ///		pager.CurrentPageIndex=e.NewPageIndex;
        ///		BindData();
        ///		}
        ///
        ///		</script>
        ///		<meta http-equiv="Content-Language" content="zh-cn">
        ///		<meta http-equiv="content-type" content="text/html;charset=gb2312">
        ///		<META NAME="Generator" CONTENT="EditPlus">
        ///		<META NAME="Author" CONTENT="Webdiyer(yhaili@21cn.com)">
        ///	</HEAD>
        ///	<body>
        ///		<form runat="server" ID="Form1">
        ///			<h2 align="center">AspNetPagerだボㄒ</h2>
        ///			<asp:DataGrid id="dg" runat="server" 
        ///			Width="760" CellPadding="4" Align="center" />
        ///			
        ///			<Webdiyer:AspNetPager runat="server" id="pager" 
        ///			OnPageChanged="ChangePage" 
        ///			HorizontalAlign="center" 
        ///			style="MARGIN-TOP:10px;FONT-SIZE:16px" 
        ///			PageSize="8" 
        ///			ShowInputBox="always" 
        ///			SubmitButtonStyle="border:1px solid #000066;height:20px;width:30px" 
        ///			InputBoxStyle="border:1px #0000FF solid;text-align:center" 
        ///			SubmitButtonText="锣" 
        ///			UrlPaging="true" 
        ///			UrlPageIndexName="pageindex" />
        ///		</form>
        ///	</body>
        ///</HTML>
        /// ]]></code>
        /// </example>
        [Browsable(true),
        Category("Paging"),
        DefaultValue(false),
       Description("Start using url to pass paging info")]
        public bool UrlPaging
        {
            get
            {
                return urlPaging;
            }
            set
            {
                urlPaging = value;
            }
        }

        /// <summary>
        /// The parameter name for URL
        /// </summary>
        [Browsable(true),
        DefaultValue("page"),
        Category("Paging"),
        Description("The parameter name for URL")]
        public string UrlPageIndexName
        {
            get { return urlPageIndexName; }
            set { urlPageIndexName = value; }
        }

        /// <summary>
        /// Current page index
        /// </summary>
        [ReadOnly(true),
        Browsable(false),
        Description("Current page index"),
        Category("Paging"),
        DefaultValue(1),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CurrentPageIndex
        {
            get
            {
                object cpage = ViewState["CurrentPageIndex"];
                int pindex = (cpage == null) ? 1 : (int)cpage;
                if (pindex > PageCount && PageCount > 0)
                    return PageCount;
                else if (pindex < 1)
                    return 1;
                return pindex;
            }
            set
            {
                int cpage = value;
                if (cpage < 1)
                    cpage = 1;
                //else if(cpage>this.PageCount)
                //    cpage=this.PageCount;
                ViewState["CurrentPageIndex"] = cpage;
            }
        }

        /// <summary>
        /// Record count
        /// </summary>
        /// <example>
        /// ボㄒ陪ボ絪祘よΑ盢眖Sql粂癘魁羆计结倒赣妮┦
        /// <p>
        /// <code><![CDATA[
        /// <HTML>
        /// <HEAD>
        /// <TITLE>Welcome to Webdiyer.com </TITLE>
        /// <script runat="server">
        ///		SqlConnection conn;
        ///		SqlCommand cmd;
        ///		void Page_Load(object src,EventArgs e)
        ///		{
        ///			conn=new SqlConnection(ConfigurationSettings.AppSettings["ConnStr"]);
        ///			if(!Page.IsPostBack)
        ///			{
        ///				cmd=new SqlCommand("select count(id) from news",conn);
        ///				conn.Open();
        ///				pager.RecordCount=(int)cmd.ExecuteScalar();
        ///				conn.Close();
        ///				BindData();
        ///			}
        ///		}
        ///
        ///		void BindData()
        ///		{
        ///			cmd=new SqlCommand("GetPagedNews",conn);
        ///			cmd.CommandType=CommandType.StoredProcedure;
        ///			cmd.Parameters.Add("@pageindex",pager.CurrentPageIndex);
        ///			cmd.Parameters.Add("@pagesize",pager.PageSize);
        ///			conn.Open();
        ///			dataGrid1.DataSource=cmd.ExecuteReader();
        ///			dataGrid1.DataBind();
        ///			conn.Close();
        ///		}
        ///		void ChangePage(object src,PageChangedEventArgs e)
        ///		{
        ///			pager.CurrentPageIndex=e.NewPageIndex;
        ///			BindData();
        ///		}
        ///		</script>
        ///		<meta http-equiv="Content-Language" content="zh-cn">
        ///		<meta http-equiv="content-type" content="text/html;charset=gb2312">
        ///		<META NAME="Generator" CONTENT="EditPlus">
        ///		<META NAME="Author" CONTENT="Webdiyer(yhaili@21cn.com)">
        ///	</HEAD>
        ///	<body>
        ///		<form runat="server" ID="Form1">
        ///			<asp:DataGrid id="dataGrid1" runat="server" />
        ///
        ///			<Webdiyer:AspNetPager id="pager" runat="server" 
        ///			PageSize="8" 
        ///			NumericButtonCount="8" 
        ///			ShowCustomInfoSection="before" 
        ///			ShowInputBox="always" 
        ///			CssClass="mypager" 
        ///			HorizontalAlign="center" 
        ///			OnPageChanged="ChangePage" />
        ///
        ///		</form>
        ///	</body>
        ///</HTML>
        /// ]]>
        /// </code></p>
        /// <code><![CDATA[
        ///CREATE procedure GetPagedNews
        ///		(@pagesize int,
        ///		@pageindex int)
        ///		as
        ///		set nocount on
        ///		declare @indextable table(id int identity(1,1),nid int)
        ///		declare @PageLowerBound int
        ///		declare @PageUpperBound int
        ///		set @PageLowerBound=(@pageindex-1)*@pagesize
        ///		set @PageUpperBound=@PageLowerBound+@pagesize
        ///		set rowcount @PageUpperBound
        ///		insert into @indextable(nid) select id from news order by addtime desc
        ///		select O.id,O.title,O.source,O.addtime from news O,@indextable t where O.id=t.nid
        ///		and t.id>@PageLowerBound and t.id<=@PageUpperBound order by t.id
        ///		set nocount off
        ///GO
        /// ]]>
        /// </code>
        /// </example>
        [Browsable(false),
        Description("The count of records to page,default 225"),
        Category("Data"),
        DefaultValue(225)]
        public int RecordCount
        {
            get
            {
                object obj = ViewState["Recordcount"];
                return (obj == null) ? 0 : (int)obj;
            }
            set { ViewState["Recordcount"] = value; }
        }

        /// <summary>
        /// Pages count haven't shown behind current page
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PagesRemain
        {
            get
            {
                return PageCount - CurrentPageIndex;
            }
        }

        /// <summary>
        /// Page size
        /// </summary>
        /// <remarks>
        /// Set page size <see cref="RecordCount"/> <see cref="PageCount"/></remarks>
        /// <example>see sample <see cref="AspNetPager"/> 8 records are shown below,
        /// <code>
        /// <![CDATA[
        ///  ...
        ///  <Webdiyer:AspNetPager id="pager" runat="server" PageSize=8 OnPageChanged="ChangePage"/>
        ///  ...
        /// ]]></code></example>
        [Browsable(true),
        Description("Page size"),
        Category("Paging"),
        DefaultValue(10)]
        public int PageSize
        {
            get
            {
                object obj = ViewState["PageSize"];
                return (obj == null) ? 10 : (int)obj;
            }
            set
            {
                ViewState["PageSize"] = value;
            }
        }

        /// <summary>
        /// Records count haven't shown behind current page
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RecordsRemain
        {
            get
            {
                if (CurrentPageIndex < PageCount)
                    return RecordCount - (CurrentPageIndex * PageSize);
                return 0;
            }
        }


        /// <summary>
        /// Page count
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PageCount
        {
            get { return (int)Math.Ceiling((double)RecordCount / (double)PageSize); }
        }


        #endregion

        #region TextBox and Submit Button

        /// <summary>
        /// Show PageSize input box
        /// </summary>
        /// <remarks>
        /// set input box to be shown
        ///</remarks>
        [Browsable(true),
        Description("Show PageSize input box"),
        Category("TextBox and Submit Button"),
        DefaultValue(ShowInputBox.Never)]
        public ShowInputBox ShowPageSize
        {
            get
            {
                object obj = ViewState["ShowPageSize"];
                return (obj == null) ? ShowInputBox.Never : (ShowInputBox)obj;
            }
            set { ViewState["ShowPageSize"] = value; }
        }

        /// <summary>
        /// Show input box
        /// </summary>
        /// <remarks>
        /// set input box to be shown
        ///</remarks>
        [Browsable(true),
        Description("Show input box"),
       Category("TextBox and Submit Button"),
        DefaultValue(ShowInputBox.Always)]
        public ShowInputBox ShowInputBox
        {
            get
            {
                object obj = ViewState["ShowInputBox"];
                return (obj == null) ? ShowInputBox.Always : (ShowInputBox)obj;
            }
            set { ViewState["ShowInputBox"] = value; }
        }

        /// <summary>
        /// Css class of input box
        /// </summary>
        [Browsable(true),
        Category("TextBox and Submit Button"),
        DefaultValue(null),
        Description("Css class of input box")]
        public string InputBoxClass
        {
            get
            {
                return (string)ViewState["InpubBoxClass"];
            }
            set
            {
                if (value.Trim().Length > 0)
                    ViewState["InputBoxClass"] = value;
            }
        }

        /// <summary>
        /// Style text of input box
        /// </summary>

        [Browsable(true),
        Category("TextBox and Submit Button"),
        DefaultValue(null),
        Description("Style text of input box")]
        public string InputBoxStyle
        {
            get
            {
                return (string)ViewState["InputBoxStyle"];
            }
            set
            {
                if (value.Trim().Length > 0)
                    ViewState["InputBoxStyle"] = value;
            }
        }

        /// <summary>
        /// Text before input box
        /// </summary>
        [Browsable(true),
        Category("TextBox and Submit Button"),
        DefaultValue(null),
        Description("Text before input box")]
        public string TextBeforeInputBox
        {
            get
            {
                return (string)ViewState["TextBeforeInputBox"];
            }
            set
            {
                ViewState["TextBeforeInputBox"] = value;
            }
        }

        /// <summary>
        /// Text after input box
        /// </summary>
        [Browsable(true),
        DefaultValue(null),
        Category("TextBox and Submit Button"),
        Description("Text after input box")]
        public string TextAfterInputBox
        {
            get
            {
                return (string)ViewState["TextAfterInputBox"];
            }
            set
            {
                ViewState["TextAfterInputBox"] = value;
            }
        }


        /// <summary>
        /// Text of submit button
        /// </summary>
        [Browsable(true),
        Category("TextBox and Submit Button"),
        DefaultValue(" Go"),
        Description("Text of submit button")]
        public string SubmitButtonText
        {
            get
            {
                object obj = ViewState["SubmitButtonText"];
                return (obj == null) ? " Go" : (string)obj;
            }
            set
            {
                if (value.Trim().Length > 0)
                    ViewState["SubmitButtonText"] = value;
            }
        }
        /// <summary>
        /// Css class of submit button
        /// </summary>
        [Browsable(true),
        Category("TextBox and Submit Button"),
        DefaultValue(null),
        Description("Css class of submit button")]
        public string SubmitButtonClass
        {
            get
            {
                return (string)ViewState["SubmitButtonClass"];
            }
            set
            {
                ViewState["SubmitButtonClass"] = value;
            }
        }

        /// <summary>
        /// Style text of submit button
        /// </summary>
        [Browsable(true),
        Category("TextBox and Submit Button"),
        DefaultValue(null),
        Description("Style text of submit button")]
        public string SubmitButtonStyle
        {
            get
            {
                return (string)ViewState["SubmitButtonStyle"];
            }
            set
            {
                ViewState["SubmitButtonStyle"] = value;
            }
        }
        /// <summary>
        /// Auto show input box when page count touch its setting value and <see cref="ShowInputBox"/> is set to Auto but not Never or Always
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Browsable(true),
        Description("Auto show input box when page count touch its setting value"),
        Category("TextBox and Submit Button"),
        DefaultValue(30)]
        public int ShowBoxThreshold
        {
            get
            {
                object obj = ViewState["ShowBoxThreshold"];
                return (obj == null) ? 30 : (int)obj;
            }
            set { ViewState["ShowBoxThreshold"] = value; }
        }


        #endregion

        #region CustomInfoSection

        /// <summary>
        /// The shown way of customize area
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Browsable(true),
        Description("The shown way of customize area"),
        DefaultValue(ShowCustomInfoSection.Left),
        Category("CustomInfoSection")]
        public ShowCustomInfoSection ShowCustomInfoSection
        {
            get
            {
                object obj = ViewState["ShowCustomInfoSection"];
                return (obj == null) ? ShowCustomInfoSection.Left : (ShowCustomInfoSection)obj;
            }
            set { ViewState["ShowCustomInfoSection"] = value; }
        }

        /// <summary>
        /// CustomInfoTextAlign
        /// </summary>
        [Browsable(true),
        Category("CustomInfoSection"),
        DefaultValue(HorizontalAlign.Left),
        Description("Custom Info Text Align")]
        public HorizontalAlign CustomInfoTextAlign
        {
            get
            {
                object obj = ViewState["CustomInfoTextAlign"];
                return (obj == null) ? HorizontalAlign.Right : (HorizontalAlign)obj;
            }
            set
            {
                ViewState["CustomInfoTextAlign"] = value;
            }
        }

        /// <summary>
        /// CustomInfoSectionWidth
        /// </summary>
        [Browsable(true),
        Category("CustomInfoSection"),
        DefaultValue(typeof(Unit), "40%"),
        Description("Custom Info Section Width")]
        public Unit CustomInfoSectionWidth
        {
            get
            {
                object obj = ViewState["CustomInfoSectionWidth"];
                return (obj == null) ? Unit.Percentage(40) : (Unit)obj;
            }
            set
            {
                ViewState["CustomInfoSectionWidth"] = value;
            }
        }

        /// <summary>
        /// CustomInfoClass
        /// </summary>
        [Browsable(true),
        Category("CustomInfoSection"),
        DefaultValue(null),
        Description("CustomInfoClass")]
        public string CustomInfoClass
        {
            get
            {
                object obj = ViewState["CustomInfoClass"];
                return (obj == null) ? CssClass : (string)obj;
            }
            set
            {
                ViewState["CustomInfoClass"] = value;
            }
        }

        /// <summary>
        /// CustomInfoStyle
        /// </summary>
        [Browsable(true),
        Category("CustomInfoSection"),
        DefaultValue(null),
        Description("CustomInfoStyle")]
        public string CustomInfoStyle
        {
            get
            {
                object obj = ViewState["CustomInfoStyle"];
                return (obj == null) ? GetStyleString() : (string)obj;
            }
            set
            {
                ViewState["CustomInfoStyle"] = value;
            }
        }

        /// <summary>
        /// Get custominfo
        /// </summary>
        [Browsable(true),
        Category("CustomInfoSection"),
        Description("Get custominfo")]
        public string CustomInfoText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                //sb.Append("Count of records:<font color=\"blue\"><b>" + this.RecordCount.ToString() + "</b></font>");
                //sb.Append("Count of pages:<font color=\"blue\"><b>" + this.PageCount.ToString() + "</b></font>");
                //sb.Append("Current page index:<font color=\"red\"><b>" + this.CurrentPageIndex.ToString() + "</b></font>");
                sb.Append("<label class=\"CurrentPageIndex\">Page&nbsp;")
                    .Append(this.CurrentPageIndex.ToString())
                    .Append("/")
                    .Append(this.PageCount.ToString())
                    .Append("&nbsp;</label>");
                return sb.ToString();
            }
        }

        #endregion

        #region Others

        /// <summary>
        /// Is pager always shown
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Browsable(true),
        Category("Behavior"),
        DefaultValue(false),
        Description("Is pager always shown")]
        public bool AlwaysShow
        {
            get
            {
                object obj = ViewState["AlwaysShow"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["AlwaysShow"] = value;
            }
        }


        /// <summary>
        /// Css class of pager 
        /// </summary>
        [Browsable(true),
        Description("Css class of pager"),
        Category("Appearance"),
        DefaultValue("divPager")]
        public override string CssClass
        {
            //get{return base.CssClass;}
            //set
            //{
            //    base.CssClass=value;
            //    cssClassName=value;
            //}
            get { return cssClassName; }
            set
            {
                cssClassName = value;
            }

        }


        /// <summary>
        /// Enable viewstate
        /// </summary>
        [Browsable(false),
        Description("It shall be set to true"),
        DefaultValue(true),
        Category("Behavior")]
        public override bool EnableViewState
        {
            get
            {
                return base.EnableViewState;
            }
            set
            {
                base.EnableViewState = true;
            }
        }

        /// <summary>
        /// Error message will be shown to user when page index is out of range
        /// </summary>
        [Browsable(true),
        Description("Error message will be shown to user when page index is out of range"),
        DefaultValue("Page index out of range"),
        Category("Data")]
        public string PageIndexOutOfRangeErrorString
        {
            get
            {
                object obj = ViewState["PageIndexOutOfRangeErrorString"];
                return (obj == null) ? "Index pager of Error" : (string)obj;
            }
            set
            {
                ViewState["PageIndexOutOfRangeErrorString"] = value;
            }
        }

        /// <summary>
        /// Error message will be shown to user when page index format which is entered by user is invalid
        /// </summary>
        [Browsable(true),
       Description("Error message will be shown to user when page index format is invalid"),
        DefaultValue("Invalid page index"),
        Category("Data")]
        public string InvalidPageIndexErrorString
        {
            get
            {
                object obj = ViewState["InvalidPageIndexErrorString"];
                return (obj == null) ? "Index pager of Error" : (string)obj;
            }
            set
            {
                ViewState["InvalidPageIndexErrorString"] = value;
            }
        }
        /// <summary>
        /// Append to no results message
        /// </summary>
        [Browsable(true),
        Description("Append to no results message"),
        Category("Data")]
        public string AppendMessage
        {
            get
            {
                string _AppendMessage = ViewState["AppendMessage"] == null ? "" : ViewState["AppendMessage"].ToString();
                if (_AppendMessage == "" && AppendName != "")
                {
                    _AppendMessage = String.Format("There are 0 {0} in the list.", AppendName);
                }
                return _AppendMessage;
            }
            set
            {
                ViewState["AppendMessage"] = value;
            }
        }
        /// <summary>
        /// AppendName
        /// </summary>
        [Browsable(true),
      Description("data name displayed in current page"),
        Category("Data")]
        public string AppendName
        {
            get
            {
                return ViewState["AppendName"] == null ? "" : ViewState["AppendName"].ToString();
            }
            set
            {
                ViewState["AppendName"] = value;
            }
        }
        #endregion

        #endregion

        #region Control Rendering Logic

        /// <summary>
        /// override<see cref="System.Web.UI.Control.OnLoad"/>method
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/></param>
        protected override void OnLoad(EventArgs e)
        {
            if (urlPaging)
            {
                currentUrl = Page.Request.Path;
                urlParams = Page.Request.QueryString;
                string pageIndex = Page.Request.QueryString[urlPageIndexName];
                int index = 1;
                try
                {
                    index = int.Parse(pageIndex);
                }
                catch { }
                OnPageChanged(new PageChangedEventArgs(index));
            }
            else
            {
                inputPageIndex = Page.Request.Form[this.UniqueID + "_input"];
            }
            base.OnLoad(e);
        }
        /// <summary>
        /// override<see cref="System.Web.UI.Control.OnPreRender"/>method
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/></param>
        protected override void OnPreRender(EventArgs e)
        {
            //if (PageCount > 1)
            //{
            string checkscript = "<script language=\"Javascript\">function doCheck" + this.ClientID + "(el){var r=new RegExp(\"^\\\\s*(\\\\d+)\\\\s*$\");if(r.test(el.value)){if(RegExp.$1<1||RegExp.$1>" + PageCount.ToString() + "){alert(\"" + PageIndexOutOfRangeErrorString + "\");document.getElementsByName(\'" + this.UniqueID + "_input\')[0].select();return false;}return true;}alert(\"" + InvalidPageIndexErrorString + "\");document.getElementsByName(\'" + this.UniqueID + "_input\')[0].select();return false;}</script>";
            string checkscriptPageSize = "<script language=\"Javascript\">function doCheckPageSize" + this.ClientID + "(el){var r=new RegExp(\"^\\\\s*(\\\\d+)\\\\s*$\");if(r.test(el.value)){if(RegExp.$1<1){alert(\"" + PageIndexOutOfRangeErrorString + "\");document.getElementsByName(\'" + this.UniqueID + "_inputPageSize\')[0].select();return false;}return true;}alert(\"" + InvalidPageIndexErrorString + "\");document.getElementsByName(\'" + this.UniqueID + "_inputPageSize\')[0].select();return false;}</script>";
            if ((ShowPageSize == ShowInputBox.Always) || (ShowPageSize == ShowInputBox.Auto && PageCount >= ShowBoxThreshold))
            {
                Type cstype = this.GetType();
                ScriptManager.RegisterClientScriptBlock(this, cstype, "checkinputPageSize" + this.ClientID, checkscriptPageSize, false);
            }
            if ((ShowInputBox == ShowInputBox.Always) || (ShowInputBox == ShowInputBox.Auto))
            {
                //ClientScriptManager csm = Page.ClientScript;
                Type cstype = this.GetType();
                //if (!csm.IsClientScriptBlockRegistered("checkinput"))
                //    csm.RegisterClientScriptBlock(cstype, "checkinput", checkscript, false);
                ScriptManager.RegisterClientScriptBlock(this, cstype, "checkinput" + this.ClientID, checkscript, false);
                string script = "<script language=\"javascript\" > <!-- \nfunction BuildUrlString(key,value){ var loc=window.location.search.substring(1); var params=loc.split(\"&\"); if(params.length<=1||(params.length==2&&params[0].toLowerCase()==key)) return location.pathname+\"?\"+key+\"=\"+value; var newparam=\"\"; var flag=false; for(i=0;i<params.length;i++){ if(params[i].split(\"=\")[0].toLowerCase()==key.toLowerCase()){ params[i]=key+\"=\"+value; flag=true; break; } } for(i=0;i<params.length;i++){ newparam+=params[i]+\"&\"; } if(flag) newparam=newparam.substring(0,newparam.length-1); else newparam+=key+\"=\"+value; return location.pathname+\"?\"+newparam; } \n//--> </script>";
                //if (!csm.IsClientScriptBlockRegistered("BuildUrlScript"))
                //    csm.RegisterClientScriptBlock(cstype, "BuildUrlScript", script, false);
                ScriptManager.RegisterClientScriptBlock(this, cstype, "BuildUrlScript", script, false);
            }
            //}
            base.OnPreRender(e);
        }

        /// <summary>
        /// override<see cref="System.Web.UI.WebControls.WebControl.AddAttributesToRender"/> 
        /// </summary>
        /// <param name="writer"><see cref="System.Web.UI.HtmlTextWriter"/></param>
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (this.Page != null)
                this.Page.VerifyRenderingInServerForm(this);
            base.AddAttributesToRender(writer);
        }

        ///<summary>
        ///override<see cref="System.Web.UI.WebControls.WebControl.RenderBeginTag"/> <see cref="AspNetPager"/>  <see cref="System.Web.UI.HtmlTextWriter"/> 
        ///</summary>
        ///<param name="writer"><see cref="System.Web.UI.HtmlTextWriter"/></param>
        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            bool showPager = (PageCount > 1 || (PageCount <= 1 && AlwaysShow && RecordCount != 0));
            base.RenderBeginTag(writer);
            if ((ShowCustomInfoSection == ShowCustomInfoSection.Left || ShowCustomInfoSection == ShowCustomInfoSection.Right) && showPager)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
                writer.AddAttribute(HtmlTextWriterAttribute.Style, GetStyleString());
                if (Height != Unit.Empty)
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
                writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
                writer.RenderBeginTag(HtmlTextWriterTag.Table);
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                WriteCellAttributes(writer, true);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
            }
        }

        ///<summary>
        ///override<see cref="System.Web.UI.WebControls.WebControl.RenderEndTag"/>  <see cref="AspNetPager"/>   <see cref="System.Web.UI.HtmlTextWriter"/>  
        ///</summary>
        ///<param name="writer"><see cref="System.Web.UI.HtmlTextWriter"/> </param>
        public override void RenderEndTag(HtmlTextWriter writer)
        {
            if ((ShowCustomInfoSection == ShowCustomInfoSection.Left || ShowCustomInfoSection == ShowCustomInfoSection.Right) && (PageCount > 1 || (PageCount <= 1 && AlwaysShow && RecordCount != 0)))
            {
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();
            }
            base.RenderEndTag(writer);
            writer.WriteLine();
        }


        /// <summary>
        /// override<see cref="System.Web.UI.WebControls.WebControl.RenderContents"/>  <see cref="System.Web.UI.HtmlTextWriter"/> 
        /// </summary>
        /// <param name="writer"><see cref="System.Web.UI.HtmlTextWriter"/></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (RecordCount == 0)
            {
                if (AppendMessage == "") AppendMessage = NO_RESULTS;
                writer.Write("<div  style=" + "\"padding-left: 5px;color: #007E7A;font-size: 9pt;\">" + AppendMessage + "</div>");

                // writer.Write("<font style=" + "\"font-size:12px\"" + " >" + NO_RESULTS +AppendMessage+ "</font>");
                return;
            }

            if (PageCount <= 1 && !AlwaysShow)
                return;

            if (ShowCustomInfoSection == ShowCustomInfoSection.Left)
            {
                writer.Write(CustomInfoText);
                //writer.AddAttribute(HtmlTextWriterAttribute.Align, "right");
                //writer.RenderEndTag();
                //WriteCellAttributes(writer, false);
                //writer.AddAttribute(HtmlTextWriterAttribute.Class,CssClass);
                //writer.RenderBeginTag(HtmlTextWriterTag.Td);
            }
            // add by Jwwang 2007-07-02
            string scriptRef = string.Empty;
            string postRef = string.Empty;
            string keydownScript = string.Empty;
            string clickScript = string.Empty;
            bool blShowPageSize = (ShowPageSize == ShowInputBox.Always) || (ShowPageSize == ShowInputBox.Auto && PageCount >= ShowBoxThreshold);
            if ((ShowInputBox == ShowInputBox.Always) || (ShowInputBox == ShowInputBox.Auto && PageCount >= ShowBoxThreshold))
            {
                scriptRef = "doCheck" + this.ClientID + "(document.getElementsByName(\'" + this.UniqueID + "_input\')[0])";
                if (blShowPageSize)
                    scriptRef += "&&doCheckPageSize" + this.ClientID + "(document.getElementsByName(\'" + this.UniqueID + "_inputPageSize\')[0])";
                postRef = "var event=arguments[0]||window.event;if(event.keyCode==13){event.cancelBubble = true;if(" + scriptRef + "){event.returnValue=false;__doPostBack(\'" + this.UniqueID + "\',document.getElementsByName(\'" + this.UniqueID + "_input\')[0].value" + (blShowPageSize ? "+','+document.getElementsByName(\'" + this.UniqueID + "_inputPageSize\')[0].value" : string.Empty) + ");return false;}else{event.returnValue=false;return false;}}";
                keydownScript = "var event=arguments[0]||window.event;if(event.keyCode==13){event.cancelBubble = true;if(" + scriptRef + "){event.returnValue=false;document.getElementsByName(\'" + this.UniqueID + "\')[0].click();return false;}else{event.returnValue=false;return false;}}";
                clickScript = "/*if(" + scriptRef + ")*/{location.href=BuildUrlString(\'" + urlPageIndexName + "\',document.getElementsByName(\'" + this.UniqueID + "_input\')[0].value" + (blShowPageSize ? "+','+document.getElementsByName(\'" + this.UniqueID + "_inputPageSize\')[0].value" : string.Empty) + ")}";
            }
            // add end
            if ((ShowInputBox == ShowInputBox.Always) || (ShowInputBox == ShowInputBox.Auto && PageCount >= ShowBoxThreshold))
            {
                writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;");
                if (TextBeforeInputBox != null)
                    writer.Write(TextBeforeInputBox);
                // add by Jwwang 2007-07-02
                if (blShowPageSize)
                {
                    writer.Write("Page Size:");
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "30px");
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, PageSize.ToString());
                    if (InputBoxStyle != null && InputBoxStyle.Trim().Length > 0)
                        writer.AddAttribute(HtmlTextWriterAttribute.Style, InputBoxStyle);
                    if (InputBoxClass != null && InputBoxClass.Trim().Length > 0)
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, InputBoxClass);
                    //if (PageCount <= 1 && AlwaysShow)
                    //    writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "true");
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID + "_inputPageSize");
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "textboxGo");
                    writer.AddAttribute("onkeypress", (urlPaging == true) ? keydownScript : postRef);
                    //writer.AddAttribute("onkeypress", "var event=arguments[0]||window.event;if(event.keyCode==13)event.returnValue=false;");
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                    writer.Write("&nbsp;&nbsp;Page:");
                }
                // add end
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "30px");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, CurrentPageIndex.ToString());
                if (InputBoxStyle != null && InputBoxStyle.Trim().Length > 0)
                    writer.AddAttribute(HtmlTextWriterAttribute.Style, InputBoxStyle);
                if (InputBoxClass != null && InputBoxClass.Trim().Length > 0)
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, InputBoxClass);
                if (PageCount <= 1 && AlwaysShow)
                    writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "true");
                writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID + "_input");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "textboxGo");
                writer.AddAttribute("onkeypress", (urlPaging == true) ? keydownScript : postRef);
                //writer.AddAttribute("onkeypress", "var event=arguments[0]||window.event;if(event.keyCode==13)event.returnValue=false;");
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();
                if (TextAfterInputBox != null)
                    writer.Write(TextAfterInputBox);
                writer.Write("&nbsp;&nbsp;");
                writer.AddAttribute(HtmlTextWriterAttribute.Type, (urlPaging == true) ? "Button" : "Submit");
                writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, "_buttongoforgridpager" + new Random().Next().ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "buttongo");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, SubmitButtonText);
                if (SubmitButtonClass != null && SubmitButtonClass.Trim().Length > 0)
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, SubmitButtonClass);
                if (SubmitButtonStyle != null && SubmitButtonStyle.Trim().Length > 0)
                    writer.AddAttribute(HtmlTextWriterAttribute.Style, SubmitButtonStyle);
                //if (PageCount <= 1 && AlwaysShow)
                //{
                //    writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "true");
                //    writer.AddAttribute(HtmlTextWriterAttribute.Style, "color:silver");
                //}
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, (urlPaging == true) ? clickScript : "return " + scriptRef);
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();
            }
            writer.Write("&nbsp;&nbsp;");
            //this.WriteButtonSpace(writer);
            int midpage = (int)((CurrentPageIndex - 1) / NumericButtonCount);
            int pageoffset = midpage * NumericButtonCount;
            int endpage = ((pageoffset + NumericButtonCount) > PageCount) ? PageCount : (pageoffset + NumericButtonCount);
            this.CreateNavigationButton(writer, "first");
            writer.Write("&nbsp;&nbsp;");
            this.CreateNavigationButton(writer, "prev");
            writer.Write("&nbsp;&nbsp;");
            if (ShowPageIndex)
            {
                if (CurrentPageIndex > NumericButtonCount)
                    CreateMoreButton(writer, pageoffset);
                for (int i = pageoffset + 1; i <= endpage; i++)
                {
                    CreateNumericButton(writer, i);
                }
                if (PageCount > NumericButtonCount && endpage < PageCount)
                    CreateMoreButton(writer, endpage + 1);
            }
            this.CreateNavigationButton(writer, "next");
            writer.Write("&nbsp;&nbsp;");
            this.CreateNavigationButton(writer, "last");
            writer.Write("&nbsp;&nbsp;");

            if (ShowCustomInfoSection == ShowCustomInfoSection.Right)
            {
                writer.RenderEndTag();
                WriteCellAttributes(writer, false);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(CustomInfoText);
            }
        }


        #endregion

        #region Private Helper Functions

        /// <summary>
        /// Change style text to string
        /// </summary>
        /// <returns></returns>
        private string GetStyleString()
        {
            if (Style.Count > 0)
            {
                string stl = null;
                string[] skeys = new string[Style.Count];
                Style.Keys.CopyTo(skeys, 0);
                for (int i = 0; i < skeys.Length; i++)
                {
                    stl += String.Concat(skeys[i], ":", Style[skeys[i]], ";");
                }
                return stl += "vertical-align:middle;";
            }
            return null;
        }

        /// <summary>
        /// Set attribute to td for navigation and customer area
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="leftCell">is it the first td</param>
        private void WriteCellAttributes(HtmlTextWriter writer, bool leftCell)
        {
            string customUnit = CustomInfoSectionWidth.ToString();
            if (ShowCustomInfoSection == ShowCustomInfoSection.Left && leftCell || ShowCustomInfoSection == ShowCustomInfoSection.Right && !leftCell)
            {
                //if(CustomInfoClass!=null&&CustomInfoClass.Trim().Length>0)
                //    writer.AddAttribute(HtmlTextWriterAttribute.Class,CustomInfoClass);
                if (CustomInfoStyle != null && CustomInfoStyle.Trim().Length > 0)
                    writer.AddAttribute(HtmlTextWriterAttribute.Style, CustomInfoStyle);
                writer.AddAttribute(HtmlTextWriterAttribute.Valign, "middle");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, customUnit);
                writer.AddAttribute(HtmlTextWriterAttribute.Align, CustomInfoTextAlign.ToString().ToLower());
            }
            else
            {
                if (CustomInfoSectionWidth.Type == UnitType.Percentage)
                {
                    customUnit = (Unit.Percentage(100 - CustomInfoSectionWidth.Value)).ToString();
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Width, customUnit);
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Valign, "middle");
                writer.AddAttribute(HtmlTextWriterAttribute.Align, HorizontalAlign.ToString().ToLower());
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Nowrap, "true");
        }

        /// <summary>
        /// Get string of hyperlink with navigation button
        /// </summary>
        /// <param name="pageIndex">the index of navigation button corresponded to page index</param>
        /// <returns>link string</returns>
        private string GetHrefString(int pageIndex)
        {
            if (urlPaging)
            {
                NameValueCollection col = new NameValueCollection();
                col.Add(urlPageIndexName, pageIndex.ToString());
                return BuildUrlString(col);
            }
            return Page.ClientScript.GetPostBackClientHyperlink(this, pageIndex.ToString());
        }

        /// <summary>
        /// URL paging method, add paging parameter to url
        /// </summary>
        /// <param name="col">paging parameter of key\value pair</param>
        /// <returns>the url string</returns>
        private string BuildUrlString(NameValueCollection col)
        {
            int i;
            string tempstr = "";
            if (urlParams == null || urlParams.Count <= 0)
            {
                for (i = 0; i < col.Count; i++)
                {
                    tempstr += String.Concat("&", col.Keys[i], "=", col[i]);
                }
                return String.Concat(currentUrl, "?", tempstr.Substring(1));
            }
            NameValueCollection newCol = new NameValueCollection(urlParams);
            string[] newColKeys = newCol.AllKeys;
            for (i = 0; i < newColKeys.Length; i++)
            {
                newColKeys[i] = newColKeys[i].ToLower();
            }
            for (i = 0; i < col.Count; i++)
            {
                if (Array.IndexOf(newColKeys, col.Keys[i].ToLower()) < 0)
                    newCol.Add(col.Keys[i], col[i]);
                else
                    newCol[col.Keys[i]] = col[i];
            }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (i = 0; i < newCol.Count; i++)
            {
                sb.Append("&");
                sb.Append(newCol.Keys[i]);
                sb.Append("=");
                sb.Append(newCol[i]);
            }
            return String.Concat(currentUrl, "?", sb.ToString().Substring(1));
        }

        /// <summary>
        /// Create navigation button
        /// </summary>
        /// <param name="writer"><see cref="System.Web.UI.HtmlTextWriter"/></param>
        /// <param name="btnname">button name</param>
        private void CreateNavigationButton(HtmlTextWriter writer, string btnname)
        {
            if (!ShowFirstLast && (btnname == "first" || btnname == "last"))
                return;
            if (!ShowPrevNext && (btnname == "prev" || btnname == "next"))
                return;
            string linktext = "";
            bool disabled;
            int pageIndex;
            string strBtnPageName = String.Concat(this.UniqueID, "_", btnname);
            string strBtnPageScript;
            bool imgButton = (PagingButtonType == PagingButtonType.Image && NavigationButtonType == PagingButtonType.Image);
            if (btnname == "prev" || btnname == "first")
            {
                disabled = (CurrentPageIndex <= 1);
                if (!ShowDisabledButtons && disabled)
                    return;
                pageIndex = (btnname == "first") ? 1 : (CurrentPageIndex == 1 ? 1 : (CurrentPageIndex - 1));
                if (imgButton)
                {
                    if (!disabled)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(pageIndex));
                        AddToolTip(writer, pageIndex);
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, String.Concat(ImagePath, btnname, ButtonImageNameExtension, ButtonImageExtension));
                        writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                        writer.AddAttribute(HtmlTextWriterAttribute.Align, ButtonImageAlign.ToString());
                        writer.RenderBeginTag(HtmlTextWriterTag.Img);
                        writer.RenderEndTag();
                        writer.RenderEndTag();
                    }
                    else
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, String.Concat(ImagePath, btnname, DisabledButtonImageNameExtension, ButtonImageExtension));
                        writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                        writer.AddAttribute(HtmlTextWriterAttribute.Align, ButtonImageAlign.ToString());
                        writer.RenderBeginTag(HtmlTextWriterTag.Img);
                        writer.RenderEndTag();
                    }
                }
                else
                {
                    linktext = (btnname == "prev") ? PrevPageText : FirstPageText;
                    if (disabled)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "true");
                        writer.AddAttribute(HtmlTextWriterAttribute.Style, "color:silver");
                    }
                    WriteCssClass(writer);
                    AddToolTip(writer, pageIndex);
                    strBtnPageScript = Page.ClientScript.GetPostBackEventReference(this, pageIndex.ToString());
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "Button");//Submit
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, strBtnPageName);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "buttonpage");
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, linktext);
                    writer.AddAttribute(HtmlTextWriterAttribute.Onclick, strBtnPageScript);
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                    //else
                    // {
                    //     WriteCssClass(writer);
                    //     AddToolTip(writer, pageIndex);
                    //     writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(pageIndex));
                    // }
                    // writer.RenderBeginTag(HtmlTextWriterTag.A);
                    // writer.Write(linktext);
                    // writer.RenderEndTag();
                }
            }
            else
            {
                disabled = (CurrentPageIndex >= PageCount);
                if (!ShowDisabledButtons && disabled)
                    return;
                pageIndex = (btnname == "last") ? PageCount : (CurrentPageIndex == PageCount ? PageCount : (CurrentPageIndex + 1));
                if (imgButton)
                {
                    if (!disabled)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(pageIndex));
                        AddToolTip(writer, pageIndex);
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, String.Concat(ImagePath, btnname, ButtonImageNameExtension, ButtonImageExtension));
                        writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                        writer.AddAttribute(HtmlTextWriterAttribute.Align, ButtonImageAlign.ToString());
                        writer.RenderBeginTag(HtmlTextWriterTag.Img);
                        writer.RenderEndTag();
                        writer.RenderEndTag();
                    }
                    else
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, String.Concat(ImagePath, btnname, DisabledButtonImageNameExtension, ButtonImageExtension));
                        writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                        writer.AddAttribute(HtmlTextWriterAttribute.Align, ButtonImageAlign.ToString());
                        writer.RenderBeginTag(HtmlTextWriterTag.Img);
                        writer.RenderEndTag();
                    }
                }
                else
                {
                    linktext = (btnname == "next") ? NextPageText : LastPageText;
                    if (disabled)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "true");
                        writer.AddAttribute(HtmlTextWriterAttribute.Style, "color:silver");
                    }
                    //else
                    //{

                    //string clickScript = "location.href=BuildUrlString(\'" + urlPageIndexName + "\',document.all[\'" + this.UniqueID + "_input\'].value)}";
                    //if (SubmitButtonClass != null && SubmitButtonClass.Trim().Length > 0)
                    //    writer.AddAttribute(HtmlTextWriterAttribute.Class, SubmitButtonClass);
                    //if (SubmitButtonStyle != null && SubmitButtonStyle.Trim().Length > 0)
                    //    writer.AddAttribute(HtmlTextWriterAttribute.Style, SubmitButtonStyle);
                    //if (PageCount <= 1 && AlwaysShow)
                    //    writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "true");

                    strBtnPageScript = Page.ClientScript.GetPostBackEventReference(this, pageIndex.ToString());
                    WriteCssClass(writer);
                    AddToolTip(writer, pageIndex);
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "Button");//Submit
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, strBtnPageName);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "buttonpage");
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, linktext);
                    writer.AddAttribute(HtmlTextWriterAttribute.Onclick, strBtnPageScript);
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();

                    //writer.AddAttribute(HtmlTextWriterAttribute.Class, "buttonpage");
                    //writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(pageIndex));
                    //}
                    //writer.RenderBeginTag(HtmlTextWriterTag.A);
                    //writer.Write(linktext);
                    //writer.RenderEndTag();
                }
            }
            WriteButtonSpace(writer);
        }

        /// <summary>
        /// Write the css class
        /// </summary>
        /// <param name="writer"><see cref="System.Web.UI.HtmlTextWriter"/></param>
        private void WriteCssClass(HtmlTextWriter writer)
        {
            //if(cssClassName!=null&&cssClassName.Trim().Length>0)
            //    writer.AddAttribute(HtmlTextWriterAttribute.Class,cssClassName);
        }

        /// <summary>
        /// Add tool tip to navigation buttons
        /// </summary>
        /// <param name="writer"><see cref="System.Web.UI.HtmlTextWriter"/></param>
        /// <param name="pageIndex">the index of navigation button corresponded to page index</param>
        private void AddToolTip(HtmlTextWriter writer, int pageIndex)
        {
            if (ShowNavigationToolTip)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Title, String.Format(NavigationToolTipTextFormatString, pageIndex));
            }
        }

        /// <summary>
        /// Create number index of navigation button
        /// </summary>
        /// <param name="writer"><see cref="System.Web.UI.HtmlTextWriter"/></param>
        /// <param name="index">index of button for page index</param>
        private void CreateNumericButton(HtmlTextWriter writer, int index)
        {
            bool isCurrent = (index == CurrentPageIndex);
            if (PagingButtonType == PagingButtonType.Image && NumericButtonType == PagingButtonType.Image)
            {
                if (!isCurrent)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(index));
                    AddToolTip(writer, index);
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    CreateNumericImages(writer, index, isCurrent);
                    writer.RenderEndTag();
                }
                else
                    CreateNumericImages(writer, index, isCurrent);
            }
            else
            {
                if (isCurrent)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "Bold");
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Color, "red");
                    writer.RenderBeginTag(HtmlTextWriterTag.Font);
                    if (NumericButtonTextFormatString.Length > 0)
                        writer.Write(String.Format(NumericButtonTextFormatString, (ChinesePageIndex == true) ? GetChinesePageIndex(index) : (index.ToString())));
                    else
                        writer.Write((ChinesePageIndex == true) ? GetChinesePageIndex(index) : index.ToString());
                    writer.RenderEndTag();
                }
                else
                {
                    WriteCssClass(writer);
                    AddToolTip(writer, index);
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(index));
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    if (NumericButtonTextFormatString.Length > 0)
                        writer.Write(String.Format(NumericButtonTextFormatString, (ChinesePageIndex == true) ? GetChinesePageIndex(index) : (index.ToString())));
                    else
                        writer.Write((ChinesePageIndex == true) ? GetChinesePageIndex(index) : index.ToString());
                    writer.RenderEndTag();
                }
            }
            WriteButtonSpace(writer);
        }

        /// <summary>
        /// Add space to button
        /// </summary>
        /// <param name="writer"></param>
        private void WriteButtonSpace(HtmlTextWriter writer)
        {
            if (PagingButtonSpacing.Value > 0)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, PagingButtonSpacing.ToString());
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.RenderEndTag();
            }
        }

        /// <summary>
        /// Get Chinese page index
        /// </summary>
        /// <param name="index">page index</param>
        /// <returns>chinese page</returns>
        private string GetChinesePageIndex(int index)
        {
            Hashtable cnChars = new Hashtable();
            cnChars.Add("0", "0");
            cnChars.Add("1", "I");
            cnChars.Add("2", "II");
            cnChars.Add("3", "III");
            cnChars.Add("4", "IV");
            cnChars.Add("5", "V");
            cnChars.Add("6", "VI");
            cnChars.Add("7", "VII");
            cnChars.Add("8", "VIII");
            cnChars.Add("9", "IX");
            string indexStr = index.ToString();
            string retStr = "";
            for (int i = 0; i < indexStr.Length; i++)
            {
                retStr = String.Concat(retStr, cnChars[indexStr[i].ToString()]);
            }
            return retStr;
        }

        /// <summary>
        /// Create image button
        /// </summary>
        /// <param name="writer"><see cref="System.Web.UI.HtmlTextWriter"/></param>
        /// <param name="index">page index</param>
        /// <param name="isCurrent">Is it current page</param>
        private void CreateNumericImages(HtmlTextWriter writer, int index, bool isCurrent)
        {
            string indexStr = index.ToString();
            for (int i = 0; i < indexStr.Length; i++)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Src, String.Concat(ImagePath, indexStr[i], (isCurrent == true) ? CpiButtonImageNameExtension : ButtonImageNameExtension, ButtonImageExtension));
                writer.AddAttribute(HtmlTextWriterAttribute.Align, ButtonImageAlign.ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();
            }
        }

        /// <summary>
        /// Create more(...) button
        /// </summary>
        /// <param name="writer"><see cref="System.Web.UI.HtmlTextWriter"/></param>
        /// <param name="pageIndex">page index</param>
        private void CreateMoreButton(HtmlTextWriter writer, int pageIndex)
        {
            WriteCssClass(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Href, GetHrefString(pageIndex));
            AddToolTip(writer, pageIndex);
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            if (PagingButtonType == PagingButtonType.Image && MoreButtonType == PagingButtonType.Image)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Src, String.Concat(ImagePath, "more", ButtonImageNameExtension, ButtonImageExtension));
                writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                writer.AddAttribute(HtmlTextWriterAttribute.Align, ButtonImageAlign.ToString());
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();
            }
            else
                writer.Write("...");
            writer.RenderEndTag();
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, PagingButtonSpacing.ToString());
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.RenderEndTag();
        }

        #endregion

        #region IPostBackEventHandler Implementation

        /// <summary>
        /// emplement<see cref="IPostBackEventHandler"/> interface <see cref="AspNetPager"/> 
        /// </summary>
        /// <param name="args"></param>
        public void RaisePostBackEvent(string args)
        {
            int pageIndex = CurrentPageIndex;
            try
            {
                if (args == null || args == "")
                    args = inputPageIndex + "," + inputPageSize;
                if (args.IndexOf(",") < 0)
                    args += "," + inputPageSize;
                string[] argsArr = args.Split(',');
                pageIndex = int.Parse(argsArr[0]);
                PageSize = int.Parse(argsArr[1]);
            }
            catch { }
            OnPageChanged(new PageChangedEventArgs(pageIndex));

        }


        #endregion

        #region IPostBackDataHandler Implementation

        /// <summary>
        /// emplement<see cref="IPostBackDataHandler"/> interface <see cref="AspNetPager"/> 
        /// </summary>
        /// <param name="pkey">controls' identify</param>
        /// <param name="pcol">name\value collection</param>
        /// <returns></returns>
        public virtual bool LoadPostData(string pkey, NameValueCollection pcol)
        {
            string str = pcol[this.UniqueID + "_input"];
            string strPageSize = pcol[this.UniqueID + "_inputPageSize"];
            if (string.IsNullOrEmpty(strPageSize))
                strPageSize = this.PageSize.ToString();
            if (str != null && str.Trim().Length > 0 && strPageSize != null && strPageSize.Trim().Length > 0)
            {
                try
                {
                    int pindex = int.Parse(str);
                    int pSize = int.Parse(strPageSize);
                    if (!this.IsViewStateEnabled || (pindex > 0 && pindex <= PageCount && pSize > 0))
                    {
                        inputPageIndex = str;
                        inputPageSize = strPageSize;
                        Page.RegisterRequiresRaiseEvent(this);
                    }
                }
                catch
                { }
            }
            return false;
        }

        /// <summary>
        /// emplement <see cref="IPostBackDataHandler"/> interface
        /// </summary>
        public virtual void RaisePostDataChangedEvent() { }

        #endregion

        #region PageChanged Event
        /// <summary>
        /// page index change
        /// </summary>
        /// <code><![CDATA[
        ///<%@ Page Language="C#"%>
        ///<%@ Import Namespace="System.Data"%>
        ///<%@Import Namespace="System.Data.SqlClient"%>
        ///<%@Import Namespace="System.Configuration"%>
        ///<%@Register TagPrefix="Webdiyer" Namespace="Wuqi.Webdiyer" Assembly="aspnetpager"%>
        ///<HTML>
        ///<HEAD>
        ///<TITLE>Welcome to Webdiyer.com </TITLE>
        ///  <script runat="server">
        ///		SqlConnection conn;
        ///		SqlCommand cmd;
        ///		void Page_Load(object src,EventArgs e)
        ///		{
        ///			conn=new SqlConnection(ConfigurationSettings.AppSettings["ConnStr"]);
        ///			if(!Page.IsPostBack)
        ///			{
        ///				cmd=new SqlCommand("GetNews",conn);
        ///				cmd.CommandType=CommandType.StoredProcedure;
        ///				cmd.Parameters.Add("@pageindex",1);
        ///				cmd.Parameters.Add("@pagesize",1);
        ///				cmd.Parameters.Add("@docount",true);
        ///				conn.Open();
        ///				pager.RecordCount=(int)cmd.ExecuteScalar();
        ///				conn.Close();
        ///				BindData();
        ///			}
        ///		}
        ///
        ///		void BindData()
        ///		{
        ///			cmd=new SqlCommand("GetNews",conn);
        ///			cmd.CommandType=CommandType.StoredProcedure;
        ///			cmd.Parameters.Add("@pageindex",pager.CurrentPageIndex);
        ///			cmd.Parameters.Add("@pagesize",pager.PageSize);
        ///			cmd.Parameters.Add("@docount",false);
        ///			conn.Open();
        ///			dataGrid1.DataSource=cmd.ExecuteReader();
        ///			dataGrid1.DataBind();
        ///			conn.Close();
        ///		}
        ///		void ChangePage(object src,PageChangedEventArgs e)
        ///		{
        ///			pager.CurrentPageIndex=e.NewPageIndex;
        ///			BindData();
        ///		}
        ///  </script>
        ///     <meta http-equiv="Content-Language" content="zh-cn">
        ///		<meta http-equiv="content-type" content="text/html;charset=gb2312">
        ///		<META NAME="Generator" CONTENT="EditPlus">
        ///		<META NAME="Author" CONTENT="Webdiyer(yhaili@21cn.com)">
        ///	</HEAD>
        ///	<body>
        ///		<form runat="server" ID="Form1">
        ///			<asp:DataGrid id="dataGrid1" runat="server" />
        ///			<Webdiyer:AspNetPager id="pager" runat="server" PageSize="8" NumericButtonCount="8" ShowCustomInfoSection="before" ShowInputBox="always" CssClass="mypager" HorizontalAlign="center" OnPageChanged="ChangePage" />
        ///		</form>
        ///	</body>
        ///</HTML>
        /// ]]>
        /// </code>
        /// <code>
        /// <![CDATA[
        ///CREATE procedure GetNews
        /// 	(@pagesize int,
        ///		@pageindex int,
        ///		@docount bit)
        ///		as
        ///		set nocount on
        ///		if(@docount=1)
        ///		select count(id) from news
        ///		else
        ///		begin
        ///		declare @indextable table(id int identity(1,1),nid int)
        ///		declare @PageLowerBound int
        ///		declare @PageUpperBound int
        ///		set @PageLowerBound=(@pageindex-1)*@pagesize
        ///		set @PageUpperBound=@PageLowerBound+@pagesize
        ///		set rowcount @PageUpperBound
        ///		insert into @indextable(nid) select id from news order by addtime desc
        ///		select O.id,O.source,O.title,O.addtime from news O,@indextable t where O.id=t.nid
        ///		and t.id>@PageLowerBound and t.id<=@PageUpperBound order by t.id
        ///		end
        ///		set nocount off
        ///GO
        /// ]]>
        /// </code>
        ///</example>
        public event PageChangedEventHandler PageChanged;

        #endregion

        #region OnPageChanged Method

        /// <summary>
        /// emplement<see cref="PageChanged"/> 
        /// </summary>
        /// <param name="e"> <see cref="PageChangedEventArgs"/> </param>
        protected virtual void OnPageChanged(PageChangedEventArgs e)
        {
            if (this.PageChanged != null)
                PageChanged(this, e);
        }

        #endregion
    }


    #region PageChangedEventHandler Delegate
    /// <summary>
    /// define <see cref="AspNetPager.PageChanged"/> delegate
    /// </summary>
    public delegate void PageChangedEventHandler(object src, PageChangedEventArgs e);

    #endregion

    #region PageChangedEventArgs Class
    /// <summary>
    ///  <see cref="AspNetPager"/> event args<see cref="AspNetPager.PageChanged"/> 
    /// </summary>
    /// <remarks>
    /// <see cref="AspNetPager"/>page index changed<see cref="AspNetPager.PageChanged"/> 
    /// </remarks>
    public sealed class PageChangedEventArgs : EventArgs
    {
        private readonly int _newpageindex;

        /// <summary>
        /// initial
        /// </summary>
        /// <param name="newPageIndex">new page index <see cref="AspNetPager"/> </param>
        public PageChangedEventArgs(int newPageIndex)
        {
            this._newpageindex = newPageIndex;
        }

        /// <code><![CDATA[
        ///<%@ Page Language="C#"%>
        ///<%@ Import Namespace="System.Data"%>
        ///<%@Import Namespace="System.Data.SqlClient"%>
        ///<%@Import Namespace="System.Configuration"%>
        ///<%@Register TagPrefix="Webdiyer" Namespace="Wuqi.Webdiyer" Assembly="aspnetpager"%>
        ///<HTML>
        ///<HEAD>
        ///<TITLE>Welcome to Webdiyer.com </TITLE>
        ///  <script runat="server">
        ///		SqlConnection conn;
        ///		SqlCommand cmd;
        ///		void Page_Load(object src,EventArgs e)
        ///		{
        ///			conn=new SqlConnection(ConfigurationSettings.AppSettings["ConnStr"]);
        ///			if(!Page.IsPostBack)
        ///			{
        ///				cmd=new SqlCommand("GetNews",conn);
        ///				cmd.CommandType=CommandType.StoredProcedure;
        ///				cmd.Parameters.Add("@pageindex",1);
        ///				cmd.Parameters.Add("@pagesize",1);
        ///				cmd.Parameters.Add("@docount",true);
        ///				conn.Open();
        ///				pager.RecordCount=(int)cmd.ExecuteScalar();
        ///				conn.Close();
        ///				BindData();
        ///			}
        ///		}
        ///
        ///		void BindData()
        ///		{
        ///			cmd=new SqlCommand("GetNews",conn);
        ///			cmd.CommandType=CommandType.StoredProcedure;
        ///			cmd.Parameters.Add("@pageindex",pager.CurrentPageIndex);
        ///			cmd.Parameters.Add("@pagesize",pager.PageSize);
        ///			cmd.Parameters.Add("@docount",false);
        ///			conn.Open();
        ///			dataGrid1.DataSource=cmd.ExecuteReader();
        ///			dataGrid1.DataBind();
        ///			conn.Close();
        ///		}
        ///		void ChangePage(object src,PageChangedEventArgs e)
        ///		{
        ///			pager.CurrentPageIndex=e.NewPageIndex;
        ///			BindData();
        ///		}
        ///  </script>
        ///     <meta http-equiv="Content-Language" content="zh-cn">
        ///		<meta http-equiv="content-type" content="text/html;charset=gb2312">
        ///		<META NAME="Generator" CONTENT="EditPlus">
        ///		<META NAME="Author" CONTENT="Webdiyer(yhaili@21cn.com)">
        ///	</HEAD>
        ///	<body>
        ///		<form runat="server" ID="Form1">
        ///			<asp:DataGrid id="dataGrid1" runat="server" />
        ///
        ///			<Webdiyer:AspNetPager id="pager" 
        ///			runat="server" 
        ///			PageSize="8" 
        ///			NumericButtonCount="8" 
        ///			ShowCustomInfoSection="before" 
        ///			ShowInputBox="always" 
        ///			CssClass="mypager" 
        ///			HorizontalAlign="center" 
        ///			OnPageChanged="ChangePage" />
        ///
        ///		</form>
        ///	</body>
        ///</HTML>
        /// ]]>
        /// </code>
        /// <code>
        /// <![CDATA[
        ///CREATE procedure GetNews
        /// 	(@pagesize int,
        ///		@pageindex int,
        ///		@docount bit)
        ///		as
        ///		set nocount on
        ///		if(@docount=1)
        ///		select count(id) from news
        ///		else
        ///		begin
        ///		declare @indextable table(id int identity(1,1),nid int)
        ///		declare @PageLowerBound int
        ///		declare @PageUpperBound int
        ///		set @PageLowerBound=(@pageindex-1)*@pagesize
        ///		set @PageUpperBound=@PageLowerBound+@pagesize
        ///		set rowcount @PageUpperBound
        ///		insert into @indextable(nid) select id from news order by addtime desc
        ///		select O.id,O.source,O.title,O.addtime from news O,@indextable t where O.id=t.nid
        ///		and t.id>@PageLowerBound and t.id<=@PageUpperBound order by t.id
        ///		end
        ///		set nocount off
        ///GO
        /// ]]>
        /// </code>
        ///</example>
        public int NewPageIndex
        {
            get { return _newpageindex; }
        }
    }
    #endregion

    #region ShowInputBox,ShowCustomInfoSection and PagingButtonType Enumerations
    /// <summary>
    /// way for Input box shown
    /// </summary>
    public enum ShowInputBox : byte
    {
        /// <summary>
        /// never show input box
        /// </summary>
        Never,
        /// <summary>
        /// auto show input box by <see cref="AspNetPager.ShowBoxThreshold"/> 
        /// </summary>
        Auto,
        /// <summary>
        /// always show input box
        /// </summary>
        Always
    }


    /// <summary>
    /// Where is the customer info shown
    /// </summary>
    public enum ShowCustomInfoSection : byte
    {
        /// <summary>
        /// never shown
        /// </summary>
        Never,
        /// <summary>
        /// shown before navigation
        /// </summary>
        Left,
        /// <summary>
        /// shown after navigation
        /// </summary>
        Right
    }

    /// <summary>
    /// specify page button type
    /// </summary>
    public enum PagingButtonType : byte
    {
        /// <summary>
        /// text
        /// </summary>
        Text,
        /// <summary>
        /// image
        /// </summary>
        Image
    }


    #endregion

    #region AspNetPager Control Designer
    /// <summary>
    /// <see cref="AspNetPager"/> designer
    /// </summary>
    public class PagerDesigner : System.Web.UI.Design.ControlDesigner
    {
        public PagerDesigner()
        {

        }
        private GridPager gp;

        /// <summary>
        /// override GetDesignTimeHtml
        /// display designer
        /// </summary>
        /// <returns></returns>
        public override string GetDesignTimeHtml()
        {
            this.gp = (GridPager)base.Component;
            this.gp.RecordCount = 0xe1;
            StringWriter writer1 = new StringWriter();
            HtmlTextWriter writer2 = new HtmlTextWriter(writer1);
            this.gp.RenderControl(writer2);
            return writer1.ToString();
        }

        /// <summary>
        /// Region can't select
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public override string GetEditableDesignerRegionContent(System.Web.UI.Design.EditableDesignerRegion region)
        {
            region.Selectable = false;
            return null;
        }

        /// <summary>
        /// override GetErrorDesignTimeHtml
        /// display errors while creating control
        /// </summary>
        /// <param name="e">exception</param>
        /// <returns>html error</returns>
        protected override string GetErrorDesignTimeHtml(Exception e)
        {
            string errorstr = "Errors while creating control:" + e.Message;
            return base.CreatePlaceHolderDesignTimeHtml(errorstr);
        }

    }
    #endregion
}
