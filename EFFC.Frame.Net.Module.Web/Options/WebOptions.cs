using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Web.Options
{
    /// <summary>
    /// Web模块调用时全局设置参数
    /// </summary>
    internal class WebOptions
    {
        public WebOptions()
        {
            StartPage = "index.go";
            RootHome = "~/";
        }
        /// <summary>
        /// Web启动时的起始页面
        /// </summary>
        public string StartPage
        {
            get;
            set;
        }
        /// <summary>
        /// 请求路径中根路径的映射关系，形如：~/root/,则表明 http|https://xxxx/root/{Resource}，视为本处理进行处理，xxxx/root/为请求的根路径，后面为请求的资源
        /// 默认为“~/”
        /// </summary>
        public string RootHome
        {
            get;
            set;
        }
    }
}
