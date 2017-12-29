using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Resource.Others.Datas;
using EFFC.Frame.Net.Resource.Others.Parameters;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace EFFC.Frame.Net.Resource.Others
{
    /// <summary>
    /// FTP连接资源工具类
    /// </summary>
    public class FtpAccess : IResourceEntity, IDisposable
    {
        string _id = "";
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
        public void Dispose()
        {
            
            
        }

        public void Release()
        {
            
        }
        protected FtpClient Init(FtpParameter p)
        {
            var tmp = p.FTP_URL.ToLower().Replace("ftp://", "");
            var host = tmp.Substring(0, tmp.IndexOf("/") <= 0 ? tmp.Length : tmp.IndexOf("/"));
            var port = host.IndexOf(":") > 0 ? int.Parse(host.Split(':')[1]) : 21;
            host = host.IndexOf(":") > 0 ? host.Split(':')[0] : host;
            var rtn = new FtpClient(host);
            rtn.Port = port;
            rtn.Credentials = new NetworkCredential(p.Login_UserId, p.Login_Password);
            rtn.SocketKeepAlive = p.KeepAlive;
            rtn.Encoding = Encoding.GetEncoding(p.EncodingString);
            rtn.ConnectTimeout = p.TimeOut;

            return rtn;
        }
        /// <summary>
        /// 判斷指定的文件是否存在
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool IsExists(FtpParameter p)
        {
           
            using (FtpClient client = Init(p))
            {
                return client.FileExists(p.RemoteFilePath);
            }
        }
        /// <summary>
        /// 判断RemoteDirectory的目录是否存在
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool IsExistsDirectory(FtpParameter p)
        {
            using (FtpClient client = Init(p))
            {
                return client.DirectoryExists(p.RemoteDirectory);
            }
        }
        /// <summary>
        /// 删除远程主机上的文件
        /// </summary>
        /// <param name="p"></param>
        public void DeleteFile(FtpParameter p)
        {
            using(var client = Init(p))
            {
                client.DeleteFile(p.RemoteFilePath);
            }
        }
        /// <summary>
        /// 下载RemoteFilePath下的文件到DownLoad_Path中
        /// </summary>
        /// <param name="p"></param>
        /// <param name="processsing"></param>
        /// <param name="end"></param>
        /// <param name="reserveObject"></param>
        public void DownLoadFile(FtpParameter p, Action<FTPStatusData> processsing = null, Action<FTPStatusData> end = null, object reserveObject = null)
        {
            var buffers = new byte[4096];
            using (var client = Init(p))
            {
                using (var s = client.OpenRead(p.RemoteFilePath))
                    using(var tos = File.OpenWrite(p.DownLoad_Path))
                {
                    byte[] buffer = new byte[p.BufferSize];
                    int bytesRead;
                    long transfer = 0;
                    double speed = 0;
                    var st = DateTime.Now;
                    var totalbyte = s.Length;
                    while (true)
                    {
                        bytesRead = s.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;
                        tos.Write(buffer, 0, bytesRead);
                        transfer += bytesRead;

                        if (processsing != null)
                        {
                            var fsd = new FTPStatusData();
                            fsd.CurrentByteLength = bytesRead;
                            fsd.TransferedByteLength = transfer;
                            fsd.TotalByteLength = totalbyte;
                            fsd.FileName = Path.GetFileName(p.RemoteFilePath);
                            fsd.CostTime = DateTime.Now - st;
                            var costms = fsd.CostTime.TotalMilliseconds;
                            if (costms != 0)
                                speed = (double)transfer / costms * 1000;
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
                        fsd.FileName = Path.GetFileName(p.RemoteFilePath);
                        fsd.CostTime = DateTime.Now - st;
                        var costms = fsd.CostTime.TotalMilliseconds;
                        if (costms != 0)
                            speed = (double)transfer / costms * 1000;
                        fsd.Speed = speed;
                        fsd.CurrentStatus = FTPStatusData.FtpStaus.End;
                        fsd.ReserveObject = reserveObject;

                        end(fsd);
                    }

                }
            }
        }
        /// <summary>
        /// 下载RemoteFilePath下的文件到Stream
        /// </summary>
        /// <param name="p"></param>
        /// <param name="processsing"></param>
        /// <param name="end"></param>
        /// <param name="reserveObject"></param>
        /// <returns></returns>
        public Stream DownLoad(FtpParameter p, Action<FTPStatusData> processsing=null, Action<FTPStatusData> end=null, object reserveObject=null)
        {
            using (var client = Init(p))
            {

                using (var stream = client.OpenRead(p.RemoteFilePath))
                {

                    string fileName =
                        Path.GetFileName(p.RemoteFilePath);

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
                        var totalbyte = stream.Length;
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
                                fsd.FileName = Path.GetFileName(p.RemoteFilePath);
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
                            fsd.FileName = Path.GetFileName(p.RemoteFilePath);
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
            }
        }
        /// <summary>
        /// 上传Upload_FilePath路径下的文件到RemoteFilePath
        /// </summary>
        /// <param name="p"></param>
        /// <param name="processsing"></param>
        /// <param name="end"></param>
        /// <param name="reserveObject"></param>
        /// <returns></returns>
        public bool UploadFile(FtpParameter p,Action<FTPStatusData> processsing=null, Action<FTPStatusData> end=null, object reserveObject=null)
        {
            using (var client = Init(p))
            {
                using (var stream = client.OpenWrite(p.RemoteFilePath))
                using (var fileStream = File.Open(p.Upload_FilePath, FileMode.Open))
                {
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
                    return true;
                }
            }
        }
        /// <summary>
        /// 上传Upload_Bytes到RemoteFilePath
        /// </summary>
        /// <param name="p"></param>
        /// <param name="processsing"></param>
        /// <param name="end"></param>
        /// <param name="reserveObject"></param>
        /// <returns></returns>
        public bool Upload(FtpParameter p, Action<FTPStatusData> processsing = null, Action<FTPStatusData> end = null, object reserveObject = null)
        {
            using (var client = Init(p))
            {
                using (var stream = client.OpenWrite(p.RemoteFilePath))
                using (var soureStream = new MemoryStream(p.Upload_Bytes))
                {
                    byte[] buffer = new byte[p.BufferSize];
                    int bytesRead;
                    long transfer = 0;
                    double speed = 0;
                    var st = DateTime.Now;
                    var totalbyte = soureStream.Length;
                    while (true)
                    {
                        bytesRead = soureStream.Read(buffer, 0, buffer.Length);
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
                    return true;
                }
            }
        }

        /// <summary>
        /// list出RemoteDirectory下的目錄
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string ListDirectory(FtpParameter p)
        {
            string rtn = "";
               using(var client = Init(p))
            {
                foreach(var item in client.GetListing(p.RemoteDirectory))
                {
                    rtn += item.Name + ",";
                }
            }

            return rtn == "" ? rtn : rtn.Substring(0, rtn.Length - 1);
        }
        /// <summary>
        /// 获得RemoteFilePath的文件大小
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string FileSize(FtpParameter p)
        {
            string rtn = "";
            using (var client = Init(p))
            {
                rtn = client.GetFileSize(p.RemoteFilePath).ToString();
            }

            return rtn;
        }
        /// <summary>
        /// 獲取RemoteDirectory下所有文件的size
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Dictionary<string, object> ListFileSize(FtpParameter p)
        {
            Dictionary<string, object> rtn = new Dictionary<string, object>();
            using (var client = Init(p))
            {
                foreach (var item in client.GetListing(p.RemoteDirectory))
                {
                    rtn.Add(item.Name, item.Size.ToString());
                }
            }

            return rtn;
        }
        /// <summary>
        /// 创建RemoteDirectory指定的目录
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool FtpMakeDir(FtpParameter p)
        {
            using (var client = Init(p))
            {
                client.CreateDirectory(p.RemoteDirectory);
            }
            return true;
        }
    }
}
