using EFFC.Frame.Net.Base.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.WebGo.Logic
{
    public abstract partial class GoLogic
    {
        private SessionHelper _session;
        /// <summary>
        /// cache操作
        /// </summary>
        public virtual SessionHelper Sessions
        {
            get
            {
                if (_session == null)
                    _session = new SessionHelper(this);
                return _session;
            }
        }

        public class SessionHelper
        {
            GoLogic _logic;
            public SessionHelper(GoLogic logic)
            {
                _logic = logic;
            }

            /// <summary>
            /// 新增session数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void AddSessionValue(string key, object value)
            {
                if (!_logic.CallContext_Parameter.WebParam.ForbiddenName.Contains(key))
                {
                    _logic.CallContext_Parameter.WebParam[DomainKey.SESSION, key] = value;
                }
                else
                {
                    throw new Exception("名称为" + key + "的数据为内定数据，不可被修改，请更换成其他的Key");
                }
            }
            /// <summary>
            /// 获取session数据
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public object GetSessionValue(string key)
            {
                return _logic.CallContext_Parameter.WebParam[DomainKey.SESSION, key];
            }
            /// <summary>
            /// 移除session数据
            /// </summary>
            /// <param name="key"></param>
            public void RemoveSessionValue(string key)
            {
                if (!_logic.CallContext_Parameter.WebParam.ForbiddenName.Contains(key))
                {
                    _logic.CallContext_Parameter.WebParam.Remove(DomainKey.SESSION, key);
                }
                else
                {
                    throw new Exception("名称为" + key + "的数据为内定数据，不可被移除，请更换成其他的Key");
                }
            }

            /// <summary>
            /// 取消Session
            /// </summary>
            public void SessionAbandon()
            {
                //var keys = CallContext_Parameter.Keys(WebParameter.ParameterDomain.Session);
                //foreach (var k in keys)
                //    CallContext_Parameter.Remove(WebParameter.ParameterDomain.Session, k);
                _logic.CallContext_Parameter.WebParam.IsNeedSessionAbandon = true;
            }
            /// <summary>
            /// 获取本次Session的ID
            /// </summary>
            public string SessionID
            {
                get
                {
                    return _logic.CallContext_Parameter.WebParam.SessionID;
                }
            }
        }
    }
}
