using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Tag.Datas;
using EFFC.Frame.Net.Module.Tag.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace EFFC.Frame.Net.Module.Tag.Tags
{
    /// <summary>
    /// 基类-标签解析器
    /// </summary>
    public abstract class BaseTag : ITagParser
    {
        /// <summary>
        /// 带大括号的标签文本
        /// </summary>
        protected string regstrwithbrace = "";
        /// <summary>
        /// 不带大括号的标签文本
        /// </summary>
        protected string regstrwithoutbrace = "";
        /// <summary>
        /// 找出args
        /// </summary>
        protected string regArgs = "";
        /// <summary>
        /// 找出括号中的content
        /// </summary>
        protected string regcontent = "";

        public BaseTag()
        {
            regstrwithbrace = @"(?isx)
                        (^|(?<=\#))" + this.TagName + @"\s*(\([\S ]*\))?\s*\{                          #普通字符“(”

                            (?>                     #分组构造，用来限定量词“*”修饰范围

                                [^{}]+              #非括弧的其它任意字符

                            |                       #分支结构

                                \{  (?<Open>)       #命名捕获组，遇到开括弧Open计数加1

                            |                       #分支结构

                                \}  (?<-Open>)      #狭义平衡组，遇到闭括弧Open计数减1

                            )*                      #以上子串出现0次或任意多次

                            (?(Open)(?!))           #判断是否还有'OPEN'，有则说明不配对，什么都不匹配

                        \}                          #普通闭括弧
                       ";
            regstrwithoutbrace = @"(?isx)
                        (^|(?<=\#))" + this.TagName + @"\s*\(                          #普通字符“(”

                            (?>                     #分组构造，用来限定量词“*”修饰范围

                                [^()]+              #非括弧的其它任意字符

                            |                       #分支结构

                                \(  (?<Open>)       #命名捕获组，遇到开括弧Open计数加1

                            |                       #分支结构

                                \)  (?<-Open>)      #狭义平衡组，遇到闭括弧Open计数减1

                            )*                      #以上子串出现0次或任意多次

                            (?(Open)(?!))           #判断是否还有'OPEN'，有则说明不配对，什么都不匹配

                        \)                          #普通闭括弧";

            regArgs = @"(?isx)
                       (?<=" + this.TagName + @"\s*\()                         #普通字符“(”

                            (?>                     #分组构造，用来限定量词“*”修饰范围

                                [^()]+              #非括弧的其它任意字符

                            |                       #分支结构

                                \(  (?<Open>)       #命名捕获组，遇到开括弧Open计数加1

                            |                       #分支结构

                                \)  (?<-Open>)      #狭义平衡组，遇到闭括弧Open计数减1

                            )*                      #以上子串出现0次或任意多次

                            (?(Open)(?!))           #判断是否还有'OPEN'，有则说明不配对，什么都不匹配

                        (?=\))                          #普通闭括弧
                       ";

            regcontent = @"(?isx)
                        (?<=(^|(?<=\#))" + this.TagName + @"\s*(\([\S ]*\))?\s*\{)                         #普通字符“(”

                            (?>                     #分组构造，用来限定量词“*”修饰范围

                                [^{}]+              #非括弧的其它任意字符

                            |                       #分支结构

                                \{  (?<Open>)       #命名捕获组，遇到开括弧Open计数加1

                            |                       #分支结构

                                \}  (?<-Open>)      #狭义平衡组，遇到闭括弧Open计数减1

                            )*                      #以上子串出现0次或任意多次

                            (?(Open)(?!))           #判断是否还有'OPEN'，有则说明不配对，什么都不匹配

                        (?=\})                          #普通闭括弧";
        }

        protected abstract string DoProcess(dynamic args, string content);
        AsyncLocal<TagData> tagdata = new AsyncLocal<TagData>();
        public void DoParse(TagParameter p, TagData d)
        {
            tagdata.Value = d;
            foreach(var item in p.BindObject.Items)
            {
                tagdata.Value.Context.AddBindObject(item.Key, item.Value);
            }

            Regex re = null;
            Regex rearg = new Regex(regArgs);
            Regex recontent = new Regex(regcontent);
            FrameDLRObject args = FrameDLRObject.CreateInstance();
            var tmpcontent = d.ParsedText;
            if (IsNeedBrace)
            {
                re = new Regex(regstrwithbrace);
            }
            else
            {
                re = new Regex(regstrwithoutbrace);
            }

            if (re.IsMatch(tmpcontent))
            {
                foreach (Match s in re.Matches(tmpcontent))
                {
                    var content = s.Value;
                    string argstr = rearg.Match(content).Value;
                    string[] argsarray = argstr.Split(',');
                    for (int i = 0; i < argsarray.Length; i++)
                    {
                        if (i < this.ArgNames.Length)
                        {
                            args.SetValue(this.ArgNames[i], argsarray[i]);
                        }
                        else
                        {
                            args.SetValue("arg" + i, argsarray[i]);
                        }
                    }

                    var bracecontent = "";
                    if (IsNeedBrace)
                    {
                        if (recontent.IsMatch(content))
                        {
                            bracecontent = recontent.Match(content).Value;
                        }
                    }

                    tmpcontent = tmpcontent.Replace("#" + content, DoProcess(args, bracecontent));
                }
            }
            d.ParsedText = tmpcontent;
        }
        /// <summary>
        /// 当前呼叫环境下的Tag Context
        /// </summary>
        protected TagContext CurrentContext
        {
            get
            {
                if (tagdata.Value != null)
                {
                    return tagdata.Value.Context;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 标签名字
        /// </summary>
        public abstract string TagName
        {
            get;
        }
        /// <summary>
        /// 标签类别
        /// </summary>
        public abstract string Category
        {
            get;
        }
        /// <summary>
        /// 参数名称集合
        /// </summary>
        public abstract string[] ArgNames
        {
            get;
        }
        /// <summary>
        /// 是否需要大括号
        /// </summary>
        public abstract bool IsNeedBrace
        {
            get;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public abstract string Description
        {
            get;
        }
        /// <summary>
        /// 加载优先级，正数，数字越小优先加载
        /// </summary>
        public virtual int Priority => 10;

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

        
    }
}
