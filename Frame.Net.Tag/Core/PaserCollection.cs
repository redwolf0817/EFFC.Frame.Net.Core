using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoJo.Frame.Net.Base.Data;

namespace JoJo.Frame.Net.Tag.Core
{
    public class ParserCollection<P,D>:DataCollection
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
        /// <param name="categoryname"></param>
        public void AddParserByCategory(string categoryname)
        {

        }

        public void RemoveParser(string tagname)
        {
            if (!this._d.ContainsKey("tag." + tagname))
            {
                this._d.Remove("tag." + tagname);
            }
        }
    }
}
