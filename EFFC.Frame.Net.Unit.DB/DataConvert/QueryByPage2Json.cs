using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using EFFC.Frame.Net.Unit.DB.Datas;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Unit.DB.DataConvert
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

            UnitDataCollection udc = null;
            if (obj is UnitDataCollection)
            {
                udc = (UnitDataCollection)obj;
            }
            else
            {
                throw new Exception("QueryByPage2Json无法转化" + obj.GetType().FullName + "类型数据!");
            }
            FrameDLRObject rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            if (udc.QueryTable != null)
            {
                rtn.SetValue("page", udc.CurrentPage + "");
                rtn.SetValue("total", udc.TotalRow + "");

                rtn.SetValue("rows", udc.QueryTable.Rows);
            }

            return rtn.ToJSONString();
        }
    }
}
