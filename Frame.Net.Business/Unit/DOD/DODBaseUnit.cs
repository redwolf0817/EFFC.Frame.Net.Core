using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Data.DataConvert;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace EFFC.Frame.Net.Business.Unit.DOD
{
    /// <summary>
    /// DOD作业主要针对各种查询操作，因此在开发时注意使用with(nolock)模式以解决事务锁的问题
    /// </summary>
    public abstract partial class DODBaseUnit : MyDynamicMetaProvider, IUnit<DODParameter>
    {
        private DODParameter _p;
        private ResourceManage _rm;
        private TransactionToken _token;
        private static object lockobj = new object();
        private string _uuid = Guid.NewGuid().ToString();

        public DODBaseUnit()
        {
        }
        public DODBaseUnit(DODParameter p)
        {
            _p = p;
        }

        public Base.Data.DataCollection DoOperate(DODParameter p)
        {
            _p = p;
            InstanceID = ComFunc.nvl(p.ExtentionObj.instanceid);
            return GetPropertyValue();
        }

        private DOCollection GetPropertyValue()
        {
            _rm = (ResourceManage)_p.GetValue(ParameterKey.RESOURCE_MANAGER);
            _token = (TransactionToken)_p.GetValue(ParameterKey.TOKEN);
            DOCollection rtn = null;
            lock (lockobj)
            {
                rtn = GetCache(_p.PropertyName);
                if (rtn == null)
                {
                    Init(_p);
                    var action = GetSourceDefinition(_p.PropertyName);
                    if (action != null)
                    {
                        CacheSetting setting = new CacheSetting();
                        rtn = action.Invoke(setting);

                        if (setting.IsCache)
                        {
                            SetCache(_p.PropertyName, rtn, setting.CacheExpiration, setting.CacheSlidingExpiration);
                        }
                    }
                    After(_p);
                }
            }
            return rtn;
        }
        /// <summary>
        /// 初始化
        /// 当属性值从cache中获取时则不会调用
        /// </summary>
        /// <param name="p"></param>
        protected virtual void Init(DODParameter p)
        {

        }
        /// <summary>
        /// 唯一ID标识
        /// </summary>
        public string UUID
        {
            get
            {
                return _uuid;
            }
        }
        /// <summary>
        /// 获取属性来源定义
        /// </summary>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        protected abstract Func<CacheSetting, DOCollection> GetSourceDefinition(string propertyname);
        /// <summary>
        /// 获取实例编号
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual string InstanceID { get; set; }
        /// <summary>
        /// 结尾处理
        /// 当属性值从cache中获取时则不会调用
        /// </summary>
        /// <param name="p"></param>
        protected virtual void After(DODParameter p)
        {

        }
        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="propertyname"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        /// <param name="sliding_expiration"></param>
        public virtual void SetCache(string propertyname, DOCollection value, DateTime expiration, TimeSpan sliding_expiration)
        {
            HttpRuntime.Cache.Insert(this.GetType().FullName + "." + propertyname + "." + InstanceID, value, null, expiration, sliding_expiration);
        }
        /// <summary>
        /// 获取缓存的属性数据
        /// </summary>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public virtual DOCollection GetCache(string propertyname)
        {
            if (HttpRuntime.Cache[this.GetType().FullName + "." + propertyname + "." + InstanceID] != null)
            {
                return (DOCollection)HttpRuntime.Cache[this.GetType().FullName + "." + propertyname + "." + InstanceID];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="propert"></param>
        protected virtual void RemoveCache(string propertyname)
        {
            HttpRuntime.Cache.Remove(this.GetType().FullName + "." + propertyname);
        }

        public class CacheSetting
        {
            bool _iscache = true;
            DateTime _expiration = Cache.NoAbsoluteExpiration;
            TimeSpan _sliding_expiration = TimeSpan.FromMinutes(30);
            string _extentionkey = "";
            /// <summary>
            /// 是否缓存，默认为是
            /// </summary>
            public bool IsCache
            {
                get
                {
                    return _iscache;
                }
                set
                {
                    _iscache = value;
                }
            }
            /// <summary>
            /// 在指定的时间过期，默认为永不过期
            /// </summary>
            public DateTime CacheExpiration
            {
                get
                {
                    return _expiration;
                }
                set
                {
                    _expiration = value;
                }
            }
            /// <summary>
            /// 多长时间不用就会过期，默认为30分钟
            /// </summary>
            public TimeSpan CacheSlidingExpiration
            {
                get
                {
                    return _sliding_expiration;
                }
                set
                {
                    _sliding_expiration = value;
                }
            }
        }

        protected override object SetMetaValue(string key, object value)
        {
            return this;
        }

        protected override object GetMetaValue(string key)
        {
            _p.PropertyName = key;
            var v = GetPropertyValue();
            if (v.ListValue != null)
            {
                return v.ListValue;
            }
            else
            {
                return v.PropertyValue.Value;
            }
        }

        protected override object InvokeMe(string methodInfo, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                foreach (var val in args)
                {
                    if (val is KeyValuePair<string, object>)
                    {
                        var item = (KeyValuePair<string, object>)val;
                        _p.SetValue(item.Key, item.Value);
                    }
                }
            }
            _p.PropertyName = methodInfo;
            var v = GetPropertyValue();
            if (v.ListValue != null)
            {
                return v.ListValue;
            }
            else
            {
                return v.PropertyValue.Value;
            }
        }
    }
}
