using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Business.Logic
{
    public partial class WebBaseLogic<P, D>
    {
        private GlobalizationHelper _global = null;
        /// <summary>
        /// 全球化
        /// </summary>
        public virtual GlobalizationHelper Globaliztion
        {
            get {
                if (_global == null)
                    _global = new GlobalizationHelper(this);
                return _global;
            }
        }


        public class GlobalizationHelper
        {
            WebBaseLogic<P, D> _logic = null;

            public GlobalizationHelper(WebBaseLogic<P, D> logic)
            {
                _logic = logic;
            }

            /// <summary>
            /// 提示信息全球化，如果没有找到对应资源则key作为返回值
            /// </summary>
            /// <param name="targetSpace">目标空间</param>
            /// <param name="key">对应资源key，如果没有找到对应资源则key作为返回值</param>
            /// <returns></returns>
            public virtual string Message(string targetSpace, string key)
            {
                return key;
            }
            /// <summary>
            /// 提示信息全球化，目标空间为本logic的name，如果没有找到对应资源则key作为返回值
            /// </summary>
            /// <param name="key">对应资源key，如果没有找到对应资源则key作为返回值</param>
            /// <returns></returns>
            public virtual string Message(string key)
            {
                return Message(_logic.Name, key);
            }
            /// <summary>
            /// 提供Global目标空间中的提示信息
            /// </summary>
            /// <param name="key">对应资源key，如果没有找到对应资源则key作为返回值</param>
            /// <returns></returns>
            public virtual string GlobalMessage(string key)
            {
                return Message("Global", key);
            }
        }
    }
}
