using System;

namespace EFFC.Frame.Net.Base.AttributeDefine
{
    /// <summary>
    /// 属性或方法的注释描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class DescAttribute:Attribute
    {
        string _desc = "";
        public DescAttribute(string desc)
        {
            _desc = desc;
        }

        /// <summary>
        /// 注释描述
        /// </summary>
        public string Desc
        {
            get
            {
                return _desc;
            }
        }
    }
}
