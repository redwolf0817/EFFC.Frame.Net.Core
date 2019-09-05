using EFFC.Extends.HostJs;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Test
{
    class TimeCastTest
    {
        public static void Test()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var dt = DateTime.Now;
            var sctb = new StaticClassTest("a");
            var m_type = sctb.GetType();
            Console.WriteLine($"1.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;

            var method = m_type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            List<string> atl = new List<string>();
            foreach (var m in method)
            {
                if (m.CustomAttributes.Count() > 0)
                {
                    foreach (var ca in m.CustomAttributes)
                    {
                        atl.Add(ca.AttributeType.Name.Replace("attribute", ""));
                    }
                }
            }
            //var list = m_type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            //       .Where(m => m.Name.ToLower() == "post")
            //       .OrderBy(p => p.GetParameters().Length)
            //       .ToList();
            Console.WriteLine($"2.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            atl.Clear();
            for (int i = 0; i < 10; i++)
            {
                var l = method
                    .Where(m => m.CustomAttributes.Count() > 0)
                    .Select(m => m.CustomAttributes.Select(sl => sl.AttributeType.Name.Replace("attribute", "")));
                Console.WriteLine($"3.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
            //foreach (var item in l)
            //{
            //    atl.AddRange(item);
            //}
            Console.WriteLine($"3.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                var l = method
                    .Where(m => !m.IsGenericMethod)
                    .Where(m => m.CustomAttributes.Count() > 0)
                    .Select(m => m.CustomAttributes.Select(sl => sl.AttributeType.Name.Replace("attribute", "")));
                Console.WriteLine($"3.1.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
            var filterlist = atl.Distinct().ToList();
            Console.WriteLine($"4.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;

            var s = FrameExposedClass.From(typeof(StaticClassTest));
            Console.WriteLine($"5.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            var ff = FrameExposedObject.From(typeof(StaticClassTest));
            Console.WriteLine($"6.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            s.Check(new object[] { "ych", 1 });
            //StaticClassTest.Check("a");
            Console.WriteLine($"7.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            ff.CheckMe(new object[] { "ych2", 2 });
            Console.WriteLine($"8.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            s.Check(new object[] { "ych", 1 });
            //StaticClassTest.Check("a");
            Console.WriteLine($"9.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            HostJs.DefaultEngineName = "";
            var fdlr = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:''
}");
            Console.WriteLine($"10" +
                $".time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            for (var i = 0; i < 10; i++)
            {
                var fdlr2 = FrameDLRObject.CreateInstanceFromat(@"{
issuccess:false,
msg:'{0}'
}", DateTime.Now.ToString("fff"));
                Console.WriteLine($"11" +
                $".time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
            for (var i = 0; i < 10; i++)
            {
                var fdlr2 = FrameDLRObject.CreateInstanceFromat(@"{
issuccess:true,
name:{1},
msg:'{0}',
ep:{
        age:20,
        sex:'male'
    }
}", DateTime.Now.ToString("fff"), "ych");
                Console.Write(((FrameDLRObject)fdlr2).ToJSONString());
                Console.WriteLine($"11.1" +
                $".time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
            for (var i = 0; i < 10; i++)
            {
                var fdlr2 = FrameDLRObject.CreateInstanceFromat(@"{
issuccess:false,
msg:'{0}'
}", DateTime.Now.ToString("fff"));
                Console.WriteLine($"11.2" +
                $".time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
            //fdlr.getvalue("a");

            for (var i = 0; i < 10; i++)
            {
                var fdlr3 = FrameDLRObject.CreateInstance(new
                {
                    issuccess = false,
                    msg = ""
                });
                Console.WriteLine($"12" +
                    $".time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
                ((FrameDLRObject)fdlr3).ToJSONString();
                Console.WriteLine($"12.1" +
                    $".time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
            for (var i = 0; i < 10; i++)
            {
                var fdlr3 = FrameDLRObject.CreateInstance(new
                {
                    issuccess = true,
                    msg = "哈哈"
                });
                Console.WriteLine($"13" +
                    $".time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;

                Console.WriteLine($"13.1" +
                    $".time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
            //s.Check(new object[] { "ych", 1 });
            //Console.WriteLine($"time cast {(DateTime.Now - dt).TotalMilliseconds}ms");
            //dt = DateTime.Now;
            //s = FrameExposedClass.From(typeof(StaticClassTestB));
            //if (((FrameExposedClass)s).HasMethod("Check"))
            //    s.Check(new object[] { "ych", 1, 23, 1 });
            //Console.WriteLine($"time cast {(DateTime.Now - dt).TotalMilliseconds}ms");
            //dt = DateTime.Now;
            //List<int> realList = new List<int>();

            //dynamic exposedList = FrameExposedObject.From(realList);

            //// Read a private field - prints 0
            //Console.WriteLine(exposedList._size);

            //// Modify a private field
            //exposedList._items = new int[] { 5, 4, 3, 2, 1 };

            //// Modify another private field
            //exposedList._size = 5;

            //// Call a private method
            //exposedList.EnsureCapacity(20);


            //// Add a value to the list
            //exposedList.Add(0);

            //// Enumerate the list. Prints "5 4 3 2 1 0"
            //foreach (var x in exposedList) Console.WriteLine(x);

            //Console.WriteLine($"time cast {(DateTime.Now - dt).TotalMilliseconds}ms");
            var jse = HostJs.NewInstance("chakra");
            for (var i = 0; i < 10; i++)
            {

                var re = jse.Evaluate(@"function test(){c.WriteLine('js run');return {issuccess:true,msg:''}} test();", new KeyValuePair<string, object>("c", new Conss()));

                Console.WriteLine($"20.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
            for (var i = 0; i < 10; i++)
            {

                var re = jse.Evaluate(@"function test(){c.WriteLine('js2 run'+new Date().getYear());return {issuccess:false,msg:'呵呵'}} test();", new KeyValuePair<string, object>("c", new Conss2()));

                Console.WriteLine($"20.time cast {(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            }
            jse.Dispose();
            //dt = DateTime.Now;
            //jse = HostJs.NewInstance("");
            //re = jse.Evaluate(@"function test2(){c.WriteLine('js2 run');return {issuccess:false,msg:''}} test2();", new KeyValuePair<string, object>("c", new Conss()));
            //jse.Dispose();
            //Console.WriteLine($"time cast {(DateTime.Now - dt).TotalMilliseconds}ms");
            Console.ReadKey();
        }

        public class Conss
        {
            public void WriteLine(string msg)
            {
                Console.WriteLine(msg);
            }
        }

        public class Conss2
        {
            public void WriteLine(string msg)
            {
                Console.WriteLine(@"哈哈哈=" + msg);
            }
        }
    }
}
