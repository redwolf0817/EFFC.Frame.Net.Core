using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.HttpCall;
using EFFC.Frame.Net.Module.HttpCall.Datas;
using EFFC.Frame.Net.Module.HttpCall.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.WeChat
{
    public class WeixinHttpCallProxy : ModuleProxy
    {
        protected override DataCollection ConvertDataCollection(ref object obj)
        {
            var d = new ResponseObject();
            return d;
        }

        protected override ParameterStd ConvertParameters(object[] obj)
        {
            var p = new HttpParameter();
            if (obj != null && obj.Length > 0)
            {
                if (obj[0] != null)
                {
                    var feo = FrameExposedObject.From(obj[0]);

                    p.ToUrl = feo.Url;
                    if (ComFunc.nvl(feo.ContentEncoding) != "")
                        p.ContentEncoding = feo.ContentEncoding;
                    if (ComFunc.nvl(feo.ContentType) != "")
                        p.ContentType = feo.ContentType;
                    if (feo.Certificate != null)
                        p.Certificate = feo.Certificate;
                    if (ComFunc.nvl(feo.RequestMethod) != "")
                        p.RequestMethod = feo.RequestMethod;
                    if (feo.PostData != null)
                    {
                        p.PostData.Load(feo.PostData);
                    }
                    if (feo.Header != null)
                    {
                        p.Header.Load(feo.Header);
                    }
                }
            }
            return p;
        }

        protected override BaseModule CreateModuleInstance()
        {
            return new HttpRemoteModule();
        }

        protected override void Dispose(ParameterStd p, DataCollection d)
        {
            p.Dispose();
            d.Dispose();
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            throw ex;
        }

        protected override void ParseDataCollection2Result(DataCollection d, ref object obj)
        {
            var rpd = (ResponseObject)d;
            var responsesobj = rpd.Result;
            var contenttype = rpd.ContentType;

            if (contenttype.ToLower().IndexOf("/xml") > 0)
            {
                if (obj is FrameDLRObject)
                {
                    var dobj = (FrameDLRObject)obj;
                    dobj.Load(responsesobj);
                }
                else if (obj is string)
                {
                    obj = ComFunc.nvl(responsesobj);
                }
            }
            else if (contenttype.ToLower().StartsWith("image")
                || contenttype.ToLower().StartsWith("audio")
                || contenttype.ToLower().StartsWith("video"))
            {
                if (obj is FrameDLRObject)
                {
                    dynamic dobj = (FrameDLRObject)obj;
                    dobj.content = responsesobj;
                    dobj.filename = rpd.FileName;
                    dobj.contenttype = contenttype;
                }
                else if (obj is Stream)
                {
                    if (responsesobj != null)
                        obj = new MemoryStream((byte[])responsesobj);
                }
            }
            else
            {
                if (obj is string)
                {
                    if (responsesobj is byte[])
                    {
                        obj = ComFunc.ByteToString(((byte[])responsesobj), Encoding.UTF8);
                    }
                    else
                    {
                        obj = ComFunc.nvl(responsesobj);
                    }
                }
                else if (obj is FrameDLRObject)
                {
                    var dobj = (FrameDLRObject)obj;
                    if (!(responsesobj is byte[]))
                    {
                        dobj.Load(responsesobj);
                    }
                }
                else
                {
                    FrameDLRObject dobj = FrameDLRObject.CreateInstance(responsesobj);
                    obj = dobj.ToModel(obj.GetType());
                }
            }
        }
    }
}
