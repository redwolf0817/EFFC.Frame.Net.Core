using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoJo.Frame.Net.Base.Common;
using JoJo.Frame.Net.Base.Interfaces;
using JoJo.Frame.Net.Base.Interfaces.System;

namespace JoJo.Frame.Net.Web.Global
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

        public class WMvcCommon
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

        public class GoCommon
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

        public class AspRCommon
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

        public class CssxCommon
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

        public class WebFormCommon
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
