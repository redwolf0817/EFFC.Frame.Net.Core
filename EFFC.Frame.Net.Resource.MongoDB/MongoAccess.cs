using EFFC.Frame.Net.Base.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Unit.DB;

namespace EFFC.Frame.Net.Base.ResouceManage.DB
{
    /// <summary>
    /// MongoDB访问器
    /// </summary>
    public class MongoAccess : IResourceEntity, ITransaction, IDBAccessInfo
    {
        MongoClient client;
        IMongoDatabase database;
        static object lockobj = new object();
        DBStatus _status = DBStatus.Empty;
        public MongoAccess()
        {
            
            //client = new MongoClient();
            //并发情况下，会造成异常发生
            lock (lockobj)
            {
                if (BsonSerializer.LookupSerializer(typeof(FrameDLRObject)) == null)
                {
                    BsonSerializer.RegisterSerializer<FrameDLRObject>(new DLRMap());
                }
            }
        }

        public MongoAccess(string mongoconn,string dbname)
        { 
            //并发情况下，会造成异常发生
            lock (lockobj)
            {
                if (BsonSerializer.LookupSerializer(typeof(FrameDLRObject)) == null)
                {
                    BsonSerializer.RegisterSerializer<FrameDLRObject>(new DLRMap());
                }
            }

            Open(mongoconn, dbname);
        }
        /// <summary>
        /// 当前链接状态
        /// </summary>
        public DBStatus CurrentStatus
        {
            get
            {
                return _status;
            }
        }
        public void Open(string mongoconn,params object[] p)
        {
            Open(mongoconn, ComFunc.nvl(p[0]));
        }
        public void Close()
        {
            if (client != null)
            {
                client = null;
                _status = DBStatus.Close;
            }
        }
        public void Open(string mongoconn, string dbname)
        {
            Release();

            client = new MongoClient(mongoconn);
            database = client.GetDatabase(dbname);
            _status = DBStatus.Open;
        }

        #region Query
        public List<FrameDLRObject> Query(string collectionname,  string json)
        {
            //修正原有接口呼叫错误的bug modified by chuan.yin in 2015/7/30
            if (json != null && json.Length > 0)
            {
                return Query(collectionname, ((FrameDLRObject)FrameDLRObject.CreateInstance(json)).ToDictionary());
            }
            else
            {
                return new List<FrameDLRObject>();
            }
        }
        
        public List<FrameDLRObject> Query(string collectionname, FrameDLRObject json)
        {
            return Query(collectionname, json.ToDictionary());
        }
        public List<FrameDLRObject> Query(string collectionname, Dictionary<string, object> json)
        {
            BsonDocument qd = new BsonDocument(json);
            var collection = database.GetCollection<FrameDLRObject>(collectionname);
            return collection.Find(qd).ToList();
        }
        /// <summary>
        /// select count
        /// </summary>
        /// <param name="collectionname"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public long Count(string collectionname, Dictionary<string, object> json)
        {

            BsonDocument qd = new BsonDocument(json);
            return database.GetCollection<BsonDocument>(collectionname).Count(qd);
        }
        /// <summary>
        /// select count
        /// </summary>
        /// <param name="collectionname"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public long Count(string collectionname, string json)
        {
            return Count(collectionname, ((FrameDLRObject)FrameDLRObject.CreateInstance(json)).ToDictionary());
        }
        /// <summary>
        /// select count
        /// </summary>
        /// <param name="collectionname"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public long Count(string collectionname, FrameDLRObject json)
        {
            return Count(collectionname, json.ToDictionary());
        }
        #endregion
        #region Update
        public bool Update(string collectionname, string queryjson, string updatejson)
        {
            return Update(collectionname,
                ((FrameDLRObject)FrameDLRObject.CreateInstance(queryjson)).ToDictionary(),
                ((FrameDLRObject)FrameDLRObject.CreateInstance(updatejson)).ToDictionary());
        }

        public bool Update(string collectionname, FrameDLRObject queryjson, FrameDLRObject updatejson)
        {
            return Update(collectionname, queryjson.ToDictionary(), updatejson.ToDictionary());
        }

        public bool Update(string collectionname, FrameDLRObject queryjson, string updatejson)
        {
            return Update(collectionname, queryjson.ToDictionary(), ((FrameDLRObject)FrameDLRObject.CreateInstance(updatejson)).ToDictionary());
        }
        public bool Update(string collectionname, string queryjson, FrameDLRObject updatejson)
        {
            return Update(collectionname, ((FrameDLRObject)FrameDLRObject.CreateInstance(queryjson)).ToDictionary(), updatejson.ToDictionary());
        }

        public bool Update(string collectionname, Dictionary<string, object> queryjson, Dictionary<string, object> updatejson)
        {
            BsonDocument qd = new BsonDocument(queryjson);
            BsonDocument up = new BsonDocument(updatejson);
            return Update(collectionname, qd, up);
        }

        public bool Update(string collectionname, BsonDocument q, BsonDocument u)
        {
            
            var collection = database.GetCollection<FrameDLRObject>(collectionname);
            var result = collection.UpdateMany(q,u);
            return result.ModifiedCount > 0;
        }
        #endregion
        #region Insert
        public bool Insert(string collectionname, FrameDLRObject item)
        {
            var collection = database.GetCollection<FrameDLRObject>(collectionname);
            collection.InsertOne(item);
            return true;
        }
        #endregion
        #region Delete
        public bool Delete(string collectionname, string queryjson)
        {
            return Delete(collectionname, ((FrameDLRObject)FrameDLRObject.CreateInstance(queryjson)).ToDictionary());
        }
        public bool Delete(string collectionname, FrameDLRObject queryjson)
        {
            return Delete(collectionname, queryjson.ToDictionary());
        }
        public bool Delete(string collectionname, Dictionary<string,object> queryjson)
        {
            var collection = database.GetCollection<FrameDLRObject>(collectionname);
            BsonDocument qd = new BsonDocument(queryjson);
            var result = collection.DeleteMany(qd);
            return result.DeletedCount > 0;
        }
        #endregion

