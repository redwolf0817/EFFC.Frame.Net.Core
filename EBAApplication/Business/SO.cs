using EFFC.Frame.Net.Module.Extend.EBA.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Extends.LinqDLR2SQL;
using System.Linq;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Unit.DB.Parameters;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EBAApplication.Business;
using EBAApplication.Unit;

namespace BA_BatchApplication.Business
{
    class SO : MyConsoleLogic
    {
        [EBAFixTime(-1, -1, 18, 30, 0)]
        [EBAGroup("so", "钱包功能使用")]
        [EBARepeatWhenException(typeof(Exception), 2)]
        [EBAIsOpen(true)]
        void load(dynamic args)
        {
            //BeginTrans();
            var uppsql = DB.NewPSQLDBUnitParameter();//psi_ba
            var up = DB.NewDBUnitParameter();//patc_ba

            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, DateTime.Now.ToString() + "开始");
            var s2 = from t in DB.LamdaTable("common_sale_proxy", "a")
                     select new
                     {
                         t.prod_code,
                         t.state,
                         t.dealer_type,
                         t.dealer_code,
                         t.platform_code
                     };//省包关系表
            var s3 = from t in DB.LamdaTable("common_hw_prod_map", "a")
                     select new
                     {
                         t.prod_code,
                         t.prod_code_hw
                     };//华为关系表

            //comp_code
            var comp = from t in DB.LamdaTable("t_company", "a")
                       where t.is_company == "Y"
                       select new
                       {
                           t.comp_code
                       };
            var salemapresult = DB.ExcuteLamda(up, s2);//省包关系表
            var prodmapresult = DB.ExcuteLamda(up, s3);//华为关系表

