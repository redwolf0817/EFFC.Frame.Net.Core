using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Token;

namespace EFFC.Frame.Net.Base.ResouceManage
{
    public class ResourceManage : IDistributeTransaction,IDisposable
    {
        protected class ResourceCollection
        {
            Dictionary<string, IResourceEntity> _d = new Dictionary<string, IResourceEntity>();

            public IResourceEntity this[string key]
            {
                get
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
            }

            public void Add(string key,IResourceEntity r)
            {
                if (!_d.ContainsKey(key))
                {
                    _d.Add(key, r);
                }
            }

            public void Release()
            {
                foreach (IResourceEntity r in _d.Values)
                {
                    r.Release();
                }

                _d.Clear();
            }

            public List<IResourceEntity> Values
            {
                get
                {
                    return _d.Values.ToList();
                }
            }
        }

        Dictionary<string, IResourceEntity> _d = new Dictionary<string, IResourceEntity>();
        Dictionary<string, ResourceCollection> _transd = new Dictionary<string, ResourceCollection>();
        static object lockobj = new object();
        /// <summary>
        /// 创建一个默认生命周期的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateInstance<T>() where T : IResourceEntity
        {
            return CreateInstance<T>("__global__");
        }
        /// <summary>
        /// 创建一个资源
        /// </summary>
        /// <typeparam name="T">IResourceEntity资源类型</typeparam>
        /// <param name="lifeCycleToken">生命周期token</param>
        /// <returns></returns>
        public T CreateInstance<T>(string lifeCycleToken) where T : IResourceEntity
        {
            Type t = typeof(T);
            T rtn = (T)Activator.CreateInstance(t, true);
            AddEntity(lifeCycleToken, (IResourceEntity)rtn);
            return rtn;
        }
        public T CreateInstance<T>(TransactionToken transToken) where T : IResourceEntity,ITransaction
        {
            if (transToken.IsExpired)
            {
                throw new Exception("Token已经过期无法创建资源！");
            }

            Type t = typeof(T);
            T rtn = (T)Activator.CreateInstance(t, true);
            
            if (rtn is ITransaction)
            {
                AddEntity(transToken, (IResourceEntity)rtn);
                if (transToken.CurrentStatus == TransactionToken.TransStatus.Begin)
                {
                    ((ITransaction)rtn).BeginTransaction(transToken.IsolationLevel);
                }
            }
            else
            {
                throw new Exception(t.FullName + "不是ITransaction");
            }
            
            return rtn;
        }

        /// <summary>
        /// 新增资源对象
        /// </summary>
        /// <param name="lifeCycleToken">生命周期token</param>
        /// <param name="e">资源实例</param>
        public void AddEntity(string lifeCycleToken, IResourceEntity e)
        {
            string key = GenerateKey(e.ID, lifeCycleToken);
            if (!_d.ContainsKey(key))
            {
                _d.Add(key, e);
            }
            else
            {
                _d[key] = e;
            }
        }

        /// <summary>
        /// 新增资源对象
        /// </summary>
        /// <param name="token">生命周期token</param>
        /// <param name="e">资源实例</param>
        public void AddEntity(TransactionToken token, IResourceEntity e)
        {
            if (!_transd.ContainsKey(token.UniqueID))
            {
                ResourceCollection rc = new ResourceCollection();
                _transd.Add(token.UniqueID, rc);
            }
            _transd[token.UniqueID].Add(e.ID, e);
        }
        /// <summary>
        /// 释放默认生命周期下的资源
        /// </summary>
        public void Release()
        {
            Release("__global__");
        }
        /// <summary>
        /// 资源释放
        /// </summary>
        /// <param name="lifeCycleToken"></param>
        public void Release(string lifeCycleToken)
        {
            lock (lockobj)
            {
                List<string> keys = _d.Keys.ToList<string>();
                foreach (string key in keys)
                {
                    if (key.StartsWith(lifeCycleToken) && _d[key] != null)
                    {
                        _d[key].Release();
                        _d[key] = null;
                        _d.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="token">TransactionToken</param>
        public void Release(TransactionToken token)
        {
            lock (lockobj)
            {
                if (_transd.ContainsKey(token.UniqueID))
                {
                    foreach (IResourceEntity e in _transd[token.UniqueID].Values)
                    {
                        e.Release();
                    }
                }
                token.Release();
                this._transd.Remove(token.UniqueID);
            }
        }
        /// <summary>
        /// 释放所有资源
        /// </summary>
        public void ReleaseAll()
        {
            lock (lockobj)
            {
                List<string> keys = _d.Keys.ToList<string>();
                foreach (string key in keys)
                {
                    if (_d[key] != null)
                    {
                        _d[key].Release();
                        _d[key] = null;
                        _d.Remove(key);
                    }
                }
                foreach (string tk in _transd.Keys)
                {
                    _transd[tk].Release();
                }
            }
        }

        protected string GenerateKey(string id, string lifeCycleToken)
        {
            string rtn = lifeCycleToken + "#" + id;
            return rtn;
        }

        public void BeginTransaction(TransactionToken token)
        {
            if (token.IsExpired)
            {
                throw new Exception("当前Token已经过期,无法使用事务");
            }

            if (token.CurrentStatus == TransactionToken.TransStatus.Begin)
            {
                throw new Exception("当前事务已经开启");
            }
            if (_transd.ContainsKey(token.UniqueID))
            {
                foreach (IResourceEntity e in _transd[token.UniqueID].Values)
                {
                    if (e is ITransaction)
                        ((ITransaction)e).BeginTransaction(token.IsolationLevel);
                }
            }
            token.Begin();

        }

        public void CommitTransaction(TransactionToken token)
        {
            if (token.IsExpired)
            {
                throw new Exception("当前Token已经过期,无法使用事务");
            }

            if (token.CurrentStatus == TransactionToken.TransStatus.Begin)
            {
                if (_transd.ContainsKey(token.UniqueID))
                {
                    foreach (IResourceEntity e in _transd[token.UniqueID].Values)
                    {
                        if (e is ITransaction)
                            ((ITransaction)e).CommitTransaction();
                    }
                }
                token.Commit();
            }

            
        }

        public void RollbackTransaction(TransactionToken token)
        {
            if (token.IsExpired)
            {
                throw new Exception("当前Token已经过期,无法使用事务");
            }

            if (token.CurrentStatus == TransactionToken.TransStatus.Begin)
            {
                if (_transd.ContainsKey(token.UniqueID))
                {
                    foreach (IResourceEntity e in _transd[token.UniqueID].Values)
                    {
                        if (e is ITransaction)
                            ((ITransaction)e).RollbackTransaction();
                    }
                }
                token.RollBack();
            }

            

        }

        public void Dispose()
        {
            ReleaseAll();
            _d.Clear();
        }
    }
}
