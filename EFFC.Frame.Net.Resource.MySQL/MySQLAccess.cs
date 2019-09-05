using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB.Parameters;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
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
        protected override int CommandTimeOut => 60 * 60000;
        /// <summary>
        /// 需要在open的时候开启trans
        /// </summary>
        private bool isneedopentrans_in_open = false;
        public override Data.DataSetStd Query(string sql, DBOParameterCollection dbp)
        {
            if (sqlcomm == null)
            {
                sqlcomm = new MySqlCommand(sql, this.conn);
                sqlcomm.CommandTimeout = CommandTimeOut;
            }
            else
            {
                sqlcomm.CommandText = sql;
            }

            //如果事務開啟，則使用事務的方式
            if (this._s == DBStatus.Begin_Trans)
                sqlcomm.Transaction = this.tran;
            DataSetStd ds = new DataSetStd();
            MySqlDataReader ddr = null;
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
                this.conn = new MySqlConnection(connString);
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
                if(this.conn.State == ConnectionState.Executing)
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
                sqlcomm.CommandTimeout = CommandTimeOut;
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
            if (this.conn.State == ConnectionState.Closed)
            {
                this.conn.Open();
            }

            MySqlBulkLoader sbc;
            sbc = new MySqlBulkLoader(this.conn);
            string tmpPath = ComFunc.GetApplicationRoot() + "/" + ComFunc.RandomCode(6) + ".csv"; //Path.GetTempFileName();
            //Console.WriteLine($"csv file path={tmpPath}");
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
                if (data is DataTableStd)
                {
                    string csv = DataTableToCsv((DataTableStd)data);
                    File.WriteAllText(tmpPath, csv);
                }
                //else if (data is IEnumerable<FrameDLRObject>)
                //{
                //    string csv = ListToCsv((IEnumerable<FrameDLRObject>)data);
                //    File.WriteAllText(tmpPath, csv);
                //}
                else if (data is IEnumerable<object>)
                {
                    string csv = ListToCsv((IEnumerable<object>)data);
                    File.WriteAllText(tmpPath, csv);
                }

                bulk.Load();
            }
            finally
            {
                File.Delete(tmpPath);
            }
            // BulkInsert(data, toTable);
        }
        private void BulkInsert(object data, string toTable)
        {
            if (data == null) return;

            var sql = $"INSERT INTO {toTable}(#col#)VALUES";
            var parameters = new DBOParameterCollection();
            if (data is IEnumerable<FrameDLRObject>)
            {
                var cols = "";
                var values = new StringBuilder();
                var param = "";
                var list = (IEnumerable<FrameDLRObject>)data;
                if (list.Count() <= 0) return;
                if (list.ElementAt(0).Keys.Count <= 0) return;

               
                foreach (var k in list.ElementAt(0).Keys)
                {
                    cols += $",{k}";
                    param += $",{ParameterFlagChar}{k}#no#";                    
                }
                sql = sql.Replace("#col#", cols.Substring(1));

                var index = 0;
                
                foreach (var item in (IEnumerable<FrameDLRObject>)data)
                {
                    values.AppendLine($",({param.Substring(1).Replace("#no#", index + "")})");
                    foreach(var key in item.Keys)
                    {
                        parameters.Add($"{key}{index}",item.GetValue(key));
                    }
                    index++;
                }

                sql += values.ToString().Substring(1);
            }
            ExecuteNoQuery(sql, parameters);


        }
        private static string ListToCsv(IEnumerable<object> list)
        {
            if (list == null || list.Count() <= 0) return "";
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。  
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。  
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。  
            StringBuilder sb = new StringBuilder();
            var obj = list.First();
            if(obj is FrameDLRObject)
            {
                var fobj = (FrameDLRObject)obj;
                var columns = fobj.Keys;

                foreach (FrameDLRObject item in list)
                {
                    var str = "";
                    foreach (var c in columns)
                    {
                        var v = item.GetValue(c);
                        if ((v is string) && v.ToString().Contains(","))
                        {
                            str += "\"" + v.ToString().Replace("\"", "\"\"") + "\",";
                        }
                        else if (v is DateTime)
                        {
                            str += ((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",";
                        }
                        else str += (v != null ? v.ToString() : "") + ",";


                    }
                    str = str.Length > 0 ? str.Substring(0, str.Length - 1) : "";
                    sb.Append(str + "\r\n");
                }
            }
            else
            {
                var columns = obj.GetType().GetTypeInfo().GetFields(BindingFlags.Instance | BindingFlags.Public).ToList();
                if (columns.Count > 0)
                {
                    foreach (var item in list)
                    {
                        var str = "";
                        foreach (var c in columns)
                        {
                            var v = c.GetValue(item);
                            if (c.FieldType == typeof(string) && v.ToString().Contains(","))
                            {
                                str += "\"" + v.ToString().Replace("\"", "\"\"") + "\",";
                            }
                            else if (v is DateTime)
                            {
                                str += ((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",";
                            }
                            else str += (v != null ? v.ToString() : "") + ",";


                        }
                        str = str.Length > 0 ? str.Substring(0, str.Length - 1) : "";
                        sb.Append(str + "\r\n");
                    }
                }
                else
                {
                    var properties = obj.GetType().GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
                    foreach (var item in list)
                    {
                        var str = "";
                        foreach (var c in properties)
                        {
                            var v = c.GetValue(item);
                            if (c.DeclaringType == typeof(string) && v.ToString().Contains(","))
                            {
                                str += "\"" + v.ToString().Replace("\"", "\"\"") + "\",";
                            }
                            else if (v is DateTime)
                            {
                                str += ((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss.fff") + ",";
                            }
                            else str += (v != null ? v.ToString() : "") + ",";
                        }
                        str = str.Length > 0 ? str.Substring(0, str.Length - 1) : "";
                        sb.Append(str + "\r\n");
                    }
                }
            }
            
            

            return sb.ToString();
        }
        ///将DataTable转换为标准的CSV  
        /// </summary>  
        /// <param name="table">数据表</param>  
        /// <returns>返回标准的CSV</returns>  
        private static string DataTableToCsv(DataTableStd table)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。  
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。  
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。  
            StringBuilder sb = new StringBuilder();
            System.Data.DataColumn colum;
            foreach (FrameDLRObject row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    var v = row.GetValue(colum.ColumnName);
                    if (colum.DataType == typeof(string) && row.GetValue(colum.ColumnName).ToString().Contains(","))
                    {
                        sb.Append("\"" + v.ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else if (colum.DataType == typeof(DateTime))
                    {
                        sb.Append(((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    }
                    else sb.Append(v != null ? v.ToString() : "");
                }
                sb.Append("\r\n");
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
        public override void Delete(object data, string toTable)
        {
            if (data == null) return;
            string sql = "delete from " + toTable;
            string where = "";
            DBOParameterCollection dpc = new DBOParameterCollection();
            if(data is FrameDLRObject)
            {
                var dobj = (FrameDLRObject)data;
                var keycols = dobj.Keys;
                foreach(var key in keycols)
                {
                    dpc.SetValue(key, dobj.GetValue(key));
                    where += (where == "" ? " where " : " and ") + $" {key}=@{key}";
                }
            }
            else
            {
                var fields = data.GetType().GetTypeInfo().GetFields(BindingFlags.Instance | BindingFlags.Public).ToList();
                foreach(var f in fields)
                {
                    dpc.SetValue(f.Name, f.GetValue(data));
                    where += (where == "" ? " where " : " and ") + $" {f.Name}=@{f.Name}";
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
            MySqlCommand dc = null;//new SqlCommand(p.StoreProcureName, this.sqlconn);
            if (this._s == DBStatus.Begin_Trans)
            {
                dc = new MySqlCommand(sp_name, this.conn, this.tran);
            }
            else
            {
                dc = new MySqlCommand(sp_name, this.conn);
            }
            dc.CommandTimeout = CommandTimeOut;
            dc.CommandType = CommandType.StoredProcedure;
            FillParametersToCommand(dc, dbp);
            MySqlDataReader ddr = null;
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
                if(ddr!= null)
                {
                    ddr.Close();
                    ddr.Dispose();
                }
                dc.Dispose();
                dc = null;
            }

            return rtn;
        }

        protected override Data.DataTableStd QueryByPage(int startRow, int endRow, int toPage, int count_of_page, string sql, string orderby, DBOParameterCollection p)
        {
            var newsql = sql;
            if (orderby != null && orderby != "")
            {
                newsql += " order by " + orderby;
            }

            newsql = string.Format(newsql + " limit {0} offset {1}", count_of_page, startRow - 1);
            //GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"MySql QueryByPage Sql={newsql}");
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

        public override string ParameterFlagChar => "@";
        MySQLExpress _express = new MySQLExpress();
        public override DBExpress MyDBExpress => _express;

        public override DBType MyType => DBType.MySql;

        /// <summary>
        /// 生成一个LinqDLR2SQL对象用于Linq操作
        /// </summary>
        /// <param name="table"></param>
        /// <param name="alianname"></param>
        /// <returns></returns>
        public override LinqDLRTable NewLinqTable(string table, string alianname = "")
        {
            var tn = alianname == "" ? table : alianname;
            LinqDLRTable rtn = LinqDLRTable.New<LinqDLRTable>(new MySqlLamdaSQLObject(tn, new MySQLOperatorFlags()), table, alianname, new MySqlGenerator());
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
