using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using System.Reflection;
using EFFC.Frame.Net.Module.Extend.EWRA.Constants;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System.Linq;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Logic
{
    /// <summary>
    /// 带校验功能的RestLogic
    /// </summary>
    public class ValidRestLogic: RestLogic
    {
        protected override void DoProcess(EWRAParameter p, EWRAData d)
        {
            var validattr = d.InvokeMethod.GetCustomAttributes<EWRAValidAttribute>(true);
            var errormsg = "校验不通过";
            if (IsValid(p, d, ref errormsg, validattr.ToArray()))
            {
                base.DoProcess(p, d);
            }
            else
            {
                d.StatusCode = RestStatusCode.INVALID_REQUEST;

                d.Error = errormsg;
            }
        }

        /// <summary>
        /// 方法执行之前的校验操作，如果所有的校验都通过则为true，否则为false
        /// 特别说明：无属性时则不做任何校验，返回true
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="errormsg"></param>
        /// <param name="validAttributes"></param>
        /// <returns></returns>
        protected virtual bool IsValid(EWRAParameter p, EWRAData d, ref string errormsg, params EWRAValidAttribute[] validAttributes)
        {
            if (validAttributes == null)
                return true;
            else
            {
                foreach (var a in validAttributes)
                {
                    if (!a.IsValid(p, d))
                    {
                        errormsg = a.ErrorMsg;
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
