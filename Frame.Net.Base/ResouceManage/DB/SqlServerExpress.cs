using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.ResouceManage.DB
{
    public class SqlServerExpress : DBExpress
    {
        const string pflag = "@";
        const string linkflag = "+";
        //table别名列表
        List<string> alianeName = new List<string>();
        //变量编号列表
        Dictionary<string, int> _nodic = new Dictionary<string, int>();
        protected override FrameDLRObject ParseExpress(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            var datacollection = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            var sql = "";
            var cols = "";
            var where = "";
            var table = "";
            var orderby = "";
            var prefix = "";
            switch (CurrentAct)
            {
                case ActType.Query:
                    sql = @"select #prefix#
    #cols# 
    from #table# 
    #where# 
    #orderby# ";
                    break;
                case ActType.QueryByPage:
                    sql = @"select #prefix#
    #cols# 
    from #table# 
    #where# ";
                    break;
                case ActType.Delete:
                    sql = @"delete 
    from #table# 
    #where# ";
                    break;
                case ActType.Insert:
                    sql = @"insert into #table##cols# ";
                    break;
                case ActType.Update:
                    sql = @"update #table# 
    set #cols# 
    #where# ";
                    break;
            }
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
                    //$distinct由$prefix替代，不再使用 deleted by chuan.yin in 2015/11/11
                    //else if (k.ToLower() == "$distinct")
                    //{
                    //    if (obj.GetValue(k) is bool
                    //        && (bool)obj.GetValue(k))
                    //    {
                    //        prifix = "distinct";
                    //    }
                    //}
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
                    if (this.CurrentAct == ActType.Query || this.CurrentAct == ActType.QueryByPage)
                    {
                        if (v is bool)
                        {
                            var isdislpay = (bool)v;
                            if (isdislpay)
                            {
                                cols += (cols.Length > 0 ? "," : "") + k;
                            }
                        }
                        else if (v is string)
                        {
                            if (ComFunc.nvl(v).StartsWith("#sql:"))
                            {
                                var asexpress = ComFunc.nvl(v).Replace("#sql:", "") + " as " + k;
                                cols += (cols.Length > 0 ? "," : "") + asexpress;
                            }
                            else
                            {
                                cols += (cols.Length > 0 ? "," : "") + "'" + v + "' as " + k;
                            }
                        }
                        else if (v is int || v is double)
                        {
                            cols += (cols.Length > 0 ? "," : "") + "" + v + " as " + k;
                        }
                        else if (v is DateTime)
                        {
                            cols += (cols.Length > 0 ? "," : "") + "convert(datetime,'" + ((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss.fff") + "',121) as " + k;
                        }
                    }
                    else if (CurrentAct == ActType.Insert)
                    {
                        var colstr = "{0}#nextcol#";
                        var valuesstr = "{0}#nextvlaue#";
                        if (v is string || v is int || v is double || v is DateTime || v is bool)
                        {
                            var pkey = GetParameterSerialno(k);
                            var realdata = ConvertObject(v);
                            if (realdata.DataType == DataType.Value)
                            {
                                datacollection.SetValue(pkey, realdata.Content);

                                if (cols == "")
                                {
                                    cols = string.Format("({0})values({1})", string.Format(colstr, k), string.Format(valuesstr, pflag + pkey));
                                }
                                else
                                {
                                    cols = cols.Replace("#nextcol#", "," + string.Format(colstr, k)).Replace("#nextvlaue#", "," + string.Format(valuesstr, pflag + pkey));
                                }
                            }
                            else
                            {
                                if (cols == "")
                                {
                                    cols = string.Format("({0})values({1})", string.Format(colstr, k), string.Format(valuesstr, realdata.Content));
                                }
                                else
                                {
                                    cols = cols.Replace("#nextcol#", "," + string.Format(colstr, k)).Replace("#nextvlaue#", "," + string.Format(valuesstr, realdata.Content));
                                }
                            }
                        }
                    }
                    else if (CurrentAct == ActType.Update)
                    {
                        if (v is string || v is int || v is double || v is DateTime || v is bool)
                        {
                            var pkey = GetParameterSerialno(k);
                            var realdata = ConvertObject(v);
                            var fstr = "{0}={1}";
                            if (realdata.DataType == DataType.Value)
                            {
                                datacollection.SetValue(pkey, realdata.Content);
                                if (cols == "")
                                {
                                    cols = string.Format(fstr, k, pflag + pkey);
                                }
                                else
                                {
                                    cols += "," + string.Format(fstr, k, pflag + pkey);
                                }
                            }
                            else
                            {
                                if (cols == "")
                                {
                                    cols = string.Format(fstr, k, realdata.Content);
                                }
                                else
                                {
                                    cols += "," + string.Format(fstr, k, realdata.Content);
                                }
                            }
                        }
                    }
                }
            }

            if (CurrentAct == ActType.Query)
            {
                cols = cols.Length > 0 ? cols : "*";
            }
            else if (CurrentAct == ActType.Insert)
            {
                cols = cols.Replace("#nextcol#", "").Replace("#nextvlaue#", "");
            }



            where = where.Length > 0 ? "where" + where : "";
            if (CurrentAct != ActType.QueryByPage)
            {
                sql = sql.Replace("#table#", table).Replace("#cols#", cols).Replace("#where#", where).Replace("#orderby#", orderby).Replace("#prefix#", prefix);
            }
            else
            {
                sql = sql.Replace("#table#", table).Replace("#cols#", cols).Replace("#where#", where).Replace("#prefix#", prefix);
            }

            rtn.table = table;
            rtn.sql = sql;
            rtn.data = datacollection;
            rtn.orderby = orderby;
            return rtn;
        }
        #region Table
        string Table(object obj, FrameDLRObject data)
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
                        var joinstr = Table(k, dobj.GetValue(k), data);
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
        string Table(string key, object obj, FrameDLRObject data)
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
                    foreach (var k in dobj.Keys)
                    {
                        if (k.ToLower() == "$as")
                        {
                            asname = ComFunc.nvl(dobj.GetValue(k));
                            join = join.Replace("#tablename#", key + " " + ComFunc.nvl(dobj.GetValue(k)));
                            if (!alianeName.Contains(asname + "."))
                            {
                                alianeName.Add(asname + ".");
                            }
                        }
                        else if (k.StartsWith("$") && k.ToLower().IndexOf("join") > 0)
                        {
                            var by = "";
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
                                foreach (var jkey in joinobj.Keys)
                                {
                                    if (jkey.ToLower() == "$by")
                                    {
                                        if (joinobj.GetValue(jkey) is string)
                                        {
                                            by = ComFunc.nvl(joinobj.GetValue(jkey));
                                        }
                                        break;
                                    }
                                }
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
                                        join = join.Replace("#on#", onstr.Length > 0 ? "on " + onstr : onstr);
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
        string Columns(string key, object obj, FrameDLRObject data)
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

        string OrderBy(object obj)
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
        string SqlWhere(FrameDLRObject obj, FrameDLRObject data)
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

        string SqlWhere(string key, object obj, FrameDLRObject data)
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
                                if (v is string || v is double || v is int || v is DateTime || v is bool)
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + "=" + pflag + pkey;
                                    }
                                    else
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + "=" + realdata.Content;
                                    }
                                }
                                break;
                            case "$neq":
                                if (v is string || v is double || v is int || v is DateTime || v is bool)
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + "<>" + pflag + pkey;
                                    }
                                    else
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + "<>" + realdata.Content;
                                    }
                                }
                                break;
                            case "$lt":
                                if (v is string || v is double || v is int || v is DateTime)
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + "<" + pflag + pkey;
                                    }
                                    else
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + "<" + realdata.Content;
                                    }
                                }
                                break;
                            case "$gt":
                                if (v is string || v is double || v is int || v is DateTime)
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + ">" + pflag + pkey;
                                    }
                                    else
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + ">" + realdata.Content;
                                    }
                                }
                                break;
                            case "$lte":
                                if (v is string || v is double || v is int || v is DateTime)
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + "<=" + pflag + pkey;
                                    }
                                    else
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + "<=" + realdata.Content;
                                    }
                                }
                                break;
                            case "$gte":
                                if (v is string || v is double || v is int || v is DateTime)
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + ">=" + pflag + pkey;
                                    }
                                    else
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + ">=" + realdata.Content;
                                    }
                                }
                                break;
                            case "$like":
                                if (v is string || v is double || v is int || v is DateTime)
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + " like '%'" + linkflag + pflag + pkey + linkflag + "'%'";
                                    }
                                }
                                break;
                            case "$likel":
                                if (v is string || v is double || v is int || v is DateTime)
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + " like ''" + linkflag + pflag + pkey + linkflag + "'%'";
                                    }
                                }
                                break;
                            case "$liker":
                                if (v is string || v is double || v is int || v is DateTime)
                                {
                                    var pkey = GetParameterSerialno(key);
                                    var realdata = ConvertObject(v);
                                    if (realdata.DataType == DataType.Value)
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + " like '%'" + linkflag + pflag + pkey + linkflag + "''";
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
                                        if (item is string || item is int || item is double)
                                        {
                                            var itemkey = pkey + "_in_" + index;
                                            data.SetValue(itemkey, item);
                                            instr += (instr.Length > 0 ? "," : "") + pflag + itemkey;
                                            index++;
                                        }
                                    }
                                    if (instr.Length > 0)
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + " in (" + instr + ")";
                                    }
                                }
                                else if (v is string)
                                {
                                    if (ComFunc.nvl(v).StartsWith("#sql:"))
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + " in (" + ComFunc.nvl(v).Replace("#sql:", "") + ")";
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
                                        if (item is string || item is int || item is double)
                                        {
                                            var itemkey = pkey + "_in_" + index;
                                            data.SetValue(itemkey, item);
                                            instr = (instr.Length > 0 ? "," : "") + pflag + itemkey;
                                        }
                                    }
                                    if (instr.Length > 0)
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + " not in (" + instr + ")";
                                    }
                                }
                                else if (v is string)
                                {
                                    if (ComFunc.nvl(v).StartsWith("#sql:"))
                                    {
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + " not in (" + ComFunc.nvl(v).Replace("#sql:", "") + ")";
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    if (obj is string || obj is double || obj is int || obj is DateTime || obj is bool)
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
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + "=" + ComFunc.nvl(obj);
                                    }
                                    else
                                    {
                                        data.SetValue(pkey, realdata.Content);
                                        rtn += (rtn.Length > 0 ? " and " : " ") + key + "=" + pflag + pkey;
                                    }
                                }
                                else
                                {
                                    data.SetValue(pkey, realdata.Content);
                                    rtn += (rtn.Length > 0 ? " and " : " ") + key + "=" + pflag + pkey;
                                }

                            }
                            else
                            {
                                data.SetValue(pkey, realdata.Content);
                                rtn += (rtn.Length > 0 ? " and " : " ") + key + "=" + pflag + pkey;
                            }
                        }
                        else
                        {
                            rtn += (rtn.Length > 0 ? " and " : " ") + key + "=" + realdata.Content;
                        }
                    }
                    else if (obj == null)
                    {
                        rtn += (rtn.Length > 0 ? " and " : " ") + key + " is null";
                    }
                    else
                    {

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
        string GetParameterSerialno(string key)
        {
            var tkey = key.Replace("(", "").Replace(")", "").Replace("'", "").Replace("\"", "").Replace("[", "").Replace("]", "").Replace(".", "").Replace("'", "").Replace(",", "");
            var rtn = "";
            if (_nodic.ContainsKey(tkey))
            {
                rtn = tkey + (_nodic[tkey] + 1);
                _nodic[tkey] = _nodic[tkey] + 1;
            }
            else
            {
                rtn = tkey + "0";
                _nodic.Add(tkey, 0);
            }
            return rtn;
        }
        /// <summary>
        /// 将json中的特殊数据做转化
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        ConvertData ConvertObject(object v)
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
                DataType = SqlServerExpress.DataType.Value;
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
