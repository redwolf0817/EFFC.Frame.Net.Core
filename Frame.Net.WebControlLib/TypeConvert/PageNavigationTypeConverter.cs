using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using EFFC.Frame.Net.WebControlLib.GridViewCustom;
using System.Web.UI.WebControls;

namespace EFFC.Frame.Net.WebControlLib.TypeConvert
{
    /// <summary>
    /// 翻页器的控件类型转换器
    /// 列出所有的PageNavigation
    /// </summary>
    public class PageNavigationControlConverter : ControlIDConverter
    {
        int a = 0;

        protected override bool FilterControl(System.Web.UI.Control control)
        {
            if (control is PageNavigation)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
