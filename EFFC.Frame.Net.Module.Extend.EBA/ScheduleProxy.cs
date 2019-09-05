using EFFC.Frame.Net.Base.Module.Proxy;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Extend.EConsole.DataCollections;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.EConsole.Parameters;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Global;

namespace EFFC.Frame.Net.Module.Extend.EBA
{
    public class ScheduleProxy : ModuleProxy
    {
        protected override DataCollection ConvertDataCollection(ref object obj)
        {
            return new ConsoleData();
        }

        protected override ParameterStd ConvertParameters(object[] obj)
        {
            var p = new ConsoleParameter();
            if (obj != null & obj.Length > 0)
            {
                
                var command = ComFunc.nvl(obj[0]).ToLower();
                if(command == "stop")
                {
                    p.ExtentionObj.stop = true;
                }
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
            return new ScheduleModule();
        }

        protected override void Dispose(ParameterStd p, DataCollection d)
        {
            p.Dispose();
            d.Dispose();
            GC.Collect();
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            p.Resources.RollbackTransaction(p.CurrentTransToken);
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.ERROR, ex.Message + "\n" + ex.StackTrace);
            if (ex.InnerException != null)
            {
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.ERROR, ex.InnerException.Message + "\n" + ex.InnerException.StackTrace);
            }
        }

        protected override void ParseDataCollection2Result(DataCollection d, ref object obj)
        {
            
        }
    }
}
