using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Resource.Sqlite;
using EFFC.Frame.Net.Resource.SQLServer;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class SqliteTest
    {
        public static void Test()
        {
            var dt = DateTime.Now;
            using (SQLServerAccess sq = new SQLServerAccess())
            {
                sq.Open("Password=sa;Persist Security Info=True;User ID=sa;Initial Catalog=ChuYuWang_UC;Data Source=.;pooling=true;connection lifetime=0;min pool size = 1;max pool size=2000");
                Console.WriteLine($"sqlserver open cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
                var sql = "select * from CodeDictionary";
                DBOParameterCollection dpc = new DBOParameterCollection();
                var result = sq.Query(sql, dpc);
                Console.WriteLine($"sqlserver cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
                result = sq.Query(sql, dpc);
                Console.WriteLine($"sqlserver2 cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
            Console.WriteLine($"sqlserver cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            using (SqliteAccess sq = new SqliteAccess())
            {
                sq.Open("Data Source=./AppData/EFFC_CMS.db;");
                Console.WriteLine($"sqlite open cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
                var sql = "select * from dt_link where id=$id order by id limit 10 offset 0";
                DBOParameterCollection dpc = new DBOParameterCollection();
                dpc.SetValue("id", 1);
                var result = sq.Query(sql, dpc);
                Console.WriteLine($"sqlite cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
                result = sq.Query(sql, dpc);
                Console.WriteLine($"sqlite2 cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
            Console.WriteLine($"sqlite cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            using (MySQLAccess sq = new MySQLAccess())
            {
                
                sq.Open("server=10.15.1.240;user id=root;password=111111;database=ptac_shop;charset=utf8;Convert Zero Datetime=true;Allow Zero Datetime=true;");
                Console.WriteLine($"1:open cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
                var sql = "SELECT operating_unit_id FROM ecs_admin_user ";
                DBOParameterCollection dpc = new DBOParameterCollection();

                //dpc.SetValue("id", 1);
                var result = sq.Query(sql, dpc);
                Console.WriteLine($"1:query cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
                dpc.SetValue("userid", "167");
                result = sq.Query(sql + " where user_id=@userid", dpc);
                Console.WriteLine($"1:query2 cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
           
            using (MySQLAccess sq = new MySQLAccess())
            {

                sq.Open("server=10.15.1.240;user id=root;password=111111;database=ptac_shop;charset=utf8;Convert Zero Datetime=true;Allow Zero Datetime=true;");
                Console.WriteLine($"2:open cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
                var sql = "SELECT operating_unit_id FROM ecs_admin_user ";
                DBOParameterCollection dpc = new DBOParameterCollection();

                //dpc.SetValue("id", 1);
                var result = sq.Query(sql, dpc);
                Console.WriteLine($"2:query cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
            
        }
    }
}
