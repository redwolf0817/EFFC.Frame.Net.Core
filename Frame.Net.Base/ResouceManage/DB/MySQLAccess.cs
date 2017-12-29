using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.ResouceManage.DB
{
    public class MySQLAccess:ADBAccess,IResourceEntity,IDisposable
    {
        string _id = "";
        MySqlConnection conn = null;
        MySqlTransaction tran = null;
        private MySqlCommand sqlcomm = null;
        /// <summary>
        /// 需要在open的时候开启trans
        /// </summary>
        private bool isneedopentrans_in_open = false;
        public override Data.DataSetStd Query(string sql, DBOParameterCollection dbp)
        {
            if (sqlcomm == null)
            {
                sqlcomm = new MySqlCommand(sql, this.conn);
                sqlcomm.CommandTimeout = 90;
            }
            else
            {
                sqlcomm.CommandText = sql;
            }
            //如果事務開啟，則使用事務的方式
            if (this._s == DBStatus.Begin_Trans)
                sqlcomm.Transaction = this.tran;


            DataSetStd ds = new DataSetStd();
            try
            {
                //如果有參數
                if (dbp != null)
                {
                    FillParametersToCommand(sqlcomm, dbp);
                }
                MySqlDataAdapter sqlAdapter = new MySqlDataAdapter(this.sqlcomm);
                sqlAdapter.Fill(ds);
            }
            catch (Exception ex)
            {
                sqlcomm.Cancel();
                throw ex;
            }
            finally
            {
                //sqlcomm.Cancel();
                //sqlcomm = null;
            }
            return ds;
        }

        public override void Open(string connString)
        {
            if (conn == null
               || this.conn.State == ConnectionState.Closed)
            {
                this.conn = new MySqlConnection(connString);
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
            if (sqlcomm != null)
            {
                sqlcomm.Cancel();
                sqlcomm = null;
            }
            if (this.conn != null && (this.conn.State == ConnectionState.Open || this.conn.State == ConnectionState.Connecting || this.conn.State == ConnectionState.Executing))
            {
                this.conn.Close();
            }
            this.conn = null;
            this._s = DBStatus.Close;
        }

        public override void BeginTransaction(System.Data.IsolationLevel level)
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
        public override void ExecuteNoQuery(string sql, DBOParameterCollection dbp)
        {
            if (sqlcomm == null)
            {
                sqlcomm = new MySqlCommand(sql, this.conn);
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
                sqlcomm.Transaction = this.tran;

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
                sqlcomm.Cancel();
                sqlcomm = null;
            }
        }

        public override void Update(System.Data.DataTable data, string selectsql)
        {
            if (sqlcomm == null)
            {
                this.sqlcomm = new MySqlCommand(selectsql, this.conn);
                this.sqlcomm.CommandTimeout = 90;
            }
            else
            {
                sqlcomm.CommandText = selectsql;
            }

            if (this._s == DBStatus.Begin_Trans)
            {
                sqlcomm.Transaction = this.tran;
            }

            MySqlDataAdapter adt = new MySqlDataAdapter(this.sqlcomm);
            MySqlCommandBuilder builder = new MySqlCommandBuilder(adt);

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

        public override void Insert(System.Data.DataTable data, string toTable)
        {
            if (this.conn.State == ConnectionState.Closed)
            {
                this.conn.Open();
            }

            MySqlBulkLoader sbc;
            sbc = new MySqlBulkLoader(this.conn);
            string tmpPath = Path.GetTempFileName();
            MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
            {
                FieldTerminator = ",",
                FieldQuotationCharacter = '"',
                EscapeCharacter = '"',
                LineTerminator = "\r\n",
                FileName = tmpPath,
                NumberOfLinesToSkip = 0,
                TableName = toTable,
            };
            try
            {

                string csv = DataTableToCsv(data);
                File.WriteAllText(tmpPath, csv);

                bulk.Load();
            }
            finally
            {
                File.Delete(tmpPath);
            }
        }
        ///将DataTable转换为标准的CSV  
        /// </summary>  
        /// <param name="table">数据表</param>  
        /// <returns>返回标准的CSV</returns>  
        private static string DataTableToCsv(DataTable table)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。  
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。  
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。  
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }


            return sb.ToString();
        }
        /// <summary>
        /// 将Type转化成SqlDbType
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public MySqlDbType ConvertBy(System.Type t)
        {
            MySqlParameter pl = new MySqlParameter();
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
                catch (Exception ex)
                {
                    //do nothing
                }
            }

            return pl.MySqlDbType;
        }
        public override void Delete(System.Data.DataTable data, string toTable)
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

            MySqlCommand tsqlcomm = new MySqlCommand("select * from " + toTable + " where 1=0 ", this.conn);
            tsqlcomm.CommandTimeout = 90;


            if (this._s == DBStatus.Begin_Trans)
                tsqlcomm.Transaction = this.tran;

            MySqlDataAdapter sda = new MySqlDataAdapter(tsqlcomm);
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
            MySqlCommandBuilder scb = new MySqlCommandBuilder();
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

        public override DBDataCollection ExcuteProcedure(string sp_name, bool isReturnDataSet, ref DBOParameterCollection dbp)
        {
            DBDataCollection rtn = new DBDataCollection();
            rtn.IsSuccess = false;

            DataSetStd ds = new DataSetStd();
            MySqlCommand dc = null;//new SqlCommand(p.StoreProcureName, this.sqlconn);
            if (this._s == DBStatus.Begin_Trans)
            {
                dc = new MySqlCommand(sp_name, this.conn, this.tran);
            }
            else
            {
                dc = new MySqlCommand(sp_name, this.conn);
            }
            dc.CommandTimeout = 90;
            dc.CommandType = CommandType.StoredProcedure;
            FillParametersToCommand(dc, dbp);
            try
            {

                if (isReturnDataSet)
                {
                    MySqlDataAdapter sqlDa = new MySqlDataAdapter();
                    sqlDa.SelectCommand = dc;
                    sqlDa.Fill(ds);
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
                dc.Cancel();
                dc = null;
            }

            return rtn;
        }

        protected override Data.DataTableStd QueryByPage(int startRow, int endRow, int toPage, int count_of_page, string sql, string orderby, DBOParameterCollection p)
        {
            var newsql = sql;
            if (orderby != null && orderby != "")
            {
                newsql += "order by " + orderby;
            }

            newsql = string.Format(newsql + " limit {0},{1}", startRow - 1, count_of_page);
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

        public string ID
        {
            get
            {

                if (_id == "")
                {
                    _id = "MySQLDBAccess" + Guid.NewGuid().ToString();
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

        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }
    }
}
