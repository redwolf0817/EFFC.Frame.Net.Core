using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Interfaces.Unit;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Business.Unit;
using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.Parameters.Flow;
using EFFC.Frame.Net.Data.UnitData;
using EFFC.Frame.Net.Flow.Exceptions;
using EFFC.Frame.Net.Flow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Flow.Core
{
    public partial class FlowBaseControl : BaseModule<FlowParameter, FlowData>
    {
        private ConfigHelper _config;
        /// <summary>
        /// db操作相关
        /// </summary>
        public virtual ConfigHelper Config
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
            FlowBaseControl _driver;

            public ConfigHelper() { }
            public ConfigHelper(FlowBaseControl driver)
            {
                _driver = driver;
            }

            public object this[string key]
            {
                get
                {
                    return _driver._p.GetValue(DomainKey.CONFIG, key);
                }
            }

        }
    }
}
