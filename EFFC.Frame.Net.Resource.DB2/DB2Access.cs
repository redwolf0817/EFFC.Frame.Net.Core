using IBM.Data.DB2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Common;

namespace EFFC.Frame.Net.Base.ResouceManage.DB
{
    public class DB2Access:ADBAccess,IResourceEntity,IDisposable
    {
        string _id = "";
        DB2Connection conn;
        DB2Transaction tran;

        /// <summary>
        /// 需要在open的时候开启trans
        /// </summary>
        private bool isneedopentrans_in_open = false;

        public string ID
        {
            get
            {
                if (_id == "")
                {
                    _id = "DB2DBAccess" + Guid.NewGuid().ToString();
                }

                return _id;
            }
        }

        public void Release()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }

        public override Data.DataSetStd Query(string sql, DBOParameterCollection dbp)
        {
            DB2Command cmd;
            DataSetStd ds = new DataSetStd();
            using (cmd = new DB2Command(sql, conn))
            {
                
                try
                {
                    //如果事務開啟，則使用事務的方式
                    if (this._s == DBStatus.Begin_Trans)
                        cmd.Transaction = this.tran;
                    
                    DB2DataAdapter rd = new DB2DataAdapter(cmd);
                    //如果有參數
                    if (dbp != null)
                    {
                        FillParametersToCommand(cmd, dbp);
                    }

                    rd.Fill(ds);


                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Cancel();
                    cmd = null;
                }
            }

            return ds;
        }

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

        public override void Open(string connString)
        {
            if (conn == null
                || this.conn.State == ConnectionState.Closed)
            {
                this.conn = new DB2Connection(connString);
            }
            if (this.conn.State != ConnectionState.Open)
            {
                this.conn.Open();
            }
            if (isneedopentrans_in_open)
            {
                this.tran = conn.BeginTransaction();
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
            if (conn != null)
            {
                conn.Close();
            }
        }

        //public override void BeginTransaction()
        //{
        //    if (conn != null)
        //    {
        //        this.tran = this.conn.BeginTransaction();
        //        this._s = DBStatus.Begin_Trans;
        //    }
        //    else
        //    {
        //        isneedopentrans_in_open = true;
        //    }
        //}

        public override void BeginTransaction(IsolationLevel level)
        {
            if (conn != null)
            {
                this.tran = this.conn.BeginTransaction(level);
                this._s = DBStatus.Begin_Trans;
            }
            else
            {
                isneedopentrans_in_open = true;
            }
        }

        public override void CommitTransaction()
        {
            if (this._s == DBStatus.Begin_Trans)
            {
                this.tran.Commit();
                this._s = DBStatus.Commit_Trans;
            }
        }

        public override void RollbackTransaction()
        {
            if (this._s == DBStatus.Begin_Trans)
            {
                this.tran.Rollback();
                this._s = DBStatus.RollBack_Trans;
            }
        }

        public override void ExecuteNoQuery(string sql, DBOParameterCollection dbp)
        {
            DB2Command cmd;
            DataSetStd ds = new DataSetStd();
            using (cmd = new DB2Command(sql, conn))
            {

                try
                {
                    //如果事務開啟，則使用事務的方式
                    if (this._s == DBStatus.Begin_Trans)
                        cmd.Transaction = this.tran;

                    cmd.CommandText = sql;
                    //如果有參數
                    if (dbp != null)
                    {
                        FillParametersToCommand(cmd, dbp);
                    }
                    if (_s == DBStatus.Begin_Trans)
                        cmd.Transaction = this.tran;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Cancel();
                    cmd = null;
                }
            }
        }

        public override void Update(System.Data.DataTable data, string selectsql)
        {
            DB2Command cmd;
            using (cmd = new DB2Command(selectsql, conn))
            {

                if (this._s == DBStatus.Begin_Trans)
                {
                    cmd.Transaction = this.tran;
                }

                DB2DataAdapter adt = new DB2DataAdapter(cmd);
                DB2CommandBuilder builder = new DB2CommandBuilder(adt);

                try
                {
                    adt.UpdateCommand = builder.GetUpdateCommand();
                    adt.Update(data);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cmd.Cancel();
                    cmd = null;
                }
            }
        }

        public override void Insert(System.Data.DataTable data, string toTable)
        {
            DB2BulkCopy bulk;
            if (this._s == DBStatus.Begin_Trans)
                bulk = new DB2BulkCopy(conn, DB2BulkCopyOptions.Default);
            else
                bulk = new DB2BulkCopy(conn);

            try
            {
                bulk.DestinationTableName = toTable;
                bulk.WriteToServer(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bulk.Close();
            }

        }
        /// <summary>
        /// 暂不提供
        /// </summary>
        /// <param name="data"></param>
        /// <param name="toTable"></param>
        public override void Delete(System.Data.DataTable data, string toTable)
        {
            //
        }

        public override DBDataCollection ExcuteProcedure(string sp_name, bool isReturnDataSet, ref DBOParameterCollection dbp)
        {
            DBDataCollection rtn = new DBDataCollection();
            rtn.IsSuccess = false;

            DataSetStd ds = new DataSetStd();
            DB2Command dc = null;
            if (this._s == DBStatus.Begin_Trans)
            {
                dc = new DB2Command(sp_name, conn, tran);
            }
            else
            {
                dc = new DB2Command(sp_name, conn);
            }
            dc.CommandType = CommandType.StoredProcedure;
            FillParametersToCommand(dc, dbp);
            try
            {

                if (isReturnDataSet)
                {
                    DB2DataAdapter sqlDa = new DB2DataAdapter();
                    sqlDa.SelectCommand = dc;
                    sqlDa.Fill(ds);
                    rtn.ReturnDataSet = ds;
                }
                else
                {
                    dc.ExecuteNonQuery();
                }
                //獲取返回值
                foreach (DB2Parameter sp in dc.Parameters)
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


        protected override Data.DataTableStd QueryByPage(int startRow, int endRow, int toPage, int count_of_page, string sql, string orderby, DBOParameterCollection p)
        {
            string orderby4page = orderby;
            if (orderby == null || orderby == "")
            {
                orderby4page = GetColumnsNameBySql(sql, p)[0] + " ASC ";
            }

            string newsql = @"select * from "
                + "(select *,ROW_NUMBER() OVER (order by " + orderby4page + " ) as RowNumber from (" + sql + ") table2) a "
                + " where RowNumber > " + (count_of_page * (toPage - 1)) + " and RowNumber <=" + (count_of_page * toPage);
               
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

        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

       
    }
}
