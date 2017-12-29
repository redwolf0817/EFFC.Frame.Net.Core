using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;


namespace EFFC.Frame.Net.Base.Module
{
    public abstract class BaseModule:IModular,IDriver
    {
        protected abstract void Run(ParameterStd p, DataCollection d);
        protected abstract void OnError(Exception ex, ParameterStd p, DataCollection d);

        public void StepStart(ParameterStd p, DataCollection d)
        {
            try
            {
                Run(p, d);
            }
            catch (Exception ex)
            {
                OnError(ex, p, d);
            }
        }

        public abstract string Name
        {
            get;
        }

        public abstract string Version
        {
            get;
        }

        public abstract string Description
        {
            get;
        }
    }

    public abstract class BaseModule<P, D> : IModular, IDriver<P, D>
        where P:ParameterStd
        where D:DataCollection
    {
        protected abstract void Run(P p, D d);
        protected abstract void OnError(Exception ex, P p, D d);

        public void StepStart(P p, D d)
        {
            try
            {
                Run(p, d);
            }
            catch (Exception ex)
            {
                OnError(ex, p, d);
            }
        }

        public abstract string Name
        {
            get;
        }

        public abstract string Version
        {
            get;
        }

        public abstract string Description
        {
            get;
        }
    }
}
