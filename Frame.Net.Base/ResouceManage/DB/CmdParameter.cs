using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;


namespace EFFC.Frame.Net.Base.ResouceManage.DB
{
    #region 執行參數類-CmdParameter
    /// <summary>
    /// 執行參數類
    /// </summary>
    [Serializable]
    public sealed class CmdParameter
    {
        string _name = "";
        object _value = null;
        DbType _type = DbType.String;
        int _length = -1;
        ParameterDirection _direction = ParameterDirection.Input;

        private CmdParameter() { }
        /// <summary>
        /// 創建一個實例對象
        /// </summary>
        /// <param name="name">參數名稱</param>
        /// <param name="value">參數值</param>
        /// <returns></returns>
        public static CmdParameter NewInstance(string name, object value)
        {
            CmdParameter rtn = new CmdParameter();
            rtn.ParameterName = name;
            rtn.ParameterValue = value;
            rtn.Direction = ParameterDirection.Input;

            return rtn;
        }
        /// <summary>
        /// 創建一個實例對象
        /// </summary>
        /// <param name="name">參數名稱</param>
        /// <param name="value">參數值</param>
        /// <param name="direction">參數指向</param>
        /// <returns></returns>
        public static CmdParameter NewInstance(string name, object value, ParameterDirection direction)
        {
            CmdParameter rtn = new CmdParameter();
            rtn.ParameterName = name;
            rtn.ParameterValue = value;
            rtn.Direction = direction;

            return rtn;
        }
        /// <summary>
        /// 創建一個實例對象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static CmdParameter NewInstance(string name, object value, DbType type)
        {
            CmdParameter rtn = new CmdParameter();
            rtn.ParameterName = name;
            rtn.ParameterValue = value;
            rtn.DataType = type;

            return rtn;
        }
        /// <summary>
        /// 創建一個實例對象
        /// </summary>
        /// <param name="name">參數名称</param>
        /// <param name="value">參數值</param>
        /// <param name="datatype">參數類型，如果參數为返回值類型则必须指定</param>
        /// <param name="length">參數长度，如果參數为返回值類型则必须指定</param>
        /// <returns></returns>
        public static CmdParameter NewInstance(string name, object value, DbType datatype, int length)
        {
            CmdParameter rtn = new CmdParameter();
            rtn.ParameterName = name;
            rtn.ParameterValue = value;
            rtn.DataType = datatype;
            rtn.DataLength = length;

            return rtn;
        }
        /// <summary>
        /// 創建一個實例對象
        /// </summary>
        /// <param name="name">參數名称</param>
        /// <param name="value">參數值</param>
        /// <param name="datatype">參數類型，如果參數为返回值類型则必须指定</param>
        /// <param name="length">參數长度，如果參數为返回值類型则必须指定</param>
        /// <param name="direction">參數指向</param>
        /// <returns></returns>
        public static CmdParameter NewInstance(string name, object value, DbType datatype, int length, ParameterDirection direction)
        {
            CmdParameter rtn = new CmdParameter();
            rtn.ParameterName = name;
            rtn.ParameterValue = value;
            rtn.Direction = direction;
            rtn.DataType = datatype;
            rtn.DataLength = length;

            return rtn;
        }
        /// <summary>
        /// 參數名稱
        /// </summary>
        public string ParameterName
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        /// <summary>
        /// 參數值
        /// </summary>
        public object ParameterValue
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        /// <summary>
        /// 參數的指向，輸入還是輸出
        /// </summary>
        public ParameterDirection Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
            }
        }
        /// <summary>
        /// 数据類型，如果该參數为返回值類型则必须指定類型和长度
        /// </summary>
        public DbType DataType
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        /// <summary>
        /// 数据類型，如果该參數为返回值類型则必须指定類型和长度
        /// </summary>
        public int DataLength
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
            }
        }
    }
    #endregion
}
