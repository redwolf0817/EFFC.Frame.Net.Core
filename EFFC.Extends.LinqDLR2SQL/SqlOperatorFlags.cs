using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL
{
    /// <summary>
    /// sql中会用到的各种操作符号
    /// </summary>
    public class SqlOperatorFlags
    {
        /// <summary>
        /// 参数符号，对应不同类型数据库的标记
        /// </summary>
        public virtual string ParamFlag { get { return "@"; } }
        /// <summary>
        /// 字符串链接符号
        /// </summary>
        public virtual string LinkFlag { get { return "&"; } }
        /// <summary>
        /// 不等于符号
        /// </summary>
        public virtual string NotEqualFlag { get { return "<>"; } }
        /// <summary>
        /// 等于符号
        /// </summary>
        public virtual string EqualFlag { get { return "="; } }
        /// <summary>
        /// 大于符号
        /// </summary>
        public virtual string GreaterFlag { get { return ">"; } }
        /// <summary>
        /// 大于等于符号
        /// </summary>
        public virtual string GreaterEqualFlag { get { return ">="; } }
        /// <summary>
        /// 小于符号
        /// </summary>
        public virtual string LessFlag { get { return "<"; } }
        /// <summary>
        /// 小于等于符号
        /// </summary>
        public virtual string LessEqualFlag { get { return "<="; } }
        /// <summary>
        /// 用于like的匹配符号
        /// </summary>
        public virtual string LikeMatchFlag { get { return "%"; } }
        /// <summary>
        /// 栏位引用符号
        /// </summary>
        public virtual string Column_Quatation { get { return "'{0}'"; } }
        /// <summary>
        /// 用于sql中is null的语句判断
        /// </summary>
        public virtual string IsNull { get { return "is null"; } }
        /// <summary>
        /// 用于sql中is not null的语句判断
        /// </summary>
        public virtual string IsNotNull { get { return "is not null"; } }
        /// <summary>
        /// sql中的+运算符
        /// </summary>
        public virtual string AddFlag { get { return "+"; } }
        /// <summary>
        /// sql中的-运算符
        /// </summary>
        public virtual string SubstractFlag { get { return "-"; } }
        /// <summary>
        /// sql中的*运算符
        /// </summary>
        public virtual string MultiplyFlag { get { return "*"; } }
        /// <summary>
        /// sql中的/运算符
        /// </summary>
        public virtual string DivideFlag { get { return "/"; } }

    }
}
