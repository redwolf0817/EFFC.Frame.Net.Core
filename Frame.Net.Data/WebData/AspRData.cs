using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Common;

namespace EFFC.Frame.Net.Data.WebData
{
    public class AspRData : WebBaseData
    {
        public string ReponseHTML
        {
            get
            {
                return ComFunc.nvl(this["ReponseHTML"]);
            }
            set
            {
                this["ReponseHTML"] = value;
            }
        }
    }
}
