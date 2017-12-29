using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Test
{
    public class StaticClassTest
    {
        static string name = "";
        static Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
        public StaticClassTest()
            :this("")
        {
            if(_methods.Count <= 0)
            {
                var t = GetType();
                foreach(var m in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    _methods.Add(m.Name, m);
                }
            }
        }
        public StaticClassTest(string name)
        {
            Name = name;
        }

        static StaticClassTest()
        {
            name = "a";
        }
        public string Name { get; }
        public static void Check(params object[] obj)
        {
            Console.WriteLine($"chekced params obj.length={(obj == null ? 0 : obj.Length)}");
        }
        public void CheckMe(params object[] obj)
        {
            Console.WriteLine($"chekced params obj.length={(obj == null ? 0 : obj.Length)}");
        }

        public void Math()
        {

        }
    }

    public class StaticClassTestB: StaticClassTest
    {
        private void CalB()
        {

        }
    }
    public class StaticClassTestA : StaticClassTest
    {
        private void CalA()
        {

        }
    }
}
