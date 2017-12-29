using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Common;

namespace EFFC.Frame.Net.Data.DataConvert
{
    public class DataTable2List<E>:IDataConvert<List<E>>
    {

        public List<E> ConvertTo(object obj)
        {
            if (obj == null)
                return null;

            List<E> rtn = new List<E>();

            DataTableStd dtt = null;
            if (obj is DataTable)
            {
                dtt = DataTableStd.ParseStd(obj);
            }
            else if (obj is DataTableStd)
            {
                dtt = (DataTableStd)obj;
            }
            else
            {
                throw new Exception("DataTable2List无法转化" + obj.GetType().FullName + "类型数据!");
            }

            Type te = typeof(E);
            PropertyInfo[] pis = te.GetProperties();
            for (int i = 0; i < dtt.RowLength; i++)
            {
                E e = Activator.CreateInstance<E>();
                
                if (e is FrameDLRObject)
                {
                    var etemp = (FrameDLRObject)(object)e;
                    string[] cols = dtt.ColumnNames;
                    foreach (var col in cols)
                    {
                        etemp.SetValue(col, dtt[i, col]);
                    }
                }
                else
                {
                    foreach (PropertyInfo fi in pis)
                    {

                        string colname = fi.Name;
                        string[] cols = dtt.ColumnNames;
                        if (cols.Contains<string>(colname, new IgoreCase()))
                        {
                            if (fi.PropertyType.FullName == typeof(bool).FullName)
                            {
                                fi.SetValue(e, dtt[i, colname] == null ? false : dtt[i, colname], null);
                            }
                            else if (fi.PropertyType.FullName == typeof(int).FullName
                                || fi.PropertyType.FullName == typeof(float).FullName
                                || fi.PropertyType.FullName == typeof(double).FullName
                                || fi.PropertyType.FullName == typeof(decimal).FullName)
                            {
                                fi.SetValue(e, dtt[i, colname] == null ? 0 : dtt[i, colname], null);
                            }
                            else if (fi.PropertyType.FullName == typeof(string).FullName)
                            {
                                fi.SetValue(e, ComFunc.nvl(dtt[i, colname]));
                            }
                            else
                            {
                                fi.SetValue(e, dtt[i, colname], null);
                            }
                        }
                    }
                }
                rtn.Add(e);
            }

            return rtn;
        }

        protected class IgoreCase:IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x.Equals(y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
