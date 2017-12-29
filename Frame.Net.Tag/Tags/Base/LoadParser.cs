using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EFFC.Frame.Net.Tag.Core;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Tag.Tags.Base
{
    /// <summary>
    /// 将文本中所有的Load标签都转化成对应的文本加载进来，提供给全局标签解析器解析
    /// 如果发现加载的文本中依然有load标签则递归找下去，直到没有一个load标签为止
    /// 参数只有一个就是文本路径，路径可以使用~作为rootpath,该参数由module中的TagParameter.RootPath提供
    /// </summary>
    public class LoadParser : ITagParser
    {
        public string TagName
        {
            get { return "load"; }
        }

        public string Category
        {
            get { return "base"; }
        }

        public string[] ArgNames
        {
            get
            {
                List<string> args = new List<string>();
                args.Add("path");
                return args.ToArray();
            }
        }
        /// <summary>
        /// 找出load标签的文本
        /// </summary>
        string regstr = @"(?isx)
                        (^|(?<=\#))load\s*\(                          #普通字符“(”

                            (?>                     #分组构造，用来限定量词“*”修饰范围

                                [^()]+              #非括弧的其它任意字符

                            |                       #分支结构

                                \(  (?<Open>)       #命名捕获组，遇到开括弧Open计数加1

                            |                       #分支结构

                                \)  (?<-Open>)      #狭义平衡组，遇到闭括弧Open计数减1

                            )*                      #以上子串出现0次或任意多次

                            (?(Open)(?!))           #判断是否还有'OPEN'，有则说明不配对，什么都不匹配

                        \)                          #普通闭括弧
                       ";
        /// <summary>
        /// 找出args
        /// </summary>
        string regArgs = @"(?isx)
                       (?<=load\()[^\s]+(?=\))
                       ";
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        public void DoParse(TagParameter p, TagData d)
        {
            d.ParsedText = LoadProcess(p.Text, p, d);
        }

        private string LoadProcess(string text, TagParameter p, TagData d)
        {
            var processedtext = text;
            Regex re = new Regex(regstr);

            Dictionary<string, string> tmpd = new Dictionary<string, string>();
            foreach (Match m in re.Matches(text))
            {
                if (!tmpd.ContainsKey(m.Value))
                {
                    var content = LoadTextFromArgs(m.Value, p, d);
                    processedtext = processedtext.Replace("#"+m.Value, LoadProcess(content, p, d));
                    tmpd.Add(m.Value, LoadProcess(content, p, d));
                }
            }

            return processedtext;
        }
        /// <summary>
        /// 自定义扩展:通过传入的参数来获取要加载后的文本
        /// </summary>
        /// <param name="tagstr">被解析后的标签串（含参数），格式为#load(xxx)</param>
        /// <returns></returns>
        protected string LoadTextFromArgs(string tagstr,TagParameter p, TagData d)
        {
            Regex reArgs = new Regex(regArgs);
            var filepath = reArgs.IsMatch(tagstr) ? reArgs.Match(tagstr).Value : "";
            filepath = filepath.Replace("~", p.RootPath).Replace("$Common", p.CommonLibPath).Replace("$RunTime", p.RunTimeLibPath)
                .Replace("$View",p.RootPath+ HostJsConstants.VIEW_PATH).Replace("$Logic",p.RootPath+HostJsConstants.LOGIC_PATH);
            if (File.Exists(filepath))
            {
                var content = File.ReadAllText(filepath, Encoding.UTF8);
                return content;
            }
            else
            {
                return "";
            }
        }

        public bool IsNeedBrace
        {
            get { return false; }
        }

        public FrameDLRObject ToJsonObject()
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.tag = TagName;
            rtn.category = Category;
            rtn.args = ArgNames;
            rtn.isbrace = IsNeedBrace;
            rtn.description = Description;
            return rtn;
        }


        public string Description
        {
            get { return "用于加载其他文本内容到本标签指定位置，其参数Path为文本路径，~表示根路径，$Common表示公共库路径，$RunTime表示运行时库路径，$View表示View的存放路径，$Logic表示Logic的存放路径"; }
        }
    }
}
