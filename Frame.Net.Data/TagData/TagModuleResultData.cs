using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Data.TagData
{
    public class TagModuleResultData:DataCollection
    {
        /// <summary>
        /// 中间文本
        /// </summary>
        public string MiddleText
        {
            get;
            set;
        }
        /// <summary>
        /// 全局文本
        /// </summary>
        public string GlobalText
        {
            get;
            set;
        }
    }
}
