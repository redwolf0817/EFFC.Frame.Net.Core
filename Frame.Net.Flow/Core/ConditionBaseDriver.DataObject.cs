using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Interfaces.Unit;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Business.Unit;
using EFFC.Frame.Net.Business.Unit.DOD;
using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.Parameters.Flow;
using EFFC.Frame.Net.Data.UnitData;
using EFFC.Frame.Net.Flow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Flow.Core
{
    public partial class ConditionBaseDriver:IConditionDriver<FlowParameter,FlowData>
    {
        private DOHelper _dod;
        /// <summary>
        /// db操作相关
        /// </summary>
        public virtual DOHelper DOD
        {
            get
            {
                if (_dod == null)
                    _dod = new DOHelper(this);

                return _dod;
            }
        }

        public class DOHelper
        {
            ConditionBaseDriver _driver;
            public DOHelper(ConditionBaseDriver driver)
            {
                _driver = driver;
            }
            public DOCollection DO<T>(string propertyname,params KeyValuePair<string,object>[] param) where T : DODBaseUnit
            {
                DODParameter pp = new DODParameter();
                pp.PropertyName = propertyname;
                foreach (var val in _driver._p.Domain(DomainKey.INPUT_PARAMETER))
                {
                    pp.SetValue(val.Key, val.Value);
                }
                foreach (var val in _driver._p.Domain(DomainKey.CONFIG))
                {
                    pp.SetValue(DomainKey.CONFIG, val.Key, val.Value);
                }
                if (param != null)
                {
                    foreach (var val in param)
                    {
                        pp.SetValue(val.Key, val.Value);
                    }
                }
                pp.FlowInstanceID = _driver._p.FlowInstanceID;
                pp.SetValue(ParameterKey.RESOURCE_MANAGER, _driver._p.Resources);
                pp.SetValue(ParameterKey.TOKEN, _driver._p.CurrentTransToken);
                var result = (DOCollection)UnitProxy<DODParameter>.Call<T>(pp);

                return result;
            }
            
        }
    }
}
