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
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Interfaces.System;

namespace EFFC.Frame.Net.Base.ResouceManage.DB
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
            try
            {
                //如果有參數
                if (dbp != null)
                {
                    FillParametersToCommand(sqlcomm, dbp);
                }
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(this.sqlcomm);
                sqlAdapter.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlcomm.Cancel();
                sqlcomm = null;
            }
            return ds;
        }

        public DataSetStd QueryWithKey(string sql, DBOParameterCollection dbp)
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
            try
            {
                //如果有參數
                if (dbp != null)
                {
                    FillParametersToCommand(sqlcomm, dbp);
                }
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(this.sqlcomm);
                sqlAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                sqlAdapter.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
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

        public override void Update(DataTable data, string selectsql)
        {
            if (sqlcomm == null)
            {
                this.sqlcomm = new SqlCommand(selectsql, this.sqlconn);
                this.sqlcomm.CommandTimeout = 90;
            }
            else
            {
                sqlcomm.CommandText = selectsql;
            }

            if (this._s == DBStatus.Begin_Trans)
            {
                sqlcomm.Transaction = this.trans;
            }

            SqlDataAdapter adt = new SqlDataAdapter(this.sqlcomm);
            SqlCommandBuilder builder = new SqlCommandBuilder(adt);

            try
            {
                adt.UpdateCommand = builder.GetUpdateCommand();
                adt.Update(data);
            }
            finally
            {
                sqlcomm.Cancel();
                sqlcomm = null;
            }
        }

        public override void Insert(DataTable data, string toTable)
        {
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
                sbc.WriteToServer(data);
            }
            finally
            {
                sbc.Close();
            }
        }

        public override DBDataCollection ExcuteProcedure(string sp_name, bool isReturnDataSet, ref DBOParameterCollection dbp)
        {
            DBDataCollection rtn =new DBDataCollection();
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
            try
            {

                if (isReturnDataSet)
                {
                    SqlDataAdapter sqlDa = new SqlDataAdapter();
                    sqlDa.SelectCommand = dc;
                    sqlDa.Fill(ds);
                    rtn.ReturnDataSet= ds;
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
                dc.Cancel();
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
        /// 根据数据集批量删除对应table中的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="toTable"></param>
        public override void Delete(DataTable data, string toTable)
        {
            if (data == null) return;
            string[] keycols = DataTableStd.GetPKName(data);
            string sql = "delete from " + toTable;
            string where = "";
            if (keycols.Length <= 0)
            {
                keycols = DataTableStd.GetColumnName(data);
            }

            foreach (string key in keycols)
            {
                if (where == "")
                {
                    where += " where " + key + "=@" + key;
                }
                else
                {
                    where += " and " + key + "=@" + key;
                }
            }

            sql = sql + where;

            SqlCommand tsqlcomm = new SqlCommand("select * from " + toTable + " where 1=0 ", this.sqlconn);
            tsqlcomm.CommandTimeout = 90;


            if (this._s == DBStatus.Begin_Trans)
                tsqlcomm.Transaction = this.trans;

            SqlDataAdapter sda = new SqlDataAdapter(tsqlcomm);
            DataSetStd ds = new DataSetStd();
            sda.FillSchema(ds, SchemaType.Source);
            tsqlcomm.CommandText = "SET NOCOUNT ON " + sql;
            sda.DeleteCommand = tsqlcomm;
            sda.DeleteCommand.Parameters.Clear();
            foreach (string key in keycols)
            {
                DataColumn dc = data.Columns[key];
                sda.DeleteCommand.Parameters.Add("@" + key, ConvertBy(dc.DataType), dc.MaxLength, key);
            }
            sda.DeleteCommand.UpdatedRowSource = UpdateRowSource.None;
            sda.UpdateBatchSize = 0;
            SqlCommandBuilder scb = new SqlCommandBuilder();
            scb.ConflictOption = ConflictOption.OverwriteChanges;
            scb.SetAllValues = false;
            scb.DataAdapter = sda;

            DataTableStd dtsdata = DataTableStd.ParseStd(data);

            try
            {
                for (int count = 0; count < dtsdata.RowLength; count++)
                {

                    foreach (string colname in ds[0].ColumnNames)
                    {
                        ds[0].SetNewRowValue(dtsdata[count, colname], colname);
                    }
                    ds[0].AddNewRow();
                    if (((count + 1) % 200 == 0) || ((count + 1) == dtsdata.RowLength))
                    {
                        ds.AcceptChanges();

                        for (int i = 0; i < 200; i++)
                        {
                            if (i >= ds[0].Value.Rows.Count) break;

                            ds[0].Value.Rows[i].BeginEdit();
                            ds[0].Value.Rows[i].Delete();
                            ds[0].Value.Rows[i].EndEdit();

                        }
                        sda.Update(ds[0].Value);
                        ds[0].ClearData();
                    }
                }
            }
            finally
            {
                tsqlcomm.Cancel();
                tsqlcomm.Dispose();
                sda.Dispose();
                ds.Dispose();
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
