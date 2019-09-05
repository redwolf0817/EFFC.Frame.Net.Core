using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Common;

namespace RestAPISample.Business.v1._0
{
    public class Students:MyRestLogic
    {
        [EWRARoute("get", "/student")]
        [EWRARouteDesc("根据登陆者的信息获取对应的学生资料")]
        object GetStudentList()
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var class_no = "";
            //如果是班主任则带出该班下的所有学生，否则带出所有的学生资料
            var s = from t in DB.LamdaTable("ZZXS0101", "a")
                    join t2 in DB.LamdaTable(up, "ZZXS0701", "d").LeftJoin() on t.bh equals t2.bh
                    join t3 in DB.LamdaTable(up, "ZZJX0202", "b").LeftJoin() on t2.SZBH equals t3.BM
                    join t4 in DB.LamdaTable(up, "ZZJX0101", "c").LeftJoin() on t3.ZYDM equals t4.ZYBH
                    join t5 in DB.LamdaTable(up, "COMMON_SEX", "t2").LeftJoin() on t.XBM equals t5.code
                    join t6 in DB.LamdaTable(up, "ZZXS05", "e").LeftJoin() on t.bh equals t6.BH
                    join t7 in DB.LamdaTable(up, "ZZXS0703", "f").LeftJoin() on t.bh equals t7.BH
                    where t.notnull(class_no, t2.SZBH == class_no)
                    select new
                    {
                        student_no = t.bh,
                        student_name = t.xm,
                        student_id_no = t.SFZJH,
                        student_sex = t5.value,
                        class_name = t3.XZBMC,
                        class_no = t3.BM,
                        major_no = t4.ZYBH,
                        major_name = t4.ZYMC,
                        root_no = t.bh.substring(6, 4),
                        ori_school_name = t6.YXXMC,
                        state = t7.ZCZKM
                    };
            var list = DB.ExcuteLamda(up, s).QueryData<FrameDLRObject>();
            return new
            {
                code = "success",
                msg = "",
                data = from t in list
                       select new
                       {
                           t.student_no,
                           t.student_name,
                           t.student_id_no,
                           t.student_sex,
                           t.class_name,
                           t.class_no,
                           t.major_no,
                           t.major_name,
                           t.root_no,
                           t.ori_school_name,
                           t.state,
                           state_name = (ComFunc.nvl(t.state) == "2" ? "报到" : (ComFunc.nvl(t.state) == "3" || ComFunc.nvl(t.state) == "" ? "未报到" : ""))
                       }
            };
        }
    }
}
