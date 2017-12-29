using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Attributes
{
    /// <summary>
    /// 标记method是否对外开放
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EWRAIsOpenAttribute:Attribute
    {
        public EWRAIsOpenAttribute(bool isopen)
        {
            IsOpen = isopen;
        }
        /// <summary>
        /// 标记该方法是否开放
        /// </summary>
        public bool IsOpen
        {
            get;
            set;
        }
    }
}
