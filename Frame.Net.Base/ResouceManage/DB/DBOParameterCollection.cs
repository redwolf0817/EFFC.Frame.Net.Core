using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Data;

namespace EFFC.Frame.Net.Base.ResouceManage.DB
{
    [Serializable]
    public sealed class DBOParameterCollection : Dictionary<string, CmdParameter>
    {
        public void Add(string name, object value)
        {
            if (!this.ContainsKey(name))
            {
                if (value is CmdParameter)
                {
                    base.Add(name, (CmdParameter)value);
                }
                else
                {
                    base.Add(name, CmdParameter.NewInstance(name, value));
                }
            }
        }

        /// <summary>
        /// 添加一個新的參數
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="d"></param>
        public void Add(string name, object value, ParameterDirection d)
        {
            if (!this.ContainsKey(name))
            {
                base.Add(name, CmdParameter.NewInstance(name, value, d));
            }
        }
        /// <summary>
        /// 添加一個新的參數
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public void Add(string name, object value, DbType type)
        {
            if (!this.ContainsKey(name))
            {
                base.Add(name, CmdParameter.NewInstance(name, value, type));
            }
        }
        /// <summary>
        /// 添加一個新的參數
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="datatype"></param>
        /// <param name="length"></param>
        public void Add(string name, object value, DbType datatype, int length)
        {
            if (!this.ContainsKey(name))
            {
                base.Add(name, CmdParameter.NewInstance(name, value, datatype, length));
            }
        }
        /// <summary>
        /// 添加一個新的參數
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="datatype"></param>
        /// <param name="length"></param>
        /// <param name="d"></param>
        public void Add(string name, object value, DbType datatype, int length, ParameterDirection d)
        {
            if (!this.ContainsKey(name))
            {
                base.Add(name, CmdParameter.NewInstance(name, value, datatype, length, d));
            }
        }
        /// <summary>
        /// 給一個已有的參數賦值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="d"></param>
        public void SetValue(string name, object value, ParameterDirection d)
        {
            if (this.ContainsKey(name))
            {
                this[name].ParameterValue = value;
                this[name].Direction = d;
            }
            else
            {
                Add(name, value, d);
            }
        }
        public void SetValue(string name, object value)
        {
            if (this.ContainsKey(name))
            {
                this[name].ParameterValue = value;
            }
            else
            {
                Add(name, value);
            }
        }

    }
}
