using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EFFC.Frame.Net.Unit.DB
{
    public abstract class SqlTypeDBExpress:DBExpress
    {
        /// <summary>
        /// SQL中的常用关键字，用于对使用这些特殊关键字做栏位名称的时候进行识别并做格式转化
        /// </summary>
        protected virtual List<string> SQLKeywords
        {
            get
            {
                return new List<string>()
                {
                    "desc",
                    "asc",
                    "dictinct",
                    "order",
                    "by",
                    "event",
                    "events",
                    "group",
                    "file",
                    "varchar",
                    "nvarchar",
                    "decimal",
                    //mysql中的关键字
                    "explain",
                    "memo"
                };
            }
        }
        /// <summary>
        /// 参数标识符
        /// </summary>
        protected abstract string ParameterFlag
        {
            get;
        }
        /// <summary>
        /// 字符串链接标识符号
        /// </summary>
        protected abstract string LinkFlag
        {
            get;
        }
        /// <summary>
        /// 栏位引用符号
        /// </summary>
        protected virtual string Column_Quatation
        {
            get { return "[{0}]"; }
        }
        //table别名列表
        protected List<string> alianeName = new List<string>();
        //变量编号列表
        protected Dictionary<string, int> _nodic = new Dictionary<string, int>();
        protected override FrameDLRObject ParseExpress(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            switch (CurrentAct)
            {
                case ActType.Query:
                    rtn = ParseQuery(obj);
                    break;
                case ActType.QueryByPage:
                    rtn = ParseQueryByPage(obj);
                    break;
                case ActType.Delete:
                    rtn = ParseDelete(obj);
                    break;
                case ActType.Insert:
                    rtn = ParseInsert(obj);
                    break;
                case ActType.Update:
                    rtn = ParseUpdate(obj);
                    break;
                case ActType.InsertSelect:
                    rtn = ParseInsertSelect(obj);
                    break;
                case ActType.CreateTable:
                    rtn = ParseCreateTable(obj);
                    break;
                case ActType.DropTable:
                    rtn = ParseDropTable(obj);
                    break;
                case ActType.AlterColumn:
                    rtn = ParseAlterColumn(obj);
                    break;
                case ActType.CopyTable:
                    rtn = ParseCopyTable(obj);
                    break;
                case ActType.CopyData:
                    rtn = ParseCopyData(obj);
                    break;
            }

            return rtn;
        }
        /// <summary>
        /// 转化CopyData指令
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual dynamic ParseCopyData(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.table = "";
            rtn.sql = "";
            return rtn;
        }
        /// <summary>
        /// 转化CopyTable指令
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual dynamic ParseCopyTable(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.table = "";
            rtn.sql = "";
            return rtn;
        }
        /// <summary>
        /// 转化DropTable指令
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual dynamic ParseDropTable(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            FrameDLRObject datacollection = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);

            var sql = "drop table #table#";
            var table = "";
            foreach (var k in obj.Keys)
            {
                if (k.StartsWith("$"))
                {
                    if (k.ToLower() == "$table")
                    {
                        table = ComFunc.nvl(obj.GetValue(k));
                    }
                }
            }
            sql = sql.Replace("#table#", table);

            rtn.table = table;
            rtn.sql = sql;
            return rtn;
        }
        /// <summary>
        /// 转化CreateTable指令
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual dynamic ParseCreateTable(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.table = "";
            rtn.sql = "";
            return rtn;
        }
        /// <summary>
        /// 转化AlterColumn指令
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual dynamic ParseAlterColumn(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.table = "";
            rtn.sql = "";
            return rtn;
        }

        protected virtual FrameDLRObject ParseQuery(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            FrameDLRObject datacollection = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            var sql = "";
            var cols = "";
            var where = "";
            var table = "";
            var orderby = "";
            var prefix = "";

            sql = @"select #prefix# #cols# #table# #where# #orderby# ";

            foreach (var k in obj.Keys)
            {
                if (k.StartsWith("$"))
                {
                    if (k.ToLower() == "$where")
                    {
                        where = SqlWhere((FrameDLRObject)obj.GetValue(k), datacollection);
                    }
                    else if (k.ToLower() == "$table")
                    {
                        table = Table(obj.GetValue(k), datacollection);
                    }
                    else if (k.ToLower() == "$orderby")
                    {
                        orderby = OrderBy(obj.GetValue(k));
                        if (this.CurrentAct != ActType.QueryByPage)
                        {
                            orderby = orderby.Length > 0 ? "order by " + orderby : orderby;
                        }
                    }
                    else if (k.ToLower() == "$prefix")
                    {
                        if (obj.GetValue(k) is string)
                        {
                            prefix = ComFunc.nvl(obj.GetValue(k));
                        }
                    }
                }
                else
                {
                    var v = obj.GetValue(k);
                    if (v is bool)
                    {
                        var isdislpay = (bool)v;
                        if (isdislpay)
                        {
                            cols += (cols.Length > 0 ? "," : "") + ColumnFormat(k);
                        }
                    }
                    else if (v is string)
                    {
                        if (ComFunc.nvl(v).StartsWith("#sql:"))
                        {
                            var asexpress = ComFunc.nvl(v).Replace("#sql:", "") + " as " + ColumnFormat(k);
                            cols += (cols.Length > 0 ? "," : "") + asexpress;
                        }
                        else
                        {
                            var pname = GetParameterSerialno("p");
                            cols += (cols.Length > 0 ? "," : "") + $"{ParameterFlag}{pname} as {ColumnFormat(k)}";
                            datacollection.SetValue(pname, v);
                        }
                    }
                    else
                    {
                        var pname = GetParameterSerialno("p");
                        cols += (cols.Length > 0 ? "," : "") + $"{ParameterFlag}{pname} as {ColumnFormat(k)}";
                        datacollection.SetValue(pname, v);
                    }
                    //暂时不提供datetime类型数据支持
                    //else if (v is DateTime)
                    //{
                    //    cols += (cols.Length > 0 ? "," : "") + "convert(datetime,'" + ((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss.fff") + "',121) as " + k;
                    //}
                }
            }

            cols = cols.Length > 0 ? cols : "*";




            where = where.Length > 0 ? "where" + where : "";
            if (table != "") table = $"from {table}";
            else table = IfTableEmptyThen4Query();
            sql = sql.Replace("#table#", table).Replace("#cols#", cols).Replace("#where#", where).Replace("#orderby#", orderby).Replace("#prefix#", prefix);

            rtn.table = table;
            rtn.sql = sql;
            rtn.data = datacollection;
            rtn.orderby = orderby;

            

            return rtn;
        }
        protected virtual FrameDLRObject ParseQueryByPage(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            var datacollection = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            var sql = "";
            var cols = "";
            var where = "";
            var table = "";
            var orderby = "";
            var prefix = "";
            sql = @"select #prefix# #cols# #table# #where# ";


            foreach (var k in obj.Keys)
            {
                if (k.StartsWith("$"))
                {
                    if (k.ToLower() == "$where")
                    {
                        where = SqlWhere((FrameDLRObject)obj.GetValue(k), datacollection);
                    }
                    else if (k.ToLower() == "$table")
                    {
                        table = Table(obj.GetValue(k), datacollection);
                    }
                    else if (k.ToLower() == "$orderby")
                    {
                        orderby = OrderBy(obj.GetValue(k));
                        if (this.CurrentAct != ActType.QueryByPage)
                        {
                            orderby = orderby.Length > 0 ? "order by " + orderby : orderby;
                        }
                    }
                    else if (k.ToLower() == "$prefix")
                    {
                        if (obj.GetValue(k) is string)
                        {
                            prefix = ComFunc.nvl(obj.GetValue(k));
                        }
                    }
                }
                else
                {
                    var v = obj.GetValue(k);

                    if (v is bool)
                    {
                        var isdislpay = (bool)v;
                        if (isdislpay)
                        {
                            cols += (cols.Length > 0 ? "," : "") + ColumnFormat(k);
                        }
                    }
                    else if (v is string)
                    {
                        if (ComFunc.nvl(v).StartsWith("#sql:"))
                        {
                            var asexpress = ComFunc.nvl(v).Replace("#sql:", "") + " as " + ColumnFormat(k);
                            cols += (cols.Length > 0 ? "," : "") + asexpress;
                        }
                        else
                        {
                            var pname = GetParameterSerialno("p");
                            cols += (cols.Length > 0 ? "," : "") + $"{ParameterFlag}{pname} as {ColumnFormat(k)}";
                            datacollection.SetValue(pname, v);
                        }
                    }
                    else
                    {
                        var pname = GetParameterSerialno("p");
                        cols += (cols.Length > 0 ? "," : "") + $"{ParameterFlag}{pname} as {ColumnFormat(k)}";
                        datacollection.SetValue(pname, v);
                    }
                }
            }

            cols = cols.Length > 0 ? cols : "*";



            where = where.Length > 0 ? "where" + where : "";
            if (table != "") table = $"from {table}";
            sql = sql.Replace("#table#", table).Replace("#cols#", cols).Replace("#where#", where).Replace("#prefix#", prefix);

            rtn.table = table;
            rtn.sql = sql;
            rtn.data = datacollection;
            rtn.orderby = orderby;

            return rtn;
        }
        protected virtual FrameDLRObject ParseInsert(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            var datacollection = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            var sql = "";
            var cols = "";
            var where = "";
            var table = "";
            var orderby = "";
            var prefix = "";

            sql = @"insert into #table##cols# ";

            foreach (var k in obj.Keys)
            {
                if (k.StartsWith("$"))
                {
                    if (k.ToLower() == "$where")
                    {
                        where = SqlWhere((FrameDLRObject)obj.GetValue(k), datacollection);
                    }
                    else if (k.ToLower() == "$table")
                    {
                        table = Table(obj.GetValue(k), datacollection);
                    }
                    else if (k.ToLower() == "$orderby")
                    {
                        orderby = OrderBy(obj.GetValue(k));
                        if (this.CurrentAct != ActType.QueryByPage)
                        {
                            orderby = orderby.Length > 0 ? "order by " + orderby : orderby;
                        }
                    }
                    else if (k.ToLower() == "$prefix")
                    {
                        if (obj.GetValue(k) is string)
                        {
                            prefix = ComFunc.nvl(obj.GetValue(k));
                        }
                    }
                }
                else
                {
                    var v = obj.GetValue(k);

                    var colstr = "{0}#nextcol#";
                    var valuesstr = "{0}#nextvlaue#";

                    var pkey = GetParameterSerialno(k);
                    var realdata = ConvertObject(v);
                    if (realdata.DataType == DataType.Value)
                    {
                        datacollection.SetValue(pkey, realdata.Content);

                        if (cols == "")
                        {
                            cols = string.Format("({0})values({1})", string.Format(colstr, $"{ColumnFormat(k)}"), string.Format(valuesstr, ParameterFlag + pkey));
                        }
                        else
                        {
                            cols = cols.Replace("#nextcol#", "," + string.Format(colstr, $"{ColumnFormat(k)}")).Replace("#nextvlaue#", "," + string.Format(valuesstr, ParameterFlag + pkey));
                        }
                    }
                    else
                    {
                        if (cols == "")
                        {
                            cols = string.Format("({0})values({1})", string.Format(colstr, $"{ColumnFormat(k)}"), string.Format(valuesstr, realdata.Content));
                        }
                        else
                        {
                            cols = cols.Replace("#nextcol#", "," + string.Format(colstr, $"{ColumnFormat(k)}")).Replace("#nextvlaue#", "," + string.Format(valuesstr, realdata.Content));
                        }
                    }
                }
            }

            cols = cols.Replace("#nextcol#", "").Replace("#nextvlaue#", "");
            where = where.Length > 0 ? "where" + where : "";
            sql = sql.Replace("#table#", table).Replace("#cols#", cols).Replace("#where#", where).Replace("#orderby#", orderby).Replace("#prefix#", prefix);

            rtn.table = table;
            rtn.sql = sql;
            rtn.data = datacollection;
            rtn.orderby = orderby;
            return rtn;
        }
        protected virtual FrameDLRObject ParseUpdate(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            var datacollection = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            var sql = "";
            var cols = "";
            var where = "";
            var table = "";
            var orderby = "";
            var prefix = "";

            sql = @"update #table# 
    set #cols# 
    #where# ";

            foreach (var k in obj.Keys)
            {
                if (k.StartsWith("$"))
                {
                    if (k.ToLower() == "$where")
                    {
                        where = SqlWhere((FrameDLRObject)obj.GetValue(k), datacollection);
                    }
                    else if (k.ToLower() == "$table")
                    {
                        table = Table(obj.GetValue(k), datacollection);
                    }
                    else if (k.ToLower() == "$orderby")
                    {
                        orderby = OrderBy(obj.GetValue(k));
                        if (this.CurrentAct != ActType.QueryByPage)
                        {
                            orderby = orderby.Length > 0 ? "order by " + orderby : orderby;
                        }
                    }
                    else if (k.ToLower() == "$prefix")
                    {
                        if (obj.GetValue(k) is string)
                        {
                            prefix = ComFunc.nvl(obj.GetValue(k));
                        }
                    }
                }
                else
                {
                    var v = obj.GetValue(k);

                    var pkey = GetParameterSerialno(k);
                    var realdata = ConvertObject(v);
                    var fstr = "{0}={1}";
                    if (realdata.DataType == DataType.Value)
                    {
                        datacollection.SetValue(pkey, realdata.Content);
                        if (cols == "")
                        {
                            cols = string.Format(fstr, $"{ColumnFormat(k)}", ParameterFlag + pkey);
                        }
                        else
                        {
                            cols += "," + string.Format(fstr, $"{ColumnFormat(k)}", ParameterFlag + pkey);
                        }
                    }
                    else
                    {
                        if (cols == "")
                        {
                            cols = string.Format(fstr, $"{ColumnFormat(k)}", realdata.Content);
                        }
                        else
                        {
                            cols += "," + string.Format(fstr, $"{ColumnFormat(k)}", realdata.Content);
                        }
                    }
                }
            }

            where = where.Length > 0 ? "where" + where : "";
            sql = sql.Replace("#table#", table).Replace("#cols#", cols).Replace("#where#", where).Replace("#orderby#", orderby).Replace("#prefix#", prefix);

            rtn.table = table;
            rtn.sql = sql;
            rtn.data = datacollection;
            rtn.orderby = orderby;
            return rtn;
        }
        protected virtual FrameDLRObject ParseDelete(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            var datacollection = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            var sql = "";
            var cols = "";
            var where = "";
            var table = "";
            var orderby = "";
            var prefix = "";

            sql = @"delete 
    from #table# 
    #where# ";

            foreach (var k in obj.Keys)
            {
                if (k.StartsWith("$"))
                {
                    if (k.ToLower() == "$where")
                    {
                        where = SqlWhere((FrameDLRObject)obj.GetValue(k), datacollection);
                    }
                    else if (k.ToLower() == "$table")
                    {
                        table = Table(obj.GetValue(k), datacollection);
                    }
                    else if (k.ToLower() == "$orderby")
                    {
                        orderby = OrderBy(obj.GetValue(k));
                        if (this.CurrentAct != ActType.QueryByPage)
                        {
                            orderby = orderby.Length > 0 ? "order by " + orderby : orderby;
                        }
                    }
                    else if (k.ToLower() == "$prefix")
                    {
                        if (obj.GetValue(k) is string)
                        {
                            prefix = ComFunc.nvl(obj.GetValue(k));
                        }
                    }
                }
            }


            where = where.Length > 0 ? "where" + where : "";
            sql = sql.Replace("#table#", table).Replace("#cols#", cols).Replace("#where#", where).Replace("#orderby#", orderby).Replace("#prefix#", prefix);

            rtn.table = table;
            rtn.sql = sql;
            rtn.data = datacollection;
            rtn.orderby = orderby;
            return rtn;
        }
        protected virtual FrameDLRObject ParseInsertSelect(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            var datacollection = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            var sql = "";
            var cols = "";
            var where = "";
            var table = "";
            var orderby = "";
            var prefix = "";

            sql = @"insert into #table##cols# #select#";
            var selectexpress = FrameDLRObject.CreateInstance();
            foreach (var k in obj.Keys)
            {
                if (k.StartsWith("$"))
                {
                    if(k.ToLower() == "$select")
                    {
                        selectexpress = ParseQuery((FrameDLRObject)obj.GetValue(k));
                    }
                    else if (k.ToLower() == "$table")
                    {
                        table = Table(obj.GetValue(k), datacollection);
                    }
                }
                else
                {
                    cols += $",{ColumnFormat(k)}";
                }
            }

            cols = cols == ""?"":$"({cols.Substring(1)})";
            where = where.Length > 0 ? "where" + where : "";
            sql = sql.Replace("#table#", table).Replace("#cols#", cols).Replace("#where#", where).Replace("#orderby#", orderby).Replace("#prefix#", prefix);
            if(selectexpress!= null && selectexpress.sql != null)
            {
                sql = sql.Replace("#select#", selectexpress.sql);
                FrameDLRObject sc = selectexpress.data;
                foreach(var key in sc.Keys)
                {
                    datacollection.SetValue(key, sc.GetValue(key));
                }
            }
            else
            {
                sql = sql.Replace("#select#", "");
            }
            rtn.table = table;
            rtn.sql = sql;
            rtn.data = datacollection;
            rtn.orderby = orderby;
            return rtn;
        }
        /// <summary>
        /// 查询表达式中，如果$table为空或没有设定的时候，from表达式的生成逻辑
        /// </summary>
        /// <returns></returns>
        protected virtual string IfTableEmptyThen4Query()
        {
            return "";
        }
        #region Table
        protected string Table(object obj, FrameDLRObject data)
        {
            var rtn = "";
            if (CurrentAct != ActType.Query && CurrentAct != ActType.QueryByPage)
            {
                if (obj is string)
                {
                    rtn = ComFunc.nvl(obj);
                }
            }
            else
            {
                if (obj is string)
                {
                    rtn = ComFunc.nvl(obj);
                }
                else if (obj is FrameDLRObject)
                {
                    var dobj = (FrameDLRObject)obj;
                    foreach (var k in dobj.Keys)
                    {
                        //去掉同名指代标记~
                        var tablename = k.IndexOf("~") > 0 ? k.Split('~')[0] : k;
                        var joinstr = Table(tablename, dobj.GetValue(k), data);
                        if (joinstr.StartsWith("join")
                            || joinstr.StartsWith("left join")
                            || joinstr.StartsWith("right join"))
                        {
                            rtn += (rtn.Length > 0 ? " " : "") + joinstr;
                        }
                        else
                        {
                            rtn += (rtn.Length > 0 ? "," : "") + joinstr;
                        }
                    }
                }
            }
            return rtn;
        }
        protected string Table(string key, object obj, FrameDLRObject data)
        {
            var rtn = "";
            var join = "#join# #tablename# #on#";
            var asname = "";
            if (!key.StartsWith("$"))
            {
                if (obj is string)
                {
                    rtn += (rtn.Length > 0 ? " " : "") + key + " " + ComFunc.nvl(obj);
                    if (!alianeName.Contains(ComFunc.nvl(obj) + "."))
                    {
                        alianeName.Add(ComFunc.nvl(obj) + ".");
                    }
                }
                else if (obj is FrameDLRObject)
                {
                    var dobj = (FrameDLRObject)obj;
                    //直接抓取as表达式
                    asname = ComFunc.nvl(dobj.GetValue("$as"));
                    join = join.Replace("#tablename#", key + " " + ComFunc.nvl(dobj.GetValue("$as")));
                    if (!alianeName.Contains(asname + "."))
                    {
                        alianeName.Add(asname + ".");
                    }
                    foreach (var k in dobj.Keys)
                    {
                        if (k.StartsWith("$") && k.ToLower().IndexOf("join") > 0)
                        {
                            //var by = "";
                            if (k.ToLower() == "$join")
                            {
                                join = join.Replace("#join#", "join");
                            }
                            else if (k.ToLower() == "$joinl")
                            {
                                join = join.Replace("#join#", "left join");
                            }
                            else if (k.ToLower() == "$joinr")
                            {
                                join = join.Replace("#join#", "right join");
                            }
                            if (dobj.GetValue(k) is FrameDLRObject)
                            {
                                var joinobj = (FrameDLRObject)dobj.GetValue(k);
                                //找出join表的别名
                                //$by不再使用
                                //foreach (var jkey in joinobj.Keys)
                                //{
                                //    if (jkey.ToLower() == "$by")
                                //    {
                                //        if (joinobj.GetValue(jkey) is string)
                                //        {
                                //            by = ComFunc.nvl(joinobj.GetValue(jkey));
                                //        }
                                //        break;
                                //    }
                                //}
                                foreach (var jkey in joinobj.Keys)
                                {
                                    if (jkey.ToLower() == "$on")
                                    {
                                        var onstr = "";
                                        if (joinobj.GetValue(jkey) is FrameDLRObject)
                                        {
                                            var onobj = (FrameDLRObject)joinobj.GetValue(jkey);
                                            onstr = SqlWhere(onobj, data);
                                        }
                                        join = join.Replace("#on#", onstr.Length > 0 ? $"on {asname}.{ComFunc.nvl(onstr)}" : onstr);
                                    }
                                }
                            }
                        }
                    }
                    rtn = join;
                }
            }
            return rtn;
        }
        #endregion
        protected string Columns(string key, object obj, FrameDLRObject data)
        {
            var rtn = "";
            if (CurrentAct == ActType.Query)
            {
                if (obj is bool)
                {
                    var isdislpay = (bool)obj;
                    if (isdislpay)
                    {
                        rtn = key;
                    }
                }
            }
            else if (CurrentAct == ActType.Insert)
            {

            }

            return rtn;
        }

        protected string OrderBy(object obj)
        {
            var rtn = "";
            if (obj is string)
            {
                rtn = ComFunc.nvl(obj);
            }
            else if (obj is FrameDLRObject)
            {
                var dobj = (FrameDLRObject)obj;
                foreach (var k in dobj.Keys)
                {
                    if (!k.StartsWith("$"))
                    {
                        var v = dobj.GetValue(k);
                        if (v is string)
                        {
                            rtn += (rtn.Length > 0 ? "," : "") + k + " " + ComFunc.nvl(v);
                        }
                    }
                }
            }

            return rtn;
        }
        #region Where
        /// <summary>
        /// 条件表达式关系解析
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual string SqlWhere(FrameDLRObject obj, FrameDLRObject data)
        {
            var rtn = "";
            foreach (var k in obj.Keys)
            {
                var v = obj.GetValue(k);
                if (k.StartsWith("$"))
                {
                    switch (k.ToLower())
                    {
                        case "$or":
                            if (v is object[])
                            {
                                var arr = (object[])v;
                                var orstr = "";
                                foreach (var item in arr)
                                {
                                    if (item is FrameDLRObject)
                                    {
                                        var condition = SqlWhere((FrameDLRObject)item, data);
                                        if (condition.Length > 0)
                                        {
                                            orstr += (orstr.Length > 0 ? " or " : "") + "(" + condition + ")";
                                        }
                                    }
                                }
                                orstr = orstr.Trim().Length > 0 ? "(" + orstr + ")" : "";
                                if (orstr.Length > 0)
                                    rtn += (rtn.Length > 0 ? " and " : "") + orstr;
                            }
                            break;
                        case "$and":
                            if (v is object[])
                            {
                                var arr = (object[])v;
                                var orstr = "";
                                foreach (var item in arr)
                                {
                                    if (item is FrameDLRObject)
                                    {
                                        orstr += (orstr.Length > 0 ? " or " : "") + SqlWhere((FrameDLRObject)item, data);
                                    }
                                }
                                orstr = orstr.Trim().Length > 0 ? "(" + orstr + ")" : "";
                                rtn += (rtn.Length > 0 ? " and " : "") + orstr;
                            }
                            break;
                        case "$exists":
                            if(v is FrameDLRObject)
                            {
                                var selectexpress = ParseQuery((FrameDLRObject)v);
                                rtn += (rtn.Length > 0 ? " and " : "") + $" exists({selectexpress.GetValue("sql")})";
                                var dp = (FrameDLRObject)selectexpress.GetValue("data");
                                foreach(var key in dp.Keys)
                                {
                                    data.SetValue(key, dp.GetValue(key));
                                }
                            }
                            break;
                        case "$notexists":
                            if (v is FrameDLRObject)
                            {
                                var selectexpress = ParseQuery((FrameDLRObject)v);
                                rtn += (rtn.Length > 0 ? " and " : "") + $" not exists({selectexpress.GetValue("sql")})";
                                var dp = (FrameDLRObject)selectexpress.GetValue("data");
                                foreach (var key in dp.Keys)
                                {
                                    data.SetValue(key, dp.GetValue(key));
                                }
                            }
                            break;
                    }
                }
                else
                {
                    var re = SqlWhere(k, v, data);
                    if (re.Trim().Length > 0)
                    {
                        rtn += (rtn.Length > 0 ? " and " : "") + re;
                    }
                }
            }
            return rtn;
        }
        /// <summary>
        /// 条件表达式操作符解析
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual string SqlWhere(string key, object obj, FrameDLRObject data)
        {

            var rtn = "";
            if (!key.StartsWith("$"))
            {
                if (obj is FrameDLRObject)
                {
                    var dobj = (FrameDLRObject)obj;
                    foreach (var k in dobj.Keys)
                    {
                        var v = dobj.GetValue(k);
                        switch (k.ToLower())
                        {
                            case "$eq":
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "=" + ParameterFlag + pkey;
                                    }
                                    else
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "=" + realdata.Content;
                                    }
                                }
                                break;
                            case "$neq":
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "<>" + ParameterFlag + pkey;
                                    }
                                    else
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "<>" + realdata.Content;
                                    }
                                }
                                break;
                            case "$lt":
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "<" + ParameterFlag + pkey;
                                    }
                                    else
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "<" + realdata.Content;
                                    }
                                }
                                break;
                            case "$gt":
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + ">" + ParameterFlag + pkey;
                                    }
                                    else
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + ">" + realdata.Content;
                                    }
                                }
                                break;
                            case "$lte":
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "<=" + ParameterFlag + pkey;
                                    }
                                    else
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "<=" + realdata.Content;
                                    }
                                }
                                break;
                            case "$gte":
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + ">=" + ParameterFlag + pkey;
                                    }
                                    else
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + ">=" + realdata.Content;
                                    }
                                }
                                break;
                            case "$like":
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + " like '%'" + LinkFlag + ParameterFlag + pkey + LinkFlag + "'%'";
                                    }
                                }
                                break;
                            case "$likel":
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + " like ''" + LinkFlag + ParameterFlag + pkey + LinkFlag + "'%'";
                                    }
                                }
                                break;
                            case "$liker":
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + " like '%'" + LinkFlag + ParameterFlag + pkey + LinkFlag + "''";
                                    }
                                }
                                break;
                            case "$in":
                                if (v is object[])
                                {
                                    var arr = (object[])v;
                                    var pkey = GetParameterSerialno(key);
                                    var index = 0;
                                    var instr = "";
                                    foreach (var item in arr)
                                    {
                                        if (item is string || item is double || item is int || item is long || item is decimal || item is float)
                                        {
                                            var itemkey = pkey + "_in_" + index;
                                            data.SetValue(itemkey, item);
                                            instr += (instr.Length > 0 ? "," : "") + ParameterFlag + itemkey;
                                            index++;
                                        }
                                    }
                                    if (instr.Length > 0)
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + " in (" + instr + ")";
                                    }
                                }
                                else if (v is string)
                                {
                                    if (ComFunc.nvl(v).StartsWith("#sql:"))
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + " in (" + ComFunc.nvl(v).Replace("#sql:", "") + ")";
                                    }
                                }
                                break;
                            case "$nin":
                                if (v is object[])
                                {
                                    var arr = (object[])v;
                                    var pkey = GetParameterSerialno(key);
                                    var index = 0;
                                    var instr = "";
                                    foreach (var item in arr)
                                    {
                                        if (item is string || item is double || item is int || item is long || item is decimal || item is float)
                                        {
                                            var itemkey = pkey + "_in_" + index;
                                            data.SetValue(itemkey, item);
                                            instr = (instr.Length > 0 ? "," : "") + ParameterFlag + itemkey;
                                        }
                                    }
                                    if (instr.Length > 0)
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + " not in (" + instr + ")";
                                    }
                                }
                                else if (v is string)
                                {
                                    if (ComFunc.nvl(v).StartsWith("#sql:"))
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + " not in (" + ComFunc.nvl(v).Replace("#sql:", "") + ")";
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    if (obj != null)
                    {
                        var pkey = GetParameterSerialno(key);
                        var realdata = ConvertObject(obj);

                        if (realdata.DataType == DataType.Value)
                        {
                            if (obj is string)
                            {
                                //右方有[别名.]开头的需要特别处理
                                var sarr = ComFunc.nvl(obj).Split('.');
                                if (sarr.Length == 2)
                                {
                                    var asname = sarr[0];
                                    if (alianeName.Contains(asname + "."))
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "=" + ComFunc.nvl(obj);
                                    }
                                    else
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "=" + ParameterFlag + pkey;
                                    }
                                }
                                else
                                {
                                    data.SetValue(pkey, realdata.Content);
                                    rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "=" + ParameterFlag + pkey;
                                }

                            }
                            else
                            {
                                data.SetValue(pkey, realdata.Content);
                                rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "=" + ParameterFlag + pkey;
                            }
                        }
                        else
                        {
                            rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + "=" + realdata.Content;
                        }
                    }
                    else
                    {
                        rtn += (rtn.Length > 0 ? " and " : " ") + ColumnFormat(key) + " is null";
                    }
                }
            }
            return rtn;
        }
        #endregion
        /// <summary>
        /// 获取参数名称
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetParameterSerialno(string key)
        {
            var tkey = ComFunc.RandomCode(6);
            var rtn = "";
            if (_nodic.ContainsKey(tkey))
            {
                rtn = tkey + (_nodic[tkey] + 1);
                _nodic[tkey] = _nodic[tkey] + 1;
            }
            else
            {
                rtn = tkey;
                _nodic.Add(tkey, 0);
            }
            return rtn;
        }
        /// <summary>
        /// 栏位的名称格式化，主要针对特殊名称，如desc,asc之类的sql关键字做转化
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected string ColumnFormat(string str)
        {

            if (SQLKeywords.Contains(ComFunc.nvl(str).ToLower()))
            {
                return string.Format(Column_Quatation,str);
            }
            else
            {
                return str;
            }
        }
        /// <summary>
        /// 将json中的特殊数据做转化
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected ConvertData ConvertObject(object v)
        {
            var rtn = new ConvertData();
            if (v is string)
            {
                if (ComFunc.nvl(v).StartsWith("#2:"))
                {
                    var bstr = ComFunc.Base64DeCode(ComFunc.nvl(v));
                    var bytes = Encoding.UTF8.GetBytes(bstr);
                    MemoryStream ms = new MemoryStream(bytes);
                    rtn.Content = ms;
                }
                else if (ComFunc.nvl(v).StartsWith("#sql:"))
                {
                    rtn.DataType = DataType.Express;
                    rtn.Content = ComFunc.nvl(v).Replace("#sql:", "");
                }
                else
                {
                    rtn.Content = v;
                }
            }
            else
            {
                rtn.Content = v;
            }
            return rtn;
        }
        protected enum DataType
        {
            Value,
            Express
        }
        protected sealed class ConvertData
        {
            public ConvertData()
            {
                DataType = SqlTypeDBExpress.DataType.Value;
            }
            public DataType DataType
            {
                get;
                set;
            }

            public object Content
            {
                get;
                set;
            }
        }
    }
}
