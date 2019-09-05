using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Unit.DB;

namespace RestAPISample.Business.v1._0
{
    public class WeixinMP:MyRestLogic
    {
        [EWRAVisible(true)]
        [EWRAAuth(true)]
        [EWRAEmptyValid("user_info")]
        [EWRARoute("post", "/WeixinMP/validuserinfo")]
        [EWRARouteDesc("校验加密用户细心耐心，并获取用户的个人资料信息")]
        [EWRAAddInput("user_info", "string", "待校验用户信息，来自于前端微信小程序请求wx.getUserInfo后的结果,数据结构参考https://developers.weixin.qq.com/miniprogram/dev/api/wx.getUserInfo.html中的Object res说明", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
data:{
        nickName:'用户微信昵称',
        gender: '性别，值为1时是男性，值为2时是女性，值为0时是未知',
        city: '所在城市',
        province: '所在省份',
        country: '所在国家',
        avatarUrl: '头像url',
        其它扩展属性
    }
}")]
        object ValidUserInfo()
        {
            var userinfo = PostDataD.user_info;
            string raw_data = ComFunc.nvl(userinfo.rawData);
            string signature = ComFunc.nvl(userinfo.signature);
            string encryptedData = ComFunc.nvl(userinfo.encryptedData);
            string iv = ComFunc.nvl(userinfo.iv);

            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable(up, "WeixinMP_SessionKey", "a")
                    where t.UserID == TokenPayLoad["user_id"]
                    select t;
            var list = s.GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "缺少登录用户信息"
                };
            }
            dynamic info = list.First();
            var session_key = info.SessionKey;
            dynamic result = WeixinMP.GetUserInfo(session_key, raw_data, signature, encryptedData, iv);
            if (result == null)
            {
                return new
                {
                    code = "failed",
                    msg = "无效的数据"
                };
            }
            return new
            {
                code = "success",
                msg = "",
                data = result
            };
        }
    }
}
