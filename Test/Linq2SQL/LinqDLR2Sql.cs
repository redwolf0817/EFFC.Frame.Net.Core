using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Test.Linq2SQL
{

    public enum LinqDLR2SQLOperation
    {
        None,
        Select,
        SelectMany,
        SelectJoin,
        Delete,
        Update,
        Insert
    }
    internal class JoinItem
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
    public class LinqDLR2Sql<TSource>
    {
        LinqDLR2SQLOperation _lastoperation = LinqDLR2SQLOperation.None;
        //串联的table集合，非join
        List<object> _selectmanytables = new List<object>();
        //join的table集合
        
        string _sql = "";
        Dictionary<string, JoinItem> _jointables = new Dictionary<string, JoinItem>();
        LinqDLR2SqlWhereOperator _lastwhere = null;
        Dictionary<string, object> _conditionvalues = new Dictionary<string, object>();
        string _joindirect = "";
        string _orderby = "";
        /// <summary>
        /// LinqDLR2Sql下的元素对象
        /// </summary>
        public TSource Item
        {
            get;
            protected set;
        }
        /// <summary>
        /// 表名
        /// </summary>
        public string Table
        {
            get;
            private set;
        }
        /// <summary>
        /// 别名
        /// </summary>
        public string AliasName
        {
            get;
            private set;
        }
        /// <summary>
        /// 自己对象的引用
        /// </summary>
        public object Me
        {
            get;
            protected set;
        }
        /// <summary>
        /// 条件中的值
        /// </summary>
        public Dictionary<string,object> ConditionValues
        {
            get { return _conditionvalues; }
        }
        /// <summary>
        /// 从selectmany中分离出来的tables
        /// </summary>
        protected List<object> SelectManyTables
        {
            get
            {
                return _selectmanytables;
            }
        }
        /// <summary>
        /// 上一次的操作
        /// </summary>
        protected LinqDLR2SQLOperation LastOperation
        {
            get
            {
                return _lastoperation;
            }
        }
        /// <summary>
        /// 上一次的where操作
        /// </summary>
        protected LinqDLR2SqlWhereOperator LastWhere
        {
            get { return _lastwhere; }
        }
        public static T New<T>(TSource item, string table = "", string aliasName = "") where T : LinqDLR2Sql<TSource>
        {
            var rtn = (T)Activator.CreateInstance(typeof(T), true);//new LinqTable<TSource>();
            rtn.Item = item;
            rtn.Table = table;
            rtn.AliasName = string.IsNullOrEmpty(aliasName) ? rtn.Table : aliasName;
            rtn.Me = rtn;
            return rtn;
        }
        /// <summary>
        /// 执行select many的自定义处理
        /// </summary>
        /// <typeparam name="TLastItem"></typeparam>
        /// <param name="pretable"></param>
        public virtual void DoSelectMany<TLastItem>(LinqDLR2Sql<TLastItem> pretable)
        {
            _selectmanytables.AddRange(pretable._selectmanytables);
            if (pretable.Table != "")
            {
                _selectmanytables.Add(pretable);
            }
            _lastoperation = LinqDLR2SQLOperation.SelectMany;
        }
        /// <summary>
        /// 执行select的自定义处理
        /// </summary>
        /// <typeparam name="TLastItem"></typeparam>
        /// <param name="pretable"></param>
        public virtual void DoSelect<TLastItem>(LinqDLR2Sql<TLastItem> pretable)
        {
            if (pretable.LastOperation == LinqDLR2SQLOperation.SelectMany)
            {
                _selectmanytables.AddRange(pretable._selectmanytables);
                _lastoperation = LinqDLR2SQLOperation.SelectMany;
                _orderby = pretable._orderby;
            }
            else if (pretable.LastOperation == LinqDLR2SQLOperation.SelectJoin)
            {
                foreach(var item in pretable._jointables)
                {
                    _jointables.Add(item.Key, item.Value);
                }
                _lastoperation = LinqDLR2SQLOperation.SelectJoin;
                _orderby = pretable._orderby;
            }
            else
            {
                _lastoperation = LinqDLR2SQLOperation.Select;
                _orderby = pretable._orderby;
            }
            _lastwhere = pretable.LastWhere;
            //生成sql
            if (LastOperation == LinqDLR2SQLOperation.Select)
            {
                var sql = "SELECT {prefix} {columns} FROM {tables} {alianname} {where} {orderby}";
                var prefix = "";
                var columns = GetColumnsSql();
                
                var tables = Table;
                var alianname = string.IsNullOrEmpty(AliasName) ? "" : $"as {AliasName}";
                var where = LastWhere == null ? "" : $"WHERE {LastWhere.Result}";
                var orderby = _orderby == "" ? "" : $"ORDER BY {_orderby}";
                sql = sql.Replace("{prefix}", prefix)
                    .Replace("{columns}", columns)
                    .Replace("{tables}", tables)
                    .Replace("{alianname}", alianname)
                    .Replace("{where}", where)
                    .Replace("{orderby}", orderby);
                if (LastWhere != null)
                {
                    foreach (var item in LastWhere.ConditionValues)
                    {
                        _conditionvalues.Add(item.Key, item.Value);
                    }
                }
                
                _sql = sql;
            }
            else if (LastOperation == LinqDLR2SQLOperation.SelectMany)
            {
                var sql = "SELECT {prefix} {columns} FROM {tables} {where} {orderby}";
                var prefix = "";
                var columns = GetColumnsSql();
                var tables = "";
                foreach (LinqDLRTable table in SelectManyTables)
                {
                    tables += $"{table.Table}{(string.IsNullOrEmpty(table.AliasName) ? "," : $" AS {table.AliasName},")}";
                }
                tables = tables == "" ? "" : tables.Substring(0, tables.Length - 1);
                var where = LastWhere == null ? "" : $"WHERE {LastWhere.Result}";
                var orderby = _orderby == "" ? "" : $"ORDER BY {_orderby}";
                sql = sql.Replace("{prefix}", prefix)
                    .Replace("{columns}", columns)
                    .Replace("{tables}", tables)
                    .Replace("{where}", where)
                    .Replace("{orderby}", orderby);

                if (LastWhere != null)
                {
                    foreach (var item in LastWhere.ConditionValues)
                    {
                        _conditionvalues.Add(item.Key, item.Value);
                    }
                }
                _sql = sql;
            }
            else if (LastOperation == LinqDLR2SQLOperation.SelectJoin)
            {
                var sql = "SELECT {prefix} {columns} FROM {tables} {where} {orderby}";
                var prefix = "";
                var columns = GetColumnsSql();
                var tables = "";
                if (_jointables.ContainsKey("from"))
                {
                    var ldt = (LinqDLRTable)_jointables["from"].Table;
                    tables = ldt.Table + (string.IsNullOrEmpty(ldt.AliasName) ? "" : $" AS {ldt.AliasName}");
                }
                foreach (var item in _jointables.Values)
                {
                    tables += $" {item.JoinString}";
                    foreach(var cv in item.ConditionValues)
                    {
                        _conditionvalues.Add(cv.Key, cv.Value);
                    }
                }
                tables = tables == "" ? "" : tables.Substring(0, tables.Length - 1);
                var where = LastWhere == null ? "" : $"WHERE {LastWhere.Result}";
                var orderby = _orderby == "" ? "" : $"ORDER BY {_orderby}";
                sql = sql.Replace("{prefix}", prefix)
                    .Replace("{columns}", columns)
                    .Replace("{tables}", tables)
                    .Replace("{where}", where)
                    .Replace("{orderby}", orderby);
                if (LastWhere != null)
                {
                    foreach (var item in LastWhere.ConditionValues)
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
        public virtual void DoJoin<TOuterItem,TInnerItem>(LinqDLR2Sql<TOuterItem> outer,LinqDLR2Sql<TInnerItem> inner,object outerkey,object innerkey)
        {
            _lastoperation = LinqDLR2SQLOperation.SelectJoin;
            foreach (var item in outer._jointables)
            {
                _jointables.Add(item.Key, item.Value);
            }
            foreach (var item in inner._jointables)
            {
                _jointables.Add(item.Key, item.Value);
            }
            //outer有table名的时候为主表
            if (!string.IsNullOrEmpty(outer.Table))
            {
                _jointables.Add("from", new JoinItem() {
                    Table = outer,
                    JoinString = ""
                });
            }
            if (!string.IsNullOrEmpty(inner.Table))
            {
                var key = $"{inner._joindirect}_join_table_{inner.Table}";
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
                        JoinString = $"{inner._joindirect.ToUpper()} JOIN {inner.Table} {(string.IsNullOrEmpty(inner.AliasName) ? "" : $"AS {inner.AliasName}")} {(on == "" ? "" : $"ON {on}")}"
                    });
                }
            }
        }
        public virtual void DoOrderBy(object key)
        {
            if(key is LinqDLRColumn)
            {
                var ldc = (LinqDLRColumn)key;

                _orderby += _orderby == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress}";
            }
            else if (Item.GetType().Name.IndexOf("LamdaSQLObject`1") >= 0)
            {
                //不处理
            }
            else
            {
                var fields = Item.GetType().GetTypeInfo().DeclaredFields;
                foreach (var f in fields)
                {
                    var v = f.GetValue(Item);
                    if (v is LinqDLRColumn)
                    {
                        var ldc = (LinqDLRColumn)v;
                        _orderby += _orderby == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress}";
                    }
                }
            }
        }
        public virtual void DoOrderByDescending(object key)
        {
            if (key is LinqDLRColumn)
            {
                var ldc = (LinqDLRColumn)key;

                _orderby += _orderby == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress} DESC";
            }
            else if (Item.GetType().Name.IndexOf("LamdaSQLObject`1") >= 0)
            {
                //不处理
            }
            else
            {
                var fields = Item.GetType().GetTypeInfo().DeclaredFields;
                foreach (var f in fields)
                {
                    var v = f.GetValue(Item);
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
        public virtual void DoWhere(LinqDLR2SqlWhereOperator where)
        {
            _lastwhere = where;
        }
        /// <summary>
        /// 转为sql语句
        /// </summary>
        /// <returns></returns>
        public virtual string toSql()
        {
            return _sql;
        }
        /// <summary>
        /// 本表join的时候使用left方式
        /// </summary>
        public LinqDLR2Sql<TSource> LeftJoin()
        {
            _joindirect = "left";
            return this;
        }
        /// <summary>
        /// 本表join的时候使用right方式
        /// </summary>
        public LinqDLR2Sql<TSource> RightJoin()
        {
            _joindirect = "right";
            return this;
        }
        private string GetColumnsSql()
        {
            var dt = DateTime.Now;
            var columns = "";
            if (Item is LinqDLRColumn)
            {
                columns += ((LinqDLRColumn)(object)Item).ColumnExpress;
            }
            else if (Item.GetType().Name.IndexOf("LamdaSQLObject`1")>= 0)
            {
                var tn = ComFunc.nvl(Item.GetType().GetTypeInfo().GetDeclaredProperty("BelongToTable").GetValue(Item));
                columns += string.IsNullOrEmpty(tn) ? $"*" : $"{tn}.*";
            }
            else
            {
                var fields = Item.GetType().GetTypeInfo().DeclaredFields;
                foreach (var f in fields)
                {
                    var v = f.GetValue(Item);
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

    public static class LinqDLR2SqlExtend
    {
        /// <summary>
        /// 做多重from筛选结果集
        /// lamda推理逻辑：
        /// 1.lamda根据泛型来进行推算，因此SelectMany的算法只适用于泛型
        /// 2.因为涉及到多重推算，即多个from，所以lamda会根据返回结果推算运用到参加下一次推算的TSource结构，
        ///   即从上自下进行from推算，第一个和第二个from根据resultSelector算出了TResult的结构，返回值必须返回一个带TResult的对象，不然后面的推算就会失败，
        ///   上面算出来的结果会参与到第三个的from运算，lamda根据上次返回的TResult作为这次运算中TSource的结构定义来参与本次运算。
        ///   后面依次类推。
        ///   所以resultSelector返回值TResult必须包含在返回值的结构定义中，不然递归推算就断裂
        ///   所以selectmany只能用泛型来做定义
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TCollector"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="from"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TResult> SelectMany<TSource, TCollector, TResult>(this LinqDLR2Sql<TSource> source, Func<TSource, LinqDLR2Sql<TCollector>> from, Func<TSource, TCollector, TResult> resultSelector)
        {
            //第一轮from
            var fromtable = from.Invoke(source.Item);
            var rtn = LinqDLR2Sql<TResult>.New<LinqDLR2Sql<TResult>>(resultSelector.Invoke(source.Item, fromtable.Item));
            rtn.DoSelectMany<TSource>(source);
            rtn.DoSelectMany<TCollector>(fromtable);

            return rtn;
        }
        /// <summary>
        /// 执行select操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TResult> Select<TSource, TResult>(this LinqDLR2Sql<TSource> source, Func<TSource, TResult> selector)
        {
            var dt = DateTime.Now;
            var v = selector.Invoke(source.Item);
            var rtn = LinqDLR2Sql<TResult>.New<LinqDLR2Sql<TResult>>(v, source.Table, source.AliasName);
            rtn.DoSelect<TSource>(source);
            return rtn;
        }

        public static LinqDLR2Sql<TResult> Join<TOuter, TInner, TKey, TResult>(this LinqDLR2Sql<TOuter> outer, LinqDLR2Sql<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            var v1 = outerKeySelector.Invoke(outer.Item);
            var v2 = innerKeySelector.Invoke(inner.Item);
            var re = resultSelector.Invoke(outer.Item, inner.Item);
            var rtn = LinqDLR2Sql<TResult>.New<LinqDLR2Sql<TResult>>(re);
            rtn.DoJoin<TOuter, TInner>(outer, inner, v1, v2);
            return rtn;
        }
        /// <summary>
        /// 执行linqDLR2Sql的where操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> Where<TSource>(this LinqDLR2Sql<TSource> source, Func<TSource, LinqDLR2SqlWhereOperator> predicate)
        {
            LinqDLR2SqlWhereOperator op = predicate.Invoke(source.Item);
            source.DoWhere(op);
            return source;
        }
        /// <summary>
        /// 正向排序操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> OrderBy<TSource, TKey>(this LinqDLR2Sql<TSource> source, Func<TSource, TKey> keySelector)
        {
            var key = keySelector.Invoke(source.Item);
            source.DoOrderBy(key);
            return source;
        }
        /// <summary>
        /// 正向排序操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> ThenBy<TSource, TKey>(this LinqDLR2Sql<TSource> source, Func<TSource, TKey> keySelector)
        {
            var key = keySelector.Invoke(source.Item);
            source.DoOrderBy(key);
            return source;
        }
        /// <summary>
        /// 逆向排序操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> OrderByDescending<TSource, TKey>(this LinqDLR2Sql<TSource> source, Func<TSource, TKey> keySelector)
        {
            var key = keySelector.Invoke(source.Item);
            source.DoOrderByDescending(key);
            return source;
        }
        /// <summary>
        /// 逆向排序操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> ThenByDescending<TSource, TKey>(this LinqDLR2Sql<TSource> source, Func<TSource, TKey> keySelector)
        {
            var key = keySelector.Invoke(source.Item);
            source.DoOrderByDescending(key);
            return source;
        }
    }
}
