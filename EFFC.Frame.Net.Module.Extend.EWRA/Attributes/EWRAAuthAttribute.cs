using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Attributes
{
    /// <summary>
    /// 是否执行授权检核
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EWRAAuthAttribute:Attribute
    {
        /// <summary>
        /// 设定是否执行授权检核
        /// </summary>
        /// <param name="isauth"></param>
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
