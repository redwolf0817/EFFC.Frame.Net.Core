using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Attributes
{
    /// <summary>
    /// 路由描述
    /// </summary>
    public class EWRARouteDescAttribute : Attribute
    {
        public EWRARouteDescAttribute(string desc)
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
