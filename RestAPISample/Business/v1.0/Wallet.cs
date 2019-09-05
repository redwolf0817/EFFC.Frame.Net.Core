using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Unit.DB;

namespace RestAPISample.Business.v1._0
{
    public class Wallet : MyRestLogic
    {
        static object lockobj = new object();
        [EWRARoute("get", "/wallet/events")]
        [EWRARouteDesc("获取事件类型列表")]
        object GetEventType()
        {
            SetCacheEnable(false);
            var eventtype = new string[] { "打款", "冻结", "取消", "核销" };
            return new { events = eventtype };
        }
        [EWRARouteDesc("获取指定用户的钱包信息")]
        public override object get(string id)
        {
            SetCacheEnable(false);
            var ou_id = ComFunc.nvl(QueryStringD.ou_id);
            var up = DB.NewDBUnitParameter();

            var s = from t in DB.LamdaTable("ecs_user_wallet", "a")
                    join t2 in DB.LamdaTable("ecs_users", "b").RightJoin() on t.user_id equals t2.user_id
                    where t2.user_id == id
                    select new
                    {
                        t2.user_id,
                        t.memo,
                        t.total_amount,
                        t.freeze_amount,
                        t.used_amount
                    };

            var result = DB.ExcuteLamda(up, s);
            if (result.QueryTable.RowLength > 0)
            {
                var ss = from t in DB.LamdaTable("ecs_user_wallet_line", "a")
                         where t.user_id == id && t.notnull(ou_id, t.ebs_ou_id == ou_id)
                         select t;
                var totalamount = DecimalStd.IsNotDecimalThen(DB.Sum(up, ss, "total_amount"));
                var freezeamount = DecimalStd.IsNotDecimalThen(DB.Sum(up, ss, "freeze_amount"));
                var usedamount = DecimalStd.IsNotDecimalThen(DB.Sum(up, ss, "used_amount"));
                var available = totalamount - freezeamount - usedamount;
                return new
                {
                    code = "success",

                    data = (from t in result.QueryData<FrameDLRObject>()
                            select new
                            {
                                t.user_id,
                                t.memo,
                                total_amount = totalamount,
                                freeze_amount = freezeamount,
                                used_amount = usedamount,
                                available_amount = available
                            }).ElementAt(0)
                };
            }
            else
            {
                return new
                {
                    code = "failed",
                    msg = "no data"
                };
            }

        }

        [EWRARouteDesc("获取所有用户的钱包信息")]
        public object get()
        {
            SetCacheEnable(false);
            var ou_id = ComFunc.nvl(QueryStringD.ou_id);
            var up = DB.NewDBUnitParameter();

            var s = from t in DB.LamdaTable("ecs_user_wallet", "a")
                    join t2 in DB.LamdaTable("ecs_users", "b").RightJoin() on t.user_id equals t2.user_id
                    select new
                    {
                        t2.user_id,
                        t2.user_name,
                        t2.customer_num,
                        t2.company_name,
                        t.memo,
                        t.total_amount,
                        t.freeze_amount,
                        t.used_amount
                    };

            var result = DB.LamdaQueryByPage(up, s, "user_name");
            if (result.QueryTable.RowLength > 0)
            {
                var ss = from t in DB.LamdaTable("ecs_user_wallet_line", "a")
                         where t.notnull(ou_id, t.ebs_ou_id == ou_id)
                         select t;
                var totalamount = DecimalStd.IsNotDecimalThen(DB.Sum(up, ss, "total_amount"));
                var freezeamount = DecimalStd.IsNotDecimalThen(DB.Sum(up, ss, "freeze_amount"));
                var usedamount = DecimalStd.IsNotDecimalThen(DB.Sum(up, ss, "used_amount"));
                var available = totalamount - freezeamount - usedamount;
                return new
                {
                    code = "success",

                    data = (from t in result.QueryData<FrameDLRObject>()
                            select new
                            {
                                t.user_id,
                                t.user_name,
                                t.customer_num,
                                t.company_name,
                                t.memo,
                                total_amount = totalamount,
                                freeze_amount = freezeamount,
                                used_amount = usedamount,
                                available_amount = available
                            }).ElementAt(0)
                };
            }
            else
            {
                return new
                {
                    code = "failed",
                    msg = "no data"
                };
            }

        }


        [EWRARoute("get", "/wallet/log")]
        [EWRARouteDesc("获取用户的流水资料")]
        object GetLog()
        {
            SetCacheEnable(false);
            string userids = ComFunc.nvl(QueryStringD.user_ids);
            var start_date = DateTimeStd.IsDateTimeThen(QueryStringD.start_date, "yyyy-MM-dd HH:mm:ss");
            var end_date = DateTimeStd.IsDateTimeThen(QueryStringD.end_date, "yyyy-MM-dd HH:mm:ss");
            var events = ComFunc.nvl(QueryStringD.events);

            var up = DB.NewDBUnitParameter();

            var s = from t in DB.LamdaTable("ecs_user_wallet_log", "a")
                    where t.notnull(start_date, t.last_time >= start_date)
                        && t.notnull(end_date, t.last_time <= end_date)
                        && t.notnull(events, t.events.contains(events))
                        && t.notnull(userids, t.user_id.within(userids))
                    select t;
            var result = DB.LamdaQueryByPage(up, s, "last_time desc,seq desc");
            var list = result.QueryData<FrameDLRObject>();

