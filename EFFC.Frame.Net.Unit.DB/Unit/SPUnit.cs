using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Unit.DB.Unit
{
    /// <summary>
    /// 存储过程操作Unit
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
