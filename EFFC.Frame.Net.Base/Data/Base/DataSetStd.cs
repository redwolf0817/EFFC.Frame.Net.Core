using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Frame.Net.Base.Interfaces.DataConvert;
using System.Linq;

namespace EFFC.Frame.Net.Base.Data
{
    /// <summary>
    /// DataSet标准化定义
    /// </summary>
    public class DataSetStd : ICloneable,IDisposable
    {
        List<DataTableStd> _tables = new List<DataTableStd>();
        /// <summary>
        /// 获取指定的table
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataTableStd this[int index]
        {
            get
            {
                return _tables[index];
            }
        }
        /// <summary>
        /// 获取table列表
        /// </summary>
        public List<DataTableStd> Tables
        {
            get
            {
                return _tables;
            }
        }
        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var rtn = new DataSetStd();
            foreach (var item in this._tables)
            {
                rtn._tables.Add((DataTableStd)item.Clone());
            }
            return rtn;
        }

        /// <summary>
        /// 根据columnName和rowIndex获取指定table中的值
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="columnName"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public object GetValue(int tableIndex, string columnName, int rowIndex)
        {
            if (this == null)
            {
                return "";
            }
            else if (tableIndex >= _tables.Count)
            {
                return "";
            }
            else
            {
                return this.Tables[tableIndex].GetValue(columnName, rowIndex);
            }
        }
        /// <summary>
        /// 根据columnIndex和rowIndex获取指定table中的值
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public object GetValue(int tableIndex, int columnIndex, int rowIndex)
        {
            if (this == null)
            {
                return "";
            }
            else if (tableIndex >= this.Tables.Count)
            {
                return "";
            }
            else
            {
                return this.Tables[tableIndex].GetValue(columnIndex, rowIndex);
            }
        }
        /// <summary>
        /// 根据columnName和rowIndex获取第一个table中的值
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public object GetValue(string columnName, int rowIndex)
        {

            return GetValue(0, columnName, rowIndex);

        }
        /// <summary>
        /// 根据columnIndex和rowIndex获取第一个table中的值
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public object GetValue(int columnIndex, int rowIndex)
        {
            return GetValue(0, columnIndex, rowIndex);
        }
        
        /// <summary>
        /// 从游标中填充数据。用于协助没有提供DataAdapter的驱动使用
        /// </summary>
        /// <param name="ddr"></param>
        /// <returns></returns>
        public static DataSetStd FillData(DbDataReader ddr)
        {
            var rtn = new DataSetStd();
            if (ddr != null)
            {
                var index = 0;
                do
                {
                    var dt = DataTableStd.ParseStd(ddr);
                    rtn._tables.Add(dt);
                } while (ddr.NextResult());
            }
            return rtn;
        }
        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            if (_tables != null)
            {
                foreach (var item in _tables)
                {
                    item.Dispose();
                }
                this._tables.Clear();
            }
            _tables = null;
            
        }
    }
}
