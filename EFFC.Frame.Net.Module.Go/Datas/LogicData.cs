using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.WebGo.Datas
{
    public partial class LogicData : DataCollection
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
            return this.DeepCopy<LogicData>();
        }

        /// <summary>
        /// 转到页号
        /// </summary>
        public int ToPage { get; set; }
        /// <summary>
        /// 当前页号
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 每页笔数
        /// </summary>
        public int Count_Per_Page { get; set; }
    }
}
