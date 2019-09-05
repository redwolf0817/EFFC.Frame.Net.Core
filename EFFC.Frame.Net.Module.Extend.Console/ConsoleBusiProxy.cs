using EFFC.Frame.Net.Base.Module.Proxy;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Extend.EConsole.DataCollections;
using EFFC.Frame.Net.Module.Extend.EConsole.Parameters;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Module.Extend.EConsole
{
    public class ConsoleBusiProxy : ModuleProxy
    {
        protected override DataCollection ConvertDataCollection(ref object obj)
        {
            return new ConsoleData();
        }

        protected override ParameterStd ConvertParameters(object[] obj)
        {
            var p = new ConsoleParameter();
            if(obj!=null & obj.Length > 0)
            {
                var la = ComFunc.nvl(obj[0]).Split('.',StringSplitOptions.RemoveEmptyEntries);
                p.CallLogicName = la.Length > 0 ? la[0] : "";
                p.CallAction = la.Length > 1 ? la[1] : "";
            }
            //config
            foreach (var item in MyConfig.GetConfigurationList("ConnectionStrings"))
            {
                p[DomainKey.CONFIG, item.Key] = ComFunc.nvl(item.Value);
            }
            p.DBConnectionString = ComFunc.nvl(p[DomainKey.CONFIG, "DefaultConnection"]);
            bool bvalue = true;
            foreach (var item in MyConfig.GetConfigurationList("EFFC"))
            {
                if (bool.TryParse(ComFunc.nvl(item.Value), out bvalue))
                {
                    p[DomainKey.CONFIG, item.Key] = bool.Parse(ComFunc.nvl(item.Value));
                }
                else if (DateTimeStd.IsDateTime(item.Value))
                {
                    p[DomainKey.CONFIG, item.Key] = DateTimeStd.ParseStd(item.Value).Value;
                }
                else
                {
                    p[DomainKey.CONFIG, item.Key] = ComFunc.nvl(item.Value);
                }
            }

            return p;
        }
        
        protected override BaseModule CreateModuleInstance()
        {
            return new ConsoleBusiModule();
        }

        protected override void Dispose(ParameterStd p, DataCollection d)
        {
            p.Dispose();
            d.Dispose();
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            p.Resources.RollbackTransaction(p.CurrentTransToken);
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.ERROR, ex.Message + "\n" + ex.StackTrace);
        }

        protected override void ParseDataCollection2Result(DataCollection d, ref object obj)
        {
            
        }
    }
}
