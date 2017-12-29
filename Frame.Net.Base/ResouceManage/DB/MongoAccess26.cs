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
using MongoDB.Driver.Builders;

namespace EFFC.Frame.Net.Base.ResouceManage.DB
{
    public class MongoAccess26 : IResourceEntity, ITransaction, IDBAccessInfo
    {
        MongoClient client;
        MongoServer server;
        MongoDatabase database;
        static object lockobj = new object();
        DBStatus _status = DBStatus.Empty;
        public MongoAccess26()
        {
            //client = new MongoClient();
            //并发情况下，会造成异常发生
            lock (lockobj)
            {
                if (!BsonSerializer.IsExistsRegistered(typeof(FrameDLRObject)))
                {
                    BsonSerializer.RegisterSerializer<FrameDLRObject>(new DLRMap());
                }
            }
        }

        public MongoAccess26(string mongoconn,string dbname)
        { 
            //并发情况下，会造成异常发生
            lock (lockobj)
            {
                if (!BsonSerializer.IsExistsRegistered(typeof(FrameDLRObject)))
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
            if (server != null && server.State == MongoServerState.Connected)
            {
                server.Disconnect();
                _status = DBStatus.Close;
            }
        }
        public void Open(string mongoconn, string dbname)
        {
            Release();

            client = new MongoClient(mongoconn);
            server = client.GetServer();
            database = server.GetDatabase(dbname);
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
            if (server.State == MongoServerState.Disconnected)
            {
                server.Reconnect();
            }

            QueryDocument qd = new QueryDocument(json);
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
            if (server.State == MongoServerState.Disconnected)
            {
                server.Reconnect();
            }
            QueryDocument qd = new QueryDocument(json);
            return database.GetCollection(collectionname).Count(qd);
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
            QueryDocument qd = new QueryDocument(queryjson);
            UpdateDocument up = new UpdateDocument(updatejson);
            return Update(collectionname, qd, up, UpdateFlags.Upsert);
        }
        public bool Update(string collectionname, IMongoQuery q, IMongoUpdate u, UpdateFlags flag)
        {
            if (server.State == MongoServerState.Disconnected)
            {
                server.Reconnect();
            }

            var collection = database.GetCollection<FrameDLRObject>(collectionname);
            var result = collection.Update(q, u, flag);
            return result.Response.GetValue("ok").ToBoolean();
        }
        #endregion
        #region Insert
        public bool Insert(string collectionname, FrameDLRObject item)
        {
            if (server.State == MongoServerState.Disconnected)
            {
                server.Reconnect();
            }

            var collection = database.GetCollection<FrameDLRObject>(collectionname);
            var result = collection.Insert(item);
            return result.Response.GetValue("ok").ToBoolean();
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
            if (server.State == MongoServerState.Disconnected)
            {
                server.Reconnect();
            }

            var collection = database.GetCollection<FrameDLRObject>(collectionname);
            QueryDocument qd = new QueryDocument(queryjson);
            var result = collection.Remove(qd);
            return result.Response.GetValue("ok").ToBoolean();
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
            if (server.State == MongoServerState.Disconnected)
            {
                server.Reconnect();
            }

            FrameDLRObject cmd = FrameDLRObject.CreateInstance(jsoncmd);
            var cd = new CommandDocument(cmd.ToDictionary());
            var result = database.RunCommand(cd);
            if (result.Ok)
            {
                var value = result.Response.GetValue("value").ToBsonDocument();
                value.Remove("_id");
                outobj = FrameDLRObject.CreateInstance(value.ToJson());
            }

            return result.Ok;
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
                QueryDocument q = dexpress.query;
                UpdateDocument u = dexpress.update;
                FieldsBuilder f = dexpress.fields;
                FrameDLRObject insert = dexpress.insert;

                if (server.State == MongoServerState.Disconnected)
                {
                    server.Reconnect();
                }
                var collection = database.GetCollection<FrameDLRObject>(collectionname);
                if (mexpress.CurrentAct == DBExpress.ActType.Update)
                {
                    collection.Update(q, u, UpdateFlags.Upsert);
                    return true;
                }
                else if (mexpress.CurrentAct == DBExpress.ActType.Insert)
                {
                    collection.Insert(insert);
                    return true;
                }
                else if (mexpress.CurrentAct == DBExpress.ActType.Delete)
                {
                    collection.Remove(q);
                    return true;
                }
                else
                {
                    var cursor = collection.Find(q).SetFields(f);
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
                    var jsfunc = new Noesis.Javascript.JavascriptFunction();
                    jsfunc.FunctionString = bd.AsBsonJavaScript.ToString();
                    rtn = jsfunc;
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
