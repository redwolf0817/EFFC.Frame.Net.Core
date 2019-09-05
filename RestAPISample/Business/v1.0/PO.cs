using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using EFFC.Frame.Net.Base.Constants;
using static EFFC.Frame.Net.Module.Extend.EWRA.Attributes.EWRAAddInputAttribute;

namespace RestAPISample.Business.v1._0
{
    public class PO : MyRestLogic
    {
        [EWRARoute("get", "/po")]
        [EWRARouteDesc("PO单查询")]
        [EWRAAddInput("start_date","datetime","起始日期","默认为空")]
        [EWRAAddInput("end_date", "datetime", "结束日期", "默认为空")]
        [EWRAOutputDesc("返回结果", @"{
""data"":[结果集列表],
""total_num"":""int,总行数"",
""total_page"":""int,总页数"",
""page_size"":""int,每页笔数""W
}")]
        object QueryPO()
        {
            SetCacheEnable(false);
            string start_date = ComFunc.nvl(QueryStringD.start_date);
            string end_date = ComFunc.nvl(QueryStringD.end_date);

            if (!IsValidBy("日期格式不正确", () =>
            {
                return (start_date == "" || DateTimeStd.IsDateTime(start_date)) && (end_date == "" || DateTimeStd.IsDateTime(end_date));
            }))
            {
                return null;
            }

            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable("HW_PO", "a")
                    where t.notnull(start_date, t.accepted_date >= DateTimeStd.IsDateTimeThen(start_date))
                        && t.notnull(end_date, t.accepted_date <= DateTimeStd.IsDateTimeThen(end_date))
                        && t.is_processed == 0
                    select t;
            var result = DB.LamdaQueryByPage(up, s, "hw_contract_no");
            var polist = (from t in result.QueryData<FrameDLRObject>()
                          select new
                          {
                              t.hw_contract_no,
                              t.cust_contract_no,
                              t.version,
                              t.status,
                              t.hw_frame_contract_no,
                              t.cust_frame_contract_no,
                              t.hw_sign_entity,
                              t.cust_sign_entity,
                              t.cust_type_lv1,
                              t.cust_type_lv2,
                              t.trade_terms,
                              t.transport_mode,
                              t.payment_terms,
                              t.is_vat_included,
                              contract_amount_cny = DoubleStd.IsDouble(t.contract_amount_cny) ? DoubleStd.ParseStd(t.contract_amount_cny).Value : 0.0,
                              contract_amount_usd = DoubleStd.IsDouble(t.contract_amount_usd) ? DoubleStd.ParseStd(t.contract_amount_usd).Value : 0.0,
                              t.retail_sample,
                              t.created_by,
                              created_date = DateTimeStd.IsDateTimeThen(t.created_date, "yyyy-MM-dd HH:mm:ss"),
                              accepted_date = DateTimeStd.IsDateTimeThen(t.accepted_date, "yyyy-MM-dd HH:mm:ss"),
                              review_completed_date = DateTimeStd.IsDateTimeThen(t.review_completed_date, "yyyy-MM-dd HH:mm:ss"),
                              signed_date = DateTimeStd.IsDateTimeThen(t.signed_date, "yyyy-MM-dd HH:mm:ss")
                          }).ToList();
            foreach (dynamic item in polist)
            {
                var sline = from t in DB.LamdaTable("HW_PO_Line", "a")
                            where t.hw_contract_no == item.hw_contract_no
                            select new
                            {
                                t.po_line_no,
                                t.line_type,
                                t.prod_code_cust,
                                t.prod_desc_cust,
                                t.prod_code_sale,
                                t.prod_desc_hw,
                                t.prod_model,
                                t.color,
                                t.quantity,
                                t.currency,
                                t.unit_price_cny,
                                t.line_amount_cny,
                                t.line_amount_usd
                            };
                item.po_line = DB.ExcuteLamda(up, sline).QueryData<object>();

                var sfee = from t in DB.LamdaTable("HW_PO_Fee", "a")
                           where t.hw_contract_no == item.hw_contract_no
                           select new
                           {
                               t.line_no,
                               t.line_type,
                               t.desc,
                               t.prod_model,
                               t.amount,
                               t.comments
                           };
                item.other_fee = DB.ExcuteLamda(up, sfee).QueryData<object>();
            }

            return new
            {
                data = polist,
                total_num = result.TotalRow,
                current_page = result.CurrentPage,
                total_page = result.TotalPage,
                page_size = result.Count_Of_OnePage
            };
        }

        [EWRARoute("get", "/delivery")]
        [EWRARouteDesc("发货状态查询")]
        [EWRADateTimeValid("start_time,end_time", false, false)]
        object DeliveryStatus()
        {
            SetCacheEnable(false);
            var hw_contract_no = ComFunc.nvl(QueryStringD.hw_contract_no);
            var start_time = ComFunc.nvl(QueryStringD.start_time);
            var end_time = ComFunc.nvl(QueryStringD.end_time);
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable("HW_DeliveryStatusInfo", "a")
                    where t.notnull(hw_contract_no, t.hw_contract_no == hw_contract_no)
                        && t.notnull(start_time, t.actual_shipped_date >= DateTimeStd.IsDateTimeThen(start_time))
                        && t.notnull(end_time, t.actual_shipped_date <= DateTimeStd.IsDateTimeThen(end_time))
                        && t.is_processed == 0
                    select t;
            var result = DB.LamdaQueryByPage(up, s, "hw_contract_no");
            return new
            {
                total_num = result.TotalRow,
                current_page = result.CurrentPage,
                total_page = result.TotalPage,
                page_size = result.Count_Of_OnePage,
                data = from t in result.QueryData<FrameDLRObject>()
                       select new
                       {
                           t.delivery_id,
                           t.packing_list_no,
                           t.hw_contract_no,
                           t.cust_contract_no,
                           t.cust_purchase_no,
                           t.cust_code_cbg,
                           t.cust_code_hw,
                           t.cust_name,
                           t.channel_id,
                           t.cust_type_lv1,
                           t.cust_type_lv2,
                           t.country_code,
                           t.prod_type_cn,
                           t.prod_type_en,
                           t.prod_business_type,
                           t.prod_code_sale,
                           t.prod_code_make,
                           t.prod_desc_hw,
                           t.prod_desc_hw_en,
                           t.prod_code_cust,
                           t.prod_desc_cust,
                           t.prod_model_hw,
                           t.prod_model_ext,
                           t.prod_color_cn,
                           t.prod_color_en,
                           t.cust_purchase_item,
                           estimate_arrive_date = DateTimeStd.IsDateTimeThen(t.estimate_arrive_date, "yyyy-MM-dd HH:mm:ss"),
                           actual_shipped_date = DateTimeStd.IsDateTimeThen(t.actual_shipped_date, "yyyy-MM-dd HH:mm:ss"),
                           t.destination_city,
                           t.destination_province,
                           t.destination_detail,
                           delivery_quantity = IntStd.IsInt(t.delivery_quantity) ? IntStd.ParseStd(t.delivery_quantity).Value : 0,
                           pod_sign_date = DateTimeStd.IsDateTimeThen(t.pod_sign_date, "yyyy-MM-dd HH:mm:ss"),
                           sign_quantity = IntStd.IsInt(t.sign_quantity) ? IntStd.ParseStd(t.sign_quantity).Value : 0,
                           t.order_line_note,
                           t.free_text
                       }
            };
        }

        private FrameDLRObject copyPOItem(dynamic source)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.hw_contract_no = source.hw_contract_no;
            rtn.cust_contract_no = source.cust_contract_no;
            rtn.version = source.version;
            rtn.status = source.status;
            rtn.hw_frame_contract_no = source.hw_frame_contract_no;
            rtn.cust_frame_contract_no = source.cust_frame_contract_no;
            rtn.hw_sign_entity = source.hw_sign_entity;
            rtn.cust_sign_entity = source.cust_sign_entity;
            rtn.cust_type_lv1 = source.cust_type_lv1;
            rtn.cust_type_lv2 = source.cust_type_lv2;
            rtn.trade_terms = source.trade_terms;
            rtn.transport_mode = source.transport_mode;
            rtn.payment_terms = source.payment_terms;
            rtn.is_vat_included = source.is_vat_included;
            rtn.contract_amount_cny = DoubleStd.IsDouble(source.contract_amount_cny) ? DoubleStd.ParseStd(source.contract_amount_cny).Value : null;
            rtn.contract_amount_usd = DoubleStd.IsDouble(source.contract_amount_usd) ? DoubleStd.ParseStd(source.contract_amount_usd).Value : null;
            rtn.retail_sample = source.retail_sample;
            rtn.created_by = source.created_by;
            rtn.created_date = DateTimeStd.IsDateTime(source.created_date) ? DateTimeStd.ParseStd(source.created_date).Value.ToString("yyyyMMddHHmmss") : null;
            rtn.accepted_date = DateTimeStd.IsDateTime(source.accepted_date) ? DateTimeStd.ParseStd(source.accepted_date).Value.ToString("yyyyMMddHHmmss") : null;
            rtn.review_completed_date = DateTimeStd.IsDateTime(source.review_completed_date) ? DateTimeStd.ParseStd(source.review_completed_date).Value.ToString("yyyyMMddHHmmss") : null;
            rtn.signed_date = DateTimeStd.IsDateTime(source.signed_date) ? DateTimeStd.ParseStd(source.signed_date).Value.ToString("yyyyMMddHHmmss") : null;
            return rtn;
        }
        private FrameDLRObject copyPOLineItem(dynamic source, string hw_contract_no, int lineindex)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.hw_contract_no = hw_contract_no;
            rtn.po_line_no = IntStd.IsInt(source.po_line_no) ? IntStd.ParseStd(source.po_line_no).Value : lineindex;
            rtn.line_type = source.line_type;
            rtn.prod_code_cust = source.prod_code_cust;
            rtn.prod_desc_cust = source.prod_desc_cust;
            rtn.prod_code_sale = source.prod_code_sale;
            rtn.prod_desc_hw = source.prod_desc_hw;
            rtn.prod_model = source.prod_model;
            rtn.color = source.color;
            rtn.quantity = IntStd.IsInt(source.quantity) ? IntStd.ParseStd(source.quantity).Value : 0;
            rtn.currency = source.currency;
            rtn.unit_price_cny = DoubleStd.IsDouble(source.unit_price_cny) ? DoubleStd.ParseStd(source.unit_price_cny).Value : 0.0;
            rtn.line_amount_cny = DoubleStd.IsDouble(source.line_amount_cny) ? DoubleStd.ParseStd(source.line_amount_cny).Value : 0.0;
            rtn.line_amount_usd = DoubleStd.IsDouble(source.line_amount_usd) ? DoubleStd.ParseStd(source.line_amount_usd).Value : 0.0;

            return rtn;
        }

        private FrameDLRObject copyPOFeeItem(dynamic source, string hw_contract_no, int lineindex)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.hw_contract_no = hw_contract_no;
            rtn.line_no = IntStd.IsInt(source.line_no) ? IntStd.ParseStd(source.line_no).Value : lineindex;
            rtn.line_type = source.line_type;
            rtn.desc = source.desc;
            rtn.prod_model = source.prod_model;
            rtn.amount = DoubleStd.IsDouble(source.amount) ? DoubleStd.ParseStd(source.amount).Value : 0.0;
            rtn.comments = source.comments;

            return rtn;
        }
        static object lockobj = new object();
        [EWRARoute("patch", "/waterline/use")]
        [EWRARouteDesc("核销")]
        [EWRAEmptyValid("items")]
        object Use()
        {
            var items = ComFunc.UrlDecode(ComFunc.nvl(PostDataD.items));
            var jsonstr = ComFunc.IsBase64Then(items);
            object[] parray = FrameDLRObject.IsJsonArrayThen(jsonstr);
            if (!IsValidBy("参数格式不正确", () =>
            {
                var rtn = jsonstr != "" && parray != null;
                if (rtn)
                {
                    foreach (dynamic item in parray)
                    {
                        if (ComFunc.nvl(item.hw_contract_no) != "" && ComFunc.nvl(item.prod_code_sale) != "" && IntStd.IsInt(item.quantity))
                        {
                            rtn = rtn && true;
                        }
                        else
                        {
                            rtn = rtn && false;
                        }
                    }
                }

                return rtn;
            }))
            {
                return null;
            }
            var merge_parray = new Dictionary<string, int>();
            foreach (dynamic pitem in parray)
            {
                var key = $"{pitem.hw_contract_no}|{pitem.prod_code_sale}";
                if (merge_parray.ContainsKey(key))
                {
                    merge_parray[key] += IntStd.ParseStd(pitem.quantity).Value;
                }
                else
                {
                    merge_parray[key] = IntStd.ParseStd(pitem.quantity).Value;
                }
            }

            lock (lockobj)
            {
                var up = DB.NewDBUnitParameter();
                BeginTrans();
                var rtnlist = new List<object>();
                foreach (var pitem in merge_parray)
                {
                    var splitstr = pitem.Key.Split("|");
                    var hw_contract_no = splitstr[0];
                    var prod_code_sale = splitstr[1];
                    var s = from t in DB.LamdaTable("hw_po", "a")
                            join t2 in DB.LamdaTable("hw_po_line", "b") on t.hw_contract_no equals t2.hw_contract_no
                            where t2.prod_code_sale == prod_code_sale && t2.hw_contract_no == hw_contract_no
                            select t2;

                    var result = DB.ExcuteLamda(up, s);


                    if (!IsValidBy("资料无效或参数无效", () => result.QueryTable.RowLength > 0))
                    {
                        return null;
                    }
                    dynamic item = result.QueryData<FrameDLRObject>()[0];
                    if (!IsValidBy("核销数量不正确", () => IntStd.IsNotIntThen(item.freeze_quantity) >= pitem.Value))
                    {
                        //RollBack();
                        return null;
                    }
                    DB.QuickUpdate(up, "hw_po_line", new
                    {
                        freeze_quantity = IntStd.IsNotIntThen(item.freeze_quantity) - pitem.Value,
                        used_quantity = IntStd.IsNotIntThen(item.used_quantity) + pitem.Value
                    }, new
                    {
                        hw_contract_no = item.hw_contract_no,
                        po_line_no = item.po_line_no
                    });
                }
                CommitTrans();

                return "success";
            }

        }

        [EWRARoute("get", "/sign")]
        [EWRARouteDesc("签收资料")]
        object DeliverySign()
        {
            SetCacheEnable(false);
            var free_text = ComFunc.nvl(QueryStringD.free_text);
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable("HW_DeliveryStatusInfo", "a")
                    where t.notnull(free_text, t.free_text == free_text)
                        && t.pod_sign_date != null
                    select t;
            var result = DB.LamdaQueryByPage(up, s, "actual_shipped_date desc,pod_sign_date desc");
            return new
            {
                total_num = result.TotalRow,
                current_page = result.CurrentPage,
                total_page = result.TotalPage,
                page_size = result.Count_Of_OnePage,
                data = from t in result.QueryData<FrameDLRObject>()
                       select new
                       {
                           t.delivery_id,
                           t.packing_list_no,
                           t.hw_contract_no,
                           t.cust_contract_no,
                           t.cust_purchase_no,
                           //t.cust_code_cbg,
                           //t.cust_code_hw,
                           //t.cust_name,
                           //t.channel_id,
                           //t.cust_type_lv1,
                           //t.cust_type_lv2,
                           t.country_code,
                           //t.prod_type_cn,
                           //t.prod_type_en,
                           //t.prod_business_type,
                           t.prod_code_sale,
                           //t.prod_code_make,
                           t.prod_desc_hw,
                           //t.prod_desc_hw_en,
                           t.prod_code_cust,
                           t.prod_desc_cust,
                           //t.prod_model_hw,
                           //t.prod_model_ext,
                           //t.prod_color_cn,
                           //t.prod_color_en,
                           t.cust_purchase_item,
                           estimate_arrive_date = DateTimeStd.IsDateTimeThen(t.estimate_arrive_date, "yyyy-MM-dd HH:mm:ss"),
                           actual_shipped_date = DateTimeStd.IsDateTimeThen(t.actual_shipped_date, "yyyy-MM-dd HH:mm:ss"),
                           t.destination_city,
                           t.destination_province,
                           t.destination_detail,
                           delivery_quantity = IntStd.IsInt(t.delivery_quantity) ? IntStd.ParseStd(t.delivery_quantity).Value : 0,
                           pod_sign_date = DateTimeStd.IsDateTimeThen(t.pod_sign_date, "yyyy-MM-dd HH:mm:ss"),
                           sign_quantity = IntStd.IsInt(t.sign_quantity) ? IntStd.ParseStd(t.sign_quantity).Value : 0,
                           t.order_line_note,
                           t.free_text
                       }
            };
        }
       
    }

}
