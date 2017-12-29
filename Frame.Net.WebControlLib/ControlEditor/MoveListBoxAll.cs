
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Drawing;

namespace EFFC.Frame.Net.WebControlLib
{
    public class MoveListBoxAll : ListBox
    {
        private HiddenField AddedBox
        {
            get
            {
                return (HiddenField)base.Controls[1];
            }
        }

        private HiddenField DeletedBox
        {
            get
            {
                return (HiddenField)base.Controls[2];
            }
        }
        public ListBox RightListBox
        {
            get
            {
                return (ListBox)base.Controls[0];
            }
        }
        public MoveListBoxAll()
        {
            this.Controls.Add(new ListBox());
            this.Controls.Add(new HiddenField());
            this.Controls.Add(new HiddenField());
        }
        private Dictionary<string, string> BaseItems
        {
            get
            {
                if (base.ViewState["BaseItems"] == null)
                {
                    base.ViewState.Add("BaseItems", new Dictionary<string, string>());
                }
                Dictionary<string, string> returnValue = (Dictionary<string, string>)base.ViewState["BaseItems"];
                return returnValue;
            }
        }

        public Dictionary<string, string> GetMastAddData()
        {
            Dictionary<string, string> MastAddData = new Dictionary<string, string>();
            for (int index = 0; index < RightListBox.Items.Count; index++)
            {
                if (!BaseItems.ContainsKey(RightListBox.Items[index].Value.Trim()))
                {
                    MastAddData.Add(RightListBox.Items[index].Value.Trim(), RightListBox.Items[index].Text.Trim());
                }
            }
            return MastAddData;
        }

        public Dictionary<string, string> GetMastDelData()
        {
            Dictionary<string, string> MastDelData = new Dictionary<string, string>();
            foreach (string itemVlaue in BaseItems.Keys)
            {
                if (RightListBox.Items.FindByValue(itemVlaue.Trim()) == null)
                {
                    MastDelData.Add(itemVlaue, BaseItems[itemVlaue]);
                }
            }
            return MastDelData;
        }

        public string[] GetMastAddValuesArray()
        {
            Dictionary<string, string> MastAddData = GetMastAddData();
            string[] returanValues = new string[MastAddData.Count];
            MastAddData.Keys.CopyTo(returanValues, 0);
            return returanValues;
        }
        public string MastAddValues
        {
            get
            {
                return string.Join(",", GetMastAddValuesArray());
            }
        }

        public string[] GetMastDelValuesArray()
        {
            Dictionary<string, string> MastDelData = GetMastDelData();
            string[] returanValues = new string[MastDelData.Count];
            MastDelData.Keys.CopyTo(returanValues, 0);
            return returanValues;
        }

        public string MastDelValues
        {
            get
            {
                return string.Join(",", GetMastDelValuesArray());
            }
        }

