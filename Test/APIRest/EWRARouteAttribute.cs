using System;
using System.Collections.Generic;
using System.Text;

namespace Test.APIRest
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EWRARouteAttribute:Attribute
    {
        /// <summary>
        /// 路由属性描述
        /// 注意:使用本属性后，方法所属的类单独会构成API独立入口，最后API的调用格式为：/{参数route}/{parmeters}
        /// </summary>
        /// <param name="verb">方法谓词:get,post,put,patch,delete</param>
        /// <param name="route">路由描述，路由不可与其他的路由重复</param>
        public EWRARouteAttribute(string verb,string route)
        {
            MethodVerb = verb.ToLower();
            Route = route;
        }
        /// <summary>
        /// 方法Verb
        /// </summary>
        public string MethodVerb
        {
            get;
            private set;
        }
        /// <summary>
        /// 路由描述
        /// </summary>
        public string Route
        {
            get;
            private set;
        }
    }
}
