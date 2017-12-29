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
    /// 针对post数据进行是否为Datetime的校验,如果校验成功则将对应的数据转化成DateTime类型
    /// </summary>
    public class EWRADateTimeValidAttribute : EWRAValidAttribute
    {
        string[] fields = null;
        public EWRADateTimeValidAttribute(string validFieldNames):base("参数格式类型不对(DateTime)",validFieldNames)
        {
            if(!string.IsNullOrEmpty(validFieldNames))
            {
                fields = validFieldNames.Split(',');
            }
        }
        protected override bool DoValid(EWRAParameter ep, EWRAData ed)
        {
            if (fields == null) return true;
            else
            {
                foreach (var f in fields)
                {
                    var v = ComFunc.nvl(ep[DomainKey.POST_DATA, f]);
                    if (!DateTimeStd.IsDateTime(v)) return false;
                    else
                    {
                        ep[DomainKey.POST_DATA, f] = DateTimeStd.ParseStd(v).Value;
                    }
                }

                return true;
            }
        }
    }
}
