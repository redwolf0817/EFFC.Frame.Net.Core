using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Interfaces.Unit;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Base.Common;

namespace EFFC.Frame.Net.Business.Unit
{
    public class SPUnit<T> : IUnit where T : IDBUnit<UnitParameter>
    {
        public DataCollection DoOperate(ParameterStd p)
        {
            string flag = p.GetValue<string>("_unit_action_flag_");
            UnitParameter up = (UnitParameter)p;
            T t = (T)Activator.CreateInstance(typeof(T), true);
            var sqlobj = t.GetSqlFunc(flag)(up);
            if (!(sqlobj is FrameDLRObject))
            {
                throw new TypeRequiredException("需要的动态数据对象类型应该为FrameDLRObject");
            }
            IDBAccessInfo dba = up.Dao;
            DBOParameterCollection dbc = up.SPParameter;
            UnitDataCollection rtn = new UnitDataCollection();
            if (dba is ADBAccess)
            {
                bool isrturnds = sqlobj.isreturnds != null ? sqlobj.isreturnds : false;
                DBDataCollection dbrtn = ((ADBAccess)dba).ExcuteProcedure(ComFunc.nvl(sqlobj.spname), isrturnds, ref dbc);
                if (dbrtn.IsSuccess)
                {
                    foreach (string s in dbrtn.Keys)
                    {
                        if (dbrtn[s] is DataSetStd)
                        {
                            rtn.QueryDatas = dbrtn.ReturnDataSet;
                        }
                        else
                        {
                            rtn.SetValue(s, dbrtn[s]);
                        }
                    }

                }
            }
            return rtn;
        }
    }
}
