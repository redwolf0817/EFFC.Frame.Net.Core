using EFFC.Frame.Net.Base.Module;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Business.Parameters;
using EFFC.Frame.Net.Base.Common;
using System.Reflection;
using EFFC.Frame.Net.Module.Business.Logic;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Module.Business.Datas;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Business.Options;

namespace EFFC.Frame.Net.Module.Business
{
    /// <summary>
    /// 业务处理模块的基类，动态加载对应的logic并执行之
    /// </summary>
    /// <typeparam name="LType"></typeparam>
    public abstract class BusiModule<LType,PType,DType> : BaseBusiModule<LType,PType,DType>
        where LType:BaseLogic<PType,DType>
        where PType:BusiModuleParameter
        where DType:BusiDataCollection
    {
        static Dictionary<string, LogicEntity> _logics = new Dictionary<string, LogicEntity>();
        static object lockobj = new object();

        protected override void OnLoadAssembly(ProxyManager ma, dynamic options, List<Type> logics)
        {
            foreach (Type t in logics)
            {
                var entity = new LogicEntity(t);
                var lkey = entity.LogicName.ToUpper();
                if (!_logics.ContainsKey(lkey))
                {
                    _logics.Add(lkey, entity);
                }
                else
                {
                    if (_logics.ContainsKey(lkey))
                    {
                        throw new DuplicateLogicException("Duplicate Logic's Name,please make sure Logic:[" + entity.LogicName + "] unique!");
                    }
                }
            }
        }

        protected override bool CheckMyParametersAndConfig(PType p, DType d)
        {
            if (string.IsNullOrEmpty(p.CallLogicName))
            {
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.WARN, string.Format(@"BusiModule执行参数检核作业不通过，缺少呼叫的logicname参数"));
                return false;
            }

            return DoCheckMyParametersAndConfig(p, d);
        }

        protected override void InvokeBusiness(PType p, DType d)
        {
            GetLogic(p.CallLogicName).process(p, d);
        }
        public override void Dispose()
        {
        }

        private LType GetLogic(string logicname)
        {
            var lkey = logicname.ToUpper();
            if (_logics.ContainsKey(lkey))
            {
                return (LType)Activator.CreateInstance(_logics[lkey].LogicType, true);
            }
            else
            {
                throw new NotFoundException("Not found Logic named " + logicname );
            }
        }
        /// <summary>
        /// 执行前的参数检查
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        protected abstract bool DoCheckMyParametersAndConfig(PType p, DType d);
        /// <summary>
        /// 执行后的参数和结果集处理
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="d"></param>
        protected abstract void AfterProcess(PType p, DType d);

        private class LogicEntity
        {
            public LogicEntity(Type t)
            {
                LogicType = t;
                var tmp = (BaseLogic<PType,DType>)Activator.CreateInstance(t,true);
                LogicName = tmp.Name;
                tmp = null;
            }

            public string LogicName
            {
                get;
                set;
            }

            public Type LogicType
            {
                get;
                set;
            }
        }
    }
}

