using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Base.Token
{
    [Serializable]
    public sealed class TransactionToken
    {
        public enum TransStatus
        {
            None,
            Begin,
            Commit,
            Rollback
        }

        private string _id = "";
        private int _expired = 30;
        private bool _isExpired = false;
        private DateTime _starttime = DateTime.Now;
        private TransStatus _transstatus = TransStatus.None;
        private FrameIsolationLevel _isolevel = FrameIsolationLevel.Default;

        public string UniqueID
        {
            get
            {
                return _id;
            }
        }

        private TransactionToken()
        {
            _id = Guid.NewGuid().ToString();
            _expired = 30;
            _starttime = DateTime.Now;
            _isolevel = FrameIsolationLevel.Default;
        }
        /// <summary>
        /// 当前事务的状态
        /// </summary>
        public TransStatus CurrentStatus
        {
            get
            {
                return _transstatus;
            }
        }
        /// <summary>
        /// 当前事务隔离级别
        /// </summary>
        public FrameIsolationLevel IsolationLevel
        {
            get
            {
                return _isolevel;
            }
        }

        /// <summary>
        /// 获取一个新的Token
        /// </summary>
        /// <returns></returns>
        public static TransactionToken NewToken()
        {
            TransactionToken rtn = new TransactionToken();
            return rtn;
        }
        /// <summary>
        /// 获取一个新的token，并设定失效时间
        /// </summary>
        /// <param name="expiredminutes"></param>
        /// <returns></returns>
        public static TransactionToken NewToken(int expiredminutes)
        {
            TransactionToken rtn = new TransactionToken();
            rtn._expired = expiredminutes;
            return rtn;
        }
        /// <summary>
        /// 获取一个新的token，并设定隔离级别
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static TransactionToken NewToken(FrameIsolationLevel level)
        {
            TransactionToken rtn = new TransactionToken();
            rtn._isolevel = level;
            return rtn;
        }
        /// <summary>
        /// 获取一个新的token，并设定隔离级别和失效时间
        /// </summary>
        /// <param name="level"></param>
        /// <param name="expiredminutes"></param>
        /// <returns></returns>
        public static TransactionToken NewToken(FrameIsolationLevel level, int expiredminutes)
        {
            TransactionToken rtn = new TransactionToken();
            rtn._isolevel = level;
            rtn._expired = expiredminutes;
            return rtn;
        }
        /// <summary>
        /// 判断本token是否已经失效
        /// </summary>
        public bool IsExpired
        {
            get
            {
                if (DateTime.Now.CompareTo(_starttime.AddMinutes(_expired)) < 0)
                {
                    _isExpired = false;
                }
                else
                {
                    _isExpired = true;
                }

                return _isExpired;
            }
        }
        /// <summary>
        /// 释放token
        /// </summary>
        public void Release()
        {
            _isExpired = true;
        }

        /// <summary>
        /// 转换成串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string rtn = "";
            rtn = ComFunc.Base64Code(this._id + ";" + this._expired + ";" + this._starttime.ToFileTimeUtc() + ";" + ComFunc.Enum2String<TransStatus>(this._transstatus)) + ";" + _isExpired;
            return rtn;
        }
        /// <summary>
        /// 判断一个串是否就是自己，即相同
        /// </summary>
        /// <param name="s">合法的token串</param>
        /// <returns></returns>
        public bool IsMe(string s)
        {
            string me = ToString();
            if (me == s)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Begin()
        {
            if (_transstatus != TransStatus.Begin)
                _transstatus = TransStatus.Begin;
            else
            {
                throw new Exception("事务已经启动!");
            }
        }

        public void Commit()
        {
            if (_transstatus == TransStatus.Begin)
            {
                _transstatus = TransStatus.Commit;
                _isExpired = true;
            }
            //else
            //{
            //    throw new Exception("事务未启动!");
            //}
        }

        public void RollBack()
        {
            if (_transstatus == TransStatus.Begin)
            {
                _transstatus = TransStatus.Rollback;
                _isExpired = true;
            }
            //else
            //{
            //    throw new Exception("事务未启动!");
            //}
        }
    }
}
