using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Unit.DB
{
    /// <summary>
    /// 数据库访问表达式解析器,将通用的json表达式解析为对应DB的sql或nonsql表达式
    /// </summary>
    public abstract class DBExpress
    {
        /// <summary>
        /// DBExpress的动作定义
        /// </summary>
        public enum ActType
        {
            /// <summary>
            /// 新增
            /// </summary>
            Insert,
            /// <summary>
            /// 查询
            /// </summary>
            Query,
            /// <summary>
            /// 翻页查询
            /// </summary>
            QueryByPage,
            /// <summary>
            /// 删除
            /// </summary>
            Delete,
            /// <summary>
            /// 修改
            /// </summary>
            Update,
            /// <summary>
            /// 新增（insert select方式）
            /// </summary>
            InsertSelect,
            /// <summary>
            /// 创建table
            /// </summary>
            CreateTable,
            /// <summary>
            /// 删除table
            /// </summary>
            DropTable,
            /// <summary>
            /// 修改table中栏位的结构
            /// </summary>
            AlterColumn,
            /// <summary>
            /// 复制表
            /// </summary>
            CopyTable,
            /// <summary>
            /// 复制数据
            /// </summary>
            CopyData
        }
        /// <summary>
        /// 设定是否需要在log中打印解析结果
        /// </summary>
        public bool IsLog
        {
            get; set;
        }
        protected FrameDLRObject express = new FrameDLRObject();
        protected ActType acttype = ActType.Query;
        /// <summary>
        /// 当前的动作类型
        /// </summary>
        public ActType CurrentAct
        {
            get
            {
                return acttype;
            }
        }
        /// <summary>
        /// 给Express加载对应的参数
        /// </summary>
        /// <param name="express"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static void Load(DBExpress express, FrameDLRObject json)
        {
            var rtn = express;
            var acttypekey = "$acttype";
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
        }
        /// <summary>
        ///  给Express加载对应的参数
        /// </summary>
        /// <param name="express"></param>
        /// <param name="json"></param>
        public static void Load(DBExpress express, string json)
        {
            FrameDLRObject obj = FrameDLRObject.CreateInstance(json);
            Load(express, obj);
        }
        /// <summary>
        /// 创建一个DBExpress对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 创建一个DBExpress对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Create<T>(string json) where T : DBExpress
        {
            FrameDLRObject obj = FrameDLRObject.CreateInstance(json);
            return Create<T>(obj);
        }
        /// <summary>
        /// and操作
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
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
        /// <summary>
        /// or操作
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
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
        /// <summary>
        /// where操作
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 解析表达式，转化成sql
        /// </summary>
        /// <returns></returns>
        public FrameDLRObject ToExpress()
        {
            dynamic rtn = ParseExpress(this.express);
            if (IsLog && GlobalCommon.Logger != null)
                GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"DBExpress解析后的sql为:{rtn.sql}");
            return rtn;
        }
        protected abstract FrameDLRObject ParseExpress(FrameDLRObject obj);

    }
}
