using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Data.WebData
{
    public class WebFormData:WebBaseData
    {
        public WebFormPageModel PageModule
        {
            get { return GetValue<WebFormPageModel>("PageModule"); }
            set { SetValue("PageModule", value); }
        }

        public WebFormPageModel DataBindModule
        {
            get { return GetValue<WebFormPageModel>("DataBindModule"); }
            set { SetValue("DataBindModule", value); }
        }

        public override object Clone()
        {
            return this.Clone<WebFormData>();
        }
    }
}
