using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Business.Datas;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Unit.DB.Unit;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;

namespace EFFC.Frame.Net.Module.Business.Logic
{
    public partial class BaseLogic<PType, DType>
    {
        private DBHelper _db;
        /// <summary>
        /// db操作相关
        /// </summary>
        public virtual DBHelper DB
        {
            get
            {
                if (_db == null)
                    _db = new DBHelper(this);

                return _db;
            }
        }

        public class DBHelper
        {
            BaseLogic<PType, DType> _logic;
            static DBType DEFAULT_DB_TYPE = DBType.None;
            public DBHelper() { }
            public DBHelper(BaseLogic<PType, DType> logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// 设定默认数据库类型
            /// </summary>
            /// <param name="dBType"></param>
            protected static void UseDefaultDBType(DBType dBType)
            {
                DEFAULT_DB_TYPE = dBType;
            }
            /// <summary>
            /// 获取一个Lamdatable对象，可以进行lamda2sql的操作
            /// </summary>
            /// <param name="tablename"></param>
            /// <param name="alianname"></param>
            /// <param name="dbtype"></param>
            /// <returns></returns>
            [Obsolete("该方法不推荐使用")]
            public virtual LinqDLRTable LamdaTable(string tablename,string alianname="",DBType dbtype = DBType.None)
            {
                return LinqDLRTable.New<LinqDLRColumn>(tablename, alianname);
            }
            /// <summary>
            /// 获取一个Lamdatable对象，可以进行lamda2sql的操作，通过up自动识别db类型
            /// </summary>
            /// <param name="up"></param>
            /// <param name="tablename"></param>
            /// <param name="alianname"></param>
            /// <returns></returns>
            public virtual LinqDLRTable LamdaTable(UnitParameter up,string tablename, string alianname = "")
            {
                if(up.Dao is ADBAccess)
                {
                    var rtn = ((ADBAccess)up.Dao).NewLinqTable(tablename,alianname);
                    return rtn;
                }
                else
                {
                    return null;
                }
                
            }
            /// <summary>
            /// 获得一个新的UnitParamter
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="rm">指定的资源管理器</param>
            /// <param name="token">指定的TransactionToken</param>
            /// <returns></returns>
            public T NewUnitParameter<T>(ResourceManage rm, TransactionToken token) where T : UnitParameter
            {
                var t = Activator.CreateInstance<T>();
                t.SetValue(ParameterKey.TOKEN, token);
                t.SetValue(ParameterKey.RESOURCE_MANAGER, rm);
                foreach (var item in _logic.CallContext_Parameter.Domain(DomainKey.CONFIG))
                {
                    t.SetValue(DomainKey.CONFIG, item.Key, item.Value);
                }
                return t;
            }
            /// <summary>
            /// 获得一个新的UnitParamter
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public T NewUnitParameter<T>() where T : UnitParameter
            {
                return NewUnitParameter<T>(_logic.CallContext_Parameter.Resources, _logic.CallContext_Parameter.CurrentTransToken);
            }
            /// <summary>
            /// 获得一个新的UnitParamter，默认Dao为DB类型的DBAccess
            /// </summary>
            /// <typeparam name="DB"></typeparam>
            /// <returns></returns>
            public UnitParameter NewDBUnitParameter<DB>() where DB : IDBAccessInfo, ITransaction, IResourceEntity
            {
                UnitParameter _up = NewUnitParameter<UnitParameter>();
                _up.Dao = _logic.CallContext_ResourceManage.CreateInstance<DB>(_logic.CallContext_CurrentToken);
                var dbconn = ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.CONFIG, ParameterKey.DBCONNECT_STRING]);

