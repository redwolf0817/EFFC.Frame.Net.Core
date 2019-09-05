using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
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
        None=0,
        /// <summary>
        /// 执行了select操作
        /// </summary>
        Select=1,
        /// <summary>
        /// 执行了select many操作（from多个表)
        /// </summary>
        SelectMany=2,
        /// <summary>
        /// 执行了join操作
        /// </summary>
        SelectJoin=4,
        /// <summary>
        /// 执行了delete操作
        /// </summary>
        Delete=8,
        /// <summary>
        /// 执行了update操作
        /// </summary>
        Update=16,
        /// <summary>
        /// 执行了insert操作
        /// </summary>
        Insert=32,
        /// <summary>
        /// 执行了Where操作
        /// </summary>
        Where=64,
        /// <summary>
        /// 执行了OrderBy操作
        /// </summary>
        OrderBy = 128,
        /// <summary>
        /// 执行了GroupBy操作
        /// </summary>
        GroupBy = 256
    }

    
    /// <summary>
    /// LinqDLR2Sql的基类，定义了LinqDLR2Sql的基本结构和已经实现的sql转化操作
    /// </summary>
    /// <typeparam name="TSource">lamda表达式执行时的动态识别类型，要想使用lamda表达式，此属性必备</typeparam>
    public class LinqDLR2Sql<TSource>: IDisposable
    {
        LinqDLR2SQLGenerator _generator = null;

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
        /// sql生成器的扩展接口
        /// </summary>
        public LinqDLR2SQLGenerator SQLGenerator
        {
            get;set;
        }
        /// <summary>
        /// 当前的操作类型
        /// </summary>
        public LinqDLR2SQLOperation CurrentOperationType
        {
            get
            {
                return SQLGenerator.CurrentOperation;
            }
        }
        /// <summary>
        /// 创建一个新的LinqDLR2SQL对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="table"></param>
        /// <param name="aliasName"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        public static T New<T>(TSource item, string table = "", string aliasName = "", LinqDLR2SQLGenerator generator = null) where T : LinqDLR2Sql<TSource>
        {
            var rtn = (T)Activator.CreateInstance(typeof(T), true);//new LinqTable<TSource>();
            rtn.Item = item;
            rtn.Table = table;
            rtn.AliasName = string.IsNullOrEmpty(aliasName) ? rtn.Table : aliasName;
            rtn.SQLGenerator = generator == null ? new GeneralLinqDLR2SQLGenerator(null) : generator;
            return rtn;
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

        public virtual void Dispose()
        {
            if(_generator != null && _generator is IDisposable)
            {
                ((IDisposable)_generator).Dispose();
            }

            _generator = null;
        }
        public override string ToString()
        {
            return ToSql();
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
            var rtn = LinqDLR2Sql<TResult>.New<LinqDLR2Sql<TResult>>(resultSelector.Invoke(source.Item, fromtable.Item),"","",source.SQLGenerator);
            using (source)
            {
                rtn.SQLGenerator.DoSelectMany(rtn, source);
                //source的SQLGenerator被rtn共用
                source.SQLGenerator = null;
            }
            using (fromtable)
            {
                rtn.SQLGenerator.DoSelectMany(rtn, fromtable);
            }

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
            var rtn = LinqDLR2Sql<TResult>.New<LinqDLR2Sql<TResult>>(v, source.Table, source.AliasName, source.SQLGenerator);
            using (source)
            {
                rtn.SQLGenerator.DoSelect(rtn);
                //SQLGenerator共用一个
                source.SQLGenerator = null;
            }
            return rtn;
        }

        public static LinqDLR2Sql<TResult> Join<TOuter, TInner, TKey, TResult>(this LinqDLR2Sql<TOuter> outer, LinqDLR2Sql<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            var v1 = outerKeySelector.Invoke(outer.Item);
            var v2 = innerKeySelector.Invoke(inner.Item);
            var re = resultSelector.Invoke(outer.Item, inner.Item);
            var rtn = LinqDLR2Sql<TResult>.New<LinqDLR2Sql<TResult>>(re, "", "", outer.SQLGenerator);
            using (outer)
            {
                using (inner)
                {
                    rtn.SQLGenerator.DoJoin(rtn, outer, inner, v1, v2);
                    //SQLGenerator共用一个
                    outer.SQLGenerator = null;
                }
            }
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
            source.SQLGenerator.DoWhere(source, op);
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
            source.SQLGenerator.DoOrderBy(source,key);
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
            source.SQLGenerator.DoOrderBy(source, key);
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
            source.SQLGenerator.DoOrderByDescending(source,key);
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
            source.SQLGenerator.DoOrderByDescending(source,key);
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
            source.SQLGenerator.DoTake(source,count);
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
            source.SQLGenerator.DoMax(source,re);
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
            source.SQLGenerator.DoMax(source,column);
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
            source.SQLGenerator.DoMin(source,re);
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
            source.SQLGenerator.DoMin(source,column);
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
            source.SQLGenerator.DoSum(source,re);
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
            source.SQLGenerator.DoSum(source,column);
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
        public static LinqDLR2Sql<TSource> Avg<TSource, TResult>(this LinqDLR2Sql<TSource> source, Func<TSource, TResult> selector)
        {
            var re = selector(source.Item);
            source.SQLGenerator.DoAvg(source, re);
            return source;
        }
        /// <summary>
        /// 执行Sum操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> Avg<TSource>(this LinqDLR2Sql<TSource> source, string column)
        {
            source.SQLGenerator.DoAvg(source, column);
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

        /// <summary>
        /// 执行GroupBy操作，单表操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TSource> GroupBy<TSource, TKey>(this LinqDLR2Sql<TSource> source, Func<TSource, TKey> keySelector)
        {
            //var l = new List<string>();
            //var s = from t in l
            //        group t by t into ss
            //        select new
            //        {
            //            a = ss.First()
            //        };
            var tmp = keySelector(source.Item);
            source.SQLGenerator.DoGroupBy(source, tmp);
            return source;
        }
        /// <summary>
        /// 执行GroupBy操作,join表操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <returns></returns>
        public static LinqDLR2Sql<TElement> GroupBy<TSource, TKey, TElement>(this LinqDLR2Sql<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            /*对应的group by语句为
             * group <TElement> by  <TKey> into g
             * 其中linq会根据return类型来推断g的结构
            */
            var tmp = keySelector(source.Item);
            //element
            var tmp2 = elementSelector(source.Item);
            using (source)
            {
                var rtn = LinqDLR2Sql<TElement>.New<LinqDLR2Sql<TElement>>(tmp2, source.Table, source.AliasName, source.SQLGenerator);

                rtn.SQLGenerator.DoGroupBy(rtn, source, tmp);
                //与source共用一个SQLGenerator
                source.SQLGenerator = null;
                return rtn;
            }
        }
    }
}
