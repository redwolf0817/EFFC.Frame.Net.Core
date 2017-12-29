using EFFC.Frame.Net.Base.AttributeDefine;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.ResouceManage.JsEngine;
using EFFC.Frame.Net.Tag.Core;
using EFFC.Frame.Net.Tag.Module;
using EFFC.Frame.Net.Tag.Tags.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Engine
{
    /// <summary>
    /// View文档
    /// </summary>
    public class ViewDocument:BaseHostJsObject
    {
        StringBuilder sb = new StringBuilder();
        HostJs jse = null;
        public ViewDocument(HostJs hostjs)
        {
            jse = hostjs;
        }
        [Desc("写入一段文本")]
        public void write(string text)
        {
            if (text != null)
                sb.Append(ComFunc.HTMLDecode(text.Replace("@@@", "&")));
        }
        [Desc("写入一段文本并换行")]
        public void writeLine(string text)
        {
            if (text != null)
            {
                var tmp = text.Replace("\r", "");
                var strarr = tmp.Split('\n');
                foreach (var s in strarr)
                {
                    sb.AppendLine(ComFunc.HTMLDecode(s.Replace("@@@", "&")));
                }
            }
        }
        [Desc("输出HTML")]
        public string OutHtml()
        {
            return sb.ToString();
        }
        public override string Description
        {
            get { return "Host Js View引擎用到的文本对象"; }
        }

        public override string Name
        {
            get { return "viewdoc"; }
        }
    }
}
