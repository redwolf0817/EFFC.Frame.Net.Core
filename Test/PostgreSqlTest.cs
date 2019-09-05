using EFFC.Frame.Net.Resource.Postgresql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class PostgreSqlTest
    {
        public static void Test()
        {
            using (PostgreSqlAccess access = new PostgreSqlAccess())
            {
                access.Open("Host=10.15.1.244;Username=psi_ba;Password=111111;Database=DH_ZHONGYOU01");
                var result = access.Query(@"select
        id,
        comp_code,
        prod_code,
        qty,
        form_date,
        modi_time,
        case busi_type 
            when '总部发货' then 1 
            when '本省发货' then 2 
            else null 
        end as type,
        state,
        dealer_code,
        dealer_code2,
        dealer_code3  
    from
        t_terminal_so_line 
limit 100 OFFSET 0", null);
                access.Close();

            }
        }
    }
}
