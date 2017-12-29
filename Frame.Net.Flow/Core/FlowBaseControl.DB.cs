using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Interfaces.Unit;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Business.Unit;
using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.Parameters.Flow;
using EFFC.Frame.Net.Data.UnitData;
using EFFC.Frame.Net.Flow.Exceptions;
using EFFC.Frame.Net.Flow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Flow.Core
{
    public partial class FlowBaseControl : BaseModule<FlowParameter, FlowData>
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
            FlowBaseControl _control;

            public DBHelper() { }
            public DBHelper(FlowBaseControl control)
            {
                _control = control;
            }

            /// <summary>
            /// 获得一个新的UnitParamter
            /// </summary>
            /// <returns></returns>
            public T NewUnitParameter<T>() where T : UnitParameter
            {
                var t = Activator.CreateInstance<T>();
                t.SetValue("__token", _control._p.CurrentTransToken);
                t.SetValue("__rm", _control._p.Resources);
                foreach (var item in _control._p.Domain(DomainKey.CONFIG))
                {
                    t.SetValue("__config", item.Key, item.Value);
                }
                return t;
            }
            /// <summary>
            /// 获得一个新的UnitParamter，默认Dao为DB类型的DBAccess
            /// </summary>
            /// <typeparam name="DB"></typeparam>
            /// <returns></returns>
            public UnitParameter NewDBUnitParameter<DB>() where DB : ADBAccess, IResourceEntity
            {
                UnitParameter _up = NewUnitParameter<UnitParameter>();
                _up.Dao = _control._p.Resources.CreateInstance<DB>(_control._p.CurrentTransToken);
                _up.DBConnString = ComFunc.nvl(_control._p[DomainKey.CONFIG, ParameterKey.DBCONNECT_STRING]);
                return _up;
            }
            /// <summary>
            /// 获得一个新的UnitParamter
            /// </summary>
            /// <returns></returns>
            public UnitParameter NewDBUnitParameter()
            {
                UnitParameter _up = NewUnitParameter<UnitParameter>();
                return _up;
            }
            /// <summary>
            /// 将Logic的数据写入到UnitParameter
            /// </summary>
            /// <param name="ld"></param>
            /// <param name="up"></param>
            public void SetUnitParameter(FlowParameter ld, UnitParameter up)
            {
                foreach (var v in ld.Domain(DomainKey.INPUT_PARAMETER))
                {
                    up.SetValue(v.Key, ComFunc.nvl(v.Value));
                }
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
                p.SetValue("_unit_action_flag_", actionflag);
                return (UnitDataCollection)UnitProxy.Call<QueryUnit<T>>(p);
            }
            /// <summary>
            /// 非查询类的db操作
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="p"></param>
            /// <param name="actionflag">动作区分标记</param>
            public virtual void NonQuery<T>(UnitParameter p, string actionflag) where T : IDBUnit<UnitParameter>
            {
                p.SetValue("_unit_action_flag_", actionflag);
                UnitProxy.Call<NonQueryUnit<T>>(p);
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
                p.SetValue("_unit_action_flag_", actionflag);
                return (UnitDataCollection)UnitProxy.Call<SPUnit<T>>(p);
            }
            public virtual void InsertIntoSqlServer(UnitParameter p, string tablename, FrameDLRObject obj)
            {
                StringBuilder sbsql = new StringBuilder();
                sbsql.Append("insert into " + tablename + "(");
                var values = "";
                var columns = "";
                DBOParameterCollection dpc = new DBOParameterCollection();
                foreach (var s in obj.Keys)
                {
                    dpc.SetValue(s, obj.GetValue(s));
                    columns += columns.Length > 0 ? "," + s : s;
                    values += values.Length > 0 ? ",@" + s : "@" + s;
                }
                sbsql.Append(columns + ")values(");
                sbsql.Append(values);
                sbsql.Append(");");
                if (p.Dao is ADBAccess)
                {
                    ((ADBAccess)p.Dao).ExecuteNoQuery(sbsql.ToString(), dpc);
                }
            }
            public virtual void InsertIntoOracle(UnitParameter p, string tablename, FrameDLRObject obj)
            {
                StringBuilder sbsql = new StringBuilder();
                sbsql.Append("insert into " + tablename + "(");
                var values = "";
                var columns = "";
                DBOParameterCollection dpc = new DBOParameterCollection();
                foreach (var s in obj.Keys)
                {
                    dpc.SetValue(s, obj.GetValue(s));
                    columns += columns.Length > 0 ? "," + s : s;
                    values += values.Length > 0 ? ",:" + s : ":" + s;
                }
                sbsql.Append(columns + ")values(");
                sbsql.Append(values);
                sbsql.Append(");");

                if (p.Dao is ADBAccess)
                {
                    ((ADBAccess)p.Dao).ExecuteNoQuery(sbsql.ToString(), dpc);
                }
            }
        }
    }
}
