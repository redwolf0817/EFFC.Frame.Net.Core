using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Test
{
    public class HWAPITest
    { 
        public static void Test()
        {
            var hwapi = new HWAPI("com.huawei.hcop.open.zhongyou", "bzBTJVN1JDdGaW5VflJZRw==", "https://apigw-beta.huawei.com/api/cbg/open");
            var result = "";//hwapi.QueryPO();
            Console.WriteLine(result);
            result = hwapi.CreatDelivery("M100001801650Z", new
            {
                PROD_CODE_SALE = "51090QWK",
                PROD_CODE_CUST = "",
                CUST_PURCHASE_ITEM = "",
                CUST_PURCHASE_QUANTITY = "1",
                CUST_PURCHASE_UNIT_PRICE = "",
                SHIPPING_TO_WAREHOUSE_ID = "ZY7450",
                COMMENTS = "abcd",
                FREE_TEXT = "P00032"
            });
            Console.WriteLine(result);
            result = hwapi.QueryDelivery("M100041712200W", "M100041712200W");
            Console.WriteLine(result);
            result = hwapi.QueryIMEI("112651751");
            Console.WriteLine(result);
            result = hwapi.Inbound(new
            {
                delivery_id = "112603006",
                prod_code_sale = "53035657",
                actual_inbound_qty = "10",
                actual_inbound_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            Console.WriteLine(result);
        }
        public class HWAPI
        {
            public HWAPI(string appid,string token,string url,string user_id= "oa_app_0003",string cust_id= "1000131458")
            {
                APP_ID = appid;
                Token = token;
                User_ID = user_id;
                Cust_ID = cust_id;
                Server_URL = url;
            }
            public string APP_ID
            {
                get;protected set;
            }
            public string Token
            {
                get; protected set;
            }
            public string User_ID
            {
                get; protected set;
            }
            public string Cust_ID
            {
                get; protected set;
            }
            public string Server_URL
            {
                get;protected set;
            }
            /// <summary>
            /// 执行接口发送操作
            /// </summary>
            /// <param name="url">目标地址</param>
            /// <param name="data">请求的参数，json格式</param>
            /// <returns></returns>
            public string Send(string url, object data)
            {
                var rtn = "";
                var dt = DateTime.Now;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                try
                {
                    FrameDLRObject content = (FrameDLRObject)FrameDLRObject.CreateInstance(data, EFFC.Frame.Net.Base.Constants.FrameDLRFlags.SensitiveCase);
                    Console.WriteLine($"body={content.ToJSONString()}");
                    req.Headers.Add("X-HW-ID", APP_ID);
                    req.Headers.Add("X-HW-APPKEY", Token);

                    byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(content.ToJSONString());
                    req.Method = "POST";
                    req.ContentType = "application/json;charset=utf-8";
                    req.ContentLength = requestBytes.Length;
                    req.Timeout = 60000;
                    req.ReadWriteTimeout = 60000;
                    Stream requestStream = req.GetRequestStream();
                    requestStream.Write(requestBytes, 0, requestBytes.Length);
                    requestStream.Close();
                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    Console.WriteLine($"send cast time:{(DateTime.Now - dt).TotalMilliseconds}ms");
                    StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
                    rtn = sr.ReadToEnd();
                    requestStream = null;
                    sr.Close();
                    res.Close();
                }
                catch (WebException ex)
                {
                    var rep = ex.Response;
                    var str = new StreamReader(rep.GetResponseStream(), System.Text.Encoding.UTF8).ReadToEnd();
                    rtn = str;
                }
                finally
                {
                    req = null;
                }

                return rtn;
            }

            /// <summary>
            /// PO资料查询
            /// </summary>
            /// <param name="start_time">起始时间</param>
            /// <param name="end_time">结束时间</param>
            /// <param name="status">状态，默认为“2”</param>
            /// <param name="topage">翻页，默认第一页</param>
            /// <param name="pagesize">每页资料笔数，默认100</param>
            /// <returns></returns>
            public string QueryPO(DateTime? start_time = null, DateTime? end_time = null, string status = "2", int topage = 1, int pagesize = 100)
            {
                var url = $"{Server_URL}/po/query";
                var data = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                data.USER_ID = User_ID;
                data.CUST_ID = Cust_ID;
                data.STATUS = status;
                data.PAGE_SIZE = pagesize;
                data.CURRENT_PAGE = topage;

                if (start_time != null && start_time != DateTime.MinValue)
                {
                    data.START_DATE = start_time.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (end_time != null && end_time != DateTime.MaxValue)
                {
                    data.END_DATE = end_time.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                return Send(url, data);
            }
            /// <summary>
            /// 发货指令
            /// </summary>
            /// <param name="hw_contract_no">华为合同号，即PO单号</param>
            /// <param name="items">物料数组，结构{
            /// PROD_CODE_SALE:"华为产品Item,即待发货的华为物料编码，必填，来自PO单",
            /// PROD_CODE_CUST:"客户Item，即中邮的物料编码，非必填，来自PO单，不填时默认为空",
            /// CUST_PURCHASE_ITEM:"客户采购凭证号，由客户定义，匹配到客户采购凭证的唯一一行数据。后面发货状态查询时可以回传，非必填",
            /// CUST_PURCHASE_QUANTITY:"客户采购数量，必填",
            /// CUST_PURCHASE_UNIT_PRICE:"客户采购单价（含税人民币），非必填",
            /// SHIPPING_TO_WAREHOUSE_ID:"目标仓库编码（唯一标识）。编码为中邮的编码，必填",
            /// COMMENTS:"备注，非必填",
            /// FREE_TEXT:"扩展文本信息,用于中邮保存自己的资料信息，长度不超过1000个字符，可以保存发货单，分货单号等资料信息，火猫发货涨停查询时可以回传，非必填"
            /// }</param>
            /// <returns>如果缺少参数则返回null</returns>
            public string CreatDelivery(string hw_contract_no, params object[] items)
            {
                if (string.IsNullOrEmpty(hw_contract_no)) return null;
                if (items == null && items.Length <= 0) return null;

                var url = $"{Server_URL}/delivery/create";
                var data = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                data.USER_ID = User_ID;
                data.CUST_ID = Cust_ID;
                data.HW_CONTRACT_NO = hw_contract_no;
                data.CUST_CONTRACT_NO = null;
                var list = new List<object>();

                foreach(var item in items)
                {
                    list.Add(FrameDLRObject.CreateInstance(item, FrameDLRFlags.SensitiveCase));
                }
                data.ITEMS = list;

                return Send(url, data);
            }
            /// <summary>
            /// 执行发货状态查询
            /// </summary>
            /// <param name="hw_contract_no">华为合同号，即PO单号</param>
            /// <param name="cust_contract_no">中邮合同号，即PO单号</param>
            /// <param name="asd_start_date">华为实际发货开始时间,默认为当前时间往前推3天</param>
            /// <param name="asd_end_date">华为实际发货结束时间，默认为当天</param>
            /// <param name="shipping_status">华为发货的状态：0:unshipped,1:shipped,2:all</param>
            /// <param name="topage">翻页，默认第一页</param>
            /// <param name="pagesize">每页资料笔数，默认100</param>
            /// <returns></returns>
            public string QueryDelivery(string hw_contract_no,string cust_contract_no,
                DateTime? asd_start_date=null,
                DateTime? asd_end_date=null,
                string shipping_status="1",
                int topage = 1, 
                int pagesize = 100)
            {
                var url = $"{Server_URL}/asn";
                var data = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                data.cust_id = Cust_ID;
                data.delivery_id = "";
                data.packing_list_no = "";
                data.hw_contract_no = hw_contract_no;
                data.cust_contract_no = cust_contract_no;
                data.cust_purchase_no = "";
                data.prod_code_sale = "";
                data.prod_model_ext = "";
                data.cust_purchase_item = "";
                data.asd_start_date = asd_start_date == null ?$"{DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd")} 00:00:00" : asd_start_date.Value.ToString("yyyy-MM-dd HH:mm:ss");
                data.asd_end_date = asd_end_date == null ? $"{DateTime.Now.ToString("yyyy-MM-dd")} 23:59:59" : asd_end_date.Value.ToString("yyyy-MM-dd HH:mm:ss");
                data.shipping_status = shipping_status;
                data.page_size = ""+pagesize;
                data.current_page = "" + topage;
                return Send(url, data);
            }
            /// <summary>
            /// 查询IMEI码
            /// </summary>
            /// <param name="delivery_id">华为DELIVERY ID</param>
            /// <param name="topage">翻页，默认第一页</param>
            /// <param name="pagesize">每页资料笔数，默认100</param>
            /// <returns></returns>
            public string QueryIMEI(string delivery_id,
                int topage = 1,
                int pagesize = 100)
            {
                var url = $"{Server_URL}/imei";
                var data = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                data.cust_id = Cust_ID;
                data.delivery_id = delivery_id;
                data.page_size = "" + pagesize;
                data.current_page = "" + topage;
                return Send(url, data);
            }
            /// <summary>
            /// 入库回写
            /// </summary>
            /// <param name="items">入库物料资料，结构{
            /// delivery_id:"华为DELIVERY ID,必输",
            /// prod_code_sale:"华为销售编码，必输",
            /// actual_inbound_qty:"客户实际入库数量,必输",
            /// actual_inbound_date:"客户实际入库完成时间,格式：yyyy-MM-dd HH:mm:ss"
            /// }</param>
            /// <returns></returns>
            public string Inbound(params object[] items)
            {
                if (items == null && items.Length <= 0) return null;
                var url = $"{Server_URL}/grn";
                var data = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                data.cust_id = Cust_ID;
                var list = new List<object>();

                foreach (var item in items)
                {
                    list.Add(FrameDLRObject.CreateInstance(item, FrameDLRFlags.SensitiveCase));
                }
                data.inboundData = list;

                return Send(url, data);
            }
        }
        
    }
}
