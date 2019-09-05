using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using EFFC.Frame.Net.Resource.SQLServer;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Unit.DB;
using System.IO;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Common;

namespace RestAPISample.Business.v1._0
{
    public class Employment:MyRestLogic
    {
        [EWRARoute("get", "/employment/jylget")]
        [EWRAAddInput("NF", "string", "年份", "默认当前时间年份", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("ZY", "string", "专业名称", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRARouteDesc("学生就业率统计")]

        [EWRAOutputDesc("返回结果", @"{
result:[{
    major_no（专业ID）:'',
    major_name（专业名称）:'',
    JYXScount(就业人数): '[
                {
                   
                    'count': ''
                }
            ]'
   Allcount（所有学生人数）: '[
                {
                   
                    'count': ''
                }
            ]'
}]
}")]

        public object JYLget()
        {
            SetCacheEnable(false);

            var NF = DateTime.Now.Year.ToString();

            var ZY = "";
            var up = DB.NewDBUnitParameter();
            if (QueryString["NF"] != null && !string.IsNullOrEmpty(QueryString["NF"].ToString()))
            {
                NF = QueryString["NF"].ToString();
            }

            var s = from c in DB.LamdaTable(up, "ZZXS1102")
                    select new
                    {

                        c.BDRQ,
                        c.BDZH,


                    };
            var list = s.GetQueryList(up);


            return list;



        }


        [EWRARoute("get", "/employment/regionget")]
        [EWRARouteDesc("区域就业分析")]
        [EWRAAddInput("NF", "string", "年份", "默认当前时间年份", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("ZY", "string", "专业名称", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAOutputDesc("返回结果", @"{
result:[{
         
        HB: ,
        SW: 
}]
}")]
        public object regionget()
        {
            SetCacheEnable(false);
            var NF = DateTime.Now.Year.ToString();
            if (QueryString["NF"] != null && !string.IsNullOrEmpty(QueryString["NF"].ToString()))
            {
                NF = QueryString["NF"].ToString();
            }
            var ZY = "";

            var up = DB.NewDBUnitParameter();


            var list = (from t in DB.LamdaTable(up, "ZZXS1102")

                        select new
                        {
                            t.XM,

                        }).GetQueryList(up);




            var HB = (from c in list

                      select c).Count();
            var SW = (from c in list

                      select c).Count();

            return new { HB, SW };
        }

        [EWRARoute("get", "/Employment/Whereaboutsget")]
        [EWRARouteDesc("学生毕业去向分析")]
        [EWRAAddInput("NF", "string", "年份", "默认当前时间年份", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("ZY", "string", "专业", "无默认值，不传则显示所有专业", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]

        [EWRAOutputDesc("返回结果", @"{
result:[{
 BYQX（毕业去向）: 0,
count（人数）: 0
}]
}")]
        public object Whereaboutsget()
        {
            SetCacheEnable(false);
            var NF = DateTime.Now.Year.ToString();
            var ZY = "";
            if (QueryString["NF"] != null && !string.IsNullOrEmpty(QueryString["NF"].ToString()))
            {
                NF = QueryString["NF"].ToString();
            }

            var up = DB.NewDBUnitParameter();


            var list = (from t in DB.LamdaTable(up, "ZZXS1102")
                        select new
                        {
                            t.BDRQ

                        }).GetQueryList(up);


            var rtn = (from t in list
                       select new
                       {
                           t.BYQX,
                           count = t.Count()

                       }).ToList();
            return rtn;
        }



        [EWRARoute("get", "/Employment/salaryget")]
        [EWRARouteDesc("学生就业薪资分析")]
        [EWRAAddInput("NF", "string", "年份", "默认当前时间年份", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("ZY", "string", "专业", "无默认值，不传则显示所有专业", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]

        [EWRAOutputDesc("返回结果", @"{
result:[{
Three: 1,
Four:1, 
Five:1, 
Fivethousand:1,
six:1, 
sixthousand:1
}]
}")]
        public object salaryget()
        {
            SetCacheEnable(false);
            var NF = DateTime.Now.Year.ToString();
            var ZY = "";
            if (QueryString["NF"] != null && !string.IsNullOrEmpty(QueryString["NF"].ToString()))
            {
                NF = QueryString["NF"].ToString();
            }

            var up = DB.NewDBUnitParameter();


            var list = (from t in DB.LamdaTable(up, "ZZXS1102")

                        select new
                        {
                            t.BDRQ,


                        }).GetQueryList(up);



            var Three = (from c in list

                         select c).Count();

            var Four = (from c in list

                        select c).Count();

            var Five = (from c in list

                        select c).Count();


            var Fivethousand = (from c in list

                                select c).Count();


            var six = (from c in list

                       select c).Count();

            var sixthousand = (from c in list
                               where int.Parse(c.JYXZ) > 6000
                               select c).Count();


            return new { Three, Four, Five, Fivethousand, six, sixthousand };
        }
    }
}
