using EFFC.Frame.Net.Base.Module.Proxy;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.Common;
using Microsoft.AspNetCore.Http;

namespace EFFC.Frame.Net.Module.Razor
{
    public class RazorProxy : ModuleProxy
    {
        protected override DataCollection ConvertDataCollection(ref object obj)
        {
            var rtn = new RazorData();

            return rtn;
        }

        protected override ParameterStd ConvertParameters(object[] obj)
        {
            var rtn = new RazorParam();
            if(obj != null && obj.Length > 0)
            {
                if(obj.Length == 1)
                {
                    if (obj[0] is RazorParam)
                    {
                        rtn = (RazorParam)obj[0];
                    }
                    else
                    {
                        var feo = FrameExposedObject.From(obj[0]);
                        rtn.ViewPath = feo.ViewPath;
                        rtn.CurrentHttpContext = feo.HttpContext;
                        if (rtn.CurrentHttpContext == null) rtn.CurrentHttpContext = feo.DefaultHttpContext.value;
                        rtn.Model = feo.Model;
                        rtn.ViewList = feo.ViewList;
                    }
                }
                else if(obj.Length == 4)
                {
                    var fea = FrameExposedArray.From(obj);
                    rtn.ViewPath = fea.String.value;
                    rtn.CurrentHttpContext = fea.HttpContext.value;
                    if (rtn.CurrentHttpContext == null) rtn.CurrentHttpContext = fea.DefaultHttpContext.value;
                    rtn.Model = fea.Object.value;
                    rtn.ViewList = fea.dictionary.value;
                }
            }
            return rtn;
        }

        protected override BaseModule CreateModuleInstance()
        {
            return new RazorViewModule();
        }

        protected override void Dispose(ParameterStd p, DataCollection d)
        {
            if (p == null) return;

            var rp = (RazorParam)p;
            rp.Resources.Dispose();
            rp.CurrentTransToken.Release();
            rp.TransTokenList.Clear();
            rp.ViewPath = null;
            rp.CurrentHttpContext = null;
            rp.ViewList = null;
            rp.Model = null;

            GC.Collect();
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            throw ex;
        }

        protected override void ParseDataCollection2Result(DataCollection d, ref object obj)
        {
            if (obj is string)
                obj = ((RazorData)d).RenderText;
            else
            {
                obj = null;
            }
        }
    }
}
