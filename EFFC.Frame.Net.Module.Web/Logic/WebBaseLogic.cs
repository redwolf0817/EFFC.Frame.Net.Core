using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Business.Logic;
using EFFC.Frame.Net.Module.Web.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Web.Logic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="PType"></typeparam>
    /// <typeparam name="DType"></typeparam>
    public abstract partial class WebBaseLogic<PType,DType> : BaseLogic<PType, DType>
        where PType:ParameterStd
        where DType: DataCollection
    {
    }
}
