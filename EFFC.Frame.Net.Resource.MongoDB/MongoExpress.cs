using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Unit.DB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.ResouceManage.DB
{
    /// <summary>
    /// mongo表达式解析
    /// </summary>
    public class MongoExpress : DBExpress
    {
        protected override FrameDLRObject ParseExpress(FrameDLRObject obj)
        {
            var rtn = FrameDLRObject.CreateInstance();
            var query = new BsonDocument(true);
            var update = new BsonDocument(true);
            FrameDLRObject insert = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            var fields = new BsonDocument(true);
            var collectionname = "";
            foreach (var k in obj.Keys)
            {
                if (k.StartsWith("$"))
                {
                    if (k.ToLower() == "$where")
                    {
                        WhereExpress((FrameDLRObject)obj.GetValue(k), query, fields);
                    }
                    else if (k.ToLower() == "$table")
                    {
                        if (obj.GetValue(k) is string)
                        {
                            collectionname = ComFunc.nvl(obj.GetValue(k));
                        }
                    }
                }
                else
                {
                    var v = obj.GetValue(k);
                    if (this.CurrentAct == ActType.Query)
                    {
                        if (v is bool)
                        {
                            var bisinclude = (bool)v;
                            if (bisinclude)
                            {
                                fields.AddRange(Builders<BsonDocument>.Projection.Include(k).ToBsonDocument());
                            }
                            else
                            {
                                fields.AddRange(Builders<BsonDocument>.Projection.Exclude(k).ToBsonDocument());
                            }
                        }
                    }
                    else if (this.CurrentAct == ActType.Insert)
                    {
                        insert.SetValue(k, v);
                    }
                    else
                    {
                        if (!(v is FrameDLRObject))
                        {
                            update.Add(k, BsonValue.Create(v));
                        }
                    }
                }
            }
            rtn.query = query;
            rtn.update = update;
            rtn.insert = insert;
            rtn.fields = fields;
            rtn.table = collectionname;
            return rtn;
        }

        void WhereExpress(FrameDLRObject obj, BsonDocument query, BsonDocument fields)
        {
            var rtn = FrameDLRObject.CreateInstance();
            foreach (var k in obj.Keys)
            {
                if (k.ToLower() != "$fields")
                {
                    var lq = WhereExpress(k, obj.GetValue(k));
                    query.AddRange(lq);
                }
                else
                {
                    fields.AddRange(ParseFields(k, obj.GetValue(k)));
                }
            }
        }

        BsonDocument WhereExpress(string key, object obj)
        {
            var rtn = new BsonDocument(true);
            var filter = Builders<BsonDocument>.Filter;
            if (key.StartsWith("$"))
            {
                if (key.ToLower() == "$or")
                {
                    BsonDocument q = new BsonDocument(true);
                    if (obj is object[])
                    {
                        var aobj = (object[])obj;
                        foreach (var item in aobj)
                        {

                            if (item is FrameDLRObject)
                            {
                                var ditem = (FrameDLRObject)item;
                                foreach (var itemkey in ditem.Keys)
                                {
                                    q.AddRange(filter.And(WhereExpress(itemkey, ditem.GetValue(itemkey))).ToBsonDocument());
                                }
                            }
                        }
                    }
                    //else if (obj is FrameDLRObject)
                    //{
                    //    var ditem = (FrameDLRObject)obj;
                    //    foreach (var itemkey in ditem.Keys)
                    //    {
                    //        l.AddRange(WhereExpress(itemkey, ditem.GetValue(itemkey)));
                    //    }
                    //}

                    if (q.Count() > 0)
                        rtn.AddRange(filter.Or(q).ToBsonDocument());

                }
                else if (key.ToLower() == "$and")
                {
                    BsonDocument q = new BsonDocument(true);
                    if (obj is object[])
                    {
                        var aobj = (object[])obj;
                        foreach (var item in aobj)
                        {
                            if (item is FrameDLRObject)
                            {
                                var ditem = (FrameDLRObject)item;
                                foreach (var itemkey in ditem.Keys)
                                {
                                    q.AddRange(filter.And(WhereExpress(itemkey, ditem.GetValue(itemkey))).ToBsonDocument());
                                }
                            }
                        }
                    }
                    if (q.Count() > 0)
                    {
                        rtn.AddRange(filter.And(q).ToBsonDocument());
                    }
                }
            }
            else
            {
                if (obj is FrameDLRObject)
                {
                    var dobj = (FrameDLRObject)obj;
                    foreach (var k in dobj.Keys)
                    {
                        var vobj = dobj.GetValue(k);
                        switch (k.ToLower())
                        {
                            case "$eq":
                                if (vobj is string || vobj is double || vobj is int || vobj is DateTime || obj is bool)
                                {
                                    rtn.AddRange(filter.Eq(key, BsonValue.Create(dobj.GetValue(k))).ToBsonDocument());
                                }
                                break;
                            case "$neq":
                                if (vobj is string || vobj is double || vobj is int || vobj is DateTime || obj is bool)
                                {
                                    rtn.AddRange(filter.Ne(key, BsonValue.Create(dobj.GetValue(k))).ToBsonDocument());
                                }
                                break;
                            case "$lt":
                                if (vobj is string || vobj is double || vobj is int || vobj is DateTime)
                                {
                                    rtn.AddRange(filter.Lt(key, BsonValue.Create(dobj.GetValue(k))).ToBsonDocument());
                                }
                                break;
                            case "$gt":
                                if (vobj is string || vobj is double || vobj is int || vobj is DateTime)
                                {
                                    rtn.AddRange(filter.Gt(key, BsonValue.Create(dobj.GetValue(k))).ToBsonDocument());
                                }
                                break;
                            case "$lte":
                                if (vobj is string || vobj is double || vobj is int || vobj is DateTime)
                                {
                                    rtn.AddRange(filter.Lte(key, BsonValue.Create(dobj.GetValue(k))).ToBsonDocument());
                                }
                                break;
                            case "$gte":
                                if (vobj is string || vobj is double || vobj is int || vobj is DateTime)
                                {
                                    rtn.AddRange(filter.Gte(key, BsonValue.Create(dobj.GetValue(k))).ToBsonDocument());
                                }
                                break;
                            case "$like":
                                if (vobj is string)
                                {
                                    rtn.AddRange(filter.Regex(key, "/" + vobj + "/").ToBsonDocument());
                                }
                                break;
                            case "$likel":
                                if (vobj is string)
                                {
                                    rtn.AddRange(filter.Regex(key, "/^" + vobj + "/").ToBsonDocument());
                                }
                                break;
                            case "$liker":
                                if (vobj is string)
                                {
                                    rtn.AddRange(filter.Regex(key, "/" + vobj + "$/").ToBsonDocument());
                                }
                                break;
                            case "$in":
                                List<BsonValue> inl = new List<BsonValue>();
                                if (vobj is object[])
                                {
                                    var array = (object[])vobj;
                                    foreach (var item in array)
                                    {
                                        if (item is string || item is int || item is double)
                                        {
                                            inl.Add(BsonValue.Create(item));
                                        }
                                    }
                                }
                                rtn.AddRange(filter.In(key, inl.ToArray()).ToBsonDocument());
                                break;
                            case "$nin":
                                List<BsonValue> ninl = new List<BsonValue>();
                                if (vobj is object[])
                                {
                                    var array = (object[])vobj;
                                    foreach (var item in array)
                                    {
                                        if (item is string || item is int || item is double)
                                        {
                                            ninl.Add(BsonValue.Create(item));
                                        }
                                    }
                                }
                                rtn.AddRange(filter.Not(filter.In(key, ninl.ToArray())).ToBsonDocument());
                                break;
                            default:
                                break;

                        }
                    }
                }
                else if (obj is string || obj is int || obj is double || obj is DateTime || obj is bool)
                {
                    rtn.AddRange(new BsonDocument(key, BsonValue.Create(obj)));
                }
            }
            return rtn;
        }

        BsonDocument ParseFields(string key, object obj)
        {

            var rtn = new BsonDocument(true);
            if (key.ToLower() == "$fields")
            {
                if (obj is FrameDLRObject)
                {
                    var dobj = (FrameDLRObject)obj;
                    foreach (var k in dobj.Keys)
                    {
                        rtn.AddRange(ParseFields(k, dobj.GetValue(k)));
                    }
                }
            }
            else
            {
                if (obj is FrameDLRObject)
                {
                    var dobj = (FrameDLRObject)obj;
                    foreach (var k in dobj.Keys)
                    {
                        if (k.ToLower() == "$slice")
                        {
                            if (dobj.GetValue(k) is int)
                            {
                                rtn.AddRange(Builders<BsonDocument>.Projection.Slice(key, IntStd.ParseStd(dobj.GetValue(k)).Value).ToBsonDocument());
                            }
                        }
                    }
                }
            }
            return rtn;
        }
    }
}
