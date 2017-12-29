using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Module.Web.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Global;
using System.Reflection;
using EFFC.Frame.Net.Base.Constants;
using System.Threading;

namespace EFFC.Frame.Net.Module.Extend.WebGo
{
    public class WebGoProxy : ModuleProxy
    {
        static Type moduletype = null;
        protected override void MyUsed(ProxyManager ma, dynamic options)
        {
            var typename = "";
            if (ComFunc.nvl(options.WebModuleName) != null)
            {
                typename = ComFunc.nvl(options.WebModuleName);
            }
            else
            {
                typename = "WebGo";
            }

            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO,
                       string.Format("Web的运行模块为{0},如要调整，请在ProxyManager.UseProxy中的options参数设定WebModuleName的值", typename));

            if (typename == "WebGo")
                moduletype = typeof(WebGo);
            else
            {

                var t = Assembly.GetEntryAssembly().GetType(typename);
                if (t == null)
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.WARN,
                       string.Format("警告：名称为{0}的Web模块未找到，系统使用默认微信Web处理模块进行替代,如要调整，请确保在ProxyManager.UseProxy中的options.WebModuleName的值为FullName，并且该类型已引用", typename));
                    moduletype = typeof(WebGo);
                }
                else
                {
                    if (t.GetTypeInfo().IsSubclassOf(typeof(WebGo)))
                        moduletype = t;
                    else
                    {
                        GlobalCommon.Logger.WriteLog(LoggerLevel.WARN,
                            string.Format("警告：名称为{0}的Web模块不是WebGo类型的处理模块，系统使用默认Web处理模块进行替代,如要调整，请确保在定义模块时继承WebGo", typename));
                        moduletype = typeof(WebGo);
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
            return new GoData();
        }

        protected override ParameterStd ConvertParameters(object[] obj)
        {
            var fea = FrameExposedArray.From(obj);
            var rtn = new WebParameter();
            rtn.CurrentHttpContext = fea.defaulthttpcontext.value;
            return rtn;
        }

        protected override void Dispose(ParameterStd p, DataCollection d)
        {
            var wp = (WebParameter)p;
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
