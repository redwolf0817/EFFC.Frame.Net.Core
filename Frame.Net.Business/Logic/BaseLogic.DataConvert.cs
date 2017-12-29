using System;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;


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

