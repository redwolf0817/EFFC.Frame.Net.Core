using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Data.DataConvert;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace EFFC.Frame.Net.Business.Unit.DOD
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
