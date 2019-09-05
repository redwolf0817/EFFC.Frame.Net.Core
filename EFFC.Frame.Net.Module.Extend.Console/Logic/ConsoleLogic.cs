using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Module.Business.Logic;
using EFFC.Frame.Net.Module.Extend.EConsole.DataCollections;
using EFFC.Frame.Net.Module.Extend.EConsole.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EConsole.Logic
{
    /// <summary>
    /// Console的业务量逻辑处理父类
    /// </summary>
    public partial class ConsoleLogic : BaseLogic<ConsoleParameter, ConsoleData>
    {
        
        protected override void DoProcess(ConsoleParameter p, ConsoleData d)
        {
            FrameDLRObject args = FrameDLRObject.CreateInstance();
            foreach(var item in p.Domain(DomainKey.INPUT_PARAMETER))
            {
                args.SetValue(item.Key, item.Value);
            }
            foreach (var item in p.Domain(DomainKey.CUSTOMER_PARAMETER))
            {
                args.SetValue(item.Key, item.Value);
            }
            var action = string.IsNullOrEmpty(p.CallAction) ? "load" : p.CallAction;
            var invokelist = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(t => t.Name.ToLower() == action.ToLower() && t.GetParameters().Count()==1).ToList();
            if(invokelist.Count() != 1)
            {
                throw new NotFoundException($"未找到名为{p.CallAction}的执行方法");
            }

            d.Result = invokelist[0].Invoke(this, new object[] { args });
        }
    }
}
