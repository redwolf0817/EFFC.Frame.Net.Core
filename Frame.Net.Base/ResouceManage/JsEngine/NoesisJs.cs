using EFFC.Frame.Net.Base.Interfaces.Core;
/*
 * Used for EFFC.Frame 1.0 & EFFC.Frame 2.5
 * Added by chuan.yin in 2014/11/13
 */
using Noesis.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.ResouceManage.JsEngine
{
    /// <summary>
    /// 宿主Js引擎
    /// </summary>
    public class NoesisJs : HostJs
    {
        JavascriptContext context = null;

        public NoesisJs()
        {
            context = new JavascriptContext();
        }

        /// <summary>
        /// 運行腳本
        /// </summary>
        /// <param name="script">string</param>
        /// <param name="kvp"></param>
        public override void Evaluate(string script, params KeyValuePair<string, object>[] kvp)
        {
            foreach (KeyValuePair<string, object> e in kvp)
            {
                context.SetParameter(e.Key, e.Value);
            }

            context.Run(script);

        }
        
        /// <summary>
        /// 獲取輸出參數
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object GetOutObject(string key)
        {
            return context.GetParameter(key);
        }
        /// <summary>
        /// 资源释放
        /// </summary>
        public override void Release()
        {
            if (context != null)
            {
                context.Dispose();
                context = null;
            }
        }
        /// <summary>
        /// 判定资源是否已经释放
        /// </summary>
        public override bool IsDisposed()
        {
            return context == null;
        }
    }
}
