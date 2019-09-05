using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Data.Base;
using System.Linq;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Common;

namespace BatchApplication.Business
{
    class SO:MyConsoleLogic
    {
        void load(dynamic args)
        {
            var uppsql = DB.NewPSQLDBUnitParameter();
            var up = DB.NewDBUnitParameter();

            

            var s2 = from t in DB.LamdaTable("common_sale_proxy", "a")
                     select new
                     {
                         t.prod_code,
                         t.state,
                         t.dealer_type,
                         t.dealer_code
                     };
            var s3 = from t in DB.LamdaTable("common_hw_prod_map", "a")
                     select new
                     {
                         t.prod_code,
                         t.prod_code_hw
                     };

            var salemapresult = DB.ExcuteLamda(up, s2);
            var prodmapresult = DB.ExcuteLamda(up, s3);

            var salemaplist = (from t in salemapresult.QueryData<FrameDLRObject>()
                           select new
                           {
                               key = t.prod_code + "|" + t.dealer_type + "|" + t.dealer_code,
                               value = t.dealer_code
                           }).ToList();
            salemaplist.AddRange(from t in salemapresult.QueryData<FrameDLRObject>()
                                 select new
                                 {
                                     key = t.prod_code + "|" + t.state.Replace("省", "").Replace("市", "").Replace("自治区", ""),
                                     value = t.dealer_code
                                 });
            salemaplist.AddRange(from t in salemapresult.QueryData<FrameDLRObject>()
                                 select new
                                 {
                                     key = t.prod_code + "|" + t.plateform_code,
                                     value = t.dealer_code
                                 });
            var salemap = salemaplist.ToDictionary<string, string>(k => k.key, v => v.value);
            var prodmap = (from t in prodmapresult.QueryData<FrameDLRObject>()
                          select new
                          {
                              key = t.prod_code_hw,
                              value = t.prod_code
                          }).ToDictionary<string,string>(k=>k.key,v=>v.value);


            var smax = (from t in DB.PSQLLamdaTable("t_sale_so", "a")
                       select t.modi_time).Max("a.modi_time");
            var maxresult = DB.ExcuteLamda(up, smax);
            var max_modi_time = maxresult.QueryTable.RowLength > 0 ? ComFunc.nvl(maxresult.QueryTable[0, 0]) : "";

            var s1 = from t in DB.PSQLLamdaTable("t_terminal_so_line", "a")
                     where t.modi_time != null && t.province_name_so != "" && t.notnull(max_modi_time, t.append(t.modi_time > max_modi_time, "::timestamp"))
                     select new
                     {
                         t.id,
                         t.comp_code,
                         t.prod_code,
                         t.qty,
                         t.form_date,
                         t.modi_time,
                         type = t.busi_type,
                         state = t.province_name_so,
                         t.dealer_code,
                         t.dealer_code2,
                         t.dealer_code3
                     };
            uppsql.Count_Of_OnePage = 10000;
            uppsql.ToPage = 1;
            var result = DB.LamdaQueryByPage(uppsql, s1, "id");
            do
            {
                var list = result.QueryData<FrameDLRObject>().Select(d =>
                {
                    dynamic rtn =  FrameDLRObject.CreateInstance();
                    //采用bulkinsert，必须保证栏位顺序与db中的一致才行
                    rtn.id = d.id;
                    rtn.dealer_code = "";
                    rtn.comp_code = d.comp_code;
                    rtn.prod_code = prodmap.ContainsKey(d.prod_code) ? prodmap[d.prod_code] : "";
                    rtn.qty = d.qty;
                    rtn.form_date = d.form_date;
                    rtn.modi_time = DateTimeStd.IsDateTimeThen(d.modi_time,"yyyy-MM-dd HH:mm:ss");
                    rtn.type = null;
                    rtn.state = d.state;

                    if (d.type == "总部发货")
                    {
                        rtn.type = 1;
                    }
                    else if (d.type == "本省发货")
                    {
                        rtn.type = 2;
                    }
                    else
                    {
                        rtn.type = null;
                    }
                    var dealercode_ok = "";
                    //一、用dealer_code，dealer_code2，dealer_code3分别去“主辅省包-平台对应关系”匹配，找到辅省包由记该省包SO，没找到继续下一步
                    //二、用dealer_code，dealer_code2，dealer_code3分别去“主辅省包-平台对应关系”匹配，找到主省包由记该省包SO，没找到继续下一步
                    //三、以上两步都没找到匹配的，则用省份名称去“主辅省包-平台对应关系”找主省包，记该省包SO
                    var key11 = rtn.prod_code + "|2|" + d.dealer_code;
                    var key12 = rtn.prod_code + "|2|" + d.dealer_code2;
                    var key13 = rtn.prod_code + "|2|" + d.dealer_code3;
                    var key21 = rtn.prod_code + "|1|" + d.dealer_code;
                    var key22 = rtn.prod_code + "|1|" + d.dealer_code2;
                    var key23 = rtn.prod_code + "|1|" + d.dealer_code3;
                    var key3 = rtn.prod_code + "|" + d.state;

                    if (salemap.ContainsKey(key11) && salemap[key11].StartsWith("R"))
                    {
                        dealercode_ok = salemap[key11];
                    }
                    else if (salemap.ContainsKey(key12) && salemap[key12].StartsWith("R"))
                    {
                        dealercode_ok = salemap[key12];
                    }
                    else if (salemap.ContainsKey(key13) && salemap[key13].StartsWith("R"))
                    {
                        dealercode_ok = salemap[key13];
                    }
                    else if (salemap.ContainsKey(key21) && salemap[key21].StartsWith("R"))
                    {
                        dealercode_ok = salemap[key21];
                    }
                    else if (salemap.ContainsKey(key22) && salemap[key22].StartsWith("R"))
                    {
                        dealercode_ok = salemap[key22];
                    }
                    else if (salemap.ContainsKey(key23) && salemap[key23].StartsWith("R"))
                    {
                        dealercode_ok = salemap[key23];
                    }
                    else if (salemap.ContainsKey(key3))
                    {
                        dealercode_ok = salemap[key3];
                    }
                    else
                    {
                        dealercode_ok = "";
                    }
                    rtn.dealer_code = dealercode_ok;

                    return rtn;
                }).ToList();

                DB.BulkInsert(up, "t_sale_so", list);
                //foreach (dynamic item in list)
                //{
                //    DB.QuickInsertNotExists(up, "t_sale_so", item, new
                //    {
                //        id = item.id
                //    });

                //}

                list.Clear();
                uppsql.ToPage++;
                result.Clear();
                result = DB.LamdaQueryByPage(uppsql, s1, "id");
            } while (result.QueryTable.RowLength > 0 && result.CurrentPage == uppsql.ToPage);
        }
    }
}
