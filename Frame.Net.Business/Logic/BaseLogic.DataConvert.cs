using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EFFC.Frame.Net.Base.AttributeDefine;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using EFFC.Frame.Net.Base.Interfaces.Unit;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Business.Unit;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;


namespace EFFC.Frame.Net.Business.Logic
{
    public partial class BaseLogic<P, D>
    {
        public class DataConvertor
        {
            /// <summary>
            /// 数据类型转化，将obj转化成T类型数据
            /// </summary>
            /// <typeparam name="Convert">转换器</typeparam>
            /// <typeparam name="T">目标数据类型</typeparam>
            /// <param name="obj">待转换数据</param>
            /// <returns></returns>
            public static T ConvertTo<Convert, T>(object obj) where Convert : IDataConvert<T>
            {
                var c = Activator.CreateInstance<Convert>();
                return c.ConvertTo(obj);
            }
            /// <summary>
            /// 数据类型转化，将From转化成To类型数据
            /// </summary>
            /// <typeparam name="Convert">转换器</typeparam>
            /// <typeparam name="From">来源类型</typeparam>
            /// <typeparam name="To">目标类型</typeparam>
            /// <param name="obj">待转换数据</param>
            /// <returns></returns>
            public static To ConvertTo<Convert, From, To>(From obj) where Convert : IDataConvert<From, To>
            {
                var c = Activator.CreateInstance<Convert>();
                return c.ConvertTo(obj);
            }
        }
    }
}

