using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var dt = DateTime.Now;
            DllLoadTest.Test();
            //JSEngineTest.Test();
            //ReflectTest.Test();
            //FrameArrayTest.Test();
            //WebGoTest.Test();
            //SqliteTest.Test();
            //ReflectVersionTest.Test();

            //LinqTest.Test();
            //InvokeTest.Test();
            //DaoLinqSqlTest.test();
            //MobileAPITest.Test();
            //RazorTest.Test();
            //JWTTest.Test();
            //JiangsuTest.Test();
            //HWAPITest.Test();
            //PostgreSqlTest.Test();
            //OracleTest.Test();
            //MiniTools.Test();
            //BaiduTest.test();
            //NPoiTest.Test();
            Console.WriteLine("当前应用路径："+ComFunc.GetApplicationRoot());
            Console.WriteLine($"cast time:{(DateTime.Now - dt).TotalMilliseconds}ms");
            Console.Read();
        }

       
    }
}