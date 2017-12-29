using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL
{
    /// <summary>
    /// LinqDLR2SQL用于sql生成的扩展接口
    /// </summary>
    public class LinqDLR2SQLGenerator
    {
        protected LinqDLR2SQLOperation _lastoperation = LinqDLR2SQLOperation.None;
        /// <summary>
        /// 串联的table集合，非join
        /// </summary>
        protected List<object> _selectmanytables = new List<object>();
        protected string _sql = "";
        /// <summary>
        /// join的table集合
        /// </summary>
        protected Dictionary<string, JoinItem> _jointables = new Dictionary<string, JoinItem>();
        protected LinqDLR2SqlWhereOperator _lastwhere = null;
        protected Dictionary<string, object> _conditionvalues = new Dictionary<string, object>();
        protected string _joindirect = "";
        protected string _orderby = "";

        /// <summary>
        /// 条件中的值
        /// </summary>
        public Dictionary<string, object> ConditionValues
        {
            get { return _conditionvalues; }
        }
        /// <summary>
        /// 执行select many的自定义处理
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TLastItem"></typeparam>
        /// <param name="source"></param>
        /// <param name="pretable"></param>
        public virtual void DoSelectMany<TSource, TLastItem>(LinqDLR2Sql<TSource> source, LinqDLR2Sql<TLastItem> pretable)
        {
            _selectmanytables.AddRange(pretable.SQLGenerator._selectmanytables);
            if (pretable.Table != "")
            {
                _selectmanytables.Add(pretable);
            }
            _lastoperation = LinqDLR2SQLOperation.SelectMany;
        }
        /// <summary>
        /// 执行select的自定义处理
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TLastItem"></typeparam>
        /// <param name="source"></param>
        /// <param name="pretable"></param>
        public virtual void DoSelect<TSource, TLastItem>(LinqDLR2Sql<TSource> source, LinqDLR2Sql<TLastItem> pretable)
        {
            if (pretable.SQLGenerator._lastoperation == LinqDLR2SQLOperation.SelectMany)
            {
                _selectmanytables.AddRange(pretable.SQLGenerator._selectmanytables);
                _lastoperation = LinqDLR2SQLOperation.SelectMany;
                _orderby = pretable.SQLGenerator._orderby;
            }
            else if (pretable.SQLGenerator._lastoperation == LinqDLR2SQLOperation.SelectJoin)
            {
                foreach (var item in pretable.SQLGenerator._jointables)
                {
                    _jointables.Add(item.Key, item.Value);
                }
                _lastoperation = LinqDLR2SQLOperation.SelectJoin;
                _orderby = pretable.SQLGenerator._orderby;
            }
            else
            {
                _lastoperation = LinqDLR2SQLOperation.Select;
                _orderby = pretable.SQLGenerator._orderby;
            }
            _lastwhere = pretable.SQLGenerator._lastwhere;
            //生成sql
            if (_lastoperation == LinqDLR2SQLOperation.Select)
            {
                var sql = "SELECT {prefix} {columns} FROM {tables} {alianname} {where} {orderby}";
                var prefix = "";
                var columns = GetColumnsSql(source);

                var tables =  source.Table;
                var alianname = string.IsNullOrEmpty(source.AliasName) ? "" : $"as {source.AliasName}";
                var where = _lastwhere == null ? "" : $"WHERE {_lastwhere.Result}";
                var orderby = _orderby == "" ? "" : $"ORDER BY {_orderby}";
                sql = sql.Replace("{prefix}", prefix)
                    .Replace("{columns}", columns)
                    .Replace("{tables}", tables)
                    .Replace("{alianname}", alianname)
                    .Replace("{where}", where)
                    .Replace("{orderby}", orderby);
                if (_lastwhere != null)
                {
                    foreach (var item in _lastwhere.ConditionValues)
                    {
                        _conditionvalues.Add(item.Key, item.Value);
                    }
                }

                _sql = sql;
            }
            else if (_lastoperation == LinqDLR2SQLOperation.SelectMany)
            {
                var sql = "SELECT {prefix} {columns} FROM {tables} {where} {orderby}";
                var prefix = "";
                var columns = GetColumnsSql(source);
                var tables = "";
                foreach (LinqDLRTable table in _selectmanytables)
                {
                    tables += $"{table.Table}{(string.IsNullOrEmpty(table.AliasName) ? "," : $" AS {table.AliasName},")}";
                }
                tables = tables == "" ? "" : tables.Substring(0, tables.Length - 1);
                var where = _lastwhere == null ? "" : $"WHERE {_lastwhere.Result}";
                var orderby = _orderby == "" ? "" : $"ORDER BY {_orderby}";
                sql = sql.Replace("{prefix}", prefix)
                    .Replace("{columns}", columns)
                    .Replace("{tables}", tables)
                    .Replace("{where}", where)
                    .Replace("{orderby}", orderby);

                if (_lastwhere != null)
                {
                    foreach (var item in _lastwhere.ConditionValues)
                    {
                        _conditionvalues.Add(item.Key, item.Value);
                    }
                }
                _sql = sql;
            }
            else if (_lastoperation == LinqDLR2SQLOperation.SelectJoin)
            {
                var sql = "SELECT {prefix} {columns} FROM {tables} {where} {orderby}";
                var prefix = "";
                var columns = GetColumnsSql(source);
                var tables = "";
                if (_jointables.ContainsKey("from"))
                {
                    var ldt = (LinqDLRTable)_jointables["from"].Table;
                    tables = ldt.Table + (string.IsNullOrEmpty(ldt.AliasName) ? "" : $" AS {ldt.AliasName}");
                }
                foreach (var item in _jointables.Values)
                {
                    tables += $" {item.JoinString}";
                    foreach (var cv in item.ConditionValues)
                    {
                        _conditionvalues.Add(cv.Key, cv.Value);
                    }
                }
                tables = tables == "" ? "" : tables.Substring(0, tables.Length - 1);
                var where = _lastwhere == null ? "" : $"WHERE {_lastwhere.Result}";
                var orderby = _orderby == "" ? "" : $"ORDER BY {_orderby}";
                sql = sql.Replace("{prefix}", prefix)
                    .Replace("{columns}", columns)
                    .Replace("{tables}", tables)
                    .Replace("{where}", where)
                    .Replace("{orderby}", orderby);
                if (_lastwhere != null)
                {
                    foreach (var item in _lastwhere.ConditionValues)
                    {
                        _conditionvalues.Add(item.Key, item.Value);
                    }
                }
                _sql = sql;
            }
        }
        /// <summary>
        /// 执行join操作
        /// </summary>
        /// <typeparam name="TOuterItem"></typeparam>
        /// <typeparam name="TInnerItem"></typeparam>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        public virtual void DoJoin<TSource, TOuterItem, TInnerItem>(LinqDLR2Sql<TSource> source, LinqDLR2Sql<TOuterItem> outer, LinqDLR2Sql<TInnerItem> inner, object outerkey, object innerkey)
        {
            _lastoperation = LinqDLR2SQLOperation.SelectJoin;
            foreach (var item in outer.SQLGenerator._jointables)
            {
                _jointables.Add(item.Key, item.Value);
            }
            foreach (var item in inner.SQLGenerator._jointables)
            {
                _jointables.Add(item.Key, item.Value);
            }
            //outer有table名的时候为主表
            if (!string.IsNullOrEmpty(outer.Table))
            {
                _jointables.Add("from", new JoinItem()
                {
                    Table = outer,
                    JoinString = ""
                });
            }
            if (!string.IsNullOrEmpty(inner.Table))
            {
                var key = $"{inner.SQLGenerator._joindirect}_join_table_{inner.Table}";
                var on = "";
                var pflag = "";
                var conditionvalue = new Dictionary<string, object>();
                //确定paramflag
                if (outerkey is LinqDLRColumn)
                {
                    pflag = ((LinqDLRColumn)outerkey).ParamFlag;
                }
                if (pflag == "" && innerkey is LinqDLRColumn)
                {
                    pflag = ((LinqDLRColumn)innerkey).ParamFlag;
                }
                //构建on条件
                if (outerkey is LinqDLRColumn)
                {
                    on += ((LinqDLRColumn)innerkey).ColumnExpress;
                }
                else
                {
                    var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
                    on += pflag + pname;
                    conditionvalue.Add(pname, outerkey);

                }
                if (innerkey is LinqDLRColumn)
                {
                    on += "=" + ((LinqDLRColumn)innerkey).ColumnExpress;
                }
                else
                {
                    var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
                    on += "=" + pflag + pname;
                    conditionvalue.Add(pname, outerkey);
                }
                if (!_jointables.ContainsKey(key))
                {
                    _jointables.Add(key, new JoinItem()
                    {
                        Table = inner,
                        JoinCondition = on,
                        ConditionValues = conditionvalue,
                        JoinString = $"{inner.SQLGenerator._joindirect.ToUpper()} JOIN {inner.Table} {(string.IsNullOrEmpty(inner.AliasName) ? "" : $"AS {inner.AliasName}")} {(on == "" ? "" : $"ON {on}")}"
                    });
                }
            }
        }
        public virtual void DoOrderBy<TSource>(LinqDLR2Sql<TSource> source, object key)
        {
            if (key is LinqDLRColumn)
            {
                var ldc = (LinqDLRColumn)key;

                _orderby += _orderby == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress}";
            }
            else if (source.Item.GetType().Name.IndexOf("LamdaSQLObject`1") >= 0)
            {
                //不处理
            }
            else
            {
                var fields = source.Item.GetType().GetTypeInfo().DeclaredFields;
                foreach (var f in fields)
                {
                    var v = f.GetValue(source.Item);
                    if (v is LinqDLRColumn)
                    {
                        var ldc = (LinqDLRColumn)v;
                        _orderby += _orderby == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress}";
                    }
                }
            }
        }
        public virtual void DoOrderByDescending<TSource>(LinqDLR2Sql<TSource> source, object key)
        {
            if (key is LinqDLRColumn)
            {
                var ldc = (LinqDLRColumn)key;

                _orderby += _orderby == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress} DESC";
            }
            else if (source.Item.GetType().Name.IndexOf("LamdaSQLObject`1") >= 0)
            {
                //不处理
            }
            else
            {
                var fields = source.Item.GetType().GetTypeInfo().DeclaredFields;
                foreach (var f in fields)
                {
                    var v = f.GetValue(source.Item);
                    if (v is LinqDLRColumn)
                    {
                        var ldc = (LinqDLRColumn)v;
                        _orderby += _orderby == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress} DESC";
                    }
                }
            }
        }
        /// <summary>
        /// 执行where的自定义处理
        /// </summary>
        /// <param name="where"></param>
        public virtual void DoWhere<TSource>(LinqDLR2Sql<TSource> source, LinqDLR2SqlWhereOperator where)
        {
            _lastwhere = where;
        }
        /// <summary>
        /// 转为sql语句
        /// </summary>
        /// <returns></returns>
        public virtual string ToSql()
        {
            return _sql;
        }
        /// <summary>
        /// 本表join的时候使用left方式
        /// </summary>
        public void DoLeftJoin<TSource>(LinqDLR2Sql<TSource> source)
        {
            _joindirect = "left";
        }
        /// <summary>
        /// 本表join的时候使用right方式
        /// </summary>
        public void DoRightJoin<TSource>(LinqDLR2Sql<TSource> source)
        {
            _joindirect = "right";
        }
        private string GetColumnsSql<TSource>(LinqDLR2Sql<TSource> source)
        {
            var dt = DateTime.Now;
            var columns = "";
            if (source.Item is LinqDLRColumn)
            {
                columns += ((LinqDLRColumn)(object)source.Item).ColumnExpress;
            }
            else if (source.Item.GetType().Name.IndexOf("LamdaSQLObject`1") >= 0)
            {
                var tn = ComFunc.nvl(source.Item.GetType().GetTypeInfo().GetDeclaredProperty("BelongToTable").GetValue(source.Item));
                columns += string.IsNullOrEmpty(tn) ? $"*" : $"{tn}.*";
            }
            else
            {
                var fields = source.Item.GetType().GetTypeInfo().DeclaredFields;
                foreach (var f in fields)
                {
                    var v = f.GetValue(source.Item);
                    if (v is LinqDLRColumn)
                    {
                        columns += ((LinqDLRColumn)v).ColumnExpress + ",";
                    }
                }
            }
            if (columns.EndsWith(",")) columns = columns.Substring(0, columns.Length - 1);
            return columns;
        }
    }

    public class JoinItem
    {
        public JoinItem()
        {
            ConditionValues = new Dictionary<string, object>();
            JoinString = "";
            JoinCondition = "";
        }
        public object Table { get; set; }
        public string JoinString { get; set; }
        public string JoinCondition { get; set; }
        public Dictionary<string, object> ConditionValues { get; set; }
    }
}
