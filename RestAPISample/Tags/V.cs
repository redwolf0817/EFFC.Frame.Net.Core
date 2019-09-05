using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Tag.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPISample.Tags
{
    public class V : BaseTag
    {
        public override string TagName => "v";

        public override string Category => "Data";

        public override string[] ArgNames => new string[] { "key" };

        public override bool IsNeedBrace => false;

        public override string Description => "数据绑定标签";
        public override int Priority => 999;


        protected override string DoProcess(dynamic args, string content)
        {
            string bindname = ComFunc.nvl(args.key);
            if (bindname == "") return "";

            var keys = bindname.Split('.');
            object value = CurrentContext.GetBindObject(keys[0]);

            for (var i = 1; i < keys.Length; i++)
            {
                var k = keys[i];
                if (value == null)
                {
                    value = "";
                    break;
                }
                if (value is FrameDLRObject)
                {
                    value = ((FrameDLRObject)value).GetValue(k);
                }
            }
            return ComFunc.nvl(value);
        }
    }
}
