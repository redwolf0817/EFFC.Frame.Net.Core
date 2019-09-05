using EFFC.Frame.Net.Module.Extend.WebGo;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Constants;
using System.Reflection;

namespace EFFC.Frame.Net.Module.Extend.WeixinWeb
{
    public class WeixinWebGoProxy : WebGoProxy
    {
        static Type moduletype = null;
        protected override void MyUsed(ProxyManager ma, dynamic options)
        {
            var TypeName_WeixinModule = "";
            if (ComFunc.nvl(options.WeixinModuleName) != "")
            {
                TypeName_WeixinModule = ComFunc.nvl(options.WeixinModuleName);
            }
            else
            {
                TypeName_WeixinModule = "WeixinWebGo";
            }

            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO,
                       string.Format("{1}的运行模块为{0},如要调整，请在ProxyManager.UseProxy中的options参数设定WeixinModuleName的值", TypeName_WeixinModule,this.GetType().Name));

            if (TypeName_WeixinModule == "WeixinWebGo")
                moduletype = typeof(WeixinWebGo);
            else
            {
                
                var t = Assembly.GetEntryAssembly().GetType(TypeName_WeixinModule);
                if (t == null)
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.WARN,
                       string.Format("警告：名称为{0}的微信Web模块未找到，系统使用默认微信Web处理模块进行替代,如要调整，请确保在ProxyManager.UseProxy中的options.WeixinModuleName的值为FullName，并且该类型已引用", TypeName_WeixinModule));
                    moduletype = typeof(WeixinWebGo);
                }
                else
                {
                    if (t.GetTypeInfo().IsSubclassOf(typeof(WeixinWebGo)))
                        moduletype = t;
                    else
                    {
                        GlobalCommon.Logger.WriteLog(LoggerLevel.WARN,
                            string.Format("警告：名称为{0}的微信Web模块不是微信Web的处理模块，系统使用默认微信Web处理模块进行替代,如要调整，请确保在定义模块时继承WeixinWebGo", TypeName_WeixinModule));
                        moduletype = typeof(WeixinWebGo);
                    }
                }
            }
        }
        protected override BaseModule CreateModuleInstance()
        {
            return (BaseModule)FrameExposedObject.Create(moduletype);
        }
    }
}
