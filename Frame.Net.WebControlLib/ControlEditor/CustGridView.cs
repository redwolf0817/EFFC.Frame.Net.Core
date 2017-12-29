using System;
using System.Web.UI;
using System.ComponentModel;
using System.Security.Permissions;
using System.Web;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Web.UI.HtmlControls;

namespace EFFC.Frame.Net.WebControlLib
{
    /// <summary>
    /// Expand Gridview control
    /// </summary>
    [
        AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal),
        AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal),
        ToolboxData("<{0}:CustGridView runat=\"server\"> </{0}:CustGridView>"),
        ToolboxBitmapAttribute(typeof(GridView))
   ]
    public class CustGridView : GridView
    {
        private int CheckBoxIndex = 0;//CheckBox放置的位置
        private int ItemNoIndex = 0;//ItemNo放置的位置
        public HiddenField hdChkSno = new HiddenField();
        private HtmlInputCheckBox CheckboxAll = new HtmlInputCheckBox();

        #region 屬性

        /// <summary>
        /// is or isn't show header all way
        /// </summary>
        [Category("Custom")]
        [Browsable(true)]
        [Description("ShowHeaderAllways")]
        [DefaultValue(true)]
        public bool ShowHeaderAllways
        {
            get
            {
                object obj = ViewState["ShowHeaderAllways"];
                if (obj != null)
                    return (bool)obj;

                return true;
            }
            set { ViewState["ShowHeaderAllways"] = value; }
        }

        private string _CssClass = "";
        /// <summary>
        /// Name of CssClass
        /// </summary>
        [Category("Custom"),
         Browsable(true),
         Description("Name of CssClass"),
         DefaultValue("")
        ]
        public override string CssClass
        {
            get
            {
                return this._CssClass;
            }
            set
            {
                this._CssClass = value;
            }
        }

        private bool _AllowPaging = true;
        /// <summary>
        /// Does paging function open
        /// </summary>
        [Category("Custom"),
         Browsable(true),
         Description("Does paging function open"),
        DefaultValue(true)
        ]
        public override bool AllowPaging
        {
            get
            {
                return this._AllowPaging;
            }
            set
            {
                this._AllowPaging = value;
            }
        }

        private bool _AutoGenerateColumns = false;
        /// <summary>
        /// Auto Generation Columns
        /// </summary>
        [Category("Custom"),
        Browsable(true),
        Description("AutoGenerationColumns"),
        DefaultValue(false)
        ]
        public override bool AutoGenerateColumns
        {
            get
            {
                if (ViewState["AutoGenerateColumns"] == null)
                    return false;
                return Convert.ToBoolean(ViewState["AutoGenerateColumns"]);
            }
            set
            {
                ViewState["AutoGenerateColumns"] = value;
            }
        }

        private string _SNWidth = "5%";
        /// <summary>
        /// Width of SeqNo column
        /// </summary>
        [Category("Custom"),
        Browsable(true),
        Description("Width of SeqNo column"),
        DefaultValue("5%")
        ]
        public string SNWidth
        {
            get
            {
                return this._SNWidth;
            }
            set
            {
                this._SNWidth = value;
            }
        }

        private bool _PagerVisible = false;
        /// <summary>
        /// is or isn't visible pager
        /// </summary>
        [Category("Custom"),
        Browsable(true),
        Description("PagerVisible"),
        DefaultValue(false)
        ]
        public bool PagerVisible
        {
            get
            {
                return this._PagerVisible;
            }
            set
            {
                this._PagerVisible = value;
            }
        }

        /// <summary>
        /// asc image URL
        /// </summary>
        [
        Description("ASC"),
        Category("order"),
        Editor("System.Web.UI.Design.UrlEditor", typeof(System.Drawing.Design.UITypeEditor)),
        DefaultValue(""),
        ]
        public string SortAscImageUrl
        {
            get
            {
                object o = ViewState["SortImageAsc"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["SortImageAsc"] = value;
            }
        }
        /// <summary>
        /// Desc 
        /// </summary>
        [
        Description("Desc"),
        Category("order"),
        Editor("System.Web.UI.Design.UrlEditor", typeof(System.Drawing.Design.UITypeEditor)),
        DefaultValue(""),
        ]
        public string SortDescImageUrl
        {
            get
            {
                object o = ViewState["SortImageDesc"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["SortImageDesc"] = value;
            }
        }

        [Category("自定義屬性")]
        [Browsable(true)]
        [Description("获取或设置是否顯示CheckBox列")]
        [DefaultValue(0)]
        public bool ShowCheckBox
        {
            get
            {
                if (ViewState["ShowCheckBox"] == null)
                    return false;
                return Convert.ToBoolean(ViewState["ShowCheckBox"]);
            }
            set
            {
                ViewState["ShowCheckBox"] = value;
            }
        }

        [Category("自定義屬性")]
        [Browsable(true)]
        [Description("获取或设置是否顯示ItemNo列")]
        [DefaultValue(0)]
        public bool ShowItemNo
        {
            get
            {
                if (ViewState["ShowItemNo"] == null)
                    return false;
                return Convert.ToBoolean(ViewState["ShowItemNo"]);
            }
            set
            {
                ViewState["ShowItemNo"] = value;
            }
        }

        [Category("自定義屬性")]
        [Browsable(true)]
        [Description("获取或设置數據總行數")]
        [DefaultValue(0)]
        public int RecordCount
        {
            get
            {
                object o = ViewState["RecordCount"];
                return o == null ? 0 : Convert.ToInt32(o);
            }
            set
            {
                ViewState["RecordCount"] = value;
            }
        }

        /// <summary>
        /// 翻頁控件當前頁碼
        /// </summary>
        [Category("自定義屬性"),
        Browsable(true),
        Description("翻頁控件當前頁碼"),
        DefaultValue(false)
        ]
        public int PageIndex
        {
            get
            {
                object o = ViewState["PageIndex"];
                return o == null ? 1 : Convert.ToInt32(o);
            }
            set
            {
                ViewState["PageIndex"] = value;
            }
        }
        [Category("CheckBox列寬度")]
        [Browsable(true)]
        [Description("获取或设置CheckBox列寬度")]
        [DefaultValue(0)]
        public string CheckBoxWith
        {
            get
            {
                if (ViewState["CheckBoxWith"] == null)
                    return "40px";
                return ViewState["CheckBoxWith"].ToString();
            }
            set
            {
                ViewState["CheckBoxWith"] = value;
            }
        }
        /// <summary>
        /// 翻頁控件每頁顯示筆數
        /// </summary>
        [Category("自定義屬性"),
        Browsable(true),
        Description(" 翻頁控件每頁顯示筆數"),
        DefaultValue(false)
        ]
        public int PageZize
        {
            get
            {
                object o = ViewState["PageSize"];
                return o == null ? 10 : Convert.ToInt32(o);
            }
            set
            {
                ViewState["PageSize"] = value;
            }
        }

        private bool _hasUpdatePanel = true;
        /// <summary>
        /// Does paging has UpdatePanel
        /// </summary>
        [Category("自定義屬性"),
         Browsable(true),
         Description("Does paging has UpdatePanel"),
        DefaultValue(true)
        ]
        public bool HasUpdatePanel
        {
            get
            {
                return this._hasUpdatePanel;
            }
            set
            {
                this._hasUpdatePanel = value;
            }
        }
        #endregion

        #region Override Function
        /// <summary>
        /// Override OnInit method
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this._CssClass.Equals("") == false)
                this.Attributes.Add("class", this._CssClass);
        }

        /// <summary>
        /// Override Render
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            this.CreateHeaderRow();
        }

        /// <summary>
        /// Override When Row create ,if select colume is ordered,then then show image of order on colume's header   
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowCreated(GridViewRowEventArgs e)
        {
            
            hdChkSno.Value = "";
            if (e.Row.RowType == DataControlRowType.Header)
            {
                CheckBoxIndex = 0;//CheckBox放置的位置
                ItemNoIndex = 0; //ItemNo放置的位置
                //是否显示CheckBox列
                if (this.ShowCheckBox)
                {
                    TableHeaderCell CheckBoxCell = new TableHeaderCell();
                    //CheckBoxCell.Attributes.Add("onclick", "javascript:TdSelectAllCheckboxes_" + this.ClientID + "('" + this.ClientID + "',$(this).find('input[type=checkbox]'),event);");
                    CheckBoxCell.Style.Add("cursor", "pointer");
                    CheckBoxCell.Style.Add("width", CheckBoxWith);
                    CheckboxAll.Attributes.Add("onclick", "javascript:SelectAllCheckboxes_" + this.ClientID + "('" + this.ClientID + "',this);");
                    CheckboxAll.ID = "CheckboxAll";
                    CheckboxAll.Attributes.Add("class", "chkAClass");
                    CheckboxAll.Attributes.Add("hidefocus","true");
                    CheckBoxCell.Controls.Add(CheckboxAll);
                    Label lb = new Label();
                    lb.Text = "全選";
                    //CheckBoxCell.Controls.Add(lb);
                    CheckBoxCell.Controls.Add(hdChkSno);
                    hdChkSno.ID = "hdChkSno";

                    e.Row.Cells.AddAt(CheckBoxIndex, CheckBoxCell);
                    //e.Row.Cells[0].Style.Add("width", "1%");
                    e.Row.Cells[0].Style.Add("text-align", "Center");
                    CheckboxAll.Checked = false;
                    ItemNoIndex = 1;
                }

                //是否显示ItemNo列
                if (this.ShowItemNo)
                {
                    TableHeaderCell CheckBoxCell = new TableHeaderCell();
                    Label lb = new Label();
                    lb.Text = "Item No.";
                    lb.Attributes.Add("class", "itemno");
                    CheckBoxCell.Style.Add("width", "60px");
                    CheckBoxCell.Controls.Add(lb);
                    e.Row.Cells.AddAt(ItemNoIndex, CheckBoxCell);
                    CheckboxAll.Checked = false;
                }

                if (SortExpression != String.Empty)
                {
                    DisplaySortOrderImages(SortExpression, e.Row);
                    this.CreateRow(0, 0, DataControlRowType.EmptyDataRow, DataControlRowState.Normal);
                }
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
                if (this.ShowCheckBox)
                {
                    TableCell CheckBoxCell = new TableCell();
                    HtmlInputCheckBox check = new HtmlInputCheckBox();
                    check.ID = "check";
                    check.Attributes.Add("class", "chkClass");
                    check.Attributes.Add("hidefocus", "true");
                    check.Attributes["OnClick"] = "AddRemoveValues_" + this.ClientID + "(this)";
                    CheckBoxCell.Style.Add("cursor", "pointer");
                    CheckBoxCell.Style.Add("text-align", "center");
                    CheckBoxCell.Style.Add("width", CheckBoxWith);
                    //CheckBoxCell.Attributes["OnClick"] = "TdAddRemoveValues_" + this.ClientID + "($(this).find('.chkClass, input[type=checkbox]'),event)";
                    CheckBoxCell.Controls.Add(check);
                    e.Row.Cells.AddAt(CheckBoxIndex, CheckBoxCell);
                    //e.Row.Cells[0].Style.Add("width", "1%");
                    e.Row.Cells[0].Style.Add("text-align", "Center");
                    ItemNoIndex = 1;
                }
                //是否显示ItemNo列
                int intBeginIndex = (this.PageIndex - 1) * PageZize + 1;
                if (this.ShowItemNo)
                {
                    TableCell CheckBoxCell = new TableCell();
                    Label lb = new Label();
                    lb.ID = "labItemNo";
                    lb.Attributes.Add("class", "itemno");
                    lb.Text = (intBeginIndex + e.Row.RowIndex).ToString();
                    CheckBoxCell.Style.Add("text-align", "center");
                    CheckBoxCell.Style.Add("width", "60px");
                    CheckBoxCell.Controls.Add(lb);
                    e.Row.Cells.AddAt(ItemNoIndex, CheckBoxCell);
                }

            }
            base.OnRowCreated(e);
        }
        #endregion

        #region 綁定排序表達式和排序順序
        /// <summary>
        /// bind sort
        /// </summary>
        public void BindSort(GridViewSortEventArgs e)
        {
            if (SortExpression == e.SortExpression)
            {
                if (SortDirection == SortDirection.Ascending)
                {
                    ViewState["SortDirection"] = SortDirection.Descending;
                }
                else
                {
                    ViewState["SortDirection"] = SortDirection.Ascending;
                }
            }
            else
            {
                ViewState["SortDirection"] = SortDirection.Ascending;
            }
            ViewState["SortExpression"] = e.SortExpression;
           
        }
        /// <summary>
        /// Sort Expression
        /// </summary>
        new public string SortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                {
                    ViewState["SortExpression"] = "";
                }
                return ViewState["SortExpression"].ToString();
            }
            set
            {
                ViewState["SortExpression"] = value;
            }
        }

        /// <summary>
        /// Sort Direction
        /// </summary>
        new public SortDirection SortDirection
        {
            get
            {
                if (ViewState["SortDirection"] == null)
                {
                    ViewState["SortDirection"] = SortDirection.Ascending;
                }
                return (SortDirection)ViewState["SortDirection"];
            }
            set
            {
                ViewState["SortDirection"] = (SortDirection)value;
            }
        }

        /// <summary>
        /// String of sort direction
        /// </summary>
        public string strSortDirection
        {
            get
            {
                if (ViewState["SortDirection"] != null
                    && (SortDirection)ViewState["SortDirection"] == SortDirection.Descending)
                {
                    return "DESC";
                }
                else
                {
                    return "ASC";
                }

            }
        }

        /// <summary>
        ///  Display sort order images
        /// </summary>
        protected void DisplaySortOrderImages(string sortExpression, GridViewRow dgItem)
        {
            string[] sortColumns = sortExpression.Split(",".ToCharArray());

            for (int i = 0; i < dgItem.Cells.Count; i++)
            {
                if (dgItem.Cells[i].Controls.Count > 0 && dgItem.Cells[i].Controls[0] is LinkButton)
                {
                    string sortOrder;
                    int sortOrderNo;
                    string column = ((LinkButton)dgItem.Cells[i].Controls[0]).CommandArgument;
                    SearchSortExpression(sortColumns, column, out sortOrder, out sortOrderNo);
                    if (sortOrderNo > 0)
                    {
                        string sortImgLoc = (sortOrder.Equals("ASC") ? SortAscImageUrl : SortDescImageUrl);

                        if (sortImgLoc != String.Empty)
                        {
                            System.Web.UI.WebControls.Image imgSortDirection = new System.Web.UI.WebControls.Image();
                            imgSortDirection.ImageUrl = sortImgLoc;
                            dgItem.Cells[i].Controls.Add(imgSortDirection);

                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Search sort expression
        /// </summary>
        protected void SearchSortExpression(string[] sortColumns, string sortColumn, out string sortOrder, out int sortOrderNo)
        {
            sortOrder = "";
            sortOrderNo = -1;
            for (int i = 0; i < sortColumns.Length; i++)
            {
                if (sortColumns[i].StartsWith(sortColumn))
                {
                    sortOrderNo = i + 1; 
                    sortOrder = ((SortDirection == SortDirection.Ascending) ? "ASC" : "DESC");
                }
            }
        }

        /// <summary>
        /// Create header row
        /// </summary>
        private void CreateHeaderRow()
        {
            if (ShowHeaderAllways)
            {
                Table maintable = null;
                if (this.Controls.Count == 0)
                {
                    maintable = new Table();
                    maintable.ApplyStyle(this.ControlStyle);
                    this.Controls.Add(maintable);
                }
                else
                    maintable = this.Controls[0] as Table;

                bool IsCreateHeader = false;

                bool IsCreateEmptyRow = false;

                if (maintable.Rows.Count == 0)
                {
                    IsCreateHeader = true;
                    IsCreateEmptyRow = true;
                }
                else
                {
                    GridViewRow gvr = maintable.Rows[0] as GridViewRow;
                    if (gvr.RowType == DataControlRowType.EmptyDataRow)
                    {
                        maintable.Rows.Clear();
                        IsCreateHeader = true;
                        IsCreateEmptyRow = true;
                    }
                    else
                    {
                        IsCreateHeader = false;
                        IsCreateEmptyRow = false;
                    }
                }

                int ColumnCount = 0;

                if (IsCreateHeader)
                {
                    GridViewRow gvr = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
                    gvr.ApplyStyle(this.HeaderStyle);
                    for (int i = 0; i < this.Columns.Count; i++)
                    {
                        DataControlField column = this.Columns[i];

                        if (column.ShowHeader)
                        {
                            ColumnCount++;
                            DataControlFieldHeaderCell tc = new DataControlFieldHeaderCell(column);
                            tc.ApplyStyle(column.HeaderStyle);
                            column.InitializeCell(tc, DataControlCellType.Header, DataControlRowState.Normal, 0);
                            gvr.Cells.Add(tc);
                        }
                    }
                    maintable.Rows.AddAt(0, gvr);
                }

                if (IsCreateEmptyRow)
                {
                    if (this.EmptyDataText != string.Empty)
                    {
                        GridViewRow gvr = new GridViewRow(0, 0, DataControlRowType.EmptyDataRow, DataControlRowState.Normal);
                        gvr.ApplyStyle(this.EmptyDataRowStyle);
                        TableCell tc = new TableCell();
                        tc.Text = this.EmptyDataText;
                        tc.ColumnSpan = ColumnCount;

                        gvr.Cells.Add(tc);

                        maintable.Rows.Add(gvr);
                    }
                }
            }
        }
        #endregion

        #region 重寫OnRowDataBound
        /// <summary>
        /// Override OnRowDataBound   
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowDataBound(GridViewRowEventArgs e)
        {
            base.OnRowDataBound(e);
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //如果显示CheckBox列
                if (this.ShowCheckBox)
                {
                    HtmlInputCheckBox check = (HtmlInputCheckBox)e.Row.Cells[CheckBoxIndex].FindControl("check");
                    check.Value = DataKeys[e.Row.RowIndex].Value.ToString();
                    
                }
            }
        }
        #endregion

        #region 重寫RenderContents
        /// <summary>
        /// Override RenderContents   
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {

            base.RenderContents(writer);

            if (this.ShowCheckBox)
            {
                if (this.HasUpdatePanel)
                {
                    string strJs = "";
                    strJs += "Array.prototype.del=function(n) { if(n<0) return this; else　return this.slice(0,n).concat(this.slice(n+1,this.length));} \n";
                    strJs += " var " + this.ClientID + "_allSno='';";
                    strJs += "function SelectAllCheckboxes_" + this.ClientID + "(mainId, chkA) {  if (chkA.checked) {";
                    strJs += " $('#'+mainId).find('input[type=checkbox]').filter('.chkClass').each(function () { if ($(this).attr('disabled') != 'disabled') {this.checked = true;} });";

                    strJs += "var selectvalue='';";
                    strJs += "$('#'+mainId).find('input[type=checkbox]').filter('.chkClass').each(function () { if ($(this).attr('disabled') != 'disabled') {selectvalue=selectvalue+this.value+',';} });";
                    strJs += "$('#" + hdChkSno.ClientID + "').val(substrValue(selectvalue));";
                    strJs += "    }";
                    strJs += "else {";
                    strJs += "   $('#'+mainId).find('input[type=checkbox]').filter('.chkClass').each(function () {  if ($(this).attr('disabled') != 'disabled') {this.checked = false;} });";
                    strJs += " $('#" + hdChkSno.ClientID + "').val('');";
                    strJs += "     }  ";
                    strJs += " }";
                    strJs += "function AddRemoveValues_" + this.ClientID + "(oChk) { ";
                    strJs += "var arr = new Array();if($('#" + hdChkSno.ClientID + "').val()!='')";
                    strJs += "{ arr=$('#" + hdChkSno.ClientID + "').val().split(',');} ";
                    strJs += "var strValue = '';";
                    strJs += "  if(oChk.checked)    { arr.unshift(oChk.value);strValue = arr.join(','); ";
                    strJs += "  var count= $('#" + this.ClientID + " tr').length-1;";
                    strJs += " if ( arr.length== count) {";
                    strJs += "  $('#" + CheckboxAll.ClientID + "').each(function () {this.checked = true;});}";
                    strJs += "              $('#" + hdChkSno.ClientID + "').val(substrValue(strValue)); ";
                    strJs += "         }";
                    strJs += "  else    {";
                    strJs += " $('#" + CheckboxAll.ClientID + "').each(function () {this.checked = false;});";
                    strJs += " $.each(arr, function (i, value) { if (value == oChk.value) { arr=arr.del(i); return; } });";
                    strJs += " strValue = arr.join(',');";
                    strJs += " $('#" + hdChkSno.ClientID + "').val(substrValue(strValue)); ";
                    strJs += "      }} ";

                    strJs += " function substrValue(strValue) {";
                    strJs += " var strend=strValue.substr(strValue.length-1,1);";
                    strJs += " if(strend==','){strValue=strValue.substring(0,strValue.length-1);}";
                    strJs += " return strValue;}";

                    strJs += " function TdSelectAllCheckboxes_" + this.ClientID + "(mainId,chkA,evt) {";
                    strJs += "  var e = evt || window.event; var evg = e.srcElement || e.target;";
                    strJs += "if (evg.tagName != 'INPUT') {";
                    strJs += " if ($(chkA).attr('checked')) { $(chkA).attr('checked',false); }";
                    strJs += " else { $(chkA).attr('checked', true); } SelectAllCheckboxes_" + this.ClientID + "(mainId,$(chkA).get(0));}}";

                    strJs += " function TdAddRemoveValues_" + this.ClientID + "(chk,evt) {if ($(chk).attr('disabled') == 'disabled') {return false;}";
                    strJs += "  var e = evt || window.event; var evg = e.srcElement || e.target;";
                    strJs += "if (evg.tagName != 'INPUT') {";
                    strJs += " if ($(chk).attr('checked')) { $(chk).attr('checked',false); }";
                    strJs += " else { $(chk).attr('checked', true); } AddRemoveValues_" + this.ClientID + "($(chk).get(0));}}";
                    ScriptManager.RegisterStartupScript(this.Page, GetType(), "CheckBoxJs" + this.ClientID, strJs, true);
                }
                else
                {
                    writer.Write("\n<SCRIPT language=JavaScript>\n");
                    writer.Write("Array.prototype.del=function(n) { if(n<0) return this; else　return this.slice(0,n).concat(this.slice(n+1,this.length));} \n");
                    writer.Write(" var " + this.ClientID + "_allSno='';");
                    writer.Write("function SelectAllCheckboxes_" + this.ClientID + "(mainId, chkA) {  if (chkA.checked) {");
                    writer.Write(" $('#'+mainId).find('input[type=checkbox]').filter('.chkClass').attr('checked', true);");
                    writer.Write("var selectvalue='';");
                    writer.Write("$('#'+mainId).find('input[type=checkbox]').filter('.chkClass').each(function () { selectvalue=selectvalue+this.value+','; });");
                    writer.Write("$('#" + hdChkSno.ClientID + "').val(substrValue(selectvalue));");
                    writer.Write("    }");
                    writer.Write("else {");
                    writer.Write("   $('#'+mainId).find('input[type=checkbox]').filter('.chkClass').attr('checked', false);");
                    writer.Write(" $('#" + hdChkSno.ClientID + "').val('');");
                    writer.Write("     }  ");
                    writer.Write(" }");

                    writer.Write("function AddRemoveValues_" + this.ClientID + "(oChk) { ");
                    writer.Write("var arr = new Array();if($('#" + hdChkSno.ClientID + "').val()!='')");
                    writer.Write("{ arr=$('#" + hdChkSno.ClientID + "').val().split(',');} ");
                    writer.Write("var strValue = '';");
                    writer.Write("  if(oChk.checked)    { arr.unshift(oChk.value);strValue = arr.join(','); ");
                    writer.Write("  var count= $('#" + this.ClientID + " tr').length-1;");
                    writer.Write(" if ( arr.length== count) {");
                    writer.Write("  $('#" + CheckboxAll.ClientID + "').attr('checked',true);}");
                    writer.Write("              $('#" + hdChkSno.ClientID + "').val(substrValue(strValue)); ");
                    writer.Write("         }");
                    writer.Write("  else    {");
                    writer.Write(" $('#" + CheckboxAll.ClientID + "').attr('checked',false);");
                    writer.Write(" $.each(arr, function (i, value) { if (value == oChk.value) { arr=arr.del(i); return; } });");
                    writer.Write(" strValue = arr.join(',');");
                    writer.Write(" $('#" + hdChkSno.ClientID + "').val(substrValue(strValue)); ");
                    writer.Write("      }} ");

                    writer.Write(" function substrValue(strValue) {");
                    writer.Write(" var strend=strValue.substr(strValue.length-1,1);");
                    writer.Write(" if(strend==','){strValue=strValue.substring(0,strValue.length-1);}");
                    writer.Write(" return strValue;}");

                    writer.Write(" function TdSelectAllCheckboxes_" + this.ClientID + "(mainId,chkA,evt) {");
                    writer.Write("  var e = evt || window.event; var evg = e.srcElement || e.target;");
                    writer.Write("if (evg.tagName != 'INPUT') {");
                    writer.Write(" if ($(chkA).attr('checked')) { $(chkA).attr('checked',false); }");
                    writer.Write(" else { $(chkA).attr('checked', true); } SelectAllCheckboxes_" + this.ClientID + "(mainId,$(chkA).get(0));}}");

                    writer.Write(" function TdAddRemoveValues_" + this.ClientID + "(chk,evt) {");
                    writer.Write("  var e = evt || window.event; var evg = e.srcElement || e.target;");
                    writer.Write("if (evg.tagName != 'INPUT') {");
                    writer.Write(" if ($(chk).attr('checked')) { $(chk).attr('checked',false); }");
                    writer.Write(" else { $(chk).attr('checked', true); } AddRemoveValues_" + this.ClientID + "($(chk).get(0));}}");
                    writer.Write(" </SCRIPT>");
                }
            }
        }
        #endregion
    }
}
