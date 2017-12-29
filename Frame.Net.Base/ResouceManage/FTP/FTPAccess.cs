using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;

namespace EFFC.Frame.Net.Base.ResouceManage.FTP
{
    /// <summary>
    /// FTP连接资源工具类
    /// </summary>
    public class FTPAccess : IResourceEntity, IDisposable
    {
        FtpWebRequest ftpRequest;
        Stream stream = null;
        FileStream fileStream = null;
        StreamReader reader = null;
        FtpWebResponse ftpResponse = null;
        string _id = "";
        Encoding encoding = null;

        #region IResouceEntity 成員

        public string ID
        {
            get
            {
                if (_id == "")
                {
                    _id = "FTPAccess" + Guid.NewGuid().ToString();
                }

                return _id;
            }
        }

        public void Release()
        {
            if (ftpRequest != null)
            {
                ftpRequest = null;
            }
            if (ftpResponse != null)
            {
                ftpResponse.Close();
                ftpResponse = null;
            }
            if (reader != null)
            {
                reader.Close();
            }
            else if (stream != null)
            {
                stream.Close();
            }

            if (fileStream != null)
                fileStream.Close();
        }

        #endregion

        private void InitByParameters(FtpParameter p)
        {
            ftpRequest = (FtpWebRequest)WebRequest.Create(p.FTP_URL);

            if (p.Login_UserId != "")
            {
                ftpRequest.Credentials = new NetworkCredential(p.Login_UserId, p.Login_Password);
            }
            ftpRequest.UseBinary = p.UseBinary;
            ftpRequest.KeepAlive = p.KeepAlive;
            ftpRequest.UsePassive = p.UsePassive;
            ftpRequest.Timeout = p.TimeOut;
            encoding = Encoding.GetEncoding(p.EncodingString);
        }
        
        /// <summary>
        /// 從fpt上找到對應的存在文件的名稱，以便忽略大小寫，提供給download方法使用
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected string FindMappingFileName(FtpParameter p)
        {
            string name = Common.ComFunc.FindFileName(p.FTP_URL);
            string dir = p.FTP_URL;
            if (name.Length > 0)
            {
                dir = dir.Replace(name, "");
            }

            FtpParameter pp = p.Clone<FtpParameter>();
            pp.FTP_URL = dir;
            StringStd l = ListDirectory(pp);

            StringStd[] ls = l.Split("\r\n");
            Dictionary<StringStd, StringStd> lists = new Dictionary<StringStd, StringStd>();
            foreach (StringStd s in ls)
            {
                lists.Add(s.ToUpper(), s);
            }

            return lists[name.ToUpper()] == null ? "" : lists[name.ToUpper()].Value;
        }

