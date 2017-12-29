using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Linq2SQL
{
    public class LinqDLR2Sql<TSource>
    {
        //串联的table集合，非join
        List<object> _selectmanytables = new List<object>();
        //待执行的多个sql
        List<string> _sqls = new List<string>();
        public TSource Item
        {
            get;
            protected set;
        }
        public string Table
        {
            get;
            private set;
        }
        public string AliasName
        {
            get;
            private set;
        }
        public object Me
        {
            get;
            protected set;
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
        public virtual void DoSelectMany<TLastItem>(LinqDLR2Sql<TLastItem> pretable)
        {
            _selectmanytables.AddRange(pretable._selectmanytables);
            if (pretable.Table != "")
                _selectmanytables.Add(pretable);

        }

        public virtual void DoSelect()
        {

        }

    }
    public class LinqDLRTable : LinqDLR2Sql<dynamic>
    {
        public static LinqDLRTable New(string table, string aliasName = "")
        {
            var tn = aliasName == "" ? table : aliasName;
            var rtn = New<LinqDLRTable>(new LamdaSQLObject(tn), table, aliasName);
            return rtn;
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
        public static LinqDLR2Sql<TResult> Select<TSource, TResult>(this LinqDLR2Sql<TSource> source, Func<TSource, TResult> selector)
        {
            var v = selector.Invoke(source.Item);
            var rtn = LinqDLR2Sql<TResult>.New<LinqDLR2Sql<TResult>>(v, source.Table, source.AliasName);

            return rtn;
        }

        public static LinqDLR2Sql<TResult> Join<TOuter, TInner, TKey, TResult>(this LinqDLR2Sql<TOuter> outer, LinqDLR2Sql<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            var v1 = outerKeySelector.Invoke(outer.Item);
            var v2 = innerKeySelector.Invoke(inner.Item);
            var re = resultSelector.Invoke(outer.Item, inner.Item);
            var rtn = LinqDLR2Sql<TResult>.New<LinqDLR2Sql<TResult>>(re);
            return rtn;
        }
        public static LinqDLR2Sql<TSource> Where<TSource>(this LinqDLR2Sql<TSource> source, Func<TSource, LinqDLR2SqlWhereOperator> predicate)
        {
            var op = predicate.Invoke(source.Item);
            return source;
        }
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source;
        }


    }
}
