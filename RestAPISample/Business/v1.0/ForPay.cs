using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using EFFC.Frame.Net.Module.Business.Logic;

namespace RestAPISample.Business.v1._0
{
    public class ForPay : MyRestLogic
    {
        [EWRARoute("get", "/forpay")]
        [EWRARouteDesc("执行翻页查询操作")]
        object Search()
        {
            SetCacheEnable(false);
            var no = ComFunc.nvl(QueryStringD.no);
            var userno = ComFunc.nvl(QueryStringD.userno);
            var status = ComFunc.nvl(QueryStringD.status);
            var start_time = DateTimeStd.IsDateTimeThen(QueryStringD.start_time, "yyyy-MM-dd HH:mm:ss");
            var end_time = DateTimeStd.IsDateTimeThen(QueryStringD.end_time, "yyyy-MM-dd HH:mm:ss");
            var sortcolumn = ComFunc.nvl(QueryStringD.sortcolumn);
            var sort = ComFunc.nvl(QueryStringD.sort);
            //当前页超过末页时，自动显示最后一页的资料
            DB.Is_Auto_To_Last_Page = true;
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable("ecs_forpay", "a")
                    join t2 in DB.LamdaTable("ecs_users", "b") on t.user_id equals t2.user_id
                    where t.notnull(no, t.forpay_no.contains(no))
                        && t2.notnull(userno, (t2.user_name.contains(userno) || t2.company_name.contains(userno) || t2.customer_num.contains(userno)))
                        && t.notnull(status, t.status == status)
                        && t.notnull(start_time, t.add_time >= start_time)
                        && t.notnull(end_time, t.add_time <= end_time)
                    select new
                    {
                        t.uid,
                        t.forpay_no,
                        t2.company_name,
                        t.foruse,
                        t.charge_amount,
                        t.charge_type,
                        t.bank_name,
                        t.bank_no,
                        t.trans_seq_no,
                        t.add_time,
                        t.status,
                        t.uploadfile_name
                    };
            var sortstr = "forpay_no desc";
            var sortrtn = "";
            var dicsort = new Dictionary<string, string>();
            dicsort.Add("seq_no", "forpay_no");
            dicsort.Add("company_name", "company_name");
            dicsort.Add("add_time", "add_time");
            dicsort.Add("foruse", "foruse");
            dicsort.Add("charge_type", "charge_type");
            dicsort.Add("charge_amount", "charge_amount");
            dicsort.Add("bank_name", "bank_name");
            dicsort.Add("bank_no", "bank_no");
            dicsort.Add("trans_seq_no", "trans_seq_no");
            dicsort.Add("status", "status");
            if (sortcolumn != "" && dicsort.ContainsKey(sortcolumn))
            {
                sortstr = dicsort[sortcolumn] + (sort == "" ? "" : " " + sort);
                sortrtn = sortcolumn + "#" + (sort == "" ? "asc" : sort);
            }
            var result = DB.LamdaQueryByPage(up, s, sortstr);
            var dic = new Dictionary<string, string>();
            dic.Add("Create", "待审核");
            dic.Add("Review", "已审核");
            dic.Add("Success", "充值成功");
            dic.Add("Failed", "充值失败");
            return new
            {
                code = "success",
                totalrow = result.TotalRow,
                totalpage = result.TotalPage,
                currentpage = result.CurrentPage,
                pagesize = result.Count_Of_OnePage,
                data = from t in result.QueryData<FrameDLRObject>()
                       select new
                       {
                           t.uid,
                           seq_no = t.forpay_no,
                           charge_amount = "￥" + DecimalStd.IsNotDecimalThen(t.charge_amount).ToString("###,###.##"),
                           t.charge_type,
                           t.bank_name,
                           t.bank_no,
                           t.trans_seq_no,
                           t.company_name,
                           t.foruse,
                           add_time = DateTimeStd.IsDateTimeThen(t.add_time, "yyyy-MM-dd HH:mm:ss"),
                           status = dic.ContainsKey(t.status) ? dic[t.status] : "",
                           filename = t.uploadfile_name
                       },
                sort = sortrtn

            };

        }
        [EWRARoute("patch", "/forpay")]
        [EWRARouteDesc("执行审核操作")]
        [EWRAEmptyValid("nos")]
        object Pass()
        {
            var uids = ComFunc.nvl(PostDataD.nos);

