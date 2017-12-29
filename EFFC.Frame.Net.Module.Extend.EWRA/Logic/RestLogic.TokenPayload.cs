using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Logic
{
    public partial class RestLogic
    {
        TokenPayload _pl;
        /// <summary>
        /// Token中payload的参数信息
        /// </summary>
        public TokenPayload TokenPayLoad
        {
            get
            {
                if(_pl == null)
                {
                    _pl = new TokenPayload(this);
                }
                return _pl;
            }
        }

        public class TokenPayload
        {
            RestLogic _logic = null;
            dynamic payload = null;
            public TokenPayload(RestLogic logic)
            {
                _logic = logic;
                payload = logic.CallContext_Parameter.AuthorizedTokenPayLoad;
            }
            /// <summary>
            /// 验证的ID号
            /// </summary>
            public string ID
            {
                get
                {
                    return ComFunc.nvl(payload.id);
                }
            }
            /// <summary>
            /// Token的超时时间设置
            /// </summary>
            public DateTime ExpireTime
            {
                get
                {
                    return payload.expire_time == null ? DateTime.MinValue : payload.expire_time;
                }
            }
            /// <summary>
            /// 根据key获取payload中的数据
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public object this[string key]
            {
                get
                {
                    return _logic.CallContext_Parameter.AuthorizedTokenPayLoad.GetValue(key);
                }
            }
        }
    }
}
