using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Tag.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestAPISample.Tags
{
    public class ForEach : BaseTag
    {
        public override string TagName => "foreach";

        public override string Category => "Data";

        public override string[] ArgNames => new string[] { "itemname", "dataname" };

        public override bool IsNeedBrace => true;
        public override int Priority => 20;

        public override string Description => "foreach循环，格式 #for(itemname,bind_data_key){html内容,需要绑定数据使用#v:itemname.columname来表示}";

        protected override string DoProcess(dynamic args, string content)
        {
            string itemname = ComFunc.nvl(args.itemname);
            string dataname = ComFunc.nvl(args.dataname);
            if (CurrentContext.GetBindObject(dataname) == null) return "";
            if (!(CurrentContext.GetBindObject(dataname) is IEnumerable<object>)) return "";
            var data = (IEnumerable<object>)CurrentContext.GetBindObject(dataname);
            var rtn = new StringBuilder();
            var index = 0;
            foreach (dynamic item in data)
            {
                var newitemname = $"{itemname}_{index}";
                rtn.AppendLine(content.Replace($"#v({itemname}", $"#v({newitemname}"));
                CurrentContext.AddBindObject(newitemname, item);
                index++;
            }
            return rtn.ToString();
        }
    }
}
