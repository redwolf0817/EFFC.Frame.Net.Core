using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.ResouceManage.DB;

namespace EFFC.Frame.Net.Base.Parameter
{
    [Serializable]
    public class DBAPageP : ParameterStd
    {
        public DBAPageP()
        {
            SetValue("SQL", "");
            SetValue("SQL_Parameters", new DBOParameterCollection());
            SetValue("Count_of_OnePage", 0);
            SetValue("OrderBy", "");
            SetValue("Current_Page", 0);
        }

        public string SQL
        {
            get
            {
                return GetValue<string>("SQL");
            }
            set
            {
                this.SetValue("SQL", value);
            }
        }
        public DBOParameterCollection SQL_Parameters
        {
            get
            {
                return GetValue<DBOParameterCollection>("SQL_Parameters");
            }
            set
            {
                this.SetValue("SQL_Parameters", value);
            }
        }
        public int Count_of_OnePage
        {
            get
            {
                return GetValue<int>("Count_of_OnePage");
            }
            set
            {
                this.SetValue("Count_of_OnePage", value);
            }
        }
        public string OrderBy
        {
            get
            {
                return GetValue<string>("OrderBy");
            }
            set
            {
                this.SetValue("OrderBy", value);
            }
        }
        public int CurrentPage
        {
            get
            {
                return GetValue<int>("Current_Page");
            }
            set
            {
                this.SetValue("Current_Page", value);
            }
        }
    }
}
