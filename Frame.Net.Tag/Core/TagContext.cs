using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace EFFC.Frame.Net.Tag.Core
{
    /// <summary>
    /// 标签上下文
    /// </summary>
    public class TagContext
    {
        private Dictionary<string, ITagParser> _parser = null;
        private Dictionary<string, object> _content = new Dictionary<string, object>();
        static object lockobj_tag = new object();

        public TagContext()
        {
            lock (lockobj_tag)
            {
                if (GlobalCommon.ApplicationCache.Get("_parsers_") != null)
                {
                    _parser = (Dictionary<string, ITagParser>)GlobalCommon.ApplicationCache.Get("_parsers_");
                }
                else
                {
                    _parser = new Dictionary<string, ITagParser>();
                    GlobalCommon.ApplicationCache.Set("_parsers_", _parser, DateTime.MaxValue);
                }
            }
        }

        /// <summary>
        /// 新增一个解析器
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parser"></param>
        public void AddTagParser(ITagParser parser)
        {
            var k = parser.TagName.ToLower();
            if (!_parser.ContainsKey(k))
            {
                _parser.Add(k, parser);
            }
        }
        /// <summary>
        /// 根据标签名称获取标签解析器实例
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public ITagParser GetTagParser(string tagname)
        {
            ITagParser rtn = null;
            var k = tagname.ToLower();
            if (_parser.ContainsKey(k))
            {
                var t = _parser[k];
                rtn = t;
            }
            return rtn;
        }
        /// <summary>
        /// 按照分类获取标签解析器
        /// </summary>
        /// <param name="categoryname"></param>
        /// <returns></returns>
        public List<ITagParser> GetTagParsers(string categoryname)
        {
            return _parser.Values.Where(c => c.Category == categoryname).ToList();
        }
        /// <summary>
        /// 获取所有的标签解析器
        /// </summary>
        /// <returns></returns>
        public List<ITagParser> GetAllTagParsers()
        {
            return _parser.Values.ToList();
        }
        /// <summary>
        /// 获取所有已经注册的标签
        /// </summary>
        public string[] TagNames
        {
            get
            {
                return _parser.Keys.ToArray();
            }
        }
        /// <summary>
        /// 将中间解析内容写入context
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="key"></param>
        /// <param name="content"></param>
        public void SetParsedContent(ITagParser parser, string key, object content)
        {
            string k = parser.TagName.ToLower() + "." + key.ToLower();
            if (_content.ContainsKey(k))
            {
                _content[k] = content;
            }
            else
            {
                _content.Add(k, content);
            }

        }
        /// <summary>
        /// 获取中间解析内容
        /// </summary>
        /// <param name="tagname"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetParsedContent(string tagname, string key)
        {

            string k = tagname.ToLower() + "." + key.ToLower();
            if (_content.ContainsKey(k))
            {
                return _content[k];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 添加绑定的对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void AddBindObject(string name, object obj)
        {
            string key = "__bind."+name;
            if (!_content.ContainsKey(key))
            {
                _content.Add(key, obj);
            }
        }
        /// <summary>
        /// 获取绑定的对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetBindObject(string name)
        {
            string key = "__bind." + name;
            if (!_content.ContainsKey(key))
            {
                return _content[key];
            }
            else
            {
                return null;
            }
        }
    }
}
