using EFFC.Frame.Net.Base.Module;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Tag.Parameters;
using EFFC.Frame.Net.Module.Tag.Datas;
using EFFC.Frame.Net.Module.Tag.Tags;
using EFFC.Frame.Net.Global;
using System.Reflection;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Base.Common;
using System.Linq;

namespace EFFC.Frame.Net.Module.Tag
{
    /// <summary>
    /// 标签解析模块
    /// </summary>
    public class TagCallModule : BaseModule
    {
        static string tagAssembly = "";
        static List<TTagEntity> _customized_taglist = new List<TTagEntity>();
        private TagParameter _mp = null;
        private TagData _md = null;

        public override string Name => "TagModule";

        public override string Description => "标签解析器";

        protected override void OnUsed(ProxyManager ma, dynamic options)
        {
            if (options != null)
            {
                if (options.TagAssembly != null)
                    tagAssembly = options.TagAssembly;
            }
            if(tagAssembly == "")
            {
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.WARN, @"TagCallModule初始化，未发现扩展Tag的Assembly参数，只加载默认标签；如果需要添加自定义标签，请在ProxyManager.UseProxy中给options.TagAssembly赋值");
            }
            else
            {
                Assembly asm = Assembly.Load(new AssemblyName(tagAssembly));
                Type[] ts = asm.GetTypes();
                var list = new List<TTagEntity>();
                foreach (Type t in ts)
                {
                    if (t.GetTypeInfo().IsSubclassOf(typeof(ITagParser)) && !t.GetTypeInfo().IsAbstract && !t.GetTypeInfo().IsInterface)
                    {
                        list.Add(new TTagEntity(t));
                    }
                }
                _customized_taglist = list.OrderBy(s => s.Priority).ThenBy(s=>s.TagName).ToList();
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.WARN,string.Format( @"TagCallModule初始化，发现扩展Tag的Assembly参数，执行加载自定义标签完成，加载自定义标签{0}个", _customized_taglist.Count));
            }
            
        }



        public override bool CheckParametersAndConfig(ParameterStd p, DataCollection d)
        {
            if (!(p is TagParameter)) return false;
            if (!(d is TagData)) return false;

            var tp = (TagParameter)p;
            var td = (TagData)d;

            return true;
        }

        public override void Dispose()
        {
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            throw ex;
        }

        protected override void Run(ParameterStd p, DataCollection d)
        {
            var tp = (TagParameter)p;
            var td = (TagData)d;

            Init(tp, td);

            DoProcess(tp, td);
        }

        /// <summary>
        /// 初始化上下文，为后续处理加载对应的解析器,可重载
        /// </summary>
        /// <param name="tp"></param>
        /// <param name="td"></param>
        protected virtual void Init(TagParameter tp,TagData td)
        {
            //加载默认标签
            td.Context.AddTagParser(new LoadParser());
            td.Context.AddTagParser(new RefParser());
            td.Context.AddTagParser(new CopyParser());
            //加载自定义标签
            foreach(var item in _customized_taglist)
            {
                td.Context.AddTagParser((ITagParser)Activator.CreateInstance(item.TTag));
            }
            
        }

        protected virtual void PreProcess(TagParameter tp, TagData td)
        {
            //先解析load标签
            var tagload = td.Context.GetTagParser("load");
            tagload.DoParse(tp, td);
        }

        protected virtual void AferProcess(TagParameter tp, TagData td)
        {

        }

        protected virtual void DoProcess(TagParameter tp, TagData td)
        {
            td.ParsedText = tp.Text;

            //进行预处理
            PreProcess(tp, td);
            var list = td.Context.GetAllTagParsers();
            foreach (var parser in list)
            {
                //if (parser.TagName.ToLower() != "load")
                //{
                parser.DoParse(tp, td);
                //}
            }
            AferProcess(tp,td);
        }

        private class TTagEntity
        {
            public TTagEntity(Type t)
            {
                TTag = t;
                var eo = FrameExposedObject.New(t);
                TagName = eo.TagName;
            }
            /// <summary>
            /// Tag类型
            /// </summary>
            public Type TTag { get; set; }
            /// <summary>
            /// Tag名称
            /// </summary>
            public string TagName { get; set; }
            /// <summary>
            /// 加载优先级
            /// </summary>
            public int Priority { get; set; }
        }
    }
}
