using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RestAPISample.Common
{
    /// <summary>
    /// 关联表达式解析，解析以下格式的表达式
    /// {表名}.{关联栏位名称}[条件表达式]->{显示栏位的名称}，其中【条件表达式】为可选部分，格式为：{栏位名称}{操作符}{值}
    /// 操作符有：=,>,>=,<,<=，:(包含，模糊匹配)
    /// </summary>
    public class RefExpress
    {
        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="express"></param>
        /// <returns></returns>
        public RefExpressResult ParseExpress(string express)
        {
            var rtn = new RefExpressResult();
            if (string.IsNullOrEmpty(express)) return rtn;

            var jsonitem = FrameDLRObject.IsJsonThen(express, null, FrameDLRFlags.SensitiveCase);
            if (jsonitem != null)
            {
                rtn.TableName = ComFunc.nvl(jsonitem.GetValue("$ref_table"));
                rtn.KeyColumn = ComFunc.nvl(jsonitem.GetValue("$ref_key_column"));
                var show = jsonitem.GetValue("$show");
                if (show != null && show is IEnumerable<object>)
                {
                    rtn.ShowColumns.AddRange(((IEnumerable<object>)show).Select(d => (string)ComFunc.nvl(d)));
                }
                var where = jsonitem.GetValue("$where");
                if (where != null && where is FrameDLRObject)
                {
                    var wobj = (FrameDLRObject)where;
                    foreach (var item in wobj.Items)
                    {
                        var op = (FrameDLRObject)item.Value;
                        rtn.Filter.Add(item.Key, new FilterEntity(op.Items[0].Key, ComFunc.nvl(op.Items[0].Value)));
                    }
                }
            }
            else
            {
                var arr = express.Split("->");
                if (arr.Length < 2)
                {
                    return null;
                }

                var re_table = new Regex(@"[A-Za-z0-9_.,]+(?=\[)");
                var re_w = new Regex(@"(?<=\[)[A-Za-z0-9_=><:,@]+(?=\])");

                var ref1 = arr[0];
                var show_ref = arr[1];


                var w_str = re_w.Match(ref1).Value;
                var table_str = w_str != "" ? re_table.Match(ref1).Value : ref1;
                var table_strs = table_str.Split('.', StringSplitOptions.RemoveEmptyEntries);
                var table_ref = table_strs[0];
                var col_ref = table_strs.Length > 1 ? table_strs[1] : "";

                rtn.TableName = table_ref;
                rtn.KeyColumn = col_ref;

                foreach (var s in show_ref.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    rtn.ShowColumns.Add(s);
                }

                foreach (var w in w_str.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    var w_strs = w.Split(new string[] { ":", "=", ">", ">=", "<", "<=" }, StringSplitOptions.RemoveEmptyEntries);
                    var w_column = w_strs.ElementAt(0);
                    var w_value = w_strs.ElementAt(1);
                    var w_op = w.Replace(w_column, "").Replace(w_value, "");

                    rtn.Filter.Add(w_column, new FilterEntity(w_op, w_value));
                }
            }

            return rtn;
        }
        public class RefExpressResult
        {
            public RefExpressResult()
            {
                Filter = new Dictionary<string, FilterEntity>();
                ShowColumns = new List<string>();
            }
            /// <summary>
            /// 关联的表名
            /// </summary>
            public string TableName { get; set; }
            /// <summary>
            /// 关联的栏位名称
            /// </summary>
            public string KeyColumn { get; set; }
            /// <summary>
            /// 过滤表达式组合
            /// </summary>
            public Dictionary<string, FilterEntity> Filter
            {
                get;
                protected set;
            }
            /// <summary>
            /// 展示栏位集合
            /// </summary>
            public List<string> ShowColumns
            {
                get;
                protected set;
            }
            /// <summary>
            /// 转成json对象
            /// </summary>
            /// <returns></returns>
            public FrameDLRObject ToJSONObject()
            {
                FrameDLRObject dobj = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                if (!string.IsNullOrEmpty(TableName))
                {
                    dobj.SetValue("$ref_table", TableName);
                    dobj.SetValue("$ref_key_column", KeyColumn);
                    dobj.SetValue("$show", ShowColumns.ToArray());
                    FrameDLRObject where = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);


                    foreach (var item in Filter)
                    {
                        FrameDLRObject op = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                        op.SetValue(item.Value.Op, item.Value.Value);
                        where.SetValue(item.Key, op);
                    }
                    dobj.SetValue("$where", where);
                }
                return dobj;
            }
            /// <summary>
            /// 转成json串
            /// </summary>
            /// <returns></returns>
            public string ToJSON()
            {
                var rtn = ToJSONObject();
                return rtn == null ? "" : rtn.ToJSONString(true);
            }
            /// <summary>
            /// 转化成DBExpress表达式
            /// </summary>
            /// <param name="this_data">当前行资料，用于解析表达式中@语句</param>
            /// <returns></returns>
            public FrameDLRObject ToDBExpress(FrameDLRObject this_data = null)
            {
                if (this_data == null) this_data = FrameDLRObject.CreateInstance();

                FrameDLRObject express = FrameDLRObject.CreateInstance($@"{{
$acttype : 'Query',
$orderby : 'sort_no',
$table:'{TableName}'
                }}", EFFC.Frame.Net.Base.Constants.FrameDLRFlags.SensitiveCase);
                foreach (var c in ShowColumns)
                {
                    express.SetValue(c, true);
                }
                if (!string.IsNullOrEmpty(KeyColumn))
                {
                    express.SetValue(KeyColumn, true);
                }
                if (Filter.Count > 0)
                {
                    FrameDLRObject where = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);

                    foreach (var witem in Filter)
                    {
                        FrameDLRObject op_express = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                        var op = witem.Value.Op.StartsWith("$") ? witem.Value.Op : $"${witem.Value.Op}";
                        //如果w_value为@开头，则表示@后面为本表的栏位名称，则从当前行资料中获取对应的参数的值
                        if (witem.Value.Value.StartsWith("@"))
                        {
                            op_express.SetValue(op, this_data.GetValue(witem.Value.Value.Replace("@", "")));
                        }
                        else
                        {
                            op_express.SetValue(op, witem.Value.Value);
                        }

                        where.SetValue(witem.Key, op_express);
                    }

                    express.SetValue("$where", where);
                }
                return express;
            }
        }
        /// <summary>
        /// 过滤条件对象
        /// </summary>
        public class FilterEntity
        {
            public FilterEntity(string op, string value)
            {
                var op_map = new Dictionary<string, string>();
                op_map.Add(":", "$like");
                op_map.Add("=", "$eq");
                op_map.Add(">", "$gt");
                op_map.Add(">=", "$gte");
                op_map.Add("<", "$lt");
                op_map.Add("<=", "$lte");

                Op = op_map.ContainsKey(op) ? op_map[op] : op;
                Value = value;
            }
            /// <summary>
            /// 操作符号
            /// </summary>
            public string Op
            {
                get;
                protected set;
            }
            /// <summary>
            /// 值
            /// </summary>
            public string Value
            {
                get;
                protected set;
            }
        }
    }
}
