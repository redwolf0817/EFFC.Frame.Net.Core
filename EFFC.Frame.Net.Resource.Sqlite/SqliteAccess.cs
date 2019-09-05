using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Base.Interfaces.Core;

using EFFC.Frame.Net.Base.Constants;
using System.Data.Common;
using EFFC.Frame.Net.Base.Common;
using System.Data;
using EFFC.Frame.Net.Base.Data;
using Microsoft.Data.Sqlite;
using EFFC.Extends.LinqDLR2SQL;

namespace EFFC.Frame.Net.Resource.Sqlite
{
    public class SqliteAccess : ADBAccess, IResourceEntity, IDisposable
    {
        SqliteTransaction trans = null;
        SqliteConnection sqlconn = null;
        SqliteCommand sqlcomm = null;
        /// <summary>
        /// 需要在open的时候开启trans
        /// </summary>
        private bool isneedopentrans_in_open = false;
        #region DBAccess
        public override string ParameterFlagChar => "$";
        SqliteExpress _express = new SqliteExpress();
        public override DBExpress MyDBExpress => _express;
        public override DBType MyType => DBType.Sqlite;


        public override void BeginTransaction(System.Data.IsolationLevel level)
        {
            if (this.sqlconn != null)
            {
                //sqlite的隔离级别ReadCommit无法使用
                //this.trans = this.sqlconn.BeginTransaction(level);
                if (this._s != DBStatus.Begin_Trans)
                {
                    this.trans = this.sqlconn.BeginTransaction();
                    this._s = DBStatus.Begin_Trans;
                }
            }
            else
            {
                isneedopentrans_in_open = true;
            }
        }

        public override void Close()
        {
            if (this.sqlconn != null && (this.sqlconn.State == ConnectionState.Open || this.sqlconn.State == ConnectionState.Connecting || this.sqlconn.State == ConnectionState.Executing))
            {
                this.sqlconn.Close();
            }
            this.sqlconn = null;
            this._s = DBStatus.Close;
        }

        public override void CommitTransaction()
        {
            if (this._s == DBStatus.Begin_Trans)
            {
                this.trans.Commit();
                this._s = DBStatus.Commit_Trans;
            }
        }

        public override void Delete(object data, string toTable)
        {
            //
        }

        public override DBDataCollection ExcuteProcedure(string sp_name, bool isReturnDataSet, ref DBOParameterCollection dbp)
        {
            DBDataCollection rtn = new DBDataCollection();
            rtn.IsSuccess = false;

            DataSetStd ds = new DataSetStd();
            SqliteCommand dc = null;//new SqlCommand(p.StoreProcureName, this.sqlconn);
            if (this._s == DBStatus.Begin_Trans)
            {
                dc = new SqliteCommand(sp_name, this.sqlconn, this.trans);
            }
            else
            {
                dc = new SqliteCommand(sp_name, this.sqlconn);
            }
            //dc.CommandTimeout = 90;
            dc.CommandType = CommandType.StoredProcedure;
            FillParametersToCommand(dc, dbp);
            SqliteDataReader ddr = null;
            try
            {

                if (isReturnDataSet)
                {
                    ddr = dc.ExecuteReader();
                    ds = DataSetStd.FillData(ddr);
                    rtn.ReturnDataSet = ds;
                }
                else
                {
                    dc.ExecuteNonQuery();
                }
                //獲取返回值
                foreach (SqliteParameter sp in dc.Parameters)
                {
                    if (sp.Direction == ParameterDirection.Output || sp.Direction == ParameterDirection.InputOutput || sp.Direction == ParameterDirection.ReturnValue)
                        rtn.SetValue(sp.ParameterName.Replace(ParameterFlagChar, ""), sp.Value);
                }

                rtn.IsSuccess = true;

            }
            finally
            {
                if(ddr != null)
                {
                    ddr.Close();
                    ddr.Dispose();
                }
                dc.Dispose();
                dc = null;
            }

            return rtn;
        }

        public override void ExecuteNoQuery(string sql, DBOParameterCollection dbp)
        {
            if (sqlcomm == null)
            {
                sqlcomm = new SqliteCommand(sql, this.sqlconn);
                //sqlcomm.CommandTimeout = 90;
            }
            else
            {
                sqlcomm.CommandText = sql;
            }

            if (dbp != null)
            {
                FillParametersToCommand(sqlcomm, dbp);
            }

            if (_s == DBStatus.Begin_Trans)
                sqlcomm.Transaction = this.trans;

            try
            {
                sqlcomm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlcomm.Dispose();
                sqlcomm = null;
            }
        }

        public override void Insert(object data, string toTable)
        {
            //
        }

        public override void Open(string connString)
        {
            if (sqlconn == null)
            {
                this.sqlconn = new SqliteConnection(connString);
            }
            if (this.sqlconn.State == ConnectionState.Closed)
            {
                this.sqlconn.Open();
            }
            if (isneedopentrans_in_open)
            {
                this.trans = sqlconn.BeginTransaction();
                isneedopentrans_in_open = false;
                this._s = DBStatus.Begin_Trans;
            }
            else
            {
                this._s = DBStatus.Open;
            }
        }

