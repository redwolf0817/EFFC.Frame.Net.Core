using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Test
{
    public class InvokeTest
    {
        public static void Test()
        {
            //new Starter().DoInvoke();
            var list = new List<FrameDLRObject>();
            list.Add(FrameDLRObject.CreateInstance(new
            {
                id = 1,
                name = ""
            }));
            list.Add(FrameDLRObject.CreateInstance(new
            {
                id = 2,
                name = ""
            }));
            var list2 = new List<FrameDLRObject>();
            list2.Add(FrameDLRObject.CreateInstance(new
            {
                id = 1,
                tid="A",
                name = ""
            }));
            list2.Add(FrameDLRObject.CreateInstance(new
            {
                id = 3,
                tid = "B",
                name = ""
            }));
            var list3 = new List<FrameDLRObject>();
            list3.Add(FrameDLRObject.CreateInstance(new
            {

                tid = "A",
                name = ""
            }));
            list3.Add(FrameDLRObject.CreateInstance(new
            {
                tid = "B",
                name = ""
            }));
            var lo = new List<object>();
            var s = from t in list
                    join t2 in list2 on t.id equals t2.id
                    join t3 in list3 on t2.tid equals t3.tid
                    group t2 by new
                    {
                        t.id,
                        t2.name
                    } into g
                    select new
                    {
                        g.First().id,
                        g.First().name,
                        c = g.Count()
                    };


        }
    }

    public class TypeA
    {
        string _ttt = "sss";
        public List<object> get()
        {
            return new List<object>();
        }
        public object get(string id)
        {
            Console.WriteLine(_ttt);
            return null;
        }
    }
    public class Starter
    {
        List<MethodInfo> _m = new List<MethodInfo>();
        public Starter()
        {
            _m.AddRange(typeof(TypeA).GetMethods( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance));
        }
        public void DoInvoke()
        {
            foreach(var m in _m)
            {
                var p = new List<object>();
                foreach(var pp in m.GetParameters())
                {
                    if(pp.ParameterType == typeof(string))
                    {
                        p.Add("");
                    }
                    else
                    {
                        p.Add(null);
                    }
                }
                m.Invoke(null,p.ToArray());
            }
        }
    }
}
