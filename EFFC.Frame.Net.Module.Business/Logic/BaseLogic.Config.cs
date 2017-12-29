using EFFC.Frame.Net.Base.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Business.Logic
{
    public partial class BaseLogic<PType, DType>
    {
        private ConfigHelper _config;
        /// <summary>
        /// Config操作
        /// </summary>
        public virtual ConfigHelper Configs
        {
            get
            {
                if (_config == null)
                    _config = new ConfigHelper(this);
                return _config;
            }
        }

        public class ConfigHelper
        {
            BaseLogic<PType, DType> _logic;
            public ConfigHelper(BaseLogic<PType, DType> logic)
            {
                _logic = logic;
            }

            public object this[string key]
            {
                get
                {
                    return _logic.CallContext_Parameter[DomainKey.CONFIG, key];
                }
            }
        }
    }
}
