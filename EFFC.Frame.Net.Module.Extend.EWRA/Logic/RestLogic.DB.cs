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
            public const string ToPage_Key = "page";
            public const string Count_per_Page_Key = "limit";
            public RestDBHeler(RestLogic logic) : base(logic)
            {
                _logic = logic;
                Is_Auto_To_Last_Page = false;
            }
            /// <summary>
            /// 当翻页查询的时候topage超过最后一页的时，是否自动调到最后一页显示资料,默认为false（基于rest和移动端应用的标准）
            /// </summary>
            public bool Is_Auto_To_Last_Page
            {
                get;
                set;
            }

            public override UnitDataCollection QueryByPage<T>(UnitParameter p, string actionflag)
            {
                InitUPPage(p);
                var topage = p.ToPage;
                var rtn = base.QueryByPage<T>(p, actionflag);
                if (!Is_Auto_To_Last_Page)
                {
                    //当topage超过总页数时，框架会自动调整到最后一页，但对于restapi来说，这是不必要的，因此当发现topage超过实际总页数时则资料清空处理
                    if (topage > rtn.TotalPage)
                    {
                        foreach (var table in rtn.QueryDatas.Tables)
                        {
                            table.ClearData();
                        }
                    }
                    rtn.CurrentPage = topage;
                }
                
                return rtn;
            }

            protected void InitUPPage(UnitParameter p)
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
                if (p.ToPage <= 0) p.ToPage = 1;

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
                    p.Count_Of_OnePage = _logic.CallContext_Parameter[DomainKey.CONFIG, "Count_per_Page"] != null ? IntStd.ParseStd(_logic.CallContext_Parameter[DomainKey.CONFIG, "Count_per_Page"]).Value : 10;
                }
               
            }
        }
    }
}