        public override Net.Base.Data.DataSetStd Query(string sql, DBOParameterCollection dbp)
        {
            if (sqlcomm == null)
            {
                sqlcomm = new SqliteCommand(sql, this.sqlconn);
                //sqlcomm.CommandTimeout = 90;
            }
            else
            {
                sqlcomm.CommandText = sql;
            }
            //如果事務開啟，則使用事務的方式
            if (this._s == DBStatus.Begin_Trans)
                sqlcomm.Transaction = this.trans;


            DataSetStd ds = new DataSetStd();
            SqliteDataReader ddr = null;
            try
            {
                //如果有參數
                if (dbp != null)
                {
                    FillParametersToCommand(sqlcomm, dbp);
                }
                ddr = sqlcomm.ExecuteReader();
                
                ds = DataSetStd.FillData(ddr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(ddr != null)
                {
                    ddr.Close();
                    ddr.Dispose();
                }
                sqlcomm.Dispose();
                sqlcomm = null;
            }
            return ds;
        }

       

        public override void RollbackTransaction()
        {
            if (this._s == DBStatus.Begin_Trans)
            {
                this.trans.Rollback();
                this._s = DBStatus.RollBack_Trans;
            }
        }

        public override void Update(object data, string selectsql)
        {
            //
        }

        protected override Net.Base.Data.DataTableStd QueryByPage(int startRow, int endRow, int toPage, int count_of_page, string sql, string orderby, DBOParameterCollection p)
        {
            string orderby4page = orderby;
            if (orderby == null || orderby == "")
            {
                orderby4page = GetColumnsNameBySql(sql, p)[0] + " ASC ";
            }

            string newsql = @"
select * from (" + sql + @") order by "+ orderby4page + @" limit " + count_of_page + " offset " + (count_of_page * (toPage - 1)) ;

            DataSetStd dss = Query(newsql, p);
            if (dss == null)
            {
                return null;
            }
            else
            {
                return dss[0];
            }
        }
        #endregion

        #region ResourceEntity
        private string _id = "";
        public string ID
        {
            get
            {
                if (_id == "")
                {
                    _id = "Sqlite_" + Guid.NewGuid().ToString();
                }

                return _id;
            }
        }

        public void Release()
        {
            if (this._s == DBStatus.Begin_Trans)
            {
                this.RollbackTransaction();
            }
            this.Close();
        }


        #endregion

        #region Dispose
        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// 依據Dictionary填充Command物件參數集
        /// </summary>
        /// <param name="command">命令物件</param>
        /// <param name="parameters">參數</param>
        private void FillParametersToCommand(DbCommand command, DBOParameterCollection parameters)
        {
            foreach (string parameterName in parameters.Keys)
            {
                DbParameter idparameter = null;
                if (!command.Parameters.Contains(ParameterFlagChar + parameters[parameterName].ParameterName))
                {
                    idparameter = command.CreateParameter();
                    idparameter.ParameterName = string.Concat(ParameterFlagChar, parameters[parameterName].ParameterName);

                    command.Parameters.Add(idparameter);
                }
                else
                {
                    idparameter = command.Parameters[ParameterFlagChar + parameterName];
                }

                if (parameters[parameterName].DataLength > 0)
                {
                    idparameter.Direction = parameters[parameterName].Direction;
                    idparameter.DbType = parameters[parameterName].DataType;
                    idparameter.Size = parameters[parameterName].DataLength;
                }


                if (parameters[parameterName].ParameterValue == null)
                {
                    idparameter.Value = DBNull.Value;
                }
                else if (ComFunc.nvl(parameters[parameterName].ParameterValue) == "")
                {
                    if (parameters[parameterName].DataType != DbType.String)
                    {
                        idparameter.Value = DBNull.Value;
                    }
                    else
                    {
                        idparameter.Value = parameters[parameterName].ParameterValue;
                    }
                }
                else
                {
                    idparameter.Value = parameters[parameterName].ParameterValue;
                }


            }
        }
        private string[] columnsName4PageOrder = null;
        private string sql4PageOrder = "";
        /// <summary>
        /// 通過sql獲得相關的欄位列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private string[] GetColumnsNameBySql(string sql, DBOParameterCollection p)
        {
            if (sql != sql4PageOrder || columnsName4PageOrder == null)
            {
                sql4PageOrder = sql;

                DataSetStd dss = Query(sql, p);
                if (dss != null)
                {
                    columnsName4PageOrder = dss[0].ColumnNames;
                }
            }

            return columnsName4PageOrder;
        }

        /// <summary>
        /// 生成一个LinqDLR2SQL对象用于Linq操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="alianname"></param>
        /// <returns></returns>
        public override LinqDLRTable NewLinqTable(string table, string alianname = "")
        {
            var tn = alianname == "" ? table : alianname;
            LinqDLRTable rtn = LinqDLRTable.New<LinqDLRTable>(new SqliteLamdaSQLObject(tn, new SqliteOperatorFlags()), table, alianname, new SqliteSqlGenerator());
            return rtn;
        }

    }
}
