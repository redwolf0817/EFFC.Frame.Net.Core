using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.ResouceManage.JsEngine;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Tag.Core;
using EFFC.Frame.Net.Tag.Tags.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Engine
{
    /// <summary>
    /// Host Js的View引擎
    /// 该引擎会将标签页面转化成hostjs文件，然后通过hostjs引擎进行输出view的html
    /// </summary>
    public partial class HostJsView:IDisposable
    {
        HostViewContext _context = null;

        public static HostJsView CreateInstance(Dictionary<string,object> model)
        {
            var rtn = new HostJsView();
            rtn.CurrentContext.SetDataModel(model);
            return rtn;
        }

        public HostJsView()
        {
            _context = new HostViewContext();
            _context.RootPath = GlobalCommon.HostCommon.RootPath;
            _context.RunTimeLibPath = GlobalCommon.HostCommon.RootPath + HostJsConstants.COMPILED_ROOT_PATH;
            _context.CommonLibPath = GlobalCommon.HostCommon.RootPath;
        }
        public HostJsView(HostViewContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 当前上下文
        /// </summary>
        public HostViewContext CurrentContext
        {
            get
            {
                return _context;
            }
        }
        /// <summary>
        /// 将源码编译成js代码
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <param name="source">源码</param>
        /// <param name="isCreateFile">是否产生编译后的文件,如果产生文件，则文件产生的目录为Host的根路径下的Compiled/View/</param>
        /// <param name="outfilepath">产生编译后的js文件的存放路径，只有在isCreateFile=true的时候有效</param>
        /// <returns></returns>
        public string Compile(string filename, string source, bool isCreateFile)
        {
            var rtn = "";
            var p = new TagParameter();
            var d = new TagData();
            p.RootPath = _context.RootPath;
            p.CommonLibPath = _context.CommonLibPath;
            p.RunTimeLibPath = _context.RunTimeLibPath;
            p.Text = source;
            p.SetValue("__tags__", _context.AllTags);
            ModuleProxyManager.Call<HostViewProxy, TagParameter, TagData>(p, d);
            rtn = d.ParsedText;
            if (isCreateFile)
            {
                if (!Directory.Exists(p.RunTimeLibPath + HostJsConstants.VIEW_PATH))
                {
                    Directory.CreateDirectory(p.RunTimeLibPath + HostJsConstants.VIEW_PATH);
                }
                var path = p.RunTimeLibPath + HostJsConstants.VIEW_PATH + filename + ".hjs";
                File.WriteAllText(path, rtn, Encoding.UTF8);
            }
            return rtn;
        }
        /// <summary>
        /// 删除已经编译好的所有文件
        /// </summary>
        public void DeleteAllCompiledFile()
        {
            var path = _context.RunTimeLibPath + HostJsConstants.VIEW_PATH;
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

        }
        /// <summary>
        /// Debug模式运行，需要人工释放js引擎资源
        /// </summary>
        /// <param name="js"></param>
        /// <returns></returns>
        public object DebugRun(string js)
        {
            return DebugRun(js, _context, "out");
        }
        /// <summary>
        /// Debug模式运行，需要人工释放js引擎资源
        /// </summary>
        /// <param name="js"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public object DebugRun(string js, HostViewContext context)
        {
            return DebugRun(js, context, "out");
        }
        /// <summary>
        ///  Debug方式运行js，该方式不会自动释放js引擎资源，需要手动执行，如果js引擎资源得不到良好释放会导致系统无法运行
        /// </summary>
        /// <param name="js"></param>
        /// <param name="context"></param>
        /// <param name="outobjname"></param>
        /// <returns></returns>
        public object DebugRun(string js, HostViewContext context,string outobjname)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach (var item in context.AllHostJsObject)
            {
                dic.Add(item.Name, item);
            }
            context.CurrentHostJsEngine.Evaluate(js, dic.ToArray());
            var outobj = context.CurrentHostJsEngine.GetOutObject(outobjname);
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
        /// Debug方式进行页面渲染，该方式不会自动释放js引擎资源，需要手动执行，如果js引擎资源得不到良好释放会导致系统无法运行
        /// </summary>
        /// <param name="js"></param>
        /// <returns></returns>
        public string RenderDebug(string js)
        {
            //JsDocument因跟hostjs路径有关需要重新装载一次
            _context.AddHostJsObject(new JsDocument(_context.CurrentHostJsEngine, _context.RootPath, _context.CommonLibPath, _context.RunTimeLibPath));
            return Render(js, _context);
        }
        /// <summary>
        /// Debug方式进行页面渲染，该方式不会自动释放js引擎资源，需要手动执行，如果js引擎资源得不到良好释放会导致系统无法运行
        /// </summary>
        /// <param name="js"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string RenderDebug(string js, HostViewContext context)
        {
            var rtn = "";
            var l = context.AllHostJsObject;
            List<KeyValuePair<string, object>> kvp = new List<KeyValuePair<string, object>>();
            foreach (var item in l)
            {
                if (item.Name.ToLower() != "model")
                {
                    kvp.Add(new KeyValuePair<string, object>(item.Name, item));
                }
            }
            kvp.Add(new KeyValuePair<string, object>("model", context.CurrentModel));
            context.CurrentHostJsEngine.Evaluate(js, kvp.ToArray());
            rtn = ((ViewDocument)context.GetHostJsObject("viewdoc")).OutHtml();

            return rtn;
        }
        /// <summary>
        /// 将js代码渲染成html
        /// </summary>
        /// <param name="js"></param>
        /// <returns></returns>
        public string Render(string js)
        {
            //JsDocument因跟hostjs路径有关需要重新装载一次
            _context.AddHostJsObject(new JsDocument(_context.CurrentHostJsEngine, _context.RootPath, _context.CommonLibPath, _context.RunTimeLibPath));
            return Render(js, _context);
        }
        /// <summary>
        /// 根据指定的上下文结构执行View渲染
        /// </summary>
        /// <param name="js"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Render(string js,HostViewContext context)
        {
            var rtn = "";
            try
            {
                var l = context.AllHostJsObject;
                List<KeyValuePair<string, object>> kvp = new List<KeyValuePair<string, object>>();
                foreach (var item in l)
                {
                    if (item.Name.ToLower() != "model")
                    {
                        kvp.Add(new KeyValuePair<string, object>(item.Name, item));
                    }
                }
                kvp.Add(new KeyValuePair<string, object>("model", context.CurrentModel));
                context.CurrentHostJsEngine.Evaluate(js, kvp.ToArray());
                rtn = ((ViewDocument)context.GetHostJsObject("viewdoc")).OutHtml();
            }
            finally
            {
                context.CurrentHostJsEngine.Release();
            }
            return rtn;
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
        /// <summary>
        /// 将整个资源都进行释放
        /// </summary>
        public void Dispose()
        {
            Release();
        }
        /// <summary>
        /// 释放js引擎资源
        /// </summary>
        public void Release()
        {
            _context.CurrentHostJsEngine.Release();
            _context.Dispose();
        }
    }

    public class HostViewContext : DataCollection,IDisposable
    {
        Dictionary<string, BaseHostJsObject> _hostobject = new Dictionary<string, BaseHostJsObject>();
        Dictionary<string, ITagParser> _hosttag = new Dictionary<string, ITagParser>();
        HostJs _currentjse = HostJs.NewInstance();
        object _datamodel = new object();
        /// <summary>
        /// ~所表示的根路径的物理地址
        /// </summary>
        public string RootPath
        {
            get;
            set;
        }
        /// <summary>
        /// 公共库路径
        /// </summary>
        public string CommonLibPath
        {
            get;
            set;
        }
        /// <summary>
        /// 运行时库路径
        /// </summary>
        public string RunTimeLibPath
        {
            get;
            set;
        }

        public HostViewContext()
        {
            RootPath = GlobalCommon.HostCommon.RootPath;
            RunTimeLibPath = GlobalCommon.HostCommon.RootPath + HostJsConstants.COMPILED_ROOT_PATH;
            CommonLibPath = GlobalCommon.HostCommon.RootPath;
            var viewdoc = new ViewDocument(_currentjse);
            _hostobject.Add(viewdoc.Name, viewdoc);
            var console = new ConsoleObject();
            _hostobject.Add(console.Name, console);
            var document = new JsDocument(_currentjse, RootPath);
            _hostobject.Add(document.Name, document);
            var comfunc = new ComFuncObject();
            _hostobject.Add(comfunc.Name, comfunc);

            var loadtag = new LoadParser();
            var reftag = new RefParser();
            var copytag = new CopyParser();
            var outtag = new OutTag();
            var hjstag = new HjsTag();
            var fortag = new ForTag();
            var iftag = new IfTag();
            var elseiftag = new ElseIfTag();
            var elsetag = new ElseTag();
            //按照先后处理标签的顺序进行标签处理
            _hosttag.Add(loadtag.TagName, loadtag);
            _hosttag.Add(reftag.TagName, reftag);
            _hosttag.Add(copytag.TagName, copytag);
            //out会变为hjs标签
            _hosttag.Add(outtag.TagName, outtag);
            _hosttag.Add(iftag.TagName, iftag);
            _hosttag.Add(elseiftag.TagName, elseiftag);
            _hosttag.Add(elsetag.TagName, elsetag);
            _hosttag.Add(fortag.TagName, fortag);
            _hosttag.Add(hjstag.TagName, hjstag);
        }
        public HostJs CurrentHostJsEngine
        {
            get
            {
                return _currentjse;
            }
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
            else
            {
                _hostobject.Remove(obj.Name);
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
        /// 设置数据模型
        /// </summary>
        /// <param name="data"></param>
        public void SetDataModel(object data)
        {
            //js引擎只识别dictionary对象为json对象
            if (data is FrameDLRObject)
            {
                _datamodel = ((FrameDLRObject)data).ToDictionary();
            }
            else
            {
                _datamodel = data;
            }
        }
        /// <summary>
        /// 获取当前的数据模型
        /// </summary>
        public object CurrentModel
        {
            get
            {
                return _datamodel;
            }
        }

        void IDisposable.Dispose()
        {
            foreach (var item in _hostobject)
            {
                if (item.Value is IDisposable)
                {
                    ((IDisposable)item.Value).Dispose();
                }
            }

            _hostobject.Clear();
            _hosttag.Clear();
            _currentjse.Dispose();
            _datamodel = null;
            _hostobject = null;
            _hosttag = null;
        }
    }
}

