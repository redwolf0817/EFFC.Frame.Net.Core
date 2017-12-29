using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Tag.Tags.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Logic
{
    public class ActionTag:BaseTag
    {

        protected override string DoProcess(dynamic args, string content)
        {
            string action = ComFunc.nvl(args.action);
            Dictionary<string,string> actioncontent = null;
            List<string> actions = null;
            if (CurrentContext.GetParsedContent(this.TagName, action) != null)
            {
                actioncontent = (Dictionary<string, string>)CurrentContext.GetParsedContent(this.TagName, action);
            }
            else
            {
                actioncontent = new Dictionary<string, string>();
                this.CurrentContext.SetParsedContent(this, action, actioncontent);
            }
            if (CurrentContext.GetParsedContent(this.TagName, "actionlist") != null)
            {
                actions = (List<string>)CurrentContext.GetParsedContent(this.TagName, "actionlist");
            }
            else
            {
                actions = new List<string>();
                CurrentContext.SetParsedContent(this, "actionlist", actions);
            }
            if (!actions.Contains(action))
            {
                actions.Add(action);
            }

            var uid = Guid.NewGuid().ToString();
            var key = "#" + uid + "#";
            actioncontent.Add(key, content);
            return key;
            
        }

        public override string TagName
        {
            get { return "action"; }
        }

        public override string Category
        {
            get { return "hostlogic"; }
        }

        public override string[] ArgNames
        {
            get { return new string[1] { "action" }; }
        }

        public override bool IsNeedBrace
        {
            get { return true; }
        }

        public override string Description
        {
            get { return "将标签中的内容标识为参数指定名称的一个命名域，编译时会根据各个域名称生成hjs文件"; }
        }
    }
}
