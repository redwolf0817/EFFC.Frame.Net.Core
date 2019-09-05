using EFFC.Frame.Net.Base.ResouceManage.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class OracleTest
    {
        public static void Test()
        {
            //var conn = "DATA SOURCE=10.2.6.9:1601/whls;PASSWORD=whlss01;PERSIST SECURITY INFO=True;USER ID=whls";
            var conn = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.2.6.9)(PORT=1601)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=TEST)));User Id=whls;Password=whlss01;";
            using (var c = new OracleAccess())
            {
                c.Open(conn);
                var result =c.Query("select * from CUX.CUX_B2B_CUST_RECIPT_CLAIM_INT;", new EFFC.Frame.Net.Unit.DB.Parameters.DBOParameterCollection());
            }
        }
    }
}
