using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Json;
using System.Text;
using System.Threading.Tasks;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using EFFC.Frame.Net.Data.UnitData;

namespace EFFC.Frame.Net.Data.DataConvert
{
    /// <summary>
    /// 将UnitDataCollection中的涉及到翻页的属性转化成Json
    /// </summary>
    public class QueryByPage2Json : IDataConvert<string>
    {
        public string ConvertTo(object obj)
        {
            if (obj == null)
                return "";

            string rtn = "";
            UnitDataCollection udc = null;
            if (obj is UnitDataCollection)
            {
                udc = (UnitDataCollection)obj;
            }
            else
            {
                throw new Exception("QueryByPage2Json无法转化" + obj.GetType().FullName + "类型数据!");
            }

            if (udc.QueryTable != null)
            {
                JsonObjectCollection jsonrtn = new JsonObjectCollection();
                //jsonrtn.Add(new JsonStringValue("Count_Of_OnePage", udc.Count_Of_OnePage + ""));
                jsonrtn.Add(new JsonStringValue("page", udc.CurrentPage + ""));
                //jsonrtn.Add(new JsonStringValue("total", udc.TotalPage + ""));
                jsonrtn.Add(new JsonStringValue("total", udc.TotalRow + ""));

                DataTableStd dts = udc.QueryTable;
                JsonArrayCollection jsonobj = new JsonArrayCollection("rows");
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

                

                rtn = jsonrtn.ToString();
            }

            return rtn;
        }
    }
}
