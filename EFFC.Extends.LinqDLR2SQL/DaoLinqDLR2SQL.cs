using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL
{
    public class DaoLinqDLR2Sql<TSource> : LinqDLR2Sql<TSource>
    {
        public DaoLinqDLR2Sql(TSource item, string table, string aliasName = "")
        {
            Item = item;
            Table = table;
            AliasName = aliasName;
        }
    }

    /// <summary>
    /// LinqDLR2Sql操作扩展实现lamda表达式操作
    /// </summary>
    public static class DaoLinqDLR2SqlExtend
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
            var rtn = LinqDLR2Sql<TResult>.New<LinqDLR2Sql<TResult>>(resultSelector.Invoke(source.Item, fromtable.Item), "", "", fromtable.LinkedDBType);
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
            var rtn = LinqDLR2Sql<TResult>.New<LinqDLR2Sql<TResult>>(v, source.Table, source.AliasName, source.LinkedDBType);
            rtn.DoSelect<TSource>(source);
            return rtn;
        }

        public static LinqDLR2Sql<TResult> Join<TOuter, TInner, TKey, TResult>(this LinqDLR2Sql<TOuter> outer, LinqDLR2Sql<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            var v1 = outerKeySelector.Invoke(outer.Item);
            var v2 = innerKeySelector.Invoke(inner.Item);
            var re = resultSelector.Invoke(outer.Item, inner.Item);
            var rtn = LinqDLR2Sql<TResult>.New<LinqDLR2Sql<TResult>>(re, "", "", outer.LinkedDBType);
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
            var ss = (from t in new List<object>()
                      group t by t.GetType() into g
                      select g
                     );
            var key = keySelector.Invoke(source.Item);
            source.DoOrderByDescending(key);
            return source;
        }
        /// <summary>
        /// 执行Take操作，相当于sql的top
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> Take<TSource>(this LinqDLR2Sql<TSource> source, int count)
        {
            source.SQLGenerator.DoTake(source, count);
            return source;
        }
        /// <summary>
        /// 执行count操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> Count<TSource>(this LinqDLR2Sql<TSource> source)
        {
            source.SQLGenerator.DoCount(source);
            return source;
        }
        /// <summary>
        /// 执行max操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> Max<TSource, TResult>(this LinqDLR2Sql<TSource> source, Func<TSource, TResult> selector)
        {
            var re = selector(source.Item);
            source.SQLGenerator.DoMax(source, re);
            return source;
        }
        /// <summary>
        /// 执行max操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> Max<TSource>(this LinqDLR2Sql<TSource> source, string column)
        {
            source.SQLGenerator.DoMax(source, column);
            return source;
        }
        /// <summary>
        /// 执行Min操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> Min<TSource, TResult>(this LinqDLR2Sql<TSource> source, Func<TSource, TResult> selector)
        {
            var re = selector(source.Item);
            source.SQLGenerator.DoMin(source, re);
            return source;
        }
        /// <summary>
        /// 执行Min操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> Min<TSource>(this LinqDLR2Sql<TSource> source, string column)
        {
            source.SQLGenerator.DoMin(source, column);
            return source;
        }
        /// <summary>
        /// 执行Sum操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> Sum<TSource, TResult>(this LinqDLR2Sql<TSource> source, Func<TSource, TResult> selector)
        {
            var re = selector(source.Item);
            source.SQLGenerator.DoSum(source, re);
            return source;
        }
        /// <summary>
        /// 执行Sum操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> Sum<TSource>(this LinqDLR2Sql<TSource> source, string column)
        {
            source.SQLGenerator.DoSum(source, column);
            return source;
        }
        /// <summary>
        /// 执行Distinct操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> Distinct<TSource>(this LinqDLR2Sql<TSource> source)
        {
            source.SQLGenerator.DoDistinct(source);
            return source;
        }
    }
}
