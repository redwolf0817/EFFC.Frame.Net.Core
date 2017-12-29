using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.ResouceManage.JsEngine;
using Noesis.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Logic
{
    public partial class BaseLogic<P,D>
    {
        HostJsHelper _hjh = null;
        public HostJsHelper Js
        {
            get
            {
                if (_hjh == null) _hjh = new HostJsHelper(this);
                return _hjh;
            }


        }
        public class HostJsHelper
        {
            protected BaseLogic<P, D> _logic;

            public HostJsHelper(BaseLogic<P, D> logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// 呼叫js脚本
            /// </summary>
            /// <param name="scriptpath">脚本路径，根路径用~表示</param>
            /// <param name="includes">需要include进来的脚本，根路径用~表示</param>
            /// <param name="p"></param>
            /// <returns></returns>
            public object Call(string scriptstr, FrameDLRObject input, params KeyValuePair<string, object>[] others)
            {
                return Call(scriptstr, input, "output", others);
            }
            /// <summary>
            /// 呼叫js脚本
            /// </summary>
            /// <param name="scriptstr"></param>
            /// <param name="input"></param>
            /// <param name="outputkey"></param>
            /// <param name="others"></param>
            /// <returns></returns>
            public object Call(string scriptstr, FrameDLRObject input, string outputkey, params KeyValuePair<string, object>[] others)
            {
                try
                {
                    var jse = _logic.CallContext_ResourceManage.CreateInstance<HostJs>();
                    input = input == null ? FrameDLRObject.CreateInstance() : input;
                    var lp = others.ToList();
                    lp.Add(new KeyValuePair<string, object>("input", input.ToDictionary()));
                    jse.Evaluate(scriptstr, lp.ToArray());
                    var obj = jse.GetOutObject(outputkey);
                    if (obj is Dictionary<string, object>)
                        return FrameDLRObject.CreateInstance((Dictionary<string, object>)obj);
                    else
                        return obj;
                }
                catch (JavascriptException jex)
                {
                    var strmsg = new StringBuilder();
                    strmsg.AppendLine(jex.V8SourceLine);
                    strmsg.AppendLine(jex.V8StackTrace);
                    throw new Exception(strmsg.ToString(), jex);
                }
            }
        }
    }
}
