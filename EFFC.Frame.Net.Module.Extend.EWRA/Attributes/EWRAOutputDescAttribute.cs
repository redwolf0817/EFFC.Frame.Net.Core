using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.EWRA.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Attributes
{
    /// <summary>
    /// Rest接口输出的描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EWRAOutputDescAttribute:Attribute
    {
        /// <summary>
        ///  Rest接口输出的描述
        /// </summary>
        /// <param name="desc">返回结果的描述</param>
        /// <param name="format">返回结构集的结构描述，比如：如果为JSON,则填写json的数据格式，如：
        /// {
        ///     "code":"执行结果状态，success为成功，failed为失败",
        ///     "msg":"执行结果的提示信息",
        ///     "data":[结果集数组]
        /// }
        /// </param>
        /// <param name="type"></param>
        public EWRAOutputDescAttribute(string desc, string format, RestContentType type = RestContentType.JSON)
        {
            Desc = desc;
            FormatDesc = format;
            ReturnType = ComFunc.Enum2String<RestContentType>(type);
        }
        /// <summary>
        /// 返回结果描述
        /// </summary>
        public string Desc { get; private set; }
        /// <summary>
        /// 返回结果格式描述
        /// </summary>
        public string FormatDesc { get; private set; }
        /// <summary>
        /// 返回结构数据格式类型
        /// </summary>
        public string ReturnType { get; private set; }
    }
}
