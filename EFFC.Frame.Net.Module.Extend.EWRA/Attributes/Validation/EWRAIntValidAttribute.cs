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
    ///  针对post数据进行是否为int的校验,如果校验成功则将对应的数据转化成int类型
    /// </summary>
    public class EWRAIntValidAttribute:EWRAValidAttribute
    {
        string[] fields = null;
        public EWRAIntValidAttribute(string validFieldNames):base("参数格式类型不对(Int)",validFieldNames)
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
                    if (!IntStd.IsInt(v)) return false;
                    else
                    {
                        ep[DomainKey.POST_DATA, f] = double.Parse(v);
                    }
                }

                return true;
            }
        }
    }
}
