using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using EFFC.Frame.Net.Base.ResouceManage.JsEngine;
using Frame.Net.Base.Interfaces.DataConvert;
using Frame.Net.Base.ResouceManage.JsEngine;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace EFFC.Frame.Net.Data.LogicData
{
    public partial class LogicData:DataCollection
    {
        public LogicData()
        {
        }
        /// <summary>
        /// Websocket传送的文字数据
        /// </summary>
        public object WebSocketRecieveData
        {
            get;
            set;
        }

        public override object Clone()
        {
            return this.Clone<LogicData>();
        }
    }
}
