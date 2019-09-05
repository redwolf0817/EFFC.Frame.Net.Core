using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL
{
    /// <summary>
    /// 用于LinqDLR2Sql构建动态栏位使用
    /// </summary>
    /// <typeparam name="TColumn"></typeparam>
    public class LamdaSQLObject<TColumn> : MyDynamicMetaProvider where TColumn:LinqDLRColumn
    {
        #region 私有变量
        Dictionary<string, LinqDLRColumn> columns = new Dictionary<string, LinqDLRColumn>();
        #endregion
        #region 公共属性
        /// <summary>
        /// 所属table
        /// </summary>
        public string BelongToTable
        {
            get;
            set;
        }
        /// <summary>
        /// SQL相关的操作符号
        /// </summary>
        public SqlOperatorFlags SqlFlags
        {
            get;private set;
        }
        #endregion
        /// <summary>
        /// 构建Lamda表达式对象
        /// </summary>
        /// <param name="table"></param>
        /// <param name="sqlflags"></param>
        public LamdaSQLObject(string table,SqlOperatorFlags sqlflags)
        {
            BelongToTable = table;
            SqlFlags = sqlflags;
        }
        #region SQL扩展方法
        /// <summary>
        /// 将c#的变量转化成LinqDLRColumn以便进行相关where操作
        /// </summary>
        /// <param name="args">args中最少有一个参数，如果有两个参数第一个参数必须为变量名称，第二个才是变量的值，如果只有一个参数则该参数为变量的值</param>
        /// <returns></returns>
        protected virtual object ConvertVariableToLinqDLRColumn(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = args.Length > 1 ? ComFunc.nvl(args[0]) : "" + Convert.ToInt64(ComFunc.RandomCode(6));
            var value = args.Length > 1 ? args[1] : args[0];

            var tmp = LinqDLRColumn.New<TColumn>("", null,SqlFlags);
            var columnexpress = $"{SqlFlags.ParamFlag}{pname}";
            var ldr = LinqDLRColumn.New<TColumn>(columnexpress, this, SqlFlags);
            ldr.ConditionValues.Add(pname, value);

            tmp = null;
            return ldr;
        }
        /// <summary>
        /// 将一个常量转化成LinqDLRColumn以便进行相关where操作
        /// </summary>
        /// <param name="args">只有一个参数</param>
        /// <returns></returns>
        protected virtual object ConvertConstaValueToLinqDLRColumn(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(6));
            var value = args[0];

            var tmp = LinqDLRColumn.New<TColumn>("", null, SqlFlags);
            var columnexpress = $"{SqlFlags.ParamFlag}{pname}";
            var ldr = LinqDLRColumn.New<TColumn>(columnexpress, this,SqlFlags);
            ldr.ConditionValues.Add(pname, value);
            tmp = null;
            return ldr;
        }
        protected virtual object IfNotNullThen(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            //第一个参数为一个值
            var value1 = args[0];
            //第二个参数为LinqDLR2SqlWhereOperator
            if (args.Length < 2) return null;
            if(args[1] is LinqDLR2SqlWhereOperator)
            {
                if(value1 != null && ComFunc.nvl(value1) != "")
                {
                    return (LinqDLR2SqlWhereOperator)args[1];
                }
                else
                {
                    return new LinqDLR2SqlWhereOperator("", null);
                }
            }
            else
            {
                return new LinqDLR2SqlWhereOperator("", null);
            }
        }
        protected virtual object IfNullThen(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            //第一个参数为一个值
            var value1 = args[0];
            //第二个参数为LinqDLR2SqlWhereOperator
            if (args.Length < 2) return null;
            if (args[1] is LinqDLR2SqlWhereOperator)
            {
                if (value1 == null && ComFunc.nvl(value1) == "")
                {
                    return (LinqDLR2SqlWhereOperator)args[1];
                }
                else
                {
                    return new LinqDLR2SqlWhereOperator("", null);
                }
            }
            else
            {
                return new LinqDLR2SqlWhereOperator("", null);
            }
        }
        protected virtual object Append(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            //第一个参数为LinqDLR2SqlWhereOperator，后面参数为string
            var where = args[0];
            if(where is LinqDLR2SqlWhereOperator)
            {
                using (var lwhere = (LinqDLR2SqlWhereOperator)where)
                {
                    var conditions = new Dictionary<string, object>();
                    foreach(var c in lwhere.ConditionValues)
                    {
                        conditions.Add(c.Key, c.Value);
                    }
                    var appendstr = "";
                    for (int i = 1; i < args.Length; i++)
                    {
                        if (args[i] is string)
                        {
                            appendstr += args[i];
                        }
                    }
                    var newwhere = new LinqDLR2SqlWhereOperator(lwhere.Result + appendstr, conditions);
                    return newwhere;
                }
            }
            else
            {
                return where;
            }
        }
        protected virtual object Convert2FunctionExpress(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            //第一个参数为string,为sql中function的名称，后面参数为function的参数
            var func_name = ComFunc.nvl(args[0]);
            var express = $"{func_name}(#args#)";
            var args_express = "";
            var tmp = LinqDLRColumn.New<TColumn>("", null, SqlFlags);
            if (args.Length > 1)
            {
                for(var i = 1; i < args.Length; i++)
                {
                    var arg = args[i];
                    if (arg is LinqDLRColumn)
                    {
                        args_express += "," + tmp.Convert2Express(arg);
                    }
                    else if (arg is string)
                    {
                        args_express += $",'{arg}'";
                    }
                    else
                    {
                        args_express += $",{arg}";
                    }
                }
            }
            args_express = args_express.Length > 0 ? args_express.Substring(1) : args_express;
            express = express.Replace("#args#", args_express);
            tmp.ColumnExpress = express;

            return tmp;
        }
        protected virtual object Convert2Column(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            var key = ComFunc.nvl(args[0]);
            return GetMetaValue(key);
        }
        protected virtual object Convert2DateTime(object[] args)
        {
            throw new NotImplementedException("未实现LamdaSQLObject.Convert2DateTime");
        }
        #endregion
        #region 动态对象扩展实现
        protected override object GetMetaValue(string key)
        {
            //if (!columns.ContainsKey(key.ToLower()))
            //{
            //    var tmp = LinqDLRColumn.New<TColumn>("", this, SqlFlags);
            //    tmp.ColumnExpress = $"{(BelongToTable == "" ? "" : BelongToTable + ".")}{string.Format(SqlFlags.Column_Quatation,key)}";
            //    columns.Add(key.ToLower(), tmp);
            //}
            //return columns[key.ToLower()];


            var tmp = LinqDLRColumn.New<TColumn>("", this, SqlFlags);
            tmp.ColumnExpress = $"{(BelongToTable == "" ? "" : BelongToTable + ".")}{string.Format(SqlFlags.Column_Quatation, key)}";
            
            //columns.Add(key.ToLower(), tmp);
            return tmp;
        }

        protected override object InvokeMe(string methodInfo, params object[] args)
        {
            if (methodInfo.ToLower() == "v")
            {
                return ConvertVariableToLinqDLRColumn(args);
            }
            if (methodInfo.ToLower() == "c")
            {
                return ConvertConstaValueToLinqDLRColumn(args);
            }
            if (methodInfo.ToLower() == "notnull")
            {
                return IfNotNullThen(args);
            }
            if (methodInfo.ToLower() == "ifnull")
            {
                return IfNullThen(args);
            }
            if (methodInfo.ToLower() == "append")
            {
                return Append(args);
            }
            if (methodInfo.ToLower() == "tofunction")
            {
                return Convert2FunctionExpress(args);
            }
            if (methodInfo.ToLower() == "column")
            {
                return Convert2Column(args);
            }
            if (methodInfo.ToLower() == "todatetime")
            {
                return Convert2DateTime(args);
            }

            return this;
        }

        protected override object SetMetaValue(string key, object value)
        {
            return this;
        }
        #endregion
        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            columns.Clear();
        }
    }
}
