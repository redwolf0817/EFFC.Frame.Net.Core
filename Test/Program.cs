using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using Frame.Net.Base.ResouceManage.JsEngine;
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
            //TimeCastTest.Test();
            //ReflectTest.Test();
            //FrameArrayTest.Test();
            //WebGoTest.Test();
            SqliteTest.Test();
            //ReflectVersionTest.Test();

            //LinqTest.Test();
            //JWTTest.Test();

            Console.WriteLine($"cast time:{(DateTime.Now - dt).TotalMilliseconds}ms");
            Console.Read();
        }

       
    }
}