using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
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
            var query = new QueryDocument(true);
            var update = new UpdateDocument(true);
            FrameDLRObject insert = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            var fields = new FieldsBuilder();
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
                                fields.Include(k);
                            }
                            else
                            {
                                fields.Exclude(k);
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

        void WhereExpress(FrameDLRObject obj, QueryDocument query, FieldsBuilder fields)
        {
            var rtn = FrameDLRObject.CreateInstance();
            foreach (var k in obj.Keys)
            {
                if (k.ToLower() != "$fields")
                {
                    var lq = WhereExpress(k, obj.GetValue(k));
                    foreach (var q in lq)
                    {
                        query.Add(q.ToBsonDocument());
                    }
                }
                else
                {
                    var f = new FieldsBuilder();
                    ParseFields(k, obj.GetValue(k), fields);
                }
            }
        }

        List<IMongoQuery> WhereExpress(string key, object obj)
        {
            var rtn = new List<IMongoQuery>();
            if (key.StartsWith("$"))
            {
                if (key.ToLower() == "$or")
                {
                    List<IMongoQuery> l = new List<IMongoQuery>();
                    if (obj is object[])
                    {
                        var aobj = (object[])obj;
                        foreach (var item in aobj)
                        {
                            QueryDocument q = new QueryDocument(true);
                            if (item is FrameDLRObject)
                            {
                                var ditem = (FrameDLRObject)item;
                                foreach (var itemkey in ditem.Keys)
                                {
                                    q.Add(Query.And(WhereExpress(itemkey, ditem.GetValue(itemkey))).ToBsonDocument());
                                }
                            }
                            l.Add(q);
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
                    if (l.Count > 0)
                    {
                        rtn.Add(Query.Or(l));
                    }
                }
                else if (key.ToLower() == "$and")
                {
                    List<IMongoQuery> l = new List<IMongoQuery>();
                    if (obj is object[])
                    {
                        var aobj = (object[])obj;
                        foreach (var item in aobj)
                        {
                            QueryDocument q = new QueryDocument(true);
                            if (item is FrameDLRObject)
                            {
                                var ditem = (FrameDLRObject)item;
                                foreach (var itemkey in ditem.Keys)
                                {
                                    q.Add(Query.And(WhereExpress(itemkey, ditem.GetValue(itemkey))).ToBsonDocument());
                                }
                            }
                            l.Add(q);
                        }
                    }
                    if (l.Count > 0)
                    {
                        rtn.Add(Query.And(l));
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
                                    rtn.Add(Query.EQ(key, BsonValue.Create(dobj.GetValue(k))));
                                }
                                break;
                            case "$neq":
                                if (vobj is string || vobj is double || vobj is int || vobj is DateTime || obj is bool)
                                {
                                    rtn.Add(Query.NE(key, BsonValue.Create(dobj.GetValue(k))));
                                }
                                break;
                            case "$lt":
                                if (vobj is string || vobj is double || vobj is int || vobj is DateTime)
                                {
                                    rtn.Add(Query.LT(key, BsonValue.Create(dobj.GetValue(k))));
                                }
                                break;
                            case "$gt":
                                if (vobj is string || vobj is double || vobj is int || vobj is DateTime)
                                {
                                    rtn.Add(Query.GT(key, BsonValue.Create(dobj.GetValue(k))));
                                }
                                break;
                            case "$lte":
                                if (vobj is string || vobj is double || vobj is int || vobj is DateTime)
                                {
                                    rtn.Add(Query.LTE(key, BsonValue.Create(dobj.GetValue(k))));
                                }
                                break;
                            case "$gte":
                                if (vobj is string || vobj is double || vobj is int || vobj is DateTime)
                                {
                                    rtn.Add(Query.GTE(key, BsonValue.Create(dobj.GetValue(k))));
                                }
                                break;
                            case "$like":
                                if (vobj is string)
                                {
                                    rtn.Add(Query.Matches(key, "/" + vobj + "/"));
                                }
                                break;
                            case "$likel":
                                if (vobj is string)
                                {
                                    rtn.Add(Query.Matches(key, "/^" + vobj + "/"));
                                }
                                break;
                            case "$liker":
                                if (vobj is string)
                                {
                                    rtn.Add(Query.Matches(key, "/" + vobj + "$/"));
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
                                rtn.Add(Query.In(key, inl.ToArray()));
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
                                rtn.Add(Query.NotIn(key, ninl.ToArray()));
                                break;
                            default:
                                break;

                        }
                    }
                }
                else if (obj is string || obj is int || obj is double || obj is DateTime || obj is bool)
                {
                    rtn.Add(new QueryDocument(key, BsonValue.Create(obj)));
                }
            }
            return rtn;
        }

        void ParseFields(string key, object obj, FieldsBuilder f)
        {
            f = f == null ? new FieldsBuilder() : f;
            if (key.ToLower() == "$fields")
            {
                if (obj is FrameDLRObject)
                {
                    var dobj = (FrameDLRObject)obj;
                    foreach (var k in dobj.Keys)
                    {
                        ParseFields(k, dobj.GetValue(k), f);
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
                                f = f.Slice(key, IntStd.ParseStd(dobj.GetValue(k)).Value);
                            }
                        }
                    }
                }
            }
        }
    }
}
