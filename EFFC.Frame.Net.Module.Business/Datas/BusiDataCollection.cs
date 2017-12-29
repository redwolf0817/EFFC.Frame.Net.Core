using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Business.Datas
{
    /// <summary>
    /// Business模块使用的数据集合
    /// </summary>
    public class BusiDataCollection:DataCollection
    {
        /// <summary>
        /// 逻辑处理完成后的结果
        /// </summary>
        public object Result
        {
            get
            {
                return this["_result_"];
            }
            set
            {
                this["_result_"] = value;
            }
        }
    }
}
