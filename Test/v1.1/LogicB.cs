using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Test.APIRest;

namespace Test.v1._1
{
    class LogicB:MyLogic
    {
        new List<object> get()
        {
            return null;
        }

        object get(List<object> parent_list)
        {
            return null;
        }
        [RouteDesc("根据ID和My的实例获取资料")]
        object get(string id,dynamic parent_info)
        {
            return null;
        }

        [RouteDesc("这个是无效的，除了parent_开头的参数外，其他的参数类型必须为string")]
        object get(int id, dynamic parent_info)
        {
            return null;
        }

        object put(dynamic parent_info)
        {
            return null;
        }
        [EWRARoute("get", "/school/{school}/student/{name}")]
        object getInfo(string name,string school)
        {
            Console.WriteLine($"{name}在{school}读一年级");
            return null;
        }

    }
}
