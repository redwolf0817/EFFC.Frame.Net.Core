using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Tag.Datas
{
    public class TagData : DataCollection
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
