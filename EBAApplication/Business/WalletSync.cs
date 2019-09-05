using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.EBA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Module.Extend.EBA;

namespace B2B_BatchApplication.Business
{
    public class WalletSync : ScheduleLogic
    {
        /// <summary>
        /// 客户打款数据同步 
        /// 查询 ECS_ForPay表 status = Create 的资料,
        /// 通过ou_id call ebsAPI 捞取数据S的资料 
        /// 比对资料,改status状态为 reviewed.
        /// 再call api将金额写入B2B钱包表(ecs_user_wallet)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [EBAFrequence(EBAFrequenceAttribute.FrequenceType.Minute,10)]
        [EBADesc("呼叫ESB查询打款审批情况后更新钱包")]
        [EBAGroup("wallet","钱包功能使用")]
        object SyncEsbForPay(dynamic args)
        {
            //GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] SyncEsbForPay====================开始====================.");
            //var up = DB.NewB2BDBUnitParameter();
            //EBSRestCall esb = new EBSRestCall();            // call esb的程序
            //WalletRestCall wallet = new WalletRestCall();   // call 钱包的程序
            //B2BRestCall b2b = new B2BRestCall();

            //up.SetValue("status", "Review");
            //// 得到所有为Review的ecs_forpay表的 资料
            //List<dynamic> list = DB.Query<PtacShopUnit>(up, "GetCreatedForPay").QueryData<dynamic>();
            //// 得到所有为Review的ouid
            //List<dynamic> ouids = DB.Query<PtacShopUnit>(up, "GetDistinctOuidForPay").QueryData<dynamic>();

            //int idLength = ouids.Count;
            //for (int i = 0; i < ouids.Count; i++)
            //{
            //    // 1.逐笔ouid 呼叫ESB取得审批情况
            //    string ouid = ComFunc.nvl(ouids[i].ou_id);
            //    GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}) 开始同步.");

            //    #region Call ESB
            //    // Calll ESB
            //    dynamic esbRtn = esb.GetESBStatus(ouid);
            //    GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}) 返回{((FrameDLRObject)esbRtn).ToJSONString()}.");
            //    // 连接服务器出错
            //    if (!string.IsNullOrEmpty(ComFunc.nvl(esbRtn.error)))
            //    {
            //        GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); 跳过,错误消息:{ComFunc.nvl(esbRtn.error)}.");
            //        continue;
            //    }
            //    // 服务器返回错误
            //    if (ComFunc.nvl(esbRtn.result.code) != "success")
            //    {
            //        GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); 跳过,服务器错误消息:{ComFunc.nvl(esbRtn.result.code)}-{ComFunc.nvl(esbRtn.result.msg)}.");
            //        continue;
            //    }
            //    #endregion

            //    List<dynamic> data = esbRtn.result.data;
            //    int dataLength = data.Count;
            //    GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); 下属资料{dataLength}笔.");

            //    for (int j = 0; j < dataLength; j++)
            //    {
            //        string recordId = ComFunc.nvl(data[j].record_id);           // 对应ecs_forpay.forpay_no
            //        string status = ComFunc.nvl(data[j].recipt_create_flag).ToUpper();     // N-新建,E-错误,S-成功
            //        string wdno = ComFunc.nvl(data[j].cash_receipt_id);
            //        decimal ebsAmount = DecimalStd.IsNotDecimalThen(data[j].credit_amount);
            //        #region 判断合法性
            //        // 判断是否是成功失败状态
            //        if (status != "SUCCESS" && status != "CHECK_ERROR")
            //        {
            //            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); record_id: {recordId}({j + 1}/{dataLength}); 跳过,返回状态{status}不属于SUCCESS或CHECK_ERROR.");
            //            continue;
            //        }

            //        // 判断ecs_forpay表中是否存在
            //        var obj = list.FirstOrDefault(m => m.forpay_no == recordId);
            //        if (obj == null)
            //        {
            //            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); record_id: {recordId}({j+1}/{dataLength}); 跳过,在ecs_forpay表不存在或状态不为review.");
            //            continue;
            //        }
            //        // 判断金额是否相等
            //        Decimal.TryParse(ComFunc.nvl(obj.charge_amount), out decimal ecsAmount);
            //        if (!ebsAmount.Equals(ecsAmount))
            //        {
            //            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); record_id: {recordId}({j + 1}/{dataLength}); 跳过,DB中金额为{ecsAmount},服务器返回{ebsAmount},两者不一致.");
            //            continue;
            //        }
            //        #endregion

