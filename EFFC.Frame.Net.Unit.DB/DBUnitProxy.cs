using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Unit.DB.Datas;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Unit.DB.Unit;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Unit.DB
{
    /// <summary>
    /// DB Unit的呼叫Proxy
    /// </summary>
    public class DBUnitProxy:UnitProxy
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TUnit"></typeparam>
        /// <param name="p"></param>
        /// <param name="actionflag"></param>
        /// <returns></returns>
        public static UnitDataCollection Query<TUnit>(UnitParameter p, string actionflag) 
            where TUnit : IDBUnit<UnitParameter>
        {
            p.SetValue("_unit_action_flag_", actionflag);
            return (UnitDataCollection)Call<QueryUnit<TUnit>>(p);
        }
        /// <summary>
        /// 翻页查询
        /// </summary>
        /// <typeparam name="TUnit"></typeparam>
        /// <param name="p"></param>
        /// <param name="actionflag"></param>
        /// <returns></returns>
        public static UnitDataCollection QueryByPage<TUnit>(UnitParameter p, string actionflag)
            where TUnit : IDBUnit<UnitParameter>
        {
            p.SetValue("_unit_action_flag_", actionflag);
            return (UnitDataCollection)Call<QueryByPageUnit<TUnit>>(p);
        }
        /// <summary>
        /// 非查询类的db操作
        /// </summary>
        /// <typeparam name="TUnit"></typeparam>
        /// <param name="p"></param>
        /// <param name="actionflag">动作区分标记</param>
        public static void NonQuery<TUnit>(UnitParameter p, string actionflag)
            where TUnit : IDBUnit<UnitParameter>
        {
            p.SetValue("_unit_action_flag_", actionflag);
            Call<NonQueryUnit<TUnit>>(p);
        }
        /// <summary>
        /// 执行存储过程操作
        /// </summary>
        /// <typeparam name="TUnit"></typeparam>
        /// <param name="p"></param>
        /// <param name="actionflag">动作区分标记</param>
        /// <returns></returns>
        public static UnitDataCollection ExcuteSP<TUnit>(UnitParameter p, string actionflag)
            where TUnit : IDBUnit<UnitParameter>
        {
            p.SetValue("_unit_action_flag_", actionflag);
            return (UnitDataCollection)Call<SPUnit<TUnit>>(p);
        }

        /// <summary>
        /// 标准化DB执行操作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="express"></param>
        /// <returns></returns>
        public static UnitDataCollection Excute(UnitParameter p, DBExpress express)
        {
            UnitDataCollection rtn = new UnitDataCollection();
            if (express != null)
            {
                p.SetValue("__json__", express);
                if (p.Dao is ADBAccess)
                {
                    if (express.CurrentAct == DBExpress.ActType.Query)
                    {
                        rtn = Query<JsonExpressUnit>(p, "");
                    }
                    else if (express.CurrentAct == DBExpress.ActType.QueryByPage)
                    {
                        rtn = QueryByPage<JsonExpressUnit>(p, "");
                    }
                    else
                    {
                        NonQuery<JsonExpressUnit>(p, "");
                    }
                }

            }
            return rtn;
        }
        /// <summary>
        /// 通过json对象执行标准化DB操作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static UnitDataCollection Excute(UnitParameter p, FrameDLRObject json)
        {
            DBExpress express = null;
            if (p.Dao is ADBAccess)
            {
                express = ((ADBAccess)p.Dao).MyDBExpress;
                DBExpress.Load(express, json);
            }

            return Excute(p, express);
        }
        /// <summary>
        /// 通过json串执行标准化DB操作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static UnitDataCollection Excute(UnitParameter p, string json)
        {
            return Excute(p, FrameDLRObject.CreateInstance(json));
        }
    }
}