                _up.DBConnString = dbconn;
                return _up;
            }
            /// <summary>
            /// 获得一个新的UnitParamter
            /// </summary>
            /// <returns></returns>
            public virtual UnitParameter NewDBUnitParameter()
            {
                var _up = new UnitParameter();
                return _up;
            }
            /// <summary>
            /// 标准化DB执行操作
            /// </summary>
            /// <param name="p"></param>
            /// <param name="express"></param>
            /// <param name="islog">用于设定是否记录解析结果，以便进行debug操作</param>
            /// <returns></returns>
            public virtual UnitDataCollection Excute(UnitParameter p, DBExpress express, bool islog=false)
            {
                express.IsLog = islog;
                return DBUnitProxy.Excute(p, express);
            }
            /// <summary>
            /// 通过json对象执行标准化DB操作
            /// </summary>
            /// <param name="p"></param>
            /// <param name="json"></param>
            /// <param name="islog">用于设定是否记录解析结果，以便进行debug操作</param>
            /// <returns></returns>
            public virtual UnitDataCollection Excute(UnitParameter p, FrameDLRObject json, bool islog = false)
            {
                return DBUnitProxy.Excute(p, json, islog);
            }
            /// <summary>
            /// 通过json串执行标准化DB操作
            /// </summary>
            /// <param name="p"></param>
            /// <param name="json"></param>
            /// <param name="islog">用于设定是否记录解析结果，以便进行debug操作</param>
            /// <returns></returns>
            public virtual UnitDataCollection Excute(UnitParameter p, string json, bool islog = false)
            {
                return Excute(p, FrameDLRObject.CreateInstance(json, FrameDLRFlags.SensitiveCase),islog);
            }
            /// <summary>
            /// 执行lamda表达式
            /// </summary>
            /// <typeparam name="TSource"></typeparam>
            /// <param name="p"></param>
            /// <param name="ltable"></param>
            /// <returns></returns>
            [Obsolete("该方法不推荐使用,可以直接使用LinqDLR2Sql对象下的Query、Insert、Update、Delete、QueryByPage方法")]
            public virtual UnitDataCollection ExcuteLamda<TSource>(UnitParameter p, LinqDLR2Sql<TSource> ltable)
            {
                var sql = ltable.ToSql();
                var data = ltable.SQLGenerator.ConditionValues;
                //GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"LamdaExpress解析后的sql为:{sql}");
                p.SetValue("sql", sql);
                foreach (var item in data)
                {
                    p.SetValue(item.Key, item.Value);
                }
                
                if ((ltable.CurrentOperationType & LinqDLR2SQLOperation.Select)== LinqDLR2SQLOperation.Select
                    || (ltable.CurrentOperationType & LinqDLR2SQLOperation.SelectJoin) == LinqDLR2SQLOperation.SelectJoin
                    || (ltable.CurrentOperationType & LinqDLR2SQLOperation.SelectMany) == LinqDLR2SQLOperation.SelectMany)
                {
                    return Query<LamdaExpressUnit>(p, "");
                }
                else
                {
                    NonQuery<LamdaExpressUnit>(p, "");
                    return new UnitDataCollection();
                }
            }
            /// <summary>
            /// lamda表达式执行querybypage，注意，querybypage的时候是不能使用orderby的，否则会保存，order by通过orderbyexpress来提供
            /// </summary>
            /// <typeparam name="TSource"></typeparam>
            /// <param name="p"></param>
            /// <param name="ltable"></param>
            /// <param name="orderbyexpress">排序表达式，排序栏位不要携带table别名</param>
            /// <returns></returns>
            [Obsolete("该方法不推荐使用,可以直接使用LinqDLR2Sql对象下的Query、Insert、Update、Delete、QueryByPage方法")]
            public virtual UnitDataCollection LamdaQueryByPage<TSource>(UnitParameter p, LinqDLR2Sql<TSource> ltable,string orderbyexpress)
            {
                var sql = ltable.ToSql();
                var data = ltable.SQLGenerator.ConditionValues;
                //GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"LamdaExpress解析后的sql为:{sql}");
                p.SetValue("sql", sql);
                p.SetValue("orderby", orderbyexpress);
                foreach (var item in data)
                {
                    p.SetValue(item.Key, item.Value);
                }
                if (ltable.CurrentOperationType == LinqDLR2SQLOperation.Select
                    || ltable.CurrentOperationType == LinqDLR2SQLOperation.SelectJoin
                    || ltable.CurrentOperationType == LinqDLR2SQLOperation.SelectMany)
                {
                    return QueryByPage<LamdaExpressUnit>(p, "");
                }
                else
                {
                    throw new FrameException("LamdaQueryByPage只适用于select类型的操作");
                }
            }
            /// <summary>
            /// 执行lamda表达式
            /// </summary>
            /// <param name="p"></param>
            /// <param name="ltable"></param>
            /// <returns></returns>
            [Obsolete("该方法不推荐使用,可以直接使用LinqDLR2Sql对象下的Query、Insert、Update、Delete、QueryByPage方法(using EFFC.Frame.Net.Unit.DB)")]
            public virtual UnitDataCollection ExcuteLamda(UnitParameter p,LinqDLR2Sql<dynamic> ltable)
            {
                return ExcuteLamda<dynamic>(p, ltable);
            }
            /// <summary>
            /// lamda表达式执行querybypage，注意，querybypage的时候是不能使用orderby的，否则会保存，order by通过orderbyexpress来提供
            /// </summary>
            /// <param name="p"></param>
            /// <param name="ltable"></param>
            /// <param name="orderbyexpress">排序表达式，排序栏位不要携带table别名</param>
            /// <returns></returns>
            [Obsolete("该方法不推荐使用,可以直接使用LinqDLR2Sql对象下的Query、Insert、Update、Delete、QueryByPage方法(using EFFC.Frame.Net.Unit.DB)")]
            public virtual UnitDataCollection LamdaQueryByPage(UnitParameter p, LinqDLR2Sql<dynamic> ltable,string orderbyexpress)
            {
                return LamdaQueryByPage<dynamic>(p, ltable, orderbyexpress);
            }
            /// <summary>
            /// 根据lamdasql获取总比数资料
            /// </summary>
            /// <typeparam name="TSource"></typeparam>
            /// <param name="p">DB访问参数</param>
            /// <param name="ltable">LinqDLR2Sql表达式</param>
            /// <returns></returns>
            [Obsolete("该方法不推荐使用,可以直接使用LinqDLR2Sql对象下的Count方法(using EFFC.Frame.Net.Unit.DB)")]
            public virtual int Count<TSource>(UnitParameter p, LinqDLR2Sql<TSource> ltable)
            {
                var result = ExcuteLamda<TSource>(p, ltable.Count());
                return result.QueryTable.RowLength > 0 ? IntStd.IsNotIntThen(result.QueryTable[0, 0]) : 0;
            }
            /// <summary>
            /// 根据lamdasql获取指定栏位的Max值
            /// </summary>
            /// <typeparam name="TSource"></typeparam>
            /// <param name="p"></param>
            /// <param name="ltable"></param>
            /// <param name="maxColumn"></param>
            /// <returns></returns>
            [Obsolete("该方法不推荐使用,可以直接使用LinqDLR2Sql对象下的Max方法(using EFFC.Frame.Net.Unit.DB)")]
            public virtual object Max<TSource>(UnitParameter p, LinqDLR2Sql<TSource> ltable,string maxColumn)
            {
                var result = ExcuteLamda<TSource>(p, ltable.Max(maxColumn));
                return result.QueryTable.RowLength > 0 ? result.QueryTable[0, 0] : null;
            }
            /// <summary>
            /// 根据lamdasql获取指定栏位的Min值
            /// </summary>
            /// <typeparam name="TSource"></typeparam>
            /// <param name="p"></param>
            /// <param name="ltable"></param>
            /// <param name="minColumn"></param>
            /// <returns></returns>
            [Obsolete("该方法不推荐使用,可以直接使用LinqDLR2Sql对象下的Min方法(using EFFC.Frame.Net.Unit.DB)")]
            public virtual object Min<TSource>(UnitParameter p, LinqDLR2Sql<TSource> ltable, string minColumn)
            {
                var result = ExcuteLamda<TSource>(p, ltable.Min(minColumn));
                return result.QueryTable.RowLength > 0 ? result.QueryTable[0, 0] : null;
            }
            /// <summary>
            /// 根据lamdasql获取指定栏位的Sum值
            /// </summary>
            /// <typeparam name="TSource"></typeparam>
            /// <param name="p"></param>
            /// <param name="ltable"></param>
            /// <param name="sumColumn"></param>
            /// <returns></returns>
            [Obsolete("该方法不推荐使用,可以直接使用LinqDLR2Sql对象下的Sum方法(using EFFC.Frame.Net.Unit.DB)")]
            public virtual object Sum<TSource>(UnitParameter p, LinqDLR2Sql<TSource> ltable, string sumColumn)
            {
                var result = ExcuteLamda<TSource>(p, ltable.Sum(sumColumn));
                return result.QueryTable.RowLength > 0 ? result.QueryTable[0, 0] : null;
            }
            /// <summary>
            /// 判断资料是否存在
            /// </summary>
            /// <typeparam name="TSource"></typeparam>
            /// <param name="p"></param>
            /// <param name="ltable"></param>
            /// <returns></returns>
            [Obsolete("该方法不推荐使用,可以直接使用LinqDLR2Sql对象下的IsExists方法(using EFFC.Frame.Net.Unit.DB)")]
            public virtual bool IsExists<TSource>(UnitParameter p, LinqDLR2Sql<TSource> ltable)
            {
                return Count<TSource>(p, ltable) > 0;
            }
            /// <summary>
            /// 快速执行简易Update操作
            /// </summary>
            /// <param name="p"></param>
            /// <param name="toTable">目标table</param>
            /// <param name="data">要更新数据对象</param>
            /// <param name="where">只支持and操作</param>
            /// <param name="islog"></param>
            /// <returns></returns>
            public virtual UnitDataCollection QuickUpdate(UnitParameter p,string toTable,object data,object where, bool islog = false)
            {
                FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Update',
$table : {0}
}", toTable);
                FrameDLRObject dataexpress = FrameDLRObject.CreateInstance(data);
                FrameDLRObject whereexpress = FrameDLRObject.CreateInstance(where);
                foreach(var item in dataexpress.Items)
                {
                    express.SetValue(item.Key, item.Value);
                }
                if(whereexpress!= null && whereexpress.Items.Count > 0)
                {
                    express.SetValue("$where", whereexpress);
                }
                return Excute(p, express);
            }
            /// <summary>
            /// 快速执行简易Insert操作
            /// </summary>
            /// <param name="p"></param>
            /// <param name="toTable"></param>
            /// <param name="data"></param>
            /// <param name="islog"></param>
            /// <returns></returns>
            public virtual UnitDataCollection QuickInsert(UnitParameter p, string toTable, object data, bool islog = false)
            {
                FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Insert',
$table : {0}
}", toTable);
                FrameDLRObject dataexpress = FrameDLRObject.CreateInstance(data);
                foreach (var item in dataexpress.Items)
                {
                    express.SetValue(item.Key, item.Value);
                }

