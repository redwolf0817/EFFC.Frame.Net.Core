
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web;
using System.ComponentModel;
using System.Collections.Specialized;

namespace EFFC.Frame.Net.WebControlLib
{
    /// <summary>
    /// Pager by Database
    /// </summary>
    public class GridViewPageingByDB : GridView, IPostBackEventHandler, IPostBackDataHandler
    {
        /// <summary>
        /// Delegate on set data source
        /// </summary>
        public event OnSetDataSourceDelegate OnSetDataSource = default(OnSetDataSourceDelegate);
        /// <summary>
        /// List of checkboxs on the page
        /// </summary>
        private List<HtmlInputCheckBox> checkBoxes = new List<HtmlInputCheckBox>();
        /// <summary>
        /// Build
        /// </summary>
        public GridViewPageingByDB()
            : base()
        {
            this.RowDataBound += new GridViewRowEventHandler(GridViewPageingByDB_RowDataBound);
            editButtonText = "Edit";
            editPageUrl = "#";
            this.Load += new EventHandler(GridViewPageingByDB_Load);
            base.AllowSorting = false;
            base.PagerSettings.Visible = false;
            base.AllowPaging = false;
            this.Sorting += new GridViewSortEventHandler(GridViewPageingByDB_Sorting);
            this.NextPageText = "<";
            this.PreviousPageText = ">";
            this.FirstPageText = "<<";
            this.LastPageText = ">>";
        }

        /// <summary>
        /// Sorting method
        /// </summary>
        void GridViewPageingByDB_Sorting(object sender, GridViewSortEventArgs e)
        {
            SetData();
        }

