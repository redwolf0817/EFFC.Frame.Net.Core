using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;

namespace EFFC.Frame.Net.Base.Parameter
{
    /// <summary>
    /// 参数集合用于架构中逐层参数的传递-不提供线程安全处理
    /// </summary>
    [Serializable]
    public class ParameterStd : BaseCollection
    {
        private Dictionary<string, object> _d = new Dictionary<string, object>();
        TransactionTokenCollection _transtokelist = new TransactionTokenCollection();
        new object lockobj = new object();
        public ParameterStd()
        {
            SetValue(ParameterKey.TOKEN, TransactionToken.NewToken());
            SetValue(ParameterKey.RESOURCE_MANAGER, new ResourceManage());
        }
        /// <summary>
        /// 事务token列表
        /// </summary>
        public TransactionTokenCollection TransTokenList
        {
            get
            {
                return _transtokelist;
            }
        }
        /// <summary>
        /// 资源管理器
        /// </summary>
        public ResourceManage Resources
        {
            get
            {
                return GetValue<ResourceManage>(ParameterKey.RESOURCE_MANAGER);
            }
        }
        /// <summary>
        /// 当前事务token
        /// </summary>
        public TransactionToken CurrentTransToken
        {
            get
            {
                if (GetValue(ParameterKey.TOKEN) == null)
                {
                    var defaulttoken = TransactionToken.NewToken();
                    this.TransTokenList.Add(defaulttoken);
                    SetValue(ParameterKey.TOKEN, defaulttoken);
                }
                else
                {
                    TransactionToken token = (TransactionToken)GetValue(ParameterKey.TOKEN);
                    if (token.IsExpired)
                    {
                        var defaulttoken = TransactionToken.NewToken();
                        this.TransTokenList.Add(defaulttoken);
                        SetValue(ParameterKey.TOKEN, defaulttoken);
                    }
                }

                return (TransactionToken)GetValue(ParameterKey.TOKEN);
            }
        }

        protected override object GetValue_Inner(string key)
        {
            if (_d.ContainsKey(key))
            {
                return _d[key];
            }
            else
            {
                return null;
            }
        }

        protected override void SetValue_Inner(string key, object value)
        {
            if (_d.ContainsKey(key))
            {
                _d[key] = value;
            }
            else
            {
                _d.Add(key, value);
            }

        }

        protected override void Remove_Inner(string key)
        {
            _d.Remove(key);
        }

        protected override void Clear_Inner()
        {
            _d.Clear();
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetMyEnumerator()
        {
            return _d.GetEnumerator();
        }

        protected override List<string> Keys_Inner
        {
            get { return _d.Keys.ToList(); }
        }

        public override void Dispose()
        {
            base.Dispose();
            _transtokelist.Clear();
            _transtokelist = null;
        }

        public override T Clone<T>()
        {
            var rtn = base.Clone<T>();
            rtn.SetValue(ParameterKey.TOKEN, TransactionToken.NewToken());
            rtn.SetValue(ParameterKey.RESOURCE_MANAGER, new ResourceManage());
            return rtn;
        }

        public override object Clone()
        {
            return this.Clone<ParameterStd>();
        }
    }
}
