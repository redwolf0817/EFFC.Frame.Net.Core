using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Consoles.Core
{
    public abstract class ConsoleBaseHandler<WP, WD> : BaseModule<WP, WD>
        where WP:ConsoleParameter
        where WD:DataCollection
    {
        protected override void OnError(Exception ex, WP p, WD d)
        {
            var errorcode = "E-" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
            GlobalCommon.ExceptionProcessor.ProcessException(this, ex, p, d);
            GlobalCommon.Logger.WriteLog(LoggerLevel.ERROR, errorcode + "\n" + ex.Message);
            p.Resources.CommitTransaction(p.CurrentTransToken);
            p.Resources.ReleaseAll();
        }
        public override string Version
        {
            get { return "0.0.1"; }
        }

        public override string Description
        {
            get { return "console批次处理器"; }
        }

        protected override void Run(WP p, WD d)
        {
            Init(p, d);
            DoProcess(p, d);
            AfterProcess(p, d);
        }

        protected abstract void DoProcess(WP p, WD d);

        protected virtual void Init(WP p, WD d)
        {
            //获取请求的资源和参数
            ResourceManage rema = new ResourceManage();
            p.SetValue<ResourceManage>(ParameterKey.RESOURCE_MANAGER, rema);
            p.SetValue<TransactionToken>(ParameterKey.TOKEN, TransactionToken.NewToken());
            if (p.ExtentionObj.args != null)
            {
                if (p.ExtentionObj.args.Length > 0)
                {
                    string reqstr = ComFunc.nvl(p.ExtentionObj.args[0]);
                    string[] arr = reqstr.Split('.');
                    p[ParameterKey.LOGIC] = arr[0];
                    p[ParameterKey.ACTION] = arr.Length > 1 ? arr[1] : "";
                }
            }
        }

        protected virtual void AfterProcess(WP p, WD d)
        {
            p.Resources.CommitTransaction(p.CurrentTransToken);
            p.Resources.ReleaseAll();
        }
    }
}
