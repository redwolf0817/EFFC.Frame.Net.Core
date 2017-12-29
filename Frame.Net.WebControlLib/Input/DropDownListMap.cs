using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using EFFC.Frame.Net.WebControlLib.Interfaces;
using System.ComponentModel;
using EFFC.Frame.Net.Base.Common;
using System.Web.UI;

[assembly: TagPrefix("EFFC.Frame.Net.WebControlLib.Input", "clinput")]
namespace EFFC.Frame.Net.WebControlLib.Input
{
    public class DropDownListMap:DropDownList,IWebDataControl
    {
        string _mapdatafield = "";
        [Browsable(true)]
        [Category("DataInfo")]
        [Description("映射栏位名称，用于转化成module")]
        [Bindable(true)]
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
