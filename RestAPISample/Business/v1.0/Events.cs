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

namespace RestAPISample.Business.v1._0
{
    public class Events : MyRestLogic
    {
        [EWRARouteDesc("获取所有的自定义事件资料")]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息',
data:[{
    no:'事件编号',
    name:'事件名称',
    category:'事件分类，如下：Customerize-自定义',
    type:'事件类型，Button-按钮事件，Link-链接事件,RowButton-单笔资料行按钮事件,RowLink-单笔资料行链接事件',
    at:'事件触发时机，有以下几种：Before-在已有执行逻辑之前，After-在已有执行逻辑之后,Replace-替换已有事件，New-全新事件',
    parent:'父事件编号，at为New的时候不可用有父事件， 如果有则是针对父事件的补充（这种事件不会出现在界面上），如果没有则是一个新的事件，只支持System分类的事件做父事件',
    desc:'事件描述',
    request_parameters:'事件参数设定，采用json格式，如下{RequestUrl:'请求的Url，如果是本系统的服务，则使用~表示，远程的请填写完整的url地址',RequestMethod:'请求的Method',RequestHeader:{key1:'value1',key2:'value2'...},//请求的自定义Header信息RequestQueryString:{key1:'value1',key2:'value2'...},//请求的Querystring参数，如果值是一种变量则使用@标记RequestPostData:{key1:'value1',key2:'value2'...}//请求的自定义PostData，如果值是一种变量则使用@标记，@后面接变量名称}'
}]
}")]
        public new object get()
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var list = (from t in DB.LamdaTable(up, "EXTEND_EVENTS", "a")
                        where t.EventCategory == "Customerize"
                        select new
                        {
                            no = t.EventNo,
                            name = t.EventName,
                            category = t.EventCategory,
                            type = t.EventType,
                            at = t.EventAt,
                            parent = t.ParentEventNo,
                            desc = t.EventDesc,
                            request_parameters = t.EventParameters
                        }).GetQueryList(up);
            return new
            {
                code = "success",
                msg = "",
                data = list
            };
        }
        [EWRARouteDesc("新增事件定义")]
        [EWRAAddInput("data", "array", @"待新增的数据列表，格式:
[
{
    no:'事件编号',
    name:'事件名称',
    type:'事件类型，Button-按钮事件，Link-链接事件,RowButton-单笔资料行按钮事件,RowLink-单笔资料行链接事件',
    at:'事件触发时机，有以下几种：Before-在已有执行逻辑之前，After-在已有执行逻辑之后,Replace-替换已有事件，New-全新事件',
    parent:'父事件编号，at为New的时候不可用有父事件，如果有则是针对父事件的补充（这种事件不会出现在界面上），如果没有则是一个新的事件，只支持System分类的事件做父事件',
    desc:'事件描述'，
    request_parameters:'事件参数设定，相关说明参见《事件扩展设计说明.docx》'
}
]
", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息'
}")]
        public override object post()
        {
            var data = PostDataD.data;

            if (!(data is IEnumerable<object>))
                return new
                {
                    code = "failed",
                    msg = "参数格式不正确",
                };
            var datalist = (IEnumerable<object>)data;
            foreach (dynamic item in datalist)
            {
                if (ComFunc.nvl(item.no) == "")
                    return new
                    {
                        code = "failed",
                        msg = "编号不可为空"
                    };
                if (ComFunc.nvl(item.name) == "")
                    return new
                    {
                        code = "failed",
                        msg = "名称不可为空"
                    };
                if (ComFunc.nvl(item.type) == "")
                    return new
                    {
                        code = "failed",
                        msg = "事件类型不可为空"
                    };
                if (ComFunc.nvl(item.at) == "")
                    return new
                    {
                        code = "failed",
                        msg = "事件触发时机不可为空"
                    };
                if (ComFunc.nvl(item.at) == "New")
                {
                    if (ComFunc.nvl(item.parent) != "")
                    {
                        return new
                        {
                            code = "failed",
                            msg = "触发时机为“全新事件”的时候不可以有父事件"
                        };
                    }
                }
                else
                {
                    if (ComFunc.nvl(item.parent) == "")
                    {
                        return new
                        {
                            code = "failed",
                            msg = "触发时机为不为“全新事件”的时候必须设定父事件"
                        };
                    }
                }
                if (ComFunc.nvl(item.request_parameters) != "")
                {
                    var dobj = FrameDLRObject.IsJsonThen(ComFunc.nvl(item.request_parameters), null, FrameDLRFlags.SensitiveCase);
                    if (dobj == null)
                        return new
                        {
                            code = "failed",
                            msg = "参数设定格式不正确"
                        };
                    else
                    {
                        if (ComFunc.nvl(dobj.RequestUrl) == "")
                        {
                            return new
                            {
                                code = "failed",
                                msg = "参数设定缺少请求URL"
                            };
                        }
                        if (ComFunc.nvl(dobj.RequestMethod) == "")
                        {
                            return new
                            {
                                code = "failed",
                                msg = "参数设定缺少请求Method"
                            };
                        }
                    }
                }
            }
            BeginTrans();
            var up = DB.NewDBUnitParameter();
            var p_list = (from t in DB.LamdaTable(up, "EXTEND_EVENTS", "a")
                          where t.EventCategory == "System"
                          select t).GetQueryList(up);
            var exists_list = (from t in DB.LamdaTable(up, "EXTEND_EVENTS", "a")
                               where t.EventCategory == "Customerize"
                               select new { t.EventNo }).GetQueryList(up);
            var allow_p_nos = p_list.Select(d => ComFunc.nvl(d.GetValue("EventNo"))).ToArray();
            var exists_nos = exists_list.Select(d => ComFunc.nvl(d.GetValue("EventNo"))).ToArray();
            foreach (dynamic item in datalist)
            {
                if (ComFunc.nvl(item.parent) != "" && !allow_p_nos.Contains((string)item.parent))
                {
                    return new
                    {
                        code = "failed",
                        msg = $"{item.no}该编号的父事件不被支持"
                    };
                }
                if (exists_nos.Contains((string)item.no))
                {
                    return new
                    {
                        code = "failed",
                        msg = $"{item.no}该编号的事件已存在"
                    };
                }
            }

            foreach (dynamic item in datalist)
            {
                item.add_id = TokenPayLoad.ID;
                item.add_ip = ClientInfo.IP;
                item.add_name = ComFunc.nvl(TokenPayLoad["username"]);
                item.add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                item.last_id = ClientInfo.IP;
                item.last_ip = ComFunc.nvl(TokenPayLoad["username"]);
                item.last_name = TokenPayLoad.ID;
                item.last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                DB.QuickInsert(up, "EXTEND_EVENTS", new
                {
                    EventNo = item.no,
                    EventName = item.name,
                    EventCategory = "Customerize",
                    EventType = item.type,
                    EventAt = item.at,
                    ParentEventNo = item.parent,
                    EventDesc = item.desc,
                    EventParameters = item.request_parameters,
                    add_id = TokenPayLoad.ID,
                    add_ip = ClientInfo.IP,
                    add_name = ComFunc.nvl(TokenPayLoad["username"]),
                    add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    last_id = ClientInfo.IP,
                    last_ip = ComFunc.nvl(TokenPayLoad["username"]),
                    last_name = TokenPayLoad.ID,
                    last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }

            CommitTrans();
            return new
            {
                code = "success",
                msg = "操作成功"
            };
        }
        [EWRARouteDesc("修改资料")]
        [EWRARoute("patch", "/events")]
        [EWRAAddInput("data", "array", @"待新增的数据列表，格式:
[
{
    no:'事件编号',
    name:'事件名称',
    type:'事件类型，Button-按钮事件，Link-链接事件,RowButton-单笔资料行按钮事件,RowLink-单笔资料行链接事件',
    at:'事件触发时机，有以下几种：Before-在已有执行逻辑之前，After-在已有执行逻辑之后,Replace-替换已有事件，New-全新事件',
    parent:'父事件编号，at为New的时候不可用有父事件，如果有则是针对父事件的补充（这种事件不会出现在界面上），如果没有则是一个新的事件，只支持System分类的事件做父事件',
    desc:'事件描述'，
    request_parameters:'事件参数设定，相关说明参见《事件扩展设计说明.docx》'
}
]
", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息'
}")]
        public object Update()
        {
            var data = PostDataD.data;

            if (!(data is IEnumerable<object>))
                return new
                {
                    code = "failed",
                    msg = "参数格式不正确",
                };
            var datalist = (IEnumerable<object>)data;
            var nos = "";
            foreach (dynamic item in datalist)
            {
                if (ComFunc.nvl(item.no) == "")
                    return new
                    {
                        code = "failed",
                        msg = "编号不可为空"
                    };
                if (ComFunc.nvl(item.name) == "")
                    return new
                    {
                        code = "failed",
                        msg = "名称不可为空"
                    };
                if (ComFunc.nvl(item.type) == "")
                    return new
                    {
                        code = "failed",
                        msg = "事件类型不可为空"
                    };
                if (ComFunc.nvl(item.at) == "")
                    return new
                    {
                        code = "failed",
                        msg = "事件触发时机不可为空"
                    };
                if (ComFunc.nvl(item.at) == "New")
                {
                    if (ComFunc.nvl(item.parent) != "")
                    {
                        return new
                        {
                            code = "failed",
                            msg = "触发时机为“全新事件”的时候不可以有父事件"
                        };
                    }
                }
                else
                {
                    if (ComFunc.nvl(item.parent) == "")
                    {
                        return new
                        {
                            code = "failed",
                            msg = "触发时机为不为“全新事件”的时候必须设定父事件"
                        };
                    }
                }
                if (ComFunc.nvl(item.request_parameters) != "")
                {
                    var dobj = FrameDLRObject.IsJsonThen(ComFunc.nvl(item.request_parameters), null, FrameDLRFlags.SensitiveCase);
                    if (dobj == null)
                        return new
                        {
                            code = "failed",
                            msg = "参数设定格式不正确"
                        };
                    else
                    {
                        if (ComFunc.nvl(dobj.RequestUrl) == "")
                        {
                            return new
                            {
                                code = "failed",
                                msg = "参数设定缺少请求URL"
                            };
                        }
                        if (ComFunc.nvl(dobj.RequestMethod) == "")
                        {
                            return new
                            {
                                code = "failed",
                                msg = "参数设定缺少请求Method"
                            };
                        }
                    }
                }

                nos += item.no + ",";
            }
            BeginTrans();
            var up = DB.NewDBUnitParameter();
            var p_list = (from t in DB.LamdaTable(up, "EXTEND_EVENTS", "a")
                          where t.EventCategory == "System"
                          select t).GetQueryList(up);
            var exists_list = (from t in DB.LamdaTable(up, "EXTEND_EVENTS", "a")
                               where t.EventCategory == "Customerize" && t.EventNo.without(nos)
                               select new { t.EventNo }).GetQueryList(up);
            var allow_p_nos = p_list.Select(d => ComFunc.nvl(d.GetValue("EventNo"))).ToArray();
            var exists_nos = exists_list.Select(d => ComFunc.nvl(d.GetValue("EventNo"))).ToArray();
            foreach (dynamic item in datalist)
            {
                if (ComFunc.nvl(item.parent) != "" && !allow_p_nos.Contains((string)item.parent))
                {
                    return new
                    {
                        code = "failed",
                        msg = $"{item.no}该编号的父事件不被支持"
                    };
                }
                if (exists_nos.Contains((string)item.no))
                {
                    return new
                    {
                        code = "failed",
                        msg = $"{item.no}该编号的事件已存在"
                    };
                }
            }

            foreach (dynamic item in datalist)
            {
                item.add_id = TokenPayLoad.ID;
                item.add_ip = ClientInfo.IP;
                item.add_name = ComFunc.nvl(TokenPayLoad["username"]);
                item.add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                item.last_id = ClientInfo.IP;
                item.last_ip = ComFunc.nvl(TokenPayLoad["username"]);
                item.last_name = TokenPayLoad.ID;
                item.last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                DB.QuickUpdate(up, "EXTEND_EVENTS", new
                {
                    EventName = item.name,
                    EventCategory = "Customerize",
                    EventType = item.type,
                    EventAt = item.at,
                    ParentEventNo = item.parent,
                    EventDesc = item.desc,
                    EventParameters = item.request_parameters,
                    last_id = TokenPayLoad.ID,
                    last_ip = ClientInfo.IP,
                    last_name = ComFunc.nvl(TokenPayLoad["username"]),
                    last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                }, new
                {
                    EventNo = item.no
                });
            }

            CommitTrans();
            return new
            {
                code = "success",
                msg = "操作成功"
            };
        }
        [EWRARouteDesc("删除资料")]
        public override bool delete(string id)
        {
            var up = DB.NewDBUnitParameter();
            BeginTrans();
            var exists_list = (from t in DB.LamdaTable(up, "EXTEND_EVENTS", "a")
                               where t.EventNo == id
                               select t).GetQueryList(up);
            DB.QuickDelete(up, "EXTEND_EVENTS", new
            {
                EventNo = id
            });
            CommitTrans();
            return true;
        }
        [EWRARoute("get", "/events/parents")]
        [EWRARouteDesc("获取可以作为父事件的事件资料")]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息',
data:[{
    no:'事件编号',
    name:'事件名称',
    category:'事件分类，如下：System-系统事件',
    desc:'事件描述'
}]
}")]
        object GetParent()
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var list = (from t in DB.LamdaTable(up, "EXTEND_EVENTS", "a")
                        where t.EventCategory == "System"
                        select new
                        {
                            no = t.EventNo,
                            name = t.EventName,
                            category = t.EventCategory,
                            desc = t.EventDesc
                        }).GetQueryList(up);
            return new
            {
                code = "success",
                msg = "",
                data = list
            };
        }
        [EWRARoute("get", "/events/s_table")]
        [EWRARouteDesc("获取事件资料-用于单表操作")]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息',
events:[{
    no:'事件编号',
    name:'事件名称',
    category:'事件分类，相关说明参见《事件扩展设计说明.docx》',
    type:'事件类型，相关说明参见《事件扩展设计说明.docx》',
    at:'事件触发时机，相关说明参见《事件扩展设计说明.docx》',
    desc:'事件描述'
}],
row_events:[{
    no:'事件编号',
    name:'事件名称',
    category:'事件分类，相关说明参见《事件扩展设计说明.docx》',
    type:'事件类型，相关说明参见《事件扩展设计说明.docx》',
    at:'事件触发时机，相关说明参见《事件扩展设计说明.docx》',
    desc:'事件描述'
}]
}")]
        object GetEvents4S_Table()
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var list = (from t in DB.LamdaTable(up, "EXTEND_EVENTS", "a")
                        where t.EventCategory == "Customerize"
                        select new
                        {
                            no = t.EventNo,
                            name = t.EventName,
                            category = t.EventCategory,
                            type = t.EventType,
                            at = t.EventAt,
                            desc = t.EventDesc
                        }).GetQueryList(up);
            return new
            {
                code = "success",
                msg = "",
                events = list.Where(w => !ComFunc.nvl(w.type).StartsWith("Row")),
                row_events = list.Where(w => ComFunc.nvl(w.type).StartsWith("Row"))
            };
        }
    }
}
