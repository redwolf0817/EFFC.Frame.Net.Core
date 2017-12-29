
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Drawing;

namespace EFFC.Frame.Net.WebControlLib.GridViewCustom
{

    [ToolboxBitmapAttribute(typeof(ListBox))]
    public class MoveListBox : ListBox
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

        //分隔符，默認為|
        public string SplitChar
        {
            get
            {
                if (ViewState["SplitChar"] == null)
                    return "|";
                return ViewState["SplitChar"].ToString();
            }
            set
            {
                ViewState["SplitChar"] = value;
            }
        }

        public MoveListBox()
        {
            this.Controls.Add(new ListBox());
            this.Controls.Add(new HiddenField());
            this.Controls.Add(new HiddenField());

            //this.Controls.Add(new RequiredFieldValidator());
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
                if (!returnValue.ContainsKey(RightListBox.Items[index].Value.Trim()))
                {
                    returnValue.Add(RightListBox.Items[index].Value.Trim(), RightListBox.Items[index].Text.Trim());
                }
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

        public string[] GetUserSelectedTextsArray()
        {
            Dictionary<string, string> UserSelectedData = GetUserSelectedData();
            string[] returnValue = new string[UserSelectedData.Count];
            UserSelectedData.Values.CopyTo(returnValue, 0);
            return returnValue;
        }

        public string UserSelectedValues
        {
            get
            {
                return string.Join(",", GetUserSelectedValuesArray());
            }
        }

        public string UserSelectedTexts
        {
            get
            {
                return string.Join(",", GetUserSelectedTextsArray());
            }
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            RightListBox.SelectionMode = ListSelectionMode.Multiple;
            this.SelectionMode = ListSelectionMode.Multiple;

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
                List<ListItem> LeftClears = new List<ListItem>();
                string[] strSplitList = new string[] { this.SplitChar };
                for (int index = 0; index < adds.Length; index++)
                {
                    string[] item = adds[index].Trim().Split(strSplitList, StringSplitOptions.None);
                    RightListBox.Items.Add(new ListItem(item[0], item[1]));
                    this.Items.Remove(this.Items.FindByValue(item[1]));
                }
                AddedBox.Value = "";
            }
            if (DeletedBox.Value != null && DeletedBox.Value != "")
            {
                string DeletedString = DeletedBox.Value.Substring(0, DeletedBox.Value.Length - 1);
                string[] dels = DeletedString.Split(";".ToCharArray());
                List<ListItem> RightClears = new List<ListItem>();
                string[] strSplitList = new string[] { this.SplitChar };
                for (int index = 0; index < dels.Length; index++)
                {
                    string[] item = dels[index].Trim().Split(strSplitList, StringSplitOptions.None);
                    this.Items.Add(new ListItem(item[0], item[1]));
                    RightListBox.Items.Remove(RightListBox.Items.FindByValue(item[1]));
                }
                DeletedBox.Value = "";
            }

            if (!Page.ClientScript.IsClientScriptBlockRegistered("MoveListBox"))
            {
                StringBuilder scriptBuilder = new StringBuilder();
                scriptBuilder.AppendLine("<script language=\"javascript\" type=\"text/javascript\">");
                scriptBuilder.AppendLine("function Add(Obj,Active,IsAll)");
                scriptBuilder.AppendLine("{");
                scriptBuilder.AppendLine("var row = $(Obj).parent().parent().parent().parent().parent().parent();");
                scriptBuilder.AppendLine("var addedHidden = $(row).find('input')[0];");
                scriptBuilder.AppendLine("var deletedHidden = $(row).find('input')[1];");
                scriptBuilder.AppendLine("var FromSelect,ToSelect;");
                scriptBuilder.AppendLine("if(Active=='toRight'){ FromSelect = $(row).find('select')[0]; ToSelect = $(row).find('select')[1];}");
                scriptBuilder.AppendLine("else{ FromSelect = $(row).find('select')[1]; ToSelect = $(row).find('select')[0];}");
                scriptBuilder.AppendLine("var sss=new Array(),l=0;");
                scriptBuilder.AppendLine("if(IsAll){for(var i=0;i<FromSelect.options.length;i++){sss[l]=FromSelect.options[i];l++;}}");
                scriptBuilder.AppendLine("else{for(var i=0;i<FromSelect.options.length;i++){if(FromSelect.options[i].selected){sss[l]=FromSelect.options[i];l++;}}}");
                scriptBuilder.AppendLine("for(var i=0;i<sss.length;i++)");
                scriptBuilder.AppendLine("{");
                scriptBuilder.AppendLine("var oOption = document.createElement('OPTION');ToSelect.options.add(oOption);oOption.innerText=sss[i].innerText;");
                scriptBuilder.AppendLine("oOption.value =sss[i].value;FromSelect.removeChild(sss[i]);var itemCode=sss[i].innerText+'" + this.SplitChar + "'+sss[i].value;");
                scriptBuilder.AppendLine("if(Active=='toRight')");
                scriptBuilder.AppendLine("{");
                scriptBuilder.AppendLine("if(deletedHidden.value!='')");
                scriptBuilder.AppendLine("{");
                scriptBuilder.AppendLine("var itemCodes=deletedHidden.value.split(';');var CanAdd=true;");
                scriptBuilder.AppendLine("for(var j=0;j<itemCodes.length;j++){if(itemCodes[j]==itemCode){deletedHidden.value=deletedHidden.value.replace(itemCode+';','');CanAdd=false;}}");
                scriptBuilder.AppendLine("if(CanAdd){addedHidden.value+=itemCode+';';}");
                scriptBuilder.AppendLine("}");
                scriptBuilder.AppendLine("else{addedHidden.value+=itemCode+';';}");
                scriptBuilder.AppendLine("}");
                scriptBuilder.AppendLine("else");
                scriptBuilder.AppendLine("{");
                scriptBuilder.AppendLine("if(addedHidden.value!='')");
                scriptBuilder.AppendLine("{");
                scriptBuilder.AppendLine("var itemCodes=addedHidden.value.split(';');var CanAdd=true;");
                scriptBuilder.AppendLine("for(var j=0;j<itemCodes.length;j++){if(itemCodes[j]==itemCode){addedHidden.value=addedHidden.value.replace(itemCode+';','');CanAdd=false;}}");
                scriptBuilder.AppendLine("if(CanAdd){deletedHidden.value+=itemCode+';';}");
                scriptBuilder.AppendLine("}");
                scriptBuilder.AppendLine("else{deletedHidden.value+=itemCode+';';}");
                scriptBuilder.AppendLine("}");
                scriptBuilder.AppendLine("}");
                scriptBuilder.AppendLine("SetButton(Obj,'button');");
                scriptBuilder.AppendLine("}");
                scriptBuilder.AppendLine("function SetButton(Obj,ctrlType)");
                scriptBuilder.AppendLine("{");
                scriptBuilder.AppendLine("var LeftSelect,RightSelect,ToRightAdd,ToLeftAdd,ToLeftAll,ToRightAll,row,table;");
                scriptBuilder.AppendLine("if(ctrlType=='button'){ table = $(Obj).parent().parent().parent().parent().parent().parent();}");
                scriptBuilder.AppendLine("else{table = $(Obj).parent().parent();}");
                scriptBuilder.AppendLine(" LeftSelect = $(table).find('select')[0]; RightSelect = $(table).find('select')[1];");
                scriptBuilder.AppendLine(" ToRightAdd = $(table).find('img')[0]; ToLeftAdd = $(table).find('img')[1];");
                scriptBuilder.AppendLine("ToRightAll = $(table).find('img')[2]; ToLeftAll = $(table).find('img')[3];");
                scriptBuilder.AppendLine("if(LeftSelect.options.length<=0){ToRightAdd.disabled='disabled';ToRightAll.disabled='disabled';}");
                scriptBuilder.AppendLine("else");
                scriptBuilder.AppendLine("{");
                scriptBuilder.AppendLine("ToRightAll.disabled='';var ToRightAdddisabled='disabled';");
                scriptBuilder.AppendLine("for(var i=0;i<LeftSelect.options.length;i++){if(LeftSelect.options[i].selected){ToRightAdddisabled='';break;}}");
                scriptBuilder.AppendLine("ToRightAdd.disabled=ToRightAdddisabled;");
                scriptBuilder.AppendLine("}");
                scriptBuilder.AppendLine("if(RightSelect.options.length<=0){ToLeftAdd.disabled='disabled';ToLeftAll.disabled='disabled';}");
                scriptBuilder.AppendLine("else");
                scriptBuilder.AppendLine("{");
                scriptBuilder.AppendLine("ToLeftAll.disabled='';var ToLeftAdddisabled='disabled';");
                scriptBuilder.AppendLine("for(var i=0;i<RightSelect.options.length;i++){if(RightSelect.options[i].selected){ToLeftAdddisabled='';break;}}");
                scriptBuilder.AppendLine("ToLeftAdd.disabled=ToLeftAdddisabled;");
                scriptBuilder.AppendLine("}");
                scriptBuilder.AppendLine("}");
                scriptBuilder.AppendLine("</script>");
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MoveListBox", scriptBuilder.ToString());
            }
        }

