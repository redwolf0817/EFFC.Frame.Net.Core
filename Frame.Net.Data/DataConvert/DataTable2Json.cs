using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Json;
using System.Text;
using System.Threading.Tasks;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;

namespace EFFC.Frame.Net.Data.DataConvert
{
    public class DataTable2Json : IDataConvert<JsonCollection>
    {

        public JsonCollection ConvertTo(object obj)
        {

            if (obj == null)
                return new JsonObjectCollection();

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
                throw new Exception("DataTable2Json无法转化" + obj.GetType().FullName + "类型数据!");
            }

            JsonArrayCollection jsonobj = new JsonArrayCollection("rows");
            for (int i = 0; i < dtt.RowLength; i++)
            {
                JsonObjectCollection jac = new JsonObjectCollection();
                foreach (string colname in dtt.ColumnNames)
                {
                    if (dtt.ColumnDateType(colname).FullName == typeof(DateTime).FullName)
                    {
                        DateTimeStd dtime = DateTimeStd.ParseStd(dtt[i, colname]);
                        jac.Add(new JsonStringValue(colname, dtime != null ? dtime.Value.ToString("yyyy/MM/dd HH:mm:ss") : ""));
                    }
                    else
                    {
                        jac.Add(new JsonStringValue(colname, ComFunc.nvl(dtt[i, colname])));
                    }
                }
                jsonobj.Add(jac);
            }


            return jsonobj;
        }
    }
}
