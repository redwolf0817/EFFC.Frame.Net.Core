using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Interfaces;
using System.Data.OracleClient;
using EFFC.Frame.Net.Base.Data;
using System.Data.Common;
using System.Data;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Constants;
using System.Text.RegularExpressions;
using EFFC.Frame.Net.Base.Common;

namespace EFFC.Frame.Net.Base.ResouceManage.DB
{
    public class OracleAccess : ADBAccess, IResourceEntity, IDisposable
    {
        string _id = "";
        OracleConnection conn;
        OracleTransaction tran;

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
                    _id = "OracleDBAccess" + Guid.NewGuid().ToString();
                }

                return _id;
            }
        }

        public void Release()
        {
            Close();
        }

        private string ReplaceSingleRefer(string str, ref Dictionary<string, string> reserved)
        {
            if (str.IndexOf("'") >= 0)
            {
                var regstr = @"(?isx)
                       '                          #普通字符“(”

                            (?>                     #分组构造，用来限定量词“*”修饰范围

                                [^']+              #非括弧的其它任意字符

                            |                       #分支结构

                                '  (?<Open>)       #命名捕获组，遇到开括弧Open计数加1

                            |                       #分支结构

                                '  (?<-Open>)      #狭义平衡组，遇到闭括弧Open计数减1

                            )*                      #以上子串出现0次或任意多次

                            (?(Open)(?!))           #判断是否还有'OPEN'，有则说明不配对，什么都不匹配

                        '                          #普通闭括弧
";
                Regex re = new Regex(regstr);
                var strtemp = str;
                foreach (Match m in re.Matches(strtemp))
                {
                    string key = "#" + Guid.NewGuid().ToString() + "#";
                    reserved.Add(key, m.Value);
                    strtemp = strtemp.Replace(m.Value, key);
                }

                return ReplaceSingleRefer(strtemp, ref reserved);
            }
            else
            {
                return str;
            }
        }

        private string[] ToSQLArray(string sql)
        {
            var reserved = new Dictionary<string, string>();
            var sqltmp = ReplaceSingleRefer(sql, ref reserved);
            sqltmp = sqltmp.Replace(";", "#__sp__#");
            string[] keys = reserved.Keys.ToArray();
            for (int i = keys.Length - 1; i >= 0; i--)
            {
                var item = reserved[keys[i]];
                sqltmp = sqltmp.Replace(keys[i], item);
            }
            var stingarr = new string[] { "#__sp__#" };
            var sqlarray = sqltmp.Split(stingarr, StringSplitOptions.RemoveEmptyEntries);

            return sqlarray;
        }

        public override Data.DataSetStd Query(string sql, DBOParameterCollection dbp)
        {
            if (_s == DBStatus.Close)
            {
                DoOpen();
            }

            OracleCommand cmd;
            DataSetStd ds = new DataSetStd();
            var sqlarr = ToSQLArray(sql);
            var index = 1;
            foreach (string s in sqlarr)
            {
                if (s.Trim() == "") continue;
                using (cmd = new OracleCommand(s, conn))
                {

                    try
                    {
                        //如果事務開啟，則使用事務的方式
                        if (this._s == DBStatus.Begin_Trans)
                            cmd.Transaction = this.tran;
                        if (s.Trim().ToLower().StartsWith("select") || s.ToLower().IndexOf(" into ") < 0)
                        {
                            OracleDataAdapter rd = new OracleDataAdapter(cmd);
                            //如果有參數
                            if (dbp != null)
                            {
                                FillParametersToCommand(cmd, dbp, s);
                            }

                            rd.Fill(ds, "table" + index);

                            index++;
                        }
                        else
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    finally
                    {
                        cmd.Cancel();
                        cmd = null;
                    }
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
                if (!command.Parameters.Contains("" + parameters[parameterName].ParameterName))
                {
                    idparameter = command.CreateParameter();
                    idparameter.ParameterName = string.Concat("", parameters[parameterName].ParameterName);

                    command.Parameters.Add(idparameter);
                }
                else
                {
                    idparameter = command.Parameters["" + parameterName];
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

        private void FillParametersToCommand(DbCommand command, DBOParameterCollection parameters,string sql)
        {
            DBOParameterCollection filtercollection = new DBOParameterCollection();
            string regstr = @"(?<=:)[A-Za-z0-9_]+\d*";
            Regex reg = new Regex(regstr);
            foreach (Match m in reg.Matches(sql))
            {
                if (parameters.ContainsKey(m.Value))
                {
                    filtercollection.Add(m.Value, parameters[m.Value]);
                }
            }

            FillParametersToCommand(command, filtercollection);

        }

        public override void Open(string connString)
        {
            if (conn == null)
            {
                this.conn = new OracleConnection(connString);
                
            }

            DoOpen();
        }

        private void DoOpen()
        {
            if (this.conn.State == ConnectionState.Closed)
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
                this._s = DBStatus.Close;
            }
        }

        public override void BeginTransaction(IsolationLevel level)
        {
            if (conn != null)
            {
                if (conn.State == ConnectionState.Open)
                {
                    this.tran = this.conn.BeginTransaction(level);
                    this._s = DBStatus.Begin_Trans;
                }
                else
                {
                    isneedopentrans_in_open = true;
                }
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
            if (_s == DBStatus.Close)
            {
                DoOpen();
            }

            OracleCommand cmd;
            DataSetStd ds = new DataSetStd();
            var sqlarr = ToSQLArray(sql);
            foreach (var s in sqlarr)
            {
                if (s.Trim() == "") continue;
                using (cmd = new OracleCommand(s, conn))
                {

                    if (cmd.Connection.State == ConnectionState.Closed)
                    {
                        cmd.Connection.Open();
                    }
                    try
                    {
                        //如果事務開啟，則使用事務的方式
                        if (this._s == DBStatus.Begin_Trans)
                            cmd.Transaction = this.tran;

                        cmd.CommandText = s;
                        //如果有參數
                        if (dbp != null)
                        {
                            FillParametersToCommand(cmd, dbp, s);
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
        }

        public override void Update(System.Data.DataTable data, string selectsql)
        {
            if (_s == DBStatus.Close)
            {
                DoOpen();
            }

            OracleCommand cmd;
            using (cmd = new OracleCommand(selectsql, conn))
            {
                if (cmd.Connection.State == ConnectionState.Closed)
                {
                    cmd.Connection.Open();
                }

                if (this._s == DBStatus.Begin_Trans)
                {
                    cmd.Transaction = this.tran;
                }

                OracleDataAdapter adt = new OracleDataAdapter(cmd);
                OracleCommandBuilder builder = new OracleCommandBuilder(adt);

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
            //OracleBulkCopy bulk;
            //if (this._s == DBStatus.Begin_Trans)
            //    bulk = new OracleBulkCopy(conn, OracleBulkCopyOptions.Default);
            //else
            //    bulk = new OracleBulkCopy(conn);

            //try
            //{
            //    bulk.DestinationTableName = toTable;
            //    bulk.WriteToServer(data);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    bulk.Close();
            //}

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
            if (_s == DBStatus.Close)
            {
                DoOpen();
            }

            DataSetStd ds = new DataSetStd();
            OracleCommand dc = null;
            if (this._s == DBStatus.Begin_Trans)
            {
                dc = new OracleCommand(sp_name, conn);
                dc.Transaction = tran;
            }
            else
            {
                dc = new OracleCommand(sp_name, conn);
            }
            dc.CommandType = CommandType.StoredProcedure;
            FillParametersToCommand(dc, dbp);
            try
            {

                if (isReturnDataSet)
                {
                    OracleDataAdapter sqlDa = new OracleDataAdapter();
                    sqlDa.SelectCommand = dc;
                    sqlDa.Fill(ds);
                    rtn.ReturnDataSet = ds;
                }
                else
                {
                    dc.ExecuteNonQuery();
                }
                //獲取返回值
                foreach (OracleParameter sp in dc.Parameters)
                {
                    if (sp.Direction == ParameterDirection.Output || sp.Direction == ParameterDirection.InputOutput || sp.Direction == ParameterDirection.ReturnValue)
                        rtn.SetValue(sp.ParameterName.Replace(":", ""), sp.Value);
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
            if (_s == DBStatus.Close)
            {
                DoOpen();
            }

            string orderby4page = orderby;
            if (orderby == null || orderby == "")
            {
                orderby4page = GetColumnsNameBySql(sql, p)[0] + " ASC ";
            }
            if (sql.Trim().EndsWith(";"))
            {
                sql = sql.Trim().Substring(0, sql.Trim().Length - 1);
            }

            string newsql = @"select * from "
                + "(select table2.*,ROW_NUMBER() OVER (order by " + orderby4page + " ) as RowNumber from (" + sql + ") table2) a "
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
