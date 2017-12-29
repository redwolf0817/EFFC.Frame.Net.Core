using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EWRAValidAttribute:Attribute
    {
        protected object[] Args
        {
            get;
            set;
        }
        public string ErrorMsg
        {
            get;
            protected set;
        }
        public EWRAValidAttribute(params object[] args)
        {
            Args = args;
            ErrorMsg = "校验不通过";
        }
        public EWRAValidAttribute(string errorMsg, params object[] args)
        {
            Args = args;
            ErrorMsg = errorMsg;
        }
        /// <summary>
        /// 进行校验，判定校验是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsValid(EWRAParameter ep,EWRAData ed)
        {
            var rtn = DoValid(ep, ed);
            return rtn;
        }

        protected virtual bool DoValid(EWRAParameter ep, EWRAData ed)
        {
            return true;
        }
    }
}
