using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.Common;
using Microsoft.SqlServer.Server;
using Frame.Net.Base.Interfaces.DataConvert;
using System.Collections;
using System.Data.Common;

namespace EFFC.Frame.Net.Base.Data
{
    public class DataTableStd : DbDataReader, ICloneable
    {

        protected List<string> pkContent = new List<string>();
        List<DataRow> _rows = new List<DataRow>();
        DataRow _schema = new DataRow();
        DataRow _curent = null;

        public DataTableStd()
        {
            pkContent.Clear();
        }

        public object Clone()
        {
            DataTableStd rtn = new DataTableStd();
            foreach (var s in this.pkContent)
            {
                rtn.pkContent.Add(s);
            }

            return rtn;
        }
        /// <summary>
        /// 獲取或者設置值
        /// </summary>
        /// <param name="x">行</param>
        /// <param name="y">列</param>
        /// <returns></returns>
        public object this[int x, string y]
        {
            get
            {
                object rtn = GetValue(y, x);
                return rtn;
            }
            set
            {
                SetValue(value, y, x);
            }
        }
        /// <summary>
        /// 獲取或者設置值
        /// </summary>
        /// <param name="x">行</param>
        /// <param name="y">列</param>
        /// <returns></returns>
        public object this[int x, int y]
        {
            get
            {
                object rtn = GetValue(y, x);
                return rtn;
            }
            set
            {
                SetValue(value, y, x);
            }
        }
        /// <summary>
        /// 返回所有的数据行的clone版本
        /// </summary>
        public List<DataRow> Rows {
            get
            {
                var rtn = new List<DataRow>();
                foreach(var item in _rows)
                {
                    rtn.Add((DataRow)item.Clone());
                }
                return rtn;
            }
        }
        public void ClearData()
        {
            this._rows.Clear();
        }

        /// <summary>
        /// 本表的行數
        /// </summary>
        public int RowLength
        {
            get
            {
                return this._rows.Count;
            }
        }
        /// <summary>
        /// 返回指定欄位的內容的MaxLength
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int ColumnMaxLength(string columnName)
        {
            return this._rows[0][columnName].Length;
        }
        /// <summary>
        /// 返回指定欄位的内容的MaxLength
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public int ColumnMaxLength(int columnIndex)
        {
            return this._rows[0][columnIndex].Length;
        }
        /// <summary>
        /// 返回指定欄位的数据类型
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Type ColumnDateType(string columnName)
        {
            return Type.GetType(this._rows[0][columnName].DataType);
        }
        /// <summary>
        /// 返回指定欄位的数据类型
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public Type ColumnDateType(int columnIndex)
        {
            return Type.GetType(this._rows[0][columnIndex].DataType);
        }

        /// <summary>
        /// 添加一个新的列
        /// </summary>
        /// <param name="p"></param>
        public void AddColumn(DataColumn p)
        {
            if (_schema.Contains(p.ColumnName))
            {
                return;
            }

            DataColumn dc = (DataColumn)p.Clone();

            this._schema.SetColumn(dc);
            foreach (var row in _rows)
            {
                row.SetColumn((DataColumn)dc.Clone());
            }
            if (_new_row != null)
            {
                _new_row.SetColumn((DataColumn)dc.Clone());
            }
        }
        /// <summary>
        /// 添加一个新的列
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="dbtype"></param>
        /// <param name="maxlength"></param>
        /// <param name="value"></param>
        public void AddColumn(string columnName, string dbtype, int maxlength, object value)
        {
            if (_schema.Contains(columnName))
            {
                return;
            }

            DataColumn dc = new DataColumn();
            dc.ColumnName = columnName;
            dc.DataType = value == null ? "" : value.GetType().FullName;
            if (dc.DataType == typeof(string).FullName)
            {
                dc.Length = maxlength;
            }
            dc.Value = value;

            this._schema.SetColumn(dc);
            foreach (var row in _rows)
            {
                row.SetColumn((DataColumn)dc.Clone());
            }
            if (_new_row != null)
            {
                _new_row.SetColumn((DataColumn)dc.Clone());
            }
        }
        /// <summary>
        /// 移除指定列
        /// </summary>
        /// <param name="columnName"></param>
        public void RemoveColumn(string columnName)
        {
            _schema.RemoveColumn(columnName);
            foreach (var row in _rows)
            {
                row.RemoveColumn(columnName);
            }
            if (_new_row != null)
            {
                _new_row.RemoveColumn(columnName);
            }
        }
        /// <summary>
        /// 判断欄位是否为自增长
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public bool isAutoIncrement(int columnIndex)
        {
            return this._schema[columnIndex].IsAutoIncrement;
        }
        /// <summary>
        /// 判断欄位是否为自增长
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool isAutoIncrement(string columnName)
        {
            return this._schema[columnName].IsAutoIncrement;
        }

