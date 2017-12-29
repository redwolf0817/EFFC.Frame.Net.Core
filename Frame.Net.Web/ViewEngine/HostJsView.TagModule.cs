using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Tag.Core;
using EFFC.Frame.Net.Tag.Module;
using EFFC.Frame.Net.Tag.Tags.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Web.ViewEngine
{
    /// <summary>
    /// hostjs view的tag解析module
    /// </summary>
    public class HostViewTagModule : TagCallModule
    {
        protected override void Init()
        {
            if (ModuleParameter.GetValue("__tags__") != null
                && ModuleParameter.GetValue("__tags__") is List<ITagParser>)
            {
                var l = (List<ITagParser>)ModuleParameter.GetValue("__tags__");
                ITagParser hjstag = null;
                foreach (var t in l)
                {
                    if (t.TagName.ToLower() != "hjs")
                    {
                        ModuleData.Context.AddTagParser(t);
                    }
                    else
                    {
                        hjstag = t;
                    }
                    
                }

                if (hjstag != null)
                {
                    ModuleData.Context.AddTagParser(hjstag);
                }
                else
                {
                    ModuleData.Context.AddTagParser(new HjsTag());
                }
            }
            else
            {
                //按照先后处理标签的顺序进行标签处理
                ModuleData.Context.AddTagParser(new LoadParser());
                ModuleData.Context.AddTagParser(new RefParser());
                ModuleData.Context.AddTagParser(new CopyParser());
                //out会变为hjs标签
                ModuleData.Context.AddTagParser(new OutTag());
                ModuleData.Context.AddTagParser(new HjsTag());
            }
        }
        protected override void AferProcess()
        {
            var text = ModuleData.ParsedText;
            var jstext = new StringBuilder();
            var reg = new Regex(@"(?isx)\#hjs[A-Za-z0-9\-]*\#");
            var arrstr = reg.Split(text);
            var m = reg.Matches(text);
            for (int i = 0; i < arrstr.Length; i++)
            {
                jstext.AppendLine(@"viewdoc.write('" + ComFunc.HTMLEncode(arrstr[i]).Replace("\r", "\\r").Replace("\n", "\\n").Replace("&", "###") + "');");

                if (i < m.Count)
                {
                    var key = m[i].Value;
                    key = key.Replace("#hjs", "").Replace("#", "");
                    jstext.AppendLine(ComFunc.nvl(ModuleData.Context.GetParsedContent("hjs", key)));
                }
            }

            ModuleData.ParsedText = jstext.ToString();
        }
    }

    public class HostViewProxy : LocalModuleProxy<TagParameter, TagData>
    {

        protected override BaseModule<TagParameter, TagData> GetModule(TagParameter p, TagData data)
        {
            var rtn = new HostViewTagModule();
            return rtn;
        }

        public override void OnError(Exception ex, TagParameter p, TagData data)
        {
            throw ex;
        }
    }
}
