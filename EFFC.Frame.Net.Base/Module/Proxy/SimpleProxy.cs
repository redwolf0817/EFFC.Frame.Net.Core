using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Base.Module.Proxy
{
    public class SimpleProxy<TModule,TParameter, TData> : ModuleProxy
        where TParameter : ParameterStd
        where TData : DataCollection
        where TModule:BaseModule
    {
        /// <summary>
        /// 简易呼叫接口
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public TData SimpleCall(TParameter p)
        {
            object rtn = Activator.CreateInstance<TData>();
            CallModule(ref rtn, p);

            return (TData)rtn;
        }

        protected override DataCollection ConvertDataCollection(ref object obj)
        {
            TData rtn = null;
            if (obj != null)
            {
                if (obj is TData)
                    rtn = (TData)obj;
                else
                    rtn = Activator.CreateInstance<TData>();
            }
            else
            {
                rtn = Activator.CreateInstance<TData>();
            }

            return rtn;
        }

        protected override ParameterStd ConvertParameters(object[] obj)
        {
            TParameter rtn = null;
            if (obj != null && obj.Length > 0)
            {
                if (obj[0] is TParameter)
                    rtn = (TParameter)obj[0];
                else
                    rtn = Activator.CreateInstance<TParameter>();
            }
            else
            {
                rtn = Activator.CreateInstance<TParameter>();
            }

            return rtn;
        }

        protected override BaseModule CreateModuleInstance()
        {
            return (TModule)Activator.CreateInstance(typeof(TModule), true);
        }

        protected override void Dispose(ParameterStd p, DataCollection d)
        {
            
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            throw ex;
        }

        protected override void ParseDataCollection2Result(DataCollection d, ref object obj)
        {
           if(obj != null)
            {
                if(obj is TData)
                {
                    if(obj == d)
                    {
                        //do nothing
                    }
                    else
                    {
                        obj = d.DeepCopy<TData>();
                    }
                }
                else
                {
                    //do nothing
                }
            }
            else
            {
                obj = d;
            }
        }
    }
}
