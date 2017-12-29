using System;
using System.Collections.Generic;
using System.Text;

namespace Frame.Net.Base.Interfaces.DataConvert
{
    public interface IJSONParsable
    {
        /// <summary>
        /// 尝试从动态对象中转化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool TryParseJSON(object obj);
    }
}
