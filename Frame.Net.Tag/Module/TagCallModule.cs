using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Tag.Core;
using EFFC.Frame.Net.Tag.Tags.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EFFC.Frame.Net.Tag.Module
{
    public class TagCallModule : BaseModule<TagParameter, TagData>
    {
        private TagParameter _mp = null;
        private TagData _md = null;

        public override string Name
        {
            get
            {
                return "TagModule";
            }
        }

        public override string Version
        {
            get
            {
                return "0.1";
            }
        }

        public override string Description
        {
            get
            {
                return "标签解析器";
            }
        }
        protected TagParameter ModuleParameter
        {
            get
            {
                return _mp;
            }
        }
        protected TagData ModuleData
        {
            get
            {
                return _md;
            }
        }

        /// <summary>
        /// 初始化上下文，为后续处理加载对应的解析器,可重载
        /// </summary>
        /// <param name="context"></param>
        protected virtual void Init()
        {
            ModuleData.Context.AddTagParser(new LoadParser());
            ModuleData.Context.AddTagParser(new RefParser());
            ModuleData.Context.AddTagParser(new CopyParser());
            if (!string.IsNullOrEmpty(GlobalCommon.TagCommon.TagAssemblyPath))
            {
                Assembly asm = Assembly.Load(GlobalCommon.TagCommon.TagAssemblyPath);
                Type[] ts = asm.GetTypes();
                foreach (Type t in ts)
                {
                    if (t.IsSubclassOf(typeof(ITagParser)) && !t.IsAbstract && !t.IsInterface)
                    {
                        ITagParser l = (ITagParser)Activator.CreateInstance(t, true);
                        ModuleData.Context.AddTagParser(l);
                    }
                }
            }
        }

        protected virtual void PreProcess()
        {
            //先解析load标签
            var tagload = ModuleData.Context.GetTagParser("load");
            tagload.DoParse(ModuleParameter, ModuleData);
        }

        protected virtual void AferProcess()
        {

        }

        protected virtual void DoProcess()
        {
            ModuleData.ParsedText = ModuleParameter.Text;

            //进行预处理
            PreProcess();
            var list = ModuleData.Context.GetAllTagParsers();
            foreach (var parser in list)
            {
                //if (parser.TagName.ToLower() != "load")
                //{
                    parser.DoParse(ModuleParameter, ModuleData);
                //}
            }
            AferProcess();
        }

        protected override void Run(TagParameter p, TagData d)
        {
            _mp = p;
            _md = d;


            Init();

            DoProcess();
        }

        protected override void OnError(Exception ex, TagParameter p, TagData d)
        {
            throw ex;
        }
    }
}