            var compcoderesult = ComFunc.nvl(DB.ExcuteLamda(up, comp).QueryTable[0, 0]);//配置
            var salemaplist = (from t in salemapresult.QueryData<FrameDLRObject>()
                               select new
                               {
                                   key = ComFunc.nvl(t.prod_code).Replace(" ", "") + "|" + t.dealer_type + "|" + t.dealer_code,
                                   value = t.dealer_code
                               }).ToList();
            salemaplist.AddRange(from t in salemapresult.QueryData<FrameDLRObject>()
                                 where ComFunc.nvl(t.dealer_type) == "1"
                                 select new
                                 {
                                     key = ComFunc.nvl(t.prod_code).Replace(" ", "") + t.state.Replace("省", "").Replace("市", "").Replace("壮族自治区", "").Replace("回族自治区", "").Replace("回族自治区", "").Replace("维吾尔自治区", "").Replace("自治区", ""),
                                     value = t.dealer_code
                                 });
            salemaplist.AddRange(from t in salemapresult.QueryData<FrameDLRObject>()
                                 where !String.IsNullOrEmpty(ComFunc.nvl(t.platform_code)) && ComFunc.nvl(t.dealer_type) == "1"
                                 select new
                                 {
                                     key = ComFunc.nvl(t.prod_code).Replace(" ", "") + t.platform_code,
                                     value = t.dealer_code
                                 });
            var salemap = salemaplist.ToDictionary<string, string>(k => k.key, v => v.value);
            var prodmap = (from t in prodmapresult.QueryData<FrameDLRObject>()
                           where (!String.IsNullOrEmpty(t.prod_code) || !String.IsNullOrEmpty(t.prod_code_hw))
                           select new
                           {
                               key = ComFunc.nvl(t.prod_code_hw).Replace(" ", ""),
                               value = ComFunc.nvl(t.prod_code).Replace(" ", "")
                           }).ToDictionary<string, string>(k => k.key, v => v.value);
            var smax = (from t in DB.PSQLLamdaTable("tempt_terminal_so_line", "a")
                        select t.modi_time).Max("a.modi_time");
            var maxresult = DB.ExcuteLamda(up, smax);
            var max_modi_time = maxresult.QueryTable.RowLength > 0 ? ComFunc.nvl(maxresult.QueryTable[0, 0]) : "";
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, max_modi_time + "开始汇入temp资料!");
            var dt = DateTime.Now;
            var s1 = from t in DB.PSQLLamdaTable("t_terminal_so_line", "id")
                     where t.modi_time != null && t.notnull(max_modi_time, t.append(t.modi_time > max_modi_time, "::timestamp"))
                     select new
                     {
                         t.id,
                         t.comp_code,
                         t.prod_code,
                         t.qty,
                         t.form_date,
                         t.modi_time,
                         t.busi_type,
                         t.province_name_st,
                         t.dealer_code,
                         t.dealer_code2,
                         t.dealer_code3,
                         t.unit,
                         t.created_date,
                         t.province_name_so,
                         t.dealer_name,
                         t.dealer_name2,
                         t.dealer_name3
                     };
            uppsql.Count_Of_OnePage = 100000;
            uppsql.ToPage = 1;
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"执行第1页查询:{s1.ToSql()}"); dt = DateTime.Now;
            var result = DB.LamdaQueryByPage(uppsql, s1, "id");
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"执行第1页查询:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;
            var count = 0;
            var today = DateTime.Now.ToString("yyyyMMdd");
            //test
            do
            {
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, DateTime.Now.ToString() + "1");
                var list = (from t in result.QueryData<FrameDLRObject>()
                            select new
                            {
                                rowid = Guid.NewGuid().ToString().Replace("-", ""),
                                t.id,
                                t.comp_code,
                                t.prod_code,
                                t.qty,
                                t.form_date,
                                t.modi_time,
                                t.busi_type,
                                t.province_name_st,
                                t.dealer_code,
                                t.dealer_code2,
                                t.dealer_code3,
                                t.unit,
                                t.created_date,
                                t.province_name_so,
                                status = "N",
                                t.dealer_name,
                                t.dealer_name2,
                                t.dealer_name3,
                                cdate = today
                            }).ToList();
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"执行转化uuid查询:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;
                DB.BulkInsert(up, "tempt_terminal_so_line", list);
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"执行bulkinsert:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;
                count += list.Count();
                list.Clear();
                uppsql.ToPage++;
                result.Clear();
                result = DB.LamdaQueryByPage(uppsql, s1, "id");
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"执行第{uppsql.ToPage}页查询:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;
            } while (result.QueryTable.RowLength > 0 && result.CurrentPage == uppsql.ToPage);
            //-------------------------------插入temp表
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, DateTime.Now.ToString() + "汇入temp:" + count + "笔资料!");
            insertTemp(up, prodmap, salemap, compcoderesult, today);
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, DateTime.Now.ToString() + "结束");
        }

        #region insertTemp
        public void insertTemp(UnitParameter up, Dictionary<string, string> prodmap, Dictionary<string, string> salemap, string compcoderesult, string today)
        {
            //先把tempt_terminal_so_line为N的都改为今天的数据
            up.SetValue("today", today);
            DB.NonQuery<SoUnit>(up, "updatesstatus");
            var temp = from t in DB.LamdaTable("tempt_terminal_so_line", "a")
                       where t.cdate == today
                       select new
                       {
                           t.id,
                           t.comp_code,
                           t.prod_code,
                           t.qty,
                           t.form_date,
                           t.modi_time,
                           t.busi_type,
                           t.province_name_st,
                           t.dealer_code,
                           t.dealer_code2,
                           t.dealer_code3,
                           t.unit,
                           t.created_date,
                           t.province_name_so,
                           t.status,
                           t.dealer_name,
                           t.dealer_name2,
                           t.dealer_name3
                       };//筛选为N的数据
            up.Count_Of_OnePage = 100000;
            up.ToPage = 1;
            var dt = DateTime.Now;
            var tempresult = DB.LamdaQueryByPage(up, temp, "id");
            var tempcount = 0;
            var c1 = 0;//总部发货c1
            var c2 = 0;//非总部发货c2
            var p3 = 0;//排除掉的p3
            var p4 = 0;//排除掉的p4
            do
            {
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"执行第{up.ToPage}页数据:{ tempresult.QueryData<FrameDLRObject>().Count}"); dt = DateTime.Now;
                var list = new List<dynamic>();
                var listall = (from t in tempresult.QueryData<FrameDLRObject>()
                               select t).ToList();
                foreach (dynamic item in listall)
                {
                    //GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"执行第{up.ToPage}页数据ID:{item.id}"); dt = DateTime.Now;
                    //Prod_code  Dealercode_ok 这2个只要一个 为空 就算没成功的数据 
                    if (prodmap.ContainsKey(item.prod_code))//Prod_code
                    {
                        dynamic rtn = FrameDLRObject.CreateInstance();
                        rtn.id = item.id;
                        rtn.dealer_code = "";
                        rtn.comp_code = item.comp_code;
                        rtn.prod_code = prodmap[item.prod_code];
                        rtn.qty = item.qty;
                        rtn.prod_code_hw = item.prod_code;
                        rtn.dealer_code1 = item.dealer_code;
                        rtn.dealer_code2 = item.dealer_code2;
                        rtn.dealer_code3 = item.dealer_code3;
                        rtn.busi_type = item.busi_type;
                        rtn.province_name_st = item.province_name_st;
                        rtn.form_date = item.form_date;
                        rtn.modi_time = item.modi_time;
                        rtn.province_name_st_desc = "";
                        var strunit = "Sample";
                        rtn.unit = item.unit.IndexOf(strunit) > -1 ? "演示机" : "商品机";
                        rtn.created_date = item.created_date;
                        rtn.province_name_so = item.province_name_so;
                        rtn.dealer_name = item.dealer_name;
                        rtn.dealer_name2 = item.dealer_name2;
                        rtn.dealer_name3 = item.dealer_name3;
                        if (item.busi_type == "总部发货")
                        {
                            if (string.IsNullOrWhiteSpace(ComFunc.nvl(item.province_name_st)))//特殊情况 唯一dealer_code为空的情况
                            {
                                rtn.dealer_code = "";
                                rtn.province_name_st_desc = "中国";
                            }
                            else
                            {
                                rtn.dealer_code = compcoderesult;
                            }
                            list.Add(rtn);
                        }
                        else
                        {
                            var dealercode_ok = "";
                            if (isExist(rtn.prod_code, item.dealer_code, item.dealer_code2, item.dealer_code3, item.province_name_st, salemap, ref dealercode_ok))//dealercode_ok
                            {
                                rtn.dealer_code = dealercode_ok;
                                list.Add(rtn);
                            }
                            else
                            {
                                p3++;
                            }
                        }
                    }
                    else
                    {
                        p4++;
                        GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "prod_code为空排除数据ID:" + item.id + ",prod_code:" + item.prod_code);
                    }

                }
                foreach (dynamic listitem in list)
                {
                    up.SetValue("uid", listitem.id);
                    var table_name = !String.IsNullOrEmpty(ComFunc.nvl(listitem.dealer_code)) ? "t_sale_so" : "t_sale_so_empty";
                    up.SetValue("tabelname", table_name);
                    DB.NonQuery<SoUnit>(up, "updateinfo");
                }
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"执行删除更新:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;
                var l1 = (from t in list
                          where !String.IsNullOrEmpty(ComFunc.nvl(t.dealer_code))
                          select new
                          {
                              t.id,
                              t.dealer_code,
                              t.comp_code,
                              t.prod_code,
                              t.qty,
                              t.prod_code_hw,
                              t.dealer_code1,
                              t.dealer_code2,
                              t.dealer_code3,
                              t.busi_type,
                              t.province_name_st,
                              t.form_date,
                              t.modi_time,
                              t.province_name_st_desc,
                              t.created_date,
                              t.dealer_name,
                              t.unit,
                              t.province_name_so,
                              t.dealer_name2,
                              t.dealer_name3
                          }).ToList();
                DB.BulkInsert(up, "t_sale_so", l1);
                c1 += l1.Count;
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"执行bulkinsert-t_sale_so:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;
                var l2 = (from t in list
                          where String.IsNullOrEmpty(ComFunc.nvl(t.dealer_code))
                          select new
                          {
                              t.id,
                              t.dealer_code,
                              t.comp_code,
                              t.prod_code,
                              t.qty,
                              t.prod_code_hw,
                              t.dealer_code1,
                              t.dealer_code2,
                              t.dealer_code3,
                              t.busi_type,
                              t.province_name_st,
                              t.form_date,
                              t.modi_time,
                              t.province_name_st_desc,
                              t.unit,
                              t.created_date,
                              t.province_name_so,
                              t.dealer_name,
                              t.dealer_name2,
                              t.dealer_name3
                          }).ToList();
                DB.BulkInsert(up, "t_sale_so_empty", l2);
                c2 += l2.Count;
                tempcount += tempresult.QueryData<FrameDLRObject>().Count();
                list.Clear();
                up.ToPage++;
                tempresult.Clear();
                tempresult = DB.LamdaQueryByPage(up, temp, "id");
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"执行第{up.ToPage}页查询:{(DateTime.Now - dt).TotalMilliseconds}"); dt = DateTime.Now;
            } while (tempresult.QueryTable.RowLength > 0 && tempresult.CurrentPage == up.ToPage);
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, DateTime.Now.ToString() + "总count数:" + tempcount + "汇入t_sale_so:" + c1 + ",t_sale_so_empty:" + c2 + "笔资料,dealercode_ok为空排除数据ID" + p3 + ",prod_code为空排除掉数据" + p4);

        }
        #endregion 

        #region isExist 
        public bool isExist(string prod_code, string dealer_code, string dealer_code2, string dealer_code3, string state, Dictionary<string, string> salemap, ref string dealercode_ok)
        {
            bool flag = false;
            //一、用dealer_code，dealer_code2，dealer_code3分别去“主辅省包-平台对应关系”匹配，找到辅省包由记该省包SO，没找到继续下一步
            //二、用dealer_code，dealer_code2，dealer_code3分别去“主辅省包-平台对应关系”匹配，找到主省包由记该省包SO，没找到继续下一步
            //三、以上两步都没找到匹配的，则用省份名称去“主辅省包-平台对应关系”找主省包，记该省包SO
            var key11 = prod_code + "|2|" + dealer_code;
            var key12 = prod_code + "|2|" + dealer_code2;
            var key13 = prod_code + "|2|" + dealer_code3;
            var key21 = prod_code + "|1|" + dealer_code;
            var key22 = prod_code + "|1|" + dealer_code2;
            var key23 = prod_code + "|1|" + dealer_code3;
            var key3 = prod_code + state;

            var key4 = prod_code + dealer_code;
            var key5 = prod_code + dealer_code2;
            var key6 = prod_code + dealer_code3;

            if (salemap.ContainsKey(key11) && salemap[key11].StartsWith("R"))
            {
                dealercode_ok = salemap[key11];
                flag = true;
            }
            else if (salemap.ContainsKey(key12) && salemap[key12].StartsWith("R"))
            {
                dealercode_ok = salemap[key12];
                flag = true;
            }
            else if (salemap.ContainsKey(key13) && salemap[key13].StartsWith("R"))
            {
                dealercode_ok = salemap[key13];
                flag = true;
            }
            else if (salemap.ContainsKey(key4) && salemap[key4].StartsWith("R"))
            {
                dealercode_ok = salemap[key4];
                flag = true;
            }
            else if (salemap.ContainsKey(key5) && salemap[key5].StartsWith("R"))
            {
                dealercode_ok = salemap[key5];
                flag = true;
            }
            else if (salemap.ContainsKey(key6) && salemap[key6].StartsWith("R"))
            {
                dealercode_ok = salemap[key6];
                flag = true;
            }
            else if (salemap.ContainsKey(key21) && salemap[key21].StartsWith("R"))
            {
                dealercode_ok = salemap[key21];
                flag = true;
            }
            else if (salemap.ContainsKey(key22) && salemap[key22].StartsWith("R"))
            {
                dealercode_ok = salemap[key22];
                flag = true;
            }
            else if (salemap.ContainsKey(key23) && salemap[key23].StartsWith("R"))
            {
                dealercode_ok = salemap[key23];
                flag = true;
            }
            else if (salemap.ContainsKey(key3))
            {
                dealercode_ok = salemap[key3];
                flag = true;
            }
            else
            {
                dealercode_ok = "";
                flag = false;
            }
            return flag;
        }
        #endregion
    }
}