        public Dictionary<string, string> GetUserSelectedData()
        {
            Dictionary<string, string> returnValue = new Dictionary<string, string>();
            for (int index = 0; index < RightListBox.Items.Count; index++)
            {
                returnValue.Add(RightListBox.Items[index].Value.Trim(), RightListBox.Items[index].Text.Trim());
            }

            return returnValue;
        }
        public string[] GetUserSelectedValuesArray()
        {
            Dictionary<string, string> UserSelectedData = GetUserSelectedData();
            string[] returnValue = new string[UserSelectedData.Count];
            UserSelectedData.Keys.CopyTo(returnValue, 0);
            return returnValue;
        }
        public string UserSelectedValues
        {
            get
            {
                return string.Join(",", GetUserSelectedValuesArray());
            }
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                for (int index = 0; index < RightListBox.Items.Count; index++)
                {
                    this.Items.Remove(this.Items.FindByValue(RightListBox.Items[index].Value));
                    if (!BaseItems.ContainsKey(RightListBox.Items[index].Value.Trim()))
                    {
                        BaseItems.Add(RightListBox.Items[index].Value.Trim(), RightListBox.Items[index].Text.Trim());
                    }
                }
            }
            if (AddedBox.Value != null && AddedBox.Value != "")
            {
                string AddedString = AddedBox.Value.Substring(0, AddedBox.Value.Length - 1);
                string[] adds = AddedString.Split(";".ToCharArray());
                //List<ListItem> LeftClears = new List<ListItem>();
                RightListBox.Items.Clear();
                for (int index = 0; index < adds.Length; index++)
                {
                    string[] item = adds[index].Trim().Split(",".ToCharArray());
                    RightListBox.Items.Add(new ListItem(item[1], item[0]));
                    //this.Items.Remove(this.Items.FindByValue(item[1]));
                }
                AddedBox.Value = "";
            }
            //if (DeletedBox.Value != null && DeletedBox.Value != "")
            //{
            //    string DeletedString = DeletedBox.Value.Substring(0, DeletedBox.Value.Length - 1);
            //    string[] dels = DeletedString.Split(";".ToCharArray());
            //    List<ListItem> RightClears = new List<ListItem>();
            //    for (int index = 0; index < dels.Length; index++)
            //    {
            //        string[] item = dels[index].Trim().Split(",".ToCharArray());
            //        this.Items.Add(new ListItem(item[0], item[1]));
            //        RightListBox.Items.Remove(RightListBox.Items.FindByValue(item[1]));
            //    }
            //    DeletedBox.Value = "";
            //}

