using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EBA.Attributes
{
    /// <summary>
    /// 用于描述批次指令信息
    /// </summary>
    public class EBADescAttribute:Attribute
    {
        /// <summary>
        /// 批次的描述信息
        /// </summary>
        /// <param name="name">批次的名称</param>
        /// <param name="desc">描述</param>
        public EBADescAttribute(string name,string desc="")
        {
            Name = name;
            Description = desc;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; protected set; }

    }
}
