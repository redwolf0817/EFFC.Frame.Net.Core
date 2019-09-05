using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using EFFC.Frame.Net.Unit.DB.Parameters;

namespace RestAPISample.Business.v1
{
    public class WorkFlowMgr:MyRestLogic
    {
        /// <summary>
        /// 获取工作流详细信息
        /// </summary>
        /// <returns></returns>
        [EWRAAuth(true)]
        [EWRARoute("get", "/WorkFlowMgr/getworkflow")]
        [EWRAAddInput("Code", "string", "工作流编码", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRARouteDesc("获取指定流程编码的流程完整信息")]
        [EWRAOutputDesc("返回结果", @"{
                     Base:{Code：工作流编码,Title:工作流名称,Type:工作流类型,Status:工作流状态默认为1,Remark:备注,CurWFID:当前工作流Master表中的WFID,AppCode:工作流应用编码},
                     Masters:[
                        { 
                            Master:{WFID:工作流版本ID,Code:工作流编码，Version：工作流版本号,Remark:备注,JsonData：工作流版本流程设计器JsonData},
                            Nodes:[{
                               Node:{NodeID:节点IDGUID，WFID:工作流版本ID-GUID,Title:节点名称,Type:节点类型1开始节点2普通节点3结束节点},
                               Actions:[{ID:行为ID-GUID,Code:行为编码,Title:行为名称,NodeID:节点ID-GUID，Type：行为类型0-不影响流程1-影响流程}],
                               Attributes:[{ID:属性ID-GUID,Name:属性名称,Value:属性值,NodeID:节点ID-GUID}],
                               UserRules:[{ID:规则ID-GUID,DepCode:部门编码,TitleCode:角色编码,NodeID:节点ID-GUID，Type：行为类型1-并集2-交集}]
                            }],
                            Lines:[{ID:线路ID-GUID,WFID:流程版本ID,ActionCode:行为编码,ExtendData:扩展属性，StartNodeID：开始节点ID,EndNodeID：结束节点ID}]
                        }]  }")]
        public  object GetWorkflow()
        {
            SetCacheEnable(false);
            var wfCode = PostDataD.Code;
            var up = DB.NewDBUnitParameter();
            ///load base info
            var wfBase = from b in DB.LamdaTable(up, "WorkFlowBase", "b")
                         where b.Code == wfCode
                         select b;

            ///Load masters
            var masters = (from m in DB.LamdaTable(up, "WorkFlowMaster", "m")
                           where m.Code == wfCode
                           select m).GetQueryList(up);
            ///Load nodes
            var nodes = (from n in DB.LamdaTable(up, "WorkFlowNode", "n")
                         join m in DB.LamdaTable(up, "WorkFlowMaster", "m") on n.WFID equals m.WFID
                         where m.Code == wfCode
                         select n).GetQueryList(up);

            //load node attributes
            var attributes = (from a in DB.LamdaTable(up, "WorkFlowNodeAttribute", "a")
                              join n in DB.LamdaTable(up, "WorkFlowNode", "n") on a.NodeID equals n.NodeID
                              join m in DB.LamdaTable(up, "WorkFlowMaster", "m") on n.WFID equals m.WFID
                              where m.Code == wfCode
                              select a).GetQueryList(up);
            //load node actions
            var actions = (from a in DB.LamdaTable(up, "WorkFlowNodeAction", "a")
                           join n in DB.LamdaTable(up, "WorkFlowNode", "n") on a.NodeID equals n.NodeID
                           join m in DB.LamdaTable(up, "WorkFlowMaster", "m") on n.WFID equals m.WFID
                           where m.Code == wfCode
                           select a).GetQueryList(up);
            ///Load lines
            var lines = (from l in DB.LamdaTable(up, "WorkFlowLine", "l")
                         join m in DB.LamdaTable(up, "WorkFlowMaster", "m") on l.WFID equals m.WFID
                         where m.Code == wfCode
                         select l).GetQueryList(up);


            //return structure data
            return (from t in wfBase.GetQueryList(up)
                    select new
                    {
                        //流程基础表数据
                        Base = new
                        {
                            Code = t.Code,
                            Title = t.Title,
                            Type = t.Type,
                            Status = t.Status,
                            Remark = t.Remark,
                            CurWFID = t.CurWFID
                        },
                        //流程版本表列表
                        Masters = (from m in masters
                                   select new
                                   {
                                       //流程版本基础数据
                                       Master = new
                                       {
                                           WFID = m.WFID,
                                           Code = m.Code,
                                           Version = m.Version,
                                           Remark = m.Remark,
                                           JsonData = m.JsonData
                                       },
                                       //流程版本节点数据列表
                                       Nodes = (from n in nodes
                                                where n.WFID == m.WFID
                                                select new
                                                {
                                                    //流程版本节点数据信息
                                                    Node = new
                                                    {
                                                        NodeID = n.NodeID,
                                                        WFID = n.WFID,
                                                        Title = n.Title,
                                                        Type = n.Type
                                                    },
                                                    //流程版本节点行为数据
                                                    Actions = (from a in actions where a.NodeID == n.NodeID select a).ToList(),
                                                    //流程版本节点属性数据
                                                    Attributes = (from a in attributes where a.NodeID == n.NodeID select a).ToList()
                                                }).ToList(),
                                       //流程版线路数据
                                       Lines = (from l in lines where l.WFID == m.WFID select l).ToList()
                                   }).ToList()
                    });
        }

        /// <summary>
        /// 获取工作流版本详细信息
        /// </summary>
        /// <returns></returns>
        [EWRAAuth(true)]
        [EWRARoute("get", "/WorkFlowMgr/getworkflowmaster")]
        [EWRAAddInput("WFID", "string", "工作流版本ID", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRARouteDesc("获取指定流程编码的流程完整信息")]
        [EWRAOutputDesc("返回结果", @" { 
                            Master:{WFID:工作流版本ID,Code:工作流编码，Version：工作流版本号,Remark:备注,JsonData：工作流版本流程设计器JsonData},
                            Nodes:[{
                               Node:{NodeID:节点IDGUID，WFID:工作流版本ID-GUID,Title:节点名称,Type:节点类型1开始节点2普通节点3结束节点},
                               Actions:[{ID:行为ID-GUID,Code:行为编码,Title:行为名称,NodeID:节点ID-GUID，Type：行为类型0-不影响流程1-影响流程}],
                               Attributes:[{ID:属性ID-GUID,Name:属性名称,Value:属性值,NodeID:节点ID-GUID}],
                               UserRules:[{ID:规则ID-GUID,DepCode:部门编码,TitleCode:角色编码,NodeID:节点ID-GUID，Type：行为类型1-并集2-交集}]
                            }],
                            Lines:[{ID:线路ID-GUID,WFID:流程版本ID,ActionCode:行为编码,ExtendData:扩展属性，StartNodeID：开始节点ID,EndNodeID：结束节点ID}]
                        }")]
        public object GetWorkflowMaster()
        {
            SetCacheEnable(false);
            var WFID = PostDataD.WFID;
            var up = DB.NewDBUnitParameter();

            return GetWorkflowMaster(up, WFID);
        }

        /// <summary>
        /// 获取工作流版本所有line&node&actions&Attributes&UserRules 数据
        /// </summary>
        /// <param name="up"></param>
        /// <param name="master"></param>
        protected object GetWorkflowMaster(UnitParameter up, string WFID)
        {

            var master = (from m in DB.LamdaTable(up, "WorkFlowMaster", "m")
                          where m.WFID == WFID
                          select m).GetQueryList(up).FirstOrDefault();

            ///Load nodes
            var nodes = (from n in DB.LamdaTable(up, "WorkFlowNode", "n")
                         where n.WFID == master.GetValue("WFID")
                         select n).GetQueryList(up);

            //load node attributes
            var attributes = (from a in DB.LamdaTable(up, "WorkFlowNodeAttribute", "a")
                              join n in DB.LamdaTable(up, "WorkFlowNode", "n") on a.NodeID equals n.NodeID
                              where n.WFID == master.GetValue("WFID")
                              select a).GetQueryList(up);
            //load node actions
            var actions = (from a in DB.LamdaTable(up, "WorkFlowNodeAction", "a")
                           join n in DB.LamdaTable(up, "WorkFlowNode", "n") on a.NodeID equals n.NodeID
                           where n.WFID == master.GetValue("WFID")
                           select a).GetQueryList(up);
            ///Load lines
            var lines = (from l in DB.LamdaTable(up, "WorkFlowLine", "l")
                         where l.WFID == master.GetValue("WFID")
                         select l).GetQueryList(up);


            //return structure data
            return new
            {
                //流程版本基础数据
                Master = new
                {
                    WFID = master.GetValue("WFID"),
                    Code = master.GetValue("Code"),
                    Version = master.GetValue("Version"),
                    Remark = master.GetValue("Remark"),
                    JsonData = master.GetValue("JsonData")
                },
                //流程版本节点数据列表
                Nodes = (from n in nodes
                         where n.WFID == master.GetValue("WFID")
                         select new
                         {
                             //流程版本节点数据信息
                             Node = new
                             {
                                 NodeID = n.NodeID,
                                 WFID = n.WFID,
                                 Title = n.Title,
                                 Type = n.Type
                             },
                             //流程版本节点行为数据
                             Actions = (from a in actions where a.NodeID == n.NodeID select a).ToList(),
                             //流程版本节点属性数据
                             Attributes = (from a in attributes where a.NodeID == n.NodeID select a).ToList()
                         }).ToList(),
                //流程版线路数据
                Lines = (from l in lines where l.WFID == WFID select l).ToList()
            };
        }

        /// <summary>
        /// 获取工作流列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EWRAAuth(true)]
        [EWRARoute("get", "/WorkFlowMgr/WorkflowBases")]
        [EWRARouteDesc("获取流程列表信息")]
        [EWRAOutputDesc("返回结果", @"[
           {
               Code:流程编码,
               Title:名称,
               Type:类型,
               Status:状态,
               Remark:备注,
               CurWFID:当前版本ID,
               CurVersion:当前版本号,
               AppCode:应用编码,
               AppName:应用名称
           }
        ]")]
        public object WorkflowBases()
        {
            SetCacheEnable(false); 
            var up = DB.NewDBUnitParameter();
            var w = from b in DB.LamdaTable(up, "WorkFlowBase", "b")
                    join a in DB.LamdaTable(up,"Applications","a") on b.AppCode equals a.Code 
                    select new
                    {
                        Code=b.Code,
                        Title = b.Title,
                        Type = b.Type,
                        Status = b.Status,
                        Remark = b.Remark,
                        CurWFID = b.CurWFID,
                        AppCode = b.AppCode,
                        AppName = a.Title
                    };
           
            return new
            {
                code = "success",
                msg = "工作流列表信息",
                data = w.GetQueryList(up)
            };
        }

        /// <summary>
        /// 创建工作流
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EWRARoute("post", "/WorkFlowMgr/CreateWorkflowBase")]
        [EWRAAddInput("Base", "object", "工作流编码", "{Code：工作流编码,Title:工作流名称,Type:工作流类型,Status:工作流状态默认为1,Remark:备注,CurWFID:当前工作流Master表中的WFID,AppCode:工作流应用编码}", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRARouteDesc("新建工作流基础信息WorkFlowBase")]
        [EWRAOutputDesc("返回结果", @"{
                code = 'success',
                msg = '工作流创建成功'
            }")]
        public object CreateWorkflowBase()
        {
            SetCacheEnable(false);
            FrameDLRObject wfBase = PostDataD.Base;
            var up = DB.NewDBUnitParameter();
            var w = from t in DB.LamdaTable(up, "WorkFlowBase", "w")
                    where t.Code == wfBase.GetValue("Code").ToString()
                    select t;
            if (!w.IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "工作流编码已存在"
                };
            }
            w.Insert(up, wfBase);
            return new
            {
                code = "success",
                msg = "工作流创建成功"
            };
        }

        /// <summary>
        /// 更新工作流
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EWRARoute("post", "/WorkFlowMgr/UpdateWorkflowBase")]
        [EWRAAddInput("Base", "object", "工作流编码", "{Code：工作流编码,Title:工作流名称,Type:工作流类型,Status:工作流状态默认为1,Remark:备注,CurWFID:当前工作流Master表中的WFID,AppCode:工作流应用编码}", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRARouteDesc("更新工作流基础信息WorkFlowBase")]
        [EWRAOutputDesc("返回结果", @"")]
        public object UpdateWorkflowBase()
        {
            SetCacheEnable(false);
            FrameDLRObject wfBase = PostDataD.Base;
            var up = DB.NewDBUnitParameter();
            var w = from t in DB.LamdaTable(up, "WorkFlowBase", "w")
                    where t.Code == wfBase.GetValue("Code").ToString()
                    select t;
            if (!w.IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "工作流编码不存在"
                };
            }
            this.DB.QuickUpdate(up, "WorkFlowBase", new { Title = wfBase.GetValue("Title"), Type = wfBase.GetValue("Type"), Status = wfBase.GetValue("Status"), Remark = wfBase.GetValue("Remark"), CurWFID = wfBase.GetValue("CurWFID"), AppCode = wfBase.GetValue("AppCode") },
                              new { Code = wfBase.GetValue("Code") });
        
            return new
            {
                code = "success",
                msg = "工作流更新成功"
            };
        }

        /// <summary>
        /// 创建工作流版本
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EWRARoute("post", "/WorkFlowMgr/CreateWorkflowMaster")]
        [EWRAAddInput("Master", "object", "工作流编码", @"{ 
                            Master:{WFID: 工作流版本ID, Code: 工作流编码，Version：工作流版本号, Remark: 备注, JsonData：工作流版本流程设计器JsonData},
                            Nodes:[{
                               Node:{NodeID:节点IDGUID，WFID:工作流版本ID-GUID, Title:节点名称, Type:节点类型1开始节点2普通节点3结束节点},
                               Actions:[{ID:行为ID-GUID, Code:行为编码, Title:行为名称, NodeID:节点ID-GUID，Type：行为类型0-不影响流程1-影响流程}],
                               Attributes:[{ID:属性ID-GUID, Name:属性名称, Value:属性值, NodeID:节点ID-GUID}],
                               UserRules:[{ID:规则ID-GUID, DepCode:部门编码, TitleCode:角色编码, NodeID:节点ID-GUID，Type：行为类型1-并集2-交集}]
                            }],
                            Lines:[{ID:线路ID-GUID, WFID:流程版本ID, ActionCode:行为编码, ExtendData:扩展属性，StartNodeID：开始节点ID, EndNodeID：结束节点ID}]
                        }", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRARouteDesc("新建工作流版本信息WorkFlowMaster")]
        [EWRAOutputDesc("返回结果", @"{
                code = 'success',
                msg ='工作流版本创建成功'
            }")]
        public object CreateWorkflowMaster()
        {
            SetCacheEnable(false);
            var wfMaster = PostDataD.Master;
            var up = DB.NewDBUnitParameter();
            var w = from t in DB.LamdaTable(up, "WorkFlowBase", "w")
                    where t.Code == wfMaster.GetValue("Code").ToString()
                    select t;
            if (!w.IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "工作流基础信息未被创建"
                };
            }

            var masterList = (from m in DB.LamdaTable(up, "WorkFlowMaster", "m")
                              orderby m.Version descending
                              where m.Code == wfMaster.GetValue("Code").ToString()
                              select m.Version).GetQueryList(up);
            var masterVMax = masterList.Count > 0 ? int.Parse(masterList[0].ToString()) : 0;
            var master = wfMaster.GetValue("Master") as FrameDLRObject;
            if (master != null)
            {
                master.SetValue("WFID", Guid.NewGuid());
                master.SetValue("Version", masterVMax + 1);
                DB.QuickInsertNotExists(up, "WorkFlowMaster", master, new { WFID = master.GetValue("WFID") });
            }

            var nodes = wfMaster.GetValue("Nodes") as List<FrameDLRObject>;
            if (nodes != null && nodes.Count > 0)
            {
                foreach (var node in nodes)
                {
                    var nobj = node.GetValue("Node") as FrameDLRObject;
                    nobj.SetValue("NodeID", Guid.NewGuid());
                    nobj.SetValue("WFID", master.GetValue("WFID"));
                    DB.QuickInsertNotExists(up, "WorkFlowNode", nobj, new { NodeID = nobj.GetValue("NodeID") });
                    var attrObjs = node.GetValue("Attributes") as List<FrameDLRObject>;
                    foreach (var attr in attrObjs)
                    {
                        attr.SetValue("ID", Guid.NewGuid());
                        DB.QuickInsertNotExists(up, "WorkFlowNodeAttribute", attr, new { ID = attr.GetValue("ID") });
                    }
                    var actObjs = node.GetValue("Actions") as List<FrameDLRObject>;
                    foreach (var act in actObjs)
                    {
                        act.SetValue("ID", Guid.NewGuid());
                        DB.QuickInsertNotExists(up, "WorkFlowNodeAction", act, new { ID = act.GetValue("ID") });
                    }
                    var userRuleObjs = node.GetValue("UserRules") as List<FrameDLRObject>;
                    foreach (var rule in userRuleObjs)
                    {
                        rule.SetValue("ID", Guid.NewGuid());
                        DB.QuickInsertNotExists(up, "WorkFlowNodeUserRule", rule, new { ID = rule.GetValue("ID") });
                    }
                }
            }
            var lines = wfMaster.GetValue("Lines") as List<FrameDLRObject>;
            if (lines != null && lines.Count > 0)
            {
                foreach (var line in lines)
                {
                    line.SetValue("ID", Guid.NewGuid());
                    DB.QuickInsertNotExists(up, "WorkFlowLine", line, new { ID = line.GetValue("ID") });
                }
            }
            return new
            {
                code = "success",
                msg = "工作流版本创建成功"
            };
        }

        /// <summary>
        /// 创建工作流版本
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EWRARoute("post", "/WorkFlowMgr/UpdateWorkflowMaster")]
        [EWRAAddInput("Master", "object", "工作流编码", @"{ 
                            Master:{WFID: 工作流版本ID, Code: 工作流编码，Version：工作流版本号, Remark: 备注, JsonData：工作流版本流程设计器JsonData},
                            Nodes:[{
                               Node:{NodeID:节点IDGUID，WFID:工作流版本ID-GUID, Title:节点名称, Type:节点类型1开始节点2普通节点3结束节点},
                               Actions:[{ID:行为ID-GUID, Code:行为编码, Title:行为名称, NodeID:节点ID-GUID，Type：行为类型0-不影响流程1-影响流程}],
                               Attributes:[{ID:属性ID-GUID, Name:属性名称, Value:属性值, NodeID:节点ID-GUID}],
                               UserRules:[{ID:规则ID-GUID, DepCode:部门编码, TitleCode:角色编码, NodeID:节点ID-GUID，Type：行为类型1-并集2-交集}]
                            }],
                            Lines:[{ID:线路ID-GUID, WFID:流程版本ID, ActionCode:行为编码, ExtendData:扩展属性，StartNodeID：开始节点ID, EndNodeID：结束节点ID}]
                        }", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRARouteDesc("更新工作流版本信息WorkFlowMaster")]
        [EWRAOutputDesc("返回结果", @"{
                code = 'success',
                msg ='工作流版本更新成功'
            }")]
        public object UpdateWorkflowMaster()
        {
            SetCacheEnable(false);
            BeginTrans();
            var wfMaster = PostDataD.Master;
            var up = DB.NewDBUnitParameter();
            var w = from t in DB.LamdaTable(up, "WorkFlowBase", "w")
                    where t.Code == wfMaster.GetValue("Code").ToString()
                    select t;
            if (!w.IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = wfMaster.GetValue("Code").ToString()+ "工作流不存在"
                };
            }

            var master = wfMaster.GetValue("Master") as FrameDLRObject;
            var m = from t in DB.LamdaTable(up, "WorkFlowMaster", "w")
                    where t.WFID == wfMaster.GetValue("WFID").ToString()
                    select t;
            if (!m.IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "工作流当前版本不存在"
                };
            } else
            {
                var status = m.GetQueryList(up).FirstOrDefault().GetValue("Status");
                if (status.ToString() == "1")
                {
                    return new
                    {
                        code = "failed",
                        msg = "已发布版本不能更新"
                    };
                }
            }
            var fullMaster = GetWorkflowMaster(up, master.GetValue("WFID").ToString());
            ClearWorkflowMaster(up, fullMaster as FrameDLRObject);

            var nodes = wfMaster.GetValue("Nodes") as List<FrameDLRObject>;
            if (nodes != null && nodes.Count > 0)
            {
                foreach (var node in nodes)
                {
                    var nobj = node.GetValue("Node") as FrameDLRObject;
                    nobj.SetValue("NodeID", Guid.NewGuid());
                    nobj.SetValue("WFID", master.GetValue("WFID"));
                    DB.QuickInsertNotExists(up, "WorkFlowNode", nobj, new { NodeID = nobj.GetValue("NodeID") });
                    var attrObjs = node.GetValue("Attributes") as List<FrameDLRObject>;
                    foreach (var attr in attrObjs)
                    {
                        attr.SetValue("ID", Guid.NewGuid());
                        DB.QuickInsertNotExists(up, "WorkFlowNodeAttribute", attr, new { ID = attr.GetValue("ID") });
                    }
                    var actObjs = node.GetValue("Actions") as List<FrameDLRObject>;
                    foreach (var act in actObjs)
                    {
                        act.SetValue("ID", Guid.NewGuid());
                        DB.QuickInsertNotExists(up, "WorkFlowNodeAction", act, new { ID = act.GetValue("ID") });
                    }
                    var userRuleObjs = node.GetValue("UserRules") as List<FrameDLRObject>;
                    foreach (var rule in userRuleObjs)
                    {
                        rule.SetValue("ID", Guid.NewGuid());
                        DB.QuickInsertNotExists(up, "WorkFlowNodeUserRule", rule, new { ID = rule.GetValue("ID") });
                    }
                }
            }
            var lines = wfMaster.GetValue("Lines") as List<FrameDLRObject>;
            if (lines != null && lines.Count > 0)
            {
                foreach (var line in lines)
                {
                    line.SetValue("ID", Guid.NewGuid());
                    DB.QuickInsertNotExists(up, "WorkFlowLine", line, new { ID = line.GetValue("ID") });
                }
            }
            CommitTrans();
            return new
            {
                code = "success",
                msg = "工作流版本更新成功"
            };
        }

        /// <summary>
        /// 未发布的工作流版本更新时先清除所有line&node&actions&Attributes&UserRules 数据
        /// </summary>
        /// <param name="up"></param>
        /// <param name="master"></param>
        protected void ClearWorkflowMaster(UnitParameter up,FrameDLRObject master)
        {
          
            var nodes = master.GetValue("Nodes") as List<FrameDLRObject>;
            if (nodes != null && nodes.Count > 0)
            {
                foreach (var node in nodes)
                { 
                    DB.QuickDelete(up, "WorkFlowNode", new { NodeID = node.GetValue("NodeID") }); 
                    var attrObjs = node.GetValue("Attributes") as List<FrameDLRObject>;
                    foreach (var attr in attrObjs)
                    {
                        DB.QuickDelete(up, "WorkFlowNodeAttribute", new { ID = attr.GetValue("ID") }); 
                    }
                    var actObjs = node.GetValue("Actions") as List<FrameDLRObject>;
                    foreach (var act in actObjs)
                    {
                        DB.QuickDelete(up, "WorkFlowNodeAction", new { ID = act.GetValue("ID") }); 
                    }
                    var userRuleObjs = node.GetValue("UserRules") as List<FrameDLRObject>;
                    foreach (var rule in userRuleObjs)
                    {
                        DB.QuickDelete(up, "WorkFlowNodeUserRule", new { ID = rule.GetValue("ID") }); 
                    }
                }
            }
            var lines = master.GetValue("Lines") as List<FrameDLRObject>;
            if (lines != null && lines.Count > 0)
            {
                foreach (var line in lines)
                {
                    DB.QuickDelete(up, "WorkFlowLine", new { ID = line.GetValue("ID") }); 
                }
            } 
        }

        /// <summary>
        /// 创建工作流版本
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EWRARoute("post", "/WorkFlowMgr/DeleteWorkflowMaster")]
        [EWRAAddInput("WFID", "string", "工作流编码", @"", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRARouteDesc("删除工作流版本信息WorkFlowMaster")]
        [EWRAOutputDesc("返回结果", @"{
                code = 'success',
                msg ='工作流版本删除成功'
            }")]
        public object DeleteWorkflowMaster()
        {
            SetCacheEnable(false);
            var wfid = PostDataD.WFID;
            var up = DB.NewDBUnitParameter();
 
             
            var m = from t in DB.LamdaTable(up, "WorkFlowMaster", "w")
                    where t.WFID == wfid
                    select t;
            if (!m.IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "工作流当前版本不存在"
                };
            }
            else
            {
                var status = m.GetQueryList(up).FirstOrDefault().GetValue("Status");
                if (status.ToString() == "1")
                {
                    return new
                    {
                        code = "failed",
                        msg = "已发布版本不能删除"
                    };
                }
            }
            var fullMaster = GetWorkflowMaster(up, wfid.ToString());
            ClearWorkflowMaster(up, fullMaster as FrameDLRObject);

            DB.QuickDelete(up, "WorkFlowMaster", new { WFID = wfid });

            return new
            {
                code = "success",
                msg = "工作流版本删除成功"
            };
        }




        /// <summary>
        /// 获取应用列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EWRAAuth(true)]
        [EWRARoute("get", "/WorkFlowMgr/Applications")]
        [EWRARouteDesc("获取应用列表信息")]
        [EWRAOutputDesc("返回结果", @"[
           {
               Code:流程编码,
               Title:名称,
               Type:类型,
               Status:状态,
               Remark:备注,
               CurWFID:当前版本ID,
               CurVersion:当前版本号,
               AppCode:应用编码,
               AppName:应用名称
           }
        ]")]
        public object Applications()
        {
            SetCacheEnable(false);
            var up = DB.NewDBUnitParameter();
            var w = from b in DB.LamdaTable(up, "WorkFlowBase", "b")
                    join a in DB.LamdaTable(up, "Applications", "a") on b.AppCode equals a.Code
                    select new
                    {
                        Code = b.Code,
                        Title = b.Title,
                        Type = b.Type,
                        Status = b.Status,
                        Remark = b.Remark,
                        CurWFID = b.CurWFID,
                        AppCode = b.AppCode,
                        AppName = a.Title
                    };

            return new
            {
                code = "success",
                msg = "工作流列表信息",
                data = w.GetQueryList(up)
            };
        }

        /// <summary>
        /// 创建应用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EWRARoute("post", "/WorkFlowMgr/CreateApplication")]
        [EWRAAddInput("Application", "object", "应用", "{Code：应用编码,Title:应用名称,Token:访问token,Status:应用状态1启用0停用,Remark:备注}", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRARouteDesc("新建应用Application")]
        [EWRAOutputDesc("返回结果", @"{
                code = 'success',
                msg = '应用创建成功'
            }")]
        public object CreateApplication()
        {
            SetCacheEnable(false);
            FrameDLRObject app = PostDataD.Application;
            var up = DB.NewDBUnitParameter();
            var w = from t in DB.LamdaTable(up, "Applications", "w")
                    where t.Code == app.GetValue("Code").ToString()
                    select t;
            if (!w.IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "应用编码已存在"
                };
            }
            w.Insert(up, app);
            return new
            {
                code = "success",
                msg = "应用创建成功"
            };
        }

        /// <summary>
        /// 更新应用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [EWRARoute("post", "/WorkFlowMgr/UpdateApplication")]
        [EWRAAddInput("Application", "object", "应用", "{Code：应用编码,Title:应用名称,Token:访问token,Status:应用状态1启用0停用,Remark:备注}", EFFC.Frame.Net.Base.Constants.RestInputPosition.PostData, false)]
        [EWRARouteDesc("更新应用")]
        [EWRAOutputDesc("返回结果", @"{
                code = 'success',
                msg = '应用更新成功'
            }")]
        public object UpdateApplication()
        {
            SetCacheEnable(false);
            FrameDLRObject app = PostDataD.Application;
            var up = DB.NewDBUnitParameter();
            var w = from t in DB.LamdaTable(up, "Applications", "w")
                    where t.Code == app.GetValue("Code").ToString()
                    select t;
            if (!w.IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "应用编码不存在"
                };
            }
            this.DB.QuickUpdate(up, "Applications", new { Title = app.GetValue("Title"),  Status = app.GetValue("Status"), Remark = app.GetValue("Remark") },
                              new { Code = app.GetValue("Code") });

            return new
            {
                code = "success",
                msg = "应用更新成功"
            };
        }
    }
}
