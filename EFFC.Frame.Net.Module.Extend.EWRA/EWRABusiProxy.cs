using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA
{
    public class EWRABusiProxy : ModuleProxy
    {
        protected override DataCollection ConvertDataCollection(ref object obj)
        {
            if (obj is EWRAData)
            {
                return (EWRAData)obj;
            }
            else
            {
                return new EWRAData();
            }
        }

        protected override ParameterStd ConvertParameters(object[] obj)
        {
            var rtn = new EWRAParameter();
            if (obj.Length > 0 && (obj[0] is EWRAParameter))
            {
                rtn = (EWRAParameter)obj[0];
            }
            return rtn;
        }

        protected override BaseModule CreateModuleInstance()
        {
            return new EWRABusiModule();
        }

        protected override void Dispose(ParameterStd p, DataCollection d)
        {
            
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            throw ex;
        }

        protected override void ParseDataCollection2Result(DataCollection d, ref object obj)
        {
           
        }
    }
}
