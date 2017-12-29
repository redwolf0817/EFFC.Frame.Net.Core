using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    /// <summary>
    /// 路由描述
    /// </summary>
    public class RouteDescAttribute : Attribute
    {
        public RouteDescAttribute(string desc)
        {
            Desc = desc;
        }

        public string Desc
        {
            get;
            protected set;
        }
    }
}
