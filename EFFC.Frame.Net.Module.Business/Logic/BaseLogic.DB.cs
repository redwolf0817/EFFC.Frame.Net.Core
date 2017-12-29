using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Unit.DB.Unit;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Business.Logic
{
    public partial class BaseLogic<PType, DType>
    {
        private DBHelper _db;
        /// <summary>
        /// db操作相关
        /// </summary>
        public virtual DBHelper DB
        {
            get
            {
                if (_db == null)
                    _db = new DBHelper(this);

                return _db;
            }
        }

        public class DBHelper
        {
            BaseLogic<PType, DType> _logic;

            public DBHelper() { }
            public DBHelper(BaseLogic<PType, DType> logic)
            {
                _logic = logic;
            }

            
            /// <summary>
            /// 获得一个新的UnitParamter
            /// </summary>
            /// <returns></returns>
            public T NewUnitParameter<T>() where T : UnitParameter
            {
                var t = Activator.CreateInstance<T>();
                t.SetValue(ParameterKey.TOKEN, _logic.CallContext_Parameter[ParameterKey.TOKEN]);
                t.SetValue(ParameterKey.RESOURCE_MANAGER, _logic.CallContext_Parameter[ParameterKey.RESOURCE_MANAGER]);
                foreach (var item in _logic.CallContext_Parameter.Domain(DomainKey.CONFIG))
                {
                    t.SetValue(DomainKey.CONFIG, item.Key, item.Value);
                }
                return t;
            }
            /// <summary>
            /// 获得一个新的UnitParamter，默认Dao为DB类型的DBAccess
            /// </summary>
            /// <typeparam name="DB"></typeparam>
            /// <returns></returns>
            public UnitParameter NewDBUnitParameter<DB>() where DB : IDBAccessInfo, ITransaction, IResourceEntity
            {
                UnitParameter _up = NewUnitParameter<UnitParameter>();
                _up.Dao = _logic.CallContext_ResourceManage.CreateInstance<DB>(_logic.CallContext_CurrentToken);
                _up.DBConnString = ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.CONFIG, ParameterKey.DBCONNECT_STRING]);
                return _up;
            }
            /// <summary>
            /// 获得一个新的UnitParamter
            /// </summary>
            /// <returns></returns>
            public virtual UnitParameter NewDBUnitParameter()
            {
                UnitParameter _up = NewUnitParameter<UnitParameter>();
                return _up;
            }
            /// <summary>
            /// 标准化DB执行操作
            /// </summary>
            /// <param name="p"></param>
            /// <param name="express"></param>
            /// <returns></returns>
            public virtual UnitDataCollection Excute(UnitParameter p, DBExpress express)
            {
                return DBUnitProxy.Excute(p, express);
            }
            /// <summary>
            /// 通过json对象执行标准化DB操作
            /// </summary>
            /// <param name="p"></param>
            /// <param name="json"></param>
            /// <returns></returns>
            public virtual UnitDataCollection Excute(UnitParameter p, FrameDLRObject json)
            {
                return DBUnitProxy.Excute(p, json);
            }
            /// <summary>
            /// 通过json串执行标准化DB操作
            /// </summary>
            /// <param name="p"></param>
            /// <param name="json"></param>
            /// <returns></returns>
            public virtual UnitDataCollection Excute(UnitParameter p, string json)
            {
                return Excute(p, FrameDLRObject.CreateInstance(json));
            }

            /// <summary>
            /// 查询操作
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="p"></param>
            /// <param name="actionflag">动作区分标记</param>
            /// <returns></returns>
            public virtual UnitDataCollection Query<T>(UnitParameter p, string actionflag) where T : IDBUnit<UnitParameter>
            {
                return DBUnitProxy.Query<T>(p, actionflag);
            }
            /// <summary>
            /// 翻页查询
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="p"></param>
            /// <param name="actionflag">动作区分标记</param>
            /// <returns></returns>
            public virtual UnitDataCollection QueryByPage<T>(UnitParameter p, string actionflag) where T : IDBUnit<UnitParameter>
            {
                return DBUnitProxy.QueryByPage<T>(p, actionflag);
            }
            /// <summary>
            /// 非查询类的db操作
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="p"></param>
            /// <param name="actionflag">动作区分标记</param>
            public virtual void NonQuery<T>(UnitParameter p, string actionflag) where T : IDBUnit<UnitParameter>
            {
                DBUnitProxy.NonQuery<T>(p, actionflag);
            }
            /// <summary>
            /// 执行存储过程操作
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="p"></param>
            /// <param name="actionflag">动作区分标记</param>
            /// <returns></returns>
            public virtual UnitDataCollection ExcuteSP<T>(UnitParameter p, string actionflag) where T : IDBUnit<UnitParameter>
            {
                return DBUnitProxy.ExcuteSP<T>(p,actionflag);
            }
        }
    }
}
