using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System.Data;
using Npgsql;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using System.IO;
using System.Reflection;
using System.Linq;
using NpgsqlTypes;
using System.Data.SqlClient;
using System.Data.Common;
using EFFC.Frame.Net.Base.Common;
using EFFC.Extends.LinqDLR2SQL;

namespace EFFC.Frame.Net.Resource.Postgresql
{
    public class PostgreSqlAccess : ADBAccess, IResourceEntity, IDisposable
    {
        string _id = "";
        NpgsqlConnection conn = null;
        NpgsqlTransaction tran = null;
        private NpgsqlCommand sqlcomm = null;
        /// <summary>
        /// 需要在open的时候开启trans
        /// </summary>
        private bool isneedopentrans_in_open = false;
        /// <summary>
        /// Command timeout设定为1小时
        /// </summary>
        protected override int CommandTimeOut => 10 * 60000;
        public override DataSetStd Query(string sql, DBOParameterCollection dbp)
        {
            var newsql = ConvertSQL(sql);
            if (sqlcomm == null)
            {
                sqlcomm = new NpgsqlCommand(newsql, this.conn);
                sqlcomm.CommandTimeout = CommandTimeOut;
            }
            else
            {
                sqlcomm.CommandText = newsql;
            }
            
            //如果事務開啟，則使用事務的方式
            if (this._s == DBStatus.Begin_Trans)
                sqlcomm.Transaction = this.tran;
            DataSetStd ds = new DataSetStd();
            NpgsqlDataReader ddr = null;
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
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    throw ex;
            //}
            finally
            {

                if (ddr != null)
                {
                    ddr.Close();
                    ddr.Dispose();
                }
                sqlcomm.Dispose();
                sqlcomm = null;
            }
            return ds;
        }

        public override void Open(string connString)
        {
            var dt = DateTime.Now;
            if (conn == null
               || this.conn.State == ConnectionState.Closed)
            {
                this.conn = new NpgsqlConnection(connString);
            }
            if (this.conn.State != ConnectionState.Open)
            {
                this.conn.Open();
            }
            Console.WriteLine($"open conn cast time:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
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
                if (this.conn.State == ConnectionState.Executing)
                {
                    sqlcomm.Cancel();
                    sqlcomm = null;
                }
                else
                {
                    sqlcomm.Dispose();
                }
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

                //PostgreSQL的参数符号为:,但因其:用法很多，故改为其它的符号替代，然后再做sql转换工作
                if (!command.Parameters.Contains(ParameterFlagChar + parameters[parameterName].ParameterName))
                {
                    idparameter = command.CreateParameter();
                    idparameter.ParameterName = string.Concat(":", parameters[parameterName].ParameterName);

                    command.Parameters.Add(idparameter);
                }
                else
                {
                    idparameter = command.Parameters[":" + parameterName];
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
            var newsql = ConvertSQL(sql);
            if (sqlcomm == null)
            {
                sqlcomm = new NpgsqlCommand(newsql, this.conn);
                sqlcomm.CommandTimeout = CommandTimeOut;
            }
            else
            {
                sqlcomm.CommandText = newsql;
            }

            if (dbp != null)
            {
                FillParametersToCommand(sqlcomm, dbp);
            }

            if (_s == DBStatus.Begin_Trans)
                sqlcomm.Transaction = this.tran;

            using (sqlcomm)
            {
                sqlcomm.ExecuteNonQuery();
            }
        }

        public override void Update(object data, string selectsql)
        {
            //
        }

        public override void Insert(object data, string toTable)
        {
            
        }
        /// <summary>
        /// PostgreSQL的参数符号为:,但因其:用法很多，故改为其它的符号替代，然后再做sql转换工作
        /// </summary>
        /// <param name="sourcesql"></param>
        /// <returns></returns>
        private string ConvertSQL(string sourcesql)
        {
            return sourcesql.Replace(ParameterFlagChar, ":");
        }
        /// <summary>
        /// 将Type转化成SqlDbType
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public NpgsqlDbType ConvertBy(System.Type t)
        {
            NpgsqlParameter pl = new NpgsqlParameter();
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

            return pl.NpgsqlDbType;
        }
        public override void Delete(object data, string toTable)
        {
            if (data == null) return;
            string sql = "delete from " + toTable;
            string where = "";
            DBOParameterCollection dpc = new DBOParameterCollection();
            if (data is FrameDLRObject)
            {
                var dobj = (FrameDLRObject)data;
                var keycols = dobj.Keys;
                foreach (var key in keycols)
                {
                    dpc.SetValue(key, dobj.GetValue(key));
                    where += (where == "" ? " where " : " and ") + $" {key}={ParameterFlagChar}{key}";
                }
            }
            else
            {
                var fields = data.GetType().GetTypeInfo().GetFields(BindingFlags.Instance | BindingFlags.Public).ToList();
                foreach (var f in fields)
                {
                    dpc.SetValue(f.Name, f.GetValue(data));
                    where += (where == "" ? " where " : " and ") + $" {f.Name}={ParameterFlagChar}{f.Name}";
                }
            }


            sql = sql + where;
            this.ExecuteNoQuery(sql, dpc);

        }

        public override DBDataCollection ExcuteProcedure(string sp_name, bool isReturnDataSet, ref DBOParameterCollection dbp)
        {
            DBDataCollection rtn = new DBDataCollection();
            rtn.IsSuccess = false;

            DataSetStd ds = new DataSetStd();
            NpgsqlCommand dc = null;//new SqlCommand(p.StoreProcureName, this.sqlconn);
            if (this._s == DBStatus.Begin_Trans)
            {
                dc = new NpgsqlCommand(sp_name, this.conn, this.tran);
            }
            else
            {
                dc = new NpgsqlCommand(sp_name, this.conn);
            }
            //dc.CommandTimeout = 90;
            dc.CommandType = CommandType.StoredProcedure;
            dc.CommandTimeout= CommandTimeOut;
            FillParametersToCommand(dc, dbp);
            NpgsqlDataReader ddr = null;
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
                        rtn.SetValue(sp.ParameterName.Replace(ParameterFlagChar, ""), sp.Value);
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

        protected override DataTableStd QueryByPage(int startRow, int endRow, int toPage, int count_of_page, string sql, string orderby, DBOParameterCollection p)
        {
            var newsql = sql;
            if (orderby != null && orderby != "")
            {
                newsql += " order by " + orderby;
            }

            newsql = string.Format(newsql + " limit {0} offset {1}", count_of_page, (startRow - 1));
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
                    _id = "NpgsqlDBAccess" + Guid.NewGuid().ToString();
                }

                return _id;
            }
        }

        public override string ParameterFlagChar => "@";
        PostgreSqlExpress _express = new PostgreSqlExpress();
        public override DBExpress MyDBExpress => _express;

        public override DBType MyType => DBType.PostgreSQL;
        /// <summary>
        /// 生成一个LinqDLR2SQL对象用于Linq操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="alianname"></param>
        /// <returns></returns>
        public override LinqDLRTable NewLinqTable(string table, string alianname = "")
        {
            var tn = alianname == "" ? table : alianname;
            LinqDLRTable rtn = LinqDLRTable.New<LinqDLRTable>(new PostgreSQLLamdaSQLObject(tn, new PostgreSqlOperatorFlags()), table, alianname, new PostgreSQLGenerator());
            return rtn;
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
