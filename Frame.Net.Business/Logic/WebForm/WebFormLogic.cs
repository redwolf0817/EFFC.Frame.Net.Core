using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Business.Logic.WebForm
{
    /// <summary>
    /// 提供给Webform business进行类型区分使用
    /// </summary>
    public abstract class WebFormLogic:WebBaseLogic<WebParameter,WebFormData>
    {
    }
}
