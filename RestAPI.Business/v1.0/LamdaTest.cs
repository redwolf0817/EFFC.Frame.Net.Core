using EFFC.Frame.Net.Resource.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;

namespace RestAPI.Business.v1._0
{
    class LamdaTest:MyRestLogic
    {
        [EWRAAuth(false)]
        public override List<object> get()
        {
            SetCacheEnable(false);
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            var s = from t in DB.LamdaTable("FunctionInfo", "", EFFC.Frame.Net.Base.Constants.DBType.Sqlite)
                    select t;
            return DB.ExcuteLamda(up, s).QueryData<object>();
        }
    }
}
