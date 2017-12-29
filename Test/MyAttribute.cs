using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MyAttribute:Attribute
    {
        public string RestMethodName { get; }
        public MyAttribute() { }
    }
}
