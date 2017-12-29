using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Data;

namespace EFFC.Frame.Net.Data.LogicData
{
    public partial class LogicData
    {
        /// <summary>
        /// 转到页号
        /// </summary>
        public int ToPage { get; set; }
        /// <summary>
        /// 当前页号
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 每页笔数
        /// </summary>
        public int Count_Per_Page { get; set; }
    }
}
