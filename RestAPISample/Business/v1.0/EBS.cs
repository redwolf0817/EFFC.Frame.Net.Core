using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;

namespace RestAPISample.Business.v1._0
{
    public class EBS:MyRestLogic
    {
        public override List<object> get()
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter4EBS();
            var s = from t in DB.LamdaTable(up, "APPS.CUX_B2B_BANK_ACCOUNT_V", "a")
                    where t.ou_id == "271"
                    select t;

            var result = DB.ExcuteLamda(up, s);
            return result.QueryData<object>();
        }
    }
}
