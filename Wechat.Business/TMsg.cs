using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Extends.LinqDLR2SQL;

namespace Wechat.Business
{
    public class TMsg : MyBaseLogic
    {
        public object SendBatchNotice(LogicData arg)
        {
            var touser = ComFunc.nvl(arg["touser"]);
            var remark = ComFunc.nvl(arg["remark"]);
            var taskname = ComFunc.nvl(arg["taskname"]);
            var starttime = DateTimeStd.IsDateTimeThen(arg["starttime"], "yyyy-MM-dd HH:mm:ss");
            var endtime = DateTimeStd.IsDateTimeThen(arg["endtime"], "yyyy-MM-dd HH:mm:ss");
            var resultdesc = ComFunc.nvl(arg["result"]);
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable("user_info", "")
                    where t.userid == touser || t.UserName == touser || t.weixinid == touser
                    select t;
            var result = DB.ExcuteLamda(up, s);
            if (result.QueryTable.RowLength <= 0)
            {
                return new
                {
                    issuccess = false,
                    msg = "用户不存在"
                };
            }
            touser = ComFunc.nvl(result.QueryTable[0, "weixinid"]);

            //var sendresult = Weixin.SendTemplateMsg(touser, "qQvNxlTbAtnkN7dM6M3-Wk0BRMLNg1f-pghYNcn3VVw", "", "", "",
            //    "批次执行通知", remark, taskname, starttime, endtime, resultdesc);

            return "";

        }
    }
}
