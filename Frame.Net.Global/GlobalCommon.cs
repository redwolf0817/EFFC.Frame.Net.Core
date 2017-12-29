using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Global
{
    /// <summary>
    /// Frame全局环境参数
    /// </summary>
    public class GlobalCommon
    {
        static ILogger logger = null;
        static IExceptionProcess _ep = null;
        static IFrameCache _cache = null;

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
        /// <summary>
        /// 框架cache
        /// </summary>
        public static IFrameCache ApplicationCache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = new FrameAppCache();
                }
                return _cache;
            }
            set
            {
                _cache = value;
            }
        }
        /// <summary>
        /// 流程公共相关
        /// </summary>
        public class FlowCommon
        {
            /// <summary>
            /// Business呼叫的Logic的AssemblyPath
            /// </summary>
            public static string LogicAssemblyPath
            {
                get;
                set;
            }
            /// <summary>
            /// flow control的assembly path
            /// </summary>
            public static string FlowDefineAssemblyPath
            {
                get;
                set;
            }
            /// <summary>
            /// FlowStep的assembly path
            /// </summary>
            public static string StepAssemblyPath
            {
                get;
                set;
            }
        }
        /// <summary>
        /// MVC View相关参数
        /// </summary>
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
        /// <summary>
        /// Go相关参数
        /// </summary>
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
        /// <summary>
        /// ASPR相关参数
        /// </summary>
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
        /// <summary>
        /// Cssx相关参数
        /// </summary>
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
        /// <summary>
        /// Web From相关参数
        /// </summary>
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
        /// <summary>
        /// Tag解析器相关参数
        /// </summary>
        public class TagCommon
        {
            /// <summary>
            /// tag parse的assembly path
            /// </summary>
            public static string TagAssemblyPath
            {
                get;
                set;
            }
        }
        /// <summary>
        /// Unit相关参数
        /// </summary>
        public class UnitCommon
        {
            /// <summary>
            /// Unit呼叫的Logic的AssemblyPath
            /// </summary>
            public static string UnitAssemblyPath
            {
                get;
                set;
            }
        }

        public class WebSocketCommon
        {
            static int _max = 10;
            /// <summary>
            /// 长时间无交互时，连接存活的最大时间(分钟),默认10分钟
            /// </summary>
            public static int MaxConnectionMinutes
            {
                get
                {
                    return _max;
                }
                set
                {
                    _max = value;
                }
            }
        }
        /// <summary>
        /// Host View的公共参数设置
        /// </summary>
        public class HostCommon
        {
            /// <summary>
            /// Host Js文件的默认根路径(含公共、View、Logic、Compiled的根路径）
            /// </summary>
            public static string RootPath
            {
                get;
                set;
            }
        }
        /// <summary>
        /// console的公共参数设置
        /// </summary>
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
        /// <summary>
        /// web的公共参数设置
        /// </summary>
        public class WebCommon
        {
            /// <summary>
            /// web的默认起始页面
            /// </summary>
            public static string StartPage
            {
                get;
                set;
            }
        }
    }
}
