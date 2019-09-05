using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA
{
    /// <summary>
    /// 路由执行流程中的元素
    /// </summary>
    public class RouteInvokeEntity
    {
        Dictionary<string, RouteInvokeInputItem> _inputs = new Dictionary<string, RouteInvokeInputItem>();
        /// <summary>
        /// 路由执行元件
        /// </summary>
        public RouteInvokeEntity()
        {
            OutputDesc = new RouteInvokeOutputDesc();
        }
        /// <summary>
        /// 执行的实例的类型
        /// </summary>
        public Type InstanceType
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 执行的方法
        /// </summary>
        public MethodInfo InvokeMethod
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 对应的路由路径
        /// </summary>
        public string Route
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 对应路由路径的正则表达式
        /// </summary>
        public string RouteRegExpress
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 对应路由描述
        /// </summary>
        public string RouteDesc
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 执行的名称
        /// </summary>
        public string InvokeName
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 排除名称为parent_开头的参数后的参数个数
        /// </summary>
        public int ParameterCountWithOutParent
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 判断是否有名称为parent_开头的参数
        /// </summary>
        public bool HasParentParameter
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 是否可见，是否显示在接口说明清单上
        /// </summary>
        public bool IsVisible
        {
            get; internal protected set;
        }
        /// <summary>
        /// 返回数据的描述
        /// </summary>
        public RouteInvokeOutputDesc OutputDesc
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 输入参数的描述集
        /// </summary>
        public List<RouteInvokeInputItem> InputItems
        {
            get
            {
                return _inputs.Values.ToList();
            }
        }
        /// <summary>
        /// 新增一个input的描述
        /// </summary>
        /// <param name="input"></param>
        public void AddInputItem(EWRAAddInputAttribute input)
        {
            if (input != null)
            {
                if (!_inputs.ContainsKey(input.Name))
                {
                    _inputs.Add(input.Name, new RouteInvokeInputItem()
                    {
                        Name = input.Name,
                        Desc = input.Desc,
                        DefaultValue = input.DefaultValue,
                        IsAllowEmpty = input.IsAllowEmpty,
                        Position = input.Position,
                        ValueType = input.ValueType
                    }
                    );
                }
            }
        }
        public RouteInvokeEntity Clone()
        {
            var rtn = new RouteInvokeEntity();
            rtn.InstanceType = InstanceType;
            rtn.InvokeMethod = InvokeMethod;
            rtn.InvokeName = InvokeName;
            rtn.Route = Route;
            rtn.RouteDesc = RouteDesc;
            return rtn;
        }
        /// <summary>
        /// 输入参数的描述
        /// </summary>
        public class RouteInvokeInputItem
        {
            /// <summary>
            /// 参数名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 参数类型
            /// </summary>
            public string ValueType { get; set; }
            /// <summary>
            /// 参数描述
            /// </summary>
            public string Desc { get; set; }
            /// <summary>
            /// 默认值
            /// </summary>
            public object DefaultValue { get; set; }
            /// <summary>
            /// 是否允许为空（不输入）
            /// </summary>
            public bool IsAllowEmpty { get; set; }
            /// <summary>
            /// 参数位置
            /// </summary>
            public string Position { get; set; }
        }
        /// <summary>
        /// 输出描述
        /// </summary>
        public class RouteInvokeOutputDesc
        {
            /// <summary>
            /// 返回结果描述
            /// </summary>
            public string Desc { get; set; }
            /// <summary>
            /// 返回结果格式描述
            /// </summary>
            public string FormatDesc { get; set; }
            /// <summary>
            /// 返回结构数据格式类型
            /// </summary>
            public string ReturnType { get; set; }
        }

    }
}
