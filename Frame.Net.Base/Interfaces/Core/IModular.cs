using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Base.Interfaces.Core
{
    /// <summary>
    /// 可模块化
    /// </summary>
    public interface IModular
    {
        string Name { get; }
        string Version { get; }
        string Description { get; }
    }
}
