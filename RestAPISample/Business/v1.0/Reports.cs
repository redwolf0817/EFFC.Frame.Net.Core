using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;

namespace RestAPISample.Business.v1._0
{
    public class Reports : MyRestLogic
    {
        [EWRARoute("get", "/reports/channel")]
        [EWRARouteDesc("渠道商报表")]
        [EWRAAddInput("start", "string", "起始日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("end", "string", "结束日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
    uid:'uid'，
    retail_name:'分销商名称',
    log_date:'记录日期',
    hits:'点击量',
    volume_count:'成交数',
    volume_rate:'成交率',
    register_count:'注册量',
    perfection_count:'用户资料完善数',
    perfection_rate:'完善率',
    income:'提成',
    draw_sum:'提现合计'
}]
}")]
        object ReportChannel()
        {
            SetCacheEnable(false);
            var start = DateTimeStd.IsDateTimeThen(QueryStringD.start, "yyyy-MM-dd HH:mm:ss");
            var end = DateTimeStd.IsDateTimeThen(QueryStringD.end, "yyyy-MM-dd HH:mm:ss");
            string user_uid = ComFunc.nvl(TokenPayLoad["no"]);
            string usertype = ComFunc.nvl(TokenPayLoad["usertype"]);
            if (!new string[] { "Channel" }.Contains(usertype))
            {
                return new
                {
                    code = "failed",
                    msg = "当前用户不可查看该报表"

                };
            }
            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "ChannelSummaryPerDay", "a")
                          join t2 in DB.LamdaTable(up, "Retail", "b") on t.retail_uid equals t2.uid
                          where t.channel_uid == user_uid && t.notnull(start, t.log_date >= start) && t.notnull(end, t.log_date <= end)
                          select new
                          {
                              t.uid,
                              retail_name = t2.name,
                              t.log_date,
                              t.hits,
                              t.volume_count,
                              t.volume_rate,
                              t.register_count,
                              t.perfection_count,
                              t.perfection_rate,
                              t.income,
                              t.draw_sum
                          }).QueryByPage(up);

            //发送短信验证码
            return new
            {
                code = "success",
                msg = "",
                total_count = result.TotalRow,
                page = result.CurrentPage,
                total_page = result.TotalPage,
                limit = result.Count_Of_OnePage,
                data = from t in result.QueryData<FrameDLRObject>()
                       select new
                       {
                           t.uid,
                           t.retail_name,
                           log_date = DateTimeStd.IsDateTimeThen(t.log_date, "yyyy-MM-dd"),
                           t.hits,
                           t.volume_count,
                           volume_rate = (DoubleStd.IsNotDoubleThen(t.volume_rate) * 100).ToString("0.00"),
                           t.register_count,
                           t.perfection_count,
                           perfection_rate = (DoubleStd.IsNotDoubleThen(t.perfection_rate) * 100).ToString("0.00"),
                           t.income,
                           t.draw_sum
                       }

            };
        }
        [EWRARoute("get", "/reports/retail")]
        [EWRARouteDesc("分销商报表")]
        [EWRAAddInput("start", "string", "起始日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("end", "string", "结束日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
    uid:'uid'，
    app_name:'APP名称',
    log_date:'记录日期',
    hits:'点击量',
    volume_count:'成单数',
    volume_rate:'成单率',
    register_count:'注册数',
    perfection_count:'完善数',
    perfection_rate:'完善率',
    income:'当日收入/佣金合计'
}]
}")]
        object ReportRetails()
        {
            SetCacheEnable(false);
            var start = DateTimeStd.IsDateTimeThen(QueryStringD.start, "yyyy-MM-dd HH:mm:ss");
            var end = DateTimeStd.IsDateTimeThen(QueryStringD.end, "yyyy-MM-dd HH:mm:ss");
            string user_uid = ComFunc.nvl(TokenPayLoad["no"]);
            string usertype = ComFunc.nvl(TokenPayLoad["usertype"]);
            if (!new string[] { "Retail" }.Contains(usertype))
            {
                return new
                {
                    code = "failed",
                    msg = "当前用户不可查看该报表"

                };
            }
            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "RetailSummaryPerDay", "a")
                          join t2 in DB.LamdaTable(up, "APPInfo", "b") on t.app_uid equals t2.uid
                          where t.retail_uid == user_uid && t.notnull(start, t.log_time >= start) && t.notnull(end, t.log_time <= end)
                          select new
                          {
                              t.uid,
                              app_name = t2.name,
                              t.log_time,
                              t.hits,
                              t.volume_count,
                              t.volume_rate,
                              t.register_count,
                              t.perfection_count,
                              t.perfection_rate,
                              t.income
                          }).QueryByPage(up);
            //发送短信验证码
            return new
            {
                code = "success",
                msg = "",
                total_count = result.TotalRow,
                page = result.CurrentPage,
                total_page = result.TotalPage,
                limit = result.Count_Of_OnePage,
                data = from t in result.QueryData<FrameDLRObject>()
                       select new
                       {
                           t.uid,
                           t.app_name,
                           log_date = DateTimeStd.IsDateTimeThen(t.log_time, "yyyy-MM-dd"),
                           t.hits,
                           t.volume_count,
                           volume_rate = (DoubleStd.IsNotDoubleThen(t.volume_rate) * 100).ToString("0.00"),
                           t.register_count,
                           t.perfection_count,
                           perfection_rate = (DoubleStd.IsNotDoubleThen(t.perfection_rate) * 100).ToString("0.00"),
                           t.income
                       }

            };
        }

        [EWRARoute("get", "/reports/retail/{report_uid}/users")]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRARouteDesc("分销商报表-获取明细")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
    log_time:'记录时间'，
    register_time:'注册时间',
    mobile:'手机号，脱敏',
    name:'姓名，脱敏',
    is_deal:'是否成交',
    is_register:'是否注册',
    is_perfection:'是否完善资料',
    deal_amount:'成交金额'
}]
}")]
        object ReportRetails_Users(string report_uid)
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
            var result = (from t in DB.LamdaTable(up, "APPExchange", "a")
                          join t2 in DB.LamdaTable(up, "UserInfo", "b").LeftJoin() on t.mobile equals t2.mobile
                          where t2.recommend_uid == info.retail_uid && t.app_uid == info.app_uid && t.log_time >= $"{dt} 00:00:00" && t.log_time <= $"{dt} 23:59:59"
                          select new
                          {
                              t.log_time,
                              register_time = t2.add_time,
                              t.mobile,
                              t2.name,
                              t.is_deal,
                              t.is_register,
                              t.is_perfection,
                              t.deal_amount
                          }).QueryByPage(up, "log_time desc");
            var tdata = from t in result.QueryData<FrameDLRObject>()
                        select new
                        {
                            log_time = DateTimeStd.IsDateTimeThen(t.log_time, "yyyy-MM-dd HH:mm:ss"),
                            register_time = DateTimeStd.IsDateTimeThen(t.register_time, "yyyy-MM-dd HH:mm:ss"),
                            mobile = t.mobile,
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
                           t.register_time,
                           mobile = ComFunc.nvl(t.mobile).Length > 4 ? $"***{ComFunc.nvl(t.mobile).Substring(ComFunc.nvl(t.mobile).Length - 4, 4)}" : "",
                           name = ComFunc.nvl(t.name).Length > 0 ? ComFunc.nvl(t.name).Substring(0, 1) + "***" : "",
                           t.is_deal,
                           t.is_register,
                           t.is_perfection,
                           t.deal_amount
                       }

            };
        }

        [EWRARoute("get", "/reports/buyback")]
        [EWRAAddInput("start", "string", "起始日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("end", "string", "结束日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRARouteDesc("回购站报表")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
data:{
    app_name:'APP名称',
    hits:'点击量',
    log_time:'记录日期',
    volume_count:'成交数',
    volume_rate:'成交率',
    register_count:'注册数',
    register_rate:'注册率',
    perfection_count:'完善数',
    perfection_rate:'完善率'
}
}")]
        object ReportBuyBack()
        {
            SetCacheEnable(false);
            var start = DateTimeStd.IsDateTimeThen(PostDataD.start, "yyyy-MM-dd HH:mm:ss");
            var end = DateTimeStd.IsDateTimeThen(PostDataD.end, "yyyy-MM-dd HH:mm:ss");
            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "BuyBackSummaryPerDay", "a")
                          join t2 in DB.LamdaTable(up, "APPInfo", "b") on t.app_uid equals t2.uid
                          where t.notnull(start, t.log_time >= start) && t.notnull(end, t.log_time <= end)
                          select new
                          {
                              t.uid,
                              app_name = t2.name,
                              t.log_time,
                              t.volume_count,
                              t.volume_rate,
                              t.register_count,
                              t.register_rate,
                              t.perfection_count,
                              t.perfection_rate,
                          }).QueryByPage(up, "log_time desc");

            //发送短信验证码
            return new
            {
                code = "success",
                msg = "",
                total_count = result.TotalRow,
                page = result.CurrentPage,
                total_page = result.TotalPage,
                limit = result.Count_Of_OnePage,
                data = from t in result.QueryData<FrameDLRObject>()
                       select new
                       {
                           t.uid,
                           t.app_name,
                           log_time = DateTimeStd.IsDateTimeThen(t.log_time, "yyyy-MM-dd HH:mm:ss"),
                           t.volume_count,
                           volume_rate = (DoubleStd.IsNotDoubleThen(t.volume_rate) * 100).ToString("0.00"),
                           t.register_count,
                           register_rate = (DoubleStd.IsNotDoubleThen(t.register_rate) * 100).ToString("0.00"),
                           t.perfection_count,
                           perfection_rate = (DoubleStd.IsNotDoubleThen(t.perfection_rate) * 100).ToString("0.00"),
                       }
            };
        }
        [EWRARoute("get", "/reports/app/today")]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRARouteDesc("导流方当日汇总")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
