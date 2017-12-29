using System;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Constants;
using System.Collections.Generic;

namespace EFFC.Frame.Net.Data.DataConvert
{
    public class DataSet2Json:IDataConvert<string>
    {

        public string ConvertTo(object obj)
        {
            string rtn = "";

            if (obj == null)
                return "";

            DataSetStd ds = null;
            if (obj is DataSetStd)
            {
                ds = (DataSetStd)obj;
            }
            else
            {
                throw new Exception("DataSet2Json无法转化" + obj.GetType().FullName + "类型数据!");
            }
            FrameDLRObject data = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            
            for (int i = 0; i < ds.TableCount; i++)
            {
                var list = new List<FrameDLRObject>();
                DataTableStd dts = ds[i];
                data.SetValue("TableData" + i, list);
                for (int j = 0; j < dts.RowLength; j++)
                {
                    FrameDLRObject item = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                    foreach (string colname in dts.ColumnNames)
                    {
                        
                        item.SetValue(colname, dts[j, colname]);
                    }
                    list.Add(item);
                }
            }

            rtn = data.ToJSONString(); ;
            return rtn;
        }
    }
}
