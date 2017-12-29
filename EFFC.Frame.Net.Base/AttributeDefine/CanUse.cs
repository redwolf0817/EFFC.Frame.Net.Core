using System;

namespace EFFC.Frame.Net.Base.AttributeDefine
{
    /// <summary>
    /// 属性或方法的是否允许使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class CanUseAttribute:Attribute
    {
        bool _iscan = true;
        /// <summary>
        /// 属性或方法的是否允许使用
        /// </summary>
        /// <param name="canuse"></param>
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
