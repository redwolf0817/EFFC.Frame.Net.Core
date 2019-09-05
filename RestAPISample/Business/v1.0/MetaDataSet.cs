using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using System.IO;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Base.Constants;
using RestAPISample.Common;

namespace RestAPISample.Business.v1._0
{
    public class MetaDataSet : MyRestLogic
    {
        RefExpress ref_express = new RefExpress();

        [EWRARouteDesc("获取下拉框或pop的内容")]
        [EWRARoute("patch", "/metadataset/pop/{id}/{column_name}")]
        [EWRAAddInput("id", "string", "元数据表的uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("column_name", "string", "指定栏位的名称", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("this_data", "string", @"页面上当前编辑的行资料，先采用url编码，再采用base64编码，原数据格式:
{
    column_name1:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
    column_name2:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
    .....
    column_nameN:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
}", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAOutputDesc("返回结果", @"如果是授权无效的情况下，返回400错，其它情况的返回结果集（状态为200）如下：{
code:'success-成功，failed-失败',
msg:'提示信息',
column_value:'表示选中值的栏位名称,如果为空，则表示接受UI设定的任意值',
column_show:[{
    column_name:'栏位名称',
    column_desc:'栏位中文名称',
    ui_type:'UI控件类型'
}],
data:[结果集]")]
        object GetPop(string id, string column_name)
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var metainfo = (from t in DB.LamdaTable(up, "EXTEND_METADATA_COLUMNS", "a")
                            where t.MetaUID == id && t.MetaColumnName == column_name
                            select new
                            {
                                reference = t.MetaReference
                            }).GetQueryList(up);
            if (metainfo.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "相关栏位资料不存在，无法继续进行操作"
                };
            }
            var column_ref = ComFunc.nvl(metainfo.First().GetValue("reference"));
            if (column_ref == "")
            {
                return new
                {
                    code = "failed",
                    msg = "相关栏位缺少关联数据，无法继续进行操作"
                };
            }

            var refResult = ref_express.ParseExpress(column_ref);
            if (refResult == null)
            {
                return new
                {
                    code = "failed",
                    msg = "相关栏位关联信息描述存在错误，无法继续进行操作"
                };
            }
            var data_str = ComFunc.nvl(PostDataD.this_data).Replace(" ", "+");
            var this_data_base64str = ComFunc.IsBase64Then(data_str);
            string this_data_str = ComFunc.UrlDecode(this_data_base64str);
            var this_data = FrameDLRObject.IsJsonThen(this_data_str);
            if (data_str != "" && (this_data_str == "" || this_data == null))
            {
                return new
                {
                    code = "failed",
                    msg = "当前行资料格式不正确"
                };
            }

            var data = DB.Excute(up, refResult.ToDBExpress(this_data), true).QueryData<FrameDLRObject>();
            //获取column_show的中文名称
            var metacolumns = (from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                               join t2 in DB.LamdaTable(up, "EXTEND_METADATA_COLUMNS", "b") on t.MetaUID equals t2.MetaUID
                               join t3 in DB.LamdaTable(up, "EXTEND_METADATA_DATATYPE", "c").LeftJoin() on t2.MetaDataType equals t3.DataType
                               where t.MetaName == refResult.TableName
                               select new
                               {
                                   column_name = t2.MetaColumnName,
                                   column_desc = t2.MetaColumnDesc,
                                   ui_type = t3.UI_TYPE
                               }).GetQueryList(up);
            var dictable = (from t in DB.LamdaTable(up, "EXTEND_DICTIONARY_TABLE", "a")
                            where t.DIC_Name == refResult.TableName
                            select new
                            {
                                t.IsTree
                            }
                              ).GetQueryList(up);
            if (dictable.Count > 0)
            {
                metacolumns.Add(FrameDLRObject.CreateInstance(@"{
column_name:'code',
column_desc:'编号',
ui_type:'Input'
}"));
                metacolumns.Add(FrameDLRObject.CreateInstance(@"{
column_name:'value',
column_desc:'值',
ui_type:'Input'
}"));
                if (BoolStd.IsNotBoolThen(dictable.First().GetValue("IsTree")))
                {
                    metacolumns.Add(FrameDLRObject.CreateInstance(@"{
column_name:'p_code',
column_desc:'父节点编号编号',
ui_type:'Input'
}"));
                    metacolumns.Add(FrameDLRObject.CreateInstance(@"{
column_name:'level',
column_desc:'层级',
ui_type:'Input'
}"));
                }
            }
            var column_map = metacolumns.ToDictionary(k => k.GetValue("column_name"), v => new { v.column_desc, v.ui_type });
            return new
            {
                code = "success",
                msg = "",
                column_value = refResult.KeyColumn,
                column_show = from t in refResult.ShowColumns
                              select new
                              {
                                  column_name = t,
                                  column_desc = column_map.ContainsKey(t) ? column_map[t].column_desc : "",
                                  ui_type = column_map.ContainsKey(t) ? column_map[t].ui_type : "Input"
                              },
                data
            };
        }
        [EWRARouteDesc("获取ui_type为picxy的弹出框内容")]
        [EWRARoute("patch", "/metadataset/pop/picxy/{id}/{column_name}")]
        [EWRAAddInput("id", "string", "元数据表的uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("column_name", "string", "指定栏位的名称", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("this_data", "string", @"页面上当前编辑的行资料，先采用url编码，再采用base64编码，原数据格式:
{
    column_name1:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
    column_name2:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
    .....
    column_nameN:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
}", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, true)]
        [EWRAOutputDesc("返回结果", @"如果是授权无效的情况下，返回400错，其它情况的返回结果集（状态为200）如下：{
code:'success-成功，failed-失败',
msg:'提示信息',
filetype:'文件的content-type类型'
filename:'文件名称',
filelength:'文件长度',
file:'文件内容，采用base64加密'
}")]
        object GetPicXYPop(string id, string column_name)
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                    where (t.metauid == id || t.metaname == id) && t.IsCreated == 1
                    select t;
            var list = s.GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "元数据表不存在",
                };
            }
            dynamic info = list.First();

            var metainfo = (from t in DB.LamdaTable(up, "EXTEND_METADATA_COLUMNS", "a")
                            where t.MetaUID == info.MetaUID && t.MetaColumnName == column_name
                            select new
                            {
                                reference = t.MetaReference
                            }).GetQueryList(up);
            if (metainfo.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "相关栏位资料不存在，无法继续进行操作"
                };
            }
            var column_ref = ComFunc.nvl(metainfo.First().GetValue("reference"));
            if (column_ref == "")
            {
                return new
                {
                    code = "failed",
                    msg = "相关栏位缺少关联数据，无法继续进行操作"
                };
            }

            var refResult = ref_express.ParseExpress(column_ref);
            if (refResult == null)
            {
                return new
                {
                    code = "failed",
                    msg = "相关栏位关联信息描述存在错误，无法继续进行操作"
                };
            }
            var data_str = ComFunc.nvl(PostDataD.this_data).Replace(" ", "+");
            var this_data_base64str = ComFunc.IsBase64Then(data_str);
            string this_data_str = ComFunc.UrlDecode(this_data_base64str);
            var this_data = FrameDLRObject.IsJsonThen(this_data_str);
            if (data_str != "" && (this_data_str == "" || this_data == null))
            {
                return new
                {
                    code = "failed",
                    msg = "当前行资料格式不正确"
                };
            }

            var data = DB.Excute(up, refResult.ToDBExpress(this_data), true);
            if (data.QueryTable.RowLength <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "无法查到指定图片数据"
                };
            }
            //只抓取第一行资料的第一个栏位的值
            var result = FileHelper.DoDownLoad(ComFunc.nvl(data.QueryTable[0, 0]));
            return result;
        }

        [EWRARouteDesc("获取指定元数据表中的数据列表")]
        [EWRAAddInput("id", "string", "元数据表uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("mode", "string", "查询模式，Normal:普通模式，翻页查询（需要提供翻页参数）；1000：抓取前1000笔资料；All:抓取所有资料", "默认为Normal", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("limit", "int", "每页笔数，当mode为Normal时需要提供", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "指定页数，当mode为Normal时需要提供", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("filter_column", "string", "过滤条件的栏位名称", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("filter_op", "string", "过滤条件的操作符号，类型如下：eq:等于，gt：大于，gte：大于等于，lt：小于，lte：小于等于，like:模糊比对", "默认为eq", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("filter_value", "string", "过滤条件的值", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息',
columns:[{
    column_name:'栏位名称,英文字符',
    column_desc:'栏位描述，中文描述',
    data_type:'栏位类型',
    is_allow_empty:'是否为允许为空，不允许为空时，必须做必输检查',
    is_pk:'是否为PK栏位',
    reference:'栏位关联的信息，该关联信息只适用于一对一的关联，一对多不适用，格式为：
                默认的格式(普通格式，返回值为【关联栏位名称】的值，如果没有设定则表示返回值由UI来决定)
                ',
    sort_num:'栏位排序编号',
    ui_type:'UI类型，有以下几种：Input-输入框（默认）;Select-下拉选框;Pop:弹出框（显示勾选列表）;Calendar:日历控件;Switch:左右滑动开关;RichText:富文本编辑器;File:文件，点击后弹框显示上传文件控件和下载按钮;Picture:点击后弹框显示上传文件控件和显示图片;MapXY:在地图上点击后返回经纬度(格式：{经度},{纬度});PicXY:在图片上点击后返回XY轴坐标(格式：{X坐标},{Y坐标})'
}],
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[元数据的结果集]
}")]
        public override object get(string id)
        {
            SetCacheEnable(false);

            string mode = ComFunc.nvl(QueryStringD.mode);
            string filter_column = ComFunc.nvl(QueryStringD.filter_column);
            string filter_op = ComFunc.nvl(QueryStringD.filter_op);
            string filter_value = ComFunc.nvl(QueryStringD.filter_value);

            if (!string.IsNullOrEmpty(filter_column))
            {
                if (!new string[] { "eq", "gt", "gte", "lt", "lte", "like" }.Contains(filter_op))
                {
                    return new
                    {
                        code = "failed",
                        msg = "过滤条件操作不符合要求"
                    };
                }
            }

            mode = string.IsNullOrEmpty(mode) ? "Normal" : mode;


            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                    where (t.metauid == id || t.metaname == id) && t.IsCreated == 1
                    select t;
            var list = s.GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "元数据表不存在",
                };
            }
            dynamic info = list.First();
            string tablename = info.metaname;
            var columns = (from t in DB.LamdaTable(up, "EXTEND_METADATA_COLUMNS", "a")
                           orderby t.sortnum
                           where t.metauid == info.metauid && t.isshow == 1
                           select new
                           {
                               column_name = t.MetaColumnName,
                               column_desc = t.MetaColumnDesc,
                               data_type = t.MetaDataType,
                               reference = t.MetaReference,
                               is_allow_empty = t.MetaAllowEmpty,
                               is_pk = t.MetaIsPK,
                               sort_num = t.SortNum,
                               is_show = t.IsShow
                           }).GetQueryList(up);
            //固定带上sort_no
            columns.Add(FrameDLRObject.CreateInstance(new
            {
                column_name = "sort_no",
                column_desc = "排序",
                data_type = "int",
                reference = "",
                is_allow_empty = true,
                is_pk = false,
                sort_num = 99999,
                is_show = true
            }));
            var support_data_type = (from t in DB.LamdaTable(up, "EXTEND_METADATA_DATATYPE", "a")
                                     select new
                                     {
                                         data_type = t.DataType,
                                         desc = t.DataTypeDesc,
                                         is_allow_empty_zero_precision = t.IsAllowEmptyOrZero_Precision,
                                         is_allow_empty_zero_scale = t.IsAllowEmptyOrZero_Scale,
                                         is_allow_pk = t.IsAllowPK,
                                         is_auto_fix = t.IsAutoFix,
                                         auto_fix_method = t.AutoFix_Method,
                                         ui_type = t.UI_TYPE
                                     }).GetQueryList(up);
            #region 构建select表达式
            FrameDLRObject express = FrameDLRObject.CreateInstance(@"{
$acttype : 'QueryByPage',
$orderby : 'sort_no',
                }", EFFC.Frame.Net.Base.Constants.FrameDLRFlags.SensitiveCase);
            switch (mode)
            {
                case "1000":
                    express.SetValue("$acttype", "QueryByPage");
                    up.Count_Of_OnePage = 1000;
                    up.ToPage = 1;
                    break;
                case "All":
                    express.SetValue("$acttype", "Query");
                    break;
                default:
                    express.SetValue("$acttype", "QueryByPage");
                    break;
            }

            FrameDLRObject tableexpress = FrameDLRObject.CreateInstance(EFFC.Frame.Net.Base.Constants.FrameDLRFlags.SensitiveCase);

            foreach (dynamic c in columns)
            {
                if (BoolStd.IsNotBoolThen(c.is_show))
                {
                    //设定select的栏位
                    express.SetValue($"{c.column_name}", true);
                    //设定是否要求输入
                    c.is_allow_empty = BoolStd.IsNotBoolThen(c.is_allow_empty);
                    c.is_pk = BoolStd.IsNotBoolThen(c.is_pk);
                }
            }
            express.SetValue("$table", tablename);

            if (!string.IsNullOrEmpty(filter_column))
            {
                FrameDLRObject where = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                FrameDLRObject exp = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                exp.SetValue($"${filter_op}", filter_value);
                where.SetValue(filter_column, exp);
                express.SetValue("$where", where);
            }
            #endregion
            var result = DB.Excute(up, express, true);
            var resultlist = result.QueryData<FrameDLRObject>();

            var data = resultlist.Select((p) =>
            {
                foreach (var k in p.Keys)
                {
                    if (p.GetValue(k) is DateTime)
                    {
                        p.SetValue(k, DateTimeStd.IsDateTimeThen(p.GetValue(k), "yyyy-MM-dd HH:mm:ss"));
                    }
                }
                return p;
            }).ToList();

            var tablemap = getTableColumnsMap(up);
            var data_type_map = support_data_type.ToDictionary(k => (string)ComFunc.nvl(k.GetValue("data_type")), v => new { v.desc, v.ui_type });
            foreach (dynamic c in columns)
            {
                //根据reference和data_type设定ui_type
                c.ui_type = data_type_map.ContainsKey(ComFunc.nvl(c.data_type)) ? data_type_map[c.data_type].ui_type : "Input";
                string reference = ComFunc.nvl(c.reference);
                var refresult = ref_express.ParseExpress(reference);
                if (refresult != null && !string.IsNullOrEmpty(refresult.TableName))
                {
                    if (!new string[] { "MapXY", "PicXY" }.Contains((string)c.ui_type))
                    {
                        if (refresult.ShowColumns.Count > 1)
                        {
                            c.ui_type = "Pop";
                        }
                        else
                        {
                            c.ui_type = "Select";
                        }
                    }
                }
            }

            if (mode == "All")
            {
                return new
                {
                    code = "success",
                    msg = "",
                    columns = columns,
                    total_count = data.Count,
                    page = 1,
                    total_page = 1,
                    limit = data.Count,
                    data = data
                };
            }
            else
            {
                return new
                {
                    code = "success",
                    msg = "",
                    columns = columns,
                    total_count = result.TotalRow,
                    page = result.CurrentPage,
                    total_page = result.TotalPage,
                    limit = result.Count_Of_OnePage,
                    data = data
                };
            }

        }
        [EWRAEmptyValid("id,data")]
        [EWRARouteDesc("新增元数据表中的数据")]
        [EWRAAddInput("id", "string", "元数据表uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("data", "array", @"待新增的数据列表，如果为file、picture类型,则需要先进行上传，再传入返回的文件路径，格式:
[
{
    column_name1:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
    column_name2:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
    .....
    column_nameN:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
}
]
", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息'
}")]
        public override object post()
        {
            var id = ComFunc.nvl(PostDataD.id);
            var data = PostDataD.data;

            if (!(data is IEnumerable<object>))
                return new
                {
                    code = "failed",
                    msg = "参数格式不正确",
                };
            var datalist = (IEnumerable<object>)data;
            BeginTrans();
            var up = DB.NewDBUnitParameter();
            var rtn = DoPost(up, id, datalist, TokenPayLoad.ID, ComFunc.nvl(TokenPayLoad["username"]), ClientInfo.IP, BoolStd.IsNotBoolThen(Configs["Is_UseLocal"]));
            CommitTrans();
            return rtn;
        }
        public object DoPost(UnitParameter up, string id, IEnumerable<object> data,
            string login_id = "", string login_name = "", string login_ip = "", bool is_use_local_upload = true)
        {
            var datalist = data.Select((p) =>
            {
                var dobj = (FrameDLRObject)p;
                dobj.Remove("RowNumber");
                return dobj;
            }).ToList();
            var s = from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                    where (t.metauid == id || t.metaname == id) && t.IsCreated == 1
                    select t;
            var list = s.GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "元数据表不存在",
                };
            }
            dynamic info = list.First();
            string tablename = info.metaname;
            var columns = (from t in DB.LamdaTable(up, "EXTEND_METADATA_COLUMNS", "a")
                           where t.metauid == info.metauid
                           select new
                           {
                               column_name = t.MetaColumnName,
                               column_desc = t.MetaColumnDesc,
                               is_pk = t.MetaIsPK,
                               data_type = t.MetaDataType,
                               is_allow_empty = t.MetaAllowEmpty,
                               precision = t.MetaDataPrecision
                           }).GetQueryList(up);
            //固定带上sort_no
            columns.Add(FrameDLRObject.CreateInstance(new
            {
                column_name = "sort_no",
                column_desc = "排序",
                is_pk = false,
                data_type = "int",
                is_allow_empty = true
            }));
            //目前系统支持的数据类型及相关约束
            var support_data_type = (from t in DB.LamdaTable(up, "EXTEND_METADATA_DATATYPE", "a")
                                     select new
                                     {
                                         data_type = t.DataType,
                                         desc = t.DataTypeDesc,
                                         is_allow_empty_zero_precision = t.IsAllowEmptyOrZero_Precision,
                                         is_allow_empty_zero_scale = t.IsAllowEmptyOrZero_Scale,
                                         is_allow_pk = t.IsAllowPK,
                                         is_auto_fix = t.IsAutoFix,
                                         auto_fix_method = t.AutoFix_Method,
                                         ui_type = t.UI_TYPE
                                     }).GetQueryList(up);

            var column_names = columns.Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var column_int = columns.Where(w => w.data_type == "int").Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var column_numbric = columns.Where(w => w.data_type == "numberic").Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var column_bit = columns.Where(w => w.data_type == "bit").Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var column_datetime = columns.Where(w => w.data_type == "datetime").Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var pkcolumns = columns.Where(p => BoolStd.IsNotBoolThen(p.is_pk, false)).Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var notemptycolumns = columns.Where(p => !BoolStd.IsNotBoolThen(p.is_allow_empty, true)).Select((p) =>
            {
                return new
                {
                    column_name = ComFunc.nvl(p.GetValue("column_name")),
                    column_desc = ComFunc.nvl(p.GetValue("column_desc"))
                };
            }).ToList();
            //系统自动填入参数的栏位
            var auto_fix_datatype = support_data_type.Where(w => BoolStd.IsNotBoolThen(w.is_auto_fix));
            var auto_fix_string = auto_fix_datatype.Select(d => ComFunc.nvl(((FrameDLRObject)d).GetValue("data_type"))).ToArray();
            var auto_fix_columns = columns.Where(p => auto_fix_string.Contains((string)ComFunc.nvl(p.data_type))).Select((p) =>
            {
                return new
                {
                    column_name = ComFunc.nvl(p.GetValue("column_name")),
                    data_type = ComFunc.nvl(p.GetValue("data_type")),
                    is_allow_empty = BoolStd.IsNotBoolThen(p.GetValue("is_allow_empty"), true),
                    precision = IntStd.IsNotIntThen(p.GetValue("precision"), 0)
                };
            }).ToList();
            //文件类型栏位
            var filetype_string = new string[] { "file", "picture" };
            var file_columns = columns.Where(p => filetype_string.Contains((string)ComFunc.nvl(p.data_type))).Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            //系统自动填入数据的栏位
            foreach (FrameDLRObject d in datalist)
            {
                //去除RowNumber
                d.Remove("RowNumber");

                foreach (var c in auto_fix_columns)
                {
                    if (ComFunc.nvl(d.GetValue(c.column_name)) == ""
                        && !c.is_allow_empty)
                    {
                        var dts = auto_fix_datatype.Where(w => w.data_type == c.data_type);
                        if (dts.Count() > 0)
                        {
                            d.SetValue(c.column_name, genAutoFixValue(ComFunc.nvl(dts.First().GetValue("auto_fix_method")), c.precision));
                        }
                    }
                }
            }
            //栏位检查
            foreach (FrameDLRObject d in datalist)
            {
                foreach (var ne in notemptycolumns)
                {
                    if (ComFunc.nvl(d.GetValue(ne.column_name)) == "")
                    {
                        return new
                        {
                            code = "failed",
                            msg = $"{ne.column_desc}栏位不可为空",
                        };
                    }
                }
                if ((from t in d.Keys
                     join t2 in pkcolumns on t equals t2
                     select t).Count() != pkcolumns.Count)
                {
                    return new
                    {
                        code = "failed",
                        msg = "数据缺少PK栏位的数据，无法进行数据新增",
                    };
                }
                foreach (var key in d.Keys)
                {
                    if (!column_names.Contains(key))
                    {
                        return new
                        {
                            code = "failed",
                            msg = "数据栏位不匹配",
                        };
                    }
                    //数据类型检查
                    if (column_int.Contains(key) && ComFunc.nvl(d.GetValue(key)) != "" && !IntStd.IsInt(d.GetValue(key)))
                    {
                        return new
                        {
                            code = "failed",
                            msg = $"值({d.GetValue(key)})不是int类型",
                        };
                    }
                    if (column_numbric.Contains(key) && ComFunc.nvl(d.GetValue(key)) != "" && !DecimalStd.IsDecimal(d.GetValue(key)))
                    {
                        return new
                        {
                            code = "failed",
                            msg = $"({d.GetValue(key)})不是numberic类型",
                        };
                    }
                    if (column_bit.Contains(key) && ComFunc.nvl(d.GetValue(key)) != "" && !BoolStd.IsBool(d.GetValue(key)))
                    {
                        return new
                        {
                            code = "failed",
                            msg = $"({d.GetValue(key)})不是bit类型",
                        };
                    }
                    if (column_datetime.Contains(key) && ComFunc.nvl(d.GetValue(key)) != "" && !DateTimeStd.IsDateTime(d.GetValue(key)))
                    {
                        return new
                        {
                            code = "failed",
                            msg = $"({d.GetValue(key)})不是datetime类型",
                        };
                    }
                }


            }
            //有pk设定的时候做检查
            if (pkcolumns.Count > 0)
            {
                foreach (FrameDLRObject d in datalist)
                {
                    //pk栏位不可为空

                    FrameDLRObject w = FrameDLRObject.CreateInstance();
                    foreach (var pk in pkcolumns)
                    {
                        if (ComFunc.nvl(d.GetValue(pk)) == "")
                        {
                            return new
                            {
                                code = "failed",
                                msg = "PK栏位不可为空",
                            };
                        }
                        w.SetValue(pk, d.GetValue(pk));
                    }
                    FrameDLRObject express = FrameDLRObject.CreateInstance($@"{{
$acttype : 'Query',
$table:'{tablename}'
}}");
                    express.SetValue("$where", w);
                    if (DB.Excute(up, express).QueryTable.RowLength > 0)
                    {
                        return new
                        {
                            code = "failed",
                            msg = "数据重复，不能继续进行操作",
                        };
                    }
                }
            }
            //如果有file、picture类型，则需要做上传文件的检查
            if (is_use_local_upload)
            {
                if (file_columns.Count > 0)
                {
                    foreach (FrameDLRObject d in datalist)
                    {
                        foreach (var c in file_columns)
                        {
                            var path = ComFunc.nvl(d.GetValue(c));
                            if (path != "" && !FileHelper.IsExists(path))
                            {
                                return new
                                {
                                    code = "failed",
                                    msg = "上传的文件不存在或上传失败"
                                };
                            }
                        }
                    }
                }
            }
            foreach (FrameDLRObject d in datalist)
            {
                //将文件copy到正式目录中
                //使用本地上传方式
                if (is_use_local_upload)
                {
                    var to_path = $"~/meta_data/{tablename}/";
                    foreach (var c in file_columns)
                    {
                        var path = ComFunc.nvl(d.GetValue(c));
                        var newpath = FileHelper.CopyUploadFileTo(path, to_path);
                        d.SetValue(c, newpath);
                    }
                }

                foreach (var cc in column_datetime)
                {
                    d.SetValue(cc, DateTimeStd.IsDateTimeThen(d.GetValue(cc), "yyyy-MM-dd HH:mm:ss"));
                }
                //如果数据为空，则转化成dbnull，以免报类型转化异常
                foreach (var cc in column_names)
                {
                    if (ComFunc.nvl(d.GetValue(cc)) == "")
                    {
                        d.SetValue(cc, DBNull.Value);
                    }
                }

                //增加操作者的信息
                d.SetValue("add_id", login_id);
                d.SetValue("add_ip", login_ip);
                d.SetValue("add_name", login_name);
                d.SetValue("add_time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                d.SetValue("last_id", login_id);
                d.SetValue("last_ip", login_ip);
                d.SetValue("last_name", login_name);
                d.SetValue("last_time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                DB.QuickInsert(up, tablename, d);


                //记录异动记录
                var changcontent = d.ToJSONString();
                DB.QuickInsert(up, $"{tablename}_change_log", new
                {
                    UID = Guid.NewGuid().ToString(),
                    ActionType = "Insert",
                    ChangeDataContent = changcontent,
                    FromSourceID = "DCAP",
                    FromSourceName = "数据中心",
                    add_id = login_id,
                    add_name = login_name,
                    add_ip = login_ip,
                    add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    last_id = login_id,
                    last_name = login_name,
                    last_ip = login_ip,
                    last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            return new
            {
                code = "success",
                msg = "操作成功",
            };
        }

        [EWRAEmptyValid("data")]
        [EWRARouteDesc("删除元数据表中的数据")]
        [EWRAAddInput("id", "string", "元数据表uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("data", "array", @"待删除的数据列表，格式:
[
{
    column_name1:'值',
    column_name2:'值',
    .....
    column_nameN:'值',
}
]", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAOutputDesc("返回结果", @"根据http的状态码来识别，204标识操作成功，404表示操作识别未找到删除的资料")]
        public override bool delete(string id)
        {
            var data = PostDataD.data;
            if (!(data is IEnumerable<object>))
            {
                return false;
            }
            var datalist = (IEnumerable<object>)data;

            BeginTrans();
            var up = DB.NewDBUnitParameter();
            var rtn = DoDelete(up, id, datalist, TokenPayLoad.ID, ComFunc.nvl(TokenPayLoad["username"]), ClientInfo.IP, BoolStd.IsNotBoolThen(Configs["Is_UseLocal"]));
            CommitTrans();
            return rtn;
        }
        public bool DoDelete(UnitParameter up, string id, IEnumerable<object> data, string login_id = "", string login_name = "", string login_ip = "", bool is_use_local_upload = true)
        {
            var datalist = data.Select((d) =>
            {
                var dobj = (FrameDLRObject)d;
                dobj.Remove("RowNumber");
                return dobj;
            }).ToList();
            var s = from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                    where (t.metauid == id || t.metaname == id) && t.IsCreated == 1
                    select t;
            var list = s.GetQueryList(up);
            if (!(from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                  where (t.metauid == id || t.metaname == id) && t.IsCreated == 1
                  select t).IsExists(up))
            {
                return false;
            }
            dynamic info = list.First();
            string tablename = info.metaname;
            var columns = (from t in DB.LamdaTable(up, "EXTEND_METADATA_COLUMNS", "a")
                           where t.metauid == info.metauid
                           select new
                           {
                               column_name = t.MetaColumnName,
                               is_pk = t.MetaIsPK,
                               data_type = t.MetaDataType
                           }).GetQueryList(up);
            //固定带上sort_no
            columns.Add(FrameDLRObject.CreateInstance(new
            {
                column_name = "sort_no",
                is_pk = false,
                data_type = "int"
            }));
            var column_names = columns.Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var pkcolumns = columns.Where(p => BoolStd.IsNotBoolThen(p.is_pk, false)).Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var filetype_string = new string[] { "file", "picture" };
            var file_columns = columns.Where(p => filetype_string.Contains((string)ComFunc.nvl(p.data_type))).Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            //栏位检查
            foreach (FrameDLRObject d in datalist)
            {
                foreach (var key in d.Keys)
                {
                    if (!column_names.Contains(key))
                    {
                        return false;
                    }
                }
            }
            foreach (FrameDLRObject d in datalist)
            {
                //如果数据中有file、picture类型数据则需要做删除
                if (is_use_local_upload)
                {
                    foreach (var fcolumn in file_columns)
                    {
                        var filepath = ComFunc.nvl(d.GetValue(fcolumn));
                        delUploadFile(filepath);
                    }
                }
                //有pk栏位的按照pk进行资料删除
                if (pkcolumns.Count <= 0)
                {
                    //有值的数据才计入条件
                    FrameDLRObject w = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                    foreach (var item in d.Items)
                    {
                        if (ComFunc.nvl(item.Value) != "")
                        {
                            w.SetValue(item.Key, item.Value);
                        }
                    }
                    DB.QuickDelete(up, tablename, w);
                }
                else
                {
                    FrameDLRObject w = FrameDLRObject.CreateInstance();
                    foreach (var pkc in pkcolumns)
                    {
                        w.SetValue(pkc, d.GetValue(pkc));
                    }
                    DB.QuickDelete(up, tablename, w);
                }
                //记录异动记录
                DB.QuickInsert(up, $"{tablename}_change_log", new
                {
                    UID = Guid.NewGuid().ToString(),
                    ActionType = "Delete",
                    ChangeDataContent = d.ToJSONString(),
                    FromSourceID = "DCAP",
                    FromSourceName = "数据中心",
                    add_id = login_id,
                    add_name = login_name,
                    add_ip = login_ip,
                    add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    last_id = login_id,
                    last_name = login_name,
                    last_ip = login_ip,
                    last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            return true;
        }

        [EWRARoute("delete", "/metadataset/{id}/all")]
        [EWRARouteDesc("删除指定元数据表中的所有数据")]
        [EWRAAddInput("id", "string", "元数据表uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAOutputDesc("返回结果", @"根据http的状态码来识别，204标识操作成功，404表示操作识别未找到删除的资料")]
        public bool DeleteAll(string id)
        {
            BeginTrans();
            var up = DB.NewDBUnitParameter();
            var rtn = DoDeleteALl(up, id);
            CommitTrans();
            return rtn;
        }
        public bool DoDeleteALl(UnitParameter up, string id, string login_id = "", string login_name = "", string login_ip = "")
        {
            var s = from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                    where (t.metauid == id || t.metaname == id) && t.IsCreated == 1
                    select t;
            var list = s.GetQueryList(up);
            if (!(from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                  where (t.metauid == id || t.metaname == id) && t.IsCreated == 1
                  select t).IsExists(up))
            {
                return false;
            }
            dynamic info = list.First();
            string tablename = info.metaname;
            (from t in DB.LamdaTable(up, tablename, "a")
             select t).Delete(up);

            return true;
        }
        [EWRAEmptyValid("data,ori_data")]
        [EWRARouteDesc("修改元数据表中的数据")]
        [EWRAAddInput("id", "string", "元数据表uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("ori_data", "array", @"未修改的原始数据数据的条件，如果为file、picture类型,则需要先进行上传，再传入返回的文件路径，格式:
[
{
    column_name1:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
    column_name2:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
    .....
    column_nameN:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
}
]", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("data", "array", @"待修改的数据列表，该数组中的每笔资料必须与ori_data中的下标和栏位都一致，如果为file、picture类型,则需要先进行上传，再传入返回的文件路径，格式:
[
{
    column_name1:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
    column_name2:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
    .....
    column_nameN:'值，如果为file、picture类型则值的格式内容为上传后的文件路径',
}
]", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息'
}")]
        public override object patch(string id)
        {
            var data = PostDataD.data;
            var ori_data = PostDataD.ori_data;
            if (!(data is IEnumerable<object>) || !(ori_data is IEnumerable<object>))
            {
                return new
                {
                    code = "failed",
                    msg = "参数格式不正确",
                };
            }
            var datalist = (IEnumerable<object>)data;
            var ori_datalist = (IEnumerable<object>)ori_data;
            var up = DB.NewDBUnitParameter();
            BeginTrans();
            var rtn = DoPatch(up, id, datalist, ori_datalist, TokenPayLoad.ID, ComFunc.nvl(TokenPayLoad["username"]), ClientInfo.IP, BoolStd.IsNotBoolThen(Configs["Is_UseLocal"]));
            CommitTrans();
            return rtn;
        }

        public object DoPatch(UnitParameter up, string id, IEnumerable<object> data, IEnumerable<object> ori_data,
            string login_id = "", string login_name = "", string login_ip = "", bool is_use_local_upload = true)
        {
            var datalist = data.Select((p) =>
            {
                var dobj = (FrameDLRObject)p;
                dobj.Remove("RowNumber");
                return dobj;
            }).ToList();
            var oridatalist = ori_data.Select((p) =>
            {
                var dobj = (FrameDLRObject)p;
                dobj.Remove("RowNumber");
                return dobj;
            }).ToList(); ;
            if (datalist.Count() != oridatalist.Count())
            {
                return new
                {
                    code = "failed",
                    msg = "ori_data与data的数据不一致",
                };
            }
            for (var i = 0; i < datalist.Count(); i++)
            {
                if (datalist[i].Keys.Count != oridatalist[i].Keys.Count)
                {
                    return new
                    {
                        code = "failed",
                        msg = "ori_data与data的数据不一致",
                    };
                }
            }

            for (int i = 0; i < datalist.Count(); i++)
            {
                if ((from t in datalist[i].Keys join t2 in oridatalist[i].Keys on t equals t2 select t).Count() != datalist[i].Keys.Count)
                {
                    return new
                    {
                        code = "failed",
                        msg = "ori_data与data的数据不一致",
                    };
                }
            }

            var s = from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                    where (t.metauid == id || t.metaname == id) && t.IsCreated == 1
                    select t;
            var list = s.GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "元数据表不存在",
                };
            }
            dynamic info = list.First();
            string tablename = info.metaname;
            var columns = (from t in DB.LamdaTable(up, "EXTEND_METADATA_COLUMNS", "a")
                           where t.metauid == info.metauid
                           select new
                           {
                               column_name = t.MetaColumnName,
                               column_desc = t.MetaColumnDesc,
                               data_type = t.MetaDataType,
                               is_pk = t.MetaIsPK,
                               is_allow_empty = t.MetaAllowEmpty,
                               precision = t.MetaDataPrecision
                           }).GetQueryList(up);
            //固定带上sort_no
            columns.Add(FrameDLRObject.CreateInstance(new
            {
                column_name = "sort_no",
                column_desc = "排序",
                is_pk = false,
                data_type = "int",
                is_allow_empty = true
            }));
            //目前系统支持的数据类型及相关约束
            var support_data_type = (from t in DB.LamdaTable(up, "EXTEND_METADATA_DATATYPE", "a")
                                     select new
                                     {
                                         data_type = t.DataType,
                                         desc = t.DataTypeDesc,
                                         is_allow_empty_zero_precision = t.IsAllowEmptyOrZero_Precision,
                                         is_allow_empty_zero_scale = t.IsAllowEmptyOrZero_Scale,
                                         is_allow_pk = t.IsAllowPK,
                                         is_auto_fix = t.IsAutoFix,
                                         auto_fix_method = t.AutoFix_Method,
                                         ui_type = t.UI_TYPE
                                     }).GetQueryList(up);

            var column_names = columns.Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var column_int = columns.Where(w => w.data_type == "int").Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var column_numbric = columns.Where(w => w.data_type == "numberic").Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var column_bit = columns.Where(w => w.data_type == "bit").Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var column_datetime = columns.Where(w => w.data_type == "datetime").Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var notemptycolumns = columns.Where(p => !BoolStd.IsNotBoolThen(p.is_allow_empty, true)).Select((p) =>
            {
                return new
                {
                    column_name = ComFunc.nvl(p.GetValue("column_name")),
                    column_desc = ComFunc.nvl(p.GetValue("column_desc"))
                };
            }).ToList();
            //系统自动填入参数的栏位
            var auto_fix_datatype = support_data_type.Where(w => BoolStd.IsNotBoolThen(w.is_auto_fix));
            var auto_fix_string = auto_fix_datatype.Select(d => ComFunc.nvl(((FrameDLRObject)d).GetValue("data_type"))).ToArray();
            var auto_fix_columns = columns.Where(p => auto_fix_string.Contains((string)ComFunc.nvl(p.data_type))).Select((p) =>
            {
                return new
                {
                    column_name = ComFunc.nvl(p.GetValue("column_name")),
                    data_type = ComFunc.nvl(p.GetValue("data_type")),
                    is_allow_empty = BoolStd.IsNotBoolThen(p.GetValue("is_allow_empty"), true),
                    precision = IntStd.IsNotIntThen(p.GetValue("precision"), 0)
                };
            }).ToList();
            var pkcolumns = columns.Where(p => BoolStd.IsNotBoolThen(p.is_pk, true) && p.data_type != "text").Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            var filetype_string = new string[] { "file", "picture" };
            var file_columns = columns.Where(p => filetype_string.Contains((string)ComFunc.nvl(p.data_type))).Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();

            //系统自动填入数据的栏位
            foreach (FrameDLRObject d in datalist)
            {
                foreach (var c in auto_fix_columns)
                {
                    if (ComFunc.nvl(d.GetValue(c.column_name)) == ""
                        && !c.is_allow_empty)
                    {
                        var dts = auto_fix_datatype.Where(w => w.data_type == c.data_type);
                        if (dts.Count() > 0)
                        {
                            d.SetValue(c.column_name, genAutoFixValue(ComFunc.nvl(dts.First().GetValue("auto_fix_method")), c.precision));
                        }
                    }
                }
            }
            var index = -1;
            //栏位检查
            foreach (FrameDLRObject d in datalist)
            {
                index++;
                foreach (var ne in notemptycolumns)
                {
                    if (ComFunc.nvl(d.GetValue(ne.column_name)) == "")
                    {
                        return new
                        {
                            code = "failed",
                            msg = $"{ne.column_desc}栏位不可为空",
                        };
                    }
                }

                if ((from t in d.Keys
                     join t2 in pkcolumns on t equals t2
                     select t).Count() != pkcolumns.Count)
                {
                    return new
                    {
                        code = "failed",
                        msg = "数据缺少PK栏位的数据，无法进行数据修改",
                    };
                }

                var newpkstring = "";
                var oripkstring = "";
                foreach (var pk in pkcolumns)
                {
                    if (ComFunc.nvl(d.GetValue(pk)) == "")
                    {
                        return new
                        {
                            code = "failed",
                            msg = "PK栏位不可为空",
                        };
                    }
                    newpkstring += $"{d.GetValue(pk)}|";
                    oripkstring += $"{oridatalist[index].GetValue(pk)}|";
                }

                if (newpkstring != oripkstring)
                {
                    return new
                    {
                        code = "failed",
                        msg = "PK栏位不可修改",
                    };
                }

                foreach (var key in d.Keys)
                {
                    if (!column_names.Contains(key))
                    {
                        return new
                        {
                            code = "failed",
                            msg = "数据栏位不匹配",
                        };
                    }
                    //数据类型检查
                    if (column_int.Contains(key) && ComFunc.nvl(d.GetValue(key)) != "" && !IntStd.IsInt(d.GetValue(key)))
                    {
                        return new
                        {
                            code = "failed",
                            msg = $"值({d.GetValue(key)})不是int类型",
                        };
                    }
                    if (column_numbric.Contains(key) && ComFunc.nvl(d.GetValue(key)) != "" && !DecimalStd.IsDecimal(d.GetValue(key)))
                    {
                        return new
                        {
                            code = "failed",
                            msg = $"({d.GetValue(key)})不是numberic类型",
                        };
                    }
                    if (column_bit.Contains(key) && ComFunc.nvl(d.GetValue(key)) != "" && !BoolStd.IsBool(d.GetValue(key)))
                    {
                        return new
                        {
                            code = "failed",
                            msg = $"({d.GetValue(key)})不是bit类型",
                        };
                    }
                    if (column_datetime.Contains(key) && ComFunc.nvl(d.GetValue(key)) != "" && !DateTimeStd.IsDateTime(d.GetValue(key)))
                    {
                        return new
                        {
                            code = "failed",
                            msg = $"({d.GetValue(key)})不是datetime类型",
                        };
                    }
                }
                //如果有file、picture类型，则需要做上传文件的检查
                if (is_use_local_upload)
                {
                    foreach (var c in file_columns)
                    {
                        var path = ComFunc.nvl(d.GetValue(c));
                        if (path != "" && !FileHelper.IsExists(path))
                        {
                            return new
                            {
                                code = "failed",
                                msg = "上传的文件不存在或上传失败"
                            };
                        }
                    }
                }

            }
            index = -1;
            var validCount = 0;
            //如果为datetime类型的栏位数据，必须针对空串和null做特殊处理，否则可能导致db会自动转换成1900-01-01
            var datetime_columns = columns.Where(w => w.data_type == "datetime").Select((p) => { return ComFunc.nvl(p.GetValue("column_name")); }).ToList();
            foreach (FrameDLRObject d in datalist)
            {
                index++;
                var isupdate = false;
                //判断是否数据没有变化，没有变化的则不做修改操作
                foreach (var k in d.Keys)
                {
                    if (d.GetValue(k) == null && oridatalist[index].GetValue(k) != null)
                    {
                        isupdate = true;
                        break;
                    }
                    if (!d.GetValue(k).Equals(oridatalist[index].GetValue(k)))
                    {
                        isupdate = true;
                        break;
                    }
                }
                if (!isupdate)
                {
                    continue;
                }
                //datetime类型栏位需要进行格式转化
                //用d的栏位进行过滤（d的栏位并非元数据中的所有栏位），防止DB中已有数据的栏位被错误更新
                foreach (var cc in column_datetime.Where(w => d.Keys.Contains(w)))
                {
                    d.SetValue(cc, DateTimeStd.IsDateTimeThen(d.GetValue(cc), "yyyy-MM-dd HH:mm:ss"));
                }
                //如果数据为空，则转化成dbnull，以免报类型转化异常
                //用d的栏位进行过滤（d的栏位并非元数据中的所有栏位），防止DB中已有数据的栏位被错误更新
                foreach (var cc in column_names.Where(w => d.Keys.Contains(w)))
                {
                    if (ComFunc.nvl(d.GetValue(cc)) == "")
                    {
                        d.SetValue(cc, DBNull.Value);
                    }
                }

                //原数据中的文件都要删除
                if (is_use_local_upload)
                {
                    var to_path = $"~/meta_data/{tablename}/";
                    foreach (var c in file_columns.Where(w => d.Keys.Contains(w)))
                    {
                        var file_path = ComFunc.nvl(oridatalist[index].GetValue(c));

                        var path = ComFunc.nvl(d.GetValue(c));
                        if (path != file_path)
                        {
                            //copy新文件到正式目录
                            delUploadFile(file_path);
                            var newpath = FileHelper.CopyUploadFileTo(path, to_path);
                            d.SetValue(c, newpath);
                        }
                    }
                }
                //增加操作者的信息
                d.SetValue("last_id", login_id);
                d.SetValue("last_ip", login_ip);
                d.SetValue("last_name", login_name);
                d.SetValue("last_time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //默认优先按照pk进行更新
                if (pkcolumns.Count > 0)
                {
                    FrameDLRObject w = FrameDLRObject.CreateInstance();
                    foreach (var ss in pkcolumns)
                    {
                        w.SetValue(ss, oridatalist[index].GetValue(ss));
                    }

                    DB.QuickUpdate(up, tablename, d, w);
                }
                else
                {
                    //如果某个栏位是text类型或为空，则不可参与where条件
                    var text_columns = columns.Where(p => p.data_type == "text");
                    var w = (FrameDLRObject)oridatalist[index].Clone();
                    if (text_columns.Count() > 0)
                    {
                        foreach (var tc in text_columns)
                        {
                            w.Remove(ComFunc.nvl(tc.GetValue("column_name")));
                        }
                    }
                    foreach (var key in w.Keys)
                    {
                        if (ComFunc.nvl(w.GetValue(key)) == "")
                        {
                            w.Remove(key);
                        }
                    }
                    DB.QuickUpdate(up, tablename, d, w, true);
                }
                //记录异动记录
                var updatedatacontent = d.ToJSONString();
                DB.QuickInsert(up, $"{tablename}_change_log", new
                {
                    UID = Guid.NewGuid().ToString(),
                    ActionType = "Update",
                    OriDataContent = oridatalist[index].ToJSONString(),
                    ChangeDataContent = updatedatacontent,
                    FromSourceID = "DCAP",
                    FromSourceName = "数据中心",
                    add_id = login_id,
                    add_name = login_name,
                    add_ip = login_ip,
                    add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    last_id = login_id,
                    last_name = login_name,
                    last_ip = login_ip,
                    last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });

                validCount++;
            }
            return new
            {
                code = "success",
                msg = $"操作成功,更新有效数据{validCount}笔",
            };
        }
        [EWRARoute("post", "/metadataset/upload")]
        [EWRAEmptyValid("file_name,file_length,file_content")]
        [EWRARouteDesc("上传文件")]
        [EWRAAddInput("file_name", "string", "文件名称", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("file_length", "double", "文件大小", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("file_content", "string", "文件内容，base64加密", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息',
upload_path:'上传成功后返回的文档路径'
}")]
        public object Upload()
        {
            string file_name = ComFunc.nvl(PostDataD.file_name);
            long file_length = Int64Std.IsNotInt64Then(PostDataD.file_length);
            string file_content = ComFunc.nvl(PostDataD.file_content).Replace(" ", "+");
            var is_keep_filename = BoolStd.IsNotBoolThen(PostDataD.is_keep_filename);
            return FileHelper.DoUploadFile(file_name, file_length, file_content, is_keep_filename);
        }
        [EWRARoute("get", "/metadataset/dl")]
        [EWRAEmptyValid("path")]
        [EWRARouteDesc("下载文件")]
        [EWRAAddInput("path", "string", "文件路径，使用urlcode编码", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息',
filetype:'文件的content-type类型'
filename:'文件名称',
filelength:'文件长度',
file:'文件内容，采用base64加密'
}")]
        public object DownLoad()
        {
            SetCacheEnable(false);
            string path = ComFunc.UrlDecode(QueryStringD.path);
            return FileHelper.DoDownLoad(path);
        }
        /// <summary>
        /// 删除上传的文件
        /// </summary>
        /// <param name="file_path">~表示根目录</param>
        /// <returns></returns>
        bool delUploadFile(string file_path)
        {
            return FileHelper.DelUploadFile(file_path);
        }

        private Dictionary<string, dynamic> getTableColumnsMap(UnitParameter up)
        {
            var s = from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                    join t2 in DB.LamdaTable(up, "EXTEND_METADATA_COLUMNS", "b") on t.metauid equals t2.metauid
                    join t3 in DB.LamdaTable(up, "EXTEND_METADATA_DATATYPE", "c").LeftJoin() on t2.MetaDataType equals t3.DataType
                    where t.iscreated == 1
                    select new
                    {
                        uid = t.metauid,
                        table_name = t.metaname,
                        column_name = t2.metacolumnname,
                        column_desc = t2.metacolumndesc,
                        ui_type = t3.UI_Type
                    };

            var list = s.GetQueryList(up);
            var dic_list = (from t in DB.LamdaTable(up, "EXTEND_DICTIONARY_TABLE", "a")
                            where t.iscreated == 1
                            select new
                            {
                                uid = t.dic_uid,
                                table_name = t.dic_name,
                                is_tree = t.istree
                            }).GetQueryList(up);

            foreach (FrameDLRObject item in dic_list)
            {
                var istree = BoolStd.IsNotBoolThen(item.GetValue("is_tree"));
                item.Remove("is_tree");
                var newitem = (FrameDLRObject)item.Clone();
                newitem.SetValue("column_name", "code");
                newitem.SetValue("column_desc", "编号");
                newitem.SetValue("ui_type", "Input");
                list.Add(newitem);
                newitem = (FrameDLRObject)item.Clone();
                newitem.SetValue("column_name", "value");
                newitem.SetValue("column_desc", "值");
                newitem.SetValue("ui_type", "Input");
                list.Add(newitem);
                if (istree)
                {
                    newitem = (FrameDLRObject)item.Clone();
                    newitem.SetValue("column_name", "level");
                    newitem.SetValue("column_desc", "层级");
                    newitem.SetValue("ui_type", "Input");
                    list.Add(newitem);

                    newitem = (FrameDLRObject)item.Clone();
                    newitem.SetValue("column_name", "p_code");
                    newitem.SetValue("column_desc", "父编号");
                    newitem.SetValue("ui_type", "Input");
                    list.Add(newitem);
                }
            }

            var rtn = new Dictionary<string, dynamic>();
            foreach (dynamic item in list)
            {
                var key = $"{item.table_name}.{item.column_name}";
                rtn.Add(key.ToLower(), item);
            }
            return rtn;
        }
        /// <summary>
        /// 根据自动填充Method的名称设定，生成对应的值
        /// </summary>
        /// <param name="fix_mathod_name">自动填充Method名称</param>
        /// <param name="length">生成值的长度，Random和Random_Code有效</param>
        /// <returns></returns>
        object genAutoFixValue(string fix_mathod_name, int length = 0)
        {
            switch (fix_mathod_name)
            {
                case "GUID":
                    return Guid.NewGuid().ToString();
                case "Random":
                    return ComFunc.RandomCode(length);
                case "Random_Code":
                    return ComFunc.RandomString(length, true, true, true, false);
                case "Now":
                    return DateTime.Now.ToString("yyyy-MM-dd");
                default:
                    return fix_mathod_name;
            }
        }
    }
}
