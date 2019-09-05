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
        #region 公开变量
        /// <summary>
        /// SQL中的相关符号
        /// </summary>
        public SqlOperatorFlags SqlFlags
        {
            get;protected set;
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
        #endregion
        #region 私有变量

        #endregion
        #region 扩展SQL的方法
        /// <summary>
        /// 执行where中的in操作
        /// </summary>
        /// <param name="args"></param>
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
            var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);
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
        /// <summary>
        /// sql中的sum操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual LinqDLRColumn Sum(object[] args)
        {
            //无参数时则无别名
            if (args == null || args.Length <= 0)
            {
                var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);
                var re = "";
                re = $"sum({ColumnExpress})";
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
            else
            {
                var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);
                var re = "";
                var alian_name = (args != null && args.Length > 0) ? ComFunc.nvl(args[0]) : "";
                if (alian_name == "") alian_name = "sum_" + ComFunc.Random(6);
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
            
        }
        /// <summary>
        /// sql中的count操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual LinqDLRColumn Count(object[] args)
        {
            var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);
            //无参数时则无别名
            if (args == null || args.Length <= 0)
            {
                var re = "";
                re = $"count({ColumnExpress})";
                rtn.ColumnExpress = re;
                foreach (var item in ConditionValues)
                {
                    if (!rtn.ConditionValues.ContainsKey(item.Key))
                    {
                        rtn.ConditionValues.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                var re = "";
                var alian_name = (args != null && args.Length > 0) ? ComFunc.nvl(args[0]) : "";
                if (alian_name == "") alian_name = "count_" + ComFunc.Random(6);
                re = $"count({ColumnExpress}) as {(alian_name)}";
                rtn.ColumnExpress = re;
                foreach (var item in ConditionValues)
                {
                    if (!rtn.ConditionValues.ContainsKey(item.Key))
                    {
                        rtn.ConditionValues.Add(item.Key, item.Value);
                    }
                }
            }
            return rtn;
        }
        /// <summary>
        /// sql中的max操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual LinqDLRColumn Max(object[] args)
        {
            var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);
            if (args == null || args.Length <= 0)
            {
               
                var re = "";
                re = $"max({ColumnExpress})";
                rtn.ColumnExpress = re;
                foreach (var item in ConditionValues)
                {
                    if (!rtn.ConditionValues.ContainsKey(item.Key))
                    {
                        rtn.ConditionValues.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                var re = "";
                var alian_name = (args != null && args.Length > 0) ? ComFunc.nvl(args[0]) : "";
                if (alian_name == "") alian_name = "max_" + ComFunc.Random(6);
                re = $"max({ColumnExpress}) as {alian_name}";
                rtn.ColumnExpress = re;
                foreach (var item in ConditionValues)
                {
                    if (!rtn.ConditionValues.ContainsKey(item.Key))
                    {
                        rtn.ConditionValues.Add(item.Key, item.Value);
                    }
                }
            }
            return rtn;
        }
        /// <summary>
        /// min操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual LinqDLRColumn Min(object[] args)
        {
            var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);
            if (args == null || args.Length <= 0)
            {
                var re = "";
                re = $"min({ColumnExpress})";
                rtn.ColumnExpress = re;
                foreach (var item in ConditionValues)
                {
                    if (!rtn.ConditionValues.ContainsKey(item.Key))
                    {
                        rtn.ConditionValues.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                var re = "";
                var alian_name = (args != null && args.Length > 0) ? ComFunc.nvl(args[0]) : "";
                if (alian_name == "") alian_name = "max_" + ComFunc.Random(6);
                re = $"min({ColumnExpress}) as {alian_name}";
                rtn.ColumnExpress = re;
                foreach (var item in ConditionValues)
                {
                    if (!rtn.ConditionValues.ContainsKey(item.Key))
                    {
                        rtn.ConditionValues.Add(item.Key, item.Value);
                    }
                }
            }
           
            return rtn;
        }
        /// <summary>
        /// avg操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual LinqDLRColumn Avg(object[] args)
        {
            var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);
            if (args == null || args.Length <= 0)
            {
                var re = "";
                re = $"avg({ColumnExpress})";
                rtn.ColumnExpress = re;
                foreach (var item in ConditionValues)
                {
                    if (!rtn.ConditionValues.ContainsKey(item.Key))
                    {
                        rtn.ConditionValues.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                var re = "";
                var alian_name = (args != null && args.Length > 0) ? ComFunc.nvl(args[0]) : "";
                if (alian_name == "") alian_name = "avg_" + ComFunc.Random(6);
                re = $"avg({ColumnExpress}) as {alian_name}";
                rtn.ColumnExpress = re;
                foreach (var item in ConditionValues)
                {
                    if (!rtn.ConditionValues.ContainsKey(item.Key))
                    {
                        rtn.ConditionValues.Add(item.Key, item.Value);
                    }
                }
            }

            return rtn;
        }
        /// <summary>
        /// 模糊匹配操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object Contains(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(6));
            ConditionValues.Add(pname, args[0]);
            var re = "";
            re = $"{ColumnExpress} LIKE '{SqlFlags.LikeMatchFlag}'{SqlFlags.LinkFlag}{SqlFlags.ParamFlag}{pname}{SqlFlags.LinkFlag}'{SqlFlags.LikeMatchFlag}'";


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

            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(6));
            ConditionValues.Add(pname, args[0]);
            var re = "";

            re = $"{ColumnExpress} LIKE {SqlFlags.ParamFlag}{pname}{SqlFlags.LinkFlag}'{SqlFlags.LikeMatchFlag}'";

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

            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(6));
            ConditionValues.Add(pname, args[0]);
            var re = "";
            re = $"{ColumnExpress} LIKE '{SqlFlags.LikeMatchFlag}'{SqlFlags.LinkFlag}{SqlFlags.ParamFlag}{pname}";


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

            var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);

            var express = $"concat({this.ColumnExpress},#replace#)";
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
        /// <summary>
        /// 构建substring表达式
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object SubString(object[] args)
        {
            if (args == null || args.Length <= 0) return this;


            var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);

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
        /// <summary>
        /// 构建substring表达式
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object ConvertToString(object[] args)
        {
            throw new NotImplementedException("未实现LinqDLRColumn.ConvertToString方法");
        }
        /// <summary>
        /// 构建Convert To DateTime表达式
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object ConvertToDateTime(object[] args)
        {
            throw new NotImplementedException("未实现LinqDLRColumn.ConvertToDateTime");
        }
        #endregion
        #region 静态方法
        /// <summary>
        /// 获取一个LinqDLRColumn的新实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnexpress"></param>
        /// <param name="belongto"></param>
        /// <param name="sqlflags"></param>
        /// <returns></returns>
        public static T New<T>(string columnexpress, LamdaSQLObject<T> belongto,SqlOperatorFlags sqlflags) where T:LinqDLRColumn
        {
            T t = (T)Activator.CreateInstance(typeof(T), true);
            t.ColumnExpress = columnexpress;
            t.BelongToObject = belongto;
            t.ConditionValues = new Dictionary<string, object>();
            t.SqlFlags = sqlflags;


            return t;
        }
        /// <summary>
        /// 生成一个对象的的栏位表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="sqlflags"></param>
        /// <returns></returns>
        public static LinqDLRColumn New<T>(object value, SqlOperatorFlags sqlflags) where T : LinqDLRColumn
        {
            if (value is LinqDLRColumn) return (LinqDLRColumn)value;

            LinqDLRColumn t = (LinqDLRColumn)Activator.CreateInstance(typeof(T), true);
            t.ConditionValues = new Dictionary<string, object>();
            t.ColumnExpress = t.Convert2Express(value);
            t.SqlFlags = sqlflags;
            return t;
        }
        /// <summary>
        /// 获取一个LinqDLRColumn的新实例
        /// </summary>
        /// <param name="dlrcolumntype"></param>
        /// <param name="columnexpress"></param>
        /// <param name="belongto"></param>
        /// <param name="sqlflags"></param>
        /// <returns></returns>
        protected static LinqDLRColumn New(Type dlrcolumntype, string columnexpress, object belongto,SqlOperatorFlags sqlflags)
        {
            LinqDLRColumn t = (LinqDLRColumn)Activator.CreateInstance(dlrcolumntype, true);
            t.ColumnExpress = columnexpress;
            t.BelongToObject = belongto;
            t.ConditionValues = new Dictionary<string, object>();
            t.SqlFlags = sqlflags;
            return t;
        }
        
        #endregion
        protected LinqDLRColumn()
        {
        }
        


        #region 动态对象操作实现
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
            if (methodInfo.ToLower() == "count")
            {
                return Count(args);
            }
            if (methodInfo.ToLower() == "max")
            {
                return Sum(args);
            }
            if (methodInfo.ToLower() == "min")
            {
                return Sum(args);
            }
            if (methodInfo.ToLower() == "avg")
            {
                return Avg(args);
            }
            if (methodInfo.ToLower() == "concat")
            {
                return Concat(args);
            }
            if(methodInfo.ToLower() == "substring")
            {
                return SubString(args);
            }
            if (methodInfo.ToLower() == "tostring")
            {
                return ConvertToString(args);
            }
            if (methodInfo.ToLower() == "todatetime")
            {
                return ConvertToDateTime(args);
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
                re = $"{ColumnExpress} {SqlFlags.IsNull}";
            }
            else
            {
                re = $"{ColumnExpress}{SqlFlags.EqualFlag}{Convert2Express(v)}";
            }

            var rtn = new LinqDLR2SqlWhereOperator(re, ConditionValues);

            return rtn;
        }

        protected override object MetaNotEqual(object v)
        {
            var re = "";
            if (v == null || v == DBNull.Value)
            {
                re = $"{ColumnExpress} {SqlFlags.IsNotNull}";
            }
            else
            {
                re = $"{ColumnExpress}{SqlFlags.NotEqualFlag}{Convert2Express(v)}";
            }
            

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }

        protected override object MetaGreaterThan(object v)
        {
            var re = "";
            re = $"{ColumnExpress}{SqlFlags.GreaterFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaGreaterThanEqual(object v)
        {
            var re = "";

            re = $"{ColumnExpress}{SqlFlags.GreaterEqualFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaLessThan(object v)
        {
            var re = "";
            re = $"{ColumnExpress}{SqlFlags.LessFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaLessThanEqual(object v)
        {
            var re = "";
            re = $"{ColumnExpress}{SqlFlags.LessEqualFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaAdd(object v)
        {
            var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);
            var re = "";
            re = $"({ColumnExpress}{SqlFlags.AddFlag}{rtn.Convert2Express(v)})";
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
            var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);
            var re = "";
            re = $"({ColumnExpress}{SqlFlags.DivideFlag}{rtn.Convert2Express(v)})";
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
            var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);
            var re = "";
            re = $"({ColumnExpress}{SqlFlags.MultiplyFlag}{rtn.Convert2Express(v)})";
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
            var rtn = New(this.GetType(),"",this.BelongToObject, SqlFlags);
            var re = "";
            re = $"({ColumnExpress}{SqlFlags.SubstractFlag}{rtn.Convert2Express(v)})";
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
        #endregion
        #region 公开方法
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
                var pname = "" + Convert.ToInt64(ComFunc.RandomCode(6));
                //防止有key重复导致的异常
                while (ConditionValues.ContainsKey(pname))
                {
                    pname = "" + Convert.ToInt64(ComFunc.RandomCode(6));
                }
                ConditionValues.Add(pname, v);

                re = $"{SqlFlags.ParamFlag}{pname}";
            }

            return re;
        }
        #endregion
        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose()
        {
            ColumnExpress = "";
            ConditionValues.Clear();

        }
    }
}
