using EFFC.Frame.Net.Base.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.ResouceManage.JsEngine
{
    /// <summary>
    /// 给hostjs使用的代码锁资源
    /// </summary>
    public class HostJsLockObejctEntity : IResourceEntity
    {
        string _workarea = "";
        public enum LockState
        {
            Free,
            Locked
        }
        /// <summary>
        /// 当前作用区域
        /// </summary>
        public string CurrentWorkArea
        {
            get
            {
                return _workarea;
            }
        }

        public HostJsLockObejctEntity(string workarea)
        {
            _workarea = workarea;
        }

        string _id = "lockobject_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        static Dictionary<string, Dictionary<string, LockItem>> _dicobj = new Dictionary<string, Dictionary<string, LockItem>>();
        static object lockobj = new object();
        public string ID
        {
            get { return _id; }
        }
        /// <summary>
        /// 根据lock名称锁定一个对象
        /// </summary>
        /// <param name="itemname"></param>
        public void Lock(string itemname)
        {
            LockItem lockitem = null;
            lock (lockobj)
            {
                Dictionary<string, LockItem> dicitems = null;
                if (_dicobj.ContainsKey(CurrentWorkArea))
                {
                    dicitems = _dicobj[CurrentWorkArea];

                }
                else
                {
                    dicitems = new Dictionary<string, LockItem>();
                    _dicobj.Add(CurrentWorkArea, dicitems);
                }

                if (dicitems.ContainsKey(itemname))
                {
                    lockitem = dicitems[itemname];
                }
                else
                {
                    lockitem = new LockItem(itemname);
                    dicitems.Add(lockitem.Name, lockitem);
                }
            }
            System.Threading.Monitor.Enter(lockitem);
            lockitem.State = LockState.Locked;
        }
        /// <summary>
        /// 释放指定名称的锁
        /// </summary>
        /// <param name="itemname"></param>
        public void Free(string itemname)
        {
            LockItem lockitem = null;
            lock (lockobj)
            {
                Dictionary<string, LockItem> dicitems = null;
                if (_dicobj.ContainsKey(CurrentWorkArea))
                {
                    dicitems = _dicobj[CurrentWorkArea];

                }
                else
                {
                    dicitems = new Dictionary<string, LockItem>();
                    _dicobj.Add(CurrentWorkArea, dicitems);
                }

                if (dicitems.ContainsKey(itemname))
                {
                    lockitem = dicitems[itemname];
                }
                else
                {
                    lockitem = new LockItem(itemname);
                    dicitems.Add(lockitem.Name, lockitem);
                }
            }
            if (System.Threading.Monitor.IsEntered(lockitem))
            {
                System.Threading.Monitor.Exit(lockitem);
                lockitem.State = LockState.Free;
            }
        }

        public void Release()
        {
            if (_dicobj.ContainsKey(CurrentWorkArea))
            {
                var dics = _dicobj[CurrentWorkArea];
                foreach (var item in dics)
                {
                    if (System.Threading.Monitor.IsEntered(item.Value))
                    {
                        System.Threading.Monitor.Exit(item.Value);
                        item.Value.State = LockState.Free;
                    }
                }
            }
        }

        protected class LockItem
        {
            public LockItem(string name)
            {
                State = LockState.Free;
                Name = name;
            }
            /// <summary>
            /// 对象的状态
            /// </summary>
            public LockState State
            {
                get;
                set;
            }
            /// <summary>
            /// 对象的名称
            /// </summary>
            public string Name
            {
                get;
                set;
            }
        }
    }
}
