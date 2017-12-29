using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Base.Data;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Logic
{
    public partial class RestLogic
    {
        DBHelper _mydb;
        /// <summary>
        /// Token中payload的参数信息
        /// </summary>
        public override DBHelper DB
        {
            get
            {
                if (_mydb == null)
                {
                    _mydb = new RestDBHeler(this);
                }
                return _mydb;
            }
        }

        public class RestDBHeler:DBHelper
        {
            RestLogic _logic = null;
            const string ToPage_Key = "page";
            const string Count_per_Page_Key = "limit";
            public RestDBHeler(RestLogic logic) : base(logic)
            {
                _logic = logic;
            }

            public override UnitDataCollection QueryByPage<T>(UnitParameter p, string actionflag)
            {
                InitUPPage(p);
                return base.QueryByPage<T>(p, actionflag);
            }

            private void InitUPPage(UnitParameter p)
            {
                if (ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.QUERY_STRING, ToPage_Key]) != "")
                {
                    p.ToPage = IntStd.ParseStd(_logic.CallContext_Parameter[DomainKey.QUERY_STRING, ToPage_Key]);
                }
                else if (ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.POST_DATA, ToPage_Key]) != "")
                {
                    p.ToPage = IntStd.ParseStd(_logic.CallContext_Parameter[DomainKey.POST_DATA, ToPage_Key]);
                }
                else
                {
                    p.ToPage = 1;
                }

                if (ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.QUERY_STRING, Count_per_Page_Key]) != "")
                {
                    p.Count_Of_OnePage = IntStd.ParseStd(_logic.CallContext_Parameter[DomainKey.QUERY_STRING, Count_per_Page_Key]);
                }
                else if (ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.POST_DATA, Count_per_Page_Key]) != "")
                {
                    p.Count_Of_OnePage = IntStd.ParseStd(_logic.CallContext_Parameter[DomainKey.POST_DATA, Count_per_Page_Key]);
                }
                else
                {
                    p.Count_Of_OnePage = _logic.CallContext_Parameter[DomainKey.CONFIG, Count_per_Page_Key] != null ? IntStd.ParseStd(_logic.CallContext_Parameter[DomainKey.CONFIG, Count_per_Page_Key]).Value : 10;
                }
               
            }
        }
    }
}
