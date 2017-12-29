using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Business.Logic;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using EFFC.Frame.Net.Module.Web.Logic;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB.Parameters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Logic
{
    /// <summary>
    /// 验证用的logic
    /// </summary>
    public partial class AuthorizationLogic
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
            AuthorizationLogic _logic = null;

            public MyDBHelper(AuthorizationLogic logic)
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
