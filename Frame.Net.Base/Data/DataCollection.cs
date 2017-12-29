using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using System.Collections.Concurrent;
using EFFC.Frame.Net.Base.Exceptions;

namespace EFFC.Frame.Net.Base.Data
{
    /// <summary>
    /// 数据集合用于架构中各个层次间的数据交换-提供线程安全处理
    /// </summary>
    [Serializable]
    public class DataCollection : BaseCollection
    {
        protected ConcurrentDictionary<string, object> _d = new ConcurrentDictionary<string, object>();
        #region 并发操作
        /// <summary>
        /// 在并发情况下尝试获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool TryGetValue(string key,out object o)
        {
            return _d.TryGetValue(key, out o);
        }
        /// <summary>
        /// 在并发情况下尝试写入数据，如果数据存在则尝试新增数据，否则尝试更新数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySetValue(string key, object value)
        {
            object o = null;
            if (_d.TryGetValue(key,out o))
            {
                return _d.TryUpdate(key, value, o);
            }
            else
            {
                return _d.TryAdd(key, value);
            }
        }
        /// <summary>
        /// 在并发情况下尝试移除数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool TryRemove(string key, out object o)
        {
            return _d.TryRemove(key, out o);
        }
        #endregion
        #region implement
        /// <summary>
        /// 获取数据，如果在并发情况下获取数据失败则返回null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetValue_Inner(string key)
        {
            object o = null;
            if (_d.TryGetValue(key, out o))
            {
                return o;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 写入数据，如果在并发情况下写入失败而抛出异常SyncException
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected override void SetValue_Inner(string key, object value)
        {
            object o = null;
            if (_d.TryGetValue(key, out o))
            {
                if (!_d.TryUpdate(key, value, o))
                {
                    throw new SyncException(string.Format("Update {0}'s value failed with syncronized mode", key));
                }
            }
            else
            {
                if (!_d.TryAdd(key, value))
                {
                    throw new SyncException(string.Format("Add {0}'s value failed with syncronized mode", key));
                }
            }
        }
        /// <summary>
        /// 移除数据，如果在并发情况下失败而抛出异常SyncException
        /// </summary>
        /// <param name="key"></param>
        protected override void Remove_Inner(string key)
        {
            object o = null;
            if (!_d.TryRemove(key, out o))
            {
                throw new SyncException(string.Format("Remove {0}'s value failed with syncronized mode", key));
            }
        }
        /// <summary>
        /// 清空数据，为非线程安全处理
        /// </summary>
        protected override void Clear_Inner()
        {
            _d.Clear();
        }

        protected override List<string> Keys_Inner
        {
            get
            {
                return _d.Keys.ToList();
            }
        }
        protected override IEnumerator<KeyValuePair<string, object>> GetMyEnumerator()
        {
            return _d.GetEnumerator();
        }
        #endregion

        public override object Clone()
        {
            return this.Clone<DataCollection>();
        }
    }
}
