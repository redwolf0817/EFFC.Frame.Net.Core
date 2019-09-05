using EFFC.Frame.Net.Base.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Business.Logic
{
    public partial class BaseLogic<PType, DType>
    {
        private BlackListHelper _blacklist;
        /// <summary>
        /// 黑名单操作
        /// </summary>
        public virtual BlackListHelper BlackList
        {
            get
            {
                if (_blacklist == null)
                    _blacklist = new BlackListHelper(this);
                return _blacklist;
            }
        }
        /// <summary>
        /// 黑名单
        /// </summary>
        public class BlackListHelper
        {
            BaseLogic<PType, DType> _logic;
            public BlackListHelper(BaseLogic<PType, DType> logic)
            {
                _logic = logic;
            }

            public object this[string key]
            {
                get
                {
                    return _logic.CallContext_Parameter[DomainKey.BLACK_LIST, key];
                }
            }
        }
    }
}
