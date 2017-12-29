using EFFC.Frame.Net.Base.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Unit.DataObjectDefine.Unit
{
    public partial class DODBaseUnit
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
            DODBaseUnit _unit;
            public ConfigHelper(DODBaseUnit logic)
            {
                _unit = logic;
            }

            public object this[string key]
            {
                get
                {
                    return _unit._p[DomainKey.CONFIG, key];
                }
            }
        }
    }
}
