using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EFFC.Frame.Net.Unit.DB.DataConvert
{
    /// <summary>
    /// DataTable转化成list
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public class DataTable2List<E> : IDataConvert<List<E>>
    {

        public List<E> ConvertTo(object obj)
        {
            if (obj == null)
                return null;

            List<E> rtn = new List<E>();

            DataTableStd dtt = null;
            if (obj is DataTableStd)
            {
                dtt = (DataTableStd)obj;
            }
            else
            {
                throw new Exception("DataTable2List无法转化" + obj.GetType().FullName + "类型数据!");
            }

            Type te = typeof(E);
            if (te == typeof(FrameDLRObject)
                || te == typeof(object))
            {
                return dtt.Rows.Select(p => (E)(object)p).ToList();
            }
            else
            {
                PropertyInfo[] pis = te.GetProperties();
                foreach (var item in dtt.Rows)
                {
                    rtn.Add(item.ToModel<E>());
                }
            }
           
            return rtn;
        }
    }
}
