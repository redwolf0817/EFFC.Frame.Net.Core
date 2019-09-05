using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EFFC.Frame.Net.Unit.DB.Unit
{
    /// <summary>
    /// 非查询操作的Unit
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NonQueryUnit<T> : IUnit where T : IDBUnit<UnitParameter>
    {

        public DataCollection DoOperate(ParameterStd p)
        {
            string flag = p.GetValue<string>("_unit_action_flag_");
            UnitParameter up = (UnitParameter)p;
            UnitDataCollection rtn = new UnitDataCollection();
            if (up.Dao is ADBAccess)
            {
                T t = (T)Activator.CreateInstance(typeof(T), true);
                var sqlobj = t.GetSqlFunc(flag)(up);
                string sql = "";
                if (sqlobj is FrameDLRObject)
                {
                    sql = sqlobj.sql;
                }
                else
                {
                    throw new TypeRequiredException("需要指定的动态数据对象：FrameDLRObject");
                }
                ADBAccess dba = (ADBAccess)up.Dao;
                DBOParameterCollection dbc = new DBOParameterCollection();

                if (!string.IsNullOrEmpty(sql))
                {
                    string regstr = "";

                    regstr = @"(?<=" + (dba.ParameterFlagChar == "$" ? "\\$" : dba.ParameterFlagChar) + @")[A-Za-z0-9_]+\d*";

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
                    tmpsql = sql.Replace("''", "#sp#");
                    foreach (Match m in re2.Matches(tmpsql))
                    {
                        tmpsql = tmpsql.Replace(m.Value, "#sp#");
                    }
                    foreach (System.Text.RegularExpressions.Match m in re.Matches(tmpsql))
                    {
                        if (up.GetValue(m.ToString()) is byte[])
                        {
                            dbc.Add(m.ToString(), up.GetValue(m.ToString()), System.Data.DbType.Binary);
                        }
                        else
                        {
                            dbc.Add(m.ToString(), up.GetValue(m.ToString()));
                        }
                    }
                }
                try
                {
                    dba.ExecuteNoQuery(sql, dbc);
                }
                catch (Exception ex)
                {
                    FrameDLRObject dp = FrameDLRObject.CreateInstance(Base.Constants.FrameDLRFlags.SensitiveCase);
                    foreach (var item in dbc)
                    {
                        if (item.Value.ParameterValue is DateTime)
                        {
                            dp.SetValue(item.Key, DateTimeStd.IsDateTimeThen(item.Value.ParameterValue, "yyyy-MM-dd HH:mm:ss"));
                        }
                        else if (item.Value.ParameterValue is DBNull)
                        {
                            dp.SetValue(item.Key, null);
                        }
                        else
                        {
                            dp.SetValue(item.Key, item.Value.ParameterValue);
                        }
                    }
                    GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.ERROR, $"NoQuerySql={sql};\nParameters={dp.ToJSONString()}");

                    throw ex;
                }
            }
            return rtn;
        }
    }
}
