using System;
using System.Collections.Generic;
using System.Text;

namespace Frame.Net.Base.Exceptions
{
    /// <summary>
    /// HostJs引擎执行时爆出的异常
    /// </summary>
    public class HostJsException:Exception
    {
        
        public HostJsException()
            : base()
        {
        }

        public HostJsException(string msg,string sourcecode,int line,int column)
            : base(msg)
        {
            Line = line;
            Column = column;
            SourceCode = sourcecode;
        }
        /// <summary>
        /// 出错代码所在的行数
        /// </summary>
        public int Line
        {
            get;
            set;
        }
        /// <summary>
        /// 出错代码所在的列数
        /// </summary>
        public int Column
        {
            get;
            set;
        }
        /// <summary>
        /// 源代码
        /// </summary>
        public string SourceCode
        {
            get;
            set;
        }
    }
}
