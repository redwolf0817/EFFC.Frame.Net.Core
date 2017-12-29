using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Data;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Business.Logic
{
    public partial class WebBaseLogic<P, D>
    {
        private ApplicationHelper _application;
        /// <summary>
        /// Application存储器操作
        /// </summary>
        public virtual ApplicationHelper Application
        {
            get
            {
                if (_application == null)
                    _application = new ApplicationHelper(this);
                return _application;
            }
        }

        public class ApplicationHelper
        {
            WebBaseLogic<P, D> _logic;
            public ApplicationHelper(WebBaseLogic<P, D> logic)
            {
                _logic = logic;
            }

            /// <summary>
            /// 新增session数据
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void AddApplicationValue(string key, object value)
            {
                if (!_logic.CallContext_Parameter.ForbiddenName.Contains(key))
                {
                    _logic.CallContext_Parameter[DomainKey.APPLICATION, key] = value;
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
            public object GetApplicationValue(string key)
            {
                return _logic.CallContext_Parameter[DomainKey.APPLICATION, key];
            }
            /// <summary>
            /// 移除session数据
            /// </summary>
            /// <param name="key"></param>
            public void RemoveApplicationValue(string key)
            {
                if (!_logic.CallContext_Parameter.ForbiddenName.Contains(key))
                {
                    _logic.CallContext_Parameter.Remove(DomainKey.APPLICATION, key);
                }
                else
                {
                    throw new Exception("名称为" + key + "的数据为内定数据，不可被移除，请更换成其他的Key");
                }
            }
        }
    }
}
