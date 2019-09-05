using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Constants;
using NPOI.SS.UserModel;
using System.IO;

namespace RestAPISample.Business.v1._0
{
    public class DReportData : MyRestLogic
    {
        [EWRARouteDesc("获取所有有效报表列表")]
        [EWRAOutputDesc("返回结果", @"{
result:[{
    ReportUID:'UID',
    ReportName:'报表名称'，
    ReportDesc:'报表描述'
}]
}")]
        public override List<object> get()
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var list = (from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE", "a")
                        where t.IsActive == 1
                        select new
                        {
                            t.ReportUID,
                            t.ReportName,
                            t.ReportDesc
                        }).GetQueryList(up);

            return list.ToList<object>();
        }

        [EWRARoute("get", "/dreportdata/{id}/query")]
        [EWRAAddInput("id", "string", "报表UID", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRARouteDesc("执行报表查询")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
report_name:'报表名称',
columns:[{
    column_name:'栏位名称',
    column_display_name:'表头显示名称'
}],
data:[{
    column_name1:'值',
    column_name2:'值',
    .....
    column_nameN:'值',
}]
}")]
        object QueryReport(string id)
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE", "a")
                    where t.ReportUID == id
                    select t;
            if (!s.IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "报表不存在"
                };
            }

            dynamic info = s.GetQueryList(up).First();
            string jsonexpress = ComFunc.nvl(info.QueryJSON);
            if (jsonexpress == "")
            {
                return new
                {
                    code = "failed",
                    msg = "报表未生成查询表达式，无法执行查询操作"
                };
            }
            var data = DB.Excute(up, jsonexpress).QueryData<FrameDLRObject>();
            var columns = (from t in DB.LamdaTable(up, "Extend_Report_Template_Columns", "a")
                           where t.ReportUID == id
                           select new
                           {
                               column_name = t.ShowColumnName,
                               column_display_name = t.ShowColumnDesc
                           });
            return new
            {
                code = "success",
                msg = "",
                report_name = info.ReportName,
                columns,
                data
            };
        }
        [EWRARoute("get", "/dreportdata/{id}/export")]
        [EWRAAddInput("id", "string", "报表UID", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRARouteDesc("报表导出")]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息',
filetype:'文件的content-type类型'
filename:'文件名称',
filelength:'文件长度',
file:'文件内容，采用base64加密'
}")]
        object ExportReport(string id)
        {
            SetCacheEnable(false);
            dynamic result = FrameDLRObject.CreateInstance(QueryReport(id));
            if (result.code != "success")
            {
                return result;
            }

            var columns = ((IEnumerable<object>)result.columns).Select(d => (FrameDLRObject)FrameDLRObject.CreateInstance(d)).ToList();
            var data = ((IEnumerable<object>)result.data).Select(d => (FrameDLRObject)FrameDLRObject.CreateInstance(d)).ToList();
            var file = GenerateExcel(columns, data);
            return new
            {
                code = "success",
                msg = "",
                filetype = ResponseHeader_ContentType.Map("xlsx"),
                filename = result.report_name,
                filelength = file.Length,
                file = ComFunc.Base64Code(file)
            };
        }

        /// <summary>
        /// 生成excel
        /// </summary>
        /// <param name="columns">栏位，格式为：{
        /// column_name:'栏位名称',
        /// column_display_name:'表头显示名称'
        /// }
        /// </param>
        /// <param name="data">数据，格式为：
        /// {
        ///column_name1:'值',
        ///column_name2:'值',
        /// .....
        ///column_nameN:'值',
        ///}
        /// </param>
        /// <returns></returns>
        public byte[] GenerateExcel(IEnumerable<FrameDLRObject> columns, IEnumerable<FrameDLRObject> data)
        {
            IWorkbook workbook = null;
            try
            {
                using (var ms = new MemoryStream())
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(ms);
                }

                ISheet sheet = workbook.GetSheetAt(0);
                var index = 0;
                //设置表头
                var header = sheet.CreateRow(index);
                var headerstyle = workbook.CreateCellStyle();
                headerstyle.Alignment = HorizontalAlignment.Center;
                headerstyle.BorderBottom = BorderStyle.Thin;
                headerstyle.BorderTop = BorderStyle.Thin;
                headerstyle.BorderLeft = BorderStyle.Thin;
                headerstyle.BorderRight = BorderStyle.Thin;
                headerstyle.FillBackgroundColor = IndexedColors.BlueGrey.Index;
                headerstyle.FillPattern = FillPattern.SolidForeground;
                var cellindex = 0;
                foreach (dynamic item in columns)
                {
                    var cell = header.CreateCell(cellindex);
                    cell.CellStyle = headerstyle;
                    cell.SetCellValue(item.column_display_name);
                    cellindex++;
                }
                cellindex = 0;
                var datastyle = workbook.CreateCellStyle();
                datastyle.Alignment = HorizontalAlignment.Center;
                datastyle.BorderBottom = BorderStyle.Thin;
                datastyle.BorderTop = BorderStyle.Thin;
                datastyle.BorderLeft = BorderStyle.Thin;
                datastyle.BorderRight = BorderStyle.Thin;
                foreach (var item in data)
                {
                    index++;
                    var row = sheet.CreateRow(index);

                    foreach (dynamic c in columns)
                    {
                        var cell = row.CreateCell(cellindex);
                        cell.CellStyle = datastyle;
                        var v = item.GetValue(c.column_name);
                        if (v == null)
                        {
                            cell.SetCellValue("");
                        }
                        else if (v is DateTime)
                        {
                            cell.SetCellValue(((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            cell.SetCellValue(v);
                        }
                    }
                    cellindex++;
                }

                byte[] buffer = new byte[1024];
                using (MemoryStream output = new MemoryStream())
                {

                    workbook.Write(output);
                    buffer = output.ToArray();
                }
                return buffer;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                }
            }
        }
    }
}
