using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Test
{
    public class NPoiTest
    {
        public static void Test()
        {
            var filepath = "E:/test.xlsx";
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            using (var ms = new FileStream(filepath, FileMode.OpenOrCreate))
            {
                IWorkbook workbook = new XSSFWorkbook();
                try
                {

                    ISheet sheetRef = workbook.CreateSheet("ref");//名为ref的工作表
                    var items = new dynamic[] {
                        new { code = "1", name = "项目" },
                        new {code ="2",name ="标段" },
                        new {code = "3",name="桥梁" },
                        new {code = "4",name = "隧道" }
                    };
                    for (int i = 0; i < items.Length; i++)//A1到A4格子里存放0001到0004，这是下拉框可以选择的4个选项
                    {
                        var r = sheetRef.CreateRow(i);
                        r.CreateCell(0).SetCellValue(items[i].code);
                        r.CreateCell(1).SetCellValue(items[i].name);
                        //sheetRef.GetRow(i);
                    }
                    IName range = workbook.CreateName();//创建一个命名公式
                    range.RefersToFormula = "ref!$A$1:$A$"+items.Length;//公式内容，就是上面的区域
                    range.NameName = "sectionName";//公式名称，可以在"公式"-->"名称管理器"中看到

                    ISheet sheet1 = workbook.CreateSheet("data");//获得第一个工作表
                    IRow row = sheet1.CreateRow(0);
                    row.CreateCell(0).SetCellValue("项目名称");
                    row.CreateCell(1).SetCellValue("地图名称");
                    row.CreateCell(2).SetCellValue("地图类型-代码");
                    row.CreateCell(3).SetCellValue("地图类型-名称");
                    row.CreateCell(4).SetCellValue("经纬度");
                    //设定公式
                    row.GetCell(3).SetCellFormula("VLOOKUP(C2,ref!A:B,2,FALSE)");
                    CellRangeAddressList regions = new CellRangeAddressList(1, 65535, 2,3);//约束范围：B1到B65535
                    XSSFDataValidationHelper helper = new XSSFDataValidationHelper((XSSFSheet)sheet1);//获得一个数据验证Helper
                    IDataValidation validation = helper.CreateValidation(helper.CreateFormulaListConstraint("sectionName"), regions);//创建一个特定约束范围内的公式列表约束（即第一节里说的"自定义"方式）
                    validation.CreateErrorBox("错误", "请按右侧下拉箭头选择!");//不符合约束时的提示
                    validation.ShowErrorBox = true;//显示上面提示 = True
                    sheet1.AddValidationData(validation);//添加进去
                    sheet1.ForceFormulaRecalculation = true;

                    workbook.Write(ms);
                }
                finally
                {
                    workbook.Close();
                }
            }
            

            
        }
    }
}
