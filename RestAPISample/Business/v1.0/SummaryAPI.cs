using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Common;
using System.Linq;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Unit.DB.Parameters;

namespace RestAPISample.Business.v1._0
{
    public class SummaryAPI : MyRestLogic
    {
        static object lockobj = new object();
        [EWRAAuth(false)]
        [EWRARoute("post", "/sync/summary/today/at9")]
        [EWRARouteDesc("汇总昨日17点到今日9点的数据")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息""
}")]
        object DoSyncSummaryPerDay9()
        {
            lock (lockobj)
            {
                DateTime start = DateTimeStd.ParseStd($"{DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")} 17:00:01").Value;
                DateTime end = DateTimeStd.ParseStd($"{DateTime.Now.ToString("yyyy-MM-dd")} 08:59:59").Value;
                var up = DB.NewDBUnitParameter();
                //BeginTrans();
                var msg = "";
                if (!DoSyncAPPSummaryPerDay(up, start, end, ref msg))
                {
                    return new
                    {
                        code = "failed",
                        msg
                    };
                }
                if (!DoSyncRetailSummaryPerDay(up, start, end, ref msg))
                {
                    return new
                    {
                        code = "failed",
                        msg
                    };
                }
                if (!DoSyncChannelSummaryPerDay(up, start, end, ref msg))
                {
                    return new
                    {
                        code = "failed",
                        msg
                    };
                }
                if (!DoSyncPlatSummaryPerDay(up, start, end, ref msg))
                {
                    return new
                    {
                        code = "failed",
                        msg
                    };
                }

