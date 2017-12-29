using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Interfaces.Unit;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Business.Unit;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Business.Logic
{
    public partial class BaseLogic<P, D>
    {
        private LogicCallHelper _lchelper;
        /// <summary>
        /// db操作相关
        /// </summary>
        public virtual LogicCallHelper LogicCall
        {
            get
            {
                if (_lchelper == null)
                    _lchelper = new LogicCallHelper(this);

                return _lchelper;
            }
        }

        public class LogicCallHelper
        {
            BaseLogic<P, D> _logic;
            private const string _sharename = "__logic_share_space__";

            public LogicCallHelper() { }
            public LogicCallHelper(BaseLogic<P, D> logic)
            {
                _logic = logic;
            }

            public P Parameter
            {
                get
                {
                    return _logic.CallContext_Parameter;
                }
            }
            public D Data
            {
                get
                {
                    return _logic.CallContext_DataCollection;
                }
            }
            /// <summary>
            /// 写入Logic间的共享数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void SetShareValue(string key, object value)
            {
                _logic.CallContext_DataCollection.SetValue(_sharename, key, value);
            }
            /// <summary>
            /// 获取Logic间的共享数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public object GetShareValue(string key, object value)
            {
                return _logic.CallContext_DataCollection.GetValue(_sharename, key);
            }
            /// <summary>
            /// 本地呼叫其他的logic
            /// </summary>
            /// <typeparam name="L"></typeparam>
            public virtual void CallLogic<L>(string action)
                where L:ILogic
            {
                var copyp = _logic.CallContext_Parameter.Clone<P>();
                if (copyp is ConsoleParameter)
                {
                    copyp.SetValue(ParameterKey.ACTION, action);
                }
                L l = (L)Activator.CreateInstance(typeof(L), true);
                l.process(copyp, _logic.CallContext_DataCollection);
            }

        }
    }
}
