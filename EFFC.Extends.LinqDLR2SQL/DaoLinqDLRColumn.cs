using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL
{
    /// <summary>
    /// LinqDLR2Sql中动态对象的元素定义
    /// </summary>
    public class LinqDLRColumn : MyDynamicMetaProvider,IDisposable
    {
        /// <summary>
        /// 参数符号，对应不同类型数据库的标记
        /// </summary>
        public virtual string ParamFlag
        {
            get
            {
                return "@";
            }
        }
        /// <summary>
        /// 字符串链接符号
        /// </summary>
        public virtual string LinkFlag
        {
            get { return "&"; }
        }
        /// <summary>
        /// 不等于符号
        /// </summary>
        public virtual string NotEqualFlag
        {
            get { return "<>"; }
        }
        /// <summary>
        /// 等于符号
        /// </summary>
        public virtual string EqualFlag
        {
            get { return "="; }
        }
        /// <summary>
        /// 大于符号
        /// </summary>
        public virtual string GreaterFlag
        {
            get { return ">"; }
        }
        /// <summary>
        /// 大于等于符号
        /// </summary>
        public virtual string GreaterEqualFlag
        {
            get { return ">="; }
        }
        /// <summary>
        /// 小于符号
        /// </summary>
        public virtual string LessFlag
        {
            get { return "<"; }
        }
        /// <summary>
        /// 小于等于符号
        /// </summary>
        public virtual string LessEqualFlag
        {
            get { return "<="; }
        }
        /// <summary>
        /// 用于like的匹配符号
        /// </summary>
        public virtual string LikeMatchFlag
        {
            get { return "%"; }
        }
        /// <summary>
        /// 栏位引用符号
        /// </summary>
        public virtual string Column_Quatation
        {
            get { return "'{0}'"; }
        }
        /// <summary>
        /// 用于sql中is null的语句判断
        /// </summary>
        public virtual string IsNull
        {
            get
            {
                return "is null";
            }
        }
        /// <summary>
        /// 用于sql中is not null的语句判断
        /// </summary>
        public virtual string IsNotNull
        {
            get
            {
                return "is not null";
            }
        }
        /// <summary>
        /// sql中的+运算符
        /// </summary>
        public virtual string AddFlag
        {
            get
            {
                return "+";
            }
        }
        /// <summary>
        /// sql中的-运算符
        /// </summary>
        public virtual string SubstractFlag
        {
            get
            {
                return "-";
            }
        }
        /// <summary>
        /// sql中的*运算符
        /// </summary>
        public virtual string MultiplyFlag
        {
            get
            {
                return "*";
            }
        }
        /// <summary>
        /// sql中的/运算符
        /// </summary>
        public virtual string DivideFlag
        {
            get
            {
                return "/";
            }
        }
        /// <summary>
        /// 执行where中的in操作
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public virtual LinqDLR2SqlWhereOperator WhereIn(object[] args)
        {
            return null;
        }
        /// <summary>
        /// 执行where中的not in操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual LinqDLR2SqlWhereOperator WhereNotIn(object[] args)
        {
            return null;
        }
        /// <summary>
        /// sql中isnull(object,othervalue)的表达式实现
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual LinqDLRColumn IsNullInSql(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            var rtn = New(this.GetType(), "", this.BelongToObject);
            var re = "";
            re = $"isnull({ColumnExpress},{rtn.Convert2Express(args[0])})";
            rtn.ColumnExpress = re;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
        public virtual LinqDLRColumn Sum(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            var rtn = New(this.GetType(), "", this.BelongToObject);
            var re = "";
            var alian_name = (args != null && args.Length > 0) ? ComFunc.nvl(args[0]) : "";
            if (alian_name == "") alian_name = "sum_" + ComFunc.Random(4);
            re = $"sum({ColumnExpress}) as {(alian_name)}";
            rtn.ColumnExpress = re;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
        public virtual LinqDLRColumn Max(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            var rtn = New(this.GetType(), "", this.BelongToObject);
            var re = "";
            var alian_name = (args != null && args.Length > 0) ? ComFunc.nvl(args[0]) : "";
            if (alian_name == "") alian_name = "max_" + ComFunc.Random(4);
            re = $"max({ColumnExpress}) as {alian_name}";
            rtn.ColumnExpress = re;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
        public virtual LinqDLRColumn Min(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            var rtn = New(this.GetType(), "", this.BelongToObject);
            var re = "";
            var alian_name = (args != null && args.Length > 0) ? ComFunc.nvl(args[0]) : "";
            if (alian_name == "") alian_name = "max_" + ComFunc.Random(4);
            re = $"min({ColumnExpress}) as {alian_name}";
            rtn.ColumnExpress = re;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
        /// <summary>
        /// 获取一个LinqDLRColumn的新实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnexpress"></param>
        /// <param name="belongto"></param>
        /// <returns></returns>
        public static T New<T>(string columnexpress, LamdaSQLObject<T> belongto) where T:LinqDLRColumn
        {
            T t = (T)Activator.CreateInstance(typeof(T), true);
            t.ColumnExpress = columnexpress;
            t.BelongToObject = belongto;
            t.ConditionValues = new Dictionary<string, object>();
            return t;
        }
        /// <summary>
        /// 获取一个LinqDLRColumn的新实例
        /// </summary>
        /// <param name="dlrcolumntype"></param>
        /// <param name="columnexpress"></param>
        /// <param name="belongto"></param>
        /// <returns></returns>
        public static LinqDLRColumn New(Type dlrcolumntype, string columnexpress, object belongto)
        {
            LinqDLRColumn t = (LinqDLRColumn)Activator.CreateInstance(dlrcolumntype, true);
            t.ColumnExpress = columnexpress;
            t.BelongToObject = belongto;
            t.ConditionValues = new Dictionary<string, object>();
            return t;
        }
        /// <summary>
        /// 生成一个对象的的栏位表达式
        /// </summary>
        /// <param name="dlrcolumntype"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LinqDLRColumn NewObjectColumn(Type dlrcolumntype, object value)
        {
            if (value is LinqDLRColumn) return (LinqDLRColumn)value;

            LinqDLRColumn t = (LinqDLRColumn)Activator.CreateInstance(dlrcolumntype, true);
            t.ConditionValues = new Dictionary<string, object>();
            t.ColumnExpress = t.Convert2Express(value);
            return t;
        }
        protected LinqDLRColumn()
        {
        }
        /// <summary>
        /// 所属的LamdaSQLObject的对象
        /// </summary>
        public Object BelongToObject
        {
            get;
            private set;
        }
        /// <summary>
        /// select出来的栏位表达式，可以为名称，别名或表达式
        /// </summary>
        public string ColumnExpress
        {
            get;
            set;
        }
        /// <summary>
        /// 栏位操作中记录的条件值
        /// </summary>
        public Dictionary<string, object> ConditionValues
        {
            get;
            protected set;
        }

        
        /// <summary>
        /// 模糊匹配操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object Contains(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
            ConditionValues.Add(pname, args[0]);
            var re = "";
            re = $"{ColumnExpress} LIKE '{LikeMatchFlag}'{LinkFlag}{ParamFlag}{pname}{LinkFlag}'{LikeMatchFlag}'";


            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        /// <summary>
        /// 左模糊匹配操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object StartWith(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
            ConditionValues.Add(pname, args[0]);
            var re = "";

            re = $"{ColumnExpress} LIKE {ParamFlag}{pname}{LinkFlag}'{LikeMatchFlag}'";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        /// <summary>
        /// 右模糊匹配操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object EndWith(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
            ConditionValues.Add(pname, args[0]);
            var re = "";
            re = $"{ColumnExpress} LIKE '{LikeMatchFlag}'{LinkFlag}{ParamFlag}{pname}";
           

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        /// <summary>
        /// 将多个栏位以字符串拼接的方式连接成新的栏位
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object Concat(object[] args)
        {
            if (args == null || args.Length <= 0) return this;

            var rtn = New(this.GetType(), "", this.BelongToObject);

            var express = $"concat({this.ColumnExpress},#replace#)";
            foreach(var item in args)
            {
                express = express.Replace("#replace#", $"{rtn.Convert2Express(item)},#replace#");
            }
            express = express.Replace(",#replace#", "");
            rtn.ColumnExpress = express;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
        /// <summary>
        /// 构建substring表达式
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object SubString(object[] args)
        {
            if (args == null || args.Length <= 0) return this;
            

            var rtn = New(this.GetType(), "", this.BelongToObject);

            var express = $"substring({this.ColumnExpress},#replace#)";
            foreach (var item in args)
            {
                express = express.Replace("#replace#", $"{rtn.Convert2Express(item)},#replace#");
            }
            express = express.Replace(",#replace#", "");
            rtn.ColumnExpress = express;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }


        protected override object GetMetaValue(string key)
        {
            return "";
        }

        protected override object InvokeMe(string methodInfo, params object[] args)
        {
            if(methodInfo.ToLower() == "contains")
            {
                return Contains(args);
            }
            if (methodInfo.ToLower() == "startwith")
            {
                return StartWith(args);
            }
            if (methodInfo.ToLower() == "endwith")
            {
                return EndWith(args);
            }
            if (methodInfo.ToLower() == "within")
            {
                return WhereIn(args);
            }
            if (methodInfo.ToLower() == "without")
            {
                return WhereNotIn(args);
            }
            if (methodInfo.ToLower() == "isnull")
            {
                return IsNullInSql(args);
            }
            if(methodInfo.ToLower() == "sum")
            {
                return Sum(args);
            }
            if (methodInfo.ToLower() == "max")
            {
                return Sum(args);
            }
            if (methodInfo.ToLower() == "min")
            {
                return Sum(args);
            }
            if(methodInfo.ToLower() == "concat")
            {
                return Concat(args);
            }
            if(methodInfo.ToLower() == "substring")
            {
                return SubString(args);
            }
            return null;
        }

        

        protected override object SetMetaValue(string key, object value)
        {
            return null;
        }

        protected override object MetaEqual(object v)
        {
            var re = "";

            if (v == null || v == DBNull.Value)
            {
                re = $"{ColumnExpress} {IsNull}";
            }
            else
            {
                re = $"{ColumnExpress}{EqualFlag}{Convert2Express(v)}";
            }

            var rtn = new LinqDLR2SqlWhereOperator(re, ConditionValues);

            return rtn;
        }

        protected override object MetaNotEqual(object v)
        {
            var re = "";
            if (v == null || v == DBNull.Value)
            {
                re = $"{ColumnExpress} {IsNotNull}";
            }
            else
            {
                re = $"{ColumnExpress}{NotEqualFlag}{Convert2Express(v)}";
            }
            

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }

        protected override object MetaGreaterThan(object v)
        {
            var re = "";
            re = $"{ColumnExpress}{GreaterFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaGreaterThanEqual(object v)
        {
            var re = "";

            re = $"{ColumnExpress}{GreaterEqualFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaLessThan(object v)
        {
            var re = "";
            re = $"{ColumnExpress}{LessFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaLessThanEqual(object v)
        {
            var re = "";
            re = $"{ColumnExpress}{LessEqualFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaAdd(object v)
        {
            var rtn = New(this.GetType(), "", this.BelongToObject);
            var re = "";
            re = $"({ColumnExpress}{AddFlag}{rtn.Convert2Express(v)})";
            rtn.ColumnExpress = re;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
        protected override object MetaDivide(object v)
        {
            var rtn = New(this.GetType(), "", this.BelongToObject);
            var re = "";
            re = $"({ColumnExpress}{DivideFlag}{rtn.Convert2Express(v)})";
            rtn.ColumnExpress = re;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
        protected override object MetaMultiply(object v)
        {
            var rtn = New(this.GetType(), "", this.BelongToObject);
            var re = "";
            re = $"({ColumnExpress}{MultiplyFlag}{rtn.Convert2Express(v)})";
            rtn.ColumnExpress = re;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
        protected override object MetaSubstract(object v)
        {
            var rtn = New(this.GetType(),"",this.BelongToObject);
            var re = "";
            re = $"({ColumnExpress}{SubstractFlag}{rtn.Convert2Express(v)})";
            rtn.ColumnExpress = re;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
        /// <summary>
        /// 将右侧值转化成对应的表达式，并添加条件值
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public virtual string Convert2Express(object v)
        {
            
            var re = "";
            if (v is LinqDLRColumn)
            {
                //因为每次操作产生的LinqDLRColumn都是新的实例，所以每操作一次，都需要将右方的LinqDLRColumn做释放
                using (var otherldc = ((LinqDLRColumn)v))
                {
                    re = $"{otherldc.ColumnExpress}";
                    foreach (var item in otherldc.ConditionValues)
                    {
                        if (!ConditionValues.ContainsKey(item.Key))
                        {
                            ConditionValues.Add(item.Key, item.Value);
                        }
                    }
                }
            }
            else
            {
                var pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
                //防止有key重复导致的异常
                while (ConditionValues.ContainsKey(pname))
                {
                    pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
                }
                ConditionValues.Add(pname, v);

                re = $"{ParamFlag}{pname}";
            }

            return re;
        }

        public virtual void Dispose()
        {
            ConditionValues.Clear();

        }
    }
}
