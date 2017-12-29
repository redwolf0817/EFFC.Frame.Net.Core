using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Tag.Tags.Base
{
    /// <summary>
    /// 引用文本解析，将引用的文本用传入的参数做标记，提供给其它标签使用，引用的文本不做任何改动
    /// 参数:
    /// flag:引用的名称
    /// isreserve:是否保留文本内容,默认为true
    /// </summary>
    public class RefParser:BaseTag
    {
        /// <summary>
        /// 进行ref标签解析
        /// </summary>
        /// <param name="args"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected override string DoProcess(dynamic args, string content)
        {
            var flag = ComFunc.nvl(args.flag);
            var is_reserve = true;
            if (!bool.TryParse(ComFunc.nvl(args.isreserve), out is_reserve))
            {
                is_reserve = true;
            }
            CurrentContext.SetParsedContent(this, flag, content);
            if (is_reserve)
                return content;
            else
                return "";
        }
        /// <summary>
        /// 标签名称
        /// </summary>
        public override string TagName
        {
            get { return "ref"; }
        }
        /// <summary>
        /// 标签种类
        /// </summary>
        public override string Category
        {
            get { return "base"; }
        }
        /// <summary>
        /// 标签参数
        /// </summary>
        public override string[] ArgNames
        {
            get { return new string[] { "flag","isreserve" }; }
        }
        /// <summary>
        /// 是否含有大括号
        /// </summary>
        public override bool IsNeedBrace
        {
            get { return true; }
        }

        public override string Description
        {
            get
            {
                return @"引用文本解析，将引用的文本用传入的参数做标记，提供给其它标签使用，引用的文本不做任何改动
     参数:
    flag:引用的名称
    isreserve:是否保留文本内容,默认为true";
            }
        }
    }
}
