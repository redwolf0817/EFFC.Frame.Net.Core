using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Tag.Core
{
    /// <summary>
    /// 标签解析器
    /// </summary>
    /// <typeparam name="P"></typeparam>
    /// <typeparam name="D"></typeparam>
    public interface ITagParser
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        string TagName { get; }
        /// <summary>
        /// 标签分类，可以为多种，每种用逗号分隔。
        /// 种类为base的会最优先执行，而且为base类的标签是最基本的，不会需要其他的标签辅助
        /// </summary>
        string Category { get; }
        /// <summary>
        /// 参数集名称列表
        /// </summary>
        string[] ArgNames { get; }
        /// <summary>
        /// 进行文本解析
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        void DoParse(TagParameter p, TagData d);
        /// <summary>
        /// 是否需要大括号
        /// </summary>
        bool IsNeedBrace
        {
            get;
        }
        /// <summary>
        /// 描述
        /// </summary>
        string Description
        {
            get;
        }
        /// <summary>
        /// 将本Tag的相关信息转化成json对象
        /// </summary>
        /// <returns></returns>
        FrameDLRObject ToJsonObject();
    }
}