data:{
    app_name:'APP名称',
    hits:'浏览数',
    volume_count:'成交数',
    volume_rate:'成交率',
    register_count:'注册数',
    perfection_count:'完善数',
    perfection_rate:'完善率',
    remain_amount:'余额',
    consume:'当日消费合计',
    recharge:'当日充值合计',
    consume_sum:'消费总计',
    recharge_sum:'充值总计',
    reqirement_count:'每日需求成交数'
}
}")]
        object ReportAPP()
        {
            SetCacheEnable(false);
            string user_uid = ComFunc.nvl(TokenPayLoad["no"]);
            string usertype = ComFunc.nvl(TokenPayLoad["usertype"]);

            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "APPSummaryPerDay", "a")
                          join t2 in DB.LamdaTable(up, "APPInfo", "b") on t.app_uid equals t2.uid
                          where t.app_uid == user_uid && t.log_time >= DateTime.Now.ToString("yyyy-MM-dd 00:00:00") && t.log_time <= DateTime.Now.ToString("yyyy-MM-dd 23:59:59")
                          orderby t.log_time descending
                          select new
                          {
                              t.uid,
                              app_name = t2.name,
                              t.volume_count,
                              t.volume_rate,
                              t.register_count,
                              t.perfection_count,
                              t.perfection_rate,
                              t.remain_amount,
                              t.consume,
                              t.recharge,
                              t.consume_sum,
                              t.recharge_sum,
                              t.reqirement_count
                          }).GetQueryList(up);
            dynamic data = result.FirstOrDefault();
            if (data != null)
            {
                data.perfection_rate = (DoubleStd.IsNotDoubleThen(data.perfection_rate) * 100).ToString("0.00");
                data.volume_rate = (DoubleStd.IsNotDoubleThen(data.volume_rate) * 100).ToString("0.00");
            }
            //发送短信验证码
            return new
            {
                code = "success",
                msg = "",
                data
            };
        }
        [EWRARoute("get", "/reports/app/history")]
        [EWRAAddInput("start", "string", "起始日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("end", "string", "结束日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRARouteDesc("导流方历史汇总")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
    app_name:'APP名称',
    hits:'浏览数',
    volume_count:'成交数',
    volume_rate:'成交率',
    register_count:'注册数',
    perfection_count:'完善数',
    perfection_rate:'完善率',
    remain_amount:'余额',
    consume:'当日消费合计',
    consume_sum:'消费合计',
    recharge_sum:'充值合计',
    reqirement_count:'每日需求成交数'
}]
}")]
        object ReportAPPHistory()
        {
            SetCacheEnable(false);
            var start = DateTimeStd.IsDateTimeThen(QueryStringD.start, "yyyy-MM-dd HH:mm:ss");
            var end = DateTimeStd.IsDateTimeThen(QueryStringD.end, "yyyy-MM-dd HH:mm:ss");
            string user_uid = ComFunc.nvl(TokenPayLoad["no"]);
            string usertype = ComFunc.nvl(TokenPayLoad["usertype"]);
            if (!new string[] { "App" }.Contains(usertype))
            {
                return new
                {
                    code = "failed",
                    msg = "当前用户不可查看该报表"

                };
            }
            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "APPSummaryPerDay", "a")
                          join t2 in DB.LamdaTable(up, "APPInfo", "b") on t.app_uid equals t2.uid
                          where t.app_uid == user_uid && t.notnull(start, t.log_date >= start) && t.notnull(end, t.log_date <= end)
                          select new
                          {
                              t.uid,
                              app_name = t2.name,
                              log_date = t.log_time,
                              t.volume_count,
                              t.volume_rate,
                              t.register_count,
                              t.perfection_count,
                              t.perfection_rate,
                              t.remain_amount,
                              t.consume,
                              t.consume_sum,
                              t.recharge_sum,
                              t.reqirement_count
                          }).QueryByPage(up, "log_date desc");


            return new
            {
                code = "success",
                msg = "",
                total_count = result.TotalRow,
                page = result.CurrentPage,
                total_page = result.TotalPage,
                limit = result.Count_Of_OnePage,
                data = from t in result.QueryData<FrameDLRObject>()
                       select new
                       {
                           t.uid,
                           t.app_name,
                           log_date = DateTimeStd.IsDateTimeThen(t.log_date, "yyyy-MM-dd HH:mm:ss"),
                           t.volume_count,
                           volume_rate = (DoubleStd.IsNotDoubleThen(t.volume_rate) * 100).ToString("0.00"),
                           t.register_count,
                           t.perfection_count,
                           perfection_rate = (DoubleStd.IsNotDoubleThen(t.perfection_rate) * 100).ToString("0.00"),
                           t.remain_amount,
                           t.consume,
                           t.consume_sum,
                           t.recharge_sum,
                           t.reqirement_count
                       }
            };
        }
        [EWRARoute("get", "/reports/plat")]
        [EWRAAddInput("start", "string", "起始日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("end", "string", "结束日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRARouteDesc("平台数据报表")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
    app_name:'APP名称',
    hits:'点击量',
    volume_count:'成交数',
    volume_rate:'成交率',
    register_count:'注册数',
    perfection_count:'完善数',
    perfection_rate:'完善率',
    consume_amount:'当日消费合计',
    recharge_amount:'当日充值合计',
    withdrew_amount:'当日提现合计',
    profit_amount:'利润合计',
    remain_amount:'余额合计',
    consume_sum:'消费合计(总计)',
    recharge_sum:'充值合计(总计)',
    withdrew_sum:'提现合计(总计)'
}]
}")]
        object ReportPlat()
        {
            SetCacheEnable(false);
            var start = DateTimeStd.IsDateTimeThen(QueryStringD.start, "yyyy-MM-dd HH:mm:ss");
            var end = DateTimeStd.IsDateTimeThen(QueryStringD.end, "yyyy-MM-dd HH:mm:ss");
            string user_uid = ComFunc.nvl(TokenPayLoad["no"]);
            string usertype = ComFunc.nvl(TokenPayLoad["usertype"]);

            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "PlatSummaryPerDay", "a")
                          where t.notnull(start, t.log_date >= start) && t.notnull(end, t.log_date <= end)
                          select new
                          {
                              t.uid,
                              t.log_date,
                              t.hits,
                              t.register_count,
                              t.perfection_count,
                              t.perfection_rate,
                              t.volume_count,
                              t.volume_rate,
                              t.consume_amount,
                              t.recharge_amount,
                              t.withdrew_amount,
                              t.profit_amount,
                              t.remain_amount,
                              t.consume_sum,
                              t.recharge_sum,
                              t.withdrew_sum
                          }).QueryByPage(up, "log_date desc");


            return new
            {
                code = "success",
                msg = "",
                total_count = result.TotalRow,
                page = result.CurrentPage,
                total_page = result.TotalPage,
                limit = result.Count_Of_OnePage,
                data = from t in result.QueryData<FrameDLRObject>()
                       select new
                       {
                           t.uid,
                           log_date = DateTimeStd.IsDateTimeThen(t.log_date, "yyyy-MM-dd HH:mm:ss"),
                           t.hits,
                           t.register_count,
                           t.perfection_count,
                           perfection_rate = (DoubleStd.IsNotDoubleThen(t.perfection_rate) * 100).ToString("0.00"),
                           t.volume_count,
                           volume_rate = (DoubleStd.IsNotDoubleThen(t.volume_rate) * 100).ToString("0.00"),
                           t.consume_amount,
                           t.recharge_amount,
                           t.withdrew_amount,
                           t.profit_amount,
                           t.remain_amount,
                           t.consume_sum,
                           t.recharge_sum,
                           t.withdrew_sum
                       }
            };
        }
        [EWRARoute("get", "/reports/channel_plat")]
        [EWRARouteDesc("渠道商报表-平台查看,报账使用")]
        [EWRAAddInput("start", "string", "起始日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("end", "string", "结束日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
    uid:'uid'，
    channel_name:'渠道商名称',
    retail_name:'分销商名称',
    log_date:'记录日期',
    hits:'点击量',
    volume_count:'成交数',
    volume_rate:'成交率',
    register_count:'注册量',
    perfection_count:'用户资料完善数',
    perfection_rate:'完善率',
    income:'提成',
    unit_price_cps:'成单单价',
    unit_price_cpa:'注册单价',
    reim_status:'报账状态'
}]
}")]
        object ReportChannel_Plat()
        {
            SetCacheEnable(false);
            var start = DateTimeStd.IsDateTimeThen(QueryStringD.start, "yyyy-MM-dd HH:mm:ss");
            var end = DateTimeStd.IsDateTimeThen(QueryStringD.end, "yyyy-MM-dd HH:mm:ss");
            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "ChannelSummaryPerDay", "a")
                          join t2 in DB.LamdaTable(up, "Retail", "b") on t.retail_uid equals t2.uid
                          join t3 in DB.LamdaTable(up, "ChannelBusiness", "c") on t.channel_uid equals t3.uid
                          join t4 in DB.LamdaTable(up, "ChannelReimbursement", "d").LeftJoin() on t.uid equals t4.report_source
                          where t.notnull(start, t.log_date >= start) && t.notnull(end, t.log_date <= end)
                          select new
                          {
                              t.uid,
                              channel_name = t3.name,
                              retail_name = t2.name,
                              t.log_date,
                              t.hits,
                              t.volume_count,
                              t.volume_rate,
                              t.register_count,
                              t.perfection_count,
                              t.perfection_rate,
                              t.income,
                              t.draw_sum,
                              t.unit_price_cpa,
                              t.unit_price_cps,
                              reim_status = t4.status
                          }).QueryByPage(up, "log_date desc,channel_name");
            //发送短信验证码
            return new
            {
                code = "success",
                msg = "",
                total_count = result.TotalRow,
                page = result.CurrentPage,
                total_page = result.TotalPage,
                limit = result.Count_Of_OnePage,
                data = from t in result.QueryData<FrameDLRObject>()
                       select new
                       {
                           t.uid,
                           t.channel_name,
                           t.retail_name,
                           log_date = DateTimeStd.IsDateTimeThen(t.log_date, "yyyy-MM-dd"),
                           t.hits,
                           t.volume_count,
                           volume_rate = (DoubleStd.IsNotDoubleThen(t.volume_rate) * 100).ToString("0.00"),
                           t.register_count,
                           t.perfection_count,
                           perfection_rate = (DoubleStd.IsNotDoubleThen(t.perfection_rate) * 100).ToString("0.00"),
                           t.income,
                           t.draw_sum,
                           t.unit_price_cpa,
                           t.unit_price_cps,
                           t.reim_status
                       }

            };
        }
        [EWRARoute("get", "/reports/retail_plat")]
        [EWRARouteDesc("分销商报表-平台查看,报账使用")]
        [EWRAAddInput("start", "string", "起始日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("end", "string", "结束日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
    uid:'uid'，
    retail_name:'分销商名称',
    app_name:'APP名称',
    log_date:'记录日期',
    hits:'点击量',
    volume_count:'成单数',
    volume_rate:'成单率',
    register_count:'注册数',
    perfection_count:'完善数',
    perfection_rate:'完善率',
    income:'当日收入/佣金合计',
    unit_price:'单价',
    reim_status:'报账状态'
}]
}")]
        object ReportRetails_Plat()
        {
            SetCacheEnable(false);
            var start = DateTimeStd.IsDateTimeThen(QueryStringD.start, "yyyy-MM-dd HH:mm:ss");
            var end = DateTimeStd.IsDateTimeThen(QueryStringD.end, "yyyy-MM-dd HH:mm:ss");

            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "RetailSummaryPerDay", "a")
                          join t2 in DB.LamdaTable(up, "APPInfo", "b") on t.app_uid equals t2.uid
                          join t3 in DB.LamdaTable(up, "Retail", "c") on t.retail_uid equals t3.uid
                          join t4 in DB.LamdaTable(up, "RetailReimbursement", "d").LeftJoin() on t.uid equals t4.report_source
                          where t.notnull(start, t.log_time >= start) && t.notnull(end, t.log_time <= end)
                          select new
                          {
                              t.uid,
                              retail_name = t3.name,
                              app_name = t2.name,
                              t.log_time,
                              t.hits,
                              t.volume_count,
                              t.volume_rate,
                              t.register_count,
                              t.perfection_count,
                              t.perfection_rate,
                              t.income,
                              t.unit_price,
                              reim_status = t4.status
                          }).QueryByPage(up, "log_time desc,retail_name");
            //发送短信验证码
            return new
            {
                code = "success",
                msg = "",
                total_count = result.TotalRow,
                page = result.CurrentPage,
                total_page = result.TotalPage,
                limit = result.Count_Of_OnePage,
                data = from t in result.QueryData<FrameDLRObject>()
                       select new
                       {
                           t.uid,
                           t.retail_name,
                           t.app_name,
                           log_date = DateTimeStd.IsDateTimeThen(t.log_time, "yyyy-MM-dd"),
                           t.hits,
                           t.volume_count,
                           volume_rate = (DoubleStd.IsNotDoubleThen(t.volume_rate) * 100).ToString("0.00"),
                           t.register_count,
                           t.perfection_count,
                           perfection_rate = (DoubleStd.IsNotDoubleThen(t.perfection_rate) * 100).ToString("0.00"),
                           t.income,
                           t.unit_price,
                           t.reim_status
                       }

            };
        }
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
                    where t2.recommend_uid == info.retail_uid && t.app_uid == info.app_uid && t.log_time >= $"{dt} 00:00:00" && t.log_time <= $"{dt} 23:59:59"
                    select new
                    {
                        t.log_time,
                        register_time = t2.add_time,
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
                            register_time = DateTimeStd.IsDateTimeThen(t.register_time, "yyyy-MM-dd HH:mm:ss"),
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
                           t.register_time,
                           mobile = ComFunc.nvl(t.mobile).Length > 4 ? $"***{ComFunc.nvl(t.mobile).Substring(ComFunc.nvl(t.mobile).Length - 4, 4)}" : "",
                           name = ComFunc.nvl(t.name).Length > 0 ? ComFunc.nvl(t.name).Substring(0, 1) + "***" : "",
                           t.is_deal,
                           t.is_register,
                           t.is_perfection,
                           t.deal_amount
                       }

            };
        }

        [EWRARoute("get", "/reports/app_plat")]
        [EWRAAddInput("start", "string", "起始日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("end", "string", "结束日期", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRARouteDesc("导流方历史汇总")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
    uid:'报表UID'，
    app_name:'APP名称',
    hits:'浏览数',
    volume_count:'成交数',
    volume_rate:'成交率',
    register_count:'注册数',
    perfection_count:'完善数',
    perfection_rate:'完善率',
    remain_amount:'余额',
    consume:'当日消费合计',
    consume_sum:'消费合计',
    recharge_sum:'充值合计',
    reqirement_count:'每日需求成交数',
    reim_status:'报账状态'
}]
}")]
        object ReportAPP_Plat()
        {
            SetCacheEnable(false);
            var start = DateTimeStd.IsDateTimeThen(QueryStringD.start, "yyyy-MM-dd HH:mm:ss");
            var end = DateTimeStd.IsDateTimeThen(QueryStringD.end, "yyyy-MM-dd HH:mm:ss");
            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "APPSummaryPerDay", "a")
                          join t2 in DB.LamdaTable(up, "APPInfo", "b") on t.app_uid equals t2.uid
                          join t3 in DB.LamdaTable(up, "AppReimbursement", "c").LeftJoin() on t.uid equals t3.report_source
                          where t.notnull(start, t.log_time >= start) && t.notnull(end, t.log_time <= end)
                          select new
                          {
                              t.uid,
                              t.hits,
                              app_name = t2.name,
                              log_date = t.log_time,
                              t.volume_count,
                              t.volume_rate,
                              t.register_count,
                              t.perfection_count,
                              t.perfection_rate,
                              t.remain_amount,
                              t.consume,
                              t.consume_sum,
                              t.recharge_sum,
                              t.reqirement_count,
                              reim_status = t3.status
                          }).QueryByPage(up, "log_date desc");


            return new
            {
                code = "success",
                msg = "",
                total_count = result.TotalRow,
                page = result.CurrentPage,
                total_page = result.TotalPage,
                limit = result.Count_Of_OnePage,
                data = from t in result.QueryData<FrameDLRObject>()
                       select new
                       {
                           t.uid,
                           t.hits,
                           t.app_name,
                           log_date = DateTimeStd.IsDateTimeThen(t.log_date, "yyyy-MM-dd HH:mm:ss"),
                           t.volume_count,
                           volume_rate = (DoubleStd.IsNotDoubleThen(t.volume_rate) * 100).ToString("0.00"),
                           t.register_count,
                           t.perfection_count,
                           perfection_rate = (DoubleStd.IsNotDoubleThen(t.perfection_rate) * 100).ToString("0.00"),
                           t.remain_amount,
                           t.consume,
                           t.consume_sum,
                           t.recharge_sum,
                           t.reqirement_count,
                           t.reim_status
                       }
            };
        }

        [EWRAEmptyValid("data")]
        [EWRARoute("patch", "/reports/app_plat/{uid}")]
        [EWRAAddInput("uid", "string", "报表UID", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("data", "json", @"结构为：{
hits:'点击数',
volume_count:'成交数',
register_count:'注册数',
perfection_count:'完善数',
consume:'当日消费合计'
}", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRARouteDesc("导流方历史汇总")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息""
}")]
        object UpdateReportAPP(string uid)
        {
            dynamic data = PostDataD.data;
            BeginTrans();
            var up = DB.NewDBUnitParameter();
            var list = (from t in DB.LamdaTable(up, "APPSummaryPerDay", "a")
                        where t.uid == uid
                        select t).GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "资料不存在"
                };
            }
            dynamic info = list.First();
            if (BoolStd.IsNotBoolThen(info.is_confirm))
            {
                return new
                {
                    code = "failed",
                    msg = "该资料已核准，无法再执行操作"
                };
            }
            var reimlist = (from t in DB.LamdaTable(up, "AppReimbursement", "a")
                            where t.report_source == info.uid && t.status != "cancel"
                            select t).GetQueryList(up);
            if (reimlist.Count > 0)
            {
                return new
                {
                    code = "failed",
                    msg = "该资料已报账"
                };
            }

            var register_count = IntStd.IsNotIntThen(data.register_count);
            DB.QuickUpdate(up, "APPSummaryPerDay", new
            {
                hits = IntStd.IsNotIntThen(data.hits),
                register_count,
                volume_count = IntStd.IsNotIntThen(data.volume_count),
                volume_rate = register_count == 0 ? 0.0 : DoubleStd.IsNotDoubleThen(data.volume_count) / register_count,
                perfection_count = IntStd.IsNotIntThen(data.perfection_count),
                perfection_rate = register_count == 0 ? 0.0 : DoubleStd.IsNotDoubleThen(data.perfection_count) / register_count,
                consume = DoubleStd.IsNotDoubleThen(data.consume)
            }, new
            {
                info.uid
            });
            CommitTrans();
            return new
            {
                code = "success",
                msg = "操作成功"
            };
        }
        [EWRAEmptyValid("data")]
        [EWRARoute("patch", "/reports/retail_plat/{uid}")]
        [EWRAAddInput("uid", "string", "报表UID", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("data", "json", @"结构为：{
hits:'点击数',
volume_count:'成交数',
register_count:'注册数',
perfection_count:'完善数',
unit_price:'单价',
income:'佣金收入'
}", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRARouteDesc("分销商报表修正")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息""
}")]
        object UpdateReportRetail(string uid)
        {
            SetCacheEnable(false);
            dynamic data = PostDataD.data;
            BeginTrans();
            var up = DB.NewDBUnitParameter();
            var list = (from t in DB.LamdaTable(up, "RetailSummaryPerDay", "a")
                        where t.uid == uid
                        select t).GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "资料不存在"
                };
            }
            dynamic info = list.First();
            if (BoolStd.IsNotBoolThen(info.is_confirm))
            {
                return new
                {
                    code = "failed",
                    msg = "该资料已核准，无法再执行操作"
                };
            }
            var reimlist = (from t in DB.LamdaTable(up, "RetailReimbursement", "a")
                            where t.report_source == info.uid && t.status != "cancel"
                            select t).GetQueryList(up);
            if (reimlist.Count > 0)
            {
                return new
                {
                    code = "failed",
                    msg = "该资料已报账"
                };
            }
            var register_count = IntStd.IsNotIntThen(data.register_count);
            DB.QuickUpdate(up, "RetailSummaryPerDay", new
            {
                hits = IntStd.IsNotIntThen(data.hits),
                register_count,
                volume_count = IntStd.IsNotIntThen(data.volume_count),
                volume_rate = register_count == 0 ? 0.0 : DoubleStd.IsNotDoubleThen(data.volume_count) / register_count,
                perfection_count = IntStd.IsNotIntThen(data.perfection_count),
                perfection_rate = register_count == 0 ? 0.0 : DoubleStd.IsNotDoubleThen(data.perfection_count) / register_count,
                unit_price = DoubleStd.IsNotDoubleThen(data.unit_price),
                income = DoubleStd.IsNotDoubleThen(data.income)
            }, new
            {
                info.uid
            });
            CommitTrans();
            return new
            {
                code = "success",
                msg = "操作成功"
            };
        }
        [EWRAEmptyValid("data")]
        [EWRARoute("patch", "/reports/channel_plat/{uid}")]
        [EWRAAddInput("uid", "string", "报表UID", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("data", "json", @"结构为：{
hits:'点击数',
volume_count:'成交数',
register_count:'注册数',
perfection_count:'完善数',
unit_price_cps:'成单单价',
unit_price_cpa:'注册单价',
income:'提成'
}", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRARouteDesc("渠道商报表修正")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息""
}")]
        object UpdateReportChannel(string uid)
        {
            dynamic data = PostDataD.data;
            BeginTrans();
            var up = DB.NewDBUnitParameter();
            var list = (from t in DB.LamdaTable(up, "ChannelSummaryPerDay", "a")
                        where t.uid == uid
                        select t).GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "资料不存在"
                };
            }
            dynamic info = list.First();
            if (BoolStd.IsNotBoolThen(info.is_confirm))
            {
                return new
                {
                    code = "failed",
                    msg = "该资料已核准，无法再执行操作"
                };
            }
            var reimlist = (from t in DB.LamdaTable(up, "ChannelReimbursement", "a")
                            where t.report_source == info.uid && t.status != "cancel"
                            select t).GetQueryList(up);
            if (reimlist.Count > 0)
            {
                return new
                {
                    code = "failed",
                    msg = "该资料已报账"
                };
            }
            var register_count = IntStd.IsNotIntThen(data.register_count);
            DB.QuickUpdate(up, "ChannelSummaryPerDay", new
            {
                hits = IntStd.IsNotIntThen(data.hits),
                register_count,
                volume_count = IntStd.IsNotIntThen(data.volume_count),
                volume_rate = register_count == 0 ? 0.0 : DoubleStd.IsNotDoubleThen(data.volume_count) / register_count,
                perfection_count = IntStd.IsNotIntThen(data.perfection_count),
                perfection_rate = register_count == 0 ? 0.0 : DoubleStd.IsNotDoubleThen(data.perfection_count) / register_count,
                unit_price_cps = DoubleStd.IsNotDoubleThen(data.unit_price_cps),
                unit_price_cpa = DoubleStd.IsNotDoubleThen(data.unit_price_cpa),
                income = DoubleStd.IsNotDoubleThen(data.income)
            }, new
            {
                info.uid
            });
            CommitTrans();
            return new
            {
                code = "success",
                msg = "操作成功"
            };
        }


        [EWRARoute("get", "/reports/user")]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRARouteDesc("用户资料汇总")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
    uid:'记录uid'
    name:'用户姓名',
    mobile:'用户手机号',
    id_no:'身份证号',
    recommend_name:'推荐人'
}]
}")]
        object ReportUser()
        {
            SetCacheEnable(false);
            string user_uid = ComFunc.nvl(TokenPayLoad["no"]);
            string usertype = ComFunc.nvl(TokenPayLoad["usertype"]);

            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "UserInfo", "a")
                          join t2 in DB.LamdaTable(up, "Retail", "b").LeftJoin() on t.recommend_uid equals t2.uid
                          select new
                          {
                              t.uid,
                              t.name,
                              t.mobile,
                              t.id_no,
                              recommend_name = t2.name
                          }).QueryByPage(up);
            var is_filter_sentive = true;
            var cur_role_names = ComFunc.nvl(TokenPayLoad["Login_Role_Name"]).Split(",");
            if (usertype == "App" || cur_role_names.Contains("运营"))
            {
                is_filter_sentive = true;
            }
            if (cur_role_names.Contains("运营主管"))
            {
                is_filter_sentive = false;
            }
            var tdata = from t in result.QueryData<FrameDLRObject>()
                        select new
                        {
                            t.uid,
                            name = ComFunc.nvl(t.name).Length > 0 ? ComFunc.AESDecrypt(ComFunc.nvl(t.name)) : "",
                            mobile = ComFunc.nvl(t.mobile),
                            id_no = ComFunc.nvl(t.id_no).Length > 0 ? ComFunc.AESDecrypt(ComFunc.nvl(t.id_no)) : "",
                            t.recommend_name
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
                           t.uid,
                           name = is_filter_sentive ? ComFunc.nvl(t.name).Length > 0 ? ComFunc.nvl(t.name).Substring(0, 1) + "***" : "" : t.name,
                           mobile = is_filter_sentive ? (ComFunc.nvl(t.mobile).Length > 4 ? $"***{ComFunc.nvl(t.mobile).Substring(ComFunc.nvl(t.mobile).Length - 4, 4)}" : "") : t.mobile,
                           id_no = is_filter_sentive ? $"{(ComFunc.nvl(t.id_no).Length > 0 ? ComFunc.nvl(t.id_no).Substring(0, 3) : "")}***" : t.id_no,
                           t.recommend_name
                       }
            };
        }
        [EWRARoute("get", "/reports/finace")]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRARouteDesc("财务统计")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
    uid:'记录uid',
    log_date:'记录日期',
    total_consume:'总消费',
    total_recharge:'充值总额',
    total_remain:'余额',
    total_profit:'利润合计',
    total_retail_profit:'分销商利润合计',
    total_channel_profit:'渠道商利润合计'
}]
}")]
        object ReportFinace()
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "FinaceSummaryPerDay", "a")
                          select new
                          {
                              t.uid,
                              t.log_date,
                              t.total_consume,
                              t.total_recharge,
                              t.total_remain,
                              t.total_profit,
                              t.total_retail_profit,
                              t.total_channel_profit
                          }).QueryByPage(up, "log_date desc");

            return new
            {
                code = "success",
                msg = "",
                total_count = result.TotalRow,
                page = result.CurrentPage,
                total_page = result.TotalPage,
                limit = result.Count_Of_OnePage,
                data = from t in result.QueryData<FrameDLRObject>()
                       select new
                       {
                           t.uid,
                           log_date = DateTimeStd.IsDateTimeThen(t.log_date, "yyyy-MM-dd"),
                           t.total_consume,
                           t.total_recharge,
                           t.total_remain,
                           t.total_profit,
                           t.total_retail_profit,
                           t.total_channel_profit
                       }
            };
        }


        [EWRARoute("patch", "/report/app/{app_summary_uid}/reimbursement")]
        [EWRAAddInput("app_summary_uid", "string", "报表UID", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, true)]
        [EWRARouteDesc("给指定的汇总报表资料报账")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息""
}")]
        object CreateAppReimbursement(string app_summary_uid)
        {
            var up = DB.NewDBUnitParameter();
            BeginTrans();
            var list = (from t in DB.LamdaTable(up, "APPSummaryPerDay", "a")
                        where t.uid == app_summary_uid
                        select t).GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "该资料不存在"
                };
            }
            dynamic info = list.First();
            if (BoolStd.IsNotBoolThen(info.is_confirm))
            {
                return new
                {
                    code = "failed",
                    msg = "该资料已核准，无法再执行操作"
                };
            }
            var reimlist = (from t in DB.LamdaTable(up, "AppReimbursement", "a")
                            where t.report_source == info.uid && t.status != "cancel"
                            select t).GetQueryList(up);
            if (reimlist.Count > 0)
            {
                return new
                {
                    code = "failed",
                    msg = "该资料已报账"
                };
            }
            DB.QuickDelete(up, "AppReimbursement", new
            {
                report_source = info.uid
            });
            DB.QuickInsert(up, "AppReimbursement", new
            {
                uid = Guid.NewGuid().ToString(),
                app_uid = info.app_uid,
                submit_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                consume = DoubleStd.IsNotDoubleThen(info.consume),
                status = "ready",
                report_source = info.uid,
                add_id = TokenPayLoad.ID,
                add_ip = ClientInfo.IP,
                add_name = ComFunc.nvl(TokenPayLoad["username"]),
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            CommitTrans();
            return new
            {
                code = "success",
                msg = "操作成功"
            };
        }
        [EWRARoute("patch", "/report/retail/{retail_summary_uid}/reimbursement")]
        [EWRAAddInput("retail_summary_uid", "string", "报表uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, true)]
        [EWRARouteDesc("给指定的汇总报表资料报账")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息""
}")]
        object CreateRetailReimbursement(string retail_summary_uid)
        {
            var up = DB.NewDBUnitParameter();
            BeginTrans();
            var list = (from t in DB.LamdaTable(up, "RetailSummaryPerDay", "a")
                        where t.uid == retail_summary_uid
                        select t).GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "该资料不存在"
                };
            }
            dynamic info = list.First();
            if (BoolStd.IsNotBoolThen(info.is_confirm))
            {
                return new
                {
                    code = "failed",
                    msg = "该资料已核准，无法再执行操作"
                };
            }
            var reimlist = (from t in DB.LamdaTable(up, "RetailReimbursement", "a")
                            where t.report_source == info.uid && t.status != "cancel"
                            select t).GetQueryList(up);
            if (reimlist.Count > 0)
            {
                return new
                {
                    code = "failed",
                    msg = "该资料已报账"
                };
            }
            DB.QuickDelete(up, "RetailReimbursement", new
            {
                report_source = info.uid
            });
            DB.QuickInsert(up, "RetailReimbursement", new
            {
                uid = Guid.NewGuid().ToString(),
                retail_uid = info.retail_uid,
                submit_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                income = DoubleStd.IsNotDoubleThen(info.income),
                status = "ready",
                report_source = info.uid,
                add_id = TokenPayLoad.ID,
                add_ip = ClientInfo.IP,
                add_name = ComFunc.nvl(TokenPayLoad["username"]),
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            CommitTrans();
            return new
            {
                code = "success",
                msg = "操作成功"
            };
        }
        [EWRARoute("patch", "/report/channel/{channel_summary_uid}/reimbursement")]
        [EWRAAddInput("channel_summary_uid", "string", "报表uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, true)]
        [EWRARouteDesc("给指定的汇总报表资料报账")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息""
}")]
        object CreateChannelReimbursement(string channel_summary_uid)
        {
            var up = DB.NewDBUnitParameter();
            BeginTrans();
            var list = (from t in DB.LamdaTable(up, "ChannelSummaryPerDay", "a")
                        where t.uid == channel_summary_uid
                        select t).GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "该资料不存在"
                };
            }
            dynamic info = list.First();
            if (BoolStd.IsNotBoolThen(info.is_confirm))
            {
                return new
                {
                    code = "failed",
                    msg = "该资料已核准，无法再执行操作"
                };
            }
            var reimlist = (from t in DB.LamdaTable(up, "ChannelReimbursement", "a")
                            where t.report_source == info.uid && t.status != "cancel"
                            select t).GetQueryList(up);
            if (reimlist.Count > 0)
            {
                return new
                {
                    code = "failed",
                    msg = "该资料已报账"
                };
            }
            DB.QuickDelete(up, "ChannelReimbursement", new
            {
                report_source = info.uid
            });
            DB.QuickInsert(up, "ChannelReimbursement", new
            {
                uid = Guid.NewGuid().ToString(),
                info.channel_uid,
                submit_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                income = DoubleStd.IsNotDoubleThen(info.income),
                status = "ready",
                report_source = info.uid,
                add_id = TokenPayLoad.ID,
                add_ip = ClientInfo.IP,
                add_name = ComFunc.nvl(TokenPayLoad["username"]),
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            CommitTrans();
            return new
            {
                code = "success",
                msg = "操作成功"
            };
        }
    }
}
