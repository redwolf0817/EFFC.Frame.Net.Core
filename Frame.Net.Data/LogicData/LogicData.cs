using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Data.LogicData
{
    public partial class LogicData:DataCollection
    {
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
