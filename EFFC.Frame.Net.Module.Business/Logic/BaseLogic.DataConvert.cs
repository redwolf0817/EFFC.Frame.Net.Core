using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Business.Logic
{
    public partial class BaseLogic<PType, DType>
    {
        /// <summary>
        /// 进行数据格式转换
        /// </summary>
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
