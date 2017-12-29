using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Json;
using System.Text;
using System.Threading.Tasks;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Data.DataConvert;
using EFFC.Frame.Net.Base.Data.Base;

namespace EFFC.Frame.Net.Data.UnitData
{
    public class UnitDataCollection:DataCollection
    {
        /// <summary>
        /// 查询的数据集
        /// </summary>
        public DataSetStd QueryDatas
        {
            get
            {
                return (DataSetStd)GetValue("QueryDatas");
            }
            set
            {
                SetValue("QueryDatas", value);
            }
        }
        /// <summary>
        /// 查询的数据集
        /// </summary>
        public DataTableStd QueryTable
        {
            get
            {
                return (DataTableStd)GetValue("QueryTable");
            }
            set
            {
                SetValue("QueryTable", value);
            }
        }
        /// <summary>
        /// 查询的数据集，将QueryDatas中指定的table转化成List
        /// </summary>
        /// <typeparam name="T">dto数据类型</typeparam>
        /// <param name="tableindex">table index</param>
        /// <returns></returns>
        public List<T> QueryData<T>(int tableindex)
        {
            DataTable2List<T> c = new DataTable2List<T>();
            List<T> rtn = c.ConvertTo(QueryDatas[tableindex]);
            return rtn;
        }
        /// <summary>
        /// 查询的数据集，将QueryTable转化成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> QueryData<T>()
        {
            DataTable2List<T> c = new DataTable2List<T>();
            List<T> rtn = c.ConvertTo(QueryTable);
            return rtn;
        }
        /// <summary>
        /// 查询的数据转化成json数据格式
        /// </summary>
        /// <returns></returns>
        public JsonCollection QueryData2Json()
        {
            DataTable2Json c = new DataTable2Json();
            JsonCollection rtn = c.ConvertTo(QueryTable);
            return rtn;
        }
        /// <summary>
        /// mongo的数据结果集
        /// </summary>
        public List<FrameDLRObject> MongoListData
        {
            get
            {
                return (List<FrameDLRObject>)GetValue("MongoListData");
            }
            set
            {
                SetValue("MongoListData", value);
            }
        }
        /// <summary>
        /// 查询的数据转化成json数据格式
        /// </summary>
        /// <returns></returns>
        public string QueryByPage2Json()
        {
            QueryByPage2Json c = new QueryByPage2Json();
            string rtn = c.ConvertTo(this);
            return rtn;
        }
        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPage
        {
            get;
            set;
        }
        /// <summary>
        /// 當前頁數
        /// </summary>
        public int CurrentPage
        {
            get;
            set;
        }
        /// <summary>
        /// 一頁顯示多少筆
        /// </summary>
        public int Count_Of_OnePage
        {
            get;
            set;
        }
        /// <summary>
        /// 總比數
        /// </summary>
        public int TotalRow
        {
            get;
            set;
        }

        public override object Clone()
        {
            var rtn = this.Clone<UnitDataCollection>();
            rtn.TotalPage = this.TotalPage;
            rtn.TotalRow = this.TotalRow;
            rtn.Count_Of_OnePage = this.Count_Of_OnePage;
            rtn.CurrentPage = this.CurrentPage;
            return rtn;
        }
    }
}
