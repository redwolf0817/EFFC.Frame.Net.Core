using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using EFFC.Frame.Net.Module.Web.Logic;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Logic
{
    public partial class RestInvokeFilterLogic
    {
        MyDBHelper _db;
        /// <summary>
        /// db操作相关
        /// </summary>
		public new MyDBHelper DB
        {
            get
            {
                if (_db == null)
                    _db = new MyDBHelper(this);

                return _db;
            }
        }

        public class MyDBHelper : DBHelper
        {
            RestInvokeFilterLogic _logic = null;

            public MyDBHelper(RestInvokeFilterLogic logic)
                : base(logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// 开启指定db，并开启链接
            /// </summary>
            /// <typeparam name="TAccess"></typeparam>
            /// <param name="dbconn">db链接串</param>
            /// <returns></returns>
            public UnitParameter NewDBUnitParameter<TAccess>(string dbconn)
                where TAccess : IDBAccessInfo, ITransaction, IResourceEntity
            {
                var rtn = base.NewDBUnitParameter<TAccess>();
                rtn.Dao.Open(dbconn);
                return rtn;
            }
            /// <summary>
            /// 用默认链接串开启指定的DB
            /// </summary>
            /// <typeparam name="TAccess"></typeparam>
            /// <returns></returns>
            public UnitParameter NewDefaultDBUnitParameter<TAccess>()
                where TAccess : IDBAccessInfo, ITransaction, IResourceEntity
            {
                return NewDBUnitParameter<TAccess>(_logic.CallContext_Parameter.DBConnectionString);

            }
        }
    }
}