                //CommitTrans();
                return new
                {
                    code = "success",
                    msg = "执行成功"
                };
            }
        }
        [EWRARoute("post", "/sync/summary/today/at17")]
        [EWRARouteDesc("汇总今日9点到今日17点的数据")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息""
}")]
        object DoSyncSummaryPerDay17()
        {
            lock (lockobj)
            {
                DateTime start = DateTimeStd.ParseStd($"{DateTime.Now.ToString("yyyy-MM-dd")} 09:00:00").Value;
                DateTime end = DateTimeStd.ParseStd($"{DateTime.Now.ToString("yyyy-MM-dd")} 17:00:00").Value;
                var up = DB.NewDBUnitParameter();
                //BeginTrans();
                var msg = "";
                if (!DoSyncAPPSummaryPerDay(up, start, end, ref msg))
                {
                    return new
                    {
                        issuccess = false,
                        msg
                    };
                }
                if (!DoSyncRetailSummaryPerDay(up, start, end, ref msg))
                {
                    return new
                    {
                        issuccess = false,
                        msg
                    };
                }
                if (!DoSyncChannelSummaryPerDay(up, start, end, ref msg))
                {
                    return new
                    {
                        issuccess = false,
                        msg
                    };
                }
                if (!DoSyncPlatSummaryPerDay(up, start, end, ref msg))
                {
                    return new
                    {
                        issuccess = false,
                        msg
                    };
                }

                //CommitTrans();
                return new
                {
                    code = "success",
                    msg = "执行成功"
                };
            }
        }
        /// <summary>
        /// 汇总APP的每日汇总报表，指定时间段
        /// </summary>
        /// <param name="up"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool DoSyncAPPSummaryPerDay(UnitParameter up, DateTime start, DateTime end, ref string msg)
        {
            //清除旧汇总资料
            (from t in DB.LamdaTable(up, "APPSummaryPerDay")
             where t.log_time >= start.ToString("yyyy-MM-dd HH:mm:ss") && t.log_time <= end.ToString("yyyy-MM-dd HH:mm:ss")
             select t).Delete(up);
            //获取前一日的汇入资料
            var limit = 10000;
            var is_end = false;
            var to = 1;

            var app_summarylist = (from t in DB.LamdaTable(up, "APPInfo", "a")
                                   select new
                                   {
                                       uid = "",
                                       app_uid = t.uid,
                                       log_time = end.ToString("yyyy-MM-dd HH:mm:ss"),
                                       hits = 0,
                                       register_count = 0,
                                       register_rate = 0,
                                       perfection_count = 0,
                                       perfection_rate = 0,
                                       volume_count = 0,
                                       volume_rate = 0,
                                       consume = 0,
                                       recharge = 0,
                                       consume_sum = 0,
                                       recharge_sum = 0,
                                       remain_amount = t.available_amount,
                                       reqirement_count = t.require_count,
                                       t.rebate_to_us_cps
                                   }).GetQueryList(up);
            //获取当日充值数据
            var s = from t in DB.LamdaTable(up, "APPCharge", "a")
                    where t.is_success == 1 && t.submit_time >= start.ToString("yyyy-MM-dd HH:mm:ss") && t.submit_time <= end.ToString("yyyy-MM-dd HH:mm:ss")
                    group new { t } by t.app_uid into g
                    select new
                    {
                        g.t.app_uid,
                        recharge_amount = g.t.amount.sum()
                    };
            var rechargeSumToday = s.GetQueryList(up);
            var rechargeSum = (from t in DB.LamdaTable(up, "APPCharge", "a")
                               where t.is_success == 1
                               group new { t } by t.app_uid into g
                               select new
                               {
                                   g.t.app_uid,
                                   recharge_amount = g.t.amount.sum()
                               }).GetQueryList(up);
            foreach (dynamic item in app_summarylist)
            {
                item.uid = Guid.NewGuid().ToString();
                dynamic rech = rechargeSumToday.Where(w => w.app_uid == item.app_uid).FirstOrDefault();
                if (rech != null)
                    item.recharge = DoubleStd.IsNotDoubleThen(rech.recharge_amount);
                dynamic rechsum = rechargeSum.Where(w => w.app_uid == item.app_uid).FirstOrDefault();
                if (rech != null)
                    item.recharge_sum = DoubleStd.IsNotDoubleThen(rech.recharge_amount);
            }
            var app_summary = app_summarylist.ToDictionary(k => ComFunc.nvl(k.GetValue("app_uid")), v => (dynamic)v);

            do
            {
                up.ToPage = to;
                up.Count_Of_OnePage = limit;
                var exchangeresult = (from t in DB.LamdaTable(up, "APPExchange", "a")
                                      join t2 in DB.LamdaTable(up, "APPInfo", "b").LeftJoin() on t.app_uid equals t2.uid
                                      where t.log_time >= start.ToString("yyyy-MM-dd HH:mm:ss") && t.log_time <= end.ToString("yyyy-MM-dd HH:mm:ss")
                                      select new
                                      {
                                          t.app_uid,
                                          t.log_time,
                                          t.mobile,
                                          t.is_deal,
                                          t.is_register,
                                          t.is_perfection,
                                          t2.require_count
                                      }).QueryByPage(up);
                var list = exchangeresult.QueryData<FrameDLRObject>();
                //计算app
                var hit_group = from t in list
                                group t by t.GetValue("app_uid") into g
                                select new
                                {
                                    app_uid = g.First().GetValue("app_uid"),
                                    require_count = g.First().GetValue("require_count"),
                                    hits = g.Count()
                                };
                var register_group = from t in list
                                     where t.is_register == 1
                                     group t by t.GetValue("app_uid") into g
                                     select new
                                     {
                                         app_uid = g.First().GetValue("app_uid"),
                                         register_count = g.Count()
                                     };
                var volume_group = from t in list
                                   where t.is_deal == 1
                                   group t by t.GetValue("app_uid") into g
                                   select new
                                   {
                                       app_uid = g.First().GetValue("app_uid"),
                                       volume_count = g.Count()
                                   };
                var perfection_group = from t in list
                                       where t.is_perfection == 1
                                       group t by t.GetValue("app_uid") into g
                                       select new
                                       {
                                           app_uid = g.First().GetValue("app_uid"),
                                           perfection_count = g.Count()
                                       };
                //点击量
                foreach (var item in hit_group)
                {
                    var key = ComFunc.nvl(item.app_uid);
                    if (!app_summary.ContainsKey(key))
                    {
                        GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"DoSyncSummaryPerDay:同步数据时发现未记录的APP_UID-{key}");
                        continue;
                    }
                    app_summary[key].hits = IntStd.IsNotIntThen(app_summary[key].hits) + IntStd.IsNotIntThen(item.hits);
                }
                //注册量
                foreach (var item in register_group)
                {
                    var key = ComFunc.nvl(item.app_uid);
                    if (!app_summary.ContainsKey(key))
                    {
                        continue;
                    }
                    app_summary[key].register_count = IntStd.IsNotIntThen(app_summary[key].register_count) + IntStd.IsNotIntThen(item.register_count);
                }
                //成单量
                foreach (var item in volume_group)
                {
                    var key = ComFunc.nvl(item.app_uid);
                    if (!app_summary.ContainsKey(key))
                    {
                        continue;
                    }
                    app_summary[key].volume_count = IntStd.IsNotIntThen(app_summary[key].volume_count) + IntStd.IsNotIntThen(item.volume_count);
                }
                //完善量
                foreach (var item in perfection_group)
                {
                    var key = ComFunc.nvl(item.app_uid);
                    if (!app_summary.ContainsKey(key))
                    {
                        continue;
                    }
                    app_summary[key].volume_count = IntStd.IsNotIntThen(app_summary[key].perfection_count) + IntStd.IsNotIntThen(item.perfection_count);
                }

                to++;
                is_end = exchangeresult.CurrentPage == exchangeresult.TotalPage;
            } while (!is_end);

