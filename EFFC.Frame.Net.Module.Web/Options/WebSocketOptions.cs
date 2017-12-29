using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Web.Options
{
    /// <summary>
    /// Websocket模块调用时全局设置参数
    /// </summary>
    internal class WebSocketOptions:WebOptions
    {
        public WebSocketOptions():base()
        {
            WebSocket_Max_Connection_Live_Minute = 10;
        }
        /// <summary>
        /// 长时间无交互时，连接存活的最大时间(分钟),默认10分钟
        /// </summary>
        public int WebSocket_Max_Connection_Live_Minute
        {
            get;
            set;
        }
    }
}
