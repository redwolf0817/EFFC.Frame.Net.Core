using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Interfaces.Unit;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Business.Unit;
using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.Parameters.Flow;
using EFFC.Frame.Net.Data.UnitData;
using EFFC.Frame.Net.Flow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Flow.Core
{
    public partial class ConditionBaseDriver:IConditionDriver<FlowParameter,FlowData>
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
            ConditionBaseDriver _driver;

            public DBHelper() { }
            public DBHelper(ConditionBaseDriver driver)
            {
                _driver = driver;
            }

            /// <summary>
            /// 創建一個普通的資源對象
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public T NewResourceEntity<T>() where T : IResourceEntity
            {
                return _driver._p.Resources.CreateInstance<T>(this.GetHashCode().ToString());
            }
            /// <summary>
            /// 創建一個事務資源對象
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public T NewTransResourceEntity<T>() where T : IResourceEntity, ITransaction
            {
                return _driver._p.Resources.CreateInstance<T>(_driver._p.CurrentTransToken);
            }

            /// <summary>
            /// 获得一个新的UnitParamter
            /// </summary>
            /// <returns></returns>
            public T NewUnitParameter<T>() where T : UnitParameter
            {
                var t = Activator.CreateInstance<T>();
                t.SetValue("__token", _driver._p.CurrentTransToken);
                t.SetValue("__rm", _driver._p.Resources);
                foreach (var item in _driver._p.Domain(DomainKey.CONFIG))
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
                _up.Dao = _driver._p.Resources.CreateInstance<DB>(_driver._p.CurrentTransToken);
                _up.DBConnString = ComFunc.nvl(_driver._p[DomainKey.CONFIG, ParameterKey.DBCONNECT_STRING]);
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
            public void SetUnitParameter(FlowLogicData ld, UnitParameter up)
            {
                foreach (var v in ld)
                {
                    up.SetValue(v.Key, ComFunc.nvl(v.Value));
                }
            }
            /// <summary>
            /// 将Logic的数据写入到UnitParameter
            /// </summary>
            /// <param name="ld"></param>
            /// <param name="up"></param>
            public void SetUnitParameter(FlowLogicData ld, UnitParameter up, string domain)
            {
                foreach (var v in ld.Domain(domain))
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
            
        }
    }
}
