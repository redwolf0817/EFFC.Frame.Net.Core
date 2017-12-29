using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Module.Business.Datas;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.WebGo.Datas
{
    public class GoBusiData:BusiDataCollection
    {
        /// <summary>
        /// 从WebModule中传过来的data
        /// </summary>
        public GoData WebData
        {
            get;
            set;
        }
    }
}