            //        // 2.全部验证通过, 呼叫钱包API
            //        if (status == "SUCCESS")
            //        {
            //            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); record_id: {recordId}({j + 1}/{dataLength}); 开始呼叫钱包.");

            //            //如果成功 就call钱包API打款 
            //            dynamic walletRtn = wallet.chargMoney(ComFunc.nvl(obj.user_id), ebsAmount, ComFunc.nvl(obj.memo), wdno, ouid);
            //            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); record_id: {recordId}({j + 1}/{dataLength}); 钱包服务器成功:{((FrameDLRObject)walletRtn).ToJSONString()}.");

            //            // 连接服务器出错
            //            if (!string.IsNullOrEmpty(ComFunc.nvl(walletRtn.error)))
            //            {
            //                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); record_id: {recordId}({j + 1}/{dataLength}); 呼叫钱包错误{ComFunc.nvl(walletRtn.error)}.");
            //                continue;
            //            }
            //            // 服务器返回错误
            //            if (ComFunc.nvl(esbRtn.result.code) != "success")
            //            {
            //                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); record_id: {recordId}({j + 1}/{dataLength}); 钱包服务器错误:{((FrameDLRObject)walletRtn).ToJSONString()}.");
            //                continue;
            //            }


            //            // 3.更新DB ecs_forpay表状态
            //            up.ClearParameters();
            //            up.SetValue("uid", obj.uid);
            //            up.SetValue("newstatus", "Success");
            //            up.SetValue("wd_no", wdno);
            //            up.SetValue("oldstatus", "Review");
            //            DB.NonQuery<PtacShopUnit>(up, "UpdateForPayStatus");
            //            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); record_id: {recordId}({j + 1}/{dataLength}); 更新ecs_forpay表完成.");
            //        }
            //        else
            //        {   // CHECK_ERROR
            //            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); record_id: {recordId}({j + 1}/{dataLength}); 无需呼叫钱包.");

            //            // 3.更新DB ecs_forpay表状态
            //            up.ClearParameters();
            //            up.SetValue("uid", obj.uid);
            //            up.SetValue("newstatus", "Failed");
            //            up.SetValue("check_msg", ComFunc.nvl(data[j].error_message));
            //            up.SetValue("oldstatus", "Review");
            //            DB.NonQuery<PtacShopUnit>(up, "UpdateForPayStatus");
            //            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] ou_id:{ouid}({i + 1}/{idLength}); record_id: {recordId}({j + 1}/{dataLength}); 更新ecs_forpay表完成.");
            //        }

                    
            //    }
            //}
            ////直接更新财务直接打款的记录，同步到客户钱包里面
            //GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] 开始同步财务直接打款的记录.");
            //var ou_ids = "2491";
            ////#839 鉴于钱包项目中所有商品均有普泰销售，现阶段写死银行信息，不从907银行账户中取列表，相关代码注释备用。
            ////dynamic ouresult = b2b.GetB2BSync();
            ////if(ouresult == null || ouresult.code != "success")
            ////{
            ////    GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}]错误消息:{ComFunc.nvl(ouresult.msg)}.");
            ////    return new
            ////    {
            ////        code = "failed",
            ////        msg = ouresult.msg
            ////    };
            ////}

