using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Module.Business.Datas;
using System.IO;

namespace RestAPISample.Business.v1._0
{
    public class MetaData:MyRestLogic
    {
        [EWRAEmptyValid("id")]
        [EWRARoute("patch", "/metadata/create")]
        [EWRARouteDesc("创建一个元数据表，并自动锁定（元数据表未锁定时才能操作）")]
        [EWRAAddInput("id", "string", "元数据表的UID,多个用逗号分隔", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息""
}")]
        object CreateTable()
        {
            string ids = ComFunc.nvl(PostDataD.id);
            BeginTrans();
            var up = DB.NewDBUnitParameter();

            var array = ids.Split(',').Where(p => ComFunc.nvl(p) != "");
            foreach (var id in array)
            {
                var s = from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                        where t.metauid == id
                        select t;
                var list = s.GetQueryList(up);
                if (!s.IsExists(up))
                {
                    return false;
                }

                dynamic info = list.First();

                bool islocked = BoolStd.IsNotBoolThen(info.islocked, false);
                if (islocked)
                {
                    return new
                    {
                        code = "failed",
                        msg = "元数据表已锁定，不可进行操作"
                    };
                }
                bool iscreated = BoolStd.IsNotBoolThen(info.iscreated, false);
                if (iscreated)
                {
                    return new
                    {
                        code = "failed",
                        msg = "元数据表已创建，不可进行操作"
                    };
                }
                if (DB.IsTableExists(up, info.metaname))
                {
                    return new
                    {
                        code = "failed",
                        msg = "元数据表已存在，不可进行操作"
                    };
                }
            }
            foreach (var id in array)
            {
                var s = from t in DB.LamdaTable(up, "EXTEND_METADATA", "a")
                        where t.metauid == id
                        select t;
                dynamic info = s.GetQueryList(up).First();
                var columns = (from t in DB.LamdaTable(up, "EXTEND_METADATA_COLUMNS", "a")
                               where t.metauid == id
                               select t).GetQueryList(up);
                var schemas = columns.Select((p) =>
                {
                    dynamic dobj = p;
                    bool tmp = false;
                    var rtn = new TableColumn();
                    rtn.Name = dobj.MetaColumnName;
                    rtn.DataType = dobj.MetaDataType;
                    if (IntStd.IsInt(dobj.MetaDataPrecision))
                    {
                        rtn.Precision = (int)dobj.MetaDataPrecision;
                    }
                    if (IntStd.IsInt(dobj.MetaDataScale))
                    {
                        rtn.Scale = (int)dobj.MetaDataScale;
                    }
                    rtn.Default = dobj.MetaDataDefault;
                    rtn.AllowNull = BoolStd.IsNotBoolThen(dobj.MetaAllowEmpty, false);
                    rtn.IsPK = BoolStd.IsNotBoolThen(dobj.MetaIsPK, false);
                    return rtn;
                }).ToList();
                schemas.AddRange(new TableColumn[] {new TableColumn() { Name = "sort_no", DataType = "int",Default="0", IsPK = false, AllowNull = true },
                    new TableColumn() { Name = "add_id", DataType = "varchar", Precision = 50, IsPK = false, AllowNull = true },
                new TableColumn() { Name = "add_name", DataType = "nvarchar", Precision = 100, IsPK = false, AllowNull = true },
                new TableColumn() { Name = "add_ip", DataType = "varchar", Precision = 15, IsPK = false, AllowNull = true },
                new TableColumn() { Name = "add_time", DataType = "datetime", IsPK = false, AllowNull = true },
                new TableColumn() { Name = "last_id", DataType = "varchar", Precision = 50, IsPK = false, AllowNull = true },
                new TableColumn() { Name = "last_name", DataType = "nvarchar", Precision = 100, IsPK = false, AllowNull = true },
                new TableColumn() { Name = "last_ip", DataType = "varchar", Precision = 15, IsPK = false, AllowNull = true },
                new TableColumn() { Name = "last_time", DataType = "datetime", IsPK = false, AllowNull = true }
                });
                DB.CreateTable(up, info.metaname, schemas.ToArray());

                s.Update(up, new
                {
                    islocked = 1,
                    iscreated = 1
                });

            }
            CommitTrans();
            return new
            {
                code = "success",
                msg = "操作成功"
            };
        }
        [EWRARoute("patch", "/metadata/copytable")]
        object CopyTable()
        {
            var up = DB.NewDBUnitParameter();
            var copy_json = FrameDLRObject.IsJsonThen(File.ReadAllText($"{ServerInfo.ServerRootPath}/DBExpressScripts/Copy_IC_Staff.json"));
            if(copy_json != null)
            {
                DB.Excute(up, copy_json);
            }

            return new
            {
                code = "success",
                msg = ""
            };
        }
        [EWRARoute("patch", "/metadata/copydata")]
        object CopyData()
        {
            var up = DB.NewDBUnitParameter();
            var copy_json = FrameDLRObject.IsJsonThen(File.ReadAllText($"{ServerInfo.ServerRootPath}/DBExpressScripts/CopyData_IC_Staff.json"));
            if (copy_json != null)
            {
                DB.Excute(up, copy_json,true);
            }

            return new
            {
                code = "success",
                msg = ""
            };
        }
    }
}
