using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using EFFC.Frame.Net.Base.Data;

namespace RestAPISample.Business.v1._0
{
    public class VerticalSnap : MyRestLogic
    {
        [EWRARoute("get", "/v_snap")]
        [EWRARouteDesc("获取车辆抓拍列表")]
        [EWRAAddInput("estate", "string", "小区编号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("plate_no", "string", "车牌号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("position", "string", "位置坐标", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("start_time", "string", "抓拍起始时间", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("end_time", "string", "抓拍结束时间", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("limit", "int", "每页笔数", "默认为10", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAAddInput("page", "int", "跳到指定页数", "默认为1", EFFC.Frame.Net.Base.Constants.RestInputPosition.QueryString, true)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息',
total_count:'总笔数',
page:'当前页数',
total_page:'总页数',
limit:'每页笔数',
data:[{
        id:'抓拍系统id号',
        plate_no:'车牌号',
        plate_color:'车牌颜色',
        vertical_color:'车牌颜色',
        occur_time:'抓拍时间',
        position:'抓拍位置',
        device_id:'设备系统编号',
        device_code:'设备编码',
        device_name:'设备名称',
        estate_id:'小区编号',
        estate_name:'小区名称',
        is_recorded:'是否已登记',
        driver_uid:'车主UID'
    }]
}")]
        public object QueryList()
        {
            var estate_id = ComFunc.nvl(PostDataD.estate);
            var position = ComFunc.nvl(PostDataD.position);
            var start_time = DateTimeStd.IsDateTimeThen(PostDataD.start_time, "yyyy-MM-dd HH:mm:ss");
            var end_time = DateTimeStd.IsDateTimeThen(PostDataD.start_time, "yyyy-MM-dd HH:mm:ss");
            var plate_no = ComFunc.nvl(PostDataD.plate_no);

            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "IC_Car_Monitor", "a")
                          join t2 in DB.LamdaTable(up, "IC_Vehicle", "b").LeftJoin() on t.CM_Plate_No equals t2.V_No
                          join t3 in DB.LamdaTable(up, "IC_Device", "c").LeftJoin() on t.CM_Device equals t3.D_UID
                          join t4 in DB.LamdaTable(up, "IC_Estate", "d").LeftJoin() on t.CM_Estate_ID equals t4.E_ID
                          join t5 in DB.LamdaTable(up, "IC_Position", "e").LeftJoin() on t.CM_Position_XY equals t5.P_GUID
                          where t.notnull(estate_id, t.CM_Estate_ID == estate_id)
                                && t.notnull(position, t.CM_Position_XY == position)
                                && t.notnull(start_time, t.CM_Occur_Time >= start_time)
                                && t.notnull(end_time, t.CM_Occur_Time <= end_time)
                                && t.notnull(plate_no, t.CM_Plate_No.contains(plate_no))
                          select new
                          {
                              id = t.CM_GUID,
                              plate_no = t.CM_Plate_No,
                              plate_color = t.CM_Plate_Color,
                              vertical_color = t.CM_Color,
                              occur_time = t.CM_Occur_Time,
                              position = t5.P_Name,
                              device_id = t3.D_UID,
                              device_code = t3.D_Code,
                              device_name = t3.D_Name,
                              estate_id = t4.E_ID,
                              estate = t4.E_Name,
                              driver_uid = t2.V_Driver,
                              t.add_time,
                              t.last_time
                          }).QueryByPage(up, "occur_time desc,add_time desc,last_time desc");

            var data = result.QueryData<FrameDLRObject>().Select((d) =>
            {
                var dobj = (FrameDLRObject)d;
                d.occur_time = DateTimeStd.IsDateTimeThen(d.occur_time, "yyyy-MM-dd HH:mm:ss");
                d.driver_add_time = DateTimeStd.IsDateTimeThen(d.driver_add_time, "yyyy-MM-dd HH:mm:ss");
                d.is_recorded = ComFunc.nvl(d.driver_uid) == "" ? false : true;
                dobj.Remove("add_time");
                dobj.Remove("last_time");
                return d;
            });
            return new
            {
                code = "success",
                msg = "",
                total_count = result.TotalRow,
                page = result.CurrentPage,
                total_page = result.TotalPage,
                limit = result.Count_Of_OnePage,
                data = from t in data
                       select t
            };
        }

        [EWRARoute("get", "/v_snap/driver/{id}")]
        [EWRARouteDesc("获取人员抓拍列表")]
        [EWRAAddInput("id", "string", "人员uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息',
data:{
        driver_uid:'车主UID',
        driver_name:'车主姓名',
        driver_id_no:'车主证件号',
        driver_add_time:'车主登记时间',
        driver_house_no:'车主门牌号',
        driver_house_unit:'车主单元号',
        driver_building_name:'车主楼栋名称',
        driver_phone:'车主联系电话'
    }
}")]
        object GetDriverInfo(string id)
        {
            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "IC_Resident", "f")
                          join t1 in DB.LamdaTable(up, "IC_House", "g").LeftJoin() on t.R_House_ID equals t1.H_ID
                          join t2 in DB.LamdaTable(up, "IC_Building", "h").LeftJoin() on t1.H_Building_ID equals t2.B_ID
                          where t.R_ID == id
                          select new
                          {
                              driver_uid = t.R_ID,
                              driver_name = t.R_Name,
                              driver_id_no = t.R_ID_No,
                              driver_add_time = t.add_time,
                              driver_house_id = t1.R_House_ID,
                              driver_house_no = t1.H_No,
                              driver_house_unit = t1.H_Unit,
                              driver_building_name = t2.B_Name
                          }).GetQueryList(up);

            if (result.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "无该车主资料"
                };
            }
            dynamic data = result.First();
            data.driver_add_time = DateTimeStd.IsDateTimeThen(data.driver_add_time, "yyyy-MM-dd HH:mm:ss");
            return new
            {
                code = "success",
                msg = "",
                data
            };
        }
        [EWRARoute("get", "/v_snap/vertical/{plate_no}")]
        [EWRARouteDesc("获取人员抓拍列表")]
        [EWRAAddInput("plate_no", "string", "车牌号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息',
data:{
        plate_no:'车牌号',
        plate_color:'车牌颜色',
        vertical_color:'车牌颜色',
        vertical_brand:'车辆品牌',
        in_position:'最近一次进场位置',
        in_time:'最近一次进场时间',
        out_position:'最近一次出场位置',
        out_time:'最近一次出场时间'
    }
}")]
        object GetVerticalInfo(string id)
        {
            var up = DB.NewDBUnitParameter();
            var result = (from t in DB.LamdaTable(up, "IC_Vehicle", "a")
                          where t.R_ID == id
                          select new
                          {
                              plate_no = t.V_No,
                              plate_color = t.V_No_Plate_Color,
                              vertical_color = t.V_Color,
                              vertical_brand = "",
                              in_position = "",
                              in_time = "",
                              out_position = "",
                              out_time = ""
                          }).GetQueryList(up);

            if (result.Count <= 0)
            {
                return new
                {
                    code = "failed",
                    msg = "无该车资料"
                };
            }
            dynamic data = result.First();
            //需要与道闸系统通讯，抓取进出场的资料
            return new
            {
                code = "success",
                msg = "",
                data
            };
        }

        [EWRARoute("post", "/v_snap")]
        [EWRARouteDesc("新增一个抓拍记录")]
        [EWRAEmptyValid("estate,position_id,device_id,occur_time,plate_no,file_name,file_length,file_content")]
        [EWRAAddInput("estate", "string", "小区编号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("position_id", "string", "位置编号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("device_id", "string", "设备编号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("occur_time", "string", "抓拍时间", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("plate_no", "string", "车牌号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("plate_color", "string", "车牌颜色", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAAddInput("vertical_color", "string", "车辆颜色", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, true)]
        [EWRAAddInput("file_name", "string", "文件名称", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("file_length", "long", "文件大小", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAAddInput("file_content", "string", "文件内容，base64加密", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息',
uid:'新增成功返回的抓拍ID'
}")]
        public override object post()
        {
            var estate_id = ComFunc.nvl(PostDataD.estate);
            var position_id = ComFunc.nvl(PostDataD.position_id);
            var device_id = ComFunc.nvl(PostDataD.device_id);
            var occur_time = DateTimeStd.IsDateTimeThen(PostDataD.occur_time, "yyyy-MM-dd HH:mm:ss");
            var plate_no = ComFunc.nvl(PostDataD.plate_no);
            var plate_color = ComFunc.nvl(PostDataD.plate_color);
            var vertical_color = ComFunc.nvl(PostDataD.vertical_color);
            string pic = ComFunc.nvl(PostDataD.file_content).Replace(" ", "+");
            string file_name = ComFunc.nvl(PostDataD.file_name);
            long file_length = Int64Std.IsNotInt64Then(PostDataD.file_length);

            BeginTrans();
            var up = DB.NewDBUnitParameter();
            if (!(from t in DB.LamdaTable(up, "IC_Position")
                  where t.P_GUID == position_id
                  select t).IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "不是合法的位置信息"
                };
            }
            if (!(from t in DB.LamdaTable(up, "IC_Device")
                  where t.D_UID == device_id
                  select t).IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "不是合法的设备"
                };
            }
            if (!(from t in DB.LamdaTable(up, "IC_Estate")
                  where t.E_ID == estate_id
                  select t).IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "小区信息不正确"
                };
            }
            if (!ComFunc.IsBase64(pic) || file_length == 0)
            {
                return new
                {
                    code = "failed",
                    msg = "抓拍图片格式不正确"
                };
            }
            if (occur_time == "")
            {
                return new
                {
                    code = "failed",
                    msg = "抓拍时间格式不正确"
                };
            }
            
            var uid = Guid.NewGuid().ToString();

            DB.QuickInsert(up, "IC_Car_Monitor", new
            {
                CM_GUID = uid,
                CM_Estate_ID = estate_id,
                CM_Occur_Time = occur_time,
                CM_Position_XY = position_id,
                CM_Device = device_id,
                CM_Pic1 = "",
                CM_IsProcessed = 0,
                add_id = TokenPayLoad.ID,
                add_name = ComFunc.nvl(TokenPayLoad["username"]),
                add_ip = ClientInfo.IP,
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_id = TokenPayLoad.ID,
                last_name = ComFunc.nvl(TokenPayLoad["username"]),
                last_ip = ClientInfo.IP,
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            CommitTrans();
            return new
            {
                code = "success",
                msg = "",
                uid
            };
        }
        [EWRARoute("delete", "/v_snap/{id}")]
        [EWRARouteDesc("删除一个抓拍记录")]
        [EWRAAddInput("id", "string", "抓拍记录uid", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        new object delete(string id)
        {
            BeginTrans();
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable(up, "IC_Car_Monitor", "a")
                    where t.CM_GUID == id
                    select t;
            var list = s.GetQueryList(up);
            if (list.Count <= 0)
            {
                return new
                {
                    code = "success",
                    msg = "资料已删除，无须处理"
                };
            }
            dynamic info = list.First();

            
            CommitTrans();
            return new
            {
                code = "success",
                msg = "操作成功"
            };
        }
    }
}