        /// <summary>
        /// 在指定的位置設置值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="columnName"></param>
        /// <param name="rowIndex"></param>
        public void SetValue(object value, string columnName, int rowIndex)
        {
            if (rowIndex >= this._rows.Count)
            {
                return;
            }
            else if (!this._schema.Contains(columnName))
            {
                return;
            }
            else
            {

                this._rows[rowIndex][columnName].Value = value;

            }
        }
        /// <summary>
        /// 在指定的位置設置值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        public void SetValue(object value, int columnIndex, int rowIndex)
        {
            if (rowIndex >= this._rows.Count)
            {
                return;
            }
            else if (this._schema.ColumnCount <= columnIndex)
            {
                return;
            }
            else
            {

                this._rows[rowIndex][columnIndex].Value = value;


            }
        }
        /// <summary>
        /// 将from表中的数据写入到本表中，按照欄位名稱对应，Append方式
        /// </summary>
        /// <param name="from"></param>
        public void SetValueApppend_From(DataTableStd from)
        {
            for (int i = 0; i < from.RowLength; i++)
            {
                foreach (DataColumn dc in from._schema)
                {
                    if (this._schema.Contains(dc.ColumnName)
                        && this._schema[dc.ColumnName].Length == dc.Length
                        && this._schema[dc.ColumnName].DataType == dc.DataType)
                    {
                        this.SetNewRowValue(from[i, dc.ColumnName], dc.ColumnName);
                    }
                }
                this.AddNewRow();
            }

        }
        /// <summary>
        /// 给指定的列赋值-所有行
        /// </summary>
        /// <param name="value"></param>
        /// <param name="columnName"></param>
        public void SetValueByColumn(object value, string columnName)
        {
            for (int i = 0; i < this.RowLength; i++)
            {
                this[i, columnName] = value;
            }
        }
        /// <summary>
        /// 给指定的列赋值-所有行
        /// </summary>
        /// <param name="value"></param>
        /// <param name="columnIndex"></param>
        public void SetValueByColumn(object value, int columnIndex)
        {
            for (int i = 0; i < this.RowLength; i++)
            {
                this[i, columnIndex] = value;
            }
        }

