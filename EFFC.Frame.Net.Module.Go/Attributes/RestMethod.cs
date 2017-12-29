using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.WebGo.Attributes
{
    /// <summary>
    /// 对应Rest请求下的method属性（Post，PUT，GET，DELETE）
    /// </summary>
    public class RestMethodAttribute : Attribute
    {
        /// <summary>
        /// Rest的Method名称
        /// </summary>
        public string RestMethodName { get; }
        public RestMethodAttribute(string methodName)
        {
            RestMethodName = methodName;
        }
    }
}
