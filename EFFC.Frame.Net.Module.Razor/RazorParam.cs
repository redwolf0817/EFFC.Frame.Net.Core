using EFFC.Frame.Net.Base.Parameter;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Razor
{
    public class RazorParam:ParameterStd
    {
        /// <summary>
        /// 准备渲染的cshtml文件路径
        /// </summary>
        public string ViewPath { get; set; }
        /// <summary>
        /// 当前http上下文环境
        /// </summary>
        public HttpContext CurrentHttpContext { get; set; }
        /// <summary>
        /// 数据模型
        /// </summary>
        public object Model { get; set; }
        /// <summary>
        /// ViewData集合
        /// </summary>
        public Dictionary<string,object> ViewList { get; set; }
    }
}
