using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EFFC.Frame.Net.Base.Data
{
    /// <summary>
    /// 框架自定义上传文件接口
    /// </summary>
    public class FrameUploadFile:HttpPostedFileBase
    {
        private string _contentType;
        private string _filename;
        private Stream _stream;
        private Encoding _encoding;

        public FrameUploadFile(string filename, string contentType, Stream stream, Encoding encode)
        {
            this._filename = filename;
            this._contentType = contentType;
            this._stream = stream;
            this._encoding = encode;
        }

        public FrameUploadFile(HttpPostedFile file)
        {
            this._filename = file.FileName;
            this._contentType = file.ContentType;
            ComFunc.CopyStream(file.InputStream, this._stream);
        }
        /// <summary>
        /// 保存文件流
        /// </summary>
        /// <param name="path"></param>
        public override void SaveAs(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                return;
            }
            FileStream s = new FileStream(path, FileMode.OpenOrCreate);
            try
            {
                _stream.Seek(0, SeekOrigin.Begin);
                ComFunc.CopyStream(_stream, s);
                //this._stream.CopyTo(s);
                s.Flush();
            }
            finally
            {
                s.Close();
            }
        }
        /// <summary>
        /// 保存文件流
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encode"></param>
        public void SaveAs(string path, Encoding encode)
        {
            if (!Path.IsPathRooted(path))
            {
                return;
            }
            
            FileStream s = new FileStream(path, FileMode.OpenOrCreate);
            
            try
            {
                _stream.Seek(0, SeekOrigin.Begin);
                ComFunc.CopyStream(_stream, s, _encoding, encode);
                s.Flush();
            }
            finally
            {
                s.Close();
            }
        }

        public override int ContentLength
        {
            get
            {
                return (int) this._stream.Length;
            }
        }

        public override string ContentType
        {
            get
            {
                return this._contentType;
            }
        }

        public override string FileName
        {
            get
            {
                return this._filename;
            }
        }

        public override Stream InputStream
        {
            get
            {
                return this._stream;
            }
        }
    }
}