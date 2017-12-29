using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;

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
            rtn = Json.Decode<List<E>>(jsonstr);

            return rtn;
        }
    }
}