        /// <summary>
        /// RowDataBound
        /// </summary>
        void GridViewPageingByDB_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!DesignMode)
            {
                if (DataSource != null && ((DataSource.GetType() == typeof(DataSet) && DataMember != null && DataMember != string.Empty && DataMember != "") || DataSource.GetType() != typeof(DataSet)))
                {

                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        if (blnHasDataRow)
                        {

                            DataRowView Rv = (DataRowView)e.Row.DataItem;

                            if (showCheckBox && blnHasDataRow)
                            {
                                if (this.DataKeyNames != null && this.DataKeyNames.Length <= 0)
                                {
                                    throw new Exception("please set " + this.ID + " DataKeyNames");
                                }
                                else
                                {
                                    string idValue = "";
                                    for (int index = 0; index < this.DataKeyNames.Length; index++)
                                    {
                                        if (Rv[this.DataKeyNames[index].ToString()].GetType() == typeof(DateTime))
                                        {
                                            idValue += Convert.ToDateTime(Rv[this.DataKeyNames[index].ToString()]).ToString("yyyy-MM-dd HH:mm:ss.fff") + ";";
                                        }
                                        else
                                        {
                                            idValue += Rv[this.DataKeyNames[index].ToString()].ToString().Trim() + ";";
                                        }
                                    }
                                    idValue = idValue.Substring(0, idValue.Length - 1);
                                    if (checkRowNumber)
                                    {
                                        idValue += ";" + e.Row.RowIndex.ToString();
                                    }
                                    if (selectedIDList.Contains(idValue))
                                    {
                                        e.Row.Cells[0].Text = e.Row.Cells[0].Text.Replace("value=\"\"", "value=\"" + idValue + "\" checked=\"checked\"");
                                    }
                                    else
                                    {

                                        e.Row.Cells[0].Text = e.Row.Cells[0].Text.Replace("value=\"\"", "value=\"" + idValue + "\"");

                                    }
                                }
                            }

                            if (showEditButton && blnHasDataRow)
                            {
                                if (this.DataKeyNames != null && this.DataKeyNames.Length <= 0)
                                {
                                    throw new Exception("please set " + this.ID + " DataKeyNames");
                                }
                                else
                                {
                                    string idValue = "";
                                    for (int index = 0; index < this.DataKeyNames.Length; index++)
                                    {
                                        if (Rv[this.DataKeyNames[index].ToString()].GetType() == typeof(DateTime))
                                        {
                                            idValue += Convert.ToDateTime(Rv[this.DataKeyNames[index].ToString()]).ToString("yyyy-MM-dd HH:mm:ss.fff") + ";";
                                        }
                                        else
                                        {
                                            idValue += Rv[this.DataKeyNames[index].ToString()].ToString().Trim() + ";";
                                        }
                                    }
                                    idValue = idValue.Substring(0, idValue.Length - 1);
                                    if (checkRowNumber)
                                    {
                                        idValue += "';" + e.Row.RowIndex.ToString();
                                    }
                                    editPageUrlParameters = (editPageUrlParameters == string.Empty) ? "?" + this.DataKeyNames[0].ToString() + "=" : editPageUrlParameters;
                                    HtmlAnchor a = (HtmlAnchor)e.Row.FindControl(this.ID + "_Edit");
                                    a.HRef = editPageUrl + editPageUrlParameters + base.Page.Server.UrlEncode(idValue);
                                }
                            }
                        }
                        else
                        {
                            for (int index = 0; index < e.Row.Cells.Count; index++)
                            {
                                foreach (Control c in e.Row.Cells[index].Controls)
                                {
                                    c.Visible = false;
                                }

                            }
                        }
                    }

                }//
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

        }

        void GridViewPageingByDB_Load(object sender, EventArgs e)
        {

            StringBuilder scriptBuider = new StringBuilder();
            scriptBuider.AppendLine("<script type=\"text/javascript\">");
            if (showCheckBox)
            {
                scriptBuider.AppendLine(" function DoGridViewSelAll(Obj)");
                scriptBuider.AppendLine("{");
                scriptBuider.AppendLine("   var tablerows=Obj.parentNode.parentNode.parentNode.rows;");
                scriptBuider.AppendLine("   for(var rowindex=1;rowindex<tablerows.length;rowindex++)");
                scriptBuider.AppendLine("   {");
                scriptBuider.AppendLine("      if(tablerows[rowindex].cells[0].hasChildNodes() && tablerows[rowindex].cells[0].childNodes[0].nodeName=='INPUT' && tablerows[rowindex].cells[0].childNodes[0].attributes['type'].value=='checkbox')");
                scriptBuider.AppendLine("      {");
                scriptBuider.AppendLine("         tablerows[rowindex].cells[0].childNodes[0].checked=Obj.checked;");
                scriptBuider.AppendLine("      }");
                scriptBuider.AppendLine("   }");
                scriptBuider.AppendLine("}");
                scriptBuider.AppendLine("function DoGridViewSel(Obj)");
                scriptBuider.AppendLine("{");
                scriptBuider.AppendLine("   var tablerows=Obj.parentNode.parentNode.parentNode.rows;");
                scriptBuider.AppendLine("   if(!Obj.checked)");
                scriptBuider.AppendLine("   {");
                scriptBuider.AppendLine("      tablerows[0].cells[0].childNodes[0].checked=false;");
                scriptBuider.AppendLine("   }");
                scriptBuider.AppendLine("}");
            }
            scriptBuider.AppendLine("function GridViewPageingByDB_GoPage(btn)");
            scriptBuider.AppendLine("{");
            scriptBuider.AppendLine("var hidPageNumber=btn.previousSibling.value;");
            scriptBuider.AppendLine("var txtPageNumber=btn.previousSibling.previousSibling.value;");
            scriptBuider.AppendLine("var re=new RegExp('^[1-9]+[0-9]*$','g');");
            scriptBuider.AppendLine("if(txtPageNumber.match(re))");
            scriptBuider.AppendLine("{");
            scriptBuider.AppendLine("if(Number(txtPageNumber)>Number(hidPageNumber))");
            scriptBuider.Append("{alert('");
            scriptBuider.Append(strPageNumberTooLong);
            scriptBuider.AppendLine("');}");
            scriptBuider.AppendLine("else");
            scriptBuider.AppendLine("{");
            scriptBuider.Append("__doPostBack('");
            scriptBuider.Append(this.ID);
            scriptBuider.AppendLine("_NumberButton',txtPageNumber);");
            scriptBuider.AppendLine("}");
            scriptBuider.AppendLine("}");
            scriptBuider.AppendLine("else");
            scriptBuider.Append("{alert('");
            scriptBuider.Append(strPageNumberErr);
            scriptBuider.AppendLine("');}");
            scriptBuider.AppendLine("}");
            scriptBuider.Append("</script>");
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "SuperGridView_SelScript"))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SuperGridView_SelScript", scriptBuider.ToString(), false);
            }

            if (Page.IsPostBack)
            {
                if (showCheckBox)
                {
                    selectedIDS = Page.Request.Form[this.ID + "_Sel"];
                    if (selectedIDS != string.Empty && selectedIDS != "" && selectedIDS != null)
                    {
                        string[] selectedIDSArray = selectedIDS.Split(",".ToCharArray());
                        selectedIDList.Clear();
                        for (int index = 0; index < selectedIDSArray.Length; index++)
                        {
                            selectedIDList.Add(selectedIDSArray[index]);
                        }
                    }
                }
                if (this.AllowPaging)
                {
                    string PostObject = Page.Request.Form["__EVENTTARGET"];
                    if (PostObject != null)
                    {
                        bool blnPageChange = false;
                        if (PostObject == this.ID + "_FirstButton")
                        {
                            PageIndex = 0;
                            blnPageChange = true;
                        }
                        else if (PostObject == this.ID + "_PrevButton")
                        {
                            PageIndex -= 1;
                            blnPageChange = true;
                        }
                        else if (PostObject == this.ID + "_NextButton")
                        {
                            PageIndex += 1;
                            blnPageChange = true;
                        }
                        else if (PostObject == this.ID + "_LastButton")
                        {
                            PageIndex = PageCount - 1;
                            blnPageChange = true;
                        }
                        else if (PostObject == this.ID + "_NumberButton")
                        {
                            PageIndex = Convert.ToInt32(Page.Request.Form["__EVENTARGUMENT"]) - 1;
                            blnPageChange = true;
                        }
                        if (blnPageChange)
                        {
                            SetData();
                        }
                    }

                }
            }

        }

        protected override void OnDataBinding(EventArgs e)
        {
            SetColumnHeaderTextByLocalResource();
            bool b = base.AllowPaging;
            b = blnAllowPaging;
            base.PageIndex = 0;
            base.OnDataBinding(e);
        }

        protected override void OnSorting(GridViewSortEventArgs e)
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

            base.OnSorting(e);

        }

        public string SelectedIDS { get { return selectedIDS; } }

        private string selectedIDS = string.Empty;

        private List<string> selectedIDList = new List<string>();

        public string EditPageUrl { get { return editPageUrl; } set { editPageUrl = value; } }

        private string editPageUrl;

        public string EditPageUrlParameters { get { return editPageUrlParameters; } set { editPageUrlParameters = value; } }

        private string editPageUrlParameters;


        [DefaultValue(false)]
        public bool ShowEditButton { get { return showEditButton; } set { showEditButton = value; } }

        private bool showEditButton = false;


        [DefaultValue(false)]
        public bool ShowCheckBox { get { return showCheckBox; } set { showCheckBox = value; } }

        private bool showCheckBox = false;


        [DefaultValue(false)]
        public bool CheckRowNumber { get { return checkRowNumber; } set { checkRowNumber = value; } }

        private bool checkRowNumber = false;


        [LocalizableAttribute(true)]
        [DefaultValue("page")]
        public string PageString { get { return strPageString; } set { strPageString = value; } }
        private string strPageString = "page";


        [LocalizableAttribute(true)]
        public string ColumnsHeaderText { get { return columnsHeaderText; } set { columnsHeaderText = value; } }
        private string columnsHeaderText = string.Empty;


        public string EditCellWidth { get { return editCellWidth; } set { editCellWidth = value; } }
        private string editCellWidth = "48px";


        [LocalizableAttribute(true)]
        public string FirstPageText { get { return this.PagerSettings.FirstPageText; } set { this.PagerSettings.FirstPageText = value; } }

        [LocalizableAttribute(true)]
        public string PreviousPageText { get { return this.PagerSettings.PreviousPageText; } set { this.PagerSettings.PreviousPageText = value; } }


        private string strPageNumberTooLong = "input page index more than max！";

        [LocalizableAttribute(true)]
        [DefaultValue("input page index more than max！")]
        public string PageNumberTooLong { get { return strPageNumberTooLong; } set { strPageNumberTooLong = value; } }

        private string strPageNumberErr = "please input right page index";

        [LocalizableAttribute(true)]
        [DefaultValue("please input right page index")]
        public string PageNumberErr { get { return strPageNumberErr; } set { strPageNumberErr = value; } }


        private string strNoneData = "no data";

        [LocalizableAttribute(true)]
        [DefaultValue("no data")]
        public string NoneData { get { return strNoneData; } set { strNoneData = value; } }

        [LocalizableAttribute(true)]
        public string NextPageText { get { return this.PagerSettings.NextPageText; } set { this.PagerSettings.NextPageText = value; } }

        [LocalizableAttribute(true)]
        public string LastPageText { get { return this.PagerSettings.LastPageText; } set { this.PagerSettings.LastPageText = value; } }


        [LocalizableAttribute(true)]
        public string EditButtonText { get { return editButtonText; } set { editButtonText = value; } }

        private string editButtonText;


        public string EditButtonImageUrl { get { return editButtonImageUrl; } set { editButtonImageUrl = value; } }

        private string editButtonImageUrl = string.Empty;


        new public int PageCount
        {
            get
            {
                if (this.ViewState["PageCount"] == null)
                {
                    this.ViewState["PageCount"] = 0;
                }
                int y = (int)this.ViewState["PageCount"];
                return (int)this.ViewState["PageCount"];
            }
        }

        private bool blnHasDataRow = false;


        new public int PageIndex
        {
            get
            {
                if (this.ViewState["PageIndex"] == null)
                {
                    this.ViewState["PageIndex"] = 0;
                }
                return (int)this.ViewState["PageIndex"];

            }
            set
            {
                this.ViewState["PageIndex"] = value;

            }
        }

        private bool blnAllowPaging;

        new public bool AllowPaging
        {
            get
            {
                return blnAllowPaging;
            }
            set
            {
                blnAllowPaging = value;
            }
        }


        new public string SortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                {
                    ViewState["SortExpression"] = "null";
                }
                return ViewState["SortExpression"].ToString();
            }
        }


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
        }


        protected override void InitializeRow(GridViewRow row, DataControlField[] fields)
        {
            base.InitializeRow(row, fields);

            TableCell cell = null;
            switch (row.RowType)
            {
                case DataControlRowType.DataRow:
                    if (showCheckBox && blnHasDataRow)
                    {
                        cell = new TableCell();
                        cell.Width = Unit.Parse("24px");
                        cell.Text = "<input type=\"checkbox\" name=\"" + this.ID + "_Sel\" onclick=\"DoGridViewSel(this);\" value=\"\">";
                        row.Cells.AddAt(0, cell);
                    }
                    if (showEditButton && blnHasDataRow)
                    {
                        cell = new TableCell();
                        cell.HorizontalAlign = HorizontalAlign.Center;
                        HtmlAnchor editLink = new HtmlAnchor();
                        editLink.ID = this.ID + "_Edit";
                        cell.Width = Unit.Parse(editCellWidth);
                        if (editButtonImageUrl != string.Empty)
                        {
                            HtmlImage editImage = new HtmlImage();
                            editImage.Border = 0;
                            editImage.Src = editButtonImageUrl;
                            editLink.Controls.Add(editImage);
                        }
                        else
                        {
                            editLink.InnerHtml = editButtonText;
                        }

                        cell.Controls.Add(editLink);
                        row.Cells.Add(cell);
                    }
                    break;
                case DataControlRowType.Header:
                    TableHeaderCell headerCell = new TableHeaderCell();
                    if (showCheckBox)
                    {

                        headerCell.Width = Unit.Parse("24px");
                        headerCell.Text = "<input type=\"checkbox\" name=\"" + this.ID + "_SelAll\" onclick=\"DoGridViewSelAll(this);\">";
                        row.Cells.AddAt(0, headerCell);
                    }
                    if (showEditButton)
                    {
                        headerCell = new TableHeaderCell();
                        headerCell.Width = Unit.Parse(editCellWidth);
                        headerCell.Wrap = false;
                        if (editButtonImageUrl != string.Empty)
                        {
                            HtmlImage editImage = new HtmlImage();
                            editImage.Border = 0;
                            editImage.Src = editButtonImageUrl;
                            headerCell.Controls.Add(editImage);
                        }
                        else
                        {
                            headerCell.Text = editButtonText;
                        }
                        row.Cells.Add(headerCell);
                    }
                    break;
                default:
                    break;
            }

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            this.CreateHeaderRow();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.PagerSettings.Visible = false;

            writer.Write("<table style=\"width:100%;border-collapse:collapse;\"><tr><td>");
            this.Width = Unit.Parse("100%");
            base.Render(writer);
            this.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            this.RowStyle.HorizontalAlign = HorizontalAlign.Left;
            if (!blnHasDataRow && this.DataSource != null)
            {
                writer.Write("</td></tr><tr><td style=\"font-size:12px;color: #FF0000;\">");
                writer.Write(strNoneData);
                writer.Write("</td></tr></table>");
            }
            else
            {
                if (this.AllowPaging && PageCount > 1)
                {
                    writer.Write("</td></tr><tr><td style=\"font-size:12px;\">");
                    HtmlTable paperTable = new HtmlTable();
                    HtmlTableRow row = new HtmlTableRow();

                    if (this.PageIndex != 0)
                    {
                        if (PagerSettings.FirstPageImageUrl != string.Empty && PagerSettings.FirstPageImageUrl != "")
                        {
                            row.Cells.Add(GetLinkCell(this.PagerSettings.FirstPageImageUrl, "javascript:__doPostBack('" + this.ID + "_FirstButton','Page$First')", true));
                        }
                        else
                        {
                            row.Cells.Add(GetLinkCell(this.PagerSettings.FirstPageText, "javascript:__doPostBack('" + this.ID + "_FirstButton','Page$First')", false));
                        }
                        if (PagerSettings.PreviousPageImageUrl != string.Empty && PagerSettings.PreviousPageImageUrl != "")
                        {
                            row.Cells.Add(GetLinkCell(this.PagerSettings.PreviousPageImageUrl, "javascript:__doPostBack('" + this.ID + "_PrevButton','Page$Prev')", true));
                        }
                        else
                        {
                            row.Cells.Add(GetLinkCell(this.PagerSettings.PreviousPageText, "javascript:__doPostBack('" + this.ID + "_PrevButton','Page$Prev')", false));
                        }
                    }
                    int intFirstIndex = (PageIndex % 10 == 0 ? PageIndex : PageIndex / 10 * 10) + 1;
                    int intLastIndex = intFirstIndex + 10;
                    intLastIndex = (intLastIndex > PageCount ? PageCount : intLastIndex);
                    intFirstIndex = ((intFirstIndex - 1) > 0 ? (intFirstIndex - 1) : intFirstIndex);
                    for (int index = intFirstIndex; index <= intLastIndex; index++)
                    {

                        if (index == this.PageIndex + 1)
                        {
                            HtmlTableCell cell = new HtmlTableCell();
                            cell.InnerHtml = index.ToString();
                            row.Cells.Add(cell);
                        }
                        else
                        {
                            row.Cells.Add(GetLinkCell(index.ToString(), "javascript:__doPostBack('" + this.ID + "_NumberButton','" + index.ToString() + "')", false));
                        }
                    }

                    if (this.PageIndex != this.PageCount - 1)
                    {
                        if (this.PagerSettings.NextPageImageUrl != string.Empty && this.PagerSettings.NextPageImageUrl != "")
                        {
                            row.Cells.Add(GetLinkCell(this.PagerSettings.NextPageImageUrl, "javascript:__doPostBack('" + this.ID + "_NextButton','Page$Next')", true));
                        }
                        else
                        {
                            row.Cells.Add(GetLinkCell(this.PagerSettings.NextPageText, "javascript:__doPostBack('" + this.ID + "_NextButton','Page$Next')", false));
                        }
                        if (this.PagerSettings.LastPageImageUrl != string.Empty && this.PagerSettings.LastPageImageUrl != "")
                        {
                            row.Cells.Add(GetLinkCell(this.PagerSettings.LastPageImageUrl, "javascript:__doPostBack('" + this.ID + "_LastButton','Page$Last')", true));
                        }
                        else
                        {
                            row.Cells.Add(GetLinkCell(this.PagerSettings.LastPageText, "javascript:__doPostBack('" + this.ID + "_LastButton','Page$Last')", false));
                        }
                    }
                    HtmlTableCell htcPageCount = new HtmlTableCell();
                    int intPageNumber = this.PageIndex + 1;
                    htcPageCount.InnerText = " " + intPageNumber.ToString() + "/" + this.PageCount + strPageString + " ";
                    row.Cells.Add(htcPageCount);
                    HtmlTableCell htcPageBox = new HtmlTableCell();
                    HtmlInputText hipPageBox = new HtmlInputText();
                    hipPageBox.Size = 2;
                    hipPageBox.Value = intPageNumber.ToString();
                    HtmlInputHidden hihPageBox = new HtmlInputHidden();
                    hihPageBox.Value = PageCount.ToString();
                    HtmlInputButton hibPageBox = new HtmlInputButton();
                    hibPageBox.Attributes.Add("onclick", "GridViewPageingByDB_GoPage(this);");
                    hibPageBox.Value = "Go";
                    htcPageBox.Controls.Add(hipPageBox);
                    htcPageBox.Controls.Add(hihPageBox);
                    htcPageBox.Controls.Add(hibPageBox);
                    row.Cells.Add(htcPageBox);
                    paperTable.Rows.Add(row);
                    paperTable.RenderControl(writer);
                }

                writer.Write("</td></tr></table>");
            }
        }


        private HtmlTableCell GetLinkCell(string text, string url, bool isImage)
        {
            HtmlTableCell cell = new HtmlTableCell();
            cell.NoWrap = true;
            HtmlAnchor a = new HtmlAnchor();
            if (isImage)
            {
                a.InnerHtml = "<img src=\"" + text + "\" border=\"0\" style=\"border:none;\"/>";
            }
            else
            {
                a.InnerHtml = text;
            }
            a.HRef = url;
            cell.Controls.Add(a);
            return cell;
        }

        


        [
        Description("ASC"),
        Category("DESC"),
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

        [
        Description("DESC"),
        Category("ASC"),
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
        /// <summary>
        /// ShowHeaderAllways
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


        public void SetColumnHeaderTextByLocalResource()
        {
            if (!DesignMode)
            {
                if (columnsHeaderText != string.Empty && columnsHeaderText != "")
                {
                    string[] headerTexts = columnsHeaderText.Split(",".ToCharArray());
                    for (int index = 0; index < this.Columns.Count; index++)
                    {
                        this.Columns[index].HeaderText = headerTexts[index];
                    }
                }
            }
        }

        public void BindData()
        {
            SetData();
        }

        private void SetData()
        {
            if (OnSetDataSource != default(OnSetDataSourceDelegate))
            {
                SetDataSourceEventArgs sdseaEventArgs = new SetDataSourceEventArgs(this.PageIndex, this.PageSize, this.SortExpression, this.SortDirection);
                OnSetDataSource(this, sdseaEventArgs);

                if (sdseaEventArgs.Table != null)
                {
                    if (sdseaEventArgs.Table.Rows.Count < 1 && this.PageIndex > 0)
                    {
                        PageIndex--;
                        sdseaEventArgs = new SetDataSourceEventArgs(PageIndex, this.PageSize, this.SortExpression, this.SortDirection);
                        OnSetDataSource(this, sdseaEventArgs);
                    }

                    if (sdseaEventArgs.Table.Rows.Count < 1)
                    {

                        blnHasDataRow = false;
                    }
                    else
                    {
                        blnHasDataRow = true;
                    }

                    this.DataSource = sdseaEventArgs.Table;
                    this.DataBind();
                    ViewState["PageCount"] = sdseaEventArgs.PageCount;
                    PageIndex = (PageIndex == -2 ? PageCount - 1 : PageIndex);
                    PageIndex = (PageIndex >= PageCount ? PageCount - 1 : PageIndex);
                    PageIndex = (PageIndex < 0 ? 0 : PageIndex);
                }
            }
        }


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

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            //string uniqueID = this.UniqueID;
            //string s = postCollection[uniqueID + ".x"];
            //string str3 = postCollection[uniqueID + ".y"];
            //if (((s != null) && (str3 != null)) && ((s.Length > 0) && (str3.Length > 0)))
            //{
            //    this.x = int.Parse(s);
            //    this.y = int.Parse(str3);
            //    this.Page.RegisterRequiresRaiseEvent(this);
            //}
            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
        }


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
                            Image imgSortDirection = new Image();
                            imgSortDirection.ImageUrl = sortImgLoc;
                            dgItem.Cells[i].Controls.Add(imgSortDirection);

                        }
                    }
                }
            }
        }


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

        protected override void OnRowCreated(GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (SortExpression != String.Empty)
                {
                    DisplaySortOrderImages(SortExpression, e.Row);
                    this.CreateRow(0, 0, DataControlRowType.EmptyDataRow, DataControlRowState.Normal);
                }
            }
            base.OnRowCreated(e);
        }
    }





    public delegate void OnSetDataSourceDelegate(object sender, SetDataSourceEventArgs e);


    public class SetDataSourceEventArgs : EventArgs
    {
        private DataTable dtblTable = null;

        public DataTable Table { get { return dtblTable; } set { dtblTable = value; } }

        private int intPageCount;

        private string strSortExpressionl = "null";

        public string SortExpression { get { return strSortExpressionl; } }

        private string strSortDirection = "ASC";

        public string SortDirection { get { return strSortDirection; } }

        public int PageCount { get { return intPageCount; } }

        private int intRowCount;

        public int RowCount
        {
            get
            {
                return intRowCount;
            }
            set
            {
                intRowCount = value;

                if (value % intPageSize == 0)
                {
                    intPageCount = intRowCount / intPageSize;
                }
                else
                {
                    intPageCount = intRowCount / intPageSize + 1;
                }
            }
        }

        private string strFirstRow;
    
        public string FirstRow { get { return strFirstRow; } }

        private string strLastRow;

        public string LastRow { get { return strLastRow; } }

        private int intPageSize = 10;

        public int PageSize { get { return intPageSize; } }


        public SetDataSourceEventArgs(int pageIndex, int pageSize, string sortExpressionl, SortDirection sortDirection)
        {
            intPageSize = pageSize;
            strSortExpressionl = sortExpressionl;
            strSortDirection = (sortDirection == System.Web.UI.WebControls.SortDirection.Ascending ? "ASC" : "DESC");
            int intFirst = pageIndex * pageSize;
            int intLast = intFirst + pageSize;
            intFirst += 1;
            strFirstRow = intFirst.ToString();
            strLastRow = intLast.ToString();
        }



        public void FillByDataTable(DataTable baseTable)
        {
            dtblTable = baseTable.Clone();
            int intFirstRow = int.Parse(strFirstRow) - 1;
            int intLastRow = int.Parse(strLastRow);
            RowCount = baseTable.Rows.Count;
            for (int index = intFirstRow; index < intLastRow; index++)
            {
                if (baseTable.Rows.Count > index && baseTable.Rows[index] != null)
                {
                    dtblTable.Rows.Add(baseTable.Rows[index].ItemArray);
                }
            }
        }

    }
}
