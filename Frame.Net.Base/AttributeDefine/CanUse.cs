using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.AttributeDefine
{
    /// <summary>
    /// 属性或方法的是否允许使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class CanUseAttribute:Attribute
    {
        bool _iscan = true;
        public CanUseAttribute(bool canuse)
        {
            _iscan = canuse;
        }

        /// <summary>
        /// 是否允许使用
        /// </summary>
        public bool IsCanUse
        {
            get
            {
                return _iscan;
            }
        }
    }
}
