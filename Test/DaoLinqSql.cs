using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Resource.SQLServer;
using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Test
{
    public class DaoLinqSqlTest
    {
        public static void test()
        {
            var express = @"{
  ""$acttype"": ""CreateTable"",
  ""$table"": ""XXXS0301"",
  ""xm"": {
    ""$datatype"": ""nvarchar"",
    ""$precision"": ""20"",
    ""$scale"": """",
    ""$default"": """",
    ""$isnull"": true
  },
""bh"":{
        ""$datatype"": ""nvarchar"",
    ""$precision"": ""20"",
    ""$scale"": """",
    ""$default"": """",
    ""$isnull"": false
    },
  ""$pk"": [ ""bh"" ] 
}
";
            var s1 = DBExpress.Create<SqlServerExpress>(express).ToExpress();

            var s = from t in new DaoLinqTable("e")
                    select new
                    {
                        t.name,
                        t.age
                    };
        }
    }
    public class DaoLinqTable: DaoLinqSql<dynamic>
    {
        public override string Column_Quatation => "[{0}]";
        public DaoLinqTable(string table, string aliasName = "") : base(table, aliasName)
        {
            Item = FrameDLRObject.CreateInstance();
        }

        protected override object NewInstance<TResult>(TResult item, string table = "", string aliasName = "")
        {
            var rtn = new DaoLinqTable(table,aliasName);
            rtn.Item = item;
            return rtn;
        }
    }

    public class DaoLinqSql<TSource>:IDisposable
    {
        public DaoLinqSql() { }
        public DaoLinqSql(string table = "", string aliasName = "")
        {
            Table = table;
            AliasName = aliasName;
            Item = (TSource)Activator.CreateInstance(typeof(TSource), true);
        }
        protected virtual object NewInstance<TResult>(TResult item, string table = "", string aliasName = "")
        {
            return null;
        }
        public virtual T New<T,TResult>(TResult item, string table = "", string aliasName = "") where T :DaoLinqSql<TResult>
        {
            var obj = NewInstance<TResult>(item, table, aliasName);
            if (obj != null && obj is T)
            {
                return (T)obj;
            }
            else
            {
                var rtn = (T)Activator.CreateInstance(typeof(T), true);
                rtn.Item = item;
                rtn.Table = table;
                rtn.AliasName = string.IsNullOrEmpty(aliasName) ? rtn.Table : aliasName;
                return rtn;
            }
           
        }
        #region 基本参数设定
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
        /// <summary>
        /// 表名
        /// </summary>
        public string Table
        {
            get;
            protected set;
        }
        /// <summary>
        /// 别名
        /// </summary>
        public string AliasName
        {
            get;
            protected set;
        }
        /// <summary>
        /// LinqDLR2Sql下的元素对象
        /// </summary>
        public TSource Item
        {
            get;
            protected set;
        }

        #endregion
        #region Linq方法
        public virtual DaoLinqSql<TResult> Select<TResult>(Func<TSource, TResult> selector)
        {
            var dt = DateTime.Now;
            var v = selector.Invoke(Item);
            var rtn = New<DaoLinqSql<TResult>, TResult>(v, Table, AliasName);
            return rtn;
        }
        #endregion

        public void Dispose()
        {

        }
    }
}
