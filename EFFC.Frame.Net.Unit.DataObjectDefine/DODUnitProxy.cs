using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Unit.DataObjectDefine.Datas;
using EFFC.Frame.Net.Unit.DataObjectDefine.Parameters;
using EFFC.Frame.Net.Unit.DataObjectDefine.Unit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EFFC.Frame.Net.Unit.DataObjectDefine
{
    /// <summary>
    /// DODUnit呼叫Proxy
    /// </summary>
    public class DODUnitProxy: UnitProxy<DODParameter>
    {
        private static Dictionary<string, Type> d = null;
        private static object lockobj = new object();

        static DODUnitProxy()
        {

        }
        /// <summary>
        /// 呼叫DOD
        /// </summary>
        /// <typeparam name="TDODUnit"></typeparam>
        /// <param name="p"></param>
        /// <returns></returns>
        public static DOCollection CallDOD<TDODUnit>(DODParameter p)
            where TDODUnit:DODBaseUnit
        {
            return (DOCollection)Call<TDODUnit>(p);
        }
        /// <summary>
        /// 获取Unit实例
        /// </summary>
        /// <param name="unitname"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static DODBaseUnit GetDOD(string unitname, DODParameter p)
        {
            lock (lockobj)
            {
                
                if (d == null)
                {
                    d = new Dictionary<string, Type>();
                    var ass = Assembly.Load(new AssemblyName(p.UnitAssembly));

                    foreach (var t in ass.GetTypes())
                    {
                        if (t.GetTypeInfo().IsSubclassOf(typeof(DODBaseUnit)) && !t.GetTypeInfo().IsAbstract && !t.GetTypeInfo().IsInterface)
                        {
                            var tkey = t.Name.ToLower().Replace("dod", "").Replace("dodunit", "").Replace("dounit", "").Replace("unit", "");
                            d.Add(tkey, t);
                        }
                    }
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
                        var prop = d[key].GetTypeInfo().GetField("_p", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.IgnoreCase | BindingFlags.NonPublic);
                        prop.SetValue(rtn, p);
                    }
                }

                if (rtn != null)
                {
                    return rtn;
                }
                else
                {
                    throw new Exception(string.Format("Can't find DODUnit named {0}", unitname));
                }
            }
        }
    }
}
