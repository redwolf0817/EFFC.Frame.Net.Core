using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation
{
    /// <summary>
    /// 针对post数据进行是否为空的校验
    /// </summary>
    public class EWRAEmptyValidAttribute:EWRAValidAttribute
    {
        string[] fields = null;
        public EWRAEmptyValidAttribute(string validFieldNames):base("参数不许为空",validFieldNames)
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
                    if (v == "") return false;
                }

                return true;
            }
        }
    }
}
