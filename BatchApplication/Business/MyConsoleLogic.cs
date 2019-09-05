using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Module.Extend.EConsole.Logic;
using EFFC.Frame.Net.Resource.Postgresql;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace BatchApplication.Business
{
    public partial class MyConsoleLogic:ConsoleLogic
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
            MyConsoleLogic _logic = null;
            private DBType DEFAULT_DB_TYPE = DBType.None;
            public MyDBHelper(MyConsoleLogic logic)
                : base(logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// 获取一个Lamdatable对象，可以进行lamda2sql的操作
            /// </summary>
            /// <param name="tablename"></param>
            /// <param name="alianname"></param>
            /// <param name="dbtype">默认mysql</param>
            /// <returns></returns>
            public new LinqDLRTable LamdaTable(string tablename, string alianname = "", DBType dbtype = DBType.MySql)
            {
                if (dbtype == DBType.None && DEFAULT_DB_TYPE != DBType.None)
                {
                    dbtype = DEFAULT_DB_TYPE;
                }
                return LinqDLRTable.New<MysqlDLRColumn>(tablename, alianname, new MySQLOperatorFlags());
            }
            public new LinqDLRTable PSQLLamdaTable(string tablename, string alianname = "", DBType dbtype = DBType.PostgreSQL)
            {
                if (dbtype == DBType.None && DEFAULT_DB_TYPE != DBType.None)
                {
                    dbtype = DEFAULT_DB_TYPE;
                }
                return LinqDLRTable.New<MysqlDLRColumn>(tablename, alianname,new PostgreSqlOperatorFlags());
            }
            /// <summary>
            /// 创建一个默认的DB访问参数
            /// </summary>
            /// <returns></returns>
            public override UnitParameter NewDBUnitParameter()
            {
                var rtn = base.NewDBUnitParameter<MySQLAccess>();
                rtn.Dao.Open(_logic.CallContext_Parameter.DBConnectionString);
                return rtn;
            }
            public UnitParameter NewPSQLDBUnitParameter()
            {
                var rtn = base.NewDBUnitParameter<PostgreSqlAccess>();
                rtn.Dao.Open(ComFunc.nvl(_logic.Configs["SourceBusiConnection"]));
                return rtn;
            }
        }
    }
}
