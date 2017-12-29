using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EWRAAuthAttribute:Attribute
    {
        public EWRAAuthAttribute(bool isauth)
        {
            IsNeedAuth = isauth;
        }
        /// <summary>
        /// 是否需要进行校验
        /// </summary>
        public bool IsNeedAuth
        {
            get;
            set;
        }
    }
}
