using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Module.Business.Datas;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Extend.WebGo.Parameters;
using EFFC.Frame.Net.Module.Web.Parameters;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Module.Extend.WebGo
{
    public class GoBusiProxy : ModuleProxy
    {
        protected override DataCollection ConvertDataCollection(ref object obj)
        {
            var rtn = new GoBusiData();
            if(obj is GoData)
            {
                var gd = (GoData)obj;
                rtn.WebData = gd;
            }
            return rtn;
        }

        protected override ParameterStd ConvertParameters(object[] obj)
        {
            var rtn = new GoBusiParameter();
            if (obj.Length > 0 && (obj[0] is WebParameter))
            {
                rtn.WebParam = (WebParameter)obj[0];
                rtn.CallLogicName = rtn.WebParam.RequestResourceName;
                rtn.CallAction = rtn.WebParam.Action;
                foreach (var s in rtn.WebParam.Domain(DomainKey.CONFIG))
                {
                    rtn.SetValue(DomainKey.CONFIG, s.Key, s.Value);
                }
            }
            return rtn;
        }

        protected override BaseModule CreateModuleInstance()
        {
            return new GoBusiModule();
        }

        protected override void Dispose(ParameterStd p, DataCollection d)
        {
            var gbp = (GoBusiParameter)p;
            var gbd = (GoBusiData)d;

            gbp.WebParam = null;
            gbd.WebData = null;
            gbp.Dispose();
            gbd.Dispose();
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            throw ex;
        }

        protected override void ParseDataCollection2Result(DataCollection d, ref object obj)
        {
            var gbd = (GoBusiData)d;
            if(obj is GoData)
            {
                obj = gbd.WebData;
            }
            
        }
    }
}
