using System;
using System.Collections.Generic;
using System.Text;

using System.Dynamic;
using System.Linq.Expressions;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Common;
using System.Reflection;
using Test.Linq2SQL;
using System.Linq;
using System.Collections;
using static Test.LinqTest;

namespace Test
{
    public class LinqTest
    {
        public static void Test()
        {
            var dt = DateTime.Now;
            //var b = from t in new CustomTable("logininfo", "a")
            //        where (t.loginid == 2 || t.name.contains("ych") || (t.createtime == DateTime.Now.AddDays(-1) && t.level < 2)) && t.createtime == DateTime.Now
            //        orderby t.loginid, t.createtime descending, t.name
            //        select new { t.loginid, t.name };

            //var c = from t in new CustomTable("logininfo", "a")
            //        join t2 in new CustomTable("role", "b") on t.loginid equals t2.loginid
            //        //where t2.rolename=="admin"
            //        select new { t.name, t2.role };

            //var d = from t in new CustomTable("logininfo", "a")
            //        from t2 in t.role
            //        from t3 in new CustomTable("logininfo", "c")
            //            //where t.loginid = t2.userid
            //        select new { t.loginid, t.name,t2.rolename };

            //var dd = from t in GMyLinqD.New( MyLinqD.New("a"))
            //         from t2 in GMyLinqD.New(MyLinqD.New("b"))
            //         from t3 in GMyLinqD.New(MyLinqD.New("c"))
            //         from t4 in GMyLinqD.New(MyLinqD.New("d"))
            //         select t.Tables;

            var dd = from t in LinqDLRTable.New("a")
                     from t2 in LinqDLRTable.New("b")
                     from t3 in LinqDLRTable.New("c")
                     from t4 in LinqDLRTable.New("d")
                     where t.uid==t2.puid & t2.uid==t3.cuid | t.age == 3
                     select new {name=t.name,cash=t4.cash };
            Console.WriteLine($"selectmany cast:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;
            //var dd2 = from t in LinqDLRTable.New("a")
            //          join t2 in LinqDLRTable.New("b") on t.id equals t2.pid
            //          where t.id="1"
            //          select new { t.name, t2.money };
            Console.WriteLine($"join cast:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;



            //var list = new List<object>();
            //var list2 = new List<object>();
            //var list3 = new List<int>();
            //list.Add(0);

            //list2.Add(1);
            //list2.Add(2);

            //list3.Add(3);

            //var l = from t in list
            //        from t2 in list2
            //        from t3 in list3
            //        where t.GetType() == t2.GetType()
            //        select new { n1 = t.ToString(), n2 = t2.ToString() };



            //Console.WriteLine(b.SQL);
            //Console.WriteLine(c.SQL);

        }
        //public class GMyLinqD:List<MyLinqD>
        //{
        //    public MyLinqD Table
        //    {
        //        get;
        //        private set;
        //    }

        //    public static GMyLinqD New(MyLinqD table)
        //    {

        //        var rtn = new GMyLinqD();
        //        rtn.Table = table;
        //        return rtn;
        //    }
        //}
        public class LinqDynamic2SqlTable<TSource>
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
            public static T New<T>(TSource item, string table="", string aliasName="") where T: LinqDynamic2SqlTable<TSource>
            {
                var rtn = (T)Activator.CreateInstance(typeof(T), true);//new LinqTable<TSource>();
                rtn.Item = item;
                rtn.Table = table;
                rtn.AliasName = string.IsNullOrEmpty(aliasName) ? rtn.Table : aliasName;
                rtn.Me = rtn;
                return rtn;
            }
            public void DoSelectMany<TLastItem>(LinqDynamic2SqlTable<TLastItem> pretable)
            {
                _selectmanytables.AddRange(pretable._selectmanytables);
                if (pretable.Table != "")
                    _selectmanytables.Add(pretable);
                
            }
            
        }
        public class LinqDLRTable : LinqDynamic2SqlTable<dynamic>
        {
            public static LinqDLRTable New(string table, string aliasName = "")
            {
                var tn = aliasName == "" ? table : aliasName;
                var rtn = New<LinqDLRTable>(new LamdaSQLObject(tn), table, aliasName);
                return rtn;
            }

            
        }
        

        
    }

    public static class LinqTableExtend
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
        public static LinqDynamic2SqlTable<TResult> SelectMany<TSource, TCollector, TResult>(this LinqDynamic2SqlTable<TSource> source, Func<TSource, LinqDynamic2SqlTable<TCollector>> from, Func<TSource, TCollector, TResult> resultSelector)
        {
            //第一轮from
            var fromtable = from.Invoke(source.Item);
            var rtn = LinqDynamic2SqlTable<TResult>.New<LinqDynamic2SqlTable<TResult>>(resultSelector.Invoke(source.Item, fromtable.Item));
            rtn.DoSelectMany<TSource>(source);
            rtn.DoSelectMany<TCollector>(fromtable);

            return rtn;
        }
        public static LinqDynamic2SqlTable<TResult> Select<TSource, TResult>(this LinqDynamic2SqlTable<TSource> source, Func<TSource, TResult> selector)
        {
            var v = selector.Invoke(source.Item);
            var rtn = LinqDynamic2SqlTable<TResult>.New<LinqDynamic2SqlTable<TResult>>(v, source.Table, source.AliasName);
            return rtn;
        }

        public static LinqDynamic2SqlTable<TResult> Join<TOuter, TInner, TKey, TResult>(this LinqDynamic2SqlTable<TOuter> outer, LinqDynamic2SqlTable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            var v1 = outerKeySelector.Invoke(outer.Item);
            var v2 = innerKeySelector.Invoke(inner.Item);
            var re = resultSelector.Invoke(outer.Item, inner.Item);
            var rtn = LinqDynamic2SqlTable<TResult>.New<LinqDynamic2SqlTable<TResult>>(re);
            return rtn;
        }
        public static LinqDynamic2SqlTable<TSource> Where<TSource>(this LinqDynamic2SqlTable<TSource> source, Func<TSource, LinqDLR2SqlWhereOperator> predicate)
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

    

