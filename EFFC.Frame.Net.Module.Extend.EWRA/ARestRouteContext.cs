using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA
{
    /// <summary>
    /// Rest的路由上下文结构，用于标识路由与Logic中执行方法的映射关系，同时可以通过路由来搜索出要执行的列表
    /// </summary>
    public abstract class ARestRouteContext
    {
        /// <summary>
        /// 当前主版本号
        /// </summary>
        public abstract string MainVersion
        {
            get;
            protected set;
        }
        /// <summary>
        /// 总路由表描述
        /// </summary>
        public abstract FrameDLRObject RouteDesc
        {
            get;
            protected set;
        }
        /// <summary>
        /// 主版本访问路由
        /// </summary>
        public abstract FrameDLRObject MainRouteDesc
        {
            get;
            protected set;
        }

        /// <summary>
        /// 根据版本号获取RestAPI调用说明
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public abstract FrameDLRObject GetRouteDesc(string version);
        /// <summary>
        /// 根据路由、谓词和参数来获取
        /// </summary>
        /// <param name="route"></param>
        /// <param name="verb"></param>
        /// <param name="parsedParams"></param>
        /// <returns></returns>
        public abstract List<RouteInvokeEntity> FindByRoute(string route, string verb, ref object[] parsedParams);
        /// <summary>
        /// 执行加载动作，将assemblyName中符合logicBaseType的所有logic都构建起路由映射表，其中须建立主版本号的路由映射表
        /// </summary>
        /// <param name="assemblyName">加载的程序集</param>
        /// <param name="mainversion">主版本号</param>
        /// <param name="logicBaseType">可执行逻辑层的基类</param>
        public abstract void Load(string assemblyName, string mainversion, Type logicBaseType);
    }
}
