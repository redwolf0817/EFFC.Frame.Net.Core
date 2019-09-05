using EFFC.Frame.Net.Base.Common;
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
using System.IO;
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
            if (d.Result is Stream ||
                d.Result is byte[])
            {
                SetContentType(RestContentType.Binary);
            }
            if(d.ContentType == RestContentType.HTML)
            {
                d.StatusCode = RestStatusCode.OK;
            }
        }
        /// <summary>
        /// 设定下载文件的名称
        /// </summary>
        /// <param name="filename"></param>
        public void SetDownLoadFileName(string filename)
        {
            this.CallContext_DataCollection.Download_File_Name = filename;
        }
        /// <summary>
        /// 设置下载文件的类型描述
        /// </summary>
        /// <param name="file_content_type"></param>
        public void SetDownLoadFileContentType(string file_content_type)
        {
            CallContext_DataCollection.File_ContentType = file_content_type;
        }
        /// <summary>
        /// 设定返回的content类型
        /// </summary>
        /// <param name="contenttype"></param>
        public void SetContentType(RestContentType contenttype)
        {
            CallContext_DataCollection.ContentType = contenttype;
        }
        /// <summary>
        /// Response跳转
        /// </summary>
        /// <param name="touri"></param>
        public void RedirectTo(string touri)
        {
            this.CallContext_DataCollection.RedirectUri = touri;
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