            ////FrameDLRObject oulist = ouresult.data;
            ////foreach(var k in oulist.Keys)
            ////{
            ////    ou_ids += "," + k;
            ////}
            ////抓取最近两天的打款记录
            //dynamic callresult = esb.GetUnApplyList(ou_ids, "CLEARED",DateTime.Now.AddDays(-1).ToString("yyyyMMdd"));
            //GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $@"[{DateTime.Now}]呼叫EBS获取客户未核销完毕收款信息出错，返回{((FrameDLRObject)callresult).ToJSONString()}.");
            //if (!string.IsNullOrEmpty(ComFunc.nvl(callresult.error)))
            //{
            //    GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}]呼叫EBS获取客户未核销完毕收款信息出错，错误消息:{ComFunc.nvl(callresult.error)}.");
            //    return new
            //    {
            //        code = "failed",
            //        msg = callresult.error
            //    };
            //}
            //if (callresult.result.code != "success")
            //{
            //    GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}]呼叫EBS获取客户未核销完毕收款信息失败，提示消息:{ComFunc.nvl(callresult.result.msg)}.");
            //    return new
            //    {
            //        code = "failed",
            //        msg = callresult.result.msg
            //    };
            //}
            //list = callresult.result.data;
            //var chargelist = from t in list
            //                 where t.status == "CLEARED"
            //                 select new
            //                 {
            //                     ou_id = t.ou_id,
            //                     ebs_customer_number = t.account_number,
            //                     ebs_wd_no = t.cash_receipt_id,
            //                     amount = t.unapplied_amount,
            //                     comments = t.comments
            //                 };
            //GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"合计财务打款记录{chargelist.Count()}笔.");
            //var index = 0;
            //idLength = chargelist.Count();
            //foreach (var item in chargelist)
            //{
            //    index++;
            //    GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"开始处理第{index}笔,客户EBS编号{ComFunc.nvl(item.ebs_customer_number)}");
            //    var s = from t in DB.MySqlLamdaTable("ecs_users", "a")
            //            where t.customer_num == item.ebs_customer_number
            //            select t.user_id;
            //    var result = DB.ExcuteLamda(up, s);
            //    if(result.QueryTable.RowLength <= 0 || ComFunc.nvl(result.QueryTable[0, "user_id"]) == "")
            //    {
            //        GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"EBS客户编号为{item.ebs_customer_number}在B2B系统中不存在，故不做打款处理.");
            //        continue;
            //    }
            //    var user_id = ComFunc.nvl(result.QueryTable[0, "user_id"]);
            //    dynamic walletRtn = wallet.chargMoney(user_id, DecimalStd.IsNotDecimalThen(item.amount), ComFunc.nvl(item.comments), ComFunc.nvl(item.ebs_wd_no), ComFunc.nvl(item.ou_id));
            //    GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $@"{index}/{idLength}， 钱包服务器调用回传信息:{((FrameDLRObject)walletRtn).ToJSONString()}.");

            //    // 连接服务器出错
            //    if (!string.IsNullOrEmpty(ComFunc.nvl(walletRtn.error)))
            //    {
            //        GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"{index}/{idLength}， 呼叫钱包错误{ComFunc.nvl(walletRtn.error)}.");
            //        continue;
            //    }
            //    // 服务器返回错误
            //    if (ComFunc.nvl(walletRtn.result.code) != "success")
            //    {
            //        GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"{index}/{idLength}，钱包服务器错误:{((FrameDLRObject)walletRtn).ToJSONString()}.");
            //        continue;
            //    }

            //    GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"结束处理第{index}笔.");
                
            //}

            //GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] 结束财务直接打款的记录.");

            //GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] SyncEsbForPay====================结束====================.");
            return true;
        }


        [EBAFrequence(EBAFrequenceAttribute.FrequenceType.Minute, 10)]
        [EBADesc("客户核销同步")]
        [EBAGroup("wallet", "钱包功能使用")]
        object SyncForWallet(dynamic args)
        {
           // GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] SyncB2BForWallet====================开始====================.");
           // B2BRestCall b2b = new B2BRestCall();            // call b2b的程序
           // EBSRestCall esb = new EBSRestCall();            // call esb的程序
           ////调用B2B的接口 获取 ou_id 
           //   // Calll ESB
           // dynamic b2bRtn = b2b.GetB2BSync();
           // GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $@"[{DateTime.Now}]返回{((FrameDLRObject)b2bRtn).ToJSONString()}.");
           // // 连接服务器出错
           // if (!string.IsNullOrEmpty(ComFunc.nvl(b2bRtn.error)))
           // {
           //     GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}]错误消息:{ComFunc.nvl(b2bRtn.error)}.");
           //     return new
           //     {
           //         code = "failed",
           //         msg = b2bRtn.error
           //     };
           // }
           // List< KeyValuePair<string,object>> data = ((FrameDLRObject)b2bRtn.data).Items;
           // GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}]; 下属资料{data.Count}笔.");
           // List<string> strouidList = new List<string>();
           // foreach (dynamic item in data )
           // {
           //   strouidList.Add(ComFunc.nvl(item.Key));//循环添加元素  // 对应ecs_forpay.forpay_no
           // }
           // strouidList = strouidList.Distinct().ToList();//去重复
                                                        
