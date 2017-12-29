using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Business.Logic;
using EFFC.Frame.Net.Module.Web.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Web.Logic
{
    /// <summary>
    /// WebBaseLogic.Info
    /// </summary>
    /// <typeparam name="PType"></typeparam>
    /// <typeparam name="DType"></typeparam>
    public partial class WebBaseLogic<PType,DType>
    {
        PostDataHelper _pd = null;

        /// <summary>
        /// PostData相关操作
        /// </summary>
        public virtual PostDataHelper PostData
        {
            get
            {
                if (_pd == null)
                    _pd = new PostDataHelper(this);

                return _pd;
            }
        }
        /// <summary>
        /// PostData的相关操作，作为动态对象使用
        /// </summary>
        public virtual dynamic PostDataD
        {
            get
            {
                return PostData;
            }
        }
        public class PostDataHelper : MyDynamicMetaProvider
        {
            WebBaseLogic<PType, DType> _logic;

            public PostDataHelper(WebBaseLogic<PType, DType> l)
            {
                _logic = l;
            }
            /// <summary>
            /// 获取Post数据
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public object this[string key]
            {
                get
                {
                    return _logic.CallContext_Parameter.GetValue(DomainKey.POST_DATA, key);
                }
            }

            protected override object GetMetaValue(string key)
            {
                return this[key];
            }

            protected override object InvokeMe(string methodInfo, params object[] args)
            {
                return null;
            }

            protected override object SetMetaValue(string key, object value)
            {
                //do nothing
                return null;
            }

            protected override object GetMetaIndexValue(object[] indexs)
            {
                if (indexs.Length > 0)
                    return this[ComFunc.nvl(indexs[0])];
                else
                    return null;
            }
        }
    }
}