            return new
            {
                code = "success",
                total_num = result.TotalRow,
                current_page = result.CurrentPage,
                total_page = result.TotalPage,
                page_size = result.Count_Of_OnePage,
                data = (from t in list
                        select new
                        {
                            t.user_id,
                            t.amount,
                            t.ebs_wd_no,
                            t.events,
                            t.memo,
                            modi_time = DateTimeStd.IsDateTimeThen(t.last_time, "yyyy-MM-dd HH:mm:ss"),
                            t.seq,
                            in_amount = (t.events == "打款") ? t.amount : 0,
                            out_amount = (t.events == "核销") ? t.amount : 0,
                            t.total_amount,
                            t.available_amount,
                            t.freeze_amount,
                            t.used_amount,
                            t.line_total_amount,
                            t.line_available_amount,
                            t.line_freeze_amount,
                            t.line_used_amount
                        }).ToList()
            };
        }

        [EWRARoute("patch", "/wallet/{id}/freeze")]
        [EWRARouteDesc("冻结指定金额")]
        [EWRADoubleValid("amount", true, true)]
        [EWRAEmptyValid("ou_id")]
        object Freeze(string id)
        {
            lock (lockobj)
            {
                double amount = PostDataD.amount;
                var memo = ComFunc.nvl(PostDataD.memo);
                var ou_id = ComFunc.nvl(PostDataD.ou_id);
                var up = DB.NewDBUnitParameter();
                BeginTrans();

                var s = from t in DB.LamdaTable("ecs_user_wallet", "a")
                        where t.user_id == id
                        select t;
                var result = DB.ExcuteLamda(up, s);
                if (result.QueryTable.RowLength <= 0)
                {
                    return new { code = "failed", msg = "账号无效" };
                }

                dynamic item = result.QueryData<FrameDLRObject>()[0];
                var tempamount = DoubleStd.IsNotDoubleThen(item.freeze_amount) + amount + DoubleStd.IsNotDoubleThen(item.used_amount);
                if (tempamount > DoubleStd.IsNotDoubleThen(item.total_amount))
                {
                    return new { code = "failed", msg = "金额溢出" };
                }
                //冻结行资料
                var leftamount = amount;
                //找出可用的行资料,需要与ou_id进行匹配
                var availablelist = DB.ExcuteLamda(up, from t in DB.LamdaTable("ecs_user_wallet_line", "a")
                                                       orderby t.line_no ascending
                                                       where t.user_id == id && t.notnull(ou_id, t.ebs_ou_id == ou_id) && (t.total_amount.isnull(0) - t.freeze_amount.isnull(0) - t.used_amount.isnull(0)) > 0
                                                       select t).QueryData<FrameDLRObject>();
                if (availablelist.Count <= 0)
                {
                    return new { code = "failed", msg = "无可用额度" };
                }
                var total_freeze_amount = DoubleStd.IsNotDoubleThen(item.freeze_amount);
                var total_used_amount = DoubleStd.IsNotDoubleThen(item.used_amount);
                var tmplist = new List<object>();
                foreach (dynamic v in availablelist)
                {
                    var available_amount = DoubleStd.IsNotDoubleThen(v.total_amount) - DoubleStd.IsNotDoubleThen(v.freeze_amount) - DoubleStd.IsNotDoubleThen(v.used_amount);

                    if (available_amount >= leftamount)
                    {
                        v.freeze_amount = DoubleStd.IsNotDoubleThen(v.freeze_amount) + leftamount;
                        v.current_freeze_amount = leftamount;
                        v.available_amount = DoubleStd.IsNotDoubleThen(v.total_amount) - DoubleStd.IsNotDoubleThen(v.freeze_amount) - DoubleStd.IsNotDoubleThen(v.used_amount);
                        v.all_total_amount = item.total_amount;
                        v.all_freeze_amount = total_freeze_amount = total_freeze_amount + leftamount;
                        v.all_used_amount = total_used_amount;
                        v.all_available_amount = DoubleStd.IsNotDoubleThen(v.all_total_amount) - DoubleStd.IsNotDoubleThen(v.all_freeze_amount) - DoubleStd.IsNotDoubleThen(v.all_used_amount);
                        leftamount = 0;
                        tmplist.Add(v);
                        break;
                    }
                    else
                    {
                        v.freeze_amount = DoubleStd.IsNotDoubleThen(v.freeze_amount) + available_amount;
                        v.current_freeze_amount = available_amount;
                        v.available_amount = DoubleStd.IsNotDoubleThen(v.total_amount) - DoubleStd.IsNotDoubleThen(v.freeze_amount) - DoubleStd.IsNotDoubleThen(v.used_amount);
                        v.all_total_amount = item.total_amount;
                        v.all_freeze_amount = total_freeze_amount = total_freeze_amount + available_amount;
                        v.all_used_amount = total_used_amount;
                        v.all_available_amount = DoubleStd.IsNotDoubleThen(v.all_total_amount) - DoubleStd.IsNotDoubleThen(v.all_freeze_amount) - DoubleStd.IsNotDoubleThen(v.all_used_amount);
                        tmplist.Add(v);
                        leftamount = leftamount - available_amount;

                    }
                }
                if (leftamount > 0)
                {
                    return new { code = "failed", msg = "无可用额度" };
                }
                foreach (dynamic obj in tmplist)
                {
                    DB.QuickUpdate(up, "ecs_user_wallet_line", new
                    {
                        freeze_amount = obj.freeze_amount,
                        memo = memo,
                        last_time = DateTime.Now,
                        last_id = "WalletAPI"
                    }, new
                    {
                        user_id = obj.user_id,
                        ebs_wd_no = obj.ebs_wd_no
                    });
                }

                DB.QuickUpdate(up, "ecs_user_wallet", new
                {
                    freeze_amount = DoubleStd.IsNotDoubleThen(item.freeze_amount) + amount
                }, new
                {
                    user_id = id
                });

                dynamic data = DB.ExcuteLamda(up, from t in DB.LamdaTable("ecs_user_wallet", "a")
                                                  where t.user_id == id
                                                  select t).QueryData<FrameDLRObject>()[0];
                var newseq = GetNewSeq(ComFunc.nvl(DB.Max(up, from t in DB.LamdaTable("ecs_user_wallet_log", "a")
                                                              where t.user_id == id
                                                              select t, "seq")));
                //记录流水
                foreach (dynamic obj in tmplist)
                {
                    DB.QuickInsert(up, "ecs_user_wallet_log", new
                    {
                        user_id = id,
                        uid = Guid.NewGuid().ToString(),
                        amount = obj.current_freeze_amount,
                        events = "冻结",
                        memo = memo,
                        ebs_wd_no = obj.ebs_wd_no,
                        ebs_ou_id = obj.ebs_ou_id,
                        last_time = DateTime.Now,
                        last_id = "WalletAPI",
                        seq = newseq,
                        line_total_amount = obj.total_amount,
                        line_freeze_amount = DecimalStd.IsNotDecimalThen(obj.freeze_amount),
                        line_used_amount = DecimalStd.IsNotDecimalThen(obj.used_amount),
                        line_available_amount = obj.available_amount,
                        total_amount = obj.all_total_amount,
                        freeze_amount = DecimalStd.IsNotDecimalThen(obj.all_freeze_amount),
                        used_amount = DecimalStd.IsNotDecimalThen(obj.all_used_amount),
                        available_amount = DecimalStd.IsNotDecimalThen(obj.all_total_amount) - DecimalStd.IsNotDecimalThen(obj.all_freeze_amount) - DecimalStd.IsNotDecimalThen(obj.all_used_amount)
                    });
                    newseq = GetNewSeq(newseq);
                }

                CommitTrans();
                return new
                {
                    code = "success",
                    wallet = new
                    {
                        user_id = data.user_id,
                        current_freeze_amount = amount,
                        total_amount = data.total_amount,
                        freeze_amount = DecimalStd.IsNotDecimalThen(data.freeze_amount),
                        used_amount = DecimalStd.IsNotDecimalThen(data.used_amount),
                        available_amount = data.total_amount - DecimalStd.IsNotDecimalThen(data.freeze_amount) - DecimalStd.IsNotDecimalThen(data.used_amount),
                        lines = from t in tmplist
                                select new
                                {
                                    line_no = ((dynamic)t).line_no,
                                    ebs_wd_no = ((dynamic)t).ebs_wd_no,
                                    ebs_ou_id = ((dynamic)t).ebs_ou_id,
                                    current_freeze_amount = ((dynamic)t).current_freeze_amount,
                                    total_amount = ((dynamic)t).total_amount,
                                    freeze_amount = ((dynamic)t).freeze_amount,
                                    used_amount = ((dynamic)t).used_amount,
                                    available_amount = DoubleStd.IsNotDoubleThen(((dynamic)t).total_amount) - DoubleStd.IsNotDoubleThen(((dynamic)t).freeze_amount) - DoubleStd.IsNotDoubleThen(((dynamic)t).used_amount)
                                }

                    }
                };
            }
        }
        [EWRARoute("patch", "/wallet/{id}/cancel")]
        [EWRARouteDesc("取消冻结金额")]
        [EWRADoubleValid("amount", true, true)]
        [EWRAEmptyValid("ou_id")]
        object Cancel(string id)
        {
            lock (lockobj)
            {


                double amount = PostDataD.amount;
                var memo = ComFunc.nvl(PostDataD.memo);
                var ou_id = ComFunc.nvl(PostDataD.ou_id);
                var ebs_wd_no = ComFunc.nvl(PostDataD.ebs_wd_no);
                var up = DB.NewDBUnitParameter();
                BeginTrans();

                var s = from t in DB.LamdaTable("ecs_user_wallet", "a")
                        where t.user_id == id
                        select t;
                var result = DB.ExcuteLamda(up, s);
                if (result.QueryTable.RowLength <= 0)
                {
                    return new { code = "failed", msg = "账号无效" };
                }
                dynamic item = result.QueryData<FrameDLRObject>()[0];
                if (amount > DoubleStd.IsNotDoubleThen(item.freeze_amount))
                {
                    return new { code = "failed", msg = "金额溢出" };
                }
                //找到行资料-倒叙搜索
                var tmpamount = amount;
                var availablelist = DB.ExcuteLamda(up, from t in DB.LamdaTable("ecs_user_wallet_line", "a")
                                                       orderby t.line_no descending
                                                       where t.user_id == id && t.freeze_amount.isnull(0) > 0 && t.notnull(ou_id, t.ebs_ou_id == ou_id) && t.notnull(ebs_wd_no, t.ebs_wd_no.within($",{ebs_wd_no},"))
                                                       select t).QueryData<FrameDLRObject>();
                if (availablelist.Count <= 0)
                {
                    return new { code = "failed", msg = "无可用额度" };
                }
                var total_freeze_amount = DoubleStd.IsNotDoubleThen(item.freeze_amount);
                var total_used_amount = DoubleStd.IsNotDoubleThen(item.used_amount);
                var tmplist = new List<object>();
                foreach (dynamic obj in availablelist)
                {
                    var freeze = DoubleStd.IsNotDoubleThen(obj.freeze_amount);
                    if (tmpamount > freeze)
                    {
                        obj.freeze_amount = 0;
                        obj.current_cancel_amount = freeze;
                        obj.available_amount = DoubleStd.IsNotDoubleThen(obj.total_amount) - DoubleStd.IsNotDoubleThen(obj.freeze_amount) - DoubleStd.IsNotDoubleThen(obj.used_amount);
                        obj.all_total_amount = item.total_amount;
                        obj.all_freeze_amount = total_freeze_amount = total_freeze_amount - freeze;
                        obj.all_used_amount = total_used_amount;
                        obj.all_available_amount = DoubleStd.IsNotDoubleThen(obj.all_total_amount) - DoubleStd.IsNotDoubleThen(obj.all_freeze_amount) - DoubleStd.IsNotDoubleThen(obj.all_used_amount);
                        tmplist.Add(obj);
                        tmpamount = tmpamount - freeze;
                    }
                    else
                    {
                        obj.freeze_amount = DoubleStd.IsNotDoubleThen(obj.freeze_amount) - tmpamount;
                        obj.current_cancel_amount = tmpamount;
                        obj.available_amount = DoubleStd.IsNotDoubleThen(obj.total_amount) - DoubleStd.IsNotDoubleThen(obj.freeze_amount) - DoubleStd.IsNotDoubleThen(obj.used_amount);
                        obj.all_total_amount = item.total_amount;
                        obj.all_freeze_amount = total_freeze_amount = total_freeze_amount - tmpamount;
                        obj.all_used_amount = total_used_amount;
                        obj.all_available_amount = DoubleStd.IsNotDoubleThen(obj.all_total_amount) - DoubleStd.IsNotDoubleThen(obj.all_freeze_amount) - DoubleStd.IsNotDoubleThen(obj.all_used_amount);
                        tmplist.Add(obj);
                        tmpamount = 0;
                        break;
                    }
                }
                if (tmpamount > 0)
                {
                    return new { code = "failed", msg = "无可用额度" };
                }

                foreach (dynamic obj in tmplist)
                {
                    DB.QuickUpdate(up, "ecs_user_wallet_line", new
                    {
                        freeze_amount = obj.freeze_amount,
                        memo = memo,
                        last_time = DateTime.Now,
                        last_id = "WalletAPI"
                    }, new
                    {
                        user_id = obj.user_id,
                        ebs_wd_no = obj.ebs_wd_no
                    });
                }

                DB.QuickUpdate(up, "ecs_user_wallet", new
                {
                    freeze_amount = DoubleStd.IsNotDoubleThen(item.freeze_amount) - amount
                }, new
                {
                    user_id = id
                });

                dynamic data = DB.ExcuteLamda(up, from t in DB.LamdaTable("ecs_user_wallet", "a")
                                                  where t.user_id == id
                                                  select t).QueryData<FrameDLRObject>()[0];
                var newseq = GetNewSeq(ComFunc.nvl(DB.Max(up, from t in DB.LamdaTable("ecs_user_wallet_log", "a")
                                                              where t.user_id == id
                                                              select t, "seq")));
                //记录流水
                foreach (dynamic obj in tmplist)
                {
                    DB.QuickInsert(up, "ecs_user_wallet_log", new
                    {
                        user_id = id,
                        uid = Guid.NewGuid().ToString(),
                        amount = obj.current_cancel_amount,
                        events = "取消",
                        memo = memo,
                        ebs_wd_no = obj.ebs_wd_no,
                        ebs_ou_id = obj.ebs_ou_id,
                        last_time = DateTime.Now,
                        last_id = "WalletAPI",
                        seq = newseq,
                        line_total_amount = obj.total_amount,
                        line_freeze_amount = DecimalStd.IsNotDecimalThen(obj.freeze_amount),
                        line_used_amount = DecimalStd.IsNotDecimalThen(obj.used_amount),
                        line_available_amount = obj.available_amount,
                        total_amount = obj.all_total_amount,
                        freeze_amount = DecimalStd.IsNotDecimalThen(obj.all_freeze_amount),
                        used_amount = DecimalStd.IsNotDecimalThen(obj.all_used_amount),
                        available_amount = DecimalStd.IsNotDecimalThen(obj.all_total_amount) - DecimalStd.IsNotDecimalThen(obj.all_freeze_amount) - DecimalStd.IsNotDecimalThen(obj.all_used_amount)
                    });
                    newseq = GetNewSeq(newseq);
                }

                CommitTrans();

                return new
                {
                    code = "success",
                    wallet = new
                    {
                        user_id = data.user_id,
                        current_cancel_amount = amount,
                        total_amount = data.total_amount,
                        freeze_amount = DecimalStd.IsNotDecimalThen(data.freeze_amount),
                        used_amount = DecimalStd.IsNotDecimalThen(data.used_amount),
                        available_amount = data.total_amount - DecimalStd.IsNotDecimalThen(data.freeze_amount) - DecimalStd.IsNotDecimalThen(data.used_amount),
                        lines = from t in tmplist
                                select new
                                {
                                    line_no = ((dynamic)t).line_no,
                                    ebs_wd_no = ((dynamic)t).ebs_wd_no,
                                    ebs_ou_id = ((dynamic)t).ebs_ou_id,
                                    current_cancel_amount = ((dynamic)t).current_cancel_amount,
                                    total_amount = ((dynamic)t).total_amount,
                                    freeze_amount = ((dynamic)t).freeze_amount,
                                    used_amount = ((dynamic)t).used_amount,
                                    available_amount = DoubleStd.IsNotDoubleThen(((dynamic)t).total_amount) - DoubleStd.IsNotDoubleThen(((dynamic)t).freeze_amount) - DoubleStd.IsNotDoubleThen(((dynamic)t).used_amount)
                                }

                    }
                };
            }
        }
        [EWRARoute("patch", "/wallet/{id}/precancel")]
        [EWRARouteDesc("预取消用于获取可以取消冻结的行资料，但不会锁定这些资料")]
        [EWRADoubleValid("amount", true, true)]
        [EWRAEmptyValid("ou_id")]
        object PreCancel(string id)
        {
            lock (lockobj)
            {


                double amount = PostDataD.amount;
                var memo = ComFunc.nvl(PostDataD.memo);
                var ou_id = ComFunc.nvl(PostDataD.ou_id);
                var ebs_wd_no = ComFunc.nvl(PostDataD.ebs_wd_no);
                var up = DB.NewDBUnitParameter();
                BeginTrans();

                var s = from t in DB.LamdaTable("ecs_user_wallet", "a")
                        where t.user_id == id
                        select t;
                var result = DB.ExcuteLamda(up, s);
                if (result.QueryTable.RowLength <= 0)
                {
                    return new { code = "failed", msg = "账号无效" };
                }
                dynamic item = result.QueryData<FrameDLRObject>()[0];
                if (amount > DoubleStd.IsNotDoubleThen(item.freeze_amount))
                {
                    return new { code = "failed", msg = "金额溢出" };
                }
                //找到行资料-倒叙搜索
                var tmpamount = amount;
                var availablelist = DB.ExcuteLamda(up, from t in DB.LamdaTable("ecs_user_wallet_line", "a")
                                                       orderby t.line_no descending
                                                       where t.user_id == id && t.freeze_amount.isnull(0) > 0 && t.notnull(ou_id, t.ebs_ou_id == ou_id) && t.notnull(ebs_wd_no, t.ebs_wd_no.within($",{ebs_wd_no},"))
                                                       select t).QueryData<FrameDLRObject>();
                if (availablelist.Count <= 0)
                {
                    return new { code = "failed", msg = "无可用额度" };
                }
                var total_freeze_amount = DoubleStd.IsNotDoubleThen(item.freeze_amount);
                var total_used_amount = DoubleStd.IsNotDoubleThen(item.used_amount);
                var tmplist = new List<object>();
                foreach (dynamic obj in availablelist)
                {
                    var freeze = DoubleStd.IsNotDoubleThen(obj.freeze_amount);
                    if (tmpamount > freeze)
                    {
                        obj.freeze_amount = 0;
                        obj.current_cancel_amount = freeze;
                        obj.available_amount = DoubleStd.IsNotDoubleThen(obj.total_amount) - DoubleStd.IsNotDoubleThen(obj.freeze_amount) - DoubleStd.IsNotDoubleThen(obj.used_amount);
                        obj.all_total_amount = item.total_amount;
                        obj.all_freeze_amount = total_freeze_amount = total_freeze_amount - freeze;
                        obj.all_used_amount = total_used_amount;
                        obj.all_available_amount = DoubleStd.IsNotDoubleThen(obj.all_total_amount) - DoubleStd.IsNotDoubleThen(obj.all_freeze_amount) - DoubleStd.IsNotDoubleThen(obj.all_used_amount);
                        tmplist.Add(obj);
                        tmpamount = tmpamount - freeze;
                    }
                    else
                    {
                        obj.freeze_amount = DoubleStd.IsNotDoubleThen(obj.freeze_amount) - tmpamount;
                        obj.current_cancel_amount = tmpamount;
                        obj.available_amount = DoubleStd.IsNotDoubleThen(obj.total_amount) - DoubleStd.IsNotDoubleThen(obj.freeze_amount) - DoubleStd.IsNotDoubleThen(obj.used_amount);
                        obj.all_total_amount = item.total_amount;
                        obj.all_freeze_amount = total_freeze_amount = total_freeze_amount - tmpamount;
                        obj.all_used_amount = total_used_amount;
                        obj.all_available_amount = DoubleStd.IsNotDoubleThen(obj.all_total_amount) - DoubleStd.IsNotDoubleThen(obj.all_freeze_amount) - DoubleStd.IsNotDoubleThen(obj.all_used_amount);
                        tmplist.Add(obj);
                        tmpamount = 0;
                        break;
                    }
                }
                if (tmpamount > 0)
                {
                    return new { code = "failed", msg = "无可用额度" };
                }

                CommitTrans();

                return new
                {
                    code = "success",
                    wallet = new
                    {
                        user_id = item.user_id,
                        current_cancel_amount = amount,
                        total_amount = item.total_amount,
                        freeze_amount = DecimalStd.IsNotDecimalThen(item.freeze_amount) - DecimalStd.IsNotDecimalThen(amount),
                        used_amount = DecimalStd.IsNotDecimalThen(item.used_amount),
                        available_amount = item.total_amount - DecimalStd.IsNotDecimalThen(item.freeze_amount) - DecimalStd.IsNotDecimalThen(item.used_amount) + DecimalStd.IsNotDecimalThen(amount),
                        lines = from t in tmplist
                                select new
                                {
                                    line_no = ((dynamic)t).line_no,
                                    ebs_wd_no = ((dynamic)t).ebs_wd_no,
                                    ebs_ou_id = ((dynamic)t).ebs_ou_id,
                                    current_cancel_amount = ((dynamic)t).current_cancel_amount,
                                    total_amount = ((dynamic)t).total_amount,
                                    freeze_amount = ((dynamic)t).freeze_amount,
                                    used_amount = ((dynamic)t).used_amount,
                                    available_amount = DoubleStd.IsNotDoubleThen(((dynamic)t).total_amount) - DoubleStd.IsNotDoubleThen(((dynamic)t).freeze_amount) - DoubleStd.IsNotDoubleThen(((dynamic)t).used_amount)
                                }

                    }
                };
            }
        }
        [EWRARoute("patch", "/wallet/{id}/use")]
        [EWRARouteDesc("扣除/核销冻结的金额")]
        [EWRADoubleValid("amount", true, true)]
        [EWRAEmptyValid("ou_id")]
        object Use(string id)
        {
            lock (lockobj)
            {
                double amount = PostDataD.amount;
                var memo = ComFunc.nvl(PostDataD.memo);
                var ou_id = ComFunc.nvl(PostDataD.ou_id);
                var ebs_wd_no = ComFunc.nvl(PostDataD.ebs_wd_no);
                var up = DB.NewDBUnitParameter();
                BeginTrans();

                var s = from t in DB.LamdaTable("ecs_user_wallet", "a")
                        where t.user_id == id
                        select t;
                var result = DB.ExcuteLamda(up, s);
                if (result.QueryTable.RowLength <= 0)
                {
                    return new { code = "failed", msg = "账号无效" };
                }

                dynamic item = result.QueryData<FrameDLRObject>()[0];
                if (amount > DoubleStd.IsNotDoubleThen(item.freeze_amount))
                {
                    return new { code = "failed", msg = "金额溢出" };
                }
                //核销行资料
                var tmpamount = amount;
                //找出可用的行资料
                var availablelist = DB.ExcuteLamda(up, from t in DB.LamdaTable("ecs_user_wallet_line", "a")
                                                       orderby t.line_no ascending
                                                       where t.user_id == id && t.freeze_amount.isnull(0) > 0 && t.notnull(ou_id, t.ebs_ou_id == ou_id) && t.notnull(ebs_wd_no, t.ebs_wd_no.within($",{ebs_wd_no},"))
                                                       select t).QueryData<FrameDLRObject>();
                if (availablelist.Count <= 0)
                {
                    return new { code = "failed", msg = "无可用额度" };
                }
                var total_freeze_amount = DoubleStd.IsNotDoubleThen(item.freeze_amount);
                var total_used_amount = DoubleStd.IsNotDoubleThen(item.used_amount);
                var tmplist = new List<object>();
                foreach (dynamic obj in availablelist)
                {
                    var freeze = DoubleStd.IsNotDoubleThen(obj.freeze_amount);
                    if (tmpamount > freeze)
                    {
                        obj.freeze_amount = 0;
                        obj.used_amount = DoubleStd.IsNotDoubleThen(obj.used_amount) + freeze;
                        obj.current_use_amount = freeze;
                        obj.available_amount = DoubleStd.IsNotDoubleThen(obj.total_amount) - DoubleStd.IsNotDoubleThen(obj.freeze_amount) - DoubleStd.IsNotDoubleThen(obj.used_amount);
                        obj.all_total_amount = item.total_amount;
                        obj.all_freeze_amount = total_freeze_amount = total_freeze_amount - freeze;
                        obj.all_used_amount = total_used_amount = total_used_amount + freeze;
                        obj.all_available_amount = DoubleStd.IsNotDoubleThen(obj.all_total_amount) - DoubleStd.IsNotDoubleThen(obj.all_freeze_amount) - DoubleStd.IsNotDoubleThen(obj.all_used_amount);
                        tmplist.Add(obj);
                        tmpamount = tmpamount - freeze;
                    }
                    else
                    {
                        obj.freeze_amount = DoubleStd.IsNotDoubleThen(obj.freeze_amount) - tmpamount;
                        obj.used_amount = DoubleStd.IsNotDoubleThen(obj.used_amount) + tmpamount;
                        obj.current_use_amount = tmpamount;
                        obj.available_amount = DoubleStd.IsNotDoubleThen(obj.total_amount) - DoubleStd.IsNotDoubleThen(obj.freeze_amount) - DoubleStd.IsNotDoubleThen(obj.used_amount);
                        obj.all_total_amount = item.total_amount;
                        obj.all_freeze_amount = total_freeze_amount = total_freeze_amount - tmpamount;
                        obj.all_used_amount = total_used_amount = total_used_amount + tmpamount;
                        obj.all_available_amount = DoubleStd.IsNotDoubleThen(obj.all_total_amount) - DoubleStd.IsNotDoubleThen(obj.all_freeze_amount) - DoubleStd.IsNotDoubleThen(obj.all_used_amount);
                        tmplist.Add(obj);
                        tmpamount = 0;
                        break;
                    }
                }
                if (tmpamount > 0)
                {
                    return new { code = "failed", msg = "无可用额度" };
                }
                foreach (dynamic obj in tmplist)
                {
                    DB.QuickUpdate(up, "ecs_user_wallet_line", new
                    {
                        freeze_amount = obj.freeze_amount,
                        used_amount = obj.used_amount,
                        memo = memo,
                        last_time = DateTime.Now,
                        last_id = "WalletAPI"
                    }, new
                    {
                        user_id = obj.user_id,
                        ebs_wd_no = obj.ebs_wd_no
                    });
                }

                DB.QuickUpdate(up, "ecs_user_wallet", new
                {
                    freeze_amount = DoubleStd.IsNotDoubleThen(item.freeze_amount) - amount,
                    used_amount = DoubleStd.IsNotDoubleThen(item.used_amount) + amount
                }, new
                {
                    user_id = id
                });

                dynamic data = DB.ExcuteLamda(up, from t in DB.LamdaTable("ecs_user_wallet", "a")
                                                  where t.user_id == id
                                                  select t).QueryData<FrameDLRObject>()[0];
                var newseq = GetNewSeq(ComFunc.nvl(DB.Max(up, from t in DB.LamdaTable("ecs_user_wallet_log", "a")
                                                              where t.user_id == id
                                                              select t, "seq")));
                //记录流水
                foreach (dynamic obj in tmplist)
                {
                    DB.QuickInsert(up, "ecs_user_wallet_log", new
                    {
                        user_id = id,
                        uid = Guid.NewGuid().ToString(),
                        amount = obj.current_use_amount,
                        events = "核销",
                        memo = memo,
                        ebs_wd_no = obj.ebs_wd_no,
                        ebs_ou_id = obj.ebs_ou_id,
                        last_time = DateTime.Now,
                        last_id = "WalletAPI",
                        seq = newseq,
                        line_total_amount = obj.total_amount,
                        line_freeze_amount = DecimalStd.IsNotDecimalThen(obj.freeze_amount),
                        line_used_amount = DecimalStd.IsNotDecimalThen(obj.used_amount),
                        line_available_amount = obj.available_amount,
                        total_amount = obj.all_total_amount,
                        freeze_amount = DecimalStd.IsNotDecimalThen(obj.all_freeze_amount),
                        used_amount = DecimalStd.IsNotDecimalThen(obj.all_used_amount),
                        available_amount = DecimalStd.IsNotDecimalThen(obj.all_total_amount) - DecimalStd.IsNotDecimalThen(obj.all_freeze_amount) - DecimalStd.IsNotDecimalThen(obj.all_used_amount)
                    });
                    newseq = GetNewSeq(newseq);
                }

                CommitTrans();
                return new
                {
                    code = "success",
                    wallet = new
                    {
                        user_id = data.user_id,
                        current_freeze_amount = amount,
                        total_amount = data.total_amount,
                        freeze_amount = DecimalStd.IsNotDecimalThen(data.freeze_amount),
                        used_amount = DecimalStd.IsNotDecimalThen(data.used_amount),
                        available_amount = data.total_amount - DecimalStd.IsNotDecimalThen(data.freeze_amount) - DecimalStd.IsNotDecimalThen(data.used_amount),
                        lines = from t in tmplist
                                select new
                                {
                                    line_no = ((dynamic)t).line_no,
                                    ebs_wd_no = ((dynamic)t).ebs_wd_no,
                                    ebs_ou_id = ((dynamic)t).ebs_ou_id,
                                    current_use_amount = ((dynamic)t).current_use_amount,
                                    total_amount = ((dynamic)t).total_amount,
                                    freeze_amount = ((dynamic)t).freeze_amount,
                                    used_amount = ((dynamic)t).used_amount,
                                    available_amount = DoubleStd.IsNotDoubleThen(((dynamic)t).total_amount) - DoubleStd.IsNotDoubleThen(((dynamic)t).freeze_amount) - DoubleStd.IsNotDoubleThen(((dynamic)t).used_amount)
                                }

                    }
                };
            }
        }
        [EWRARoute("put", "/wallet/{id}")]
        [EWRARouteDesc("充值或打款")]
        [EWRADoubleValid("amount", true, true)]
        [EWRAEmptyValid("wd_no,ou_id")]
        object Charge(string id)
        {
            if (ComFunc.nvl(id) == "") return new { code = "failed", msg = "账号无效" };

