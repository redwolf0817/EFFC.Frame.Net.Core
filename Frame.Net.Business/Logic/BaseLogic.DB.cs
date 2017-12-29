using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Interfaces.Unit;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Business.Unit;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Business.Logic
{
    public partial class BaseLogic<P, D>
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
            BaseLogic<P, D> _logic;

            public DBHelper() { }
            public DBHelper(BaseLogic<P, D> logic)
            {
                _logic = logic;
            }

            /// <summary>
            /// 創建一個普通的資源對象
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public T NewResourceEntity<T>() where T : IResourceEntity
            {
                return _logic.CallContext_ResourceManage.CreateInstance<T>(this.GetHashCode().ToString());
            }
            /// <summary>
            /// 創建一個事務資源對象
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public T NewTransResourceEntity<T>() where T : IResourceEntity, ITransaction
            {
                return NewTransResourceEntity<T>(_logic.CallContext_CurrentToken);
            }
            /// <summary>
            /// 創建一個事務資源對象
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="token">指定的事务</param>
            /// <returns></returns>
            public T NewTransResourceEntity<T>(TransactionToken token) where T : IResourceEntity, ITransaction
            {
                return _logic.CallContext_ResourceManage.CreateInstance<T>(token);
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
            public UnitParameter NewDBUnitParameter<DB>() where DB : IDBAccessInfo,ITransaction, IResourceEntity
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
            /// 将Logic的数据写入到UnitParameter
            /// </summary>
            /// <param name="ld"></param>
            /// <param name="up"></param>
            public void SetUnitParameter(LogicData ld, UnitParameter up)
            {
                foreach (var val in ld)
                {
                    up.SetValue(val.Key, val.Value);
                }
            }
            /// <summary>
            /// 将Logic的数据写入到UnitParameter
            /// </summary>
            /// <param name="ld"></param>
            /// <param name="up"></param>
            public void SetUnitParameter(LogicData ld, UnitParameter up, string domain)
            {
                foreach (var val in ld.Domain(domain))
                {
                    up.SetValue(val.Key, val.Value);
                }
            }
            /// <summary>
            /// 标准化DB执行操作
            /// </summary>
            /// <param name="p"></param>
            /// <param name="express"></param>
            /// <returns></returns>
            public virtual UnitDataCollection Excute(UnitParameter p, DBExpress express)
            {
                UnitDataCollection rtn = new UnitDataCollection();
                if (express != null)
                {
                    p.SetValue("__json__", express);
                    if (p.Dao is ADBAccess)
                    {
                        if (express.CurrentAct == DBExpress.ActType.Query)
                        {
                            rtn = Query<JsonExpressUnit>(p, "");
                        }
                        else if (express.CurrentAct == DBExpress.ActType.QueryByPage)
                        {
                            rtn = QueryByPage<JsonExpressUnit>(p, "");
                        }
                        else
                        {
                            NonQuery<JsonExpressUnit>(p, "");
                        }
                    }
                    else if (p.Dao is MongoAccess26)
                    {
                        var result = ((MongoAccess26)p.Dao).Excute(express);
                        if(express.CurrentAct == DBExpress.ActType.Query)
                        {
                            rtn.MongoListData = (List<FrameDLRObject>)result;
                        }
                    }
                }
                return rtn;
            }
            /// <summary>
            /// 通过json对象执行标准化DB操作
            /// </summary>
            /// <param name="p"></param>
            /// <param name="json"></param>
            /// <returns></returns>
            public virtual UnitDataCollection Excute(UnitParameter p, FrameDLRObject json)
            {
                DBExpress express = null;
                if (p.Dao is OracleAccess)
                {
                    express = DBExpress.Create<OracleExpress>(json);
                }
                else if (p.Dao is SQLServerAccess
                    || p.Dao is SQLServerAccess2000)
                {
                    express = DBExpress.Create<SqlServerExpress>(json);
                }
                else if (p.Dao is MySQLAccess)
                {
                    express = DBExpress.Create<MySQLExpress>(json);
                }
                else if (p.Dao is MongoAccess26)
                {
                    express = DBExpress.Create<MongoExpress>(json);
                }
                return Excute(p, express);
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
                p.SetValue("_unit_action_flag_", actionflag);
                return (UnitDataCollection)UnitProxy.Call<QueryUnit<T>>(p);
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
                p.SetValue("_unit_action_flag_", actionflag);
                return (UnitDataCollection)UnitProxy.Call<QueryByPageUnit<T>>(p);
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
                    var pkey = s.Replace("(", "").Replace(")", "").Replace("'", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(".", "");
                    dpc.SetValue(pkey, obj.GetValue(s));
                    columns += columns.Length > 0 ? "," + s : s;
                    values += values.Length > 0 ? ",@" + pkey : "@" + pkey;
                }
                sbsql.Append(columns + ")values(");
                sbsql.Append(values);
                sbsql.Append(");");

                if (p.Dao == null)
                {
                    p.Dao = _logic.CallContext_ResourceManage.CreateInstance<SQLServerAccess>(p.CurrentTransToken);
                    p.Dao.Open(ComFunc.nvl(_logic.Configs[ParameterKey.DBCONNECT_STRING]));
                }
                //目前只支持关系型数据库
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
                    var pkey = s.Replace("(", "").Replace(")", "").Replace("'", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(".", "");
                    dpc.SetValue(pkey, obj.GetValue(s));
                    columns += columns.Length > 0 ? "," + s : s;
                    values += values.Length > 0 ? ",:" + pkey : ":" + pkey;
                }
                sbsql.Append(columns + ")values(");
                sbsql.Append(values);
                sbsql.Append(");");

                if (p.Dao == null)
                {
                    p.Dao = _logic.CallContext_ResourceManage.CreateInstance<OracleAccess>(p.CurrentTransToken);
                    p.Dao.Open(ComFunc.nvl(_logic.Configs[ParameterKey.DBCONNECT_STRING]));
                }
                //目前只支持关系型数据库
                if (p.Dao is ADBAccess)
                {
                    ((ADBAccess)p.Dao).ExecuteNoQuery(sbsql.ToString(), dpc);
                }
            }            
            public virtual void DeleteFromSQLServer(UnitParameter p, string tablename, FrameDLRObject obj)
            {
                StringBuilder sbsql = new StringBuilder();
                sbsql.Append("delete from " + tablename + " where 1=1 ");
                var values = "";
                var columns = "";
                DBOParameterCollection dpc = new DBOParameterCollection();
                foreach (var s in obj.Keys)
                {
                    var pkey = s.Replace("(", "").Replace(")", "").Replace("'", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(".", "");
                    dpc.SetValue(pkey, obj.GetValue(s));
                    sbsql.Append(string.Format(" and {0}=@{0}", pkey));
                }
                if (p.Dao == null)
                {
                    p.Dao = _logic.CallContext_ResourceManage.CreateInstance<SQLServerAccess>(p.CurrentTransToken);
                    p.Dao.Open(ComFunc.nvl(_logic.Configs[ParameterKey.DBCONNECT_STRING]));
                }
                //目前只支持关系型数据库
                if (p.Dao is ADBAccess)
                {
                    ((ADBAccess)p.Dao).ExecuteNoQuery(sbsql.ToString(), dpc);
                }
            }
            public virtual void DeleteFromOracle(UnitParameter p, string tablename, FrameDLRObject obj)
            {
                StringBuilder sbsql = new StringBuilder();
                sbsql.Append("delete from " + tablename + " where 1=1 ");
                var values = "";
                var columns = "";
                DBOParameterCollection dpc = new DBOParameterCollection();
                foreach (var s in obj.Keys)
                {
                    var pkey = s.Replace("(", "").Replace(")", "").Replace("'", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(".", "");
                    dpc.SetValue(pkey, obj.GetValue(s));
                    sbsql.Append(string.Format(" and {0}=:{0}", pkey));
                }
                if (p.Dao == null)
                {
                    p.Dao = _logic.CallContext_ResourceManage.CreateInstance<OracleAccess>(p.CurrentTransToken);
                    p.Dao.Open(ComFunc.nvl(_logic.Configs[ParameterKey.DBCONNECT_STRING]));
                }
                //目前只支持关系型数据库
                if (p.Dao is ADBAccess)
                {
                    ((ADBAccess)p.Dao).ExecuteNoQuery(sbsql.ToString(), dpc);
                }
            }
            /// <summary>
            /// 根据obj的定义更新单表，obj定义格式如下
            /// {
            /// col1:value,
            /// coln:value,
            /// where:{
            /// c1:{$op:value}
            /// cn:{$op:value}
            /// }
            /// }
            /// </summary>
            /// <param name="p"></param>
            /// <param name="table"></param>
            /// <param name="obj"></param>
            public virtual void UpdateOracle(UnitParameter p, string tablename, FrameDLRObject obj)
            {
                StringBuilder sbsql = new StringBuilder();
                sbsql.Append("update " + tablename + " set ");
                StringBuilder columns = new StringBuilder();
                string where = "";
                DBOParameterCollection dpc = new DBOParameterCollection();
                foreach (var s in obj.Keys)
                {
                    if (s.ToLower() != "where")
                    {
                        var pkey = s.Replace("(", "").Replace(")", "").Replace("'", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(".", "");
                        dpc.SetValue(pkey, obj.GetValue(s));
                        columns.Append(columns.Length > 0 ? "," : "");
                        columns.AppendLine(string.Format("{0}=:{0}", s));
                    }
                    else
                    {
                        where = ParseWhereExpress(DBType.Oracle, obj.GetValue(s), ref dpc);
                    }
                }
                sbsql.Append(columns);
                sbsql.Append(where + ";");
                if (p.Dao == null)
                {
                    p.Dao = _logic.CallContext_ResourceManage.CreateInstance<OracleAccess>(p.CurrentTransToken);
                    p.Dao.Open(ComFunc.nvl(_logic.Configs[ParameterKey.DBCONNECT_STRING]));
                }
                //目前只支持关系型数据库
                if (p.Dao is ADBAccess)
                {
                    ((ADBAccess)p.Dao).ExecuteNoQuery(sbsql.ToString(), dpc);
                }
            }
            /// <summary>
            /// 根据obj的定义更新单表，obj定义格式如下
            /// {
            /// col1:value,
            /// coln:value,
            /// where:{
            /// c1:{$op:value}
            /// cn:{$op:value}
            /// }
            /// }
            /// </summary>
            /// <param name="p"></param>
            /// <param name="table"></param>
            /// <param name="obj"></param>
            public virtual void UpdateSQLServer(UnitParameter p, string tablename, FrameDLRObject obj)
            {
                StringBuilder sbsql = new StringBuilder();
                sbsql.Append("update " + tablename + " set ");
                StringBuilder columns = new StringBuilder();
                string where = "";
                DBOParameterCollection dpc = new DBOParameterCollection();
                foreach (var s in obj.Keys)
                {
                    if (s.ToLower() != "where")
                    {
                        var pkey = s.Replace("(", "").Replace(")", "").Replace("'", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(".", "");
                        dpc.SetValue(pkey, obj.GetValue(s));
                        columns.Append(columns.Length > 0 ? "," : "");
                        columns.AppendLine(string.Format("{0}=@{0}", s));
                    }
                    else
                    {
                        where = ParseWhereExpress(DBType.SqlServer, obj.GetValue(s), ref dpc);
                    }
                }
                sbsql.Append(columns);
                sbsql.Append(where);
                if (p.Dao == null)
                {
                    p.Dao = _logic.CallContext_ResourceManage.CreateInstance<SQLServerAccess>(p.CurrentTransToken);
                    p.Dao.Open(ComFunc.nvl(_logic.Configs[ParameterKey.DBCONNECT_STRING]));
                }
                //目前只支持关系型数据库
                if (p.Dao is ADBAccess)
                {
                    ((ADBAccess)p.Dao).ExecuteNoQuery(sbsql.ToString(), dpc);
                }
            }
            protected enum DBType
            {
                Oracle,
                SqlServer,
                DB2,
                MySql
            }
            /// <summary>
            /// 根据json的条件定义构成where语句
            /// </summary>
            /// <param name="dbtype"></param>
            /// <param name="where"></param>
            /// <param name="dbc"></param>
            /// <returns></returns>
            protected virtual string ParseWhereExpress(DBType dbtype, object where,ref DBOParameterCollection dbc)
            {
                if (where == null || !(where is FrameDLRObject))
                {
                    return "";
                }
                var fwhere = (FrameDLRObject)where;
                var varflag= "@";
                var strlinkflag = "+";
                if(dbtype == DBType.Oracle){
                    varflag = ":";
                    strlinkflag = "||";
                }else if(dbtype == DBType.SqlServer){
                     varflag = "@";
                     strlinkflag = "+";
                }
                var wherestr = new StringBuilder();
                foreach (var key in fwhere.Keys)
                {
                    var item = fwhere.GetValue(key);
                    var pkey = key.Replace("(", "").Replace(")", "").Replace("'", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(".", "");
                    if (item is FrameDLRObject)
                    {
                        var ditem = (FrameDLRObject)item;
                        foreach (var op in ditem.Keys)
                        {
                            
                            if (op.ToLower() == "$eq")
                            {
                                wherestr.Append(wherestr.Length > 0 ? " and " : " ");
                                wherestr.AppendFormat("{0}={1}{2}", key, varflag, pkey);
                                dbc.SetValue(pkey, ditem.GetValue(op));
                                break;
                            }
                            if (op.ToLower() == "$lt")
                            {
                                wherestr.Append(wherestr.Length > 0 ? " and " : " ");
                                wherestr.AppendFormat("{0}<{1}{2}", key, varflag, pkey);
                                dbc.SetValue(pkey, ditem.GetValue(op));
                                break;
                            }
                            if (op.ToLower() == "$gt")
                            {
                                wherestr.Append(wherestr.Length > 0 ? " and " : " ");
                                wherestr.AppendFormat("{0}>{1}{2}", key, varflag, pkey);
                                dbc.SetValue(pkey, ditem.GetValue(op));
                                break;
                            }
                            if (op.ToLower() == "$lte")
                            {
                                wherestr.Append(wherestr.Length > 0 ? " and " : " ");
                                wherestr.AppendFormat("{0}<={1}{2}", key, varflag, pkey);
                                dbc.SetValue(pkey, ditem.GetValue(op));
                                break;
                            }
                            if (op.ToLower() == "$gte")
                            {
                                wherestr.Append(wherestr.Length > 0 ? " and " : " ");
                                wherestr.AppendFormat("{0}>={1}{2}", key, varflag, pkey);
                                dbc.SetValue(pkey, ditem.GetValue(op));
                                break;
                            }
                            if (op.ToLower() == "$in")
                            {
                                wherestr.Append(wherestr.Length > 0 ? " and " : " ");
                                var val = ditem.GetValue(op);
                                if (val is object[])
                                {
                                    var aval = (object[])val;
                                    var instr = "";
                                    var index = 0;
                                    foreach (var s in aval)
                                    {
                                        instr += (instr.Length > 0 ? "," : "") + varflag + pkey + index;
                                        dbc.SetValue(pkey + index, s);
                                        index++;
                                    }
                                    if (aval.Length > 0)
                                    {
                                        wherestr.AppendFormat("{0} in ({1})", key, instr);
                                    }
                                }
                                else
                                {
                                    wherestr.AppendFormat("{0} in ({1})", key, varflag + pkey);
                                    dbc.SetValue(pkey, ditem.GetValue(op));
                                }
                                break;
                            }
                            if (op.ToLower() == "$like")
                            {
                                wherestr.Append(wherestr.Length > 0 ? " and " : " ");
                                wherestr.AppendFormat("{0} like '%'{3}{1}{2}{3}'%'", key, varflag, pkey, strlinkflag);
                                dbc.SetValue(pkey, ditem.GetValue(op));
                                break;
                            }
                            if (op.ToLower() == "$likel")
                            {
                                wherestr.Append(wherestr.Length > 0 ? " and " : " ");
                                wherestr.AppendFormat("{0} like {1}{2}{3}'%'", key, varflag, pkey, strlinkflag);
                                dbc.SetValue(pkey, ditem.GetValue(op));
                                break;
                            }
                            if (op.ToLower() == "$liker")
                            {
                                wherestr.Append(wherestr.Length > 0 ? " and " : " ");
                                wherestr.AppendFormat("{0} like '%'{3}{1}{2}", key, varflag, pkey, strlinkflag);
                                dbc.SetValue(pkey, ditem.GetValue(op));
                                break;
                            }
                        }
                    }
                    else
                    {
                        wherestr.Append(wherestr.Length > 0 ? " and " : " ");
                        wherestr.AppendFormat("{0}={1}{2}", key, varflag, pkey);
                        dbc.SetValue(pkey, item);
                        break;
                    }
                }
                return (wherestr.Length >0?" where ":"")+ wherestr.ToString();
            }

            private class JsonExpressUnit : IDBUnit<UnitParameter>
            {

                public Func<UnitParameter, dynamic> GetSqlFunc(string flag)
                {
                    return Load;
                }

                private dynamic Load(UnitParameter arg)
                {
                    var rtn = FrameDLRObject.CreateInstance();
                    var json = arg.GetValue("__json__");
                    if (json != null && json is DBExpress)
                    {
                        var express = (DBExpress)json;
                        var re = express.ToExpress();
                        var sql = ComFunc.nvl(re.GetValue("sql"));
                        var data = (FrameDLRObject)re.GetValue("data");
                        var orderby = ComFunc.nvl(re.GetValue("orderby"));
                        foreach (var k in data.Keys)
                        {
                            arg.SetValue(k, data.GetValue(k));
                        }
                        if (express.CurrentAct == DBExpress.ActType.QueryByPage)
                        {
                            rtn.sql = sql;
                            rtn.orderby = orderby;
                        }
                        else
                        {
                            rtn.sql = sql;
                        }

                    }
                    return rtn;
                }
            }
        }
    }
}
