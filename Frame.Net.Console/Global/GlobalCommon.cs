using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.System;

namespace EFFC.Frame.Net.Consoles.Global
{
    public class GlobalCommon
    {
        static ILogger logger = null;
        static IExceptionProcess _ep = null;

        /// <summary>
        /// Logger
        /// </summary>
        public static ILogger Logger
        {
            get
            {
                return logger;
            }

            set
            {
                logger = value;
            }
        }

        /// <summary>
        /// 異常處理
        /// </summary>
        public static IExceptionProcess ExceptionProcessor
        {
            get
            {
                return _ep;
            }
            set
            {
                _ep = value;
            }
        }

        public class ConsoleCommon
        {
            /// <summary>
            /// Business呼叫的Logic的AssemblyPath
            /// </summary>
            public static string LogicAssemblyPath
            {
                get;
                set;
            }
        }
    }
}