            foreach (var item in app_summary.Values)
            {
                //计算完善率，成单率
                if (DoubleStd.IsNotDoubleThen(item.register_count) != 0)
                {
                    item.perfection_rate = DoubleStd.IsNotDoubleThen(item.perfection_count) / DoubleStd.IsNotDoubleThen(item.register_count);
                    item.volume_rate = DoubleStd.IsNotDoubleThen(item.volume_count) / DoubleStd.IsNotDoubleThen(item.register_count);
                    item.consume = IntStd.IsNotIntThen(item.volume_count) * DoubleStd.IsNotDoubleThen(item.rebate_to_us_cps);
                }
                DB.QuickInsert(up, "APPSummaryPerDay", new
                {
                    item.uid,
                    item.app_uid,
                    item.log_time,
                    item.hits,
                    item.register_count,
                    item.perfection_count,
                    item.perfection_rate,
                    item.volume_count,
                    item.volume_rate,
                    item.consume,
                    item.recharge,
                    item.consume_sum,
                    item.recharge_sum,
                    item.reqirement_count,
                    add_id = "admin",
                    add_ip = "",
                    add_name = "admin",
                    add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            return true;
        }
        public bool DoSyncRetailSummaryPerDay(UnitParameter up, DateTime start, DateTime end, ref string msg)
        {
            //清除旧汇总资料
            (from t in DB.LamdaTable(up, "RetailSummaryPerDay")
             where t.log_time >= start.ToString("yyyy-MM-dd HH:mm:ss") && t.log_time <= end.ToString("yyyy-MM-dd HH:mm:ss")
             select t).Delete(up);
            //获取前一日的汇入资料
            var limit = 10000;
            var is_end = false;
            var to = 1;
            var retailSummary = new Dictionary<string, dynamic>();
            do
            {
                up.ToPage = to;
                up.Count_Of_OnePage = limit;
                var exchangeresult = (from t in DB.LamdaTable(up, "APPExchange", "a")
                                      join t2 in DB.LamdaTable(up, "APPInfo", "b") on t.app_uid equals t2.uid
                                      join t3 in DB.LamdaTable(up, "UserInfo", "c") on t.mobile equals t3.mobile
                                      join t4 in DB.LamdaTable(up, "Retail", "d") on t3.recommend_uid equals t4.uid
                                      where t.log_time >= start.ToString("yyyy-MM-dd HH:mm:ss") && t.log_time <= end.ToString("yyyy-MM-dd HH:mm:ss")
                                      select new
                                      {
                                          t.app_uid,
                                          t.log_time,
                                          t.mobile,
                                          t.is_deal,
                                          t.is_register,
                                          t.is_perfection,
                                          retail_uid = t3.recommend_uid
                                      }).QueryByPage(up);
                var list = exchangeresult.QueryData<FrameDLRObject>();
                //计算app
                var hit_group = from t in list
                                group t by new { app_uid = t.GetValue("app_uid"), retail_uid = t.GetValue("retail_uid") } into g
                                select new
                                {
                                    app_uid = g.First().GetValue("app_uid"),
                                    retail_uid = g.First().GetValue("retail_uid"),
                                    require_count = g.First().GetValue("require_count"),
                                    hits = g.Count()
                                };
                var register_group = from t in list
                                     where t.is_register == 1
                                     group t by new { app_uid = t.GetValue("app_uid"), retail_uid = t.GetValue("retail_uid") } into g
                                     select new
                                     {
                                         app_uid = g.First().GetValue("app_uid"),
                                         retail_uid = g.First().GetValue("retail_uid"),
                                         register_count = g.Count()
                                     };
                var volume_group = from t in list
                                   where t.is_deal == 1
                                   group t by new { app_uid = t.GetValue("app_uid"), retail_uid = t.GetValue("retail_uid") } into g
                                   select new
                                   {
                                       app_uid = g.First().GetValue("app_uid"),
                                       retail_uid = g.First().GetValue("retail_uid"),
                                       volume_count = g.Count()
                                   };
                var perfection_group = from t in list
                                       where t.is_perfection == 1
                                       group t by new { app_uid = t.GetValue("app_uid"), retail_uid = t.GetValue("retail_uid") } into g
                                       select new
                                       {
                                           app_uid = g.First().GetValue("app_uid"),
                                           retail_uid = g.First().GetValue("retail_uid"),
                                           perfection_count = g.Count()
                                       };
                //点击量
                foreach (var item in hit_group)
                {
                    var key = ComFunc.nvl(item.app_uid) + ComFunc.nvl(item.retail_uid);
                    if (!retailSummary.ContainsKey(key))
                    {
                        var dobj = FrameDLRObject.CreateInstance();
                        dobj.uid = Guid.NewGuid().ToString();
                        dobj.retail_uid = item.retail_uid;
                        dobj.app_uid = item.app_uid;
                        dobj.log_time = end.ToString("yyyy-MM-dd");
                        dobj.add_id = "admin";
                        dobj.add_ip = "";
                        dobj.add_name = "admin";
                        dobj.add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        retailSummary.Add(key, dobj);
                    }
                    retailSummary[key].hits = IntStd.IsNotIntThen(retailSummary[key].hits) + IntStd.IsNotIntThen(item.hits);
                }
                //注册量
                foreach (var item in register_group)
                {
                    var key = ComFunc.nvl(item.app_uid) + ComFunc.nvl(item.retail_uid);
                    retailSummary[key].register_count = IntStd.IsNotIntThen(retailSummary[key].register_count) + IntStd.IsNotIntThen(item.register_count);
                }
                //成单量
                foreach (var item in volume_group)
                {
                    var key = ComFunc.nvl(item.app_uid) + ComFunc.nvl(item.retail_uid);
                    retailSummary[key].volume_count = IntStd.IsNotIntThen(retailSummary[key].volume_count) + IntStd.IsNotIntThen(item.volume_count);
                }
                //完善量
                foreach (var item in perfection_group)
                {
                    var key = ComFunc.nvl(item.app_uid) + ComFunc.nvl(item.retail_uid);
                    retailSummary[key].perfection_count = IntStd.IsNotIntThen(retailSummary[key].perfection_count) + IntStd.IsNotIntThen(item.perfection_count);
                }

                to++;
                is_end = exchangeresult.CurrentPage >= exchangeresult.TotalPage;
            } while (!is_end);
            //抓取分销商的提现记录
            var withdrew_dic = (from t in DB.LamdaTable(up, "RetailAccountLog", "a")
                                join t2 in DB.LamdaTable(up, "Retail", "b") on t.retail_uid equals t2.uid
                                where t.action == "提现" && t.log_time >= start.ToString("yyyy-MM-dd HH:mm:ss") && t.log_time <= end.ToString("yyyy-MM-dd HH:mm:ss")
                                group new { t, t2 } by new { t.retail_uid, t2.rebate, t2.royalty_category } into g
                                select new
                                {
                                    g.t.retail_uid,
                                    g.t2.rebate,
                                    g.t2.royalty_category,
                                    withdrew_sum = g.t.amount.sum()
                                }).GetQueryList(up).ToDictionary(k => ComFunc.nvl(k.GetValue("retail_uid")), v => (dynamic)v);
            foreach (var item in retailSummary.Values)
            {
                if(DoubleStd.IsNotDoubleThen(item.register_count) != 0)
                {
                    item.perfection_rate = DoubleStd.IsNotDoubleThen(item.perfection_count) / DoubleStd.IsNotDoubleThen(item.register_count);
                    item.volume_rate = DoubleStd.IsNotDoubleThen(item.volume_count) / DoubleStd.IsNotDoubleThen(item.register_count);
                }
                else
                {
                    item.perfection_rate = 0;
                    item.volume_rate = 0;
                }
                
                item.withdrew_sum = withdrew_dic.ContainsKey(ComFunc.nvl(item.retail_uid)) ? DoubleStd.IsNotDoubleThen(withdrew_dic[ComFunc.nvl(item.retail_uid)].withdrew_sum) : 0;
                item.income = 0;
                if (withdrew_dic.ContainsKey(ComFunc.nvl(item.retail_uid)))
                {
                    var dobj = withdrew_dic[ComFunc.nvl(item.retail_uid)];
                    switch (ComFunc.nvl(dobj.royalty_category))
                    {
                        case "cps":
                            item.income = DoubleStd.IsNotDoubleThen(item.volume_count) * DoubleStd.IsNotDoubleThen(dobj.rebate);
                            break;
                        case "cpa":
                            item.income = DoubleStd.IsNotDoubleThen(item.register_count) * DoubleStd.IsNotDoubleThen(dobj.rebate);
                            break;
                        default:
                            item.income = 0;
                            break;
                    }

                }

                DB.QuickInsert(up, "RetailSummaryPerDay", item);
            }

            return true;
        }
        public bool DoSyncChannelSummaryPerDay(UnitParameter up, DateTime start, DateTime end, ref string msg)
        {
            //清除旧汇总资料
            (from t in DB.LamdaTable(up, "ChannelSummaryPerDay")
             where t.log_date >= start.ToString("yyyy-MM-dd HH:mm:ss") && t.log_date <= end.ToString("yyyy-MM-dd HH:mm:ss")
             select t).Delete(up);
            //从分销商报表中统计数据
            var channelSummaryList = (from t in DB.LamdaTable(up, "RetailSummaryPerDay", "a")
                                      join t2 in DB.LamdaTable(up, "Retail", "b") on t.retail_uid equals t2.uid
                                      join t3 in DB.LamdaTable(up, "ChannelBusiness", "c") on t2.belong_channel_uid equals t3.uid
                                      where t.log_time >= start.ToString("yyyy-MM-dd HH:mm:ss") && t.log_time <= end.ToString("yyyy-MM-dd HH:mm:ss")
                                      group new { t, t2, t3 } by new { t.retail_uid, t2.royalty_category, t3.uid, t3.rebate_cps, t3.rebate_cpa } into g
                                      select new
                                      {
                                          channel_uid = g.t3.uid,
                                          retail_uid = g.t.retail_uid,
                                          log_date = end.ToString("yyyy-MM-dd HH:mm:ss"),
                                          hits = g.t.hits.sum(),
                                          volume_count = g.t.volume_count.sum(),
                                          volume_rate = 0,
                                          register_count = g.t.register_count.sum(),
                                          perfection_count = g.t.perfection_count.sum(),
                                          perfection_rate = 0,
                                          unit_price_cps = g.t3.rebate_cps,
                                          unit_price_cpa = g.t3.rebate_cpa,
                                          g.t2.royalty_category,
                                          income = 0
                                      }).GetQueryList(up);
            foreach (dynamic item in channelSummaryList)
            {
                if (DoubleStd.IsNotDoubleThen(item.register_count) != 0)
                {
                    item.perfection_rate = DoubleStd.IsNotDoubleThen(item.perfection_count) / DoubleStd.IsNotDoubleThen(item.register_count);
                    item.volume_rate = DoubleStd.IsNotDoubleThen(item.volume_count) / DoubleStd.IsNotDoubleThen(item.register_count);
                }

                switch (ComFunc.nvl(item.royalty_category))
                {
                    case "cps":
                        item.income = DoubleStd.IsNotDoubleThen(item.volume_count) * DoubleStd.IsNotDoubleThen(item.unit_price_cps);
                        break;
                    case "cpa":
                        item.income = DoubleStd.IsNotDoubleThen(item.register_count) * DoubleStd.IsNotDoubleThen(item.unit_price_cpa);
                        break;
                    default:
                        item.income = 0;
                        break;
                }

                DB.QuickInsert(up, "ChannelSummaryPerDay", new
                {
                    uid = Guid.NewGuid().ToString(),
                    item.channel_uid,
                    item.retail_uid,
                    item.log_date,
                    item.hits,
                    item.volume_count,
                    item.volume_rate,
                    item.register_count,
                    item.perfection_count,
                    item.perfection_rate,
                    item.unit_price_cps,
                    item.unit_price_cpa,
                    item.income,
                    add_id = "admin",
                    add_ip = "",
                    add_name = "admin",
                    add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }

            return true;
        }

        public bool DoSyncPlatSummaryPerDay(UnitParameter up, DateTime start, DateTime end, ref string msg)
        {
            //清除旧汇总资料
            (from t in DB.LamdaTable(up, "PlatSummaryPerDay")
             where t.log_date >= start.ToString("yyyy-MM-dd HH:mm:ss") && t.log_date <= end.ToString("yyyy-MM-dd HH:mm:ss")
             select t).Delete(up);

            var platSummaryList = (from t in DB.LamdaTable(up, "APPSummaryPerDay", "a")
                                   where t.log_time >= start.ToString("yyyy-MM-dd HH:mm:ss") && t.log_time <= end.ToString("yyyy-MM-dd HH:mm:ss")
                                   select new
                                   {
                                       log_date = end.ToString("yyyy-MM-dd HH:mm:ss"),
                                       hits = t.hits.sum(),
                                       register_count = t.register_count.sum(),
                                       perfection_count = t.perfection_count.sum(),
                                       volume_count = t.volume_count.sum(),
                                       consume_amount = t.consume.sum(),
                                       recharge_amount = t.recharge.sum(),
                                       consume_sum = t.consume_sum.sum(),
                                       recharge_sum = t.recharge_sum.sum()
                                   }).GetQueryList(up);
            var withdrewChannelSummaryTodayList = (from t in DB.LamdaTable(up, "ChannelWithDraw", "a")
                                                   where t.status == "audit" && t.submit_time >= start.ToString("yyyy-MM-dd HH:mm:ss") && t.submit_time <= end.ToString("yyyy-MM-dd HH:mm:ss")
                                                   select new
                                                   {
                                                       withdrew_sum = t.amount.sum()
                                                   }).GetQueryList(up);
            var withdrewChannelSummaryList = (from t in DB.LamdaTable(up, "ChannelWithDraw", "a")
                                              where t.status == "audit"
                                              select new
                                              {
                                                  withdrew_sum = t.amount.sum()
                                              }).GetQueryList(up);
            var withdrewRetailSummaryTodayList = (from t in DB.LamdaTable(up, "RetailWithDraw", "a")
                                                  where t.status == "audit" && t.submit_time >= start.ToString("yyyy-MM-dd HH:mm:ss") && t.submit_time <= end.ToString("yyyy-MM-dd HH:mm:ss")
                                                  select new
                                                  {
                                                      withdrew_sum = t.amount.sum()
                                                  }).GetQueryList(up);
            var withdrewRetailSummaryList = (from t in DB.LamdaTable(up, "RetailWithDraw", "a")
                                             where t.status == "audit"
                                             select new
                                             {
                                                 withdrew_sum = t.amount.sum()
                                             }).GetQueryList(up);
            var withdrew_today = 0.0;
            if (withdrewChannelSummaryTodayList.Count > 0)
            {
                withdrew_today = DoubleStd.IsNotDoubleThen(withdrewChannelSummaryTodayList.First().GetValue("withdrew_sum")) + withdrew_today;
            }
            if (withdrewRetailSummaryTodayList.Count > 0)
            {
                withdrew_today = DoubleStd.IsNotDoubleThen(withdrewRetailSummaryTodayList.First().GetValue("withdrew_sum")) + withdrew_today;
            }
            var withdrew_sum = 0.0;
            if (withdrewChannelSummaryList.Count > 0)
            {
                withdrew_sum = DoubleStd.IsNotDoubleThen(withdrewChannelSummaryList.First().GetValue("withdrew_sum")) + withdrew_today;
            }
            if (withdrewRetailSummaryList.Count > 0)
            {
                withdrew_sum = DoubleStd.IsNotDoubleThen(withdrewRetailSummaryList.First().GetValue("withdrew_sum")) + withdrew_today;
            }

            foreach (dynamic item in platSummaryList)
            {
                item.uid = Guid.NewGuid().ToString();
                if (DoubleStd.IsNotDoubleThen(item.register_count) != 0)
                {
                    item.perfection_rate = DoubleStd.IsNotDoubleThen(item.perfection_count) / DoubleStd.IsNotDoubleThen(item.register_count);
                    item.volume_rate = DoubleStd.IsNotDoubleThen(item.volume_count) / DoubleStd.IsNotDoubleThen(item.register_count);
                }
                item.withdrew_amount = withdrew_today;
                item.withdrew_sum = withdrew_sum;

                DB.QuickInsert(up, "PlatSummaryPerDay", new
                {
                    item.uid,
                    item.log_date,
                    item.hits,
                    item.register_count,
                    item.perfection_count,
                    item.perfection_rate,
                    item.volume_count,
                    item.volume_rate,
                    item.consume_amount,
                    item.recharge_amount,
                    item.consume_sum,
                    item.recharge_sum,
                    item.withdrew_amount,
                    item.withdrew_sum,
                    add_id = "admin",
                    add_ip = "",
                    add_name = "admin",
                    add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            return true;
        }
        [EWRAAuth(false)]
        [EWRARoute("get", "/reports/retail_plat/{report_uid}/users")]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRARouteDesc("分销商报表-获取明细-平台查看,报账使用")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
    log_time:'记录时间'，
    mobile:'手机号，脱敏',
    name:'姓名，脱敏',
    is_deal:'是否成交',
    is_register:'是否注册',
    is_perfection:'是否完善资料',
    deal_amount:'成交金额'
}]
}")]
        object ReportRetails_Users_Plat(string report_uid)
        {
            SetCacheEnable(false);
            string user_uid = ComFunc.nvl(TokenPayLoad["no"]);

            var up = DB.NewDBUnitParameter();
            var list = (from t in DB.LamdaTable(up, "RetailSummaryPerDay", "a")
                        where t.uid == report_uid
                        select t).GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "没有该报表的资料"
                };
            }
            dynamic info = list.First();
            var dt = DateTimeStd.IsDateTimeThen(info.log_time, "yyyy-MM-dd");
            var s = from t in DB.LamdaTable(up, "APPExchange", "a")
                    join t2 in DB.LamdaTable(up, "UserInfo", "b").LeftJoin() on t.mobile equals t2.mobile
                    where t2.recommend_uid == info.retail_uid && t.log_time >= $"{dt} 00:00:00" && t.log_time <= $"{dt} 23:59:59"
                    select new
                    {
                        t.log_time,
                        t.mobile,
                        t2.name,
                        t.is_deal,
                        t.is_register,
                        t.is_perfection,
                        t.deal_amount
                    };
            var result = s.QueryByPage(up, "log_time desc");
            var tdata = from t in result.QueryData<FrameDLRObject>()
                        select new
                        {
                            log_time = DateTimeStd.IsDateTimeThen(t.log_time, "yyyy-MM-dd HH:mm:ss"),
                            mobile = ComFunc.nvl(t.mobile),
                            name = ComFunc.nvl(t.name).Length > 0 ? ComFunc.AESDecrypt(ComFunc.nvl(t.name)) : "",
                            is_deal = BoolStd.IsNotBoolThen(t.is_deal),
                            is_register = BoolStd.IsNotBoolThen(t.is_register),
                            is_perfection = BoolStd.IsNotBoolThen(t.is_perfection),
                            t.deal_amount
                        };
            return new
            {
                code = "success",
                msg = "",
                total_count = result.TotalRow,
                page = result.CurrentPage,
                total_page = result.TotalPage,
                limit = result.Count_Of_OnePage,
                data = from t in tdata
                       select new
                       {
                           t.log_time,
                           mobile = ComFunc.nvl(t.mobile).Length > 3 ? $"{ ComFunc.nvl(t.mobile).Substring(0, 3)}***{ComFunc.nvl(t.mobile).Substring(ComFunc.nvl(t.mobile).Length - 2, 2)}" : "",
                           name = $"{(ComFunc.nvl(t.name).Length > 0 ? ComFunc.nvl(t.name).Substring(0, 1) : "")}***",
                           t.is_deal,
                           t.is_register,
                           t.is_perfection,
                           t.deal_amount
                       }

            };
        }
    }


}
