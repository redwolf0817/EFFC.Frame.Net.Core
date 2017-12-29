using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Data.WebData
{
    public class GoData : WebBaseData
    {
        /// <summary>
        /// 待返回response的结果串
        /// </summary>
        public object ResponseData
        {
            get
            {
                return this[ParameterKey.RESPONSE_DATA];
            }
            set
            {
                this[ParameterKey.RESPONSE_DATA] = value;
            }
        }
        /// <summary>
        /// 返回数据的类型,默认为json数据
        /// </summary>
        public GoResponseDataType ContentType
        {
            get
            {
                if (GetValue(ParameterKey.CONTENT_TYPE) == null)
                    SetValue(ParameterKey.CONTENT_TYPE, GoResponseDataType.Json);
                return (GoResponseDataType)GetValue(ParameterKey.CONTENT_TYPE);
            }
            set
            {
                SetValue(ParameterKey.CONTENT_TYPE, value);
            }
        }

        /// <summary>
        /// Response跳转，此属性为高优先级
        /// </summary>
        public string RedirectUri
        {
            get { return ComFunc.nvl(this["RedirectUri"]); }
            set { this["RedirectUri"] = value; }
        }

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

        public override object Clone()
        {
            return this.Clone<GoData>();
        }
    }
}
