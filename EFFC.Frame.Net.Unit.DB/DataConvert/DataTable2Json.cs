using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Unit.DB.DataConvert
{
    public class DataTable2Json : IDataConvert<string>
    {

        public string ConvertTo(object obj)
        {
            DataTableStd dtt = null;
            if (obj is DataTableStd)
            {
                dtt = (DataTableStd)obj;
            }
            else
            {
                throw new Exception("DataTable2Json无法转化" + obj.GetType().FullName + "类型数据!");
            }
            FrameDLRObject data = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            data.SetValue("rows", dtt.Rows);
           
            return data.ToJSONString();
        }
    }
}