            double amount = PostDataD.amount;
            var memo = ComFunc.nvl(PostDataD.memo);
            var wd_no = ComFunc.nvl(PostDataD.wd_no);
            var ou_id = ComFunc.nvl(PostDataD.ou_id);
            var up = DB.NewDBUnitParameter();
            BeginTrans();

            dynamic useritem = null;
            var s = from t in DB.LamdaTable("ecs_users", "a")
                    where t.user_id == id && t.is_validated == 1
                    select t;
            var userresult = DB.ExcuteLamda(up, s);
            if (userresult.QueryTable.RowLength <= 0)
            {
                return new { code = "failed", msg = "账号无效" };
            }
            else
            {
                useritem = userresult.QueryData<FrameDLRObject>()[0];
            }

            var result = DB.ExcuteLamda(up, from t in DB.LamdaTable("ecs_user_wallet", "a")
                                            where t.user_id == id
                                            select t);

            if (amount <= 0)
            {
                return new { code = "failed", msg = "金额不合法" };
            }
            if (DB.IsExists(up, from t in DB.LamdaTable("ecs_user_wallet_line", "a")
                                where t.ebs_wd_no == wd_no
                                select t))
            {
                return new { code = "failed", msg = "WD编号重复" };
            }



            if (result.QueryTable.RowLength <= 0)
            {
                DB.QuickInsert(up, "ecs_user_wallet", new
                {
                    user_id = id,
                    user_name = useritem.user_name,
                    total_amount = amount,
                    memo = memo,
                    add_time = DateTime.Now,
                    add_id = "WalletAPI",
                    last_time = DateTime.Now,
                    last_id = "WalletAPI"
                });
            }
            else
            {
                dynamic item = result.QueryData<FrameDLRObject>()[0];
                DB.QuickUpdate(up, "ecs_user_wallet", new
                {
                    total_amount = DoubleStd.IsNotDoubleThen(item.total_amount) + amount
                }, new
                {
                    user_id = id
                });
            }
            //写入line
            var newlineno = IntStd.IsNotIntThen(DB.Max(up, from t in DB.LamdaTable("ecs_user_wallet_line", "a")
                                                           where t.user_id == id
                                                           select t, "line_no")) + 1;
            DB.QuickInsert(up, "ecs_user_wallet_line", new
            {
                user_id = id,
                line_no = newlineno,
                ebs_wd_no = wd_no,
                ebs_ou_id = ou_id,
                total_amount = amount,
                memo = memo,
                add_time = DateTime.Now,
                add_id = "WalletAPI",
                last_time = DateTime.Now,
                last_id = "WalletAPI"
            });

