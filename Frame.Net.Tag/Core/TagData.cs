using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;

namespace EFFC.Frame.Net.Tag.Core
{
    public class TagData:DataCollection
    {
        TagContext _content = new TagContext();
        /// <summary>
        /// 解析完成之后的结果对象
        /// </summary>
        public string ParsedText
        {
            get
            {
                if (GetValue("result", "obj") != null)
                    return ComFunc.nvl(GetValue("result", "obj"));
                else
                    return "";
            }
            set
            {
                SetValue("result", "obj", value);
            }
        }
        /// <summary>
        /// 标签解析的上下文
        /// </summary>
        public TagContext Context
        {
            get
            {
                return _content;
            }
        }
    }
}
