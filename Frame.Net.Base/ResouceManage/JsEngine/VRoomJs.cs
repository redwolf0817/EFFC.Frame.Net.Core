/*
 * Install notice:
 * Please read article from https://github.com/pauldotknopf/vroomjs-core
 * Used for EFFC.Frame 3.5
 * Added by chuan.yin in 2017/3/21
 */
using Frame.Net.Base.Exceptions;
using Frame.Net.Base.ResouceManage.JsEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VroomJs;

namespace EFFC.Frame.Net.Base.ResouceManage.JsEngine
{
    /// <summary>
    /// 采用VroomJs作为HostJs引擎内核
    /// </summary>
    public class VRoomJs : HostJs
    {
        VroomJs.JsEngine js = null;
        JsContext currentContext = null;
        List<JsContext> lContext = new List<JsContext>();

        public VRoomJs()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                VroomJs.AssemblyLoader.EnsureLoaded(); // windows only
            }
            js = new VroomJs.JsEngine();
        }

        /// <summary>
        /// 運行腳本,每执行一次会新增一个JsContext
        /// </summary>
        /// <param name="script">string</param>
        /// <param name="kvp"></param>
        public override void Evaluate(string script, params KeyValuePair<string, object>[] kvp)
        {
            Console.WriteLine("Evaluate before");
            currentContext = js.CreateContext();
            Console.WriteLine("CreateContext after");
            lContext.Add(currentContext);
            Console.WriteLine("lContext add after");
            foreach (KeyValuePair<string, object> e in kvp)
            {
                currentContext.SetVariable(e.Key, e.Value);
            }
            Console.WriteLine("SetVariable after");
            try
            {
                currentContext.Execute(script);
                Console.WriteLine("Execute after");
            }
            catch (JsException je)
            {
                throw new HostJsException(je.Message, script, je.Line, je.Column);
            }

        }

        /// <summary>
        /// 獲取輸出參數
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object GetOutObject(string key)
        {
            return Convert2Object(currentContext, currentContext.GetVariable(key));
        }

        private object Convert2Object(JsContext context, object obj)
        {
            if (obj is JsObject)
            {
                var dic = new Dictionary<string, object>();
                var jo = (JsObject)obj;
                foreach (var item in context.GetMemberNames(jo))
                {
                    var v = context.GetPropertyValue(jo, item);
                    dic.Add(item, Convert2Object(context, v));
                }

                return dic;
            }
            else if (obj is DateTime)
            {
                return ((DateTime)obj).ToLocalTime();
            }
            else if(obj is JsFunction)
            {
                var jf = (JsFunction)obj;
                return jf.ToString();
            }
            else
            {
                return obj;
            }
        }
    
        /// <summary>
        /// 资源释放
        /// </summary>
        public override void Release()
        {
            foreach (var item in lContext)
            {
                if (item != null && !item.IsDisposed)
                {
                    item.Dispose();
                }
            }
            if (js != null && !js.IsDisposed)
            {
                js.Dispose();
            }
        }
        /// <summary>
        /// 判定资源是否已经释放
        /// </summary>
        public override bool IsDisposed()
        {
            if (js != null)
                return js.IsDisposed;
            else
                return true;
        }
    }
}
