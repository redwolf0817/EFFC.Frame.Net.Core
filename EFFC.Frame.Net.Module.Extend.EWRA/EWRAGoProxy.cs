using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="LType"></typeparam>
    public class EWRAGoProxy : ModuleProxy
    {
        static Type moduletype = null;
        protected override void MyUsed(ProxyManager ma, dynamic options)
        {
            var typename = "";
            if (ComFunc.nvl(options.RestAPIModuleName) != "")
            {
                typename = ComFunc.nvl(options.RestAPIModuleName);
            }
            else
            {
                typename = "EWRA";
            }

            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO,
                       string.Format("RestAPI的运行模块为{0},如要调整，请在ProxyManager.UseProxy中的options参数设定RestAPIModuleName的值", typename));

            if (typename == "EWRA")
                moduletype = typeof(EWRAGo);
            else
            {

                var t = Assembly.GetEntryAssembly().GetType(typename);
                if (t == null)
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.WARN,
                       string.Format("警告：名称为{0}的RestAPI模块未找到，系统使用默认EWRA处理模块进行替代,如要调整，请确保在ProxyManager.UseProxy中的options.RestAPIModuleName的值为FullName，并且该类型已引用", typename));
                    moduletype = typeof(EWRAGo);
                }
                else
                {
                    if (t.GetTypeInfo().IsSubclassOf(typeof(EWRAGo)))
                        moduletype = t;
                    else
                    {
                        GlobalCommon.Logger.WriteLog(LoggerLevel.WARN,
                            string.Format("警告：名称为{0}的模块不是EWRA类型的处理模块，系统使用默认RestAPI处理模块进行替代,如要调整，请确保在定义模块时继承EWRAGo", typename));
                        moduletype = typeof(EWRAGo);
                    }
                }
            }
        }
        protected override BaseModule CreateModuleInstance()
        {
            return (BaseModule)FrameExposedObject.Create(moduletype);
        }
        protected override DataCollection ConvertDataCollection(ref object obj)
        {
            return new EWRAData();
        }

        protected override ParameterStd ConvertParameters(object[] obj)
        {
            var fea = FrameExposedArray.From(obj);
            var rtn = new EWRAParameter();
            rtn.CurrentHttpContext = fea.defaulthttpcontext.value;
            return rtn;
        }

        protected override void Dispose(ParameterStd p, DataCollection d)
        {
            var wp = (EWRAParameter)p;
            wp.Dispose();
            d.Dispose();
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
