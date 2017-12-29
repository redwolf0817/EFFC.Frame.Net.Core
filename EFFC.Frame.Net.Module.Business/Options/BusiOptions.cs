using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Business.Options
{
    /// <summary>
    /// BusiModule OnUsed加载时用到的选项
    /// </summary>
    public class BusiOptions
    {
        /// <summary>
        /// Logic所在dll的AssemblyName
        /// </summary>
        public string BusinessAssemblyName
        {
            get;
            set;
        }
    }
}
