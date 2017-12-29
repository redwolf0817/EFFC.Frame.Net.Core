using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.Parameters.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Flow.Interfaces
{
    /// <summary>
    /// 用于定义具体流程实体实行操作
    /// </summary>
    /// <typeparam name="P"></typeparam>
    /// <typeparam name="D"></typeparam>
    public interface IFlowDefine<P,D>
        where P:FlowParameter
        where D:FlowData
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        string FlowName { get; }
        /// <summary>
        /// 流程版本号
        /// </summary>
        FlowVersion Version { get; }
        /// <summary>
        /// 获取定义的条件驱动器
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        IConditionDriver<P, D> GetDriver(P p, D d);
    }
}
