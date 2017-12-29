using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;

namespace EFFC.Frame.Net.Base.Interfaces.Core
{
    /// <summary>
    /// 模块呼叫接口
    /// </summary>
    public interface IModularProxy
    {
        bool CallModule(ParameterStd p, DataCollection data);
        void OnError(Exception ex, ParameterStd p, DataCollection data);
    }
    /// <summary>
    /// 模块呼叫接口
    /// </summary>
    /// <typeparam name="P"></typeparam>
    /// <typeparam name="D"></typeparam>
    public interface IModularProxy<P, D>
        where P : ParameterStd
        where D : DataCollection
    {
        bool CallModule(P p, D data);
        void OnError(Exception ex, P p, D data);
    }
    public interface IModularAsyncProxy
    {
        bool BeginCallModule(ParameterStd p, DataCollection data, Action<ParameterStd,DataCollection> callback);
        bool EndCallModule(ParameterStd p, DataCollection data);
        void WaitMe(ParameterStd p, DataCollection data);
        void OnError(Exception ex, ParameterStd p, DataCollection data);
    }
    public interface IModularAsyncProxy<P, D>
        where P : ParameterStd
        where D : DataCollection
    {
        bool BeginCallModule(P p, D data, Action<P, D> callback);
        bool EndCallModule(P p, D d);
        void WaitMe(P p, D data);
        void OnError(Exception ex, P p, D data);
    }
}
