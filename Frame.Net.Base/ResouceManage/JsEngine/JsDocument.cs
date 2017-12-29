using EFFC.Frame.Net.Base.AttributeDefine;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.ResouceManage.JsEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.ResouceManage.JsEngine
{
    public class JsDocument:BaseHostJsObject
    {
        string _jsroot = "";
        string _commlib = "";
        string _runtimelib = "";
        HostJs jse = null;
        public JsDocument(HostJs je, string jsroot, string commlibpath, string runtimelibpath)
        {
            _commlib = commlibpath;
            _runtimelib = runtimelibpath;
            _jsroot = jsroot;
            jse = je;
        }
        public JsDocument(HostJs je, string jsroot)
        {
            _commlib = jsroot;
            _runtimelib = jsroot + HostJsConstants.COMPILED_ROOT_PATH;
            _jsroot = jsroot;
            jse = je;
        }
        [Desc("加载一段js到当前上下文，参数path中：~表示根路径，$Common表示公共库路径，$RunTime表示运行时库路径")]
        public void load(string path)
        {
            var fpath = path.Replace("~", _jsroot).Replace("$Common", _commlib).Replace("$RunTime", _runtimelib);
            if (File.Exists(fpath))
            {
                var js = File.ReadAllText(fpath, Encoding.UTF8);
                jse.Evaluate(js);
            }
        }
        public override string Description
        {
            get { return "Host Js引擎用到的文本对象"; }
        }

        public override string Name
        {
            get { return "Document"; }
        }
    }
}