            if (!Page.ClientScript.IsClientScriptBlockRegistered("MoveListBox"))
            {
                string clientscript = "function Add(Obj,Active,IsAll)                                                                              " +
                                    "{                                                                                                           " +
                                    "    var row=Obj.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;                          " +
                                    "    var addedHidden=Obj.parentNode.parentNode.parentNode.parentNode.rows[4].cells[0].childNodes[0];         " +
                                    "    var deletedHidden=Obj.parentNode.parentNode.parentNode.parentNode.rows[4].cells[0].childNodes[1];       " +
                                    "    var FromSelect,ToSelect;                                                                                " +
                                    "    if(Active=='toRight')                                                                                   " +
                                    "    {FromSelect=row.cells[0].childNodes[0];ToSelect=row.cells[2].childNodes[0];}                            " +
                                    "    else                                                                                                    " +
                                    "    {FromSelect=row.cells[2].childNodes[0];ToSelect=row.cells[0].childNodes[0];}                            " +
                                    "    var sss=new Array(),l=0;                                                                                " +
                                    "    if(IsAll)                                                                                               " +
                                    "    {                                                                                                       " +
                                    "        for(var i=0;i<FromSelect.options.length;i++){sss[l]=FromSelect.options[i];l++;}                     " +
                                    "    }                                                                                                       " +
                                    "    else                                                                                                    " +
                                    "    {                                                                                                       " +
                                    "        for(var i=0;i<FromSelect.options.length;i++)                                                        " +
                                    "	 {                                                                                                   " +
                                    "	 	if(FromSelect.options[i].selected)                                                           " +
                                    "	 	{                                                                                            " +
                                    "	 		sss[l]=FromSelect.options[i];l++;                                                    " +
                                    "	 	}                                                                                            " +
                                    "	 }                                                                                                   " +
                                    "    }                                                                                                       " +
                                    "                                                                                                            " +
                                    "    for(var i=0;i<sss.length;i++)                                                                           " +
                                    "    {                                                                                                       " +
                                    "       if(Active=='toRight')                                                                                " +
                                    "       {                                                                                                    " +
                                    "		   if(!IsRightExist(sss[i].value,ToSelect))                                                  " +
                                    "		   {                                                                                         " +
                                    "              var oOption = document.createElement('OPTION');                                                  " +
                                    "			   ToSelect.options.add(oOption);                                                    " +
                                    "			   oOption.innerText=sss[i].innerText;                                               " +
                                    "			   oOption.value =sss[i].value;                                                      " +
                                    "		   }                                                                                         " +
                                    "       }                                                                                                    " +
                                    "       else                                                                                                 " +
                                    "       {                                                                                                    " +
                                    "			FromSelect.removeChild(sss[i]);                                                      " +
                                    "       }                                                                                                    " +
                                    "    }                                                                                                       " +
                                    "    addedHidden.value = '';                                                                                 " +
                                    "    if(Active!='toRight')                                                                                   " +
                                    "    {                                                                                                       " +
                                    "          for(var i=0;i<FromSelect.options.length;i++)                                                      " +
                                    "          {                                                                                                 " +
                                    "              addedHidden.value += FromSelect.options[i].value+','+ FromSelect.options[i].innerText+';';    " +
                                    "          }                                                                                                 " +
                                    "    }                                                                                                       " +
                                    "    else                                                                                                    " +
                                    "    {                                                                                                       " +
                                    "          for(var i=0;i<ToSelect.options.length;i++)                                                        " +
                                    "          {                                                                                                 " +
                                    "              addedHidden.value += ToSelect.options[i].value+','+ ToSelect.options[i].innerText+';';        " +
                                    "          }	                                                                                             " +
                                    "    }                                                                                                       " +
                                    "    SetButton(Obj,'button');                                                                                " +
                                    "}                                                                                                           " +
                                    "function IsRightExist(option,right)                                                                         " +
                                    "{                                                                                                           " +
                                    "      for(var i=0;i<right.options.length;i++)                                                               " +
                                    "	  {                                                                                                  " +
                                    "		  if(right.options[i].value == option)                                                       " +
                                    "		      return true;                                                                           " +
                                    "	  }                                                                                                  " +
                                    "	  return false;                                                                                      " +
                                    "}                                                                                                           " +
                                    "function SetButton(Obj,ctrlType)                                                                            " +
                                    "{                                                                                                           " +
                                    "	var LeftSelect,RightSelect,ToRightAdd,ToLeftAdd,ToLeftAll,ToRightAll,row,table;                      " +
                                    "	if(ctrlType=='button')                                                                               " +
                                    "	{                                                                                                    " +
                                    "		table=Obj.parentNode.parentNode.parentNode.parentNode;row=table.parentNode.parentNode;       " +
                                    "	}                                                                                                    " +
                                    "	else                                                                                                 " +
                                    "	{                                                                                                    " +
                                    "		row=Obj.parentNode.parentNode.parentNode.parentNode;table=row.rows[0].cells[1].childNodes[0];" +
                                    "	}                                                                                                    " +
                                    "	LeftSelect=row.cells[0].childNodes[0];RightSelect=row.cells[2].childNodes[0];                        " +
                                    "	ToRightAdd=table.rows[0].cells[0].childNodes[0];ToLeftAdd=table.rows[1].cells[0].childNodes[0];      " +
                                    "	ToRightAll=table.rows[2].cells[0].childNodes[0];ToLeftAll=table.rows[3].cells[0].childNodes[0];      " +
                                    "	if(LeftSelect.options.length<=0)                                                                     " +
                                    "	{                                                                                                    " +
                                    "		ToRightAdd.disabled='disabled';ToRightAll.disabled='disabled';                               " +
                                    "	}                                                                                                    " +
                                    "	else                                                                                                 " +
                                    "	{                                                                                                    " +
                                    "		ToRightAll.disabled='';var ToRightAdddisabled='disabled';                                    " +
                                    "		for(var i=0;i<LeftSelect.options.length;i++)                                                 " +
                                    "		{                                                                                            " +
                                    "			if(LeftSelect.options[i].selected)                                                   " +
                                    "			{                                                                                    " +
                                    "				ToRightAdddisabled='';                                                       " +
                                    "				break;                                                                       " +
                                    "			}                                                                                    " +
                                    "		}                                                                                            " +
                                    "		ToRightAdd.disabled=ToRightAdddisabled;                                                      " +
                                    "	}                                                                                                    " +
                                    "	if(RightSelect.options.length<=0)                                                                    " +
                                    "	{                                                                                                    " +
                                    "		ToLeftAdd.disabled='disabled';ToLeftAll.disabled='disabled';                                 " +
                                    "	}                                                                                                    " +
                                    "	else                                                                                                 " +
                                    "	{                                                                                                    " +
                                    "		ToLeftAll.disabled='';var ToLeftAdddisabled='disabled';                                      " +
                                    "		for(var i=0;i<RightSelect.options.length;i++)                                                " +
                                    "		{                                                                                            " +
                                    "			if(RightSelect.options[i].selected)                                                  " +
                                    "			{                                                                                    " +
                                    "				ToLeftAdddisabled='';                                                        " +
                                    "				break;                                                                       " +
                                    "			}                                                                                    " +
                                    "		}                                                                                            " +
                                    "		ToLeftAdd.disabled=ToLeftAdddisabled;                                                        " +
                                    "	}                                                                                                    " +
                                    "}                                                                                                           ";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MoveListBox", clientscript, true);
            }
        }

