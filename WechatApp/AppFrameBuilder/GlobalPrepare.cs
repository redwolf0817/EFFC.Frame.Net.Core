using EFFC.Frame.Net.Module.Web.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WechatApp.AppFrameBuilder
{
    public class GlobalPrepare
    {
        static List<string> _ignorelist = null;
        /// <summary>
        /// 判断当前请求是否忽略登录验证
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsIgnoreLoginAuth(WebParameter p)
        {
            if(_ignorelist == null)
            {
                _ignorelist = new List<string>();
                _ignorelist.Add("admin");
                _ignorelist.Add("admin/login");
                _ignorelist.Add("admin/logout");
            }
            var s = p.RequestResourceName + (p.Action == "" ? "" : $"/{p.Action}");
            if (!_ignorelist.Contains(s.ToLower())
                && !_ignorelist.Contains($"{p.RequestResourceName.ToLower()}/*"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
