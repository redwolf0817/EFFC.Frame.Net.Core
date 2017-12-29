using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using System.Reflection;

namespace EFFC.Frame.Net.Tag.Core
{
    public class ContextPaserCollection<P, D> : DataCollection
        where P:TagParameter
        where D:TagDataCollection
    {
        public ITagParser<P, D> this[string tagname]
        {
            get
            {
                if (GetValue("tag", tagname) != null)
                {
                    return (ITagParser<P, D>)GetValue("tag." + tagname);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 添加一个Tag解析器
        /// </summary>
        /// <param name="parser"></param>
        public void AddParser(ITagParser<P, D> parser)
        {
            if (!this._d.ContainsKey("tag." + parser.TagName))
            {
                SetValue("tag", parser.TagName, parser);
            }
        }
        /// <summary>
        /// 根据分类添加需要用到的Tag解析器
        /// </summary>
        /// <param name="categoryname">分类名称</param>
        public void AddParserByCategory(string categoryname)
        {
            AddParserByCategory(categoryname, "EFFC.Frame.Net.Tag");
        }
        /// <summary>
        /// 根据分类添加需要用到的Tag解析器
        /// </summary>
        /// <param name="categoryname">分类名称</param>
        /// <param name="assemblyPath">dll的assembly的路径名称</param>
        public void AddParserByCategory(string categoryname,string assemblyPath)
        {
            Assembly asm = Assembly.Load(assemblyPath);
            Type[] ts = asm.GetTypes();
            foreach (var c in ts)
            {
                if (c.IsInterface || c.IsAbstract)
                {
                    continue;
                }
                //判断是否为IModular的实现
                if (c.GetInterface(typeof(ITagParser<P,D>).FullName) != null)
                {
                    var t = ((ITagParser<P,D>)c);
                    if (t.Category.Equals(categoryname, StringComparison.OrdinalIgnoreCase))
                    {
                        AddParser(t);
                    }
                }
            }
        }

        public void RemoveParser(string tagname)
        {
            Remove("tag", tagname);
        }
    }
}
