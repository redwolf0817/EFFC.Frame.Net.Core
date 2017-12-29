using EFFC.Frame.Net.Base.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Logic
{
    namespace Config
    {

    }
    public partial class BaseLogic<P,D>
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
            BaseLogic<P, D> _logic;
            public ConfigHelper(BaseLogic<P, D> logic)
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
