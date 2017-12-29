using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.ResouceManage.JsEngine;
using EFFC.Frame.Net.Business.Logic;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Tag.Core;
using EFFC.Frame.Net.Tag.Tags.Base;
using Noesis.Javascript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Engine
{
    /// <summary>
    /// HostLogic所需要的Js引擎
    /// </summary>
    public class HostLogicEngine
    {
        HostLogicContext _context = null;
        WebBaseLogic<WebParameter, GoData> _logic = null;
        public HostLogicEngine(WebBaseLogic<WebParameter, GoData> logic)
        {
            _context = new HostLogicContext();
            _context.RootPath = GlobalCommon.HostCommon.RootPath;
            _context.RunTimeLibPath = GlobalCommon.HostCommon.RootPath + "/" + HostJsConstants.COMPILED_ROOT_PATH;
            _context.CommonLibPath = GlobalCommon.HostCommon.RootPath;
            _logic = logic;
            InitContext(_context, logic);
        }
        public HostLogicEngine(HostLogicContext context, WebBaseLogic<WebParameter, GoData> logic)
        {
            _context = context;
            _logic = logic;
            InitContext(_context, logic);
        }
        public static void InitContext(HostLogicContext context, WebBaseLogic<WebParameter, GoData> logic)
        {
            context.AddHostJsObject(new JsDocument(context.CurrentJsEngine, context.RootPath, context.CommonLibPath, context.RunTimeLibPath));
            context.AddHostJsObject(new ConsoleObject());
            context.AddHostJsObject(new ServerObject(logic));
            context.AddHostJsObject(new ClientObject(logic));
            context.AddHostJsObject(new SessionObject(logic));
            context.AddHostJsObject(new LoginInfoObject(logic));
            context.AddHostJsObject(new CookieObject(logic));
            context.AddHostJsObject(new ConfigObject(logic));
            context.AddHostJsObject(new DBObject(logic));
            context.AddHostJsObject(new InputObject(logic));
            context.AddHostJsObject(new LogicObject(logic));
            context.AddHostJsObject(new ComFuncObject());
            context.AddHostJsObject(new FrameCacheObject(logic));
            context.AddHostJsObject(new LockObject(logic));
        }
        /// <summary>
        /// 将源码编译成js代码
        /// </summary>
        /// <param name="source">源码</param>
        /// <param name="filename">文件名称</param>
        /// <param name="isCreateFile">是否产生编译后的js文件,如果产生文件，则文件产生的目录为运行时库所在的路径,默认为Host的根路径下的Compiled/Logic/</param>
        /// <param name="outfilepath">产生编译后的js文件的存放路径，只有在isCreateFile=true的时候有效</param>
        /// <returns></returns>
        public Dictionary<string, string> Compile(string filename, string source, bool isCreateFile)
        {
            var p = new TagParameter();
            var d = new TagData();
            p.RootPath = _context.RootPath;
            p.CommonLibPath = _context.CommonLibPath;
            p.RunTimeLibPath = _context.RunTimeLibPath;
            p.Text = source;
            p.SetValue("__tags__", _context.AllTags);
            ModuleProxyManager.Call<HostLogicProxy, TagParameter, TagData>(p, d);
            Dictionary<string, string> contents = (Dictionary<string, string>)d.ExtentionObj.result;
            if (isCreateFile)
            {
                if (!Directory.Exists(p.RunTimeLibPath))
                {
                    Directory.CreateDirectory(p.RunTimeLibPath);
                }
                if (!Directory.Exists(p.RunTimeLibPath + HostJsConstants.LOGIC_PATH))
                {
                    Directory.CreateDirectory(p.RunTimeLibPath + HostJsConstants.LOGIC_PATH);
                }
                foreach (var item in contents)
                {
                    var path = p.RunTimeLibPath + HostJsConstants.LOGIC_PATH + filename + (item.Key != "" ? "." + item.Key : "") + ".hjs";
                    
                    File.WriteAllText(path, item.Value, Encoding.UTF8);
                }
            }
            return contents;
        }
        /// <summary>
        /// 删除已经编译好的所有文件
        /// </summary>
        public void DeleteAllCompiledFile()
        {
            var path = _context.RunTimeLibPath + HostJsConstants.LOGIC_PATH;
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

        }
        /// <summary>
        /// 运行格式化js，将js封装成function运行，可以使用return语句，用于HostLogic使用
        /// </summary>
        /// <param name="js"></param>
        /// <returns></returns>
        public object RunFormatJs(string js)
        {
            var formatjs = @"function ___r___(){
    " + js + @"
}; 
var _____out______ = ___r___();";
            return RunJs(formatjs, CurrentContext, "_____out______");
        }
        /// <summary>
        /// 运行格式化js，将js封装成function运行，可以使用return语句，用于HostLogic使用
        /// </summary>
        /// <param name="js"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object RunFormatJs(string js, HostLogicContext context)
        {
            var formatjs = @"function ___r___(){
    " + js + @"
}; 
var _____out______ = ___r___();";
            return RunJs(formatjs, context, "_____out______");
        }
        /// <summary>
        /// 运行格式化js，将js封装成function运行，可以使用return语句，用于HostLogic使用
        /// </summary>
        /// <param name="js"></param>
        /// <returns></returns>
        public object DebugRunFormatJs(string js)
        {
            var formatjs = @"function ___r___(){
    " + js + @"
}; 
var _____out______ = ___r___();";
            return DebugRunJs(formatjs, CurrentContext, "_____out______");
        }
        /// <summary>
        /// 运行格式化js，将js封装成function运行，可以使用return语句，用于HostLogic使用
        /// </summary>
        /// <param name="js"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object DebugRunFormatJs(string js, HostLogicContext context)
        {
            var formatjs = @"function ___r___(){
    " + js + @"
}; 
var _____out______ = ___r___();";
            return DebugRunJs(formatjs, context, "_____out______");
        }
        /// <summary>
        /// 运行脚本，并获取计算结果
        /// </summary>
        /// <param name="js"></param>
        /// <returns></returns>
        public object RunJs(string js)
        {
            //JsDocument因跟hostjs路径有关需要重新装载一次
            _context.SetHostJsObject(new JsDocument(_context.CurrentJsEngine, _context.RootPath, _context.CommonLibPath, _context.RunTimeLibPath));
            return RunJs(js, _context, "out");
        }
        /// <summary>
        /// Debug方式运行js，该方式不会自动释放js引擎资源，需要手动执行，如果js引擎资源得不到良好释放会导致系统无法运行
        /// </summary>
        /// <param name="js"></param>
        /// <returns></returns>
        public object DebugRunJs(string js)
        {
            //JsDocument因跟hostjs路径有关需要重新装载一次
            _context.SetHostJsObject(new JsDocument(_context.CurrentJsEngine, _context.RootPath, _context.CommonLibPath, _context.RunTimeLibPath));
            return DebugRunJs(js, _context);
        }
        /// <summary>
        /// 运行js，默认返回参数名称为out
        /// </summary>
        /// <param name="js"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object RunJs(string js, HostLogicContext context)
        {
            return RunJs(js, context, "out");
        }
        /// <summary>
        /// debug方式运行js，默认返回参数名称为out
        /// </summary>
        /// <param name="js"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object DebugRunJs(string js, HostLogicContext context)
        {
            return DebugRunJs(js, context, "out");
        }
        /// <summary>
        /// 根据指定的上下文执行hostjs
        /// </summary>
        /// <param name="js"></param>
        /// <param name="context"></param>
        /// <param name="outobjname"></param>
        /// <returns></returns>
        public object RunJs(string js, HostLogicContext context, string outobjname)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (var item in context.AllHostJsObject)
                {
                    dic.Add(item.Name, item);
                }
                
                context.CurrentJsEngine.Evaluate(js, dic.ToArray());
                var outobj = context.CurrentJsEngine.GetOutObject(outobjname);
                if (outobj == null)
                {
                    outobj = FrameDLRObject.CreateInstance();
                }
                if (outobj is Dictionary<string, object>)
                {
                    return FrameDLRObject.CreateInstance(outobj, FrameDLRFlags.SensitiveCase);
                }
                else
                {
                    return outobj;
                }
            }
            finally
            {
                context.CurrentJsEngine.Release();
            }
        }
        /// <summary>
        ///  Debug方式运行js，该方式不会自动释放js引擎资源，需要手动执行，如果js引擎资源得不到良好释放会导致系统无法运行
        /// </summary>
        /// <param name="js"></param>
        /// <param name="context"></param>
        /// <param name="outobjname"></param>
        /// <returns></returns>
        public object DebugRunJs(string js, HostLogicContext context,string outobjname)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach (var item in context.AllHostJsObject)
            {
                dic.Add(item.Name, item);
            }
            context.CurrentJsEngine.Evaluate(js, dic.ToArray());
            var outobj = context.CurrentJsEngine.GetOutObject(outobjname);
            if (outobj == null)
            {
                outobj = FrameDLRObject.CreateInstance();
            }
            if (outobj is Dictionary<string, object>)
            {
                return FrameDLRObject.CreateInstance(outobj, FrameDLRFlags.SensitiveCase);
            }
            else
            {
                return outobj;
            }
        }
        /// <summary>
        /// 获取控制台的信息
        /// </summary>
        /// <returns></returns>
        public string GetConsoleMsg()
        {
            var c = (ConsoleObject)CurrentContext.GetHostJsObject(new ConsoleObject().Name);
            return c.OutMsg;
        }
        /// <summary>
        /// 当前上下文
        /// </summary>
        public HostLogicContext CurrentContext
        {
            get
            {
                return _context;
            }
        }
        /// <summary>
        /// Logic Js的保留关键字
        /// </summary>
        public string[] ReserveKeys
        {
            get
            {
                return HostJs.ReserveKeys;
            }
        }
        /// <summary>
        /// 服务器保留对象
        /// </summary>
        public FrameDLRObject ServerReserverObjectKey
        {
            get
            {
                FrameDLRObject rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                foreach (var item in _context.AllHostJsObject)
                {
                    rtn.SetValue(item.Name, item.ConstructorDesc);
                }
                return rtn;
            }
        }
        /// <summary>
        /// 服务器保留标签
        /// </summary>
        public FrameDLRObject ReserverTags
        {
            get
            {
                FrameDLRObject rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                foreach (var item in _context.AllTags)
                {
                    rtn.SetValue(item.TagName, item.ToJsonObject());
                }
                return rtn;
            }
        }
    }
    /// <summary>
    /// HostLogic的运行时的上下文
    /// </summary>
    public class HostLogicContext : DataCollection
    {
        Dictionary<string, BaseHostJsObject> _hostobject = new Dictionary<string, BaseHostJsObject>();
        Dictionary<string, ITagParser> _hosttag = new Dictionary<string, ITagParser>();
        HostJs _currentjse = null;
        /// <summary>
        /// ~所表示的根路径,Host Js的物理地址
        /// </summary>
        public string RootPath
        {
            get;
            set;
        }
        /// <summary>
        /// $Common所表示的公共库路径
        /// </summary>
        public string CommonLibPath
        {
            get;
            set;
        }
        /// <summary>
        /// $RunTime所表示的运行时库路径
        /// </summary>
        public string RunTimeLibPath
        {
            get;
            set;
        }
        public HostLogicContext()
        {
            _currentjse = HostJs.NewInstance();
            var loadtag = new LoadParser();
            var reftag = new RefParser();
            var copytag = new CopyParser();
            var actiontag = new ActionTag();
            //按照先后处理标签的顺序进行标签处理
            _hosttag.Add(loadtag.TagName, loadtag);
            _hosttag.Add(reftag.TagName, reftag);
            _hosttag.Add(copytag.TagName, copytag);
            _hosttag.Add(actiontag.TagName, actiontag);
            RootPath = GlobalCommon.HostCommon.RootPath;
            CommonLibPath = GlobalCommon.HostCommon.RootPath;
            RunTimeLibPath = GlobalCommon.HostCommon.RootPath + HostJsConstants.COMPILED_ROOT_PATH;
        }
        public HostLogicContext(HostJs jse)
        {
            _currentjse = jse;
        }
        /// <summary>
        /// 添加一个tag
        /// </summary>
        /// <param name="tag"></param>
        public void AddTag(ITagParser tag)
        {
            if (!_hosttag.ContainsKey(tag.TagName))
            {
                _hosttag.Add(tag.TagName, tag);
            }
        }
        /// <summary>
        /// 根据tag的名称获取一个tag
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public ITagParser GetTag(string tagname)
        {
            if (_hosttag.ContainsKey(tagname))
            {
                return _hosttag[tagname];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取所有的Tag
        /// </summary>
        public List<ITagParser> AllTags
        {
            get
            {
                return _hosttag.Values.ToList();
            }
        }
        /// <summary>
        /// 添加一个hostjs对象
        /// </summary>
        /// <param name="obj"></param>
        public void AddHostJsObject(BaseHostJsObject obj)
        {
            if (!_hostobject.ContainsKey(obj.Name))
            {
                _hostobject.Add(obj.Name, obj);
            }
        }
        /// <summary>
        /// 添加一个对象，有则更新，没有则新增
        /// </summary>
        /// <param name="obj"></param>
        public void SetHostJsObject(BaseHostJsObject obj)
        {
            if (!_hostobject.ContainsKey(obj.Name))
            {
                _hostobject.Add(obj.Name, obj);
            }
            else
            {
                _hostobject.Remove(obj.Name);
                _hostobject.Add(obj.Name, obj);
            }
        }
        /// <summary>
        /// 根据名称获取一个hostjs对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BaseHostJsObject GetHostJsObject(string name)
        {
            if (_hostobject.ContainsKey(name))
            {
                return _hostobject[name];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取所有的hostjs对象
        /// </summary>
        public List<BaseHostJsObject> AllHostJsObject
        {
            get
            {
                return _hostobject.Values.ToList();
            }
        }
        /// <summary>
        /// 当前的hostjs引擎
        /// </summary>
        public HostJs CurrentJsEngine
        {
            get
            {
                if (_currentjse == null)
                    _currentjse = HostJs.NewInstance();
                return _currentjse;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var item in _hostobject)
            {
                item.Value.Dispose();
            }
            _hostobject.Clear();
            _hosttag.Clear();
            _hostobject = null;
            _hosttag = null;
            _currentjse.Dispose();
        }
    }
}

