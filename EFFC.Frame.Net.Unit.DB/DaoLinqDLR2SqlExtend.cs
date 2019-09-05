using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Unit.DB.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Unit.DB
{
    /// <summary>
    /// LinqDLR2Sql操作扩展实现Dao层级的相关操作
    /// </summary>
    public static class DaoLinqDLR2SqlExtend
    {
        /// <summary>
        /// 直接执行query操作，并返回结果
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static UnitDataCollection Query<TSource>(this LinqDLR2Sql<TSource> source,UnitParameter up)
        {
            var sql = source.ToSql();
            var data = source.SQLGenerator.ConditionValues;
            up.SetValue("sql", sql);
            foreach (var item in data)
            {
                up.SetValue(item.Key, item.Value);
            }
            return DBUnitProxy.Query<LamdaExpressUnit>(up, "");
        }
        /// <summary>
        /// 直接执行query操作，并返回结果集
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static List<FrameDLRObject> GetQueryList<TSource>(this LinqDLR2Sql<TSource> source, UnitParameter up)
        {
            return source.Query(up).QueryData<FrameDLRObject>();
        }
        /// <summary>
        /// 直接执行union query操作，并返回结果集
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="order_by"></param>
        /// <param name="is_all"></param>
        /// <param name="express"></param>
        /// <returns></returns>
        public static List<FrameDLRObject> GetUnionQueryList<TSource>(this LinqDLR2Sql<TSource> source, UnitParameter up, string order_by, bool is_all = false, params LinqDLR2Sql<TSource>[] express)
        {
            return source.UnionQuery(up, order_by, is_all, express).QueryData<FrameDLRObject>();
        }
        /// <summary>
        /// 直接执行翻页查询
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="orderby">排序语句</param>
        /// <returns></returns>
        public static UnitDataCollection QueryByPage<TSource>(this LinqDLR2Sql<TSource> source, UnitParameter up, string orderby = "")
        {
            var sql = source.ToSql();
            var data = source.SQLGenerator.ConditionValues;
            up.SetValue("sql", sql);
            up.SetValue("orderby", orderby);
            foreach (var item in data)
            {
                up.SetValue(item.Key, item.Value);
            }
            return DBUnitProxy.QueryByPage<LamdaExpressUnit>(up, "");
        }
        /// <summary>
        /// 直接执行union query操作，并返回结果
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="order_by"></param>
        /// <param name="is_all"></param>
        /// <param name="express"></param>
        /// <returns></returns>
        public static UnitDataCollection UnionQuery<TSource>(this LinqDLR2Sql<TSource> source, UnitParameter up,string order_by, bool is_all = false, params LinqDLR2Sql<TSource>[] express)
        {
            var sql = source.ToSql();
            var data = source.SQLGenerator.ConditionValues;
            if (express != null)
            {
                foreach(var e in express)
                {
                    sql += $@" {(is_all?"union all":"union")}
{e.ToSql()}";
                    foreach(var item in e.SQLGenerator.ConditionValues)
                    {
                        data.Add(item.Key, item.Value);
                    }
                }
            }
            sql = $"select * from ({sql}) t";
            if (!string.IsNullOrEmpty(order_by))
            {
                sql += $" order by {order_by}";
            }
            up.SetValue("sql", sql);
            foreach (var item in data)
            {
                up.SetValue(item.Key, item.Value);
            }
            return DBUnitProxy.Query<LamdaExpressUnit>(up, "");
        }
        /// <summary>
        /// 直接执行union querybypage操作，并返回结果
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="order_by"></param>
        /// <param name="is_all"></param>
        /// <param name="express"></param>
        /// <returns></returns>
        public static UnitDataCollection UnionQueryByPage<TSource>(this LinqDLR2Sql<TSource> source, UnitParameter up,string order_by, bool is_all = false, params LinqDLR2Sql<TSource>[] express)
        {
            var sql = source.ToSql();
            up.SetValue("orderby", order_by);
            var data = source.SQLGenerator.ConditionValues;
            if (express != null)
            {
                foreach (var e in express)
                {
                    sql += $@"
{(is_all ? "union all" : "union")}
{e.ToSql()}";
                    foreach (var item in e.SQLGenerator.ConditionValues)
                    {
                        data.Add(item.Key, item.Value);
                    }
                }
            }
            up.SetValue("sql", sql);
            foreach (var item in data)
            {
                up.SetValue(item.Key, item.Value);
            }
            return DBUnitProxy.QueryByPage<LamdaExpressUnit>(up, "");
        }
        /// <summary>
        /// 直接执行Update操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="columns"></param>
        public static void Update<TSource>(this LinqDLR2Sql<TSource> source, UnitParameter up, object columns)
        {
            if (source.SQLGenerator is DaoSqlGenerator)
            {
                ((DaoSqlGenerator)source.SQLGenerator).DoUpdate(source, columns);
                var sql = ((DaoSqlGenerator)source.SQLGenerator).CurrentSQL;
                var data = source.SQLGenerator.ConditionValues;
                up.SetValue("sql", sql);
                foreach (var item in data)
                {
                    up.SetValue(item.Key, item.Value);
                }
                DBUnitProxy.NonQuery<LamdaExpressUnit>(up, "nonquery");
            }
            else
            {
                throw new NotSupportedException("当前对象不支持Update操作");
            }
        }
        /// <summary>
        /// 直接执行Delete操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        public static void Delete<TSource>(this LinqDLR2Sql<TSource> source,UnitParameter up)
        {
            if (source.SQLGenerator is DaoSqlGenerator)
            {
                ((DaoSqlGenerator)source.SQLGenerator).DoDelete(source);
                var sql = ((DaoSqlGenerator)source.SQLGenerator).CurrentSQL;
                var data = source.SQLGenerator.ConditionValues;
                up.SetValue("sql", sql);
                foreach (var item in data)
                {
                    up.SetValue(item.Key, item.Value);
                }
                DBUnitProxy.NonQuery<LamdaExpressUnit>(up, "nonquery");
            }
            else
            {
                throw new NotSupportedException("当前对象不支持Delete操作");
            }
        }
        /// <summary>
        /// 直接执行Insert操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="columns"></param>
        public static void Insert<TSource>(this LinqDLR2Sql<TSource> source,UnitParameter up, object columns)
        {
            if (source.SQLGenerator is DaoSqlGenerator)
            {
                ((DaoSqlGenerator)source.SQLGenerator).DoInsert(source, columns);
                var sql = ((DaoSqlGenerator)source.SQLGenerator).CurrentSQL;
                var data = source.SQLGenerator.ConditionValues;
                up.SetValue("sql", sql);
                foreach (var item in data)
                {
                    up.SetValue(item.Key, item.Value);
                }
                DBUnitProxy.NonQuery<LamdaExpressUnit>(up, "nonquery");
            }
            else
            {
                throw new NotSupportedException("当前对象不支持Insert操作");
            }
        }

        /// <summary>
        /// 直接执行count的DB操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static int Count<TSource>(this LinqDLR2Sql<TSource> source, UnitParameter up)
        {
            var result = source.Count().Query(up);
            return result.QueryTable.RowLength > 0 ? IntStd.IsNotIntThen(result.QueryTable[0, 0]) : 0;
        }
        /// <summary>
        /// 直接执行max操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static object Max<TSource, TResult>(this LinqDLR2Sql<TSource> source, UnitParameter up, Func<TSource, TResult> selector)
        {
            return source.Max(selector).Query(up).QueryTable[0,0];
        }
        /// <summary>
        /// 执行max操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static object Max<TSource>(this LinqDLR2Sql<TSource> source, UnitParameter up, string column)
        {
            return source.Max(column).Query(up).QueryTable[0, 0];
        }
        /// <summary>
        /// 执行Min操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static object Min<TSource, TResult>(this LinqDLR2Sql<TSource> source, UnitParameter up, Func<TSource, TResult> selector)
        {
            return source.Min(selector).Query(up).QueryTable[0, 0];
        }
        /// <summary>
        /// 执行Min操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static object Min<TSource>(this LinqDLR2Sql<TSource> source, UnitParameter up, string column)
        {
            return source.Min(column).Query(up).QueryTable[0, 0];
        }
        /// <summary>
        /// 执行Sum操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static object Sum<TSource, TResult>(this LinqDLR2Sql<TSource> source, UnitParameter up, Func<TSource, TResult> selector)
        {
            return source.Sum(selector).Query(up).QueryTable[0, 0];
        }
        /// <summary>
        /// 执行Sum操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static object Sum<TSource>(this LinqDLR2Sql<TSource> source, UnitParameter up, string column)
        {
            return source.Sum(column).Query(up).QueryTable[0, 0];
        }
        /// <summary>
        /// 执行Avg操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static object Avg<TSource, TResult>(this LinqDLR2Sql<TSource> source, UnitParameter up, Func<TSource, TResult> selector)
        {
            return source.Avg(selector).Query(up).QueryTable[0, 0];
        }
        /// <summary>
        /// 执行Avg操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static object Avg<TSource>(this LinqDLR2Sql<TSource> source, UnitParameter up, string column)
        {
            return source.Avg(column).Query(up).QueryTable[0, 0];
        }
        /// <summary>
        /// 执行Distinct操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static UnitDataCollection Distinct<TSource>(this LinqDLR2Sql<TSource> source, UnitParameter up)
        {
            return source.Distinct().Query(up);
        }

        /// <summary>
        /// 判断资料是否存在
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static bool IsExists<TSource>(this LinqDLR2Sql<TSource> source,UnitParameter up)
        {
            return source.Count(up) > 0;
        }
    }
}
