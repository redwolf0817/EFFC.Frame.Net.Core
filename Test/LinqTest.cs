using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Module.HttpCall;
using EFFC.Frame.Net.Resource.Postgresql;
using EFFC.Frame.Net.Resource.Sqlite;
using EFFC.Frame.Net.Resource.SQLServer;
using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    public class LinqTest
    {
        public static void Test()
        {
            new APIAccess().Get("xxxvasdfa");
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
            var str = new StringBuilder();
            for (var i = 0; i < 1; i++)
            {

                //str.AppendLine(Guid.NewGuid().GetHashCode().ToString());
                new TestLoop().Run();
            }
            //File.WriteAllText("e:/random.txt", str.ToString());

            //var l1 = new List<object>();
            //var l2 = new List<dynamic>();
            //var l3 = new List<dynamic>();
            //var s = (from t in l1
            //         select t).Distinct();
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
    public class APIAccess : SimpleRestCallHelper
    {
        public string Get(string url)
        {
            var result = base.Get(url, new
            {
                Authorization = $"Bearer xxxx",
                Sub_System_id = ""
            });
            if (result.StartsWith("Failed:")) result = "";
            return result;
        }
    }
    public class TestLoop
    {
        public void Run()
        {
            var dt = DateTime.Now;
            //var s = from t in new List<object>()
            //        where t.ToString() == ""
            //        select t;
            var ssa = new SQLServerAccess();
            
            var lastweek_start = ComFunc.GetMondayDate(DateTime.Now).AddDays(-7);
            var lastweek_end = ComFunc.GetSundayDate(DateTime.Now).AddDays(-7);
            var dd = from t in ssa.NewLinqTable("IC_Statistic_Water", "a")
                     where t.SW_Estate == "1"
                     && t.todatetime(t.SW_Year.tostring(4).concat("-", (t.SW_Month + 100).tostring(3).substring(2, 2), "-", (t.SW_Day + 100).tostring(3).substring(2, 2))) >= t.todatetime($"{lastweek_start.Year}-{lastweek_start.Month}-{lastweek_start.Day}")
                     && t.todatetime(t.SW_Year.tostring(4).concat("-", (t.SW_Month + 100).tostring(3).substring(2, 2), "-", (t.SW_Day + 100).tostring(3).substring(2, 2))) <= t.todatetime($"{lastweek_end.Year}-{lastweek_end.Month}-{lastweek_end.Day}")
                     select new
                     {
                         hour = t.SW_Hour,
                         count = t.SW_Total
                     };
            //var filter = "a";
            //var start_time = "";
            //var end_time = "";
            //var s = from t in LinqDLRTable.New<LinqDLRColumn>("a", "", new SqlOperatorFlags())
            //        where t.notnull(filter, (t.Request_Route.contains(filter) || t.Request_IP.contains(filter) || t.Request_SubSystem_Name.contains(filter)))
            //        && t.notnull(start_time, t.add_time >= start_time) && t.notnull(end_time, t.add_time <= end_time) && t.start_time.isnull("").within(",1,2,3")
            //        orderby t.add_time descending
            //        select t;
            Console.WriteLine(dd.ToSql());
            var s = from t in LinqDLRTable.New<LinqDLRColumn>("a", "a", new SqlOperatorFlags())
                    join t2 in LinqDLRTable.New<LinqDLRColumn>("b", "b", new SqlOperatorFlags()) on t.id equals t2.code
                    join t3 in LinqDLRTable.New<LinqDLRColumn>("b", "c", new SqlOperatorFlags()) on t.id2 equals t3.code
                    group t by new { t.a, t.b } into g
                    select g;
            var sql = s.ToSql();
            Console.WriteLine(sql);
            sql = (from t in LinqDLRTable.New<LinqDLRColumn>("a", "", new SqlOperatorFlags())
                   orderby t.time descending
                   select t).ToSql();
            Console.WriteLine(sql);
            sql = (from t in LinqDLRTable.New<LinqDLRColumn>("a", "", new SqlOperatorFlags())
                   orderby t.time descending ,t.id
                   select new { t.id, t.name }).ToSql();
            Console.WriteLine(sql);
            sql = (from t in LinqDLRTable.New<LinqDLRColumn>("a", "", new SqlOperatorFlags())
                   group t by new { t.a, t.b } into g
                   select g).ToSql();
            Console.WriteLine(sql);
            sql = (from t in LinqDLRTable.New<LinqDLRColumn>("a", "", new SqlOperatorFlags())
                   from t2 in LinqDLRTable.New<LinqDLRColumn>("b", "", new SqlOperatorFlags())
                   where t.id == t2.id && t.time == "2018/08/17"
                   select new
                   {
                       t.id,
                       t2.name
                   }).ToSql();
            Console.WriteLine(sql);
            sql = (from t in LinqDLRTable.New<LinqDLRColumn>("a", "", new SqlServerOperatorFlags())
                   from t2 in LinqDLRTable.New<LinqDLRColumn>("b", "", new SqlServerOperatorFlags())
                   from t3 in LinqDLRTable.New<LinqDLRColumn>("c", "", new SqlServerOperatorFlags())
                   where t.id == t2.id
                   group new { t,t2,t3} by new
                   {
                       t.id,
                       t2.time
                   } into g
                   select new
                   {
                       g.t.id,
                       g.t2.name
                   }).ToSql();
            Console.WriteLine(sql);
            Console.WriteLine($"selectmany cast:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;

            
            
        }
    }
    
}

    

