using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB;

namespace EFFC.Frame.Net.Resource.SQLServer
{
    public sealed class SQLServerAccess2000:ADBAccess,IResourceEntity,IDisposable
    {
        private SqlTransaction trans = null;
        private SqlCommand sqlcomm= null;
        private SqlConnection sqlconn = null;
        private List<string> _sqlhistory = new List<string>();
        /// <summary>
        /// 需要在open的时候开启trans
        /// </summary>
        private bool isneedopentrans_in_open = false;

        public SQLServerAccess2000() { }

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
                if (!command.Parameters.Contains("@" + parameters[parameterName].ParameterName))
                {
                    idparameter = command.CreateParameter();
                    idparameter.ParameterName = string.Concat("@", parameters[parameterName].ParameterName);
                    
                    command.Parameters.Add(idparameter);
                }
                else
                {
                    idparameter = command.Parameters["@" + parameterName];
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

        #region IResouceEntity 成員
        private string _id ="";

        public string ID
        {
            get
            {
                if (_id == "")
                {
                    _id = "DBAccess" + Guid.NewGuid().ToString();
                }

                return _id;
            }
        }
        public override string ParameterFlagChar => "@";

        SqlServerExpress _express = new SqlServerExpress();
        public override DBExpress MyDBExpress => _express;
        /// <summary>
        /// 釋放連接資源
        /// </summary>
        public void Release()
        {
            if (this._s == DBStatus.Begin_Trans)
            {
                this.RollbackTransaction();
            }
            this.Close();
        }

        #endregion

        public override DataSetStd Query(string sql, DBOParameterCollection dbp)
        {
            if (sqlcomm == null)
            {
                sqlcomm = new SqlCommand(sql, this.sqlconn);
                sqlcomm.CommandTimeout = 90;
            }
            else
            {
                sqlcomm.CommandText = sql;
            }
            //如果事務開啟，則使用事務的方式
            if (this._s == DBStatus.Begin_Trans)
                sqlcomm.Transaction = this.trans;


            DataSetStd ds = new DataSetStd();
            SqlDataReader ddr = null;
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
                if (ddr != null)
                {
                    ddr.Close();
                    ddr.Dispose();
                }
                    
                sqlcomm.Cancel();
                sqlcomm = null;
            }
            return ds;
        }

        public override void Open(string connString)
        {
            if (sqlconn == null || this.sqlconn.State == ConnectionState.Closed)
            {
                this.sqlconn = new SqlConnection(connString);
            }
            if (this.sqlconn.State != ConnectionState.Open)
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

        /// <summary>
        /// 開啓事務處理
        /// </summary>
        public override void BeginTransaction(IsolationLevel level)
        {
            if (this.sqlconn != null)
            {
                this.trans = this.sqlconn.BeginTransaction(level);
                this._s = DBStatus.Begin_Trans;
            }
            else
            {
                isneedopentrans_in_open = true;
            }
        }
        /// <summary>
        /// 回滾事務
        /// </summary>
        public override void RollbackTransaction()
        {
            if (this._s == DBStatus.Begin_Trans)
            {
                this.trans.Rollback();
                this._s = DBStatus.RollBack_Trans;
            }
        }



        public override void ExecuteNoQuery(string sql, DBOParameterCollection dbp)
        {
            if (sqlcomm == null)
            {
                sqlcomm = new SqlCommand(sql, this.sqlconn);
                sqlcomm.CommandTimeout = 90;
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
            finally
            {
                sqlcomm.Cancel();
                sqlcomm = null;
            }
        }
        /// <summary>
        /// （未实现）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="selectsql"></param>
        public override void Update(object data, string selectsql)
        {
            //
        }

        public override void Insert(object data, string toTable)
        {
            if (data == null) return;
            if (!(data is DataTableStd))
            {
                return;
            }

            if (this.sqlconn.State == ConnectionState.Closed)
            {
                this.sqlconn.Open();
            }

            SqlBulkCopy sbc;
            if (this._s == DBStatus.Begin_Trans)
                sbc = new SqlBulkCopy(this.sqlconn, SqlBulkCopyOptions.Default, this.trans);
            else
                sbc = new SqlBulkCopy(this.sqlconn);

            try
            {

                sbc.DestinationTableName = toTable;
                sbc.WriteToServer((DataTableStd)data);
            }
            finally
            {
                sbc.Close();
            }
        }

        public override DBDataCollection ExcuteProcedure(string sp_name, bool isReturnDataSet, ref DBOParameterCollection dbp)
        {
            DBDataCollection rtn = new DBDataCollection();
            rtn.IsSuccess = false;

            DataSetStd ds = new DataSetStd();
            SqlCommand dc = null;//new SqlCommand(p.StoreProcureName, this.sqlconn);
            if (this._s == DBStatus.Begin_Trans)
            {
                dc = new SqlCommand(sp_name, this.sqlconn, this.trans);
            }
            else
            {
                dc = new SqlCommand(sp_name, this.sqlconn);
            }
            dc.CommandTimeout = 90;
            dc.CommandType = CommandType.StoredProcedure;
            FillParametersToCommand(dc, dbp);
            SqlDataReader ddr = null;
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
                foreach (SqlParameter sp in dc.Parameters)
                {
                    if (sp.Direction == ParameterDirection.Output || sp.Direction == ParameterDirection.InputOutput || sp.Direction == ParameterDirection.ReturnValue)
                        rtn.SetValue(sp.ParameterName.Replace("@", ""), sp.Value);
                }

                rtn.IsSuccess = true;

            }
            finally
            {
                if (ddr != null)
                {
                    ddr.Close();
                    ddr.Dispose();
                }
                dc.Dispose();
                dc = null;
            }

            return rtn;
        }
        /// <summary>
        /// 将Type转化成SqlDbType
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public SqlDbType ConvertBy(System.Type t)
        {
            SqlParameter pl = new SqlParameter();
            System.ComponentModel.TypeConverter tc = System.ComponentModel.TypeDescriptor.GetConverter(pl.DbType);
            if (tc.CanConvertFrom(t))
            {
                pl.DbType = (DbType)tc.ConvertFrom(t.Name);
            }
            else
            {
                try
                {
                    pl.DbType = (DbType)tc.ConvertFrom(t.Name);
                }
                catch
                {
                    //do nothing
                }
            }

            return pl.SqlDbType;
        }

        /// <summary>
        /// 根据数据集批量删除对应table中的数据（未实现）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="toTable"></param>
        public override void Delete(object data, string toTable)
        {
            //
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

        protected override DataTableStd QueryByPage(int startRow, int endRow, int toPage, int count_of_page, string sql, string orderby, DBOParameterCollection p)
        {
            string orderby4page = orderby;
            if (orderby == null || orderby == "")
            {
                orderby4page = GetColumnsNameBySql(sql, p)[0] + " ASC ";
            }

            string newsql = @"select *,NEWID() as _page_row_guid into #tmptt from(
                    " + sql + @")a

                    SELECT TOP " + count_of_page + @" *
                    FROM #tmptt
                    WHERE _page_row_guid NOT IN
                              (
                              SELECT TOP " + (count_of_page * (toPage - 1)) + @"  _page_row_guid FROM #tmptt ORDER BY " + orderby4page + @"
                              )
                    ORDER BY " + orderby4page + @"

                    drop table #tmptt";
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
        /// <summary>
        /// GC回收機制
        /// </summary>
        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

    }
}
