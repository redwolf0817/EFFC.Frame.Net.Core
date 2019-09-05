using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPISample.Business.v1._0
{
    public class AuthPage:MyRestLogic
    {
        [EWRAVisible(true)]
        [EWRAAuth(false)]
        [EWRARoute("get", "/auth_page")]
        [EWRARouteDesc("创建微信公众号菜单")]
        [EWRAAddInput("tkey", "string", "跳转系统key", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAOutputDesc("返回结果", @"一段html页面")]
        public object AuthTo()
        {
            SetCacheEnable(false);
            SetContentType(EFFC.Frame.Net.Module.Extend.EWRA.Constants.RestContentType.HTML);
            var tkey = ComFunc.nvl(QueryStringD.GetValue("tkey"));
            var rurl = $"{ServerInfo.SiteHostUrl}/weixinhome.pageauthprocess.wx";
            var tourl = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_userinfo&state={2}#wechat_redirect", "", ComFunc.UrlEncode(rurl), tkey);
            var html = string.Format(@"<script>
location.href = '{0}';
</script>", tourl);
            RedirectTo(tourl);
            return html;
        }
    }
}