        /// <summary>
        /// 根據columnName和rowIndex獲取值
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public object GetValue(string columnName, int rowIndex)
        {
            if (this == null)
            {
                return null;
            }
            else if (rowIndex >= this._rows.Count)
            {
                return null;
            }
            else if (!this._schema.Contains(columnName))
            {
                return null;
            }
            else
            {
                if (this._rows[rowIndex][columnName] != null && this._rows[rowIndex][columnName].Value != null)
                {
                    if (this._schema[columnName].DataType == typeof(string).FullName)
                    {
                        return ComFunc.nvl(this._rows[rowIndex][columnName].Value);
                    }
                    else
                    {
                        return this._rows[rowIndex][columnName].Value;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 根據columnName和rowIndex獲取值
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public object GetValue(int columnIndex, int rowIndex)
        {
            if (this == null)
            {
                return null;
            }
            else if (rowIndex >= this._rows.Count)
            {
                return null;
            }
            else if (columnIndex >= this._schema.ColumnCount)
            {
                return null;
            }
            else
            {
                if (this._rows[rowIndex][columnIndex] != null && this._rows[rowIndex][columnIndex].Value != null)
                {
                    if (this._schema[columnIndex].DataType == typeof(string).FullName)
                    {
                        return ComFunc.nvl(this._rows[rowIndex][columnIndex].Value);
                    }
                    else
                    {
                        return this._rows[rowIndex][columnIndex].Value;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        DataRow _new_row = null;
        /// <summary>
        /// 新增一個臨時行
        /// </summary>
        public void NewRow()
        {
            this._new_row = (DataRow)_schema.Clone();
        }
        /// <summary>
        /// 給新增行寫值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="columnName"></param>
        public void SetNewRowValue(object value, string columnName)
        {
            if (this._new_row == null) NewRow();
            if (columnName == null)
            {
                return;
            }
            else if (!this._schema.Contains(columnName))
            {
                return;
            }
            else
            {

                _new_row[columnName].Value = value;

            }

        }
        /// <summary>
        /// 給新增行寫值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="columnIndex"></param>
        public void SetNewRowValue(object value, int columnIndex)
        {
            if (this._new_row == null) NewRow();

            if (columnIndex >= this._schema.ColumnCount)
            {
                return;
            }
            else
            {

                _new_row[columnIndex].Value = value;

            }

        }
        /// <summary>
        /// 獲取临时新增行的数据
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public object GetNewRowValue(string columnName)
        {
            if (this._new_row == null)
            {
                return null;
            }
            else if (!this._schema.Contains(columnName))
            {
                return null;
            }
            else
            {
                if (this._new_row[columnName] != null && this._new_row[columnName].Value != null)
                {
                    if (this._schema[columnName].DataType == typeof(string).FullName)
                    {
                        return ComFunc.nvl(this._new_row[columnName].Value);
                    }
                    else
                    {
                        return this._new_row[columnName].Value;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 獲取临时新增行的数据
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public object GetNewRowValue(int columnIndex)
        {
            if (this._new_row == null)
            {
                return null;
            }
            else if (columnIndex >= this._schema.ColumnCount)
            {
                return null;
            }
            else
            {
                if (this._new_row[columnIndex] != null && this._new_row[columnIndex].Value != null)
                {
                    if (this._schema[columnIndex].DataType == typeof(string).FullName)
                    {
                        return ComFunc.nvl(this._new_row[columnIndex].Value);
                    }
                    else
                    {
                        return this._new_row[columnIndex].Value;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 將新增的臨時行Add到table中
        /// </summary>
        public void AddNewRow()
        {
            this._rows.Add(_new_row);
            _new_row = null;
        }

        /// <summary>
        /// 獲得该表的PK，dt必须带schema
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataColumn[] GetPK(DataTableStd dt)
        {
            if (dt == null) return null;

            return dt.PK;
        }

        public DataColumn[] PK
        {
            get
            {
                var rtn = new List<DataColumn>();
                foreach (var dr in _schema)
                {
                    if (dr.IsPK)
                    {
                        rtn.Add(dr);
                    }
                }

                return rtn.ToArray();
            }
        }

        /// <summary>
        /// 獲得该表的PK的名稱列表，dt必须带schema
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string[] GetPKName(DataTableStd dt)
        {
            DataColumn[] dc = GetPK(dt);
            string[] rtn = new string[dc.Length];
            for (int i = 0; i < dc.Length; i++)
            {
                rtn[i] = dc[i].ColumnName;
            }

            return rtn;
        }

        public string[] PKNames
        {
            get { return DataTableStd.GetPKName(this); }
        }
        /// <summary>
        /// 獲得该表的Columns的名稱列表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string[] GetColumnName(DataTableStd dt)
        {
            string[] rtn = new string[dt._schema.ColumnCount];
            for (int i = 0; i < dt._schema.ColumnCount; i++)
            {
                rtn[i] = dt._schema[i].ColumnName;
            }

            return rtn;
        }

        public string[] ColumnNames
        {
            get { return DataTableStd.GetColumnName(this); }
        }

        public override int Depth => 0;

        public override int FieldCount => this._schema.ColumnCount;

        public override bool HasRows => this._rows.Count > 0;

        public override bool IsClosed => true;

        public override int RecordsAffected => throw new NotImplementedException();

        public override object this[string name] => throw new NotImplementedException();

        public override object this[int ordinal] => throw new NotImplementedException();

        /// <summary>
        /// 找出Dt中RowCount
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int RowNumber(DataTableStd dt)
        {
            int rtn = 0;

            if (dt != null)
            {
                rtn = dt._rows.Count;
            }

            return rtn;
        }
        /// <summary>
        /// 复制本表的结构
        /// </summary>
        /// <returns></returns>
        public DataTableStd CloneStd()
        {
            return (DataTableStd)this.Clone();
        }

        /// <summary>
        /// 根據columns搜索与values中相同的数据，判断是否存在
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool IsExist(Dictionary<string, object> values)
        {
            bool rtn = true;

            if (RowNumber(this) <= 0)
            {
                rtn = false;
            }
            else
            {
                for (int i = 0; i < RowNumber(this); i++)
                {
                    bool tmp = true;
                    foreach (KeyValuePair<string, object> kvp in values)
                    {
                        tmp = tmp & (ComFunc.nvl(GetValue(kvp.Key, i)) == ComFunc.nvl(kvp.Value));
                    }
                    //如果為true，則表示有完全與values相同的資料存在，否則當前這行資料與values不相同，則瀏覽下一行資料
                    if (tmp)
                    {
                        rtn = true;
                        break;
                    }
                    else
                    {
                        rtn = rtn & tmp;
                    }
                }
            }

            return rtn;
        }

        /// <summary>
        /// clone一個DataRow
        /// </summary>
        /// <param name="rowindex"></param>
        /// <param name="todt"></param>
        public void CloneDataRow(int rowindex, ref DataTableStd todt)
        {
            todt.NewRow();
            foreach (DataColumn dc in this._schema)
            {
                object v = GetValue(dc.ColumnName, rowindex);
                todt.SetNewRowValue(v == null ? DBNull.Value : v, dc.ColumnName);
            }
            todt.AddNewRow();
        }
        /// <summary>
        /// 转化成标准类型
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static DataTableStd ParseStd(object o)
        {
            if (o == null)
            {
                return null;
            }
            else if(o is DataTableStd)
            {
                return (DataTableStd)o;
            }
            else if (o is DbDataReader)
            {
                var ddr = (DbDataReader)o;
                var rtn = new DataTableStd();
                while (ddr.Read())
                {
                    if (!ddr.CanGetColumnSchema())
                    {
                        
                    }
                    else
                    {
                        var cschema = ddr.GetColumnSchema();
                        rtn.NewRow();
                        for (int i = 0; i < ddr.FieldCount; i++)
                        {
                            rtn.AddColumn(new DataColumn()
                            {
                                ColumnName = cschema[i].ColumnName,
                                DataDBType = cschema[i].DataTypeName,
                                DataType = cschema[i].DataType.FullName,
                                IsAllowNull = cschema[i].AllowDBNull == null ? false : cschema[i].AllowDBNull.Value,
                                IsAutoIncrement = cschema[i].IsAutoIncrement == null ? false : cschema[i].IsAutoIncrement.Value,
                                IsPK = cschema[i].IsKey == null ? false : cschema[i].IsKey.Value,
                                Length = cschema[i].ColumnSize == null ? -1 : cschema[i].ColumnSize.Value
                            });

                            rtn.SetNewRowValue(ddr[cschema[i].ColumnName], cschema[i].ColumnName);
                        }
                    }
                    

                    rtn.AddNewRow();
                }

                return rtn;
            }
            else
            {
                return (DataTableStd)o;
            }
        }

        /// <summary>
        /// 根據指定column个数转化成标准类型
        /// </summary>
        /// <param name="columnCount"></param>
        /// <returns></returns>
        public static DataTableStd ParseStd(int columnCount)
        {
            DataTableStd dt = new DataTableStd();

            for (int i = 0; i < columnCount; i++)
            {
                dt._schema.SetColumn(DataColumn.CreatInstanse("F" + i));
            }

            return dt;
        }

        /// <summary>
        /// 根據指定columns转化成标准类型
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static DataTableStd ParseStd(string[] columns)
        {
            DataTableStd dt = new DataTableStd();

            for (int i = 0; i < columns.Length; i++)
            {
                dt._schema.SetColumn(DataColumn.CreatInstanse(columns[i]));
            }

            return dt;
        }

        /// <summary>
        /// 将table中的指定一行的数据转化成字符串
        /// </summary>
        /// <param name="splitComm"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public string ToString(string splitComm, int rowIndex)
        {
            string str = "";
            foreach (DataColumn dc in this._schema)
            {
                str = str.Length > 0 ? str + splitComm + GetValue(dc.ColumnName, rowIndex) : str + GetValue(dc.ColumnName, rowIndex);
            }

            return str;
        }
        /// <summary>
        /// 将table中的Header转化成字符串
        /// </summary>
        /// <param name="splitComm"></param>
        /// <returns></returns>
        public string HeaderToString(string splitComm)
        {
            string str = "";
            foreach (DataColumn dc in this._schema)
            {
                str = str.Length > 0 ? str + splitComm + dc.ColumnName : str + dc.ColumnName;
            }

            return str;
        }

        /// <summary>
        /// 将table中的数据转化成字符串（数据量大的时候不建议使用）
        /// </summary>
        /// <param name="splitComm"></param>
        /// <param name="isIncHeader"></param>
        /// <returns></returns>
        public StringBuilder ToString(string splitComm, bool isIncHeader)
        {
            StringBuilder rtn = new StringBuilder();
            if (isIncHeader)
            {
                rtn.AppendLine(HeaderToString(splitComm));
            }

            for (int i = 0; i < this.RowLength; i++)
            {
                rtn.AppendLine(this.ToString(splitComm, i));
            }

            return rtn;
        }

        public override bool GetBoolean(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override decimal GetDecimal(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override double GetDouble(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override float GetFloat(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetInt32(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetInt64(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override string GetName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override string GetString(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(int ordinal)
        {
            return _curent[ordinal].Value;
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override bool NextResult()
        {
            throw new NotImplementedException();
        }
        int _currentIndex = 0;
        public override bool Read()
        {
            if (_currentIndex >= this.RowLength) return false;

            _curent = _rows[_currentIndex];
            _currentIndex++;
            return true;
        }
    }
    /// <summary>
    /// Data的行定义
    /// </summary>
    public class DataRow: ICloneable,IEnumerable<DataColumn>
    {
        Dictionary<string, DataColumn> _lcolumn = new Dictionary<string, DataColumn>();
        static object lockobj = new object();
        /// <summary>
        /// 获取或设置存储在由名称指定的列中的数据。
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public DataColumn this[string columnName]
        {
            get
            {
                if (_lcolumn.ContainsKey(columnName.ToLower()))
                {
                    return _lcolumn[columnName.ToLower()];
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取或设置存储在由索引指定的列中的数据。
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public DataColumn this[int columnIndex]
        {
            get
            {
                lock (lockobj)
                {
                    var key = Index2Key(columnIndex);
                    if (key != "")
                    {
                        return _lcolumn[key];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        /// <summary>
        /// 写入一个Column
        /// </summary>
        /// <param name="column"></param>
        public void SetColumn(DataColumn column)
        {
            lock (lockobj)
            {
                if (column != null)
                {
                    if (_lcolumn.ContainsKey(column.ColumnName.ToLower()))
                    {
                        _lcolumn[column.ColumnName.ToLower()] = null;
                        _lcolumn[column.ColumnName.ToLower()] = column;
                    }
                    else
                    {
                        _lcolumn.Add(column.ColumnName.ToLower(), column);
                    }
                }
            }
        }
        /// <summary>
        /// 写入一个Column
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="dbmaxlength"></param>
        public void SetColumn(string name, string dbtype, object value, int dbmaxlength = -1)
        {
            lock (lockobj)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (_lcolumn.ContainsKey(name.ToLower()))
                    {
                        _lcolumn[name.ToLower()].DataDBType = dbtype;
                        _lcolumn[name.ToLower()].Value = value;
                    }
                    else
                    {
                        _lcolumn.Add(name.ToLower(), DataColumn.CreatInstanse(name, value.GetType().Name, dbtype, value, dbmaxlength));
                    }
                }
            }
        }
        /// <summary>
        /// 获取Column的数量
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return _lcolumn.Keys.Count;
            }
        }

        private string Index2Key(int index)
        {
            var array = new string[_lcolumn.Count];
            _lcolumn.Keys.CopyTo(array, 0);
            return array.Length > index ? array[index] : "";
        }

        public object Clone()
        {

            var rtn = new DataRow();
            lock (lockobj)
            {
                foreach (var item in this)
                {
                    rtn.SetColumn((DataColumn)item.Clone());
                }
            }
            return rtn;

        }
        /// <summary>
        /// 判断是否含有columnName的列
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool Contains(string columnName)
        {
            return _lcolumn.ContainsKey(columnName.ToLower());
        }
        /// <summary>
        /// 移除一列
        /// </summary>
        /// <param name="columnName"></param>
        public void RemoveColumn(string columnName)
        {
            if(Contains(columnName))
            _lcolumn.Remove(columnName.ToLower());
        }

        public IEnumerator<DataColumn> GetEnumerator()
        {
            return this._lcolumn.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._lcolumn.Values.GetEnumerator();
        }
    }
    /// <summary>
    /// DataColumn定义
    /// </summary>
    public class DataColumn:DataCollection
    {
        /// <summary>
        /// String的Type描述
        /// </summary>
        public static string String_Type = "System.String";
        /// <summary>
        /// Int的Type描述
        /// </summary>
        public static string Int32_Type = "System.Int32";
        /// <summary>
        /// Int64的Type描述
        /// </summary>
        public static string Int64_Type = "System.Int64";
        /// <summary>
        /// Doube的Type描述
        /// </summary>
        public static string Double_Type = "System.Double";
        /// <summary>
        /// Float的Type描述
        /// </summary>
        public static string Float_Type = "System.Float";
        /// <summary>
        /// DateTime的Type描述
        /// </summary>
        public static string DateTime_Type = "System.DateTime";

        /// <summary>
        /// 創建一個實例，栏位类型為默认的String类型，长度為无限制，允許為空
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static DataColumn CreatInstanse(string column)
        {
            DataColumn cp = new DataColumn();
            cp.ColumnName = column;

            return cp;
        }
        /// <summary>
        /// 創建一個實例，長度為無限制，允許為空
        /// </summary>
        /// <param name="column"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataColumn CreatInstanse(string column, string type)
        {
            DataColumn cp = new DataColumn();
            cp.ColumnName = column;
            cp.DataType = type;

            return cp;
        }
        /// <summary>
        /// 創建一個實例，允許為空
        /// </summary>
        /// <param name="column"></param>
        /// <param name="type"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static DataColumn CreatInstanse(string column, string type, int maxLength)
        {
            DataColumn cp = new DataColumn();
            cp.ColumnName = column;
            cp.DataType = type;
            cp.Length = maxLength;

            return cp;
        }
        /// <summary>
        /// 創建一個實例，允許為空
        /// </summary>
        /// <param name="column"></param>
        /// <param name="type"></param>
        /// <param name="dbtype"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static DataColumn CreatInstanse(string column, string type,string dbtype,object value, int maxLength)
        {
            DataColumn cp = new DataColumn();
            cp.ColumnName = column;
            cp.DataType = type;
            cp.Length = maxLength;
            cp.DataDBType = dbtype;

            return cp;
        }

        public DataColumn()
        {
            this.SetValue("DataType", String_Type);
            this.SetValue("Length", -1);
            this.SetValue("IsAllowNull", true);
            this.SetValue("IsPK", false);
            this.SetValue("IsAutoIncrement", false);
            this.SetValue("AutoIncrementSeed", 0);
            if (!IsAutoIncrement)
            {
                this.SetValue("AutoIncrementStep", -1);
            }
            else
            {
                this.SetValue("AutoIncrementStep", 0);
            }
        }
        /// <summary>
        /// DBType
        /// </summary>
        public string DataDBType
        {
            get
            {
                return GetValue<string>("DataDBType");
            }
            set
            {
                SetValue("DataDBType", value);
            }
        }
        /// <summary>
        /// 栏位名称
        /// </summary>
        public string ColumnName
        {
            get
            {
                return GetValue<string>("ColumnName");
            }
            set
            {
                SetValue("ColumnName", value);
            }
        }
        /// <summary>
        /// 数据在.net中的数据类型
        /// </summary>
        public string DataType
        {
            get
            {
                return GetValue<string>("DataType");
            }
            set
            {
                SetValue("DataType", value);
            }
        }
        /// <summary>
        /// 数据长度
        /// </summary>
        public int Length
        {
            get
            {
                return GetValue<int>("Length");
            }
            set
            {
                SetValue("Length", value);
            }
        }
        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool IsAllowNull
        {
            get
            {
                return GetValue<bool>("IsAllowNull");
            }
            set
            {
                if (!this.IsPK)
                {
                    SetValue("IsAllowNull", value);
                }
                else
                {
                    SetValue("IsAllowNull", false);
                }
            }
        }
        /// <summary>
        /// 是否为PK
        /// </summary>
        public bool IsPK
        {
            get
            {
                return GetValue<bool>("IsPK");
            }
            set
            {
                SetValue("IsPK", value);
                if (value)
                {
                    IsAllowNull = false;
                }
            }
        }
        /// <summary>
        /// 是否为自增类型
        /// </summary>
        public bool IsAutoIncrement
        {
            get
            {
                return GetValue<bool>("IsAutoIncrement");
            }
            set
            {
                SetValue("IsAutoIncrement", value);
            }
        }
        /// <summary>
        /// 自增张种子
        /// </summary>
        public int AutoIncrementSeed
        {
            get
            {
                return GetValue<int>("AutoIncrementSeed");
            }
            set
            {
                SetValue("AutoIncrementSeed", value);
            }
        }
        /// <summary>
        /// 自增张幅度
        /// </summary>
        public int AutoIncrementStep
        {
            get
            {
                return GetValue<int>("AutoIncrementStep");
            }
            set
            {
                SetValue("AutoIncrementStep", value);
            }
        }
        /// <summary>
        /// 数据值
        /// </summary>
        public object Value
        {
            get
            {
                return GetValue("value");
            }
            set
            {
                SetValue("value", value);
                this.DataType = value.GetType().FullName;
            }
        }

        public override object Clone()
        {
            return Clone<DataColumn>();
        }
    }
}
