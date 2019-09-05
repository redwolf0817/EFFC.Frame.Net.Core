using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBAApplication.Unit
{
    class SoUnit : BaseDBUnit
    {
        void sample_queryByPage(UnitParameter up, dynamic sql)
        {
            if (ComFunc.nvl(up.GetValue("prod_code_custs")) == "")
            {
                sql.sql = @"select a.* from hw_po_line a join hw_po b on a.hw_contract_no=b.hw_contract_no where a.quantity>ifnull(a.freeze_quantity,0)+ifnull(a.used_quantity,0) and b.status='Signed' and b.is_valid_ebs=1 ";
            }
            else
            {
                sql.presql = @"call PROCEDURE_split(@prod_code_custs,',');";
                sql.sql = @"select a.* from hw_po_line a join hw_po b on a.hw_contract_no=b.hw_contract_no where a.quantity>ifnull(a.freeze_quantity,0)+ifnull(a.used_quantity,0) and b.status='Signed' and b.is_valid_ebs=1 and a.prod_code_cust in (select * from splittable)";
            }

            sql.orderby = @"hw_contract_no,po_line_no";
        }
        void sample(UnitParameter up, dynamic sql)
        {
            sql.sql = @"INSERT INTO PO_FEE(
hw_contract_no,
line_no,
line_type,
desc,
prod_model,
amount,
comments
)
SELECT DISTINCT
$hw_contract_no as hw_contract_no,
$line_no as line_no,
$line_type as line_type,
$desc as desc,
$prod_model as prod_model,
$amount as amount,
$comments as comments
where not exists(select hw_contract_no from PO_FEE where hw_contract_no = $hw_contract_no and line_no = $line_no)";
        }

        void updateinfo(UnitParameter up, dynamic sql)
        {
            sql.sql = @"delete from t_sale_so where locate(concat(',',id,','),@uid);delete from t_sale_so_empty where locate(concat(',',id,','),@uid); update tempt_terminal_so_line set status='Y' where locate(concat(',',id,','),@uid)";
        }
    }
}
