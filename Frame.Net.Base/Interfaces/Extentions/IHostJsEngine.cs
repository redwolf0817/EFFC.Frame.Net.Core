﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Frame.Net.Base.Interfaces.Extentions
{
    /// <summary>
    /// HostJs接口定义
    /// </summary>
    interface IHostJsEngine
    {
        void Evaluate(string script, params KeyValuePair<string, object>[] kvp);
        object GetOutObject(string key);
    }
}