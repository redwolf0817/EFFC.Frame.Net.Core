using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EBA.Attributes
{
    /// <summary>
    /// 用于描述批次指令信息
    /// </summary>
    public class EBAIsOpenAttribute : Attribute
    {
        /// <summary>
        /// 指定批次是否开启
        /// </summary>
        /// <param name="isopen"></param>
        public EBAIsOpenAttribute(bool isopen=true)
        {
            IsOpen = isopen;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public bool IsOpen
        {
            get;
            protected set;
        }

    }
}
