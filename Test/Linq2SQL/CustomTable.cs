using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Test.Linq2SQL
{
    public class CustomTable : MyDynamicMetaProvider
    {
        public string ParamFlag
        {
            get
            {
                return "@";
            }
        }
        public string LinkFlag
        {
            get { return "&"; }
        }
        public Dictionary<string, object> SQLParameters
        {
            get;
            private set;
        }
        public string SQL
        {
            get;
            private set;
        }
        object[] parameters = null;
        protected Dictionary<string, LinqDLRColumn> Columns
        {
            get;
            private set;
        }
        protected string TableName
        {
            get;
            private set;
        }
        protected string AliasName
        {
            get;
            private set;
        }
        protected string Conditions
        {
            get;
            private set;
        }
        protected string OrderByString
        {
            get;
            private set;
        }
        protected string SelectString
        {
            get;
            private set;
        }
        public CustomTable(string tablename, string aliasName = "", params object[] p)
        {
            TableName = tablename;
            AliasName = aliasName == "" ? tablename : aliasName;
            parameters = p;
            Columns = new Dictionary<string, LinqDLRColumn>();
            SQLParameters = new Dictionary<string, object>();
        }

        public CustomTable Select(Func<dynamic, object> q)
        {
            var c = q.Invoke(this);
            if (c is CustomTable)
            {
                SelectString = $"{AliasName}.*";
            }
            else if (c is LinqDLRColumn)
            {
                SelectString = $"{AliasName}.{((LinqDLRColumn)c).Name}";
            }
            else if (c is string)
            {
                SelectString = ComFunc.nvl(c);
            }
            else
            {
                var fields = c.GetType().GetTypeInfo().GetFields(BindingFlags.CreateInstance | BindingFlags.Public);
                if (fields.Length <= 0)
                {
                    SelectString = $"{AliasName}.*";
                }
                else
                {
                    foreach (var f in fields)
                    {
                        if (f.FieldType == typeof(LinqDLRColumn))
                        {
                            LinqDLRColumn s = (LinqDLRColumn)f.GetValue(c);
                            SelectString += $",{s.ToString()} AS {f.Name}";
                        }
                        else
                        {
                            SelectString += $",{f.Name}";
                        }
                    }
                    SelectString = SelectString.Substring(1);
                }
            }

            SQL = $"SELECT {SelectString} FROM {TableName}";
            if (AliasName != TableName)
            {
                SQL += $" {AliasName}";
            }
            if (!string.IsNullOrEmpty(Conditions))
            {
                SQL += $" WHERE {Conditions}";
            }
            if (!string.IsNullOrEmpty(OrderByString))
            {
                SQL += $" ORDER BY {OrderByString}";
            }
            return this;
        }

        public CustomTable Where(Func<dynamic, CustomOperator> w)
        {
            var s = w.Invoke(this);
            Conditions = s.Result;
            return this;
        }
        public CustomTable Where(Func<dynamic, int, CustomOperator> w)
        {
            var s = w.Invoke(this, 0);
            Conditions = s.Result;
            return this;
        }
        public CustomTable OrderBy(Func<dynamic, LinqDLRColumn> ob)
        {
            var s = ob.Invoke(this).ToString();
            OrderByString = $"{s}";
            return this;
        }
        public CustomTable OrderByDescending(Func<dynamic, LinqDLRColumn> ob)
        {
            var s = ob.Invoke(this).ToString();
            OrderByString = $"{s} desc";
            return this;
        }
        public CustomTable ThenBy(Func<dynamic, LinqDLRColumn> ob)
        {
            var s = ob.Invoke(this).ToString();
            OrderByString += $",{s}";
            return this;
        }
        public CustomTable ThenByDescending(Func<dynamic, LinqDLRColumn> ob)
        {
            var s = ob.Invoke(this).ToString();
            OrderByString += $",{s} desc";
            return this;
        }

        public CustomTable Join(CustomTable ob, Func<dynamic, object> a, Func<dynamic, object> b, Func<dynamic, dynamic, object> c)
        {
            var rea = a.Invoke(this);
            var reb = b.Invoke(ob);
            var rec = c.Invoke(this, ob);
            return ob;
        }

        public CustomTable GroupJoin(CustomTable ob, Func<dynamic, object> a, Func<dynamic, object> b, Func<dynamic, dynamic, object> c)
        {
            return this;
        }

        public CustomTable SelectMany( Func<dynamic, object> a, Func<dynamic, dynamic, object> b)
        {
            return this;
        }
        protected override object SetMetaValue(string key, object value)
        {

            if (!Columns.ContainsKey(key.ToLower()))
            {
                Columns.Add(key.ToLower(), new LinqDLRColumn(key, AliasName));
            }
            dynamic c = Columns[key.ToLower()];
            return c;
        }

        protected override object GetMetaValue(string key)
        {
            if (!Columns.ContainsKey(key.ToLower()))
            {
                Columns.Add(key.ToLower(), new LinqDLRColumn(key, AliasName));
            }
            return Columns[key.ToLower()];
        }

        protected override object InvokeMe(string methodInfo, params object[] args)
        {
            return this;
        }

    }
}
