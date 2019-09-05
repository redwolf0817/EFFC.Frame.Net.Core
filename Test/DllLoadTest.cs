using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Test
{
    public class DllLoadTest
    {
        public static void Test()
        {
            var dll = Assembly.LoadFile("D:\\work\\publish\\RH\\RDTP.dll");
            var logics = dll.GetTypes().Where(p => p.GetTypeInfo().IsSubclassOf(typeof(ValidRestLogic)));
            foreach(var l in logics)
            {
                Console.WriteLine(l.Name);
            }
        }
    }
}
