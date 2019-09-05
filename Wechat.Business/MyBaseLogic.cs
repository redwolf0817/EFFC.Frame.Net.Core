using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.WeixinWeb.Logic;
using EFFC.Frame.Net.Resource.Sqlite;
using EFFC.Frame.Net.Resource.SQLServer;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Extends.LinqDLR2SQL;

namespace Wechat.Business
{
    public abstract partial class MyBaseLogic:QiyeWxGoLogic
    {
        MyDBHelper _db;
        /// <summary>
        /// db操作相关
        /// </summary>
		public new MyDBHelper DB
        {
            get
            {
                if (_db == null)
                    _db = new MyDBHelper(this);

                return _db;
            }
        }

        public class MyDBHelper : DBHelper
        {
            MyBaseLogic _logic = null;
            static string ToPage_Key = "toPage";
            static string Count_per_Page_Key = "Count_per_Page";
            static string CurrentPage_Key = "CurrentPage";
            static string Total_Page_Key = "Total_Page";
            static string Total_Row_Key = "Total_Row";


            public MyDBHelper(MyBaseLogic logic)
                : base(logic)
            {
                _logic = logic;
            }

            public override UnitParameter NewDBUnitParameter()
            {
                var rtn = base.NewDBUnitParameter<SqliteAccess>();
                rtn.Dao.Open(rtn.DBConnString);
                return rtn;
            }
            public override LinqDLRTable LamdaTable(string tablename, string alianname = "", DBType dbtype = DBType.Sqlite)
            {
                return base.LamdaTable(tablename, alianname, dbtype);
            }
            public UnitParameter NewDBUnitParameter4Business()
            {
                var rtn = base.NewDBUnitParameter<SQLServerAccess>();
                rtn.Dao.Open(ComFunc.nvl(_logic.Configs["FrontBusiConnection"]));
                return rtn;
            }

            public override UnitDataCollection QueryByPage<T>(UnitParameter p, string actionflag)
            {
                InitUPPage(p);
                UnitDataCollection rtn = base.QueryByPage<T>(p, actionflag);

                _logic.CallContext_DataCollection.WebData.SetValue(Count_per_Page_Key, rtn.Count_Of_OnePage);
                _logic.CallContext_DataCollection.WebData.SetValue(CurrentPage_Key, rtn.CurrentPage);
                _logic.CallContext_DataCollection.WebData.SetValue(Total_Page_Key, rtn.TotalPage);
                _logic.CallContext_DataCollection.WebData.SetValue(Total_Row_Key, rtn.TotalRow);
                return rtn;
            }

            public override UnitDataCollection Excute(UnitParameter p, DBExpress express,bool islog= false)
            {
                if(express.CurrentAct == DBExpress.ActType.QueryByPage)
                {
                    InitUPPage(p);
                }

                return base.Excute(p, express);
            }

            public override UnitDataCollection Excute(UnitParameter p, FrameDLRObject json, bool islog = false)
            {
                InitUPPage(p);
                return base.Excute(p, json);
            }
            public override UnitDataCollection Excute(UnitParameter p, string json, bool islog = false)
            {
                InitUPPage(p);
                return base.Excute(p, json);
            }

            private void InitUPPage(UnitParameter p)
            {
                if (ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, ToPage_Key]) != "")
                {
                    p.ToPage = IntStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, ToPage_Key]);
                }
                else if (ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.QUERY_STRING, ToPage_Key]) != "")
                {
                    p.ToPage = IntStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.QUERY_STRING, ToPage_Key]);
                }
                //easyui使用的参数
                else if (ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.QUERY_STRING, "page"]) != "")
                {
                    p.ToPage = IntStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.QUERY_STRING, "page"]);
                }
                else if (ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "page"]) != "")
                {
                    p.ToPage = IntStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "page"]);
                }
                else
                {
                    p.ToPage = 1;
                }

                if (ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, Count_per_Page_Key]) != "")
                {
                    p.Count_Of_OnePage = IntStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, Count_per_Page_Key]);
                }
                else if (ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.QUERY_STRING, Count_per_Page_Key]) != "")
                {
                    p.Count_Of_OnePage = IntStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.QUERY_STRING, Count_per_Page_Key]);
                }
                //easyui使用的参数
                else if (ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.QUERY_STRING, "rows"]) != "")
                {
                    p.Count_Of_OnePage = IntStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.QUERY_STRING, "rows"]);
                }
                else if (ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "rows"]) != "")
                {
                    p.Count_Of_OnePage = IntStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "rows"]);
                }
                else
                {
                    p.Count_Of_OnePage = _logic.CallContext_Parameter.WebParam[DomainKey.CONFIG, Count_per_Page_Key] != null ? IntStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.CONFIG, Count_per_Page_Key]).Value : 10;
                }
            }
        }
    }
}
