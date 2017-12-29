using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Unit.DB.Datas
{
    public class DBDataCollection:DataCollection
    {
        /// <summary>
        /// 查询过后返回的数据集
        /// </summary>
        public DataSetStd ReturnDataSet
        {
            get
            {
                return (DataSetStd)GetValue("ReturnDataSet");
            }
            set
            {
                SetValue("ReturnDataSet", value);
            }
        }
        /// <summary>
        /// 获取返回的数据集，并进行类型转化
        /// </summary>
        /// <typeparam name="T">IDataConvert</typeparam>
        /// <typeparam name="E">返回的数据类型</typeparam>
        /// <returns></returns>
        public E GetReturnDataList<T, E>() where T : IDataConvert<E>
        {
            return this.GetValue<T, E>("ReturnDataSet");
        }
        /// <summary>
        /// 执行是否成功
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return (bool)GetValue("IsSuccess");
            }
            set
            {
                SetValue("IsSuccess", value);
            }
        }
    }
}
