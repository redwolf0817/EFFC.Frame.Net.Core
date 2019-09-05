using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Common;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Attributes
{
    /// <summary>
    /// 添加接口中对参数的说明
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,AllowMultiple =true)]
    public class EWRAAddInputAttribute:Attribute
    {
        /// <summary>
        /// 定义输入参数的描述
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="type">参数类型描述</param>
        /// <param name="desc">参数描述</param>
        /// <param name="defaultvalue">默认值描述</param>
        /// <param name="position">参数传入的位置</param>
        /// <param name="isAllowEmpty">是否允许为空</param>
        public EWRAAddInputAttribute(string name, string type, string desc, string defaultvalue = null, RestInputPosition position = RestInputPosition.PostData, bool isAllowEmpty=true)
        {
            Name = name;
            ValueType = type;
            Desc = desc;
            DefaultValue = defaultvalue;
            IsAllowEmpty = isAllowEmpty;
            Position = ComFunc.Enum2String<RestInputPosition>(position);
        }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get;private set; }
        /// <summary>
        /// 参数类型
        /// </summary>
        public string ValueType { get; private set; }
        /// <summary>
        /// 参数描述
        /// </summary>
        public string Desc { get; private set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; private set; }
        /// <summary>
        /// 是否允许为空（不输入）
        /// </summary>
        public bool IsAllowEmpty { get; private set; }
        /// <summary>
        /// 参数位置
        /// </summary>
        public string Position { get; private set; }
    }
}