        #region Excute
        /// <summary>
        /// 执行一段指令
        /// </summary>
        /// <param name="jsoncmd"></param>
        /// <param name="outobj"></param>
        /// <returns></returns>
        public bool Excute(string jsoncmd, ref FrameDLRObject outobj)
        {
            outobj = database.RunCommand<FrameDLRObject>(jsoncmd);

            return true;
        }
        /// <summary>
        /// 执行DBExpress动作
        /// </summary>
        /// <param name="express"></param>
        /// <returns></returns>
        public object Excute(DBExpress express)
        {
            if (express is MongoExpress)
            {
                var mexpress = (MongoExpress)express;
                dynamic dexpress =  mexpress.ToExpress();
                string collectionname = ComFunc.nvl(dexpress.table);
                BsonDocument q = dexpress.query;
                BsonDocument u = dexpress.update;
                BsonDocument f = dexpress.fields;
                FrameDLRObject insert = dexpress.insert;

                var collection = database.GetCollection<FrameDLRObject>(collectionname);
                if (mexpress.CurrentAct == DBExpress.ActType.Update)
                {
                    collection.UpdateMany(q, u);
                    return true;
                }
                else if (mexpress.CurrentAct == DBExpress.ActType.Insert)
                {
                    collection.InsertOne(insert);
                    return true;
                }
                else if (mexpress.CurrentAct == DBExpress.ActType.Delete)
                {
                    collection.DeleteMany(q);
                    return true;
                }
                else
                {
                    var cursor = collection.Find(q).Project(f);
                    return cursor.ToList();
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        public void Release()
        {
            Close();
        }

        private class DLRMap : IBsonSerializer<FrameDLRObject>
        {

            public FrameDLRObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                FrameDLRObject rtn = null;
                var d = BsonDocumentSerializer.Instance.Deserialize(context, args);

                rtn = Bson2Dynamic(d);
                return rtn;
            }

            public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, FrameDLRObject value)
            {
                Serialize(context, args, value);
            }

            object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                FrameDLRObject rtn = null;
                var d = BsonDocumentSerializer.Instance.Deserialize(context, args);

                rtn = Bson2Dynamic(d);
                return rtn;
            }

            public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
            {
                FrameDLRObject v = (FrameDLRObject)value;
                BsonDocumentSerializer bs = new BsonDocumentSerializer();
                BsonDocument bd = new BsonDocument(v.ToDictionary());

                bs.Serialize(context, args, bd);
            }

            private FrameDLRObject Bson2Dynamic(BsonDocument bd)
            {
                var doc = bd;
                FrameDLRObject fobj = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                foreach (var item in doc.Elements)
                {
                    fobj.SetValue(item.Name, Bson2Object(item.Value));
                }

                return fobj;
            }

            private object Bson2Object(BsonValue bd)
            {
                object rtn = null;
                if (bd.IsBsonNull)
                {
                    rtn = null;
                }
                else if (bd.IsBsonDateTime)
                {
                    //mongo存储的实际均为UTC标准时间，系统在运行时都是本地化的时间
                    rtn = bd.ToLocalTime();
                }
                else if (bd.IsBsonJavaScript)
                {
                    rtn = bd.AsBsonJavaScript.ToString();
                }
                else if (bd.IsBsonArray)
                {
                    var arr = bd.AsBsonArray;
                    var list = new List<object>();
                    foreach (var item in arr)
                    {
                        list.Add(Bson2Object(item));
                    }

                    rtn = list.ToArray();
                }
                else if (bd.IsBsonDocument)
                {
                    rtn = Bson2Dynamic(bd.AsBsonDocument);
                }
                else if (bd.IsObjectId)
                {
                    rtn = bd.AsObjectId.Pid;
                }
                else if (bd.IsInt32)
                {
                    rtn = bd.AsInt32;
                }
                else if (bd.IsInt64)
                {
                    rtn = bd.AsInt64;
                }
                else if (bd.IsDouble)
                {
                    rtn = bd.AsDouble;
                }
                else if (bd.IsNumeric)
                {
                    rtn = decimal.Parse(bd.ToString());
                }
                else if (bd.IsBoolean)
                {
                    rtn = bd.ToBoolean();
                }
                else
                {
                    rtn = bd.AsString;
                }

                return rtn;
            }

            public Type ValueType
            {
                get { return typeof(FrameDLRObject); }
            }
        }

        public string ID
        {
            get { return "mongodb_"+DateTime.Now.ToString("yyyyMMddHHmmssfff"); }
        }

        public DBType MyType => DBType.MongoDB;

        public void BeginTransaction()
        {
            BeginTransaction(FrameIsolationLevel.Default);
        }

        public void CommitTransaction()
        {
            _status = DBStatus.Commit_Trans;
        }

        public void RollbackTransaction()
        {
            _status = DBStatus.RollBack_Trans;
        }

        public void BeginTransaction(Frame.Net.Base.Constants.FrameIsolationLevel level)
        {
            _status = DBStatus.Begin_Trans;
        }


       
    }
}
