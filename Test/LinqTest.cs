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
            for (var i = 0; i < 100; i++)
            {
                var dd = from t in LinqDLRTable.New("a")
                         from t2 in LinqDLRTable.New("b")
                         from t3 in LinqDLRTable.New("c")
                         from t4 in LinqDLRTable.New("d")
                         where t.uid == t2.puid && (t2.uid == t3.cuid || t.data == DateTime.Now.Ticks)
                         select new { name = t.name, cash = t4.cash };

                Console.WriteLine($"selectmany cast:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;
                var dd2 = from t in LinqDLRTable.New("a")
                          join t2 in LinqDLRTable.New("b").LeftJoin() on t.id equals t2.pid
                          join t3 in LinqDLRTable.New("c") on t2.id equals t3.pid
                          where t.id == "1" & t.uid == t2.puid & (t2.uid == t3.cuid | t.age == new Random().Next())
                          orderby t.date,t2.id descending,t2.orderbyno
                          select t;
                var dd3 = from t in dd2
                join t1 in LinqDLRTable.New("d").RightJoin() on t.id equals t1.uid
                select new { t, t1.product };
                Console.WriteLine($"selectjoin cast:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;
            }
            
            //var l1 = new List<dynamic>();
            //var l2 = new List<dynamic>();
            //var l3 = new List<dynamic>();
            //l1.Add(new LamdaSQLObject<LinqDLRColumn>("a"));
            //l1.Add(new LamdaSQLObject<LinqDLRColumn>("b"));
            //l1.Add(new LamdaSQLObject<LinqDLRColumn>("c"));
            //l1.Add(new LamdaSQLObject<LinqDLRColumn>("d"));

            //l2.Add(new LamdaSQLObject<LinqDLRColumn>("a"));
            //l2.Add(new LamdaSQLObject<LinqDLRColumn>("b"));
            //l2.Add(new LamdaSQLObject<LinqDLRColumn>("c"));
            //l2.Add(new LamdaSQLObject<LinqDLRColumn>("d"));

            //l3.Add(new LamdaSQLObject<LinqDLRColumn>("a"));
            //l3.Add(new LamdaSQLObject<LinqDLRColumn>("b"));
            //l3.Add(new LamdaSQLObject<LinqDLRColumn>("c"));
            //l3.Add(new LamdaSQLObject<LinqDLRColumn>("d"));
            //var dd3 = from t in l1
            //          join t2 in l2 on t.id equals t2.pid
            //          join t3 in l3 on t2.id equals t3.pid
            //          where t.id == "1" & t.uid == t2.puid & (t2.uid == t3.cuid | t.age == 3)
            //          select t;

            //Console.WriteLine($"join2 cast:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;
        }
    }

    public static class LinqDLR2SqlExtend
    {
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var dt = DateTime.Now;
            var rtn = new List<TSource>();

            foreach(var item in source)
            {
                var re = predicate.Invoke(item);
                if (re)
                    rtn.Add(item);
            }
            Console.WriteLine($"IEnumerable Where cast:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;
            return rtn;
        }
    }
}

    

