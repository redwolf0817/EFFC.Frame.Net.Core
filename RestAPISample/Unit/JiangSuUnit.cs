using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPISample.Unit
{
    public class JiangSuUnit : BaseDBUnit
    {
        void addOrderInfo(UnitParameter up, dynamic sql)
        {
            sql.sql = @"INSERT INTO orders (order_no,
order_status,
order_region,
region_name,
area_num,
area_name,
build_time,
order_deliv_time,
build_name,
build_tel,
dept_code,
dept_name,
dist_code,
dist_name,
order_money,
deliv_name,
deliv_mobile,
deliv_address,
is_meeting,
meeting_name,
pay_mode,
all_pay_money,
hongxun_order_no)  SELECT DISTINCT $order_no as order_no,
$order_status as order_status,
$order_region as order_region,
$region_name as region_name,
$area_num as area_num,
$area_name as area_name,
$build_time as build_time,
$order_deliv_time as order_deliv_time,
$build_name as build_name,
$build_tel as build_tel,
$dept_code as dept_code,
$dept_name as dept_name,
$dist_code as dist_code,
$dist_name as dist_name,
$order_money as order_money,
$deliv_name as deliv_name,
$deliv_mobile as deliv_mobile,
$deliv_address as deliv_address,
$is_meeting as is_meeting,
$meeting_name as meeting_name,
$pay_mode as pay_mode,
$all_pay_money as all_pay_money,
$hongxun_order_no as hongxun_order_no  
WHERE not exists (select order_no from orders where order_no = $order_no)";
        }

        void addordergift(UnitParameter up, dynamic sql)
        {
            sql.sql = @"INSERT INTO orders_gifts(
order_no,
order_gift_num,
prom_id,
gift_type,
gift_num,
gift_name,
gift_count
)select DISTINCT
$order_no as order_no,
$order_gift_num as order_gift_num,
$prom_id as prom_id,
$gift_type as gift_type,
$gift_num as gift_num,
$gift_name as gift_name,
$gift_count as gift_count
where  not exists (select order_no from orders_gifts where order_no=$order_no and gift_num = $gift_num);
";
        }

        void addordergoods(UnitParameter up, dynamic sql)
        {
            sql.sql = @"INSERT INTO orders_goods(
order_no,
detail_num,
goods_type,
goods_num,
goods_price,
goods_count
)select DISTINCT
$order_no as order_no,
$detail_num as detail_num,
$goods_type as goods_type,
$goods_num as goods_num,
$goods_price as goods_price,
$goods_count as goods_count
where not exists (select order_no from orders_goods where order_no=$order_no and detail_num = $detail_num);
";
        }
        void addorderpay(UnitParameter up, dynamic sql)
        {
            sql.sql = @"INSERT INTO orders_paymode(
uid,
order_no,
pay_time,
pay_money,
pay_type
)select DISTINCT
$uid as uid,
$order_no as order_no,
$pay_time as pay_time,
$pay_money as pay_money,
$pay_type as pay_type
where not exists (select order_no from orders_paymode where order_no=$order_no);
";
        }
    }
}
