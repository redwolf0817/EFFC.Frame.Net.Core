using System;
using System.Collections.Generic;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using System.Reflection;
using EFFC.Frame.Net.Base.Data.Base;
using System.Linq;

namespace EFFC.Frame.Net.Data.DataConvert
{
    public class Json2List<E>:IDataConvert<List<E>>
    {
        public List<E> ConvertTo(object obj)
        {
            if (obj == null)
                return null;

            List<E> rtn = null;
            string jsonstr = "";
            
            if (obj is string)
            {
                jsonstr = ComFunc.nvl(obj);
            }
            else
            {
                throw new Exception("Json2List无法转化" + obj.GetType().FullName + "类型数据!");
            }
            rtn = new List<E>();
            var arr = FrameDLRObject.CreateArray(jsonstr);
            Type te = typeof(E);
            PropertyInfo[] pis = te.GetProperties();
            for (int i = 0; i < arr.Length; i++)
            {
                E e = Activator.CreateInstance<E>();
                var data = arr[i];
                if(obj is Dictionary<string, object>)
                {
                    var item = (Dictionary<string, object>)data;
                    if (e is FrameDLRObject)
                    {
                        var etemp = (FrameDLRObject)(object)e;
                        var cols = item.Keys;
                        foreach (var col in cols)
                        {
                            etemp.SetValue(col, item[col]);
                        }
                    }
                    else
                    {
                        foreach (PropertyInfo fi in pis)
                        {

                            string colname = fi.Name;
                            var cols = item.Keys.ToArray();
                            if (cols.Contains<string>(colname, new IgoreCase()))
                            {
                                if (fi.PropertyType.FullName == typeof(bool).FullName)
                                {
                                    fi.SetValue(e, item[colname] == null ? false : item[colname], null);
                                }
                                else if (fi.PropertyType.FullName == typeof(int).FullName
                                    || fi.PropertyType.FullName == typeof(float).FullName
                                    || fi.PropertyType.FullName == typeof(double).FullName
                                    || fi.PropertyType.FullName == typeof(decimal).FullName)
                                {
                                    fi.SetValue(e, item[colname] == null ? 0 : item[colname], null);
                                }
                                else if (fi.PropertyType.FullName == typeof(string).FullName)
                                {
                                    fi.SetValue(e, ComFunc.nvl(item[colname]));
                                }
                                else
                                {
                                    fi.SetValue(e, item[colname], null);
                                }
                            }
                        }
                    }
                }
                
                rtn.Add(e);
            }

            return rtn;
        }
        protected class IgoreCase : IEqualityComparer<string>
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
