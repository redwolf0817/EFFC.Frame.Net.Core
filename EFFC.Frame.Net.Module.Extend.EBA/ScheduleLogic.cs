using EFFC.Frame.Net.Module.Extend.EConsole.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Module.Extend.EConsole.DataCollections;
using EFFC.Frame.Net.Module.Extend.EConsole.Parameters;
using EFFC.Frame.Net.Module.Business.Logic;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Constants;
using System.Reflection;
using System.Linq;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Common;

namespace EFFC.Frame.Net.Module.Extend.EBA
{
    public partial class ScheduleLogic: ConsoleLogic
    {
        protected DateTime NextExcuteTime
        {
            get { return DateTimeStd.IsDateTime(CallContext_Parameter.ExtentionObj.Next_Excute_Time) ? DateTimeStd.ParseStd(CallContext_Parameter.ExtentionObj.Next_Excute_Time) : DateTime.MaxValue ; }
        }
        /// <summary>
        /// 当前执行任务的名称
        /// </summary>
        protected string CurrentExcuteName
        {
            get
            {
                return ComFunc.nvl(CallContext_Parameter.ExtentionObj.ExcuteName);
            }
        }
        /// <summary>
        /// 当前执行任务的描述
        /// </summary>
        protected string CurrentExcuteDescription
        {
            get
            {
                return ComFunc.nvl(CallContext_Parameter.ExtentionObj.ExcuteDescription);
            }
        }
        protected override void DoProcess(ConsoleParameter p, ConsoleData d)
        {
            try
            {
                base.DoProcess(p, d);
            }
            catch (Exception ex)
            {
                OnTaskError(ex);
                throw ex;
            }
            finally
            {
                AfterExcute();
            }
        }

        protected virtual void OnTaskError(Exception ex)
        {
            throw new NotImplementedException();
        }

        public virtual void AfterExcute() { }
        
    }
}