                return Excute(p, express, islog);
            }
            /// <summary>
            /// 快速执行建议Insert操作，根据key值判定如果资料存在则不新增，否则执行新增操作
            /// </summary>
            /// <param name="p"></param>
            /// <param name="toTable"></param>
            /// <param name="data"></param>
            /// <param name="bykeys"></param>
            /// <param name="islog"></param>
            /// <returns></returns>
            public virtual UnitDataCollection QuickInsertNotExists(UnitParameter p,string toTable, object data,object bykeys,bool islog = false)
            {
                //json表达式格式
                /*
    {
    $acttype : 'InsertSelect',
	$table:'orders_goods',
    order_no:true,
    detail_num:true,
    $select:{
        $prefix:'distinct',
        order_no:{0},
        detail_num:{1},
        $where:{
            $notexists:{
                $table:'orders_goods',
                order_no:true,
                $where:{
                    order_no:{0},
					detail_num:{1}
				}
            }
        }
    }
}
                 */
                FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'InsertSelect',
$table : {0}
}", toTable);
                FrameDLRObject dataexpress = FrameDLRObject.CreateInstance(data);
                FrameDLRObject keyexpress = FrameDLRObject.CreateInstance(bykeys);

                FrameDLRObject selectexpress = FrameDLRObject.CreateInstance(@"{
$prefix:'distinct'
}");
                FrameDLRObject selectwheresexpress = FrameDLRObject.CreateInstance();
                selectexpress.SetValue("$where", selectwheresexpress);
                express.SetValue("$select", selectexpress);

                foreach (var item in dataexpress.Items)
                {
                    express.SetValue(item.Key, true);
                    selectexpress.SetValue(item.Key, item.Value);
                }
                FrameDLRObject notexitsexpress = FrameDLRObject.CreateInstanceFromat(@"{
$table : {0}
}", toTable);
                selectwheresexpress.SetValue("$notexists", notexitsexpress);
                FrameDLRObject notexitswhereexpress = FrameDLRObject.CreateInstance();
                notexitsexpress.SetValue("$where", notexitswhereexpress);
                

                foreach (var item in keyexpress.Items)
                {
                    notexitsexpress.SetValue(item.Key, true);
                    notexitswhereexpress.SetValue(item.Key, item.Value);
                }

                return Excute(p, express, islog);
            }
            /// <summary>
            /// 快速执行建议Delete操作
            /// </summary>
            /// <param name="p"></param>
            /// <param name="toTable"></param>
            /// <param name="where"></param>
            /// <param name="islog"></param>
            /// <returns></returns>
            public virtual UnitDataCollection QuickDelete(UnitParameter p, string toTable, object where, bool islog=false)
            {
                FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Delete',
$table : {0}
}", toTable);
                FrameDLRObject whereexpress = FrameDLRObject.CreateInstance(where);
                if (whereexpress != null && whereexpress.Items.Count > 0)
                {
                    express.SetValue("$where", whereexpress);
                }
                return Excute(p, express);
            }
            /// <summary>
            /// 直接调用DB驱动执行大数据量新增操作
            /// 并非所有的数据库引擎支持大数据量的新增操作，具体请看相关操作说明
            /// </summary>
            /// <param name="p"></param>
            /// <param name="toTable"></param>
            /// <param name="data"></param>
            /// <returns></returns>
            public virtual UnitDataCollection BulkInsert(UnitParameter p,string toTable,object data)
            {
                var rtn = new UnitDataCollection();
                if (p.Dao != null && p.Dao is ADBAccess)
                {
                    ((ADBAccess)p.Dao).Insert(data, toTable);
                }
                return rtn;
            }
            /// <summary>
            /// 创建Table
            /// </summary>
            /// <param name="up"></param>
            /// <param name="toTable"></param>
            /// <param name="columns"></param>
            public virtual void CreateTable(UnitParameter up, string toTable, params TableColumn[] columns)
            {
                if (columns == null || columns.Length <= 0) return;
                FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'CreateTable',
$table : {0}
}",FrameDLRFlags.SensitiveCase, toTable);
                var pk = new List<object>();
                foreach (var c in columns)
                {
                    FrameDLRObject cobj = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                    cobj.SetValue("$datatype", c.DataType);
                    cobj.SetValue("$precision", c.Precision);
                    cobj.SetValue("$scale", c.Scale);
                    cobj.SetValue("$default", c.Default);
                    cobj.SetValue("$isnull", c.IsPK ? false : c.AllowNull);
                    if (c.IsPK)
                    {
                        pk.Add(c.Name);
                    }
                    express.SetValue(c.Name, cobj);
                }

                express.SetValue("$pk", pk);
                Excute(up, express);
            }
            /// <summary>
            /// 直接删除Table
            /// </summary>
            /// <param name="up"></param>
            /// <param name="toTable"></param>
            public virtual void DropTable(UnitParameter up,string toTable)
            {
                FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'DropTable',
$table : {0}
}", FrameDLRFlags.SensitiveCase, toTable);
                Excute(up, express);
            }
            /// <summary>
            /// Copy Table
            /// </summary>
            /// <param name="up"></param>
            /// <param name="from_table"></param>
            /// <param name="to_Table"></param>
            /// <param name="with_data"></param>
            /// <param name="is_log">是否记录log</param>
            public virtual void CopyTable(UnitParameter up,string from_table, string to_Table,bool with_data=false, bool is_log = false)
            {
                FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'CopyTable',
$table : {0},
$to_table:{1},
$with_data:{2}
}", FrameDLRFlags.SensitiveCase, from_table, to_Table,with_data);
                Excute(up, express, is_log);
            }
            /// <summary>
            /// Copy Data
            /// </summary>
            /// <param name="up"></param>
            /// <param name="from_table">来源表</param>
            /// <param name="to_Table">目标表</param>
            /// <param name="if_not_exists">为true的时候添加not exists的where条件，否则不过滤，注意：该过滤条件不是根据每笔资料进行比对的，而是根据目标表是否存在数据来作为判定依据，因此会出现只要目标表存在资料，无论与来源数据是否相同都不会执行写入操作</param>
            /// <param name="columns">要copy的栏位，指定来源表的栏位，值为目标表的栏位,格式必须为{'from_column':'to_column'}</param>
            /// <param name="where">条件表达式，与Query指令的表达式一样的格式，该表达式用于过滤来源表</param>
            /// <param name="is_log">是否记录log</param>
            public virtual void CopyData(UnitParameter up, string from_table, string to_Table, bool if_not_exists = false, object columns=null, object where=null,bool is_log=false)
            {
                FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'CopyData',
$table : {0},
$to_table:{1},
$if_not_exists:{2}
}", FrameDLRFlags.SensitiveCase, from_table, to_Table, if_not_exists);
                if(columns != null)
                {
                    FrameDLRObject columnexpress = FrameDLRObject.CreateInstance(columns, FrameDLRFlags.SensitiveCase);
                    foreach (var item in columnexpress.Items)
                    {
                        express.SetValue(item.Key, item.Value);
                    }
                }
                if(where != null)
                {
                    express.SetValue("$where", FrameDLRObject.CreateInstance(where, FrameDLRFlags.SensitiveCase));
                }
                Excute(up, express, is_log);
            }
            /// <summary>
            /// 查询操作
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="p"></param>
            /// <param name="actionflag">动作区分标记</param>
            /// <returns></returns>
            public virtual UnitDataCollection Query<T>(UnitParameter p, string actionflag) where T : IDBUnit<UnitParameter>
            {
                return DBUnitProxy.Query<T>(p, actionflag);
            }
            /// <summary>
            /// 翻页查询
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="p"></param>
            /// <param name="actionflag">动作区分标记</param>
            /// <returns></returns>
            public virtual UnitDataCollection QueryByPage<T>(UnitParameter p, string actionflag) where T : IDBUnit<UnitParameter>
            {
                return DBUnitProxy.QueryByPage<T>(p, actionflag);
            }
            /// <summary>
            /// 非查询类的db操作
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="p"></param>
            /// <param name="actionflag">动作区分标记</param>
            public virtual void NonQuery<T>(UnitParameter p, string actionflag) where T : IDBUnit<UnitParameter>
            {
                DBUnitProxy.NonQuery<T>(p, actionflag);
            }
            /// <summary>
            /// 执行存储过程操作
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="p"></param>
            /// <param name="actionflag">动作区分标记</param>
            /// <returns></returns>
            public virtual UnitDataCollection ExcuteSP<T>(UnitParameter p, string actionflag) where T : IDBUnit<UnitParameter>
            {
                return DBUnitProxy.ExcuteSP<T>(p,actionflag);
            }

           
            /// <summary>
            /// 根据连接串解析db类型和有效的连接串
            /// </summary>
            /// <param name="conn">原始连接串</param>
            /// <param name="parsedConn">解析后的连接串</param>
            /// <returns></returns>
            protected DBType ParseDBConnection(string conn, out string parsedConn)
            {
                var reg = new Regex("^#\\w+#");
                if (reg.IsMatch(conn))
                {
                    parsedConn = reg.Replace(conn, "");
                    var typestr = reg.Match(conn).Value.Replace("#", "").ToLower();
                    if (typestr == "sqlite")
                    {
                        return DBType.Sqlite;
                    }
                    else if (typestr == "sqlserver")
                    {
                        return DBType.SqlServer;
                    }
                    else if (typestr == "mysql")
                    {
                        return DBType.MySql;
                    }
                    else if (typestr == "db2")
                    {
                        return DBType.DB2;
                    }
                    else if (typestr == "oracle")
                    {
                        return DBType.Oracle;
                    }
                    else
                    {
                        return DBType.None;
                    }
                }
                else
                {
                    parsedConn = conn;
                    return DBType.None;
                }
            }
        }
        
    }

   
}
