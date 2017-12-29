using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;

namespace EFFC.Frame.Net.Base.Interfaces.Core
{
    /// <summary>
    /// 模块的最小单位-Unit，只可本地调用
    /// </summary>
    public interface IUnit
    {
        DataCollection DoOperate(ParameterStd p);
    }
    /// <summary>
    /// 模块的最小单位-Unit，只可本地调用
    /// </summary>
    /// <typeparam name="P"></typeparam>
    public interface IUnit<P> where P : ParameterStd
    {
        DataCollection DoOperate(P p);
    }
}
