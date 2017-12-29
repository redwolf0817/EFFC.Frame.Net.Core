using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EFFC.Frame.Net.Unit.DataObjectDefine.Unit
{
    public partial class DODBaseUnit
    {
        private const string daokey = "__dod_dao_dic_key____";
        private static object lockdaoobj = new object();
        private T GetDao<T>(string connstr, string dbname) where T : IResourceEntity, IDBAccessInfo
        {
            T rtn = default(T);

            rtn = _rm.CreateInstance<T>();
            rtn.Open(connstr, dbname);
            return rtn;
        }
        /// <summary>
        /// DB资料查询
        /// 自动识别sql中的参数，并从传入的参数集中找对应的参数，如果没有，则需补充自定义的参数定义
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connstr"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual UnitDataCollection Query<T>(string connstr, string sql, params KeyValuePair<string, object>[] param) where T : ADBAccess, IResourceEntity
        {
            T dao = GetDao<T>(connstr, null);//_rm.CreateInstance<T>(_token);
            try
            {
                //dao.Open(connstr);
                DBOParameterCollection dbp = new DBOParameterCollection();
                Dictionary<string, object> _dp = _p.Domain(DomainKey.INPUT_PARAMETER);
                foreach (var v in param)
                {
                    if (!_dp.ContainsKey(v.Key))
                    {
                        _dp.Add(v.Key, v.Value);
                    }
                }

                if (!string.IsNullOrEmpty(sql))
                {
                    string regstr = "";
                    regstr = @"(?<=" + dao.ParameterFlagChar + @")[A-Za-z0-9_]+\d*";

                    string regexpress = @"(?isx)
                                (')                                                           #开始标记“<tag...>”
                                (?>                                                                  #分组构造，用来限定量词“*”修饰范围
                                \1  (?<Open>)                                                 #命名捕获组，遇到开始标记，入栈，Open计数加1
                                |\1  (?<-Open>)                                                   #狭义平衡组，遇到结束标记，出栈，Open计数减1
                                |[^']*                                                   #右侧不为开始或结束标记的任意字符
                                )
                                (?(Open)(?!))                                                        #判断是否还有'OPEN'，有则说明不配对，什么都不匹配
                                \1                                                                #结束标记“</tag>”
                     ";
                    Regex re = new Regex(regstr);
                    string tmpsql = "";
                    Regex re2 = new Regex(regexpress);
                    tmpsql = sql;
                    foreach (Match m in re2.Matches(tmpsql))
                    {
                        tmpsql = tmpsql.Replace(m.Value, "#sp#");
                    }
                    foreach (System.Text.RegularExpressions.Match m in re.Matches(tmpsql))
                    {
                        if (_dp[m.ToString()] is byte[])
                        {
                            dbp.Add(m.ToString(), _dp[m.ToString()], System.Data.DbType.Binary);
                        }
                        else
                        {
                            dbp.Add(m.ToString(), _dp[m.ToString()]);
                        }
                    }
                }


                UnitDataCollection rtn = new UnitDataCollection();
                rtn.QueryDatas = dao.Query(sql, dbp);
                if (rtn.QueryDatas != null)
                {
                    if (rtn.QueryDatas.Tables.Count > 0)
                    {
                        rtn.QueryTable = rtn.QueryDatas[0];
                    }
                }

                return rtn;
            }
            finally
            {
                dao.Close();
            }
        }
        /// <summary>
        /// DB资料查询，连接串使用默认框架默认连接
        /// 自动识别sql中的参数，并从传入的参数集中找对应的参数，如果没有，则需补充自定义的参数定义
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected UnitDataCollection Query<T>(string sql, params KeyValuePair<string, object>[] param) where T : ADBAccess, IResourceEntity
        {
            return Query<T>(ComFunc.nvl(Configs[ParameterKey.DBCONNECT_STRING]), sql, param);
        }
        /// <summary>
        /// DB资料非查询
        /// 自动识别sql中的参数，并从传入的参数集中找对应的参数，如果没有，则需补充自定义的参数定义
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connstr"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual void NonQuery<T>(string connstr, string sql, params KeyValuePair<string, object>[] param) where T : ADBAccess, IResourceEntity
        {
            T dao = GetDao<T>(connstr, null);
            try
            {
                DBOParameterCollection dbp = new DBOParameterCollection();
                Dictionary<string, object> _dp = _p.Domain(DomainKey.INPUT_PARAMETER);
                foreach (var v in param)
                {
                    if (!_dp.ContainsKey(v.Key))
                    {
                        _dp.Add(v.Key, v.Value);
                    }
                }

                if (!string.IsNullOrEmpty(sql))
                {
                    string regstr = "";
                    regstr = @"(?<=" + dao.ParameterFlagChar + @")[A-Za-z0-9_]+\d*";
                    string regexpress = @"(?isx)
                                (')                                                           #开始标记“<tag...>”
                                (?>                                                                  #分组构造，用来限定量词“*”修饰范围
                                \1  (?<Open>)                                                 #命名捕获组，遇到开始标记，入栈，Open计数加1
                                |\1  (?<-Open>)                                                   #狭义平衡组，遇到结束标记，出栈，Open计数减1
                                |[^']*                                                   #右侧不为开始或结束标记的任意字符
                                )
                                (?(Open)(?!))                                                        #判断是否还有'OPEN'，有则说明不配对，什么都不匹配
                                \1                                                                #结束标记“</tag>”
                     ";
                    Regex re = new Regex(regstr);
                    string tmpsql = "";
                    Regex re2 = new Regex(regexpress);
                    tmpsql = sql;
                    foreach (Match m in re2.Matches(tmpsql))
                    {
                        tmpsql = tmpsql.Replace(m.Value, "#sp#");
                    }
                    foreach (System.Text.RegularExpressions.Match m in re.Matches(tmpsql))
                    {
                        if (_dp[m.ToString()] is byte[])
                        {
                            dbp.Add(m.ToString(), _dp[m.ToString()], System.Data.DbType.Binary);
                        }
                        else
                        {
                            dbp.Add(m.ToString(), _dp[m.ToString()]);
                        }
                    }
                }


                UnitDataCollection rtn = new UnitDataCollection();
                dao.ExecuteNoQuery(sql, dbp);
            }
            finally
            {
                dao.Close();
            }
        }
        /// <summary>
        ///  DB资料非查询，连接串使用默认框架默认连接
        /// 自动识别sql中的参数，并从传入的参数集中找对应的参数，如果没有，则需补充自定义的参数定义
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        protected void NonQuery<T>(string sql, params KeyValuePair<string, object>[] param) where T : ADBAccess, IResourceEntity
        {
            NonQuery<T>(ComFunc.nvl(Configs[ParameterKey.DBCONNECT_STRING]), sql, param);
        }
        /// <summary>
        /// 执行sp
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connstr"></param>
        /// <param name="spname"></param>
        /// <param name="isreturnds"></param>
        /// <param name="dbp"></param>
        /// <returns></returns>
        protected virtual UnitDataCollection ExcuteSP<T>(string connstr, string spname, bool isreturnds, DBOParameterCollection dbp) where T : ADBAccess, IResourceEntity
        {
            T dao = GetDao<T>(connstr, null);
            try
            {
                UnitDataCollection rtn = new UnitDataCollection();
                DBDataCollection dbrtn = dao.ExcuteProcedure(ComFunc.nvl(spname), isreturnds, ref dbp);
                if (dbrtn.IsSuccess)
                {
                    foreach (string s in dbrtn.Keys)
                    {
                        if (dbrtn[s] is DataSetStd)
                        {
                            rtn.QueryDatas = dbrtn.ReturnDataSet;
                        }
                        else
                        {
                            rtn.SetValue(s, dbrtn[s]);
                        }
                    }

                }
                return rtn;
            }
            finally
            {
                dao.Close();
            }
        }
        /// <summary>
        /// 执行SP，连接串使用默认框架默认连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spname"></param>
        /// <param name="isreturnds"></param>
        /// <param name="dbp"></param>
        /// <returns></returns>
        protected UnitDataCollection ExcuteSP<T>(string spname, bool isreturnds, DBOParameterCollection dbp) where T : ADBAccess, IResourceEntity
        {
            return ExcuteSP<T>(ComFunc.nvl(Configs[ParameterKey.DBCONNECT_STRING]), spname, isreturnds, dbp);
        }
    }
}
