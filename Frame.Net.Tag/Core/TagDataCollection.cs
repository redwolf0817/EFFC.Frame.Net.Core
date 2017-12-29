using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;

namespace EFFC.Frame.Net.Tag.Core
{
    public class TagDataCollection:DataCollection
    {
        /// <summary>
        /// 解析完成之后的结果对象
        /// </summary>
        public object ResultObj
        {
            get
            {
                if (GetValue("result", "obj") != null)
                    return GetValue("result", "obj");
                else
                    return null;
            }
            set
            {
                SetValue("result", "obj", value);
            }
        }
    }
}
