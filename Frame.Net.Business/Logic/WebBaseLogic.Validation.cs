using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
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
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;

namespace EFFC.Frame.Net.Business.Logic
{
    public partial class WebBaseLogic<P, D>
    {
        private ValidationHelper _valid = null;
        /// <summary>
        /// cache操作
        /// </summary>
        public virtual ValidationHelper Validation
        {
            get {
                if (_valid == null)
                {
                    _valid = new ValidationHelper(this);
                }
                return _valid; 
            }
        }

        public class ValidationHelper
        {
            WebBaseLogic<P, D> _logic = null;
            public ValidationHelper(WebBaseLogic<P, D> logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// 进行必输验证
            /// </summary>
            /// <param name="rule">json结构，key为传入参数名称，value为提示信息</param>
            /// <returns></returns>
            public virtual dynamic RequireValidation(FrameDLRObject rule)
            {
                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:''
}");
                foreach (var s in rule.Keys)
                {
                    var input = _logic.CallContext_Parameter[DomainKey.POST_DATA, s];
                    input = input == null ? _logic.CallContext_Parameter[DomainKey.QUERY_STRING, s] : input;
                    if (input == null)
                    {
                        rtn.issuccess = false;
                        rtn.msg = rule.GetValue(s);
                        return rtn;
                    }
                    else
                    {
                        if (input is string)
                        {
                            if (ComFunc.nvl(input) == "")
                            {
                                rtn.issuccess = false;
                                rtn.msg = rule.GetValue(s);
                                return rtn;
                            }
                        }
                    }
                }
                return rtn;
            }
            /// <summary>
            /// 进行必输验证
            /// </summary>
            /// <param name="json">key为传入参数名称，value为提示信息</param>
            /// <returns></returns>
            public dynamic RequireValidation(string json)
            {
                return RequireValidation(FrameDLRObject.CreateInstance(json));
            }
            /// <summary>
            /// 判定是否为日期类型
            /// </summary>
            /// <param name="rule">json结构，key为传入参数名称，value为提示信息</param>
            /// <returns></returns>
            public virtual dynamic DateTimeValidation(FrameDLRObject rule)
            {
                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:''
}");
                foreach (var s in rule.Keys)
                {
                    var input = _logic.CallContext_Parameter[DomainKey.POST_DATA, s];
                    input = input == null ? _logic.CallContext_Parameter[DomainKey.QUERY_STRING, s] : input;
                    if (!DateTimeStd.IsDateTime(input))
                    {
                        rtn.issuccess = false;
                        rtn.msg = rule.GetValue(s);
                        return rtn;
                    }
                }
                return rtn;
            }
            /// <summary>
            /// 判定是否为日期类型
            /// </summary>
            /// <param name="json">key为传入参数名称，value为提示信息</param>
            /// <returns></returns>
            public dynamic DateTimeValidation(string json)
            {
                return DateTimeValidation(FrameDLRObject.CreateInstance(json));
            }
            /// <summary>
            /// 判定是否为数字类型
            /// </summary>
            /// <param name="rule">key为传入参数名称，value为提示信息</param>
            /// <returns></returns>
            public virtual dynamic NumberValidation(FrameDLRObject rule)
            {
                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:''
}");
                foreach (var s in rule.Keys)
                {
                    var input = _logic.CallContext_Parameter[DomainKey.POST_DATA, s];
                    input = input == null ? _logic.CallContext_Parameter[DomainKey.QUERY_STRING, s] : input;
                    if (!DoubleStd.IsDouble(input))
                    {
                        rtn.issuccess = false;
                        rtn.msg = rule.GetValue(s);
                        return rtn;
                    }
                }
                return rtn;
            }
        }
    }
}
