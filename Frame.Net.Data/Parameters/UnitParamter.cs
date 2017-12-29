using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Data.Parameters
{
    public class UnitParameter:ParameterStd
    {
        DBOParameterCollection _spp = null;
        public UnitParameter()
        {
            Count_Of_OnePage = 5;
            CurrentPage = 0;
            ToPage = 0;
            _spp = new DBOParameterCollection();
        }

        public IDBAccessInfo Dao
        {
            get
            {
                return (IDBAccessInfo)GetValue("Dao");
            }
            set
            {
                SetValue("Dao", value);
            }
        }
        /// <summary>
        /// 存储过程的参数集
        /// </summary>
        public DBOParameterCollection SPParameter
        {
            get
            {
                return _spp;
            }
        }

        /// <summary>
        /// 一页显示资料笔数
        /// </summary>
        public int Count_Of_OnePage
        {
            get
            {
                return (int)GetValue("Count_Of_OnePage");
            }
            set
            {
                SetValue("Count_Of_OnePage", value);
            }
        }
        /// <summary>
        /// 当前页数
        /// </summary>
        public int CurrentPage
        {
            get
            {
                return (int)GetValue("CurrentPage");
            }
            set
            {
                SetValue("CurrentPage", value);
            }
        }
        /// <summary>
        /// 转转页数
        /// </summary>
        public int ToPage
        {
            get
            {
                return (int)GetValue("ToPage");
            }
            set
            {
                SetValue("ToPage", value);
            }
        }

        public string DBConnString
        {
            get
            {
                return ComFunc.nvl(GetValue("DBConnString"));
            }
            set
            {
                SetValue("DBConnString", value);
            }
        }
        /// <summary>
        /// 清空参数，但保留dao和dbconnstring,resource manager和transtoken
        /// </summary>
        public void ClearParameters()
        {
            IDBAccessInfo dao = Dao;
            string dbs = DBConnString;
            var rm = Resources;
            var token = CurrentTransToken;

            this.Clear();
            this.Dao = dao;
            this.DBConnString = dbs;
            SetValue(ParameterKey.TOKEN, token);
            SetValue(ParameterKey.RESOURCE_MANAGER, rm);
        }
        /// <summary>
        /// Unit中的操作标记
        /// </summary>
        public string OperationType
        {
            get;
            set;
        }

        public void SetValue(FrameDLRObject obj)
        {
            foreach (var key in obj.Keys)
            {
                SetValue(key, obj.GetValue(key));
            }
        }

        public override object Clone()
        {
            return this.Clone<UnitParameter>();
        }
    }
}