        /// <summary>
        /// 判斷指定的文件是否存在
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool IsExists(FtpParameter p)
        {
            string name = Common.ComFunc.FindFileName(p.FTP_URL);
            string dir = p.FTP_URL;
            if (name.Length > 0)
            {
                dir = dir.Replace(name, "");
            }
            FtpParameter pp = p.Clone<FtpParameter>();
            pp.FTP_URL = dir;
            StringStd l = ListDirectory(pp);
            StringStd[] ls = l.Split("\r\n");
            List<StringStd> lists = new List<StringStd>();
            foreach (StringStd s in ls)
            {
                lists.Add(s.ToUpper());
            }

            if (lists.Contains(name.ToUpper()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 下載文件，FTP_URL,DownLoad_Path,DownLoad_FileName必須有傳入，否則報異常
        /// 如果发现下载的目录有同名档将删除同名档
        /// </summary>
        /// <param name="p"></param>
        public void DownLoad(FtpParameter p)
        {
            DownLoad(p, null, null, null);
        }
        /// <summary>
        /// 下載文件，FTP_URL,DownLoad_Path,DownLoad_FileName必須有傳入，否則報異常
        /// 如果发现下载的目录有同名档将删除同名档
        /// </summary>
        /// <param name="p"></param>
        /// <param name="processsing"></param>
        /// <param name="end"></param>
        /// <param name="reserveObject"></param>
        public void DownLoad(FtpParameter p, Action<FTPStatusData> processsing, Action<FTPStatusData> end, object reserveObject)
        {
            try
            {
                string name = Common.ComFunc.FindFileName(p.FTP_URL);
                string dir = p.FTP_URL;
                if (name.Length > 0)
                {
                    dir = dir.Replace(name, "");
                }

                //忽略文件名稱大小寫的處理
                //if (name.ToUpper() != name)
                //{
                //    name = FindMappingFileName(p);
                //}

                p.FTP_URL = dir + "/" + name;

                InitByParameters(p);

                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                FtpWebResponse downloadResponse =
                    (FtpWebResponse)ftpRequest.GetResponse();

                stream = downloadResponse.GetResponseStream();

                string fileName =
                    Path.GetFileName(ftpRequest.RequestUri.AbsolutePath);

                if (fileName.Length == 0)
                {
                    reader = new StreamReader(stream);
                }
                else
                {
                    if (!Directory.Exists(p.DownLoad_Path))
                    {
                        Directory.CreateDirectory(p.DownLoad_Path);
                    }
                    //如果有指定文件名称
                    if (!string.IsNullOrEmpty(p.DownLoad_FileName))
                    {
                        if (File.Exists(p.DownLoad_Path + "/" + p.DownLoad_FileName))
                        {
                            File.Delete(p.DownLoad_Path + "/" + p.DownLoad_FileName);
                        }
                        fileStream = File.Create(p.DownLoad_Path + "/" + p.DownLoad_FileName);
                    }
                    else
                    {
                        if (File.Exists(p.DownLoad_Path + "/" + fileName))
                        {
                            File.Delete(p.DownLoad_Path + "/" + fileName);
                        }
                        fileStream = File.Create(p.DownLoad_Path + "/" + fileName);
                    }

                    byte[] buffer = new byte[p.BufferSize];
                    int bytesRead;
                    long transfer = 0;
                    double speed = 0;
                    var st = DateTime.Now;
                    var totalbyte = fileStream.Length;
                    while (true)
                    {
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;
                        fileStream.Write(buffer, 0, bytesRead);
                        transfer += bytesRead;

                        if (processsing != null)
                        {
                            var fsd = new FTPStatusData();
                            fsd.CurrentByteLength = bytesRead;
                            fsd.TransferedByteLength = transfer;
                            fsd.TotalByteLength = totalbyte;
                            fsd.FileName = Path.GetFileName(name);
                            fsd.CostTime = DateTime.Now - st;
                            var s = fsd.CostTime.TotalMilliseconds;
                            if (s != 0)
                                speed = (double)transfer / s * 1000;
                            fsd.Speed = speed;
                            fsd.CurrentStatus = FTPStatusData.FtpStaus.Processing;
                            fsd.ReserveObject = reserveObject;

                            processsing(fsd);
                        }
                    }

                    if (end != null)
                    {
                        var fsd = new FTPStatusData();
                        fsd.CurrentByteLength = bytesRead;
                        fsd.TransferedByteLength = transfer;
                        fsd.TotalByteLength = totalbyte;
                        fsd.FileName = Path.GetFileName(name);
                        fsd.CostTime = DateTime.Now - st;
                        var s = fsd.CostTime.TotalMilliseconds;
                        if (s != 0)
                            speed = (double)transfer / s * 1000;
                        fsd.Speed = speed;
                        fsd.CurrentStatus = FTPStatusData.FtpStaus.End;
                        fsd.ReserveObject = reserveObject;

                        end(fsd);
                    }
                }
            }
            finally
            {
                Release();
            }
        }
        /// <summary>
        /// 下载文件，返回数据流
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MemoryStream DownLoadStream(FtpParameter p)
        {
            return DownLoadStream(p, null, null, null);
        }
        /// <summary>
        /// 下载文件，返回数据流
        /// </summary>
        /// <param name="p"></param>
        /// <param name="processsing"></param>
        /// <param name="end"></param>
        /// <param name="reserveObject"></param>
        /// <returns></returns>
        public MemoryStream DownLoadStream(FtpParameter p, Action<FTPStatusData> processsing, Action<FTPStatusData> end, object reserveObject)
        {
            try
            {
                string name = Common.ComFunc.FindFileName(p.FTP_URL);
                string dir = p.FTP_URL;
                if (name.Length > 0)
                {
                    dir = dir.Replace(name, "");
                }

                //忽略文件名稱大小寫的處理
                //if (name.ToUpper() != name)
                //{
                //    name = FindMappingFileName(p);
                //}

                InitByParameters(p);

                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                FtpWebResponse downloadResponse =
                    (FtpWebResponse)ftpRequest.GetResponse();

                stream = downloadResponse.GetResponseStream();

                string fileName =
                    Path.GetFileName(ftpRequest.RequestUri.AbsolutePath);

                MemoryStream rtn = null;
                if (fileName.Length == 0)
                {
                    rtn = new MemoryStream();
                }
                else
                {
                    //如果有指定文件名称
                    rtn = new MemoryStream();

                    byte[] buffer = new byte[p.BufferSize];
                    int bytesRead;
                    long transfer = 0;
                    double speed = 0;
                    var st = DateTime.Now;
                    var totalbyte = fileStream.Length;
                    while (true)
                    {
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;
                        rtn.Write(buffer, 0, bytesRead);

                        transfer += bytesRead;
                        

                        if (processsing != null)
                        {
                            var fsd = new FTPStatusData();
                            fsd.CurrentByteLength = bytesRead;
                            fsd.TransferedByteLength = transfer;
                            fsd.TotalByteLength = totalbyte;
                            fsd.FileName = Path.GetFileName(name);
                            fsd.CostTime = DateTime.Now - st;
                            var s = fsd.CostTime.TotalMilliseconds;
                            if (s != 0)
                                speed = (double)transfer / s * 1000;
                            fsd.Speed = speed;
                            fsd.CurrentStatus = FTPStatusData.FtpStaus.Processing;
                            fsd.ReserveObject = reserveObject;

                            processsing(fsd);
                        }
                    }

                    if (end != null)
                    {
                        var fsd = new FTPStatusData();
                        fsd.CurrentByteLength = bytesRead;
                        fsd.TransferedByteLength = transfer;
                        fsd.TotalByteLength = totalbyte;
                        fsd.FileName = Path.GetFileName(name);
                        fsd.CostTime = DateTime.Now - st;
                        var s = fsd.CostTime.TotalMilliseconds;
                        if (s != 0)
                            speed = (double)transfer / s * 1000;
                        fsd.Speed = speed;
                        fsd.CurrentStatus = FTPStatusData.FtpStaus.End;
                        fsd.ReserveObject = reserveObject;

                        end(fsd);
                    }
                }

                return rtn;
            }
            finally
            {
                Release();
            }
        }
        /// <summary>
        /// 上傳文件，FTP_URL,Upload_FilePath必須有傳入，否則報異常
        /// </summary>
        /// <param name="p"></param>
        public void Upload(FtpParameter p)
        {
            Upload(p, null, null, null);
        }
        /// <summary>
        /// 上傳文件，FTP_URL,Upload_FilePath必須有傳入，否則報異常
        /// </summary>
        /// <param name="p"></param>
        /// <param name="processsing">上传过程中的处理事件</param>
        /// <param name="end">上传完成的处理事件</param>
        /// <param name="reserveObject">传入的保留参数对象</param>
        public void Upload(FtpParameter p, Action<FTPStatusData> processsing,Action<FTPStatusData> end, object reserveObject)
        {
            try
            {
                InitByParameters(p);
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                // UploadFile is not supported through an Http proxy
                // so we disable the proxy for this request.
                ftpRequest.Proxy = null;

                stream = ftpRequest.GetRequestStream();
                fileStream = File.Open(p.Upload_FilePath, FileMode.Open);

                byte[] buffer = new byte[p.BufferSize];
                int bytesRead;
                long transfer = 0;
                double speed = 0;
                var st = DateTime.Now;
                var totalbyte = fileStream.Length;
                while (true)
                {
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;

                    stream.Write(buffer, 0, bytesRead);
                    transfer += bytesRead;
                   

                    if (processsing != null)
                    {
                        var fsd = new FTPStatusData();
                        fsd.CurrentByteLength = bytesRead;
                        fsd.TransferedByteLength = transfer;
                        fsd.TotalByteLength = totalbyte;
                        fsd.FileName = Path.GetFileName(p.FTP_URL);
                        fsd.CostTime = DateTime.Now - st;
                        var s = fsd.CostTime.TotalMilliseconds;
                        if (s != 0)
                            speed = (double)transfer / s * 1000;
                        fsd.Speed = speed;
                        fsd.CurrentStatus = FTPStatusData.FtpStaus.Processing;
                        fsd.ReserveObject = reserveObject;

                        processsing(fsd);
                    }
                }

                // The request stream must be closed before getting 
                // the response.
                stream.Close();

                ftpResponse =
                    (FtpWebResponse)ftpRequest.GetResponse();

                if (end != null)
                {
                    var fsd = new FTPStatusData();
                    fsd.CurrentByteLength = bytesRead;
                    fsd.TransferedByteLength = transfer;
                    fsd.TotalByteLength = totalbyte;
                    fsd.FileName = Path.GetFileName(p.FTP_URL);
                    fsd.CostTime = DateTime.Now - st;
                    var s = fsd.CostTime.TotalMilliseconds;
                    if (s != 0)
                        speed = (double)transfer / s * 1000;
                    fsd.Speed = speed;
                    fsd.CurrentStatus = FTPStatusData.FtpStaus.End;
                    fsd.ReserveObject = reserveObject;

                    end(fsd);
                }
            }
            finally
            {
                Release();
            }
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="p"></param>
        /// <param name="fileStream"></param>
        public void Upload(FtpParameter p, Stream fileStream)
        {
            Upload(p, fileStream, null, null, null);
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="p"></param>
        /// <param name="fileStream"></param>
        /// <param name="processsing">上传过程中的处理事件</param>
        /// <param name="end">上传完成的处理事件</param>
        /// <param name="reserveObject">传入的保留参数对象</param>
        public void Upload(FtpParameter p, Stream fileStream, Action<FTPStatusData> processsing, Action<FTPStatusData> end, object reserveObject)
        {
            try
            {
                FtpCheckDirectoryExist(p);
                InitByParameters(p);
                
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                // UploadFile is not supported through an Http proxy
                // so we disable the proxy for this request.
                ftpRequest.Proxy = null;

                stream = ftpRequest.GetRequestStream();

                byte[] buffer = new byte[p.BufferSize];
                int bytesRead;
                long transfer = 0;
                double speed = 0;
                var st = DateTime.Now;
                var totalbyte = fileStream.Length;
                while (true)
                {
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;
                    stream.Write(buffer, 0, bytesRead);

                    transfer += bytesRead;
                    

                    if (processsing != null)
                    {
                        var fsd = new FTPStatusData();
                        fsd.CurrentByteLength = bytesRead;
                        fsd.TransferedByteLength = transfer;
                        fsd.TotalByteLength = totalbyte;
                        fsd.FileName = Path.GetFileName(p.FTP_URL);
                        fsd.CostTime = DateTime.Now - st;
                        var s = fsd.CostTime.TotalMilliseconds;
                        if (s != 0)
                            speed = (double)transfer / s * 1000;
                        fsd.Speed = speed;
                        fsd.CurrentStatus = FTPStatusData.FtpStaus.Processing;
                        fsd.ReserveObject = reserveObject;

                        processsing(fsd);
                    }
                }

                // The request stream must be closed before getting 
                // the response.
                stream.Close();

                ftpResponse =
                    (FtpWebResponse)ftpRequest.GetResponse();

                if (end != null)
                {
                    var fsd = new FTPStatusData();
                    fsd.CurrentByteLength = bytesRead;
                    fsd.TransferedByteLength = transfer;
                    fsd.TotalByteLength = totalbyte;
                    fsd.FileName = Path.GetFileName(p.FTP_URL);
                    fsd.CostTime = DateTime.Now - st;
                    var s = fsd.CostTime.TotalMilliseconds;
                    if (s != 0)
                        speed = (double)transfer / s * 1000;
                    fsd.Speed = speed;
                    fsd.CurrentStatus = FTPStatusData.FtpStaus.End;
                    fsd.ReserveObject = reserveObject;

                    end(fsd);
                }
            }
            finally
            {
                Release();
            }
        }

       

        
        /// <summary>
        /// list出ftp的目錄
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string ListDirectory(FtpParameter p)
        {
            string rtn = "";
            try
            {
                InitByParameters(p);

                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse listResponse =
                    (FtpWebResponse)ftpRequest.GetResponse();

                reader = new StreamReader(listResponse.GetResponseStream(), encoding);
                rtn = reader.ReadToEnd();
            }
            finally
            {
                Release();
            }

            return rtn;
        }
        
        /// <summary>
        /// 獲取指定文件的size
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string FileSize(FtpParameter p)
        {
            string rtn = "";
            try
            {
                InitByParameters(p);
                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse listResponse =
                    (FtpWebResponse)ftpRequest.GetResponse();

                rtn = listResponse.ContentLength.ToString();
            }
            finally
            {
                Release();
            }

            return rtn;
        }
        
        /// <summary>
        /// 獲取指定url下所有文件的size
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Dictionary<string, object> ListFileSize(FtpParameter p)
        {
            Dictionary<string, object> rtn = new Dictionary<string, object>();
            InitByParameters(p);
            string listd = ListDirectory(p);
            listd = listd.Replace("\r", "");
            string[] arraryd = listd.Split('\n');

            for (int i = 0; i < arraryd.Length; i++)
            {
                if (arraryd[i].Length <= 0)
                    continue;

                string filename = "";
                //Unix格式专用
                if (arraryd[i].Trim().IndexOf("/") >= 0)
                {
                    string[] ss = arraryd[i].Trim().Split('/');
                    filename = ss[ss.Length - 1];
                }
                else
                {
                    filename = arraryd[i].Trim();
                }

                string fileurl = p.FTP_URL + "/" + filename;
                string filesize = FileSize(p);
                rtn[arraryd[i]] = filesize;
            }


            return rtn;
        }
        //判断文件的目录是否存,不存则创建  
        private void FtpCheckDirectoryExist(FtpParameter p)
        {
            string fullDir = FtpParseDirectory(p.FTP_URL);
            
            string[] dirs = fullDir.Split('/');
            string curDir = dirs[0] + "//" + dirs[2] + "/";
            for (int i = 3; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                //如果是以/开始的路径,第一个为空    
                if (dir != null && dir.Length > 0)
                {
                    curDir += dir + "/";

                    var ftpRequest = (FtpWebRequest)WebRequest.Create(curDir);
                    ftpRequest.Proxy = null;
                    ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                    
                    try
                    {
                        if (p.Login_UserId != "")
                        {
                            ftpRequest.Credentials = new NetworkCredential(p.Login_UserId, p.Login_Password);
                        }
                        FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                        response.Close();
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        ftpRequest.Abort();
                    }
                }
            }
        }

        private string FtpParseDirectory(string destFilePath)
        {
            return destFilePath.Substring(0, destFilePath.LastIndexOf("/"));
        }

        //创建目录  
        public Boolean FtpMakeDir(FtpParameter p)
        {
            InitByParameters(p);
            ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                response.Close();
            }
            finally
            {
                Release();
            }
            return true;
        }  
        /// <summary>
        /// GC回收机制
        /// </summary>
        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }
    }
}
