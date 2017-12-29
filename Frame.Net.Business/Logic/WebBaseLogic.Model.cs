using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EFFC.Frame.Net.Base.AttributeDefine;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Interfaces.Unit;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Business.Unit;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using EFFC.Frame.Net.Base.Constants;


namespace EFFC.Frame.Net.Business.Logic
{
    public partial class WebBaseLogic<P, D>
    {
        /// <summary>
        /// 从Querystring和postdata中获取数据，并生成对应的Model，效能较低，谨慎使用
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <returns></returns>
        public M GetModel<M>()
        {
            M rtn = (M)Activator.CreateInstance(typeof(M), true);
            Type te = typeof(M);
            PropertyInfo[] pis = te.GetProperties();
            
            foreach (PropertyInfo fi in pis)
            {
                foreach (var val in CallContext_Parameter.Domain(DomainKey.QUERY_STRING))
                {
                    string colname = fi.Name;
                    if (fi.Name.Equals(val.Key,StringComparison.OrdinalIgnoreCase))
                    {
                        SetPropertyValue<M>(ref rtn, fi,val.Value);
                    }
                }
                foreach (var val in CallContext_Parameter.Domain(DomainKey.POST_DATA))
                {
                    string colname = fi.Name;
                    if (fi.Name.Equals(val.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        SetPropertyValue<M>(ref rtn, fi, val.Value);
                    }
                }
            }

            return rtn;
        }

        private void SetPropertyValue<M>(ref M m, PropertyInfo fi, object value)
        {
            
            if (fi.PropertyType.FullName == typeof(DateTime).FullName)
            {
                fi.SetValue(m, DateTimeStd.ParseStd(value).Value, null);
            } else if(fi.GetCustomAttributes(typeof(ConvertorAttribute),false).Length > 0)
            {
                ConvertorAttribute ca = (ConvertorAttribute)Attribute.GetCustomAttribute(fi, typeof(ConvertorAttribute), false);
                fi.SetValue(m, ca.Convertor.ConvertTo(value), null);
            }
            else
            {
                fi.SetValue(m, value, null);
            }
           
        }
    }
}

