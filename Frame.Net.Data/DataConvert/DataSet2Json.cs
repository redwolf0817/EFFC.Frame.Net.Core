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
    public class DataSet2Json:IDataConvert<string>
    {

        public string ConvertTo(object obj)
        {
            string rtn = "";

            if (obj == null)
                return "";

            DataSet ds = null;
            if (obj is DataSet)
            {
                ds = (DataSetStd)obj;
            }
            else
            {
                throw new Exception("DataSet2Json无法转化" + obj.GetType().FullName + "类型数据!");
            }

            JsonObjectCollection jsonrtn = new JsonObjectCollection();
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTableStd dts = DataTableStd.ParseStd(ds.Tables[i]);
                JsonArrayCollection jsonobj = new JsonArrayCollection("TableData" + i);
                for (int j = 0; j < dts.RowLength; j++)
                {
                    JsonObjectCollection jac = new JsonObjectCollection();
                    foreach (string colname in dts.ColumnNames)
                    {
                        if (dts.ColumnDateType(colname).FullName == typeof(DateTime).FullName)
                        {
                            DateTimeStd dtime = DateTimeStd.ParseStd(dts[j, colname]);
                            jac.Add(new JsonStringValue(colname, dtime != null ? dtime.Value.ToString("yyyy/MM/dd HH:mm:ss") : ""));
                        }
                        else
                        {
                            jac.Add(new JsonStringValue(colname, ComFunc.nvl(dts[j, colname])));
                        }
                    }
                    jsonobj.Add(jac);
                }
                jsonrtn.Add(jsonobj);
            }

            rtn = "{" + jsonrtn.ToString() + "}";
            return rtn;
        }
    }
}
