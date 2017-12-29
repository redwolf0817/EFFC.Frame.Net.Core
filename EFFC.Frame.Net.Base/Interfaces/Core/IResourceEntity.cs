using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Base.Interfaces.Core
{
    public interface IResourceEntity
    {
        /// <summary>
        /// 每个资源实例都有一个自己的UniCode
        /// </summary>
        string ID { get; }
        /// <summary>
        /// 每个资源都应该要进行资源释放
        /// </summary>
        void Release();
    }
}
