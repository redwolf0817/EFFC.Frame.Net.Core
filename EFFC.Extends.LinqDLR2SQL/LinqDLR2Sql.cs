using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL
{
    /// <summary>
    /// LinqDLR2SQL的操作枚举
    /// </summary>
    public enum LinqDLR2SQLOperation
    {
        /// <summary>
        /// 无操作
        /// </summary>
        None,
        /// <summary>
        /// 执行了select操作
        /// </summary>
        Select,
        /// <summary>
        /// 执行了select many操作（from多个表)
        /// </summary>
        SelectMany,
        /// <summary>
        /// 执行了join操作
        /// </summary>
        SelectJoin,
        /// <summary>
        /// 执行了delete操作
        /// </summary>
        Delete,
        /// <summary>
        /// 执行了update操作
        /// </summary>
        Update,
        /// <summary>
        /// 执行了insert操作
        /// </summary>
        Insert
    }
    
    /// <summary>
    /// LinqDLR2Sql的基类，定义了LinqDLR2Sql的基本结构和已经实现的sql转化操作
    /// </summary>
    /// <typeparam name="TSource">lamda表达式执行时的动态识别类型，要想使用lamda表达式，此属性必备</typeparam>
    public class LinqDLR2Sql<TSource>
    {
        LinqDLR2SQLGenerator _generator = new LinqDLR2SQLGenerator();

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
        /// sql生成器的扩展接口
        /// </summary>
        public LinqDLR2SQLGenerator SQLGenerator
        {
            get
            {
                return _generator;
            }
            set
            {
                _generator = value;
            }
        }
        /// <summary>
        /// 创建一个新的LinqDLR2SQL对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="table"></param>
        /// <param name="aliasName"></param>
        /// <returns></returns>
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
        public void DoSelectMany<TLastItem>(LinqDLR2Sql<TLastItem> pretable)
        {
            SQLGenerator.DoSelectMany(this, pretable);
        }
        /// <summary>
        /// 执行select的自定义处理
        /// </summary>
        /// <typeparam name="TLastItem"></typeparam>
        /// <param name="pretable"></param>
        public void DoSelect<TLastItem>(LinqDLR2Sql<TLastItem> pretable)
        {
            SQLGenerator.DoSelect(this, pretable);
        }
        /// <summary>
        /// 执行join操作
        /// </summary>
        /// <typeparam name="TOuterItem"></typeparam>
        /// <typeparam name="TInnerItem"></typeparam>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        public void DoJoin<TOuterItem,TInnerItem>(LinqDLR2Sql<TOuterItem> outer,LinqDLR2Sql<TInnerItem> inner,object outerkey,object innerkey)
        {
            SQLGenerator.DoJoin(this, outer, inner, outerkey, innerkey);
        }
        public void DoOrderBy(object key)
        {
            SQLGenerator.DoOrderBy(this, key);
        }
        public void DoOrderByDescending(object key)
        {
            SQLGenerator.DoOrderByDescending(this, key);
        }
        /// <summary>
        /// 执行where的自定义处理
        /// </summary>
        /// <param name="where"></param>
        public void DoWhere(LinqDLR2SqlWhereOperator where)
        {
            SQLGenerator.DoWhere(this,where);
        }
        /// <summary>
        /// 转为sql语句
        /// </summary>
        /// <returns></returns>
        public string ToSql()
        {
            return SQLGenerator.ToSql();
        }
        /// <summary>
        /// 本表join的时候使用left方式
        /// </summary>
        public LinqDLR2Sql<TSource> LeftJoin()
        {
            SQLGenerator.DoLeftJoin(this);
            return this;
        }
        /// <summary>
        /// 本表join的时候使用right方式
        /// </summary>
        public LinqDLR2Sql<TSource> RightJoin()
        {
            SQLGenerator.DoRightJoin(this);
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
    /// <summary>
    /// LinqDLR2Sql操作扩展实现lamda表达式操作
    /// </summary>
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