        public override void DataBind()
        {
            base.DataBind();
            RightListBox.DataTextField = this.DataTextField;
            RightListBox.DataValueField = this.DataValueField;
            RightListBox.DataBind();
            BaseItems.Clear();
            for (int index = 0; index < RightListBox.Items.Count; index++)
            {
                this.Items.Remove(this.Items.FindByValue(RightListBox.Items[index].Value));
                if (!BaseItems.ContainsKey(RightListBox.Items[index].Value.Trim()))
                {
                    BaseItems.Add(RightListBox.Items[index].Value.Trim(), RightListBox.Items[index].Text.Trim());
                }
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            this.Attributes.Add("onchange", "SetButton(this,'select');");
            RightListBox.Attributes.Add("onchange", "SetButton(this,'select');");
            RightListBox.Width = this.Width;
            RightListBox.Height = this.Height;
            //左側List註冊雙擊事件
            this.Attributes.Add("onDblClick", "Add(this,'toRight',false);");
            //右側List註冊雙擊事件
            this.RightListBox.Attributes.Add("onDblClick", "Add(this,'toLeft',false);");

            writer.Write("<table class=\"movetable\" border=\"0\" cellspacing=\"3\" cellpadding=\"0\"><tr><td style=\"width:320px;\">");
            base.Render(writer);
            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.AppendLine("</td>");
            codeBuilder.AppendLine("<td width=\"80\" align=\"center\" ><table  border=\"0\" cellspacing=\"13\" cellpadding=\"0\">");
            codeBuilder.AppendLine("<tr>");
            // codeBuilder.AppendLine("<td><input class='btn' style=\"width:30px\" type=\"button\" name=\"Submit\" value=\">\" src=\"~/Common/images/SinglearrowRight.gif\" onClick=\"Add(this,'toRight',false);\" disabled=\"disabled\" /></td>");
            codeBuilder.AppendLine("<td><img id=\"Submit\" src=\"../../images/SinglearrowRight.gif\" alt=\"\" onclick=\"Add(this,'toRight',false);\" /></td>");
            codeBuilder.AppendLine("</tr>");
            codeBuilder.AppendLine("<tr>");
            // codeBuilder.AppendLine("<td><input class='btn' style=\"width:30px\" type=\"button\" name=\"Submit2\" value=\"<\"  onClick=\"Add(this,'toLeft',false);\" disabled=\"disabled\" /></td>");
            codeBuilder.AppendLine("<td><img id=\"Submit2\" src=\"../../images/SinglearrowLeft.gif\" alt=\"\" onClick=\"Add(this,'toLeft',false);\" /></td>");
            codeBuilder.AppendLine("</tr>");
            codeBuilder.AppendLine("<tr>");
            // codeBuilder.AppendLine("<td><input class='btn' style=\"width:30px\" type=\"button\" name=\"Submit3\" value=\">>\"  onClick=\"Add(this,'toRight',true);\" /></td>");
            codeBuilder.AppendLine("<td><img id=\"Submit3\" src=\"../../images/arrowRight.gif\" alt=\"\" onClick=\"Add(this,'toRight',true);\" /></td>");
            codeBuilder.AppendLine("</tr>");
            codeBuilder.AppendLine("<tr>");
            // codeBuilder.AppendLine("<td><input class='btn' style=\"width:30px\" type=\"button\" name=\"Submit4\" value=\"<<\" onClick=\"Add(this,'toLeft',true);\" /></td>");
            codeBuilder.AppendLine("<td><img id=\"Submit3\" src=\"../../images/arrowLeft.gif\" alt=\"\"  onClick=\"Add(this,'toLeft',true);\" /></td>");
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
    }
}
