using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EBA.Attributes
{
    /// <summary>
    /// 用于描述批次分组信息
    /// </summary>
    public class EBAGroupAttribute:Attribute
    {
        /// <summary>
        /// 批次的分组的组名
        /// </summary>
        /// <param name="name">组的名称</param>
        /// <param name="desc">描述</param>
        public EBAGroupAttribute(string groupName,string desc="")
        {
            GroupName = groupName;
            Description = desc;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string GroupName
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
