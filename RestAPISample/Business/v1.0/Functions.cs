using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Unit.DB;

namespace RestAPISample.Business.v1._0
{
    public class Functions:MyRestLogic
    {
        public override List<object> get()
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable(up, "EXTEND_FUNCTION", "a")
                    orderby t.sortnum
                    select t;
            return s.Query(up).QueryData<FrameDLRObject>()
                .Select((p) =>
                {
                    dynamic obj = p;
                    return new
                    {
                        obj.functionno,
                        obj.functionname,
                        obj.functionurl,
                        obj.p_functionno,
                        obj.functionlevel,
                        obj.action,
                        obj.sortnum,
                        obj.is_menu
                    };
                }).ToList<object>();
        }

        [EWRAEmptyValid("function_no,function_name")]
        [EWRARouteDesc("新增一个功能")]
        [EWRAAddInput("function_no", "string", "功能编号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("function_name", "string", "功能名称", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("function_url", "string", "功能链接", "默认为空", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAAddInput("is_menu", "string", "是否显示在菜单中", "默认为false", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAAddInput("p_function_no", "string", "父功能编号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
function_no:""成功时候返回的功能编号""
}")]
        public override object post()
        {
            var no = ComFunc.nvl(PostDataD.function_no);
            var name = ComFunc.nvl(PostDataD.function_name);
            var url = ComFunc.nvl(PostDataD.function_url);
            var is_menu = ComFunc.nvl(PostDataD.is_menu) == "" ? false : bool.Parse(ComFunc.nvl(PostDataD.is_menu));
            var p_no = ComFunc.nvl(PostDataD.p_function_no);

            var up = DB.NewDBUnitParameter();
            if ((from t in DB.LamdaTable(up, "EXTEND_FUNCTION", "a")
                                where t.functionno == no || t.functionname == name
                                select t).IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "功能编号/名称已存在"
                };
            }
            var level = 0;
            if (p_no != "")
            {
                var p_inforesult = (from t in DB.LamdaTable(up, "EXTEND_FUNCTION", "a")
                                                      where t.functionno == p_no
                                                      select t).Query(up);
                if (p_inforesult.QueryTable.RowLength <= 0)
                {
                    return new
                    {
                        code = "failed",
                        msg = "父功能不存在"
                    };
                }

                var p_info = p_inforesult.QueryData<FrameDLRObject>().First();
                level = IntStd.IsNotIntThen(p_info.GetValue("FunctionLevel"), -1) + 1;
            }
            DB.LamdaTable(up, "EXTEND_FUNCTION", "a").Insert(up,new
            {
                FunctionNo = no,
                FunctionName = name,
                FunctionUrl = url,
                P_FunctionNo = p_no,
                FunctionLevel = level,
                Is_Menu = is_menu
            });
            //DB.QuickInsert(up, "EXTEND_FUNCTION", new
            //{
            //    FunctionNo = no,
            //    FunctionName = name,
            //    FunctionUrl = url,
            //    P_FunctionNo = p_no,
            //    FunctionLevel = level,
            //    Is_Menu = is_menu
            //});

            return new
            {
                code = "success",
                msg = "新增成功",
                function_no = no
            };
        }
        [EWRARouteDesc("删除一个功能")]
        [EWRAAddInput("id", "string", "功能编号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAOutputDesc("返回结果", @"根据http的状态码来识别，204标识操作成功，404表示操作识别未找到删除的资料")]
        public override bool delete(string id)
        {
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable(up, "EXTEND_FUNCTION", "a")
                    where t.functionno == id
                    select t;
            if (!s.IsExists(up))
            {
                return false;
            }
            s.Delete(up);

            return true;
        }
        [EWRARouteDesc("修改一个功能")]
        [EWRAAddInput("function_no", "string", "功能编号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAAddInput("function_name", "string", "功能名称", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("function_url", "string", "功能链接", "默认为空", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAAddInput("is_menu", "string", "是否显示在菜单中", "默认为false", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAAddInput("p_function_no", "string", "父功能编号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息"",
function_no:""成功时候返回的功能编号""
}")]
        public override object patch(string id)
        {
            var up = DB.NewDBUnitParameter();
            var name = ComFunc.nvl(PostDataD.function_name);
            var url = ComFunc.nvl(PostDataD.function_url);
            var is_menu = ComFunc.nvl(PostDataD.is_menu) == "" ? false : bool.Parse(ComFunc.nvl(PostDataD.is_menu));
            var p_no = ComFunc.nvl(PostDataD.p_function_no);

            var s = from t in DB.LamdaTable(up, "EXTEND_FUNCTION", "a")
                    where t.functionno == id
                    select t;
            if (!s.IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "数据不存在"
                };
            }
            if ((from t in DB.LamdaTable(up, "EXTEND_FUNCTION", "a")
                 where t.functionname == name && t.functionno != id
                 select t).IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "功能名称重复"
                };
            }
            var level = 0;
            if (p_no != "")
            {
                var p_inforesult = (from t in DB.LamdaTable(up, "EXTEND_FUNCTION", "a")
                                    where t.functionno == p_no
                                    select t).Query(up);
                if (p_inforesult.QueryTable.RowLength <= 0)
                {
                    return new
                    {
                        code = "failed",
                        msg = "父功能不存在"
                    };
                }

                var p_info = p_inforesult.QueryData<FrameDLRObject>().First();
                level = IntStd.IsNotIntThen(p_info.GetValue("FunctionLevel"), -1) + 1;
            }
            s.Update(up, new
            {
                FunctionName = name,
                FunctionUrl = url,
                P_FunctionNo = p_no,
                FunctionLevel = level,
                Is_Menu = is_menu
            });

            return new
            {
                code = "success",
                msg = "修改成功",
                function_no = id
            };
        }
    }
}
