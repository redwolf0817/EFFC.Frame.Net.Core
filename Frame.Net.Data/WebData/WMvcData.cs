using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using EFFC.Frame.Net.Base.Data.Base;

namespace EFFC.Frame.Net.Data.WebData
{
    public class WMvcData:WebBaseData
    {
        /// <summary>
        /// 要显示的页面路径，格式:~/Views/xxx/xxx.cshtml
        /// </summary>
        public string ViewPath
        {
            get { return ComFunc.nvl(this["ViewPath"]); }
            set { this["ViewPath"] = value; }
        }
        /// <summary>
        /// 模板页面的名称
        /// </summary>
        public string StartViewName
        {
            get { return ComFunc.nvl(this["StartViewName"]); }
            set { this["StartViewName"] = value; }
        }
        /// <summary>
        /// Mvc中的Module数据对象
        /// </summary>
        public object MvcModuleData
        {
            get { return this["MvcModuleData"]; }
            set { this["MvcModuleData"] = value; }
        }
        /// <summary>
        /// 返回的脚本，不需要添加<script></script>标签
        /// </summary>
        public string Scripts
        {
            get { return ComFunc.nvl(this["Scripts"]); }
            set { this["Scripts"] = value; }
        }
        /// <summary>
        /// Response跳转，此属性为高优先级，该值不为空时，不会处理ViewPath
        /// </summary>
        public string RedirectUri
        {
            get { return ComFunc.nvl(this["RedirectUri"]); }
            set { this["RedirectUri"] = value; }
        }

        public override object Clone()
        {
            return this.Clone<WMvcData>();
        }
    }
}
