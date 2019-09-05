using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL
{
    /// <summary>
    /// LinqDLR2SQL生成器，用于LinqDLR2SQL生成sql，可以继承和扩展
    /// </summary>
    public abstract class LinqDLR2SQLGenerator : IDisposable
    {
        #region 需继承实现的方法
        /// <summary>
        /// 设定SQL相关的符号标记
        /// </summary>
        public abstract SqlOperatorFlags SqlFlags
        {
            get;
        }

        #endregion;
        #region 内部变量
        /// <summary>
        /// sql语句
        /// </summary>
        private string _sql = "";
        /// <summary>
        /// join的方向，left，right等
        /// </summary>
        private string _joindirect = "";
        #endregion
        #region 对外变量
        /// <summary>
        /// 条件中的值
        /// </summary>
        public Dictionary<string, object> ConditionValues { get; private set; } = new Dictionary<string, object>();
        /// <summary>
        /// 当前操作类型
        /// </summary>
        public LinqDLR2SQLOperation CurrentOperation { get; private set; } = LinqDLR2SQLOperation.None;
        /// <summary>
        /// Join的table合集
        /// </summary>
        protected Dictionary<string, JoinItem> Jointables { get; private set; } = new Dictionary<string, JoinItem>();
        /// <summary>
        /// sql中的前缀修饰词
        /// </summary>
        protected string Prefix { get; private set; } = "";
        /// <summary>
        /// 串联的table集合，非join
        /// </summary>
        protected List<object> Selectmanytables { get; private set; } = new List<object>();
        /// <summary>
        /// 排序表达式
        /// </summary>
        protected string Orderby { get; private set; } = "";
        /// <summary>
        /// sql中的尾部词
        /// </summary>
        protected string Pearfix { get; set; } = "";
        /// <summary>
        /// sql中from后面的table
        /// </summary>
        protected string Tables { get; set; } = "";
        /// <summary>
        /// where条件
        /// </summary>
        protected LinqDLR2SqlWhereOperator Lastwhere { get; set; } = null;
        /// <summary>
        /// where表达式
        /// </summary>
        protected string Where { get; set; } = "";
        /// <summary>
        /// sql中的栏位项
        /// </summary>
        protected string Columns { get; set; } = "";
        /// <summary>
        /// Group by 表达式
        /// </summary>
        protected string GroupBy { get; set; } = "";
        #endregion
        #region 内部方法
        private void BuildSQL<TSource>(LinqDLR2Sql<TSource> source)
        {
            Columns = GetColumnsSql(source);


            _sql = "SELECT {prefix} {columns} FROM {tables} {where} {groupby} {orderby} {pearfix}";
            var tables = "";
            //生成sql
            if ((CurrentOperation & LinqDLR2SQLOperation.Select) == LinqDLR2SQLOperation.Select)
            {
                tables = source.Table;
                var alianname = string.IsNullOrEmpty(source.AliasName) ? "" : $" {source.AliasName}";
                tables += alianname;
            }
            else if ((CurrentOperation & LinqDLR2SQLOperation.SelectMany) == LinqDLR2SQLOperation.SelectMany)
            {
                tables = "";
                foreach (LinqDLRTable table in Selectmanytables)
                {
                    tables += $"{table.Table}{(string.IsNullOrEmpty(table.AliasName) ? "," : $" {table.AliasName},")}";
                }
                tables = tables == "" ? "" : tables.Substring(0, tables.Length - 1);
            }
            else if ((CurrentOperation & LinqDLR2SQLOperation.SelectJoin) == LinqDLR2SQLOperation.SelectJoin)
            {
                if (Jointables.ContainsKey("from"))
                {
                    var ldt = (LinqDLRTable)Jointables["from"].Table;
                    tables = ldt.Table + (string.IsNullOrEmpty(ldt.AliasName) ? "" : $" {ldt.AliasName}");
                }
                foreach (var item in Jointables.Values)
                {
                    tables += $" {item.JoinString}";
                    foreach (var cv in item.ConditionValues)
                    {
                        ConditionValues.Add(cv.Key, cv.Value);
                    }
                }
                tables = tables == "" ? "" : tables;
            }
            else if ((CurrentOperation & LinqDLR2SQLOperation.Where) == LinqDLR2SQLOperation.Where)
            {
                tables = source.Table;
                var alianname = string.IsNullOrEmpty(source.AliasName) ? "" : $" {source.AliasName}";
                tables += alianname;
            }
            else if ((CurrentOperation & LinqDLR2SQLOperation.OrderBy) == LinqDLR2SQLOperation.OrderBy)
            {
                tables = source.Table;
                var alianname = string.IsNullOrEmpty(source.AliasName) ? "" : $" {source.AliasName}";
                tables += alianname;
            }

            Tables = tables;
        }
        private string GetColumnsSql<TSource>(LinqDLR2Sql<TSource> source)
        {
            if ((source.SQLGenerator.CurrentOperation & LinqDLR2SQLOperation.GroupBy) == LinqDLR2SQLOperation.GroupBy)
            {
                return GetColumnsSqlFrom(source.Item, true);
            }
            else
            {
                return GetColumnsSqlFrom(source.Item);
            }

        }
        private string GetColumnsSqlFrom(object source, bool isGroupby = false)
        {
            if (source == null) return "";

            var dt = DateTime.Now;
            var columns = "";

            if (source is LinqDLRColumn)
            {
                columns += ((LinqDLRColumn)(object)source).ColumnExpress;
                if (((LinqDLRColumn)(object)source).ConditionValues != null)
                {
                    foreach (var item in ((LinqDLRColumn)(object)source).ConditionValues)
                    {
                        if (ConditionValues.ContainsKey(item.Key) && ConditionValues[item.Key] == item.Value)
                        {
                            continue;
                        }
                        else
                        {
                            ConditionValues.Add(item.Key, item.Value);
                        }
                    }
                }
            }
            else if (ComFunc.IsImplementedRawGeneric(source.GetType(), typeof(LamdaSQLObject<>)))
            {
                if (!isGroupby)
                {
                    var tn = ComFunc.nvl(source.GetType().GetTypeInfo().GetProperty("BelongToTable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).GetValue(source));
                    columns += string.IsNullOrEmpty(tn) ? $"*" : $"{tn}.*";
                }
                else
                {
                    var tmp = source.GetType().GetTypeInfo().GetDeclaredField("columns").GetValue(source);
                    var carray = tmp == null ? new Dictionary<string, LinqDLRColumn>() : (Dictionary<string, LinqDLRColumn>)tmp;
                    foreach (var c in carray)
                    {
                        columns += $"{c.Value.ColumnExpress},";
                        foreach (var item in (c.Value.ConditionValues))
                        {
                            if (ConditionValues.ContainsKey(item.Key) && ConditionValues[item.Key] == item.Value)
                            {
                                continue;
                            }
                            else
                            {
                                ConditionValues.Add(item.Key, item.Value);
                            }
                        }
                    }
                }
            }
            else
            {
                var fields = source.GetType().GetTypeInfo().DeclaredProperties;
                foreach (var f in fields)
                {
                    var column_quatation = SqlFlags.Column_Quatation;



                    var v = f.GetValue(source);
                    if (v is LinqDLRColumn)
                    {
                        columns += $"{((LinqDLRColumn)v).ColumnExpress} AS {string.Format(column_quatation, f.Name)},";
                        if (((LinqDLRColumn)v).ConditionValues != null)
                        {
                            foreach (var item in ((LinqDLRColumn)v).ConditionValues)
                            {
                                if (ConditionValues.ContainsKey(item.Key) && ConditionValues[item.Key] == item.Value)
                                {
                                    continue;
                                }
                                else
                                {
                                    ConditionValues.Add(item.Key, item.Value);
                                }
                            }
                        }
                    }
                    else if (v is int
                       || v is double
                       || v is Int64
                       || v is float
                       || v is decimal)
                    {
                        columns += $"{v} AS {string.Format(column_quatation, f.Name)},";
                    }
                    else if (v is string)
                    {
                        columns += $"'{v}' AS {string.Format(column_quatation, f.Name)},";
                    }
                    else if (v is DateTime)
                    {
                        columns += $"'{((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss")}' AS {string.Format(column_quatation, f.Name)},";
                    }
                }
            }
            if (columns == "")
            {
                if (isGroupby) columns = GroupBy;
            }
            if (columns.EndsWith(",")) columns = columns.Substring(0, columns.Length - 1);
            return columns;
        }
        /// <summary>
        /// 获取一个Column对象，可扩展
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual LinqDLRColumn NewColumnInstance(object value)
        {
            return LinqDLRColumn.New<LinqDLRColumn>(value, SqlFlags);
        }
        #endregion
        #region Linq相关方法
        /// <summary>
        /// 执行select的自定义处理
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        public virtual void DoSelect<TSource>(LinqDLR2Sql<TSource> source)
        {
            if ((CurrentOperation & LinqDLR2SQLOperation.SelectMany) == LinqDLR2SQLOperation.SelectMany)
            {
                CurrentOperation = LinqDLR2SQLOperation.SelectMany;
            }
            else if ((CurrentOperation & LinqDLR2SQLOperation.SelectJoin) == LinqDLR2SQLOperation.SelectJoin)
            {
                CurrentOperation = LinqDLR2SQLOperation.SelectJoin;
            }
            else
            {
                CurrentOperation = LinqDLR2SQLOperation.Select;
            }


            if (Lastwhere != null)
            {
                foreach (var item in Lastwhere.ConditionValues)
                {
                    if (!ConditionValues.ContainsKey(item.Key))
                        ConditionValues.Add(item.Key, item.Value);
                }
            }
            if ((CurrentOperation & LinqDLR2SQLOperation.GroupBy) == LinqDLR2SQLOperation.GroupBy)
            {
                CurrentOperation = CurrentOperation | LinqDLR2SQLOperation.GroupBy;
            }
            BuildSQL(source);

        }
        /// <summary>
        /// 执行join操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOuterItem"></typeparam>
        /// <typeparam name="TInnerItem"></typeparam>
        /// <param name="source"></param>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerkey"></param>
        /// <param name="innerkey"></param>
        public virtual void DoJoin<TSource, TOuterItem, TInnerItem>(LinqDLR2Sql<TSource> source, LinqDLR2Sql<TOuterItem> outer, LinqDLR2Sql<TInnerItem> inner, object outerkey, object innerkey)
        {
            CurrentOperation = LinqDLR2SQLOperation.SelectJoin;
            //foreach (var item in ((LinqDLR2SQLGenerator)outer.SQLGenerator).Jointables)
            //{
            //    Jointables.Add(item.Key, (JoinItem)item.Value.Clone());
            //}
            foreach (var item in ((LinqDLR2SQLGenerator)inner.SQLGenerator).Jointables)
            {
                Jointables.Add(item.Key, (JoinItem)item.Value.Clone());
            }
            //outer有table名的时候为主表
            if (!string.IsNullOrEmpty(outer.Table))
            {
                Jointables.Add("from", new JoinItem()
                {
                    Table = outer,
                    JoinString = ""
                });
            }
            if (!string.IsNullOrEmpty(inner.Table))
            {
                var key = $"{((LinqDLR2SQLGenerator)inner.SQLGenerator)._joindirect}_join_table_{inner.Table}_{inner.AliasName}";
                var on = "";
                var conditionvalue = new Dictionary<string, object>();

                on = BuildOnCondition(outerkey, innerkey, conditionvalue);
                if (!Jointables.ContainsKey(key))
                {
                    Jointables.Add(key, new JoinItem()
                    {
                        Table = inner,
                        JoinCondition = on,
                        ConditionValues = conditionvalue,
                        JoinString = $"{((LinqDLR2SQLGenerator)inner.SQLGenerator)._joindirect.ToUpper()} JOIN {inner.Table} {(string.IsNullOrEmpty(inner.AliasName) ? "" : $" {inner.AliasName}")} {(on == "" ? "" : $"ON {on}")}"
                    });
                }
            }

            BuildSQL(source);
        }
        private string BuildOnCondition(object outer, object inner, Dictionary<string, object> conditions)
        {
            var onequaltemplate = "#express# AND ";
            var on = "";
            //on的左右对象的中field的个数应该是相等的，如果不相等则以outer的属性个数为主，inner则自动依照遍历的下标位置与左侧匹配，右侧没有的则视为null值
            if (outer != null)
            {
                if (outer.GetType().Name.StartsWith("<>f__AnonymousType"))
                {
                    FrameDLRObject outerdobj = FrameDLRObject.CreateInstance(outer, Frame.Net.Base.Constants.FrameDLRFlags.SensitiveCase);
                    FrameDLRObject innerdobj = null;
                    if (inner.GetType().Name.StartsWith("<>f__AnonymousType"))
                    {
                        innerdobj = FrameDLRObject.CreateInstance(inner, Frame.Net.Base.Constants.FrameDLRFlags.SensitiveCase);
                    }

                    var index = 0;
                    foreach (var item in outerdobj.Items)
                    {
                        if (innerdobj == null)
                            on += onequaltemplate.Replace("#express#", BuildSingleOnCondition(item.Value, inner, conditions));
                        else
                        {
                            var innerkey = innerdobj.Keys.Count > index ? innerdobj.Keys[index] : innerdobj.Keys[innerdobj.Keys.Count];

                            on += onequaltemplate.Replace("#express#", BuildSingleOnCondition(item.Value, innerdobj.GetValue(innerkey), conditions));
                        }
                        index++;
                    }
                    on = on.Trim();
                    on = on != "" ? on.Substring(0, on.Length - 3) : "";
                }
                else
                {
                    on += BuildSingleOnCondition(outer, inner, conditions);
                }
            }

            return on;
        }
        private string BuildSingleOnCondition(object outer, object inner, Dictionary<string, object> conditions)
        {
            var on = "";
            if (outer != null)
            {
                dynamic leftcolumn = NewColumnInstance(outer);
                using (LinqDLR2SqlWhereOperator where = ((dynamic)leftcolumn) == inner)
                {
                    on += where.Result;
                    foreach (var item in where.ConditionValues)
                    {
                        conditions.Add(item.Key, item.Value);
                    }
                }
            }

            return on;
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
            foreach(var item in pretable.SQLGenerator.Selectmanytables)
            {
                if (!Selectmanytables.Contains(item))
                {
                    Selectmanytables.Add(item);
                }
            }
            if (pretable.Table != "" && !Selectmanytables.Contains(pretable))
            {
                Selectmanytables.Add(pretable);
            }
            CurrentOperation = CurrentOperation | LinqDLR2SQLOperation.SelectMany;
        }
        /// <summary>
        /// 执行排序操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        public virtual void DoOrderBy<TSource>(LinqDLR2Sql<TSource> source, object key)
        {

            if (key is LinqDLRColumn)
            {
                var ldc = (LinqDLRColumn)key;

                Orderby += Orderby == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress}";
            }
            else if (ComFunc.IsImplementedRawGeneric(source.GetType(), typeof(LamdaSQLObject<>)))
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
                        Orderby += Orderby == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress}";
                    }
                }
            }

            CurrentOperation = source.SQLGenerator.CurrentOperation | LinqDLR2SQLOperation.OrderBy;
            //linq语法的特性，当执行单表操作的时候，直接orderby xxx select t这种情况下，是不会走select方法的，只会走orderby，
            //因此当出现只会走orderby操作时，说明lamda表达式只走了where而没有走select
            //因此在where处预先初始化一次sql相关参数的

            BuildSQL(source);
        }
        /// <summary>
        /// 执行逆向排序
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        public virtual void DoOrderByDescending<TSource>(LinqDLR2Sql<TSource> source, object key)
        {
            if (key is LinqDLRColumn)
            {
                var ldc = (LinqDLRColumn)key;

                Orderby += Orderby == "" ? $"{ldc.ColumnExpress} DESC" : $",{ldc.ColumnExpress} DESC";
            }
            else if (ComFunc.IsImplementedRawGeneric(source.GetType(), typeof(LamdaSQLObject<>)))
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
                        Orderby += Orderby == "" ? $"{ldc.ColumnExpress} DESC" : $",{ldc.ColumnExpress} DESC";
                    }
                }
            }

            CurrentOperation = source.SQLGenerator.CurrentOperation | LinqDLR2SQLOperation.OrderBy;
            //linq语法的特性，当执行单表操作的时候，直接orderby xxx select t这种情况下，是不会走select方法的，只会走orderby，
            //因此当出现只会走orderby操作时，说明lamda表达式只走了where而没有走select
            //因此在where处预先初始化一次sql相关参数的

            BuildSQL(source);
        }
        /// <summary>
        /// 执行where操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="where"></param>
        public virtual void DoWhere<TSource>(LinqDLR2Sql<TSource> source, LinqDLR2SqlWhereOperator where)
        {
            CurrentOperation = source.SQLGenerator.CurrentOperation | LinqDLR2SQLOperation.Where;
            Lastwhere = where;

            //linq语法的特性，当执行单表操作的时候，直接where xxx select t这种情况下，是不会走select方法的，只会走where，
            //因此当出现where操作时，说明lamda表达式只走了where而没有走select
            //因此在where处预先初始化一次sql相关参数的
            Orderby = source.SQLGenerator.Orderby;

            Lastwhere = source.SQLGenerator.Lastwhere;

            if (Lastwhere != null)
            {
                foreach (var item in Lastwhere.ConditionValues)
                {
                    if (!ConditionValues.ContainsKey(item.Key))
                        ConditionValues.Add(item.Key, item.Value);
                }
            }

            BuildSQL(source);
        }
        /// <summary>
        /// 执行take操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="count"></param>
        public virtual void DoTake<TSource>(LinqDLR2Sql<TSource> source, int count)
        {
            var flagname = source.SQLGenerator.SqlFlags.GetType().Name.ToLower();
            if (flagname.StartsWith("mysql"))
            {
                Pearfix = "LIMIT " + count;
            }
            else if (flagname.StartsWith("sqlite"))
            {
                Pearfix = "LIMIT " + count;
            }
            else if (flagname.StartsWith("sqlserver"))
            {
                Prefix = "TOP " + count;
            }
            else
            {
                Prefix = "TOP " + count;
            }
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
        /// <summary>
        /// 执行count操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        public void DoCount<TSource>(LinqDLR2Sql<TSource> source)
        {
            Prefix = "COUNT(1)";
        }
        /// <summary>
        /// 执行max操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="column"></param>
        public void DoMax<TSource, TResult>(LinqDLR2Sql<TSource> source, TResult column)
        {
            if (column is LinqDLRColumn)
            {
                var c = (LinqDLRColumn)(object)column;
                Prefix = $"MAX({c.ColumnExpress})";
            }
        }
        /// <summary>
        /// 执行Max操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="column"></param>
        public void DoMax<TSource>(LinqDLR2Sql<TSource> source, string column)
        {
            Prefix = $"MAX({column})";
        }
        /// <summary>
        /// 执行Min操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="column"></param>
        public void DoMin<TSource, TResult>(LinqDLR2Sql<TSource> source, TResult column)
        {
            if (column is LinqDLRColumn)
            {
                var c = (LinqDLRColumn)(object)column;
                Prefix = $"MIN({c.ColumnExpress})";
            }
        }
        /// <summary>
        /// 执行Min操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="column"></param>
        public void DoMin<TSource>(LinqDLR2Sql<TSource> source, string column)
        {
            Prefix = $"MIN({column})";
        }
        /// <summary>
        /// 执行Sum操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="column"></param>
        public void DoSum<TSource, TResult>(LinqDLR2Sql<TSource> source, TResult column)
        {
            if (column is LinqDLRColumn)
            {
                var c = (LinqDLRColumn)(object)column;
                Prefix = $"SUM({c.ColumnExpress})";
            }
        }
        /// <summary>
        /// 执行Sum操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="column"></param>
        public void DoSum<TSource>(LinqDLR2Sql<TSource> source, string column)
        {
            Prefix = $"SUM({column})";
        }
        /// <summary>
        /// 执行Avg操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="column"></param>
        public void DoAvg<TSource, TResult>(LinqDLR2Sql<TSource> source, TResult column)
        {
            if (column is LinqDLRColumn)
            {
                var c = (LinqDLRColumn)(object)column;
                Prefix = $"AVG({c.ColumnExpress})";
            }
        }
        /// <summary>
        /// 执行Avg操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="column"></param>
        public void DoAvg<TSource>(LinqDLR2Sql<TSource> source, string column)
        {
            Prefix = $"AVG({column})";
        }
        /// <summary>
        /// 执行distinct操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        public void DoDistinct<TSource>(LinqDLR2Sql<TSource> source)
        {
            Prefix = $"DISTINCT";
        }
        /// <summary>
        /// Group by操作
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="pretable"></param>
        /// <param name="groupitems"></param>
        public void DoGroupBy<TElement, TSource>(LinqDLR2Sql<TElement> source, LinqDLR2Sql<TSource> pretable, object groupitems)
        {
            //从pretable中将generator的信息copy过来
            //CopyFrom(pretable.SQLGenerator);

            CurrentOperation = CurrentOperation | LinqDLR2SQLOperation.GroupBy;
            if (groupitems is LinqDLRColumn)
            {
                var ldc = (LinqDLRColumn)groupitems;

                GroupBy += GroupBy == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress}";
            }
            else if (groupitems.GetType().Name.StartsWith("<>f__AnonymousType"))
            {
                var fields = groupitems.GetType().GetTypeInfo().DeclaredFields;
                foreach (var f in fields)
                {
                    var v = f.GetValue(groupitems);
                    if (v is LinqDLRColumn)
                    {
                        var ldc = (LinqDLRColumn)v;
                        GroupBy += GroupBy == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress}";
                    }
                }
            }
        }
        /// <summary>
        /// 执行group by操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="groupitems"></param>
        public void DoGroupBy<TSource>(LinqDLR2Sql<TSource> source, object groupitems)
        {
            CurrentOperation = CurrentOperation | LinqDLR2SQLOperation.GroupBy;
            if (groupitems is LinqDLRColumn)
            {
                var ldc = (LinqDLRColumn)groupitems;

                GroupBy += GroupBy == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress}";
            }
            else if (groupitems.GetType().Name.StartsWith("<>f__AnonymousType"))
            {
                var fields = groupitems.GetType().GetTypeInfo().DeclaredFields;
                foreach (var f in fields)
                {
                    var v = f.GetValue(groupitems);
                    if (v is LinqDLRColumn)
                    {
                        var ldc = (LinqDLRColumn)v;
                        GroupBy += GroupBy == "" ? ldc.ColumnExpress : $",{ldc.ColumnExpress}";
                    }
                }
            }
        }
        #endregion
        #region 扩展方法
        /// <summary>
        /// 转为sql语句
        /// </summary>
        /// <returns></returns>
        public virtual string ToSql()
        {
            var sql = "";
            //linq语法的特性，当执行单表操作的时候，直接where/orderby xxx select t这种情况下，是不会走select方法的，只会走where/orderby，
            //因此当出现where/orderby操作时，说明lamda表达式只走了where/orderby而没有走select
            if ((CurrentOperation & LinqDLR2SQLOperation.Select) != LinqDLR2SQLOperation.Select
                && ((CurrentOperation & LinqDLR2SQLOperation.Where) == LinqDLR2SQLOperation.Where
                || (CurrentOperation & LinqDLR2SQLOperation.OrderBy) == LinqDLR2SQLOperation.OrderBy))
            {
                //操作修正为select操作
                CurrentOperation = CurrentOperation | LinqDLR2SQLOperation.Select;
            }
            //目前只支持select操作
            if ((CurrentOperation & LinqDLR2SQLOperation.Select) == LinqDLR2SQLOperation.Select
                || (CurrentOperation & LinqDLR2SQLOperation.SelectJoin) == LinqDLR2SQLOperation.SelectJoin
                || (CurrentOperation & LinqDLR2SQLOperation.SelectMany) == LinqDLR2SQLOperation.SelectMany)
            {
                var prefix = Prefix;
                var pearfix = Pearfix;
                sql = _sql;
                var columns = Columns;
                var tables = Tables;
                var where = (Lastwhere == null || string.IsNullOrEmpty(Lastwhere.Result)) ? "" : $"WHERE {Lastwhere.Result}";
                var groupby = GroupBy == "" ? "" : $"GROUP BY {GroupBy}";
                var orderby = Orderby == "" ? "" : $"ORDER BY {Orderby}";
                if (prefix.StartsWith("COUNT")
                    || prefix.StartsWith("MAX")
                    || prefix.StartsWith("MIN")
                    || prefix.StartsWith("SUM"))
                {
                    sql = sql.Replace("{prefix}", "")
                   .Replace("{columns}", prefix)
                   .Replace("{tables}", tables)
                   .Replace("{where}", where)
                   .Replace("{groupby}", groupby)
                   .Replace("{orderby}", orderby)
                   .Replace("{pearfix}", pearfix);
                }
                else
                {
                    sql = sql.Replace("{prefix}", prefix)
                    .Replace("{columns}", columns)
                    .Replace("{tables}", tables)
                    .Replace("{where}", where)
                    .Replace("{groupby}", groupby)
                    .Replace("{orderby}", orderby)
                    .Replace("{pearfix}", pearfix);
                }

                //清除prefix和pearfix使该表达式可以继续重复使用
                Prefix = "";
                Pearfix = "";

            }
            return sql;
        }
        #endregion
        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose()
        {
            if (Selectmanytables != null)
                Selectmanytables.Clear();
            Selectmanytables = null;

            if (Jointables != null)
            {
                foreach (var item in Jointables)
                {
                    item.Value.Dispose();
                }
                Jointables.Clear();
            }
            Jointables = null;

            if (ConditionValues != null)
                ConditionValues.Clear();
            ConditionValues = null;

            if (Lastwhere != null)
                Lastwhere.Dispose();
            Lastwhere = null;

            _sql = null;
            Prefix = null;
            Pearfix = null;
            Columns = null;
            Tables = null;
            Where = null;
            Orderby = null;
        }
        /// <summary>
        /// copy
        /// </summary>
        /// <param name="from"></param>
        public void CopyFrom(LinqDLR2SQLGenerator from)
        {
            Columns = from.Columns;
            if (from.ConditionValues != null)
            {
                foreach (var item in from.ConditionValues)
                {
                    if (!ConditionValues.ContainsKey(item.Key))
                    {
                        if (!ConditionValues.ContainsKey(item.Key))
                            ConditionValues.Add(item.Key, item.Value);
                    }
                }
            }
            CurrentOperation = from.CurrentOperation;
            GroupBy = from.GroupBy;
            if (from.Jointables != null)
            {
                foreach (var item in from.Jointables)
                {
                    if (!Jointables.ContainsKey(item.Key))
                    {
                        Jointables.Add(item.Key, item.Value);
                    }
                }
            }
            Lastwhere = from.Lastwhere;
            if (Lastwhere != null)
            {
                foreach (var item in Lastwhere.ConditionValues)
                {
                    if (!ConditionValues.ContainsKey(item.Key))
                    {
                        if (!ConditionValues.ContainsKey(item.Key))
                            ConditionValues.Add(item.Key, item.Value);
                    }
                }
            }
            Orderby = from.Orderby;
            Pearfix = from.Pearfix;
            Prefix = from.Prefix;
            if (from.Selectmanytables != null)
            {
                foreach (var item in from.Selectmanytables)
                {
                    if (!Selectmanytables.Contains(item))
                    {
                        Selectmanytables.Add(item);
                    }
                }
            }
            Tables = from.Tables;
            Where = from.Where;

            _joindirect = from._joindirect;
            _sql = from._sql;
        }
    }
    /// <summary>
    /// Join的对象
    /// </summary>
    public class JoinItem : IDisposable, ICloneable
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

        public object Clone()
        {
            var rtn = new JoinItem();
            rtn.Table = Table;
            rtn.JoinString = JoinString;
            rtn.JoinCondition = JoinCondition;
            foreach (var item in ConditionValues)
            {
                if (item.Value is ICloneable)
                {
                    rtn.ConditionValues.Add(item.Key, ((ICloneable)item.Value).Clone());
                }
                else
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }

            return rtn;
        }

        public void Dispose()
        {
            if (Table is IDisposable)
            {
                ((IDisposable)Table).Dispose();
            }
            else
            {
                Table = null;
            }
            JoinString = null;
            JoinCondition = null;
            ConditionValues.Clear();
            ConditionValues = null;
        }


    }
}
