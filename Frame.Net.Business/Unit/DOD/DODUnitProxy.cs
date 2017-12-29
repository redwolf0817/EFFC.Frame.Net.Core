using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Unit.DOD
{
    /// <summary>
    /// DODUnit呼叫Proxy
    /// </summary>
    public class DODUnitProxy
    {
        private static object lockobj = new object();
        public static DODBaseUnit GetDOD(string unitname, DODParameter p)
        {
            Dictionary<string, Type> d = null;
            lock (lockobj)
            {
                if (GlobalCommon.ApplicationCache.Get("__dod_unit_dic__") != null)
                {
                    d = (Dictionary<string, Type>)GlobalCommon.ApplicationCache.Get("__dod_unit_dic__");
                }
                else
                {
                    d = new Dictionary<string, Type>();
                    GlobalCommon.ApplicationCache.Set("__dod_unit_dic__", d, TimeSpan.FromHours(1));
                }
                var key = unitname.ToLower().Replace("dod", "").Replace("dodunit", "").Replace("dounit", "").Replace("unit", "");
                DODBaseUnit rtn = null;
                if (d.ContainsKey(key))
                {
                    var ctor = d[key].GetConstructor(new Type[] { p.GetType() });
                    if (ctor != null)
                    {
                        rtn = (DODBaseUnit)Activator.CreateInstance(d[key], p);
                    }
                    else
                    {
                        rtn = (DODBaseUnit)Activator.CreateInstance(d[key]);
                        var prop = d[key].BaseType.GetField("_p", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.NonPublic);
                        prop.SetValue(rtn, p);
                    }
                }
                else
                {
                    var ass = Assembly.Load(GlobalCommon.UnitCommon.UnitAssemblyPath);

                    foreach (var t in ass.GetTypes())
                    {
                        if (t.IsSubclassOf(typeof(DODBaseUnit)) && !t.IsAbstract && !t.IsInterface)
                        {
                            var tkey = t.Name.ToLower().Replace("dod", "").Replace("dodunit", "").Replace("dounit", "").Replace("unit", "");
                            d.Add(tkey, t);
                            if (tkey == key)
                            {
                                var ctor = d[key].GetConstructor(new Type[] { p.GetType() });
                                if (ctor != null)
                                {
                                    rtn = (DODBaseUnit)Activator.CreateInstance(d[key], p);
                                }
                                else
                                {
                                    rtn = (DODBaseUnit)Activator.CreateInstance(d[key]);
                                    var prop = d[key].BaseType.GetField("_p", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.NonPublic);
                                    prop.SetValue(rtn, p);
                                }
                            }
                        }
                    }


                }

                if (rtn != null)
                {
                    return rtn;
                }
                else
                {
                    throw new NotFoundException(string.Format("Can't find DODUnit named {0}", unitname));
                }
            }
        }
    }
}