            dynamic data = DB.ExcuteLamda(up, from t in DB.LamdaTable("ecs_user_wallet", "a")
                                              where t.user_id == id
                                              select t).QueryData<FrameDLRObject>()[0];

            var newseq = GetNewSeq(ComFunc.nvl(DB.Max(up, from t in DB.LamdaTable("ecs_user_wallet_log", "a")
                                                          where t.user_id == id
                                                          select t, "seq")));
            //记录流水

            DB.QuickInsert(up, "ecs_user_wallet_log", new
            {
                user_id = id,
                uid = Guid.NewGuid().ToString(),
                amount = amount,
                events = "打款",
                memo = memo,
                ebs_wd_no = wd_no,
                ebs_ou_id = ou_id,
                last_time = DateTime.Now,
                last_id = "WalletAPI",
                seq = newseq,
                line_total_amount = amount,
                line_freeze_amount = 0,
                line_used_amount = 0,
                line_available_amount = amount,
                total_amount = data.total_amount,
                freeze_amount = DecimalStd.IsNotDecimalThen(data.freeze_amount),
                used_amount = DecimalStd.IsNotDecimalThen(data.used_amount),
                available_amount = DecimalStd.IsNotDecimalThen(data.total_amount) - DecimalStd.IsNotDecimalThen(data.freeze_amount) - DecimalStd.IsNotDecimalThen(data.used_amount)
            });

