using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Tag.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Tag.Module
{
    public class TagLocalProxy : LocalModuleProxy<TagParameter,TagData>
    {
        protected override BaseModule<TagParameter, TagData> GetModule(TagParameter p, TagData data)
        {
            var rtn = new TagCallModule();
            return rtn;
        }

        public override void OnError(Exception ex, TagParameter p, TagData data)
        {
            throw ex;
        }
    }
}
