using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Data;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Common;

[assembly: TagPrefix("EFFC.Frame.Net.WebControlLib.Navigation", "clng")]
namespace EFFC.Frame.Net.WebControlLib.Navigation
{
    public class FrameworkMenu : Control
    {
        bool _enabled = true;
        string _width = "100px";
        string _height = "60px";
        string _cssbody = "";
        string _cssStatic = "";
        string _cssDynamic = "";
        string _cssBody = "";
        string _cssStaticLink = "";
        string _cssDynamicLink = "";
        string _cssStaticBody = "";
        string _cssDynamicBody = "";
        string _cssStaticFont = "";
        string _cssDynamicFont = "";
        string _cssDynamicFoot = "";
        string _cssStaticFoot = "";
        string _cssStaticOnlyLevel1 = "";
        string _functionLevel = "level";
        string _functionname = "funcname";
        string _functionid = "funcno";
        string _functionurl = "functionurl";
        string _functionparentid = "ParentFuncNo";
        DataTable _dtSource = null;
        int _maxLevel = 0;

        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定是否可操作")]
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
        [Description("设定宽度")]
        public string Width
        {
            get { return _width; }
            set { _width = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定高度")]
        public string Height
        {
            get { return _height; }
            set { _height = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定菜单的样式")]
        public string Css
        {
            get { return _cssbody; }
            set { _cssbody = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定设定静态(一级)菜单的样式")]
        public string CssStatic
        {
            get { return _cssStatic; }
            set { _cssStatic = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定设定动态(二级及以下)菜单的样式")]
        public string CssDynamic
        {
            get { return _cssDynamic; }
            set { _cssDynamic = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定设定MenuBody菜单的样式")]
        public string CssBody
        {
            get { return _cssBody; }
            set { _cssBody = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定设定Static的连接样式")]
        public string CssStaticLink
        {
            get { return _cssStaticLink; }
            set { _cssStaticLink = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定设定Dynamic的连接样式")]
        public string CssDynamicLink
        {
            get { return _cssDynamicLink; }
            set { _cssDynamicLink = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定设定Dynamic的body样式")]
        public string CssDynamicBody
        {
            get { return _cssDynamicBody; }
            set { _cssDynamicBody = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定设定static的body样式")]
        public string CssStaticBody
        {
            get { return _cssStaticBody; }
            set { _cssStaticBody = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定设定static的字体样式")]
        public string CssStaticFont
        {
            get { return _cssStaticFont; }
            set { _cssStaticFont = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定设定dynamic的字体样式")]
        public string CssDynamicFont
        {
            get { return _cssDynamicFont; }
            set { _cssDynamicFont = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定设定dynamic的foot样式")]
        public string CssDynamicFoot
        {
            get { return _cssDynamicFoot; }
            set { _cssDynamicFoot = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定设定Static的foot样式")]
        public string CssStaticFoot
        {
            get { return _cssStaticFoot; }
            set { _cssStaticFoot = value; }
        }
        [Category("BaseProperties")]
        [Browsable(true)]
        [Description("设定设定Static的只有1级菜单时候的样式")]
        public string CssStaticOnlyLevel1
        {
            get { return _cssStaticOnlyLevel1; }
            set { _cssStaticOnlyLevel1 = value; }
        }
        [Category("ColumnMap")]
        [Browsable(true)]
        [Description("设定对应的FunctionID的栏位名称")]
        public string ColumnMap_FunctionID
        {
            get { return _functionid; }
            set { _functionid = value; }
        }
        [Category("ColumnMap")]
        [Browsable(true)]
        [Description("设定对应的FunctionName的栏位名称")]
        public string ColumnMap_FunctionName
        {
            get { return _functionname; }
            set { _functionname = value; }
        }
        [Category("ColumnMap")]
        [Browsable(true)]
        [Description("设定对应的FunctionLevel的栏位名称")]
        public string ColumnMap_FunctionLevel
        {
            get { return _functionLevel; }
            set { _functionLevel = value; }
        }
        [Category("ColumnMap")]
        [Browsable(true)]
        [Description("设定对应的FunctionUrl的栏位名称")]
        public string ColumnMap_FunctionUrl
        {
            get { return _functionurl; }
            set { _functionurl = value; }
        }
        [Category("ColumnMap")]
        [Browsable(true)]
        [Description("设定对应的FunctionUrl的栏位名称")]
        public string ColumnMap_ParentFunctionID
        {
            get { return _functionparentid; }
            set { _functionparentid = value; }
        }


        /// <summary>
        /// 需要绑定的DataSource
        /// </summary>
        public DataTable DataSource
        {
            get { return _dtSource; }
            set { _dtSource = value; }
        }

        public void BindData()
        {
            if (DataSource != null)
            {
                DataTableStd dtt = DataTableStd.ParseStd(DataSource);
                for (int i = 0; i < dtt.RowLength; i++)
                {
                    int curlevel = IntStd.ParseStd(dtt[i, ColumnMap_FunctionLevel]).Value;
                    if (IntStd.ParseStd(dtt[i, ColumnMap_FunctionLevel]) > _maxLevel)
                    {
                        _maxLevel = curlevel;
                    }
                }
            }
        }

        protected string ConvertUrl(string url)
        {
            string rtn = "";
            string root = ResolveUrl("~");
            rtn = url.Replace("~",root.Substring(0,root.Length-1));
            return rtn;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //设计模式
            if (DesignMode)
            {
                //div
                writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width);
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, this.Height);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
                writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, Css);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                //body ul
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssBody);
                writer.RenderBeginTag(HtmlTextWriterTag.Ul);

                //一级菜单Li
                writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssStatic);
                writer.RenderBeginTag(HtmlTextWriterTag.Li);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssStaticLink);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, "#;");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write("Level1");
                writer.RenderEndTag();

                //二级级菜单
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssDynamicBody);
                writer.RenderBeginTag(HtmlTextWriterTag.Ul);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssDynamic);
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssDynamicLink);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write("Level2");
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssDynamic);
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssDynamicLink);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write("Level2");
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssDynamic);
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssDynamicLink);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write("Level2");
                writer.RenderEndTag();
                writer.RenderEndTag();
                //三级菜单
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssDynamicBody);
                writer.RenderBeginTag(HtmlTextWriterTag.Ul);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssDynamic);
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssDynamicLink);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write("Level3");
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderEndTag();

                //li
                writer.RenderEndTag();
                //ul
                writer.RenderEndTag();
                //Div
                writer.RenderEndTag();
            }
            else
            {

                if (DataSource != null)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, "menuTree");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    BuildLevel1(writer);
                    //li
                    writer.RenderEndTag();
                }
            }

        }

        protected void BuildLevel1(HtmlTextWriter writer)
        {
            DataTableStd dtt = DataTableStd.ParseStd(DataSource);
            string where = ColumnMap_FunctionLevel + "=1";
            DataTableStd dttLevel = dtt.SelectByWhere(where);
            int leve1Index = 0;
            for (int i = 0; i < dttLevel.RowLength; i++)
            {
                string functionid = ComFunc.nvl(dttLevel[i, ColumnMap_FunctionID]);
                
                writer.AddAttribute(HtmlTextWriterAttribute.Id, "menuLevel1_" + functionid);
                writer.AddAttribute("functionid", functionid);
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "ShowMenu(this," + leve1Index + ");");
                writer.RenderBeginTag(HtmlTextWriterTag.H1);

                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(ComFunc.nvl(dttLevel[i, ColumnMap_FunctionName]));
                writer.RenderEndTag();

                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "no");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                //ul
                writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                //如果含有子菜单
                if (HasChild(functionid))
                {           
                    BuildMenu(2, functionid, writer);
                }
                writer.RenderEndTag();
                writer.RenderEndTag();

                leve1Index++;
            }
        }

        protected void BuildMenu(int level, string parentid, HtmlTextWriter writer)
        {
            DataTableStd dtt = DataTableStd.ParseStd(DataSource);
            string where = ColumnMap_FunctionLevel + "=" + level;
            if (!string.IsNullOrEmpty(parentid))
            {
                where += " AND " + ColumnMap_ParentFunctionID + "='" + parentid + "'";
            }
            DataTableStd dttLevel = dtt.SelectByWhere(where);
            for (int i = 0; i < dttLevel.RowLength; i++)
            {
                string functionid = ComFunc.nvl(dttLevel[i, ColumnMap_FunctionID]);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "toolbar");
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                if (ComFunc.nvl(dttLevel[i, ColumnMap_FunctionUrl]) != "")
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, ConvertUrl(ComFunc.nvl(dttLevel[i, ColumnMap_FunctionUrl])));
                    writer.AddAttribute("target","iFrameRight");
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write(ComFunc.nvl(dttLevel[i, ColumnMap_FunctionName]));
                    writer.RenderEndTag();
                }
                else
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:");
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write(ComFunc.nvl(dttLevel[i, ColumnMap_FunctionName]));
                    writer.RenderEndTag();
                }

                writer.RenderEndTag();
                //如果含有子菜单
                if (HasChild(functionid))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    //ul
                    writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                    BuildMenu(level + 1, functionid, writer);
                    writer.RenderEndTag();

                    writer.RenderEndTag();
                }
            }
        }

        private bool HasChild(string parentid)
        {
            DataTableStd dtt = DataTableStd.ParseStd(DataSource);
            DataTableStd dttLevel = dtt.SelectByWhere(ColumnMap_ParentFunctionID + "='" + parentid + "'");
            if (dttLevel.RowLength > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            Render(writer);
        }
    }
}
