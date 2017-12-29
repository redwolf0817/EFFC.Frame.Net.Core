using System;
using System.Collections.Generic;
using System.Text;

namespace Test.v1._1
{
    class LogicC:LogicB
    {
        object get(string id, dynamic parent_info)
        {
            Console.WriteLine($"{Name} excute,id={id}");
            return null;
        }
    }
}
