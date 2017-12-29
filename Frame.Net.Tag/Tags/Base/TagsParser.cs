using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EFFC.Frame.Net.Tag.Core;
using EFFC.Frame.Net.Base.Data.Base;

namespace EFFC.Frame.Net.Tag.Tags.Base
{
    public class TagsParser:ITagParser
    {
        /// <summary>
        /// 全标签解析正则
        /// </summary>
        string regstr = @"(?isx)
                        (^|(?<=\s))\#[A-Za-z0-9_]+(\([\S ]*\))?\s*(\{                          #普通字符“(”

                            (?>                     #分组构造，用来限定量词“*”修饰范围

                                [^{}]+              #非括弧的其它任意字符

                            |                       #分支结构

                                \{  (?<Open>)       #命名捕获组，遇到开括弧Open计数加1

                            |                       #分支结构

                                \}  (?<-Open>)      #狭义平衡组，遇到闭括弧Open计数减1

                            )*                      #以上子串出现0次或任意多次

                            (?(Open)(?!))           #判断是否还有'OPEN'，有则说明不配对，什么都不匹配

                        \}){0,1}                          #普通闭括弧
                       ";
        /// <summary>
        /// 抓取标签名称和参数
        /// </summary>
        string regTagName_args = @"(?isx)
                        (?<=(^|(?<=\s))\#)[A-Za-z0-9_]+(\([\S ]*\))?(?=\s*\{                          #普通字符“(”

                            (?>                     #分组构造，用来限定量词“*”修饰范围

                                [^{}]+              #非括弧的其它任意字符

                            |                       #分支结构

                                \{  (?<Open>)       #命名捕获组，遇到开括弧Open计数加1

                            |                       #分支结构

                                \}  (?<-Open>)      #狭义平衡组，遇到闭括弧Open计数减1

                            )*                      #以上子串出现0次或任意多次

                            (?(Open)(?!))           #判断是否还有'OPEN'，有则说明不配对，什么都不匹配

                        \}                          #普通闭括弧
)
                       ";
        /// <summary>
        /// 标签的名称
        /// </summary>
        string regTagName = @"(?isx)
                        (?<=(^|(?<=\s)))[A-Za-z0-9_]+(?=(\([\S ]*\))?)";
        /// <summary>
        /// 标签的参数
        /// </summary>
        string regTagArgs = @"(?isx)
                        (?<=(^|(?<=\s))[A-Za-z0-9_]+\()[\S ]*(?=\))";

        public string TagName
        {
            get { return ""; }
        }

        public string Category
        {
            get { return "base"; }
        }

        public string[] ArgNames
        {
            get { return new string[0]; }
        }

        public void DoParse(TagParameter p, TagData d)
        {
            if (string.IsNullOrEmpty(d.ParsedText)) return;

            var tmpcontent = d.ParsedText;
            //解析全文本，将里面符合标签规则的文本全部分离出来，以供相关程式使用
            Regex re = new Regex(regstr);
            Regex reTagName_Args = new Regex(regTagName_args);
            Regex reTagName = new Regex(regTagName);
            Regex reArgs = new Regex(regTagArgs);
            

            List<TagEntity> lentity = new List<TagEntity>();
            //匹配所有标签
            foreach (Match rm in re.Matches(tmpcontent))
            {
                var uid = Guid.NewGuid().ToString();
                tmpcontent = tmpcontent.Replace(rm.Value, "<?tmp%" + uid + "%>");
                lentity.Add(new TagEntity(uid, rm.Value));
            }

            foreach (var t in lentity)
            {
                d.ParsedText = t.Content;
                var tagname_arg = reTagName_Args.IsMatch(t.Content) ? reTagName_Args.Match(t.Content).Value : "";
                if (tagname_arg != "")
                {
                    var tagname = reTagName.Match(tagname_arg).Value;
                    var parser = d.Context.GetTagParser(tagname);
                    //解析标签
                    parser.DoParse(p, d);

                    tmpcontent = tmpcontent.Replace("<?tmp%" + t.UID + "%>", d.ParsedText);
                }
            }

            d.ParsedText = tmpcontent;

        }

        public bool IsNeedBrace
        {
            get { return true ; }
        }

        private class TagEntity
        {
            public TagEntity(string uid, string content)
            {
                UID = uid;
                Content = content;
            }
            public string UID
            {
                get;
                set;
            }

            public string Content
            {
                get;
                set;
            }
        }


        public Net.Base.Data.Base.FrameDLRObject ToJsonObject()
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.tag = TagName;
            rtn.category = Category;
            rtn.args = ArgNames;
            rtn.isbrace = IsNeedBrace;
            return rtn;
        }


        public string Description
        {
            get { return ""; }
        }
    }
}
