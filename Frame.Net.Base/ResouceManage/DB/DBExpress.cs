using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.ResouceManage.DB
{
    /// <summary>
    /// 数据库访问表达式解析器,将通用的json表达式解析为对应DB的sql或nonsql表达式
    /// </summary>
    public abstract class DBExpress
    {
        public enum ActType
        {
            Insert,
            Query,
            QueryByPage,
            Delete,
            Update
        }
        protected FrameDLRObject express = new FrameDLRObject();
        protected ActType acttype = ActType.Query;
        public ActType CurrentAct
        {
            get
            {
                return acttype;
            }
        }
        public static T Create<T>(FrameDLRObject json) where T : DBExpress
        {
            T rtn = (T)Activator.CreateInstance(typeof(T), true);
            var acttypekey = json.IgnoreCase ? "$acttype" : "$ActType";
            if (ComFunc.nvl(json.GetValue(acttypekey)) != "")
            {
                rtn.acttype = ComFunc.EnumParse<ActType>(ComFunc.nvl(json.GetValue(acttypekey)));
                json.Remove(acttypekey);
            }
            else
            {
                rtn.acttype = ActType.Query;
            }

            rtn.express = json;
            return rtn;
        }
        public static T Create<T>(string json) where T : DBExpress
        {
            FrameDLRObject obj = FrameDLRObject.CreateInstance(json);
            return Create<T>(obj);
        }
        public DBExpress And(FrameDLRObject json)
        {
            List<object> l = new List<object>();
            if (this.express.GetValue("$and") != null)
            {
                var obj = this.express.GetValue("$and");
                if (obj is object[])
                {
                    var array = (object[])obj;
                    l.AddRange(array);
                }
                else
                {
                    l.Add(obj);
                }
            }
            l.Add(json);
            this.express.SetValue("$and", l.ToArray());

            return this;
        }
        public DBExpress Or(FrameDLRObject json)
        {
            List<object> l = new List<object>();
            if (this.express.GetValue("$or") != null)
            {
                var obj = this.express.GetValue("$or");
                if (obj is object[])
                {
                    var array = (object[])obj;
                    l.AddRange(array);
                }
                else
                {
                    l.Add(obj);
                }
            }
            l.Add(json);
            this.express.SetValue("$or", l.ToArray());

            return this;
        }
        public DBExpress Where(FrameDLRObject json)
        {
            if (this.express.GetValue("$where") != null)
            {
                var obj = this.express.GetValue("$where");
                if (obj is FrameDLRObject)
                {
                    var dobj = (FrameDLRObject)obj;
                    foreach (var k in json.Keys)
                    {
                        dobj.SetValue(k, json.GetValue(k));
                    }
                }
            }
            else
            {
                this.express.SetValue("$where", json);
            }
            return this;
        }
        public FrameDLRObject ToExpress()
        {
            return ParseExpress(this.express);
        }
        protected abstract FrameDLRObject ParseExpress(FrameDLRObject obj);

    }
}
