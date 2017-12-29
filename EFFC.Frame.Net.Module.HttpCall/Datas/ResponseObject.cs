using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.HttpCall.Datas
{
    public class ResponseObject:DataCollection
    {
        FrameDLRObject _header = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);

        public ResponseObject()
        {
            StatusCode = 200;
        }
        /// <summary>
        /// 呼叫结果
        /// </summary>
        public object Result
        {
            get;
            set;
        }
        /// <summary>
        /// 返回的结果的Content-type
        /// </summary>
        public string ContentType
        {
            get;
            set;
        }
        /// <summary>
        /// response的header数据
        /// </summary>
        public FrameDLRObject Header
        {
            get
            {
                return _header;
            }
        }
        /// <summary>
        /// response的status code
        /// </summary>
        public int StatusCode
        {
            get;
            set;
        }
        /// <summary>
        /// 下载下来的文件名称
        /// </summary>
        public string FileName
        {
            get;
            set;
        }
    }
}
