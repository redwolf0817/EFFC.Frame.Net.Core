using System;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Business.Logic
{
    public partial class WebBaseLogic<P, D>
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
            WebBaseLogic<P, D> _logic;
            public SessionHelper(WebBaseLogic<P, D> logic)
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
                if (!_logic.CallContext_Parameter.ForbiddenName.Contains(key))
                {
                    _logic.CallContext_Parameter[DomainKey.SESSION, key] = value;
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
                return _logic.CallContext_Parameter[DomainKey.SESSION, key];
            }
            /// <summary>
            /// 移除session数据
            /// </summary>
            /// <param name="key"></param>
            public void RemoveSessionValue(string key)
            {
                if (!_logic.CallContext_Parameter.ForbiddenName.Contains(key))
                {
                    _logic.CallContext_Parameter.Remove(DomainKey.SESSION, key);
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
                _logic.CallContext_Parameter.IsNeedSessionAbandon = true;
            }
            /// <summary>
            /// 获取本次Session的ID
            /// </summary>
            public string SessionID
            {
                get
                {
                    return _logic.CallContext_Parameter.SessionID;
                }
            }
        }
    }
}
