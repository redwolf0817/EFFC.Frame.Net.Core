using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using EFFC.Frame.Net.Base.Data;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation
{
    /// <summary>
    /// 针对post数据进行是否为double的校验,如果校验成功则将对应的数据转化成double类型
    /// </summary>
    public class EWRADoubleValidAttribute : EWRAValidAttribute
    {
        string[] fields = null;
        bool is_check_empty_error = false;
        bool is_convert = true;
        /// <summary>
        /// 进行double类型检查
        /// </summary>
        /// <param name="validFieldNames">待检核栏位的名称，多个之间用逗号分隔</param>
        /// <param name="isCheckEmpty">参数为空是否算为错误，true：则检查是否为空，为空算作检核不通过，false：则不坚持为空，为空算作检核通过</param>
        /// <param name="isConvert">参数检核通过后是否要做类型转化，true：要转化成DateTime类型，false：不转化</param>
        public EWRADoubleValidAttribute(string validFieldNames,bool isCheckEmpty= true, bool isConvert = true) :base("参数格式类型不对(Double)",validFieldNames)
        {
            if(!string.IsNullOrEmpty(validFieldNames))
            {
                fields = validFieldNames.Split(',');
            }
            is_check_empty_error = isCheckEmpty;
            is_convert = isConvert;
        }
        protected override bool DoValid(EWRAParameter ep, EWRAData ed)
        {
            if (fields == null) return true;
            else
            {
                foreach (var f in fields)
                {
                    var ispostdata = ep.ContainsKey(DomainKey.POST_DATA, f);
                    var isquerystring = ep.ContainsKey(DomainKey.QUERY_STRING, f);
                    var v = ComFunc.nvl(ep[DomainKey.POST_DATA, f]);
                    v = v == "" ? ComFunc.nvl(ep[DomainKey.QUERY_STRING, f]) : v;
                    if (is_check_empty_error)
                    {
                        if (!DoubleStd.IsDouble(v)) return false;
                        else
                        {
                            if (is_convert)
                            {
                                if (ispostdata)
                                    ep[DomainKey.POST_DATA, f] = double.Parse(v);
                                if (isquerystring)
                                    ep[DomainKey.QUERY_STRING, f] = double.Parse(v);
                            }
                        }
                    }
                    else
                    {
                        if (v != "" && !DoubleStd.IsDouble(v)) return false;
                        else
                        {
                            if (v != "")
                            {
                                if (is_convert)
                                {
                                    if (ispostdata)
                                        ep[DomainKey.POST_DATA, f] = double.Parse(v);
                                    if (isquerystring)
                                        ep[DomainKey.QUERY_STRING, f] = double.Parse(v);
                                } 
                            }
                        }
                    }
                }

                return true;
            }
        }
    }
}
