using System;
using System.Collections.Generic;
using System.Text;

namespace Frame.Net.Base.Exceptions
{
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

        public int Line
        {
            get;
            set;
        }

        public int Column
        {
            get;
            set;
        }

        public string SourceCode
        {
            get;
            set;
        }
    }
}
