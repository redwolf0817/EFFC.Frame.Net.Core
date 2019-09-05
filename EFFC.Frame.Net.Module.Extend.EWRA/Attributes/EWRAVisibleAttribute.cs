using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Attributes
{
    /// <summary>
    /// 设定接口方法是否可见，没有设定默认为可见
    /// 与IsOpen不同，接口只是不显示在接口说明列表上，但依然可以使用，而IsOpen则决定该接口方法是否可用
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EWRAVisibleAttribute:Attribute
    {
        /// <summary>
        /// 设定接口是否在接口列表上可见
        /// </summary>
        /// <param name="is_visible"></param>
        public EWRAVisibleAttribute(bool is_visible=true)
        {
            IsVisible = is_visible;
        }
        /// <summary>
        /// 设定是否可见
        /// </summary>
        public bool IsVisible { get; private set; }
    }
}
