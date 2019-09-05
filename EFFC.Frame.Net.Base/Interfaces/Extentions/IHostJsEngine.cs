using System;
using System.Collections.Generic;
using System.Text;

namespace Frame.Net.Base.Interfaces.Extentions
{
    /// <summary>
    /// HostJs接口定义
    /// </summary>
    public interface IHostJsEngine
    {
        void Excute(string script, params KeyValuePair<string, object>[] kvp);
        object Evaluate(string script, params KeyValuePair<string, object>[] kvp);
        object GetOutObject(string key);
    }
}
