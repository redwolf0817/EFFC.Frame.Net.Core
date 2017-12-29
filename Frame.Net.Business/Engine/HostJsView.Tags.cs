using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Tag.Tags.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Engine
{
    /// <summary>
    /// Hjs标签，不可内嵌，内容只能为js语句
    /// </summary>
    public class HjsTag : BaseTag
    {
       

        public override string[] ArgNames
        {
            get { return new string[] { }; }
        }

        public override string Category
        {
            get { return "hostview"; }
        }

        protected override string DoProcess(dynamic args, string content)
        {
            var uid = Guid.NewGuid().ToString();
            CurrentContext.SetParsedContent(this, uid, content.Replace(SymbalCode.LEFT_BRACE,"{").Replace(SymbalCode.RIGHT_BRACE,"}") );
            return "#" + TagName + uid + "#";
        }

        public override bool IsNeedBrace
        {
            get { return true; }
        }

        public override string TagName
        {
            get { return "hjs"; }
        }

        public override string Description
        {
            get { return "将标签中的内容直接标记为hjs脚本内容，在编译时直接作为可执行的内容"; }
        }
    }
    /// <summary>
    /// out标签,可以被很多标签内嵌，因此请在可以内嵌out标签解析器之前加载out标签，但out标签必须在hjs之前加载
    /// </summary>
    public class OutTag : BaseTag
    {
        public override string[] ArgNames
        {
            get
            {
                var rtn = new string[]{
                    "value"
                };
                return rtn;
            }
        }

        public override string Category
        {
            get { return "hostview"; }
        }

        protected override string DoProcess(dynamic args, string content)
        {
            return @"#hjs{viewdoc.write(" + args.value + @");}";
        }

        public override bool IsNeedBrace
        {
            get { return false; }
        }

        public override string TagName
        {
            get { return "out"; }
        }

        public override string Description
        {
            get { return "将标签中的参数只作为输出识别，该标签会转变为#hjs{viewdoc.write(参数);}"; }
        }
    }
    /// <summary>
    /// for标签，加载时请在hjs标签之前加载
    /// </summary>
    public class ForTag : BaseTag
    {

        protected override string DoProcess(dynamic args, string content)
        {
            return RecurseProcess(args, content);
        }

        private string RecurseProcess(dynamic args, string content)
        {
            return ProcessForIfElse(this.TagName, args, content);
        }

        public static string ProcessForIfElse(string tagname, dynamic args, string content)
        {
            var regArgs = @"(?isx)
                       (?<=(for|if|else|else if)\s*\()[^\(|\)]+(?=\))
                       ";

            var regcontent = @"(?isx)
                        (?<=(^|(?<=\#))(for|if|else|else if)\s*(\([\S ]*\))?\s*\{)                         #普通字符“(”

                            (?>                     #分组构造，用来限定量词“*”修饰范围

                                [^{}]+              #非括弧的其它任意字符

                            |                       #分支结构

                                \{  (?<Open>)       #命名捕获组，遇到开括弧Open计数加1

                            |                       #分支结构

                                \}  (?<-Open>)      #狭义平衡组，遇到闭括弧Open计数减1

                            )*                      #以上子串出现0次或任意多次

                            (?(Open)(?!))           #判断是否还有'OPEN'，有则说明不配对，什么都不匹配

                        (?=\})                          #普通闭括弧";
            var regstrwithbrace = @"(?isx)
                        (^|(?<=\#))(for|if|else|else if)\s*(\([\S ]*\))?\s*\{                          #普通字符“(”

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
            Regex re = null;
            Regex rearg = new Regex(regArgs);
            Regex recontent = new Regex(regcontent);
            var tmpcontent = content;
            
            re = new Regex(regstrwithbrace);
          
            if (re.IsMatch(content))
            {
                foreach (Match s in re.Matches(content))
                {
                    var mycontent = s.Value;
                    FrameDLRObject myargs = FrameDLRObject.CreateInstance();
                    string argstr = rearg.Match(mycontent).Value;
                    string[] argsarray = argstr.Split(',');
                    for (int i = 0; i < argsarray.Length; i++)
                    {
                        myargs.SetValue("value", argsarray[i]);
                    }

                    var bracecontent = "";
                    if (recontent.IsMatch(mycontent))
                    {
                        bracecontent = recontent.Match(mycontent).Value;
                    }

                    var tmptagname = mycontent.Replace(bracecontent, "").Replace("(" + argstr + ")", "").Replace("{","").Replace("}","").Replace("\r","").Replace("\n","").Trim();
                    tmpcontent = tmpcontent.Replace("#" + mycontent, ProcessForIfElse(tmptagname, myargs, bracecontent));
                }
            }

            var rtn = new StringBuilder();
            var arrwitharg = new string[] { "for", "if", "else if" };
            if (arrwitharg.Contains(tagname))
            {
                rtn.Append(@"#hjs{" + tagname + @"(" + args.value + @")" + SymbalCode.LEFT_BRACE + @"}
    " + tmpcontent + @"
#hjs{" + SymbalCode.RIGHT_BRACE + @"}");
            }
            else
            {
                rtn.Append(@"#hjs{" + tagname + SymbalCode.LEFT_BRACE + @"}
    " + tmpcontent + @"
#hjs{" + SymbalCode.RIGHT_BRACE + @"}");
            }
            return rtn.ToString();
        }

        public override string TagName
        {
            get { return "for"; }
        }

        public override string Category
        {
            get { return "hostview"; }
        }

        public override string[] ArgNames
        {
            get
            {
                var rtn = new string[]{
                    "value"
                };
                return rtn;
            }
        }

        public override bool IsNeedBrace
        {
            get { return true; }
        }

        public override string Description
        {
            get { return "#for(js表达式)：带大括号，转化成\"#hjs{for(js表达式){xxxx}}\"的文本，内嵌#out标签和#if,#elseif,#else标签"; }
        }
    }
    /// <summary>
    /// if标签，可以被内嵌在for标签中，在加载的时候请在for标签之前加载
    /// </summary>
    public class IfTag : BaseTag
    {

        protected override string DoProcess(dynamic args, string content)
        {
//            var rtn = new StringBuilder();
//            rtn.Append(@"#hjs{if(" + args.value + @")" + SymbalCode.LEFT_BRACE + @"}
//    " + content + @"
//#hjs{" + SymbalCode.RIGHT_BRACE + @"}");
//            return rtn.ToString();
            return ForTag.ProcessForIfElse(this.TagName, args, content);
        }

        public override string TagName
        {
            get { return "if"; }
        }

        public override string Category
        {
            get { return "hostview"; }
        }

        public override string[] ArgNames
        {
            get
            {
                var rtn = new string[]{
                    "value"
                };
                return rtn;
            }
        }

        public override bool IsNeedBrace
        {
            get { return true; }
        }

        public override string Description
        {
            get { return "#if(js表达式)：带大括号，转化成\"#hjs{if(js表达式){xxxx}}\"的文本"; }
        }
    }
    /// <summary>
    /// elseif标签，可以被内嵌在for标签中，在加载的时候请在for标签之前加载
    /// 编写代码时，注意elseif标签紧跟在if标签之后，中间不要插入任何代码，本标签不会对if标签的位置进行任何检查
    /// </summary>
    public class ElseIfTag : BaseTag
    {

        protected override string DoProcess(dynamic args, string content)
        {
//            var rtn = new StringBuilder();
//            rtn.Append(@"#hjs{else if(" + args.value + @")" + SymbalCode.LEFT_BRACE + @"}
//    " + content + @"
//#hjs{" + SymbalCode.RIGHT_BRACE + @"}");
//            return rtn.ToString();
            return ForTag.ProcessForIfElse(this.TagName, args, content);
        }

        public override string TagName
        {
            get { return "elseif"; }
        }

        public override string Category
        {
            get { return "hostview"; }
        }

        public override string[] ArgNames
        {
            get
            {
                var rtn = new string[]{
                    "value"
                };
                return rtn;
            }
        }

        public override bool IsNeedBrace
        {
            get { return true; }
        }

        public override string Description
        {
            get { return "#elseif(js表达式)：带大括号，转化成\"#hjs{else if(js表达式){xxxx}}\"的文本"; }
        }
    }
    /// <summary>
    /// else标签，可以被内嵌在for标签中，在加载的时候请在for标签之前加载
    /// 编写代码时，注意elseif标签紧跟在if标签之后，中间不要插入任何代码，本标签不会对if标签的位置进行任何检查
    /// </summary>
    public class ElseTag : BaseTag
    {

        protected override string DoProcess(dynamic args, string content)
        {
//            var rtn = new StringBuilder();
//            rtn.Append(@"#hjs{else" + SymbalCode.LEFT_BRACE + @"}
//    " + content + @"
//#hjs{" + SymbalCode.RIGHT_BRACE + @"}");
//            return rtn.ToString();
            return ForTag.ProcessForIfElse(this.TagName, args, content);
        }

        public override string TagName
        {
            get { return "else"; }
        }

        public override string Category
        {
            get { return "hostview"; }
        }

        public override string[] ArgNames
        {
            get
            {
                var rtn = new string[]{
                };
                return rtn;
            }
        }

        public override bool IsNeedBrace
        {
            get { return true; }
        }

        public override string Description
        {
            get { return "#else：带大括号，转化成\"#hjs{else{xxxx}}\"的文本"; }
        }
    }
}
