using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.ResouceManage.JsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EFFC.Frame.Net.Base.Data.Base
{
    /// <summary>
    /// 构建框架使用的数据集（线程安全）
    /// </summary>
    public abstract class BaseCollection : ISerializable, IXmlSerializable, ICloneable, IEnumerable<KeyValuePair<string, object>>,IDisposable
    {
        protected SerializationInfo _info;
        protected StreamingContext _context;
        protected object lockobj = new object();
        private string _uuid = Guid.NewGuid().ToString();
        /// <summary>
        /// Constructor
        /// </summary>
        public BaseCollection()
        {
        }
        #region 抽象方法
        /// <summary>
        /// 参数数据的唯一编号
        /// </summary>
        public string UUID
        {
            get
            {
                return _uuid;
            }
        }
        /// <summary>
        /// Keys
        /// </summary>
        protected abstract List<string> Keys_Inner
        {
            get;
        }
        /// <summary>
        /// setvalue实现
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected abstract void SetValue_Inner(string key, object value);
        /// <summary>
        /// getvalue的实现
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected abstract object GetValue_Inner(string key);
        /// <summary>
        /// remove value的实现
        /// </summary>
        /// <param name="key"></param>
        protected abstract void Remove_Inner(string key);
        /// <summary>
        /// clear all的实现
        /// </summary>
        protected abstract void Clear_Inner();
        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator<KeyValuePair<string, object>> GetMyEnumerator();
        #endregion
        /// <summary>
        /// Key数据集
        /// </summary>
        public List<string> Keys
        {
            get
            {
                lock (lockobj)
                {
                    return Keys_Inner;
                }
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>(string key)
        {
            var rtn = GetValue(key);
            if (rtn == null)
            {
                return default(T);
            }
            else
            {
                if (rtn is T)
                {
                    return (T)rtn;
                }
                else
                {
                    return default(T);
                }
            }
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue<T>(string key, T value)
        {
            lock (lockobj)
            {
                SetValue_Inner(key, value);
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetValue(string key)
        {
            return GetValue_Inner(key);
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">数据</param>
        public void SetValue(string key, object value)
        {
            lock (lockobj)
            {
                SetValue_Inner(key, value);
            }
        }
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            lock (lockobj)
            {
                Remove_Inner(key);
            }
        }
        /// <summary>
        /// 清空数据集
        /// </summary>
        public void Clear()
        {
            lock (lockobj)
            {
                Clear_Inner();
            }
        }
        /// <summary>
        /// 获取或设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                SetValue(key, value);
            }
        }
        /// <summary>
        /// 按照域获取或设置数据
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string domain, string key]
        {
            get
            {
                return GetValue(domain, key);
            }
            set
            {
                SetValue(domain, key, value);
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="domain">域</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public object GetValue(string domain, string key)
        {
            return GetValue(domain + "." + key);
        }
        /// <summary>
        /// 获取数据，通过数据转换器进行转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public E GetValue<T, E>(string key) where T : IDataConvert<E>
        {
            object obj = this[key];
            T t = (T)Activator.CreateInstance(typeof(T), true);
            return t.ConvertTo(obj);
        }
        /// <summary>
        /// 获取数据，通过数据转换器进行转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="domain"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public E GetValue<T, E>(string domain, string key) where T : IDataConvert<E>
        {
            object obj = this[domain, key];
            T t = (T)Activator.CreateInstance(typeof(T), true);
            return t.ConvertTo(obj);
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="domain">域</param>
        /// <param name="key">key</param>
        /// <param name="value">数据</param>
        public void SetValue(string domain, string key, object value)
        {
            SetValue(domain + "." + key, value);
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">数据</param>
        public void SetValue<T, E>(string key, E value) where T : IDataConvert<E>
        {
            object obj = this[key];
            T t = (T)Activator.CreateInstance(typeof(T), true);
            SetValue(key, t.ConvertTo(obj));
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="domain"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue<T, E>(string domain, string key, E value) where T : IDataConvert<E>
        {
            object obj = this[key];
            T t = (T)Activator.CreateInstance(typeof(T), true);
            SetValue(domain, key, t.ConvertTo(obj));
        }
        /// <summary>
        /// 抓取某个域下面的数据
        /// </summary>
        /// <param name="domain">域，要抓取多个域的话，每个域用逗号分隔</param>
        /// <returns></returns>
        public Dictionary<string, object> Domain(string domain)
        {
            List<KeyValuePair<string, object>> rtn = new List<KeyValuePair<string, object>>();
            //当多线程并发时会发生数据集被修改的情况
            lock (lockobj)
            {
                foreach (var s in domain.Split(','))
                {
                    rtn.AddRange(this.Where(c => c.Key.StartsWith(s + ".")));
                }
            }

            return rtn.Select(t => new { t.Key, t.Value }).ToDictionary(t => t.Key.Split('.')[1], t => t.Value);
        }
        /// <summary>
        /// clone
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            BaseCollection rtn = Clone<BaseCollection>();
            return rtn;
        }
        /// <summary>
        /// clone，对于内部元素的复制只复制实现了ICloneable的，其他的不复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Clone<T>() where T : BaseCollection
        {
            T rtn = (T)Activator.CreateInstance(typeof(T), true);
            lock (lockobj)
            {
                foreach (var s in this)
                {
                    if (s.Value is ICloneable)
                    {
                        rtn.SetValue(s.Key, ((ICloneable)s.Value).Clone());
                    }
                    else if (s.Value is DateTime)
                    {
                        rtn.SetValue(s.Key, new DateTime(((DateTime)s.Value).Ticks));
                    }
                    else if (s.Value is List<object>)
                    {
                        var to = new List<object>();
                        var from = (List<Object>)s.Value;
                        foreach (var item in from)
                        {
                            if (item is ICloneable)
                            {
                                to.Add(((ICloneable)item).Clone());
                            }
                        }
                        rtn.SetValue(s.Key, to);
                    }
                    else if (s.Value is List<string>)
                    {
                        var to = new List<string>();
                        var from = (List<string>)s.Value;
                        foreach (var item in from)
                        {
                            to.Add(item);
                        }
                        rtn.SetValue(s.Key, to);
                    }
                    else if (s.Value is List<FrameDLRObject>)
                    {
                        var to = new List<FrameDLRObject>();
                        var from = (List<FrameDLRObject>)s.Value;
                        foreach (var item in from)
                        {
                            to.Add((FrameDLRObject)item.Clone());
                        }
                    }
                    else if (s.Value is Dictionary<string, object>)
                    {
                        var to = new Dictionary<string, object>();
                        var from = (Dictionary<string, object>)s.Value;
                        foreach (var item in from)
                        {
                            if (item.Value is ICloneable)
                            {
                                to.Add(item.Key, ((ICloneable)item.Value).Clone());
                            }
                        }
                        rtn.SetValue(s.Key, to);
                    }
                    else if (s.Value is Dictionary<string, string>)
                    {
                        var to = new Dictionary<string, string>();
                        var from = (Dictionary<string, string>)s.Value;
                        foreach (var item in from)
                        {
                            to.Add(item.Key, item.Value);
                        }
                        rtn.SetValue(s.Key, to);
                    }
                    else if (s.Value is Dictionary<string, FrameDLRObject>)
                    {
                        var to = new Dictionary<string, FrameDLRObject>();
                        var from = (Dictionary<string, FrameDLRObject>)s.Value;
                        foreach (var item in from)
                        {
                            to.Add(item.Key, (FrameDLRObject)item.Value.Clone());
                        }
                        rtn.SetValue(s.Key, to);
                    }
                    else
                    {
                        rtn.SetValue(s.Key, s.Value);
                    }
                }
            }
            return rtn;
        }
        /// <summary>
        /// 删除value
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="key"></param>
        public void Remove(string domain, string key)
        {
            Remove(domain + "." + key);
        }
        /// <summary>
        /// 扩展属性
        /// </summary>
        public dynamic ExtentionObj
        {
            get
            {
                if (GetValue("ExtentionObj") == null)
                {
                    SetValue("ExtentionObj", FrameDLRObject.CreateInstance());
                }
                return (FrameDLRObject)GetValue("ExtentionObj");
            }
        }
        #region 序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="st"></param>
        public virtual void DoSerialization(SerializableType st)
        {
            if (st == SerializableType.Binary)
            {
                string keys = "";
                foreach (string key in Keys)
                {
                    _info.AddValue(key, GetValue(key));
                    string typename = "";
                    if (GetValue(key) != null)
                    {
                        typename = GetValue(key).GetType().FullName;
                    }
                    _info.AddValue(key + "_Type#", typename);
                    keys += keys.Length <= 0 ? key : ";" + key;
                }
                _info.AddValue("DatatdKeys#", keys);
            }
            else
            {

            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="st"></param>
        public virtual void DeSerialization(SerializableType st)
        {
            if (st == SerializableType.Binary)
            {
                string keys = _info.GetString("DatatdKeys#");
                string[] keyarray = keys.Split(';');
                foreach (string key in keyarray)
                {
                    string typename = _info.GetString(key + "_Type#");
                    if (!string.IsNullOrEmpty(typename))
                    {
                        Type t = Type.GetType(_info.GetString(key + "_Type#"));
                        SetValue(key, _info.GetValue(key, t));
                    }
                    else
                    {
                        SetValue(key, null);
                    }
                }
            }
            else
            {

            }
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _info = info;
            _context = context;
            DoSerialization(SerializableType.Binary);
        }
        private BaseCollection(SerializationInfo info, StreamingContext context)
        {
            _info = info;
            _context = context;
            DeSerialization(SerializableType.Binary);
        }
        /// <summary>
        /// xml序列化
        /// </summary>
        /// <returns></returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        /// <summary>
        /// xml反序列化
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            DeSerialization(SerializableType.Xml);
        }
        /// <summary>
        /// xml序列化
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            DoSerialization(SerializableType.Xml);
        }

        #endregion


        /// <summary>
        /// 可枚举循环
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return GetMyEnumerator();
        }
        /// <summary>
        /// 可枚举循环
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetMyEnumerator();
        }
        /// <summary>
        /// 数据资源释放
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var item in this)
            {
                if (item.Value is IDisposable)
                {
                    ((IDisposable)item.Value).Dispose();
                }
            }

            this.Clear();
        }
    }
}
