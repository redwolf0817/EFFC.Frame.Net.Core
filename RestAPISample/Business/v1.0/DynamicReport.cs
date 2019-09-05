using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using EFFC.Frame.Net.Base.Data.Base;
using System.IO;
using EFFC.Frame.Net.Base.Constants;

namespace RestAPISample.Business.v1._0
{
    public class DynamicReport : MyRestLogic
    {
        [EWRARouteDesc("获取所有报表模板列表")]
        [EWRAOutputDesc("返回结果", @"{
result:[{
    ReportUID:'UID',
    ReportName:'报表名称'，
    ReportDesc:'报表描述',
    IsActive:'是否激活'
}]
}")]
        public override List<object> get()
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var list = (from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE", "a")
                        select new
                        {
                            t.ReportUID,
                            t.ReportName,
                            t.ReportDesc,
                            t.IsActive
                        }).GetQueryList(up);

            return list.Select((d) =>
            {
                d.IsActive = BoolStd.IsNotBoolThen(d.IsActive);
                return d;
            }).ToList<object>();
        }
        [EWRARouteDesc("获取指定报表模板的明细数据")]
        [EWRAAddInput("id", "string", "模板uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息',
ReportUID:'UID',
ReportName:'报表名称'，
ReportDesc:'报表描述',
IsActive:'是否激活',
Tables:[
    {
        TableName:'关联表名',
        IsMain:'是否为主表',
        RefColumns:['关联栏位描述，格式:{栏位名称}={表名}.{栏位名称}']
    }
    ],
ShowColumns:[{
        TableName:'表名',
        Columns:[{
            ColumnName:'栏位名称',
            ColumnDisplayDesc:'报表标题名称'
        }]
    }]
}]
}")]
        public override object get(string id)
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE", "a")
                    join t2 in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE_TABLES", "b") on t.reportuid equals t2.reportuid
                    join t3 in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE_COLUMNS", "c") on new { t2.reportuid, t2.tablename } equals new { t3.reportuid, t3.tablename }
                    where t.reportuid == id
                    select t;
            if (!s.IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "报表不存在或未激活"
                };
            }

            dynamic info = (from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE", "a")
                            where t.reportuid == id
                            select t).GetQueryList(up).First();

            var tables = (from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE_TABLES", "a")
                          where t.reportuid == id
                          select new
                          {
                              t.TableName,
                              t.IsMain,
                              t.ReferenceColumns
                          }).GetQueryList(up);
            foreach (var item in tables)
            {
                var c = ComFunc.nvl(item.GetValue("ReferenceColumns"));
                var clist = new List<object>();
                if (c != "")
                {
                    clist.AddRange(c.Split(','));
                }
                item.SetValue("RefColumns", clist);
                item.Remove("ReferenceColumns");
                item.SetValue("IsMain", BoolStd.IsNotBoolThen(item.GetValue("IsMain")));
            }

            var columns = (from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE_COLUMNS", "a")
                           where t.reportuid == id
                           select new
                           {
                               t.TableName,
                               t.ShowColumnName,
                               t.ShowColumnDesc
                           }).GetQueryList(up);

            return new
            {
                code = "success",
                msg = "",
                ReportUID = info.reportuid,
                ReportName = info.reportname,
                ReportDesc = info.reportdesc,
                IsActive = BoolStd.IsNotBoolThen(info.IsActive),
                Tables = tables,
                ShowColumns = from t in columns
                              group t by new
                              {
                                  TableName = t.GetValue("TableName")
                              } into g
                              select new
                              {
                                  TableName = g.First().GetValue("TableName"),
                                  Columns = from tt in g
                                            select new
                                            {
                                                ColumnName = tt.GetValue("ShowColumnName"),
                                                ColumnDisplayDesc = tt.GetValue("ShowColumnDesc")
                                            }
                              }
            };
        }
        [EWRAEmptyValid("report_name")]
        [EWRARouteDesc("新增一个报表模板的设定")]
        [EWRAAddInput("report_name", "string", "报表名称", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("report_desc", "string", "报表描述", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAAddInput("tables", "array", @"关联表信息，格式:
[
{
   table_name:'关联表名,每个table只能加一次，不可出现重复的表名',
   is_main:'是否为主表,为false时，ref_columns才可以输入',
   ref_columns:['关联栏位描述，为空的时候该表则是主表，否则为从表，主表只能有一个，格式:{栏位名称}={表名}.{栏位名称}']
}
]", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("columns", "array", @"报表栏位信息，格式:
[
{
   table_name:'关联表名,不可为空',
   column_name:'栏位名称,不可为空',
   column_display:'表头显示名称,不可为空'
}
]", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息'
}")]
        public override object post()
        {
            var report_name = ComFunc.nvl(PostDataD.report_name);
            var report_desc = ComFunc.nvl(PostDataD.report_desc);
            var tables = PostDataD.tables;
            var columns = PostDataD.columns;

            if (tables == null || !(tables is IEnumerable<object>))
            {
                return new
                {
                    code = "failed",
                    msg = "关联表信息不可为空或格式不正确"
                };
            }
            if (columns == null || !(columns is IEnumerable<object>))
            {
                return new
                {
                    code = "failed",
                    msg = "报表栏位信息不可为空或格式不正确"
                };
            }
            var tablelist = (IEnumerable<object>)tables;
            var columnlist = (IEnumerable<object>)columns;
            var mainicount = 0;
            var pre_table_name = "";
            foreach (dynamic item in tablelist)
            {
                if (ComFunc.nvl(item.table_name) == "")
                {
                    return new
                    {
                        code = "failed",
                        msg = "关联表信息数据格式不正确"
                    };
                }
                if (ComFunc.nvl(item.table_name) == pre_table_name)
                {
                    return new
                    {
                        code = "failed",
                        msg = "不可有重复的表"
                    };
                }
                if (item.ref_coumns != null)
                {
                    if (!(item.ref_columns is IEnumerable<object>))
                    {
                        return new
                        {
                            code = "failed",
                            msg = "关联表信息数据格式不正确"
                        };
                    }
                }

                if (BoolStd.IsNotBoolThen(item.is_main))
                {
                    mainicount++;
                }
                pre_table_name = ComFunc.nvl(item.table_name);
            }
            if (mainicount > 1)
            {
                return new
                {
                    code = "failed",
                    msg = "关联表只可以有一个主表(即关联栏位描述为空)"
                };
            }


            foreach (dynamic item in columnlist)
            {
                if (ComFunc.nvl(item.table_name) == ""
                    || ComFunc.nvl(item.column_name) == ""
                    || ComFunc.nvl(item.column_display) == "")
                {
                    return new
                    {
                        code = "failed",
                        msg = "关联表信息数据格式不正确"
                    };
                }
            }
            var up = DB.NewDBUnitParameter();
            BeginTrans();
            if ((from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE", "a")
                 where t.ReportName == report_name
                 select t).IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "报表名称已存在"
                };
            }
            var jsonexpress = buildQueryJSON(tablelist, columnlist);
            if (jsonexpress == null)
            {
                return new
                {
                    code = "failed",
                    msg = "报表栏位信息或关联表信息不正确，无法生产报表表达式"
                };
            }

            var uid = Guid.NewGuid().ToString();

            DB.QuickInsert(up, "EXTEND_REPORT_TEMPLATE", new
            {
                ReportUID = uid,
                ReportName = report_name,
                ReportDesc = report_desc,
                QueryJSON = jsonexpress.ToJSONString(),
                IsActive = 1,
                add_id = TokenPayLoad.ID,
                add_name = TokenPayLoad["username"],
                add_ip = ClientInfo.IP,
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_id = TokenPayLoad.ID,
                last_name = TokenPayLoad["username"],
                last_ip = ClientInfo.IP,
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });

            foreach (dynamic item in tablelist)
            {
                var refcolumns = "";
                if (((IEnumerable<object>)item.ref_columns).Count() > 0)
                {
                    refcolumns = ((IEnumerable<object>)item.ref_columns).Aggregate((x, y) => (ComFunc.nvl(x) + "," + ComFunc.nvl(y))).ToString();
                }

                DB.QuickInsert(up, "EXTEND_REPORT_TEMPLATE_TABLES", new
                {
                    ReportUID = uid,
                    TableName = item.table_name,
                    ReferenceColumns = refcolumns,
                    IsMain = BoolStd.ConvertTo(item.is_main, 1, 0),
                    add_id = TokenPayLoad.ID,
                    add_name = TokenPayLoad["username"],
                    add_ip = ClientInfo.IP,
                    add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    last_id = TokenPayLoad.ID,
                    last_name = TokenPayLoad["username"],
                    last_ip = ClientInfo.IP,
                    last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            foreach (dynamic item in columnlist)
            {
                DB.QuickInsert(up, "EXTEND_REPORT_TEMPLATE_COLUMNS", new
                {
                    ReportUID = uid,
                    TableName = item.table_name,
                    ShowColumnName = item.column_name,
                    ShowColumnDesc = item.column_display,
                    add_id = TokenPayLoad.ID,
                    add_name = TokenPayLoad["username"],
                    add_ip = ClientInfo.IP,
                    add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    last_id = TokenPayLoad.ID,
                    last_name = TokenPayLoad["username"],
                    last_ip = ClientInfo.IP,
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
        [EWRAEmptyValid("report_name")]
        [EWRARouteDesc("修改一个报表模板的设定")]
        [EWRAAddInput("id", "string", "报表UID", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("report_name", "string", "报表名称", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("report_desc", "string", "报表描述", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAAddInput("tables", "array", @"关联表信息，格式:
[
{
   table_name:'关联表名,每个table只能加一次，不可出现重复的表名',
   is_main:'是否为主表,为false时，ref_columns才可以输入',
   ref_columns:['关联栏位描述，为空的时候该表则是主表，否则为从表，主表只能有一个，格式:{栏位名称}={表名}.{栏位名称}']
}
]", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("columns", "array", @"报表栏位信息，格式:
[
{
   table_name:'关联表名,不可为空',
   column_name:'栏位名称,不可为空',
   column_display:'表头显示名称,不可为空'
}
]", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息'
}")]
        public override object patch(string id)
        {
            var report_name = ComFunc.nvl(PostDataD.report_name);
            var report_desc = ComFunc.nvl(PostDataD.report_desc);
            var tables = PostDataD.tables;
            var columns = PostDataD.columns;

            if (tables == null || !(tables is IEnumerable<object>))
            {
                return new
                {
                    code = "failed",
                    msg = "关联表信息不可为空或格式不正确"
                };
            }
            if (columns == null || !(columns is IEnumerable<object>))
            {
                return new
                {
                    code = "failed",
                    msg = "报表栏位信息不可为空或格式不正确"
                };
            }
            var tablelist = (IEnumerable<object>)tables;
            var columnlist = (IEnumerable<object>)columns;
            var mainicount = 0;
            var pre_table_name = "";
            foreach (dynamic item in tablelist)
            {
                if (ComFunc.nvl(item.table_name) == "")
                {
                    return new
                    {
                        code = "failed",
                        msg = "关联表信息数据格式不正确"
                    };
                }
                if (ComFunc.nvl(item.table_name) == pre_table_name)
                {
                    return new
                    {
                        code = "failed",
                        msg = "不可有重复的表"
                    };
                }
                if (item.ref_coumns != null)
                {
                    if (!(item.ref_columns is IEnumerable<object>))
                    {
                        return new
                        {
                            code = "failed",
                            msg = "关联表信息数据格式不正确"
                        };
                    }
                }

                if (BoolStd.IsNotBoolThen(item.is_main))
                {
                    mainicount++;
                }
                pre_table_name = ComFunc.nvl(item.table_name);
            }
            if (mainicount > 1)
            {
                return new
                {
                    code = "failed",
                    msg = "关联表只可以有一个主表(即关联栏位描述为空)"
                };
            }


            foreach (dynamic item in columnlist)
            {
                if (ComFunc.nvl(item.table_name) == ""
                    || ComFunc.nvl(item.column_name) == ""
                    || ComFunc.nvl(item.column_display) == "")
                {
                    return new
                    {
                        code = "failed",
                        msg = "关联表信息数据格式不正确"
                    };
                }
            }
            var up = DB.NewDBUnitParameter();
            BeginTrans();
            if (!(from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE", "a")
                  where t.ReportUID == id
                  select t).IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "报表不存在"
                };
            }

            if ((from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE", "a")
                 where t.ReportName == report_name && t.ReportUID != id
                 select t).IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "报表名称已存在"
                };
            }
            var jsonexpress = buildQueryJSON(tablelist, columnlist);
            if (jsonexpress == null)
            {
                return new
                {
                    code = "failed",
                    msg = "报表栏位信息或关联表信息不正确，无法生产报表表达式"
                };
            }

            var uid = Guid.NewGuid().ToString();

            DB.QuickUpdate(up, "EXTEND_REPORT_TEMPLATE", new
            {
                ReportName = report_name,
                ReportDesc = report_desc,
                QueryJSON = jsonexpress.ToJSONString(),
                last_id = TokenPayLoad.ID,
                last_name = TokenPayLoad["username"],
                last_ip = ClientInfo.IP,
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            }, new
            {
                ReportUID = uid,
            });
            DB.QuickDelete(up, "EXTEND_REPORT_TEMPLATE_TABLES", new
            {
                ReportUID = uid,
            });
            foreach (dynamic item in tablelist)
            {
                var refcolumns = "";
                if (((IEnumerable<object>)item.ref_columns).Count() > 0)
                {
                    refcolumns = ((IEnumerable<object>)item.ref_columns).Aggregate((x, y) => (ComFunc.nvl(x) + "," + ComFunc.nvl(y))).ToString();
                }
                DB.QuickInsert(up, "EXTEND_REPORT_TEMPLATE_TABLES", new
                {
                    ReportUID = uid,
                    TableName = item.table_name,
                    ReferenceColumns = refcolumns,
                    IsMain = BoolStd.ConvertTo(item.is_main, 1, 0),
                    add_id = TokenPayLoad.ID,
                    add_name = TokenPayLoad["username"],
                    add_ip = ClientInfo.IP,
                    add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    last_id = TokenPayLoad.ID,
                    last_name = TokenPayLoad["username"],
                    last_ip = ClientInfo.IP,
                    last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            DB.QuickDelete(up, "EXTEND_REPORT_TEMPLATE_COLUMNS", new
            {
                ReportUID = uid,
            });
            foreach (dynamic item in columnlist)
            {
                DB.QuickInsert(up, "EXTEND_REPORT_TEMPLATE_COLUMNS", new
                {
                    ReportUID = uid,
                    TableName = item.table_name,
                    ShowColumnName = item.column_name,
                    ShowColumnDesc = item.column_display,
                    add_id = TokenPayLoad.ID,
                    add_name = TokenPayLoad["username"],
                    add_ip = ClientInfo.IP,
                    add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    last_id = TokenPayLoad.ID,
                    last_name = TokenPayLoad["username"],
                    last_ip = ClientInfo.IP,
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
        [EWRARouteDesc("删除一个报表模板的设定")]
        [EWRAAddInput("id", "string", "报表UID", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAOutputDesc("返回结果", @"根据http的状态码来识别，204标识操作成功，404表示操作识别未找到删除的资料")]
        public bool delete(string id)
        {
            var up = DB.NewDBUnitParameter();
            if (!(from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE", "a")
                  where t.ReportUID == id
                  select t).IsExists(up))
            {
                return false;
            }
            DB.QuickDelete(up, "EXTEND_REPORT_TEMPLATE", new
            {
                ReportUID = id,
            });
            DB.QuickDelete(up, "EXTEND_REPORT_TEMPLATE_TABLES", new
            {
                ReportUID = id,
            });
            DB.QuickDelete(up, "EXTEND_REPORT_TEMPLATE_COLUMNS", new
            {
                ReportUID = id,
            });
            return true;
        }
        [EWRARoute("patch", "/dynamicreport/{id}/active")]
        [EWRARouteDesc("激活一个报表模板的设定")]
        [EWRAAddInput("id", "string", "报表UID", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息'
}")]
        object DoActive(string id)
        {
            var up = DB.NewDBUnitParameter();
            if (!(from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE", "a")
                  where t.ReportUID == id
                  select t).IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "报表不存在"
                };
            }
            DB.QuickUpdate(up, "EXTEND_REPORT_TEMPLATE", new
            {
                IsActive = 1
            }, new
            {
                ReportUID = id,
            });

            return new
            {
                code = "success",
                msg = "操作成功"
            };
        }
        [EWRARoute("patch", "/dynamicreport/{id}/deactive")]
        [EWRARouteDesc("取消一个报表模板的设定")]
        [EWRAAddInput("id", "string", "报表UID", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息'
}")]
        object DeActive(string id)
        {
            var up = DB.NewDBUnitParameter();
            if (!(from t in DB.LamdaTable(up, "EXTEND_REPORT_TEMPLATE", "a")
                  where t.ReportUID == id
                  select t).IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "报表不存在"
                };
            }
            DB.QuickUpdate(up, "EXTEND_REPORT_TEMPLATE", new
            {
                IsActive = 0
            }, new
            {
                ReportUID = id,
            });

            return new
            {
                code = "success",
                msg = "操作成功"
            };
        }
        [EWRARoute("get", "/dynamicreport/alltableschema")]
        [EWRARouteDesc("获取所有元数据和词典表的规格信息")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息""
data:[{
    uid:'元数据表的uid',
    name:'元数据表名',
    desc:'元数据表描述',
    category_no:'类别编号',
    category_name:'类别名称',
    columns:[{
        column_name:'栏位名称',
        column_desc:'栏位描述',
        column_memo:'栏位备注'
    }]
}]
}")]
        object GetTableSchema()
        {
            var up = DB.NewDBUnitParameter();
            SetCacheEnable(false);
            var rtn = new List<object>();
            var list = (from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                        join t2 in DB.LamdaTable(up, "EXTEND_METADATA_COLUMNS", "b") on t.metauid equals t2.metauid
                        join t3 in DB.LamdaTable(up, "EXTEND_METADATA_CATEGORY", "c").LeftJoin() on t.metacategoryno equals t3.categoryno
                        orderby t.metaname, t2.sortnum, t2.metacolumnname ascending
                        select new
                        {
                            uid = t.metauid,
                            name = t.metaname,
                            desc = t.metadesc,
                            category_no = t3.categoryno,
                            category_name = t3.categoryname,
                            column_name = t2.metacolumnname,
                            column_desc = t2.metacolumndesc,
                            column_memo = t2.memo
                        }).GetQueryList(up);
            var metagrouplist = from t in list
                                group t by new
                                {
                                    uid = t.GetValue("uid"),
                                    name = t.GetValue("name"),
                                    desc = t.GetValue("desc"),
                                    category_no = t.GetValue("category_no"),
                                    category_name = t.GetValue("category_name")
                                } into g
                                select new
                                {
                                    uid = g.First().GetValue("uid"),
                                    name = g.First().GetValue("name"),
                                    desc = g.First().GetValue("desc"),
                                    category_no = g.First().GetValue("category_no"),
                                    category_name = g.First().GetValue("category_name"),
                                    columns = from gg in g
                                              select new
                                              {
                                                  column_name = gg.GetValue("column_name"),
                                                  column_desc = gg.GetValue("column_desc"),
                                                  column_memo = gg.GetValue("column_memo")
                                              }
                                };
            rtn.AddRange(metagrouplist);

            var diclist = (from t in DB.LamdaTable(up, "EXTEND_DICTIONARY_TABLE", "a")
                           orderby t.dic_name
                           select new
                           {
                               uid = t.dic_uid,
                               name = t.dic_name,
                               desc = t.dic_desc,
                               category_no = "DIC",
                               category_name = "词典表",
                               is_tree = t.istree
                           }).GetQueryList(up);
            var tmp = false;
            var diccolumns = new dynamic[] { new {column_name = "code",column_desc="编号",only_tree = false},
                                            new{ column_name = "value",column_desc="值",only_tree = false},
                                            new{ column_name = "level",column_desc="层级",only_tree = true},
                                            new{ column_name = "p_code",column_desc="父编号",only_tree = true}
            };
            var dicgrouplist = from t in diclist
                               select new
                               {
                                   t.uid,
                                   t.name,
                                   t.desc,
                                   t.category_no,
                                   t.category_name,
                                   columns = (BoolStd.IsNotBoolThen(t.is_tree)) ? from gg in diccolumns
                                                                                  select new
                                                                                  {
                                                                                      gg.column_name,
                                                                                      gg.column_desc,
                                                                                      column_memo = ""
                                                                                  } : from gg in diccolumns
                                                                                      where gg.only_tree == false
                                                                                      select new
                                                                                      {
                                                                                          gg.column_name,
                                                                                          gg.column_desc,
                                                                                          column_memo = ""
                                                                                      }
                               };
            rtn.AddRange(dicgrouplist);
            return new
            {
                code = "success",
                msg = "",
                data = rtn
            };
        }


        /// <summary>
        /// 构建报表查询的DBExpress表达式
        /// </summary>
        /// <param name="tables">关联表信息，格式:
        ///[
        /// {
        /// table_name:'关联表名',
        /// is_main:'是否为主表',
        /// ref_columns:['关联栏位描述，格式:{表名}.{栏位名称}']
        /// }
        ///]</param>
        /// <param name="columns">
        /// 报表栏位信息，格式:
        ///  [
        /// {
        /// table_name:'关联表名,不可为空',
        /// column_name:'栏位名称,不可为空',
        ///  column_display:'表头显示名称,不可为空'
        ///  }
        /// ]
        /// </param>
        /// <returns>返回为null的时候证明无法组合成表达式</returns>
        FrameDLRObject buildQueryJSON(IEnumerable<object> tables, IEnumerable<object> columns)
        {
            FrameDLRObject rtn = FrameDLRObject.CreateInstance();
            rtn.SetValue("$acttype", "Query");
            //找出主表,从表
            var maintable = new List<dynamic>();
            var subtable = new List<dynamic>();
            var alianmap = new Dictionary<string, string>();
            var index = 0;
            foreach (dynamic t in tables)
            {
                if (BoolStd.IsNotBoolThen(t.is_main))
                {
                    maintable.Add(t);
                }
                else
                {
                    subtable.Add(t);
                }
                alianmap.Add(t.table_name, $"t{index}");
                index++;
            }
            if (maintable.Count > 1) return null;


            FrameDLRObject table = FrameDLRObject.CreateInstance();
            table.SetValue(maintable.First().table_name, alianmap[maintable.First().table_name]);
            foreach (dynamic t in subtable)
            {
                var alian = alianmap[t.table_name];
                if (t.ref_columns != null)
                {
                    FrameDLRObject texpress = FrameDLRObject.CreateInstance();
                    texpress.SetValue("$as", alian);
                    var ref_columns = ((IEnumerable<object>)t.ref_columns).Select(d => ComFunc.nvl(d));
                    FrameDLRObject joinexpress = FrameDLRObject.CreateInstance();
                    texpress.SetValue("$join", joinexpress);
                    FrameDLRObject onexpress = FrameDLRObject.CreateInstance();
                    joinexpress.SetValue("$on", onexpress);
                    foreach (var c in ref_columns)
                    {
                        var left_column = c.Split('=')[0].Trim();
                        var join_table = c.Split('=')[1].Trim().Split('.')[0].Trim();
                        var by_column = c.Split('=')[1].Trim().Split('.')[1].Trim();
                        var alian_join_table = alianmap[join_table];
                        onexpress.SetValue(left_column, $"#sql:{alian_join_table}.{by_column}");
                    }

                    table.SetValue(t.table_name, texpress);
                }
            }
            rtn.SetValue("$table", table);
            foreach (dynamic item in columns)
            {
                var table_name = item.table_name;
                var column_name = item.column_name;
                if (!alianmap.ContainsKey(table_name))
                {
                    return null;
                }
                rtn.SetValue(column_name, $"#sql:{alianmap[table_name]}.{column_name}");
            }
            return rtn;
        }
    }
}
