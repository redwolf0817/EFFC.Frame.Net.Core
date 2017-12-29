using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Linq2SQL
{
    public class LamdaSQLObject : MyDynamicMetaProvider
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
                columns.Add(key.ToLower(), new LinqDLRColumn(key, this));
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
