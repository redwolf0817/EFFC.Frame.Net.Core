using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Web.Datas
{
    /// <summary>
    /// Rest请求的专用DataCollection
    /// </summary>
    public class RestDataCollection:DataCollection
    {
        /// <summary>
        /// 是否执行cache操作
        /// </summary>
        public bool IsCache
        {
            get
            {
                if (this["__IS_CACHE__"] == null)
                {
                    this["__IS_CACHE__"] = true;
                }
                return (bool)this["__IS_CACHE__"];
            }
            set
            {
                this["__IS_CACHE__"] = value;
            }
        }
        /// <summary>
        /// 返回的结果对象
        /// </summary>
        public object Result
        {
            get;
            set;
        }
        
    }
}
