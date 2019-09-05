using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Resource.SQLServer
{
    /// <summary>
    /// SqlServer的DBExpress解析器
    /// </summary>
    public class SqlServerExpress : SqlTypeDBExpress
    {
        protected override string ParameterFlag => "@";

        protected override string LinkFlag => "+"; 
        protected override dynamic ParseCopyData(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            var table = ComFunc.nvl(obj.GetValue("$table"));
            var to_table = ComFunc.nvl(obj.GetValue("$to_table"));
            var if_not_exists = BoolStd.IsNotBoolThen(obj.GetValue("$if_not_exists"), false);
            var filter = obj.GetValue("$where");
            var sql = "INSERT INTO #new_table_name##columns_to# SELECT #columns_from# FROM #table_name# #where#";
            var not_exists_filter = $"NOT EXISTS(SELECT 1 FROM {to_table} #where#)";
            var columns_to = "";
            var columns_from = "";
            var datacollection = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            var where = "";
            var not_exists_where = where;

            if (filter!= null && filter is FrameDLRObject)
            {
                where = SqlWhere((FrameDLRObject)filter, datacollection);
                not_exists_where = where;
            }
            foreach(var k in obj.Keys.Where(w => !w.StartsWith("$")))
            {
                columns_from += $",{ColumnFormat(k)} AS {ColumnFormat(ComFunc.nvl(obj.GetValue(k)))}" ; 
                columns_to += $",{ColumnFormat(ComFunc.nvl(obj.GetValue(k)))}";
                not_exists_where = not_exists_where.Replace(k, ComFunc.nvl(obj.GetValue(k)));
            }

            columns_to = string.IsNullOrEmpty(columns_to) ? "" : $"({columns_to.Substring(1)})";
            columns_from = string.IsNullOrEmpty(columns_from) ? "*" : columns_from.Substring(1);
            if (if_not_exists)
            {
                not_exists_filter = !string.IsNullOrEmpty(not_exists_where) ? not_exists_filter.Replace("#where#", $"WHERE {not_exists_where}") : not_exists_filter.Replace("#where#","");
                where = string.IsNullOrEmpty(where) ? not_exists_filter : where + " AND " + not_exists_filter;
            }
            where = !string.IsNullOrEmpty(where) ? "WHERE " + where : "";


            sql = sql.Replace("#table_name#", table)
                .Replace("#new_table_name#", to_table)
                .Replace("#where#", where)
                .Replace("#columns_to#", columns_to)
                .Replace("#columns_from#",columns_from);

            rtn.sql = sql;
            rtn.table = table;
            rtn.data = datacollection;
            return rtn;
        }

        protected override dynamic ParseCopyTable(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            var table = ComFunc.nvl(obj.GetValue("$table"));
            var to_table = ComFunc.nvl(obj.GetValue("$to_table"));
            var with_data = BoolStd.IsNotBoolThen(obj.GetValue("$with_data"));
            var sql = "SELECT * INTO #new_table_name# FROM #table_name# #where#";
            var columns = "";
            var where = "";
            if (!with_data)
            {
                where = "WHERE 1=2";
            }
            columns = string.IsNullOrEmpty(columns) ? "*" : columns;
            sql = sql.Replace("#table_name#", table)
                .Replace("#new_table_name#", to_table)
                .Replace("#where#", where);

            rtn.sql = sql;
            rtn.table = table;
            return rtn;
        }

        protected override dynamic ParseAlterColumn(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            var table = "";
            var alter_add_template = @"Alter Table #table# Add #column_name# #data_type# #null# #default#";
            var alter_modify_template = @"Alter Table #table# Alter Column #column_name# #data_type# #null#";
            var alter_drop_template = @"Alter Table #table# Drop Column #column_name#";
            var alter_rename_template = @"exec sp_rename '#table#.#column_name#','#new_column_name#'";
            var alter_drop_add_template = @"Alter Table #table# Drop Column #column_name#
Alter Table #table# Add #column_name# #data_type# #null# #default#";
            var sql = new StringBuilder();

            var columns = new List<FrameDLRObject>();
            table = ComFunc.nvl(obj.GetValue("$table"));

            foreach (var item in obj.Items.Where(d=>d.Key.StartsWith("$")==false))
            {
                FrameDLRObject col = (FrameDLRObject)item.Value;
                var column_name = item.Key;
                if (col != null)
                {
                    var alter_action = ComFunc.nvl(col.GetValue("$alter_action"));
                    var new_name = ComFunc.nvl(col.GetValue("$new_name"));
                    var datatype = ComFunc.nvl(col.GetValue("$datatype"));
                    var precision = IntStd.IsNotIntThen(col.GetValue("$precision"), -1);
                    var scale = IntStd.IsNotIntThen(col.GetValue("$scale"), -1);
                    var default_value = ComFunc.nvl(col.GetValue("$default"));
                    var is_null = BoolStd.IsNotBoolThen(col.GetValue("$isnull"), true);

                    switch (alter_action)
                    {
                        case "rename":
                            if (new_name != "")
                                sql.AppendLine(alter_rename_template.Replace("#table#", table).Replace("#column_name#", column_name).Replace("#new_column_name#", new_name));
                            break;
                        case "add":
                            sql.AppendLine(alter_add_template
                                .Replace("#table#", table)
                                .Replace("#column_name#", column_name)
                                .Replace("#data_type#", datatype)
                                .Replace("#null#", is_null?"null":"not null")
                                .Replace("#default#", convertExpressDefault2DBDefault(default_value,datatype)));
                            break;
                        case "modify":
                            sql.AppendLine(alter_modify_template
                                .Replace("#table#", table)
                                .Replace("#column_name#", column_name)
                                .Replace("#data_type#", datatype)
                                .Replace("#null#", is_null ? "null" : "not null"));
                            break;
                        case "drop":
                            sql.AppendLine(alter_drop_template
                                .Replace("#table#", table)
                                .Replace("#column_name#", column_name));
                            break;
                        case "drop_add":
                            sql.AppendLine(alter_drop_add_template
                                .Replace("#table#", table)
                                .Replace("#column_name#", column_name)
                                .Replace("#data_type#", datatype)
                                .Replace("#null#", is_null ? "null" : "not null")
                                .Replace("#default#", convertExpressDefault2DBDefault(default_value, datatype)));
                            break;
                        default:
                            break;
                    }

                }
            }
            
           
            rtn.sql = sql;
            rtn.table = table;
            return rtn;
        }

        protected override dynamic ParseCreateTable(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            var table = "";
            var columns = "";
            var pk = "";
            var pk_template = @",CONSTRAINT [#pkname#] PRIMARY KEY CLUSTERED
        (#pkcolumns# ) WITH ( IGNORE_DUP_KEY = OFF ) ON [PRIMARY]";
            var sql = $@"CREATE TABLE #table#
    (
#columns#
#pk#
    )
ON  [PRIMARY]";
            foreach (var k in obj.Keys)
            {
                if (k.StartsWith("$"))
                {
                    if (k.ToLower() == "$table")
                    {
                        table = ComFunc.nvl(obj.GetValue(k));
                    }
                    else if (k.ToLower() == "$pk")
                    {
                        var pkobj = obj.GetValue(k);
                        var pkcolumns = "";
                        if (pkobj != null && pkobj is IEnumerable<object>)
                        {
                            var pklist = (IEnumerable<object>)pkobj;
                            foreach (var s in pklist)
                            {
                                pkcolumns += $"{s},";
                            }
                        }
                        if (pkcolumns != "")
                        {
                            pk = pk_template.Replace("#pkcolumns#", pkcolumns.Substring(0, pkcolumns.Length - 1));
                        }
                    }
                }
                else
                {
                    if (obj.GetValue(k) is FrameDLRObject)
                    {
                        columns += $@"{parseColumnExpress(k, (FrameDLRObject)obj.GetValue(k))},";
                    }
                    
                }
            }
            if (columns != "")
            {
                columns = columns.Substring(0, columns.Length - 1);
            }
            pk = pk.Replace("#pkname#", $"PK_{table}");
            sql = sql.Replace("#table#",table).Replace("#columns#", columns).Replace("#pk#", pk);
            rtn.sql = sql;
            rtn.table = table;
            return rtn;
        }

        private string parseColumnExpress(string columnname,FrameDLRObject column)
        {
            var rtn = "[#name#] #datatype#  #default# #isnull#";
            var datatype = "";
            var oridatatype = "";
            var precision = -1;
            var scale = -1;
            var defaultvalue = "";
            var nullexpress = "";
            foreach (var k in column.Keys)
            {
                if (k.StartsWith("$"))
                {
                    if (k.ToLower() == "$datatype")
                    {
                        oridatatype = ComFunc.nvl(column.GetValue(k));
                        datatype = convertExpressDataType2DBType(ComFunc.nvl(column.GetValue(k)));
                    }
                    else if (k.ToLower() == "$precision")
                    {
                        //file和picture类型默认给长度1000
                        if (ComFunc.nvl(column.GetValue("$datatype")).ToLower() == "file")
                        {
                            precision = 1000;
                        }
                        else if (ComFunc.nvl(column.GetValue("$datatype")).ToLower() == "picture")
                        {
                            precision = 1000;
                        }
                        else if (ComFunc.nvl(column.GetValue("$datatype")).ToLower() == "guid")
                        {
                            precision = 50;
                        }
                        else if (ComFunc.nvl(column.GetValue("$datatype")).ToLower() == "map_xy")
                        {
                            precision = 500;
                        }
                        else if (ComFunc.nvl(column.GetValue("$datatype")).ToLower() == "pic_xy")
                        {
                            precision = 500;
                        }
                        else
                        {
                            precision = IntStd.IsNotIntThen(column.GetValue(k), -1);
                        }
                    }
                    else if (k.ToLower() == "$scale")
                    {
                        scale = IntStd.IsNotIntThen(column.GetValue(k), -1);
                    }
                    else if (k.ToLower() == "$default")
                    {
                        defaultvalue = ComFunc.nvl(column.GetValue(k));
                    }
                    else if (k.ToLower() == "$isnull")
                    {
                        var isnull = true;
                        if (column.GetValue(k) is bool)
                        {
                            isnull = (bool)column.GetValue(k);
                        }
                        nullexpress = isnull ? "NULL" : "NOT NULL";
                    }
                }
            }
            defaultvalue = convertExpressDefault2DBDefault(defaultvalue,oridatatype);
            //datatype类型判断，添加precision和scale
            if (new string[] { "varchar", "nvarchar","guid", "random", "random_code","map_xy","pic_xy" }.Contains(oridatatype.ToLower()))
            {
                if (precision > -1)
                {
                    datatype = $"{datatype}({precision})";
                }
            }
            else if (new string[] { "numberic" }.Contains(oridatatype.ToLower()))
            {
                if (precision > -1)
                {
                    datatype = $"{datatype}({precision},{(scale > -1 ? scale : 0)})";
                }
            }
            else if (new string[] { "file", "picture" }.Contains(oridatatype.ToLower()))
            {
                if (precision > -1)
                {
                    datatype = $"{datatype}({precision})";
                }
            }
            rtn = rtn.Replace("#name#", columnname).Replace("#datatype#", datatype).Replace("#default#", defaultvalue).Replace("#isnull#", nullexpress);
            return rtn;
        }

        private string convertExpressDataType2DBType(string type)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("varchar", "varchar");
            dic.Add("nvarchar", "nvarchar");
            dic.Add("int", "int");
            dic.Add("numberic", "numeric");
            dic.Add("bit", "bit");
            dic.Add("datetime", "datetime");
            dic.Add("text", "text");
            dic.Add("file", "nvarchar");
            dic.Add("picture", "nvarchar");
            dic.Add("guid", "varchar");
            dic.Add("random", "nvarchar");
            dic.Add("random_code", "nvarchar");
            dic.Add("map_xy", "nvarchar");
            dic.Add("pic_xy", "nvarchar");
            dic.Add("json", "text");
            return dic.ContainsKey(type) ? dic[type] : "";
        }
        private string convertExpressDefault2DBDefault(string express,string oridatatype)
        {
            if (string.IsNullOrEmpty(express)) return "";

            if (express.ToLower() == "now()")
            {
                return "DEFAULT(getdate())";
            }else if(express.ToLower().StartsWith("increament("))
            {
                return $"IDENTITY(1,{express.ToLower().Replace("increament(","").Replace(")","")})";
            }
            else
            {
                if(new string[] { "varchar", "nvarchar" }.Contains(oridatatype.ToLower()))
                {
                    return $"DEFAULT('{express}')";
                }
                else
                {
                    return $"DEFAULT({express})";
                }
                
            }
        }
    }
}
