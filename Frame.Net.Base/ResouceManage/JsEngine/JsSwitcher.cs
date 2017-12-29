using Frame.Net.Base.Exceptions;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Msie;
using JavaScriptEngineSwitcher.Vroom;
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
        /// msie：该引擎可以在windows上良好运行，采用IE的内核引擎，可以在多种引擎模式间转换
        /// </summary>
        /// <param name="name"></param>
        public JsSwitcher(string name="Chakra")
        {
            engineSwitcher.EngineFactories
                .AddChakraCore()
                .AddVroom(new VroomSettings
                {
                    MaxYoungSpaceSize = 4194304,
                    MaxOldSpaceSize = 8388608
                })
                .AddMsie(new MsieSettings
                {
                    EngineMode = JsEngineMode.Auto
                });
            switch (name.ToLower())
            {
                case "chakra":
                    engineSwitcher.DefaultEngineName = ChakraCoreJsEngine.EngineName;
                    break;
                case "vroomjs":
                    engineSwitcher.DefaultEngineName = VroomJsEngine.EngineName;
                    break;
                case "msie":
                    engineSwitcher.DefaultEngineName = MsieJsEngine.EngineName;
                    break;
                default:
                    engineSwitcher.DefaultEngineName = ChakraCoreJsEngine.EngineName;
                    break;
            }
            js = engineSwitcher.CreateDefaultEngine();
            isdisposed = false;
        }
        public override void Evaluate(string script, params KeyValuePair<string, object>[] kvp)
        {
            if(kvp != null)
            {
                foreach(var obj in kvp)
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
        }

        public override object GetOutObject(string key)
        {
            return Convert2Object(js, js.GetVariableValue(key));
        }

        private object Convert2Object(IJsEngine js, object obj)
        {
            switch (js.Name) {
                case VroomJsEngine.EngineName:
                    return Convert2Object_VroomJs(js,obj);
                case ChakraCoreJsEngine.EngineName:
                    return Convert2Object_Chakra(js,obj);
                default:
                    return obj;
            }
        }

        private object Convert2Object_VroomJs(IJsEngine js, object obj)
        {
            if (obj is JsObject)
            {
                var dic = new Dictionary<string, object>();
                var jo = (JsObject)obj;
                var vroomjs = (VroomJsEngine)js;
                foreach (var item in vroomjs.CurrentContext.GetMemberNames(jo))
                {
                    var v = vroomjs.CurrentContext.GetPropertyValue(jo, item);
                    dic.Add(item, Convert2Object_VroomJs(js, v));
                }

                return dic;
            }
            else if (obj is DateTime)
            {
                return ((DateTime)obj).ToLocalTime();
            }
            else if (obj is JsFunction)
            {
                var jf = (JsFunction)obj;
                return jf.ToString();
            }
            else
            {
                return obj;
            }
        }

        private object Convert2Object_Chakra(IJsEngine js,object obj)
        {
            return obj;
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