           // for (int i = 0; i < strouidList.Count; i++)
           // {

           //     string ouid = ComFunc.nvl(strouidList[i]);
           //     dynamic esbRtn = null;
           //     dynamic rtnlist = FrameDLRObject.CreateInstance();
           //     //循环掉接口
           //     var topage = 1;
           //     esbRtn = esb.GetMatchList(ouid, DateTime.Now.AddDays(-1).ToString("yyyyMMdd"));
           //     GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $@"[{DateTime.Now}] ou_id:{strouidList[i]} 返回{((FrameDLRObject)esbRtn).ToJSONString()}.");
           //     if (!string.IsNullOrEmpty(ComFunc.nvl(esbRtn.error)))
           //     {
           //         GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}]错误消息:{ComFunc.nvl(esbRtn.error)}.");
           //         return new
           //         {
           //             code = "failed",
           //             msg = esbRtn.error
           //         };
           //     }
           //     if (IntStd.IsNotIntThen(esbRtn.result.totalRecord, 0) > 0)
           //     {
           //         rtnlist.AddRange(esbRtn.result.data);
           //     }
           //     // 连接服务器出错
              
           //     Send(esbRtn, ouid);
           // }
           // GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}] SyncB2BForWallet====================结束====================.");
            return true;
        }

        public object Send(dynamic esbRtn,string ouid)
        {
            //var up = DB.NewB2BDBUnitParameter();
            //var localup = DB.NewDBUnitParameter();
            //WalletRestCall wallet = new WalletRestCall();   // call 钱包的程序
            //var list = (from t in DB.Query<PtacShopUnit>(up, "GetEcsUsers").QueryData<FrameDLRObject>()
            //     select new
            //     {
            //         key = t.customer_num,
            //         value = t.user_id
            //     }).ToDictionary(p=>p.key,p=>p.value);
            //var logresult = DB.Query<PtacShopUnit>(localup, "GetMatchLog");
            //var loglist = logresult.QueryData<FrameDLRObject>();
            //esbRtn.result.data.AddRange(loglist); //把之前失败的也重新核销一次

            //foreach (dynamic item in esbRtn.result.data)
            //{
            //    var esbuserid = ComFunc.nvl(item.cust_acc_num);
            //    var userid = ComFunc.nvl(list[esbuserid]);
            //    decimal amount = DecimalStd.IsNotDecimalThen(item.amount_applied);
            //    var receiptid = ComFunc.nvl(item.cash_receipt_id);//收款ID
            //    var applicationid = ComFunc.nvl(item.receivable_application_id);//流水id 
            //    GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $@"开始核销esbuserid:{esbuserid},userid:{userid},amount:{amount},receiptid:{receiptid},applicationid:{applicationid}.");
            //    //调用钱包接口
            //    dynamic walletRtn = wallet.macthMoney(ComFunc.nvl(userid), amount, "", ouid, receiptid, applicationid);
            //    if (!string.IsNullOrEmpty(ComFunc.nvl(walletRtn.error)))
            //    {
            //        GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}]呼叫钱包错误{ComFunc.nvl(walletRtn.error)}.");
            //        continue;
            //    }
            //    else if (ComFunc.nvl(walletRtn.result.code) != "success")
            //    {
            //        GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $@"[{DateTime.Now}]呼叫钱包错误{ComFunc.nvl(walletRtn.result.msg)}.");
            //        continue;
            //    }
            //    else
            //    {
            //        var flag = ComFunc.nvl(walletRtn.result.code) != "success" ? "1" : "0";//失败是1 成功是0 
            //        var newlist = FrameDLRObject.CreateInstance();
            //        newlist.uid = Guid.NewGuid().ToString();
            //        newlist.userid = userid;
            //        newlist.cust_acc_num = amount;
            //        newlist.cash_receipt_id = receiptid;
            //        newlist.receivable_application_id = applicationid;
            //        newlist.stauts = flag;
            //        newlist.add_time = DateTime.Now.ToString("yyyyMMddHHmmss");

            //        DB.QuickUpdate(localup, "match_log", newlist, new
            //        {
            //            receivable_application_id = newlist.applicationid
            //        });

            //        DB.QuickInsertNotExists(localup, "match_log", newlist, new { receivable_application_id = newlist.applicationid });
            //        //要么成功 要么失败
            //    }
            //}
            return true;
        }
    }
}
