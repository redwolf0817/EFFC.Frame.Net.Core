using EFFC.Frame.Net.Tag.Tags.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Web.ViewEngine
{
    /// <summary>
    /// Hjs标签
    /// </summary>
    public class HjsTag : BaseTag
    {
        public override string[] ArgNames
        {
            get { return new string[] { }; }
        }

        public override string Category
        {
            get { return "hostview"; }
        }

        protected override string DoProcess(dynamic args, string content)
        {
            var uid = Guid.NewGuid().ToString();
            CurrentContext.SetParsedContent(this, uid, content);
            return "#" + TagName + uid + "#";
        }

        public override bool IsNeedBrace
        {
            get { return true; }
        }

        public override string TagName
        {
            get { return "hjs"; }
        }

        public override string Description
        {
            get { return "将标签中的内容直接标记为hjs脚本内容，在编译时直接作为可执行的内容"; }
        }
    }
    /// <summary>
    /// out标签
    /// </summary>
    public class OutTag : BaseTag
    {
        public override string[] ArgNames
        {
            get
            {
                var rtn = new string[]{
                    "value"
                };
                return rtn;
            }
        }

        public override string Category
        {
            get { return "hostview"; }
        }

        protected override string DoProcess(dynamic args, string content)
        {
            return @"#hjs{viewdoc.write(" + args.value + @");}";
        }

        public override bool IsNeedBrace
        {
            get { return false; }
        }

        public override string TagName
        {
            get { return "out"; }
        }

        public override string Description
        {
            get { return "将标签中的参数只作为输出识别，该标签会转变为#hjs{viewdoc.write(参数);}"; }
        }
    }
}
