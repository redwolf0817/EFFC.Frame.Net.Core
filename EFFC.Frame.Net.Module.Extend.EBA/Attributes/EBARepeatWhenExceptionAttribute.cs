using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EBA.Attributes
{
    /// <summary>
    /// 用于描述批次指令信息
    /// </summary>
    public class EBARepeatWhenExceptionAttribute : Attribute
    {
        /// <summary>
        /// 批次的描述信息
        /// </summary>
        /// <param name="name">批次的名称</param>
        /// <param name="desc">描述</param>
        public EBARepeatWhenExceptionAttribute(Type exception_type=null,int repeat_times=1)
        {
            ExceptionType = exception_type;
            RepeatTimes = repeat_times;
        }
        /// <summary>
        /// 遇到异常的类型才执行重复
        /// </summary>
        public Type ExceptionType
        {
            get;
            protected set;
        }
        /// <summary>
        /// 重复次数
        /// </summary>
        public int RepeatTimes { get; protected set; }

    }
}
