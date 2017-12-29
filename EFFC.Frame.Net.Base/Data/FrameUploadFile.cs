using EFFC.Frame.Net.Base.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EFFC.Frame.Net.Base.Data
{
    /// <summary>
    /// 框架自定义上传文件接口
    /// </summary>
    public class FrameUploadFile:IFormFile
    {
        private string _contentType;
        private string _filename;
        private Stream _stream;
        private Encoding _encoding;
        public FrameUploadFile(IFormFile file)
        {
            this._filename = file.Name;
            this._contentType = file.ContentType;
            this._stream = new MemoryStream();
            file.OpenReadStream().CopyTo(this._stream);
            this._encoding = Encoding.UTF8;
        }
        public FrameUploadFile(string filename, string contentType, Stream stream, Encoding encode)
        {
            this._filename = filename;
            this._contentType = contentType;
            this._stream = stream;
            this._encoding = encode;
        }

        public FrameUploadFile(string filename,string contentype, Stream file)
        {
            this._filename = filename;
            this._contentType = contentype;
            ComFunc.CopyStream(file, this._stream);
        }
        /// <summary>
        /// 保存文件流
        /// </summary>
        /// <param name="path"></param>
        public void SaveAs(string path)
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
                s.Dispose();
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
                s.Dispose();
            }
        }

        public Stream OpenReadStream()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Stream target)
        {
            throw new NotImplementedException();
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public int ContentLength
        {
            get
            {
                return (int) this._stream.Length;
            }
        }

        public string ContentType
        {
            get
            {
                return this._contentType;
            }
        }

        public string FileName
        {
            get
            {
                return this._filename;
            }
        }

        public Stream InputStream
        {
            get
            {
                return this._stream;
            }
        }

        public string ContentDisposition => throw new NotImplementedException();

        public IHeaderDictionary Headers => throw new NotImplementedException();

        public long Length => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();
    }
}