using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Resource.Sqlite;
using RestAPISample.Unit;

namespace RestAPISample.Business.v1._0
{
    public class Jiangsu:MyRestLogic
    {
        [EWRARoute("get", "/dms/jiangsu/orderhis")]
        public object GetOrdersHis()
        {
            SetCacheEnable(false);
            object rtn = null;

            var up = DB.NewDBUnitParameter();
            DB.QuickInsertNotExists(up, "orders_goods", new
            {
                order_no = "B20180312152249720607",
                detail_num = "S20180312152249720608",
                goods_type = "SPLX_SJZD",
                goods_num = "JSYD-DDSCSPP-DDSCSXH6-02",
                goods_price = 10.0,
                goods_count = 10
            },new {
                order_no = "B20180312152249720607",
                detail_num = "S20180312152249720608",
            });

            return rtn;
        }
        [EWRARoute("put", "/dms/jiangsu/order_done")]
        [EWRARouteDesc("订单已处理更新")]
        [EWRAEmptyValid("order_no")]
        public object UpdateDoneFlag()
        {
            string order_nos = ComFunc.nvl(PostDataD.order_no);
            var up = DB.NewDBUnitParameter();

            //var arr = order_nos.Split(',', StringSplitOptions.RemoveEmptyEntries);
            //foreach (var no in arr)
            //{
            //    DB.QuickUpdate(up, "orders", new { order_status = "DONE" }, new { order_no = no });
            //}

            return "success";

        }
        [EWRARoute("get", "/imei")]
        [EWRARouteDesc("IMEI资料查询")]
        object QueryIMEI()
        {
            SetCacheEnable(false);
            var delivery_id = QueryStringD.delivery_id;
            string start_time = DateTimeStd.IsDateTimeThen(QueryStringD.start_time, "yyyy-MM-dd HH:mm:ss");
            string end_time = DateTimeStd.IsDateTimeThen(QueryStringD.end_time, "yyyy-MM-dd HH:mm:ss");

            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable("HW_IMEI_Info", "a")
                    join t2 in DB.LamdaTable("hw_deliverystatusinfo", "b") on t.delivery_id equals t2.delivery_id
                    where t.notnull(delivery_id, t.delivery_id == delivery_id)
                    && t.notnull(start_time, t.create_time >= start_time)
                    && t.notnull(end_time, t.end_time <= end_time)
                    select new
                    {
                        t.delivery_id,
                        t.prod_model_ext,
                        t.prod_code_sale,
                        t.prod_code_make,
                        t.prod_color_cn,
                        t.prod_color_en,
                        t.prod_barcode,
                        t.imei,
                        t.imei_1,
                        t.imei_2,
                        t.meid_hex,
                        t.meid_dec,
                        t.meid_dec_18,
                        t.meid_hex_14,
                        t.wifi,
                        t.mac,
                        t.mac_1,
                        t.mac_2,
                        t.ean_upc_code,
                        t.software_version,
                        t.spc,
                        t.sim_card_no,
                        t.caton_id_hw,
                        t.pallet_id_hw,
                        t.pallet_id_cust,
                        t.caton_id_cust,
                        t2.prod_code_cust,
                        t2.prod_desc_cust
                    };

            var result = DB.LamdaQueryByPage(up, s, "delivery_id");
            return new
            {
                total_num = result.TotalRow,
                current_page = result.CurrentPage,
                total_page = result.TotalPage,
                page_size = result.Count_Of_OnePage,
                data = result.QueryData<FrameDLRObject>()
            };
        }

        [EWRARoute("post", "/dms/jiangsu/orders_hongxun")]
        [EWRARouteDesc("鸿迅推送DMS订单资料")]
        [EWRAEmptyValid("orders")]
        public object InsertOrdersFromHongxun()
        {
            var content = ComFunc.UrlDecode(ComFunc.nvl(PostDataD.orders));

            var up = DB.NewDBUnitParameter();

            var xml = FrameDLRObject.CreateInstance(ComFunc.Base64DeCode(content));
            if (xml != null)
            {
                if (xml.error == null)
                {
                    List<object> listorder = new List<object>();
                    List<object> listpay = new List<object>();
                    List<object> listgoods = new List<object>();
                    List<object> listgift = new List<object>();
                    //查看是否只有一笔资料
                    if (xml.request.body.order_lists.order is FrameDLRObject)
                    {
                        var orderitem = xml.request.body.order_lists.order;
                        listorder.Add(copyOrderItem(orderitem));
                        var order_no = orderitem.order_no;
                        //pay
                        if (orderitem.multiply_pay != null && orderitem.multiply_pay != "")
                        {
                            var payitem = orderitem.multiply_pay.pay;
                            if (payitem is FrameDLRObject)
                            {
                                listpay.Add(copyPayItem(payitem, order_no));
                            }
                            else
                            {
                                List<object> payarray = payitem;
                                foreach (var obj in payarray)
                                {
                                    listpay.Add(copyPayItem(obj, order_no));
                                }
                            }
                        }
                        //goods
                        if (orderitem.item_list != null && orderitem.item_list != "")
                        {
                            var goodsobj = orderitem.item_list.item;
                            if (goodsobj is FrameDLRObject)
                            {
                                listgoods.Add(copyGoodsItem(goodsobj, order_no));
                            }
                            else
                            {
                                List<object> goodsarray = goodsobj;
                                foreach (var obj in goodsarray)
                                {
                                    listgoods.Add(copyGoodsItem(obj, order_no));
                                }
                            }
                        }
                        //gift
                        if (orderitem.gift_list != null && orderitem.gift_list != "")
                        {
                            var giftsobj = orderitem.gift_list.gift;
                            if (giftsobj is FrameDLRObject)
                            {
                                listgift.Add(copyGiftItem(giftsobj, order_no));
                            }
                            else
                            {
                                List<object> giftsarray = giftsobj;
                                foreach (var obj in giftsarray)
                                {
                                    listgift.Add(copyGiftItem(obj, order_no));
                                }
                            }
                        }
                    }
                    else
                    {
                        List<object> list = xml.request.body.order_lists.order;
                        foreach (dynamic orderitem in list)
                        {
                            listorder.Add(copyOrderItem(orderitem));
                            var order_no = orderitem.order_no;
                            //pay
                            if (orderitem.multiply_pay != null && orderitem.multiply_pay != "")
                            {
                                var payitem = orderitem.multiply_pay.pay;
                                if (payitem is FrameDLRObject)
                                {
                                    listpay.Add(copyPayItem(payitem, order_no));
                                }
                                else
                                {
                                    List<object> payarray = payitem;
                                    foreach (var obj in payarray)
                                    {
                                        listpay.Add(copyPayItem(obj, order_no));
                                    }
                                }
                            }
                            //goods
                            if (orderitem.item_list != null && orderitem.item_list != "")
                            {
                                var goodsobj = orderitem.item_list.item;
                                if (goodsobj is FrameDLRObject)
                                {
                                    listgoods.Add(copyGoodsItem(goodsobj, order_no));
                                }
                                else
                                {
                                    List<object> goodsarray = goodsobj;
                                    foreach (var obj in goodsarray)
                                    {
                                        listgoods.Add(copyGoodsItem(obj, order_no));
                                    }
                                }
                            }
                            //gift
                            if (orderitem.gift_list != null && orderitem.gift_list != "")
                            {
                                var giftsobj = orderitem.gift_list.gift;
                                if (giftsobj is FrameDLRObject)
                                {
                                    listgift.Add(copyGiftItem(giftsobj, order_no));
                                }
                                else
                                {
                                    List<object> giftsarray = giftsobj;
                                    foreach (var obj in giftsarray)
                                    {
                                        listgift.Add(copyGiftItem(obj, order_no));
                                    }
                                }
                            }
                        }
                    }

                    foreach (var item in listorder)
                    {
                        //DB.QuickInsert(up, "orders", item);
                        var dobj = (FrameDLRObject)item;
                        foreach (var k in dobj.Keys)
                        {
                            up.SetValue(k, dobj.GetValue(k));
                        }
                        DB.NonQuery<JiangSuUnit>(up, "addorderinfo");
                    }
                    foreach (var item in listpay)
                    {
                        //DB.QuickInsert(up, "orders_paymode", item);
                        var dobj = (FrameDLRObject)item;
                        foreach (var k in dobj.Keys)
                        {
                            up.SetValue(k, dobj.GetValue(k));
                        }
                        DB.NonQuery<JiangSuUnit>(up, "addorderpay");
                    }
                    foreach (var item in listgoods)
                    {
                        //DB.QuickInsert(up, "orders_goods", item);
                        var dobj = (FrameDLRObject)item;
                        foreach (var k in dobj.Keys)
                        {
                            up.SetValue(k, dobj.GetValue(k));
                        }
                        DB.NonQuery<JiangSuUnit>(up, "addordergoods");
                    }
                    foreach (var item in listgift)
                    {
                        //DB.QuickInsert(up, "orders_gifts", item);
                        var dobj = (FrameDLRObject)item;
                        foreach (var k in dobj.Keys)
                        {
                            up.SetValue(k, dobj.GetValue(k));
                        }
                        DB.NonQuery<JiangSuUnit>(up, "addordergift");
                    }
                }

                return "ok";
            }
            else
            {
                return "格式不正确或内容有问题";
            }
        }

        #region 订单资料转化
        private object copyOrderItem(dynamic source)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.order_no = source.order_no;
            rtn.order_region = source.order_region;
            rtn.region_name = source.region_name;
            rtn.area_num = source.area_num;
            rtn.area_name = source.area_name;
            rtn.build_time = source.build_time;
            rtn.order_deliv_time = source.order_deliv_time;
            rtn.build_name = source.build_name;
            rtn.build_tel = source.build_tel;
            rtn.dept_code = source.dept_code;
            rtn.dept_name = source.dept_name;
            rtn.dist_code = source.dist_code;
            rtn.dist_name = source.dist_name;
            rtn.order_money = source.order_money;
            rtn.deliv_name = source.deliv_name;
            rtn.deliv_mobile = source.deliv_mobile;
            rtn.deliv_address = source.deliv_address;
            rtn.is_meeting = source.is_meeting;
            rtn.meeting_name = source.meeting_name;
            rtn.pay_mode = source.pay_mode;
            rtn.all_pay_money = source.all_pay_money;
            rtn.order_status = "HZTDFH";//待发货
            rtn.hongxun_order_no = source.hongxun_order_no;

            return rtn;
        }
        private object copyPayItem(dynamic source, string order_no)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.pay_time = source.pay_time;
            rtn.pay_money = ComFunc.nvl(source.pay_money) == "" ? null : double.Parse(ComFunc.nvl(source.pay_money));
            rtn.pay_type = source.pay_type;
            rtn.uid = Guid.NewGuid().ToString();
            rtn.order_no = order_no;

            return rtn;
        }
        private object copyGoodsItem(dynamic source, string order_no)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.detail_num = source.detail_num;
            rtn.goods_type = source.goods_type;
            rtn.goods_num = source.goods_num;
            rtn.goods_price = ComFunc.nvl(source.goods_price) == "" ? 0 : double.Parse(ComFunc.nvl(source.goods_price));
            rtn.goods_count = ComFunc.nvl(source.goods_count) == "" ? 0 : int.Parse(ComFunc.nvl(source.goods_price));
            rtn.order_no = order_no;

            return rtn;
        }
        private object copyGiftItem(dynamic source, string order_no)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.order_gift_num = source.order_gift_num;
            rtn.prom_id = source.prom_id;
            rtn.gift_type = source.gift_type;
            rtn.gift_num = source.gift_num;
            rtn.gift_name = source.gift_name;
            rtn.gift_count = ComFunc.nvl(source.gift_count) == "" ? 0 : int.Parse(ComFunc.nvl(source.gift_count));
            rtn.order_no = order_no;

            return rtn;
        }
        #endregion
    }
}
