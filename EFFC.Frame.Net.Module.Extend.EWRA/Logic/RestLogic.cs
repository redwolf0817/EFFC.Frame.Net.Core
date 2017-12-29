using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Business.Logic;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using EFFC.Frame.Net.Module.Extend.EWRA.Constants;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using EFFC.Frame.Net.Module.Web.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Logic
{
    /// <summary>
    /// Rest API专用Logic
    /// </summary>
    public partial class RestLogic : WebBaseLogic<EWRAParameter, EWRAData>
    {
        protected override void DoProcess(EWRAParameter p, EWRAData d)
        {
            d.Result = d.InvokeMethod.Invoke(this, d.InvokeParameters); 
        }
        
        [EWRAIsOpen(false)]
        public virtual List<object> get()
        {
            return new List<object>();
        }
        [EWRAIsOpen(false)]
        public virtual object get(string id)
        {
            return FrameDLRObject.CreateInstance();
        }
        [EWRAIsOpen(false)]
        public virtual object put()
        {
            return new
            {
                id = ""
            };
        }
        [EWRAIsOpen(false)]
        public virtual object post()
        {
            return new
            {
                id = ""
            };
        }
        [EWRAIsOpen(false)]
        public virtual object patch(string id)
        {
            return new
            {
                id = ""
            };
        }
        [EWRAIsOpen(false)]
        public virtual bool delete(string id)
        {
            return true;
        }
    }
}
