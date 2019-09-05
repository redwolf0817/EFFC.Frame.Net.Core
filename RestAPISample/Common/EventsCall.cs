using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.HttpCall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPISample.Common
{
    public class EventsCall : SimpleRestCallHelper
    {
        public EventsCall()
        {
        }

        /// <summary>
        /// 发送事件请求
        /// </summary>
        /// <param name="method">请求的方法</param>
        /// <param name="url">请求的url</param>
        /// <param name="context">请求的参数上下文</param>
        /// <param name="header">请求的header资料</param>
        /// <param name="postdata">请求的post资料</param>
        /// <returns></returns>
        public string Send(string method, string url, EventsCallContext context = null, FrameDLRObject header = null, FrameDLRObject postdata = null)
        {
            if (header != null)
            {
                foreach (var key in header.Keys)
                {
                    if (ComFunc.nvl(header.GetValue(key)).StartsWith("$") && context.ContainsKey(ComFunc.nvl(header.GetValue(key))))
                    {
                        header.SetValue(key, context[ComFunc.nvl(header.GetValue(key))]);
                    }
                }
            }
            if (postdata != null)
            {
                foreach (var key in postdata.Keys)
                {
                    if (ComFunc.nvl(postdata.GetValue(key)).StartsWith("$") && context.ContainsKey(ComFunc.nvl(postdata.GetValue(key))))
                    {
                        postdata.SetValue(key, context[ComFunc.nvl(postdata.GetValue(key))]);
                    }
                }
            }
            if (method.ToLower() == "get")
            {
                return Get(url, header);
            }
            else
            {
                return base.Send(url, postdata, header, method);
            }
        }

        public class EventsCallContext : DataCollection
        {
            public EventsCallContext()
            {
                this["$_now"] = DateTime.Now;
            }
            /// <summary>
            /// $_row标记的行资料
            /// </summary>
            public object RowData
            {
                get
                {
                    return this["$_row"];
                }
                set
                {
                    this["$_row"] = value;
                }
            }
            /// <summary>
            /// $_login_id标记的登陆者id
            /// </summary>
            public string Login_ID
            {
                get
                {
                    return ComFunc.nvl(this["$_login_id"]);
                }
                set
                {
                    this["$_login_id"] = value;
                }
            }
            /// <summary>
            /// $_login_name标记的登录者名称
            /// </summary>
            public string Login_Name
            {
                get
                {
                    return ComFunc.nvl(this["$_login_name"]);
                }
                set
                {
                    this["$_login_name"] = value;
                }
            }



        }
    }
}
