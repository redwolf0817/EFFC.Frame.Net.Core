using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using EFFC.Frame.Net.Module.Tag.Datas;
using EFFC.Frame.Net.Module.Tag.Parameters;
using EFFC.Frame.Net.Resource.Sqlite;
using EFFC.Frame.Net.Resource.SQLServer;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using static EFFC.Frame.Net.Module.Extend.EWRA.Logic.RestLogic;

namespace RestAPISample.Business
{
    public partial class MyRestLogic:ValidRestLogic
    {
        /// <summary>
        /// 设定view路径
        /// </summary>
        /// <param name="path">view路径，~表示根路径</param>
        protected void SetViewPath(string path)
        {
            CallContext_DataCollection.ExtentionObj.viewpath = path;
        }
        protected override void DoProcess(EWRAParameter p, EWRAData d)
        {
            base.DoProcess(p, d);
            //if (d.ContentType == EFFC.Frame.Net.Module.Extend.EWRA.Constants.RestContentType.HTML)
            //{
            //    SetCacheEnable(false);
            //    var tagp = new TagParameter();
            //    object tagd = new TagData();
            //    tagp.RootPath = MyConfig.GetConfiguration("View", "Template_Path");
            //    var viewpath = ComFunc.nvl(d.ExtentionObj.viewpath).Replace("~", tagp.RootPath);

            //    if (File.Exists(viewpath))
            //    {
            //        FrameDLRObject bindobject = FrameDLRObject.CreateInstance(d.Result, FrameDLRFlags.SensitiveCase);
            //        foreach (var item in bindobject.Items)
            //        {
            //            ((TagData)tagd).Context.AddBindObject(item.Key, item.Value);
            //        }
            //        tagp.Text = File.ReadAllText(viewpath);
            //        GlobalCommon.Proxys["tag"].CallModule(ref tagd, tagp);
            //        d.Result = ((TagData)tagd).ParsedText;
            //        d.StatusCode = EFFC.Frame.Net.Module.Extend.EWRA.Constants.RestStatusCode.OK;
            //    }
            //    else
            //    {
            //        d.StatusCode = EFFC.Frame.Net.Module.Extend.EWRA.Constants.RestStatusCode.NOT_FOUND;
            //    }

            //}
        }

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

        public class MyDBHelper : RestDBHeler
        {
            MyRestLogic _logic = null;
            private DBType DEFAULT_DB_TYPE = DBType.None;
            public MyDBHelper(MyRestLogic logic)
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
            public new LinqDLRTable LamdaTable(string tablename, string alianname = "", DBType dbtype = DBType.SqlServer)
            {
                if (dbtype == DBType.None && DEFAULT_DB_TYPE != DBType.None)
                {
                    dbtype = DEFAULT_DB_TYPE;
                }
                return LinqDLRTable.New<SqlServerDLRColumn>(tablename, alianname, new SqlServerOperatorFlags());
            }

            /// <summary>
            /// 判断一个table是否已经存在
            /// </summary>
            /// <param name="up"></param>
            /// <param name="tablename"></param>
            /// <returns></returns>
            public bool IsTableExists(UnitParameter up, string tablename)
            {
                try
                {
                    var i = (from t in LamdaTable(up, tablename, "a") where new LinqDLR2SqlWhereOperator("1=0", null) select t).Take(1).Count(up);
                    return true;
                }
                catch (SqlException ex)
                {
                    return false;
                }
            }

            /// <summary>
            /// 创建一个默认的DB访问参数
            /// </summary>
            /// <returns></returns>
            public override UnitParameter NewDBUnitParameter()
            {
                var dbtype = ComFunc.nvl(_logic.Configs["DB_Type"]);
                UnitParameter rtn = null;
                if (dbtype.ToLower() == "sqlserver")
                {
                    rtn = base.NewDBUnitParameter<SQLServerAccess>();
                }
                else if (dbtype.ToLower() == "mysql")
                {
                    rtn = base.NewDBUnitParameter<MySQLAccess>();
                }
                else if (dbtype.ToLower() == "oracle")
                {
                    rtn = base.NewDBUnitParameter<OracleAccess>();
                }
                else if (dbtype.ToLower() == "sqlite")
                {
                    rtn = base.NewDBUnitParameter<SqliteAccess>();
                }
                InitUPPage(rtn);
                rtn.Dao.Open(_logic.CallContext_Parameter.DBConnectionString);
                return rtn;
            }
            /// <summary>
            /// 创建一个EBS的DB访问参数
            /// </summary>
            /// <returns></returns>
            public UnitParameter NewDBUnitParameter4EBS()
            {
                var rtn = base.NewDBUnitParameter<OracleAccess>();
                rtn.Dao.Open(ComFunc.nvl(_logic.Configs["EBSConnection"]));
                return rtn;
            }
        }
    }
}
