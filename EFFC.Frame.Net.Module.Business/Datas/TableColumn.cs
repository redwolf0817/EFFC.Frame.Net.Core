using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Business.Datas
{
    /// <summary>
    /// 用于创建Table时栏位的描述定义
    /// </summary>
    public class TableColumn
    {
        /// <summary>
        /// 栏位名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据类型，固定为varchar,nvarchar,int,numberic,bit,datetime,text
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 数据精度，int类型，当数据类型varchar，nvarchar，numberic有效
        /// </summary>
        public int Precision { get; set; } = -1;
        /// <summary>
        /// 比例标准，int类型，仅numberic类型有效
        /// </summary>
        public int Scale { get; set; } = -1;
        /// <summary>
        /// 默认值或表达式，当前支持的标准表达式为:now()(mysql,sqlite不支持),increament(整数)(oracle,sqlite不支持)
        /// </summary>
        public string Default { get; set; }
        /// <summary>
        /// 是否允许为空，为bool类型
        /// </summary>
        public bool AllowNull { get; set; }
        /// <summary>
        /// 是否为PK
        /// </summary>
        public bool IsPK { get; set; }
    }
}
