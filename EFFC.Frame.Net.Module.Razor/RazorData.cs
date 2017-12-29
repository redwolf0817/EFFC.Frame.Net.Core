using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Razor
{
    public class RazorData:DataCollection
    {
        /// <summary>
        /// 渲染完成的结果
        /// </summary>
        public string RenderText
        {
            get;set;
        }
    }
}
