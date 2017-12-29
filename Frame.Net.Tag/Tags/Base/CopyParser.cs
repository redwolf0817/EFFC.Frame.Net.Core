using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Tag.Tags.Base
{
    /// <summary>
    /// 将ref标记的内容拷贝到本标签位置
    /// 参数：ref_p:ref标记的名称
    ///       times：拷贝次数
    /// </summary>
    public class CopyParser:BaseTag
    {
        protected override string DoProcess(dynamic args, string content)
        {
            string ref_p = ComFunc.nvl(args.ref_p);
            int times = IntStd.IsInt(args.times) ? IntStd.ParseStd(args.times).Value : 1;
            var rtn = "";
            if (ref_p != "" && CurrentContext.GetParsedContent("ref",ref_p) != null)
            {
                for (int i = 0; i < times; i++)
                {
                    rtn += ComFunc.nvl(CurrentContext.GetParsedContent("ref", ref_p));
                }
            }
            else
            {
                rtn = "";
            }

            return rtn;
        }

        public override string TagName
        {
            get { return "copy"; }
        }

        public override string Category
        {
            get { return "base"; }
        }

        public override string[] ArgNames
        {
            get { return new string[]{"ref_p","times"}; }
        }

        public override bool IsNeedBrace
        {
            get { return false; }
        }

        public override string Description
        {
            get { return @" 将ref标记的内容拷贝到本标签位置
参数：ref_p:ref标记的名称
      times：拷贝次数"; }
        }
    }
}
