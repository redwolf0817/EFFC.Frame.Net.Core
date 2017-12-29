using EFFC.ChakraCore;
using EFFC.VRoomJs;
using Frame.Net.Base.Exceptions;
using JavaScriptEngineSwitcher.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VroomJs;

namespace Frame.Net.Base.ResouceManage.JsEngine
{
    /// <summary>
    /// Js引擎切换器
    /// </summary>
    public class JsSwitcher : HostJs
    {
        JsEngineSwitcher engineSwitcher = JsEngineSwitcher.Instance;
        IJsEngine js = null;
        bool isdisposed = false;
        object jscontext = null;
        /// <summary>
        /// 默认使用chakra引擎
        /// </summary>
        public JsSwitcher() : this("")
        {

        }
        /// <summary>
        /// 初始化一个js引擎转换器，默认使用chakra引擎，
        /// chakra：该引擎在windows和ubuntu上都可以良好运行，edeg的内核引擎
        /// VRoomJs：该引擎可以在windows上良好运行，使用v8引擎
        /// </summary>
        /// <param name="name"></param>
        public JsSwitcher(string name="Chakra")
        {
            engineSwitcher.EngineFactories
                .AddEFFCChakraCore()
                .AddEFFCVroom(new EFFCVroomSettings
                {
                    MaxYoungSpaceSize = 4194304,
                    MaxOldSpaceSize = 8388608
                });
            switch (name.ToLower())
            {
                case "chakra":
                    engineSwitcher.DefaultEngineName = EFFCChakraCoreJsEngine.EngineName;
                    break;
                case "vroomjs":
                    engineSwitcher.DefaultEngineName = EFFCVroomJsEngine.EngineName;
                    break;
                default:
                    engineSwitcher.DefaultEngineName = EFFCChakraCoreJsEngine.EngineName;
                    break;
            }
            js = engineSwitcher.CreateDefaultEngine();
            isdisposed = false;
        }
        public override object Evaluate(string script, params KeyValuePair<string, object>[] kvp)
        {
            object result = null;
            if(kvp != null)
            {
                foreach(var obj in kvp)
                {
                    js.EmbedHostObject(obj.Key, obj.Value);
                }
            }
            try
            {
                result = js.Evaluate(script);
            }
            catch (JavaScriptEngineSwitcher.Core.JsException jse)
            {
                if(jse.InnerException != null)
                {
                    if (jse.InnerException is VroomJs.JsException)
                    {
                        var vjse = (VroomJs.JsException)jse.InnerException;
                        throw new HostJsException(jse.Message, script, vjse.Line, vjse.Column);
                    }
                }
                else
                {
                    throw new HostJsException(jse.Message, script, 0, 0);
                }
            }

            return result;
        }

        public override void Excute(string script, params KeyValuePair<string, object>[] kvp)
        {
            if (kvp != null)
            {
                foreach (var obj in kvp)
                {
                    js.EmbedHostObject(obj.Key, obj.Value);
                }
            }
            try
            {
                js.Execute(script);
            }
            catch (JavaScriptEngineSwitcher.Core.JsException jse)
            {
                if (jse.InnerException != null)
                {
                    if (jse.InnerException is VroomJs.JsException)
                    {
                        var vjse = (VroomJs.JsException)jse.InnerException;
                        throw new HostJsException(jse.Message, script, vjse.Line, vjse.Column);
                    }
                }
                else
                {
                    throw new HostJsException(jse.Message, script, 0, 0);
                }
            }
        }

        public override object GetOutObject(string key)
        {
            return js.GetVariableValue(key);
        }

        public override bool IsDisposed()
        {
            return isdisposed;
        }

        public override void Release()
        {
            isdisposed = true;
            js.Dispose();
        }
    }
}
