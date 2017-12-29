using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;

namespace EFFC.Frame.Net.Base.AttributeDefine
{
    /// <summary>
    /// property的数据类型转换器
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConvertorAttribute : Attribute
    {
        IDataConvert _c = null;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="convert"></param>
        public ConvertorAttribute(IDataConvert convert)
        {
            _c = convert;
        }
        /// <summary>
        /// 转换器
        /// </summary>
        public IDataConvert Convertor
        {
            get { return _c; }
        }
    }
}
