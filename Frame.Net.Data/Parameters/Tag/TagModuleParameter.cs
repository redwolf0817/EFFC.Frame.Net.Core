using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Parameter;

namespace EFFC.Frame.Net.Data.Parameters.Tag
{
    public class TagModuleParameter:ParameterStd
    {
        /// <summary>
        /// 待解析文本
        /// </summary>
        public string OriginalText
        {
            get
            {
                return GetValue<string>("__OriginalText");
            }
            set
            {
                SetValue("__OriginalText", value);
            }
        }
    }
}
