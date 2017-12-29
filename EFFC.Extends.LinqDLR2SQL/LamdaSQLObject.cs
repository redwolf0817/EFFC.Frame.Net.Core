using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL
{
    /// <summary>
    /// 用于LinqDLR2Sql构建动态栏位使用
    /// </summary>
    /// <typeparam name="TLinqDLRColumn"></typeparam>
    public class LamdaSQLObject<TLinqDLRColumn> : MyDynamicMetaProvider where TLinqDLRColumn:LinqDLRColumn
    {
        Dictionary<string, LinqDLRColumn> columns = new Dictionary<string, LinqDLRColumn>();
        public string BelongToTable
        {
            get;
            set;
        }
        public LamdaSQLObject(string table)
        {
            BelongToTable = table;
        }
        protected override object GetMetaValue(string key)
        {
            if (!columns.ContainsKey(key.ToLower()))
            {
                columns.Add(key.ToLower(), LinqDLRColumn.New<TLinqDLRColumn>($"{(BelongToTable==""?"":BelongToTable+".")}{key}", this));
            }
            return columns[key.ToLower()];
        }

        protected override object InvokeMe(string methodInfo, params object[] args)
        {
            return this;
        }

        protected override object SetMetaValue(string key, object value)
        {
            return this;
        }
    }
}