            CommitTrans();
            return new
            {
                code = "success",
                wallet = new
                {
                    user_id = data.user_id,
                    total_amount = data.total_amount,
                    freeze_amount = DecimalStd.IsNotDecimalThen(data.freeze_amount),
                    used_amount = DecimalStd.IsNotDecimalThen(data.used_amount),
                    available_amount = DecimalStd.IsNotDecimalThen(data.total_amount) - DecimalStd.IsNotDecimalThen(data.freeze_amount) - DecimalStd.IsNotDecimalThen(data.used_amount)
                }
            };
        }

        private string GetNewSeq(string maxseq)
        {
            var currentfirst = DateTime.Now.ToString("yyyyMMdd") + "1".PadLeft(8, '0');
            if (maxseq == "") return currentfirst;

            if (Int64Std.IsNotInt64Then(maxseq) >= Int64Std.IsNotInt64Then(currentfirst))
            {
                return (Int64Std.IsNotInt64Then(maxseq) + 1).ToString();
            }
            else
            {
                return currentfirst;
            }

        }
        [EWRARoute("get", "/finace/detail")]
        [EWRARouteDesc("财务获取收支明细")]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("start_time", "string", "起始时间", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("end_time", "string", "结束时间", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
        log_time:'记录日期',
        name:'名称',
        amount:'提现金额',
        submit_time:'提交时间',
        status_text:'审核状态',
        audit_time:'审核时间',
        audit_explain:'审核意见'
    }]
}")]
        object GetDetailList()
        {
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable(up, "AppAccountLog", "a")
                    join t2 in DB.LamdaTable(up,"APPInfo","b") on t.app_uid equals t2.uid
                    select new
                    {
                        name = t2.name,
                        t.log_time,
                        t.amount,
                        t.action,
                        t.memo
                    };
            var s2 = from t in DB.LamdaTable(up, "ChannelAccountLog", "a")
                     join t2 in DB.LamdaTable(up, "ChannelBusiness", "b") on t.channel_uid equals t2.uid
                     select new
                     {
                         name = t2.name,
                         t.log_time,
                         t.amount,
                         t.action,
                         t.memo
                     };
            var s3 = from t in DB.LamdaTable(up, "RetailAccountLog", "a")
                     join t2 in DB.LamdaTable(up, "Retail", "b") on t.retail_uid equals t2.uid
                     select new
                     {
                         name = t2.name,
                         t.log_time,
                         t.amount,
                         t.action,
                         t.memo
                     };
            var list = s.GetUnionQueryList(up,"log_time desc", true, s2,s3);
            return new
            {
                data = list
            };
        }
    }
}
