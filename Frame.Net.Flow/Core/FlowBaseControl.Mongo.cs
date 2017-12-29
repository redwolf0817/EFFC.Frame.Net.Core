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
        private MongoAccess26 _mongo;
        /// <summary>
        /// db操作相关
        /// </summary>
        public virtual MongoAccess26 Mongo
        {
            get
            {
                if (_mongo == null)
                    _mongo = new MongoAccess26();

                return _mongo;
            }
        }
    }
}
