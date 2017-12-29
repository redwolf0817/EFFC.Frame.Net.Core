using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Tag.Core;
using EFFC.Frame.Net.Tag.Module;
using EFFC.Frame.Net.Tag.Tags.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Logic
{
    public class HostLogicTagModule : TagCallModule
    {
        protected override void Init()
        {
            if (ModuleParameter.GetValue("__tags__") != null
                && ModuleParameter.GetValue("__tags__") is List<ITagParser>)
            {
                var l = (List<ITagParser>)ModuleParameter.GetValue("__tags__");
                foreach (var item in l)
                {
                    ModuleData.Context.AddTagParser(item);
                }
            }
            else
            {
                //按照先后处理标签的顺序进行标签处理
                ModuleData.Context.AddTagParser(new LoadParser());
                ModuleData.Context.AddTagParser(new RefParser());
                ModuleData.Context.AddTagParser(new CopyParser());
                ModuleData.Context.AddTagParser(new ActionTag());
            }
        }
        protected override void AferProcess()
        {
            var text = ModuleData.ParsedText;
            Dictionary<string, string> parsetexts = new Dictionary<string, string>();
            //根据不通的action生成不同的文本
            if (ModuleData.Context.GetParsedContent("action", "actionlist") != null)
            {
                var actionlist = (List<string>)ModuleData.Context.GetParsedContent("action", "actionlist");
                var reg = new Regex(@"(?isx)\#[A-Za-z0-9\-]*\#");
                foreach (var s in actionlist)
                {
                    var tmptext = text;
                    var contents = (Dictionary<string, string>)ModuleData.Context.GetParsedContent("action", s);
                    foreach (var item in contents)
                    {
                        tmptext = tmptext.Replace(item.Key, item.Value);
                    }


                    tmptext = reg.Replace(tmptext, "");
                    parsetexts.Add(s, tmptext);
                }

                
            }
            else
            {
                parsetexts.Add("", text);
            }

            ModuleData.ExtentionObj.result = parsetexts;
        }
    }


    public class HostLogicProxy : LocalModuleProxy<TagParameter, TagData>
    {

        protected override BaseModule<TagParameter, TagData> GetModule(TagParameter p, TagData data)
        {
            var rtn = new HostLogicTagModule();
            return rtn;
        }

        public override void OnError(Exception ex, TagParameter p, TagData data)
        {
            throw ex;
        }
    }
}