        public override void DataBind()
        {
            base.DataBind();
            RightListBox.DataTextField = this.DataTextField;
            RightListBox.DataValueField = this.DataValueField;
            RightListBox.DataBind();
        }
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            this.Attributes.Add("onchange", "SetButton(this,'select');");
            RightListBox.Attributes.Add("onchange", "SetButton(this,'select');");
            RightListBox.Width = this.Width;
            RightListBox.Height = this.Height;

            writer.Write("<table  border=\"0\" cellspacing=\"3\" cellpadding=\"0\"><tr><td>");
            base.Render(writer);
            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.AppendLine("</td>");
            codeBuilder.AppendLine("<td width=\"10\"><table  border=\"0\" cellspacing=\"3\" cellpadding=\"0\">");
            codeBuilder.AppendLine("<tr>");
            codeBuilder.AppendLine("<td><input type=\"button\" name=\"Submit\" value=\" △ \" onClick=\"Add(this,'toRight',false);\" disabled=\"disabled\" /></td>");
            codeBuilder.AppendLine("</tr>");
            codeBuilder.AppendLine("<tr>");
            codeBuilder.AppendLine("<td><input type=\"button\" name=\"Submit2\" value=\" ■ \"  onClick=\"Add(this,'toLeft',false);\" disabled=\"disabled\" /></td>");
            codeBuilder.AppendLine("</tr>");
            codeBuilder.AppendLine("<tr>");
            codeBuilder.AppendLine("<td><input type=\"button\" name=\"Submit3\" value=\" △|\"  onClick=\"Add(this,'toRight',true);\" /></td>");
            codeBuilder.AppendLine("</tr>");
            codeBuilder.AppendLine("<tr>");
            codeBuilder.AppendLine("<td><input type=\"button\" name=\"Submit4\" value=\"|■ \" onClick=\"Add(this,'toLeft',true);\" /></td>");
            codeBuilder.AppendLine("</tr>");
            codeBuilder.AppendLine("<tr><td>");
            writer.Write(codeBuilder.ToString());
            codeBuilder.Remove(0, codeBuilder.Length);
            AddedBox.RenderControl(writer);
            DeletedBox.RenderControl(writer);
            codeBuilder.AppendLine("</td></tr>");
            codeBuilder.AppendLine("</table></td>");
            codeBuilder.AppendLine("<td>");
            writer.Write(codeBuilder.ToString());
            codeBuilder.Remove(0, codeBuilder.Length);
            RightListBox.RenderControl(writer);
            codeBuilder.AppendLine("</td>");
            codeBuilder.AppendLine("</tr>");
            codeBuilder.AppendLine("</table>");
            writer.Write(codeBuilder.ToString());
        }





        //public ListItemCollection SelectedItems { get { return selectedItems; } }//---
        //private ListItemCollection selectedItems = new ListItemCollection();//---

        //public ListItemCollection AddedItems { get { return addedItems; } }
        //private ListItemCollection addedItems = new ListItemCollection();

        //public ListItemCollection DeletedItems { get { return deletedItems; } }
        //private ListItemCollection deletedItems = new ListItemCollection();

        //public string AddedItemValues { get { return addedItemValues; } }
        //private string addedItemValues = null;

        //public string DeletedItemValues { get { return deletedItemValues; } }
        //private string deletedItemValues = null;

        //private string addedCode = null;
        //private string deletedCode = null;

        //public void SelectedItemsDataBind(DataTable table, string valueFieldName, string textFieldName)
        //{
        //    for (int index = 0; index < table.Rows.Count; index++)
        //    {
        //        selectedItems.Add(new ListItem(table.Rows[index][textFieldName].ToString(), table.Rows[index][valueFieldName].ToString()));
        //    }
        //}

        //public void SelectedItemsDataBind(DataTable table)
        //{
        //    SelectedItemsDataBind(table, this.DataValueField, this.DataTextField);
        //}
        //public MoveListBox()
        //{
        //    base.Attributes.Add("onchange", "SetButton(this,'select');");
        //}

        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //    if (!DesignMode)
        //    {
        //        if (Page.IsPostBack)
        //        {
        //            addedCode = Page.Request.Form[this.ID + "_Added"];

        //            addedItemValues = "";
        //            if (addedCode != null && addedCode != "" && addedCode != string.Empty)
        //            {
        //                addedCode = addedCode.Substring(0, addedCode.Length - 1);
        //                addedItems.Clear();
        //                string[] addedCodes = addedCode.Split(";".ToCharArray());
        //                for (int index = 0; index < addedCodes.Length; index++)
        //                {
        //                    string[] itemCodes = addedCodes[index].Split(",".ToCharArray());
        //                    addedItems.Add(new ListItem(itemCodes[0], itemCodes[1]));
        //                    addedItemValues += itemCodes[1].Trim() + ",";
        //                }
        //                addedItemValues = addedItemValues.Substring(0, addedItemValues.Length - 1);
        //            }

        //            deletedCode = Page.Request.Form[this.ID + "_Deleted"];

        //            deletedItemValues = "";
        //            if (deletedCode != null && deletedCode != "" && deletedCode != string.Empty)
        //            {
        //                deletedCode = deletedCode.Substring(0, deletedCode.Length - 1);
        //                deletedItems.Clear();
        //                string[] deletedCodes = deletedCode.Split(";".ToCharArray());
        //                for (int index = 0; index < deletedCodes.Length; index++)
        //                {
        //                    string[] itemCodes = deletedCodes[index].Split(",".ToCharArray());
        //                    deletedItems.Add(new ListItem(itemCodes[0], itemCodes[1]));
        //                    deletedItemValues += itemCodes[1].Trim() + ",";
        //                }
        //                deletedItemValues = deletedItemValues.Substring(0, deletedItemValues.Length - 1);
        //            }
        //        }
        //        if (!Page.ClientScript.IsClientScriptBlockRegistered("MoveListBox"))
        //        {
        //            StringBuilder scriptBuilder = new StringBuilder();
        //            scriptBuilder.AppendLine("<script language=\"javascript\" type=\"text/javascript\">");
        //            scriptBuilder.AppendLine("function Add(Obj,Active,IsAll)");
        //            scriptBuilder.AppendLine("{");
        //            scriptBuilder.AppendLine("var row=Obj.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;");
        //            scriptBuilder.AppendLine("var addedHidden=Obj.parentNode.parentNode.parentNode.parentNode.rows[4].cells[0].childNodes[0];");
        //            scriptBuilder.AppendLine("var deletedHidden=Obj.parentNode.parentNode.parentNode.parentNode.rows[4].cells[0].childNodes[2];");
        //            scriptBuilder.AppendLine("var FromSelect,ToSelect;");
        //            scriptBuilder.AppendLine("if(Active=='toRight'){FromSelect=row.cells[0].childNodes[0];ToSelect=row.cells[2].childNodes[0];}");
        //            scriptBuilder.AppendLine("else{FromSelect=row.cells[2].childNodes[0];ToSelect=row.cells[0].childNodes[0];}");
        //            scriptBuilder.AppendLine("var sss=new Array(),l=0;");
        //            scriptBuilder.AppendLine("if(IsAll){for(var i=0;i<FromSelect.options.length;i++){sss[l]=FromSelect.options[i];l++;}}");
        //            scriptBuilder.AppendLine("else{for(var i=0;i<FromSelect.options.length;i++){if(FromSelect.options[i].selected){sss[l]=FromSelect.options[i];l++;}}}");
        //            scriptBuilder.AppendLine("for(var i=0;i<sss.length;i++)");
        //            scriptBuilder.AppendLine("{");
        //            scriptBuilder.AppendLine("var oOption = document.createElement('OPTION');ToSelect.options.add(oOption);oOption.innerText=sss[i].innerText;");
        //            scriptBuilder.AppendLine("oOption.value =sss[i].value;FromSelect.removeChild(sss[i]);var itemCode=sss[i].innerText+','+sss[i].value;");
        //            scriptBuilder.AppendLine("if(Active=='toRight')");
        //            scriptBuilder.AppendLine("{");
        //            scriptBuilder.AppendLine("if(deletedHidden.value!='')");
        //            scriptBuilder.AppendLine("{");
        //            scriptBuilder.AppendLine("var itemCodes=deletedHidden.value.split(';');var CanAdd=true;");
        //            scriptBuilder.AppendLine("for(var j=0;j<itemCodes.length;j++){if(itemCodes[j]==itemCode){deletedHidden.value=deletedHidden.value.replace(itemCode+';','');CanAdd=false;}}");
        //            scriptBuilder.AppendLine("if(CanAdd){addedHidden.value+=itemCode+';';}");
        //            scriptBuilder.AppendLine("}");
        //            scriptBuilder.AppendLine("else{addedHidden.value+=itemCode+';';}");
        //            scriptBuilder.AppendLine("}");
        //            scriptBuilder.AppendLine("else");
        //            scriptBuilder.AppendLine("{");
        //            scriptBuilder.AppendLine("if(addedHidden.value!='')");
        //            scriptBuilder.AppendLine("{");
        //            scriptBuilder.AppendLine("var itemCodes=addedHidden.value.split(';');var CanAdd=true;");
        //            scriptBuilder.AppendLine("for(var j=0;j<itemCodes.length;j++){if(itemCodes[j]==itemCode){addedHidden.value=addedHidden.value.replace(itemCode+';','');CanAdd=false;}}");
        //            scriptBuilder.AppendLine("if(CanAdd){deletedHidden.value+=itemCode+';';}");
        //            scriptBuilder.AppendLine("}");
        //            scriptBuilder.AppendLine("else{deletedHidden.value+=itemCode+';';}");
        //            scriptBuilder.AppendLine("}");
        //            scriptBuilder.AppendLine("}");
        //            scriptBuilder.AppendLine("SetButton(Obj,'button');");
        //            scriptBuilder.AppendLine("}");
        //            scriptBuilder.AppendLine("function SetButton(Obj,ctrlType)");
        //            scriptBuilder.AppendLine("{");
        //            scriptBuilder.AppendLine("var LeftSelect,RightSelect,ToRightAdd,ToLeftAdd,ToLeftAll,ToRightAll,row,table;");
        //            scriptBuilder.AppendLine("if(ctrlType=='button'){table=Obj.parentNode.parentNode.parentNode.parentNode;row=table.parentNode.parentNode;}");
        //            scriptBuilder.AppendLine("else{row=Obj.parentNode.parentNode.parentNode.parentNode;table=row.rows[0].cells[1].childNodes[0];}");
        //            scriptBuilder.AppendLine("LeftSelect=row.cells[0].childNodes[0];RightSelect=row.cells[2].childNodes[0];");
        //            scriptBuilder.AppendLine("ToRightAdd=table.rows[0].cells[0].childNodes[0];ToLeftAdd=table.rows[1].cells[0].childNodes[0];");
        //            scriptBuilder.AppendLine("ToRightAll=table.rows[2].cells[0].childNodes[0];ToLeftAll=table.rows[3].cells[0].childNodes[0];");
        //            scriptBuilder.AppendLine("if(LeftSelect.options.length<=0){ToRightAdd.disabled='disabled';ToRightAll.disabled='disabled';}");
        //            scriptBuilder.AppendLine("else");
        //            scriptBuilder.AppendLine("{");
        //            scriptBuilder.AppendLine("ToRightAll.disabled='';var ToRightAdddisabled='disabled';");
        //            scriptBuilder.AppendLine("for(var i=0;i<LeftSelect.options.length;i++){if(LeftSelect.options[i].selected){ToRightAdddisabled='';break;}}");
        //            scriptBuilder.AppendLine("ToRightAdd.disabled=ToRightAdddisabled;");
        //            scriptBuilder.AppendLine("}");
        //            scriptBuilder.AppendLine("if(RightSelect.options.length<=0){ToLeftAdd.disabled='disabled';ToLeftAll.disabled='disabled';}");
        //            scriptBuilder.AppendLine("else");
        //            scriptBuilder.AppendLine("{");
        //            scriptBuilder.AppendLine("ToLeftAll.disabled='';var ToLeftAdddisabled='disabled';");
        //            scriptBuilder.AppendLine("for(var i=0;i<RightSelect.options.length;i++){if(RightSelect.options[i].selected){ToLeftAdddisabled='';break;}}");
        //            scriptBuilder.AppendLine("ToLeftAdd.disabled=ToLeftAdddisabled;");
        //            scriptBuilder.AppendLine("}");
        //            scriptBuilder.AppendLine("}");
        //            scriptBuilder.AppendLine("</script>");
        //            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MoveListBox", scriptBuilder.ToString());
        //        }

        //    }

        //}

        //protected override void Render(System.Web.UI.HtmlTextWriter writer)
        //{
        //    writer.Write("<table  border=\"0\" cellspacing=\"3\" cellpadding=\"0\"><tr><td>");
        //    List<ListItem> delItems = new List<ListItem>();
        //    if (Page.IsPostBack)
        //    {
        //        List<ListItem> delSelectedItems = new List<ListItem>();
        //        for (int selectedIndex = 0; selectedIndex < selectedItems.Count; selectedIndex++)
        //        {
        //            string selectedItemValue = selectedItems[selectedIndex].Value.Trim();
        //            for (int deletedIndex = 0; deletedIndex < deletedItems.Count; deletedIndex++)
        //            {
        //                string deletedItemValue = deletedItems[deletedIndex].Value.Trim();
        //                if (selectedItemValue == deletedItemValue)
        //                {
        //                    delSelectedItems.Add(selectedItems[selectedIndex]);
        //                }
        //            }
        //        }
        //        for (int index = 0; index < delSelectedItems.Count; index++)
        //        {
        //            selectedItems.Remove(delSelectedItems[index]);
        //        }
        //    }
        //    for (int baseIndex = 0; baseIndex < base.Items.Count; baseIndex++)
        //    {
        //        string baseItemValue = base.Items[baseIndex].Value.Trim();
        //        for (int selectedIndex = 0; selectedIndex < selectedItems.Count; selectedIndex++)
        //        {
        //            string selectedItemValue = selectedItems[selectedIndex].Value.Trim();
        //            if (baseItemValue == selectedItemValue)
        //            {
        //                delItems.Add(base.Items[baseIndex]);
        //            }
        //        }
        //        if (Page.IsPostBack)
        //        {
        //            for (int index = 0; index < addedItems.Count; index++)
        //            {
        //                string addedItemValue = addedItems[index].Value.Trim();

        //                if (baseItemValue == addedItemValue)
        //                {
        //                    delItems.Add(base.Items[baseIndex]);
        //                }
        //            }
        //        }
        //    }
        //    for (int index = 0; index < delItems.Count; index++)
        //    {
        //        base.Items.Remove(delItems[index]);
        //    }
        //    base.Render(writer);
        //    StringBuilder codeBuilder = new StringBuilder();
        //    codeBuilder.AppendLine("</td>");
        //    codeBuilder.AppendLine("<td width=\"10\"><table  border=\"0\" cellspacing=\"3\" cellpadding=\"0\">");
        //    codeBuilder.AppendLine("<tr>");
        //    codeBuilder.AppendLine("<td><input type=\"button\" name=\"Submit\" value=\" △ \" onClick=\"Add(this,'toRight',false);\" disabled=\"disabled\" /></td>");
        //    codeBuilder.AppendLine("</tr>");
        //    codeBuilder.AppendLine("<tr>");
        //    codeBuilder.AppendLine("<td><input type=\"button\" name=\"Submit2\" value=\" ■ \"  onClick=\"Add(this,'toLeft',false);\" disabled=\"disabled\" /></td>");
        //    codeBuilder.AppendLine("</tr>");
        //    codeBuilder.AppendLine("<tr>");
        //    codeBuilder.AppendLine("<td><input type=\"button\" name=\"Submit3\" value=\" △|\"  onClick=\"Add(this,'toRight',true);\" /></td>");
        //    codeBuilder.AppendLine("</tr>");
        //    codeBuilder.AppendLine("<tr>");
        //    codeBuilder.AppendLine("<td><input type=\"button\" name=\"Submit4\" value=\"|■ \" onClick=\"Add(this,'toLeft',true);\" /></td>");
        //    codeBuilder.AppendLine("</tr>");
        //    codeBuilder.AppendLine("<tr>");
        //    if (addedCode != null && addedCode != "" && addedCode != string.Empty)
        //    {
        //        codeBuilder.AppendLine("<td><input type=\"hidden\" name=\"" + this.ID + "_Added\" value=\"" + addedCode + ";\" />");
        //    }
        //    else
        //    {
        //        codeBuilder.AppendLine("<td><input type=\"hidden\" name=\"" + this.ID + "_Added\"  />");
        //    }

        //    if (deletedCode != null && deletedCode != "" && deletedCode != string.Empty)
        //    {
        //        codeBuilder.AppendLine("<input type=\"hidden\" name=\"" + this.ID + "_Deleted\" value=\"" + deletedCode + ";\" /></td>");
        //    }
        //    else
        //    {
        //        codeBuilder.AppendLine("<input type=\"hidden\" name=\"" + this.ID + "_Deleted\" /></td>");
        //    }
        //    codeBuilder.AppendLine("</tr>");
        //    codeBuilder.AppendLine("</table></td>");
        //    codeBuilder.AppendLine("<td><select name=\"selectedData\" size=\"10\" id=\"selectedData\" style=\"width:" + Width.ToString() + ";height:" + Height.ToString() + "\"  onChange=\"SetButton(this,'select');\">");
        //    for (int index = 0; index < selectedItems.Count; index++)
        //    {
        //        codeBuilder.AppendLine("<option value=\"" + selectedItems[index].Value + "\">" + selectedItems[index].Text + "</option>");
        //    }
        //    for (int index = 0; index < addedItems.Count; index++)
        //    {
        //        codeBuilder.AppendLine("<option value=\"" + addedItems[index].Value + "\">" + addedItems[index].Text + "</option>");
        //    }
        //    codeBuilder.AppendLine("</select></td>");
        //    codeBuilder.AppendLine("</tr>");
        //    codeBuilder.AppendLine("</table>");
        //    writer.Write(codeBuilder.ToString());
        //}
    }
}
