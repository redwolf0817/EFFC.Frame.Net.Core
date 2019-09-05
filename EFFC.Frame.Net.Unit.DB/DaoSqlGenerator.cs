using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Unit.DB.Unit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EFFC.Frame.Net.Unit.DB
{
    /// <summary>
    /// Dao相关的LinqDLR2Sql的操作
    /// </summary>
    public class DaoSqlGenerator : GeneralLinqDLR2SQLGenerator
    {
        /// <summary>
        /// 当前生成的SQL语句
        /// </summary>
        public string CurrentSQL
        {
            get;
            protected set;
        }
        /// <summary>
        /// Dao相关的LinqDLR2Sql的操作
        /// </summary>
        /// <param name="sqlflags"></param>
        public DaoSqlGenerator(SqlOperatorFlags sqlflags) : base(sqlflags)
        {
        }
        /// <summary>
        /// 执行update操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        public virtual void DoUpdate<TSource>(LinqDLR2Sql<TSource> source,object columns)
        {
            if (columns == null) throw new ArgumentNullException("columns");
            if (CurrentOperation == LinqDLR2SQLOperation.SelectJoin
               || CurrentOperation == LinqDLR2SQLOperation.SelectMany)
            {
                throw new NotSupportedException("当前模式不支持Update操作");
            }

            FrameDLRObject columnsobj = FrameDLRObject.CreateInstance(columns);
            var columnsstr = "";
            foreach(var key in columnsobj.Keys)
            {
                columnsstr += $"{string.Format(SqlFlags.Column_Quatation, key)}={Convert2Express(columnsobj.GetValue(key))},";
            }
            columnsstr = columnsstr == "" ? "" : columnsstr.Substring(0, columnsstr.Length - 1);

            var sql = "UPDATE {tables} SET {columns} {where}";
            var alianname = string.IsNullOrEmpty(source.AliasName) ? "" : $"{source.AliasName}";
            var tables = source.Table;
            var where = (Lastwhere == null || string.IsNullOrEmpty(Lastwhere.Result)) ? "" : $"WHERE {Lastwhere.Result.Replace($"{alianname}.", "")}";
            CurrentSQL = sql.Replace("{tables}", tables).Replace("{columns}", columnsstr).Replace("{where}", where);
        }
        /// <summary>
        /// 执行Delete操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        public virtual void DoDelete<TSource>(LinqDLR2Sql<TSource> source)
        {
            if (CurrentOperation == LinqDLR2SQLOperation.SelectJoin
                || CurrentOperation == LinqDLR2SQLOperation.SelectMany)
            {
                throw new NotSupportedException("当前模式不支持Delete操作");
            }

            var sql = "DELETE FROM {tables} {where}";
            var alianname = string.IsNullOrEmpty(source.AliasName) ? "" : $"{source.AliasName}";
            var tables = source.Table;
            var where = (Lastwhere == null || string.IsNullOrEmpty(Lastwhere.Result)) ? "" : $"WHERE {Lastwhere.Result.Replace($"{alianname}.","")}";
            CurrentSQL = sql.Replace("{tables}", tables).Replace("{where}", where);
        }
        /// <summary>
        /// 执行Insert操作
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="columns"></param>
        public virtual void DoInsert<TSource>(LinqDLR2Sql<TSource> source,object columns)
        {
            if (columns == null) throw new ArgumentNullException("columns");
            if (CurrentOperation == LinqDLR2SQLOperation.SelectJoin
               || CurrentOperation == LinqDLR2SQLOperation.SelectMany)
            {
                throw new NotSupportedException("当前模式不支持INSERT操作");
            }

            FrameDLRObject columnsobj = FrameDLRObject.CreateInstance(columns);
            var columnsstr = "";
            var valuestr = "";
            foreach (var key in columnsobj.Keys)
            {
                columnsstr += $"{string.Format(SqlFlags.Column_Quatation, key)},";
                valuestr += $"{Convert2Express(columnsobj.GetValue(key))},";
            }
            columnsstr = columnsstr == "" ? "" : columnsstr.Substring(0, columnsstr.Length - 1);
            valuestr = valuestr == "" ? "" : valuestr.Substring(0, valuestr.Length - 1);

            var sql = "INSERT INTO {tables}({columns})VALUES({values})";
            var alianname = string.IsNullOrEmpty(source.AliasName) ? "" : $"{source.AliasName}";
            var tables = source.Table;
            CurrentSQL = sql.Replace("{tables}", tables).Replace("{columns}", columnsstr).Replace("{values}", valuestr);
        }
        

        /// <summary>
        /// 将右侧值转化成对应的表达式，并添加条件值
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual string Convert2Express(object v)
        {
            var re = "";

            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
            //防止有key重复导致的异常
            while (ConditionValues.ContainsKey(pname))
            {
                pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
            }
            ConditionValues.Add(pname, v);

            re = $"{SqlFlags.ParamFlag}{pname}";


            return re;
        }
    }
}
