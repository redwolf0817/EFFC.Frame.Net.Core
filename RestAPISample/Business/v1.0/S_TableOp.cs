using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using RestAPISample.Common;
using EFFC.Frame.Net.Unit.DB.Parameters;

namespace RestAPISample.Business.v1._0
{
    public class S_TableOp:MyRestLogic
    {
        [EWRARouteDesc("列表查询")]
        [EWRARoute("get", "/s_table/{id}")]
        [EWRAAddInput("id", "string", "动态功能的uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("limit", "int", "每页笔数", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "跳到指定页数", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("xxx", "string", "动态功能中设定的查询条件参数", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAOutputDesc("返回结果", @"如果是授权无效的情况下，返回400错，其它情况的返回结果集（状态为200）如下：{
code:'success-成功，failed-失败',
msg:'提示信息',
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[数据集]
}")]
        public object QueryList(string id)
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable(up, "EXTEND_SINGLETABLE_OP", "a")
                    where (t.s_uid == id || t.S_Name == id)
                    select t;
            var list = s.GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "设定资料不存在"
                };
            }
            dynamic info = list.First();
           
            var metainfo = (from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                            where t.MetaUID == info.MetaUID && t.IsCreated == 1
                            select new
                            {
                                table_name = t.MetaName
                            }).GetQueryList(up);
            if (metainfo.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "元数据表不存在，无法继续进行操作"
                };
            }
            var table_name = ComFunc.nvl(metainfo.First().GetValue("table_name"));
            //组织查询表达式
            FrameDLRObject express = FrameDLRObject.CreateInstance($@"{{
$acttype : 'QueryByPage',
$orderby : 'sort_no',
$table:'{table_name}',
sort_no:true
                }}", EFFC.Frame.Net.Base.Constants.FrameDLRFlags.SensitiveCase);
            #region 获取查询条件信息
            var conditionlist = (from t in DB.LamdaTable(up, "EXTEND_SINGLETABLE_OP_CONDITIONS", "a")
                                 join t2 in DB.LamdaTable(up, "EXTEND_METADATA_COLUMNS", "b").LeftJoin() on t.S_ColumnName equals t2.MetaColumnName
                                 where t.S_UID == info.S_UID && t2.MetaUID == info.MetaUID
                                 select new
                                 {
                                     op = t.S_ConditionOP,
                                     column_name = t.S_ColumnName,
                                     parameter_name = t.S_Parameter,
                                     data_type = t2.MetaDataType
                                 }).GetQueryList(up);
            FrameDLRObject where_express = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            foreach (dynamic citem in conditionlist)
            {
                if (ComFunc.nvl(QueryString[citem.parameter_name]) == "") continue;
                var value = ConvertConditionValue(citem.data_type, ComFunc.nvl(QueryString[citem.parameter_name]));
                FrameDLRObject exp = where_express.GetValue(citem.column_name);
                if (exp == null)
                {
                    exp = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                }
                switch (citem.op)
                {
                    case "like":
                        exp.SetValue("$like", value);
                        where_express.SetValue(citem.column_name, exp);
                        break;
                    case "=":
                        where_express.SetValue(citem.column_name, value);
                        break;
                    case ">":
                        exp.SetValue("$gt", value);
                        where_express.SetValue(citem.column_name, exp);
                        break;
                    case ">=":
                        exp.SetValue("$gte", value);
                        where_express.SetValue(citem.column_name, exp);
                        break;
                    case "<":
                        exp.SetValue("$lt", value);
                        where_express.SetValue(citem.column_name, exp);
                        break;
                    case "<=":
                        exp.SetValue("$lte", value);
                        where_express.SetValue(citem.column_name, exp);
                        break;
                }
            }
            if (where_express.Items.Count > 0)
            {
                express.SetValue("$where", where_express);
            }
            #endregion
            #region 组织select栏位
            var columns = (from t in DB.LamdaTable(up, "EXTEND_SINGLETABLE_OP_COLUMNS", "a")
                           where t.S_UID == info.S_UID
                           select new
                           {
                               column_name = t.S_ColumnName
                           }).GetQueryList(up);
            foreach (dynamic item in columns)
            {
                express.SetValue(item.column_name, true);
            }
            #endregion
            var result = DB.Excute(up, express, true);

            return new
            {
                code = "success",
                msg = "",
                total_count = result.TotalRow,
                page = result.CurrentPage,
                total_page = result.TotalPage,
                limit = result.Count_Of_OnePage,
                data = result.QueryData<FrameDLRObject>().Select((d) =>
                {
                    FrameDLRObject dobj = (FrameDLRObject)d;
                    dobj.Remove("RowNumber");
                    //时间类型需要转string
                    foreach (var item in dobj.Items)
                    {
                        if (item.Value is DateTime)
                        {
                            dobj.SetValue(item.Key, DateTimeStd.IsDateTimeThen(item.Value));
                        }
                    }
                    return dobj;
                })
            };
        }

        private object ConvertConditionValue(string datatype, object v)
        {
            if (new string[] { "datetime" }.Contains(datatype.ToLower()))
            {
                return DateTimeStd.IsDateTimeThen(v, "yyyy-MM-dd HH:mm:ss");
            }
            else if (new string[] { "bit" }.Contains(datatype.ToLower()))
            {
                return BoolStd.ConvertTo(v, 1, 0);
            }
            else if (new string[] { "text", "picture", "file" }.Contains(datatype.ToLower()))
            {
                return "";
            }
            else
            {
                return v;
            }
        }

        private bool IsAuth(string action_auth)
        {
            if (action_auth == "") return true;
            var authfunctions = ComFunc.nvl(TokenPayLoad["Auth_Actions"]);
            return authfunctions.Split(',').Contains(action_auth);
        }

        [EWRARoute("post", "/s_table/event/{id}/eval")]
        [EWRARouteDesc("获取事件资料-用于单表操作")]
        [EWRAAddInput("this_data", "string", @"页面上当前编辑的行资料，先采用url编码，再采用base64编码，原数据格式:
{
    column_name1:'值',
    column_name2:'值',
    .....
    column_nameN:'值',
}", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息'
}")]
        object CallEvent(string id)
        {
            var data_str = ComFunc.nvl(PostDataD.this_data).Replace(" ", "+");
            var this_data_base64str = ComFunc.IsBase64Then(data_str);
            string this_data_str = ComFunc.UrlDecode(this_data_base64str);
            var this_data = FrameDLRObject.IsJsonThen(this_data_str);
            var up = DB.NewDBUnitParameter();
            return DoCallEvent(up, id, this_data);
        }
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="up"></param>
        /// <param name="id"></param>
        /// <param name="this_data"></param>
        /// <returns></returns>
        private object DoCallEvent(UnitParameter up, string id, object this_data)
        {
            var list = (from t in DB.LamdaTable(up, "EXTEND_EVENTS", "a")
                        where t.EventNo == id
                        select new
                        {
                            no = t.EventNo,
                            name = t.EventName,
                            category = t.EventCategory,
                            type = t.EventType,
                            at = t.EventAt,
                            parent = t.
                            desc = t.EventDesc,
                            parameters = t.EventParameters
                        }).GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "执行失败：事件设定不存在"
                };
            }
            dynamic info = list.First();
            var event_call = new EventsCall();
            var parameters = FrameDLRObject.IsJsonThen(info.parameters, null, FrameDLRFlags.SensitiveCase);
            if (parameters == null)
            {
                return new
                {
                    code = "failed",
                    msg = "执行失败：确少执行参数"
                };
            }
            string url = ComFunc.nvl(parameters.RequestUrl);
            string method = ComFunc.nvl(parameters.RequestMethod);
            FrameDLRObject query_string = ComFunc.nvl(parameters.RequestQueryString) == "" ? null : parameters.RequestQueryString;
            FrameDLRObject header = ComFunc.nvl(parameters.RequestHeader) == "" ? null : parameters.RequestHeaders;
            FrameDLRObject post_data = ComFunc.nvl(parameters.RequestPostData) == "" ? null : parameters.RequestPostData;
            var call_context = new EventsCall.EventsCallContext();
            call_context.RowData = ComFunc.Base64Code(FrameDLRObject.CreateInstance(this_data, FrameDLRFlags.SensitiveCase).tojsonstring());
            call_context.Login_ID = TokenPayLoad.ID;
            call_context.Login_Name = ComFunc.nvl(TokenPayLoad["username"]);
            if (url.StartsWith("~"))
            {
                return CallLocalLogic(url.Replace("~", ""), method, header, query_string, post_data);
            }
            else
            {
                if (query_string != null)
                {
                    var qs = "";
                    foreach (var item in query_string.Items)
                    {
                        qs += $"{item.Key}={ComFunc.UrlEncode(item.Value)}&";
                    }
                    qs = qs.Length > 0 ? qs.Substring(0, qs.Length - 1) : "";
                    if (url.IndexOf("?") > 0)
                    {

                        url += $"&{qs}";
                    }
                    else
                    {
                        url += $"?{qs}";
                    }
                }
                string result = event_call.Send(url, method, call_context, header, post_data);
                if (result == null)
                {
                    return new
                    {
                        code = "failed",
                        msg = "执行失败：返回结果为空"
                    };
                }
                else if (result.StartsWith("Failed:"))
                {
                    return new
                    {
                        code = "failed",
                        msg = $"执行失败：{result}"
                    };
                }
                else
                {
                    return FrameDLRObject.IsJsonThen(result, null, FrameDLRFlags.SensitiveCase);
                }
            }
        }
    }
}