            var up = DB.NewDBUnitParameter();
            var upebs = DB.NewDBUnitParameter4EBS();
            var exists_s = from t in DB.LamdaTable("ecs_forpay", "a")
                            where t.uid.within("," + uids + ",") && t.status == "Review"
                            select t;
            var exists = DB.IsExists(up, exists_s);
            if (DB.IsExists(up, exists_s)) return "已审核的资料不可再做审核操作";
            BeginTrans();
            var s = from t in DB.LamdaTable("ecs_forpay", "a")
                    join t2 in DB.LamdaTable("ecs_users", "b") on t.user_id equals t2.user_id
                    where t.uid.within("," + uids + ",")
                    select new
                    {
                        t.user_id,
                        t.uid,
                        t.forpay_no,
                        t.foruse,
                        t.charge_amount,
                        t.charge_type,
                        t.bank_name,
                        t.bank_no,
                        t.opp_acc_name,
                        t.trans_seq_no,
                        t.ou_id,
                        t.memo,
                        t.bank_acc_id,
                        t.bank_acc_no,
                        t.add_time,
                        t2.customer_num
                    };
            var list = DB.ExcuteLamda(up, s).QueryData<FrameDLRObject>().Select((t)=>
            {
                string charge_type = "";
                if(t.charge_type == "现金")
                {
                    charge_type = "CASH";
                }else if(t.charge_type == "银行转账")
                {
                    charge_type = "BANK_TRANSFER";
                }
                return new
                {
                    record_id = t.forpay_no,
                    ou_id = t.ou_id,
                    cust_num = t.customer_num,
                    recharge_type = charge_type,
                    bank_acc_id = t.bank_acc_id,
                    bank_acc_num = t.bank_acc_no,
                    debit_amount = 0,
                    credit_amount = t.charge_amount,
                    currency = "CNY",
                    opp_date = t.add_time,
                    opp_num = t.trans_seq_no,
                    explanation = t.memo,
                    opp_acc_bank = t.bank_name,
                    opp_acc_name = t.opp_acc_name,
                    opp_acc_num = t.bank_no,
                    recipt_create_flag = "N"
                };
            });
            foreach(var item in list)
            {
                DB.QuickInsert(upebs, "CUX.CUX_B2B_CUST_RECIPT_CLAIM_INT", item);
            }
            

            DB.Excute(up, FrameDLRObject.CreateInstance(@"{
$acttype : 'Update',
$table:'ecs_forpay',
status:'Review',
$where:{
        'locate(uid,\'" + uids + @"\')':{
            $gt:0
        }
    }
}"));
            CommitTrans();
            return "success";
        }
        [EWRARoute("get", "/forpay/{id}/file")]
        [EWRARouteDesc("执行审核操作")]
        object Download(string id)
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable("ecs_forpay", "a")
                    where t.uid == id
                    select t;
            var result = DB.ExcuteLamda(up, s);
            if (!IsValidBy("查无资料", () =>
            {
                return result.QueryTable.RowLength > 0;
            }))
            {
                return null;
            }

            dynamic fileinfo = result.QueryData<FrameDLRObject>()[0];
            return new
            {
                code = "success",
                filename = fileinfo.uploadfile_name,
                filetype = fileinfo.uploadfile_type,
                filesize = fileinfo.uploadfile_size,
                filecontent = ComFunc.UrlEncode(fileinfo.uploadfile)
            };

        }
    }
}
