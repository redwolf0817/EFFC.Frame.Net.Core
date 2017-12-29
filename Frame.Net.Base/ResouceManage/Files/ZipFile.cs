using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using ICSharpCode.SharpZipLib.Checksums;

namespace EFFC.Frame.Net.Base.ResouceManage.Files
{
    public class CompressFile : IResourceEntity, IDisposable
    {
        /// <summary>
        /// 壓縮操作對象
        /// </summary>
        ZipFile zipOper;
        FastZip fastzipOper;

        FileStream Zip_File;
        ZipOutputStream ZipStream;
        ZipEntry ZipEntry;

        string id="";
        /// <summary>
        /// 構造函數
        /// </summary>
        public CompressFile()
        {
            
            id = "ZIP_" + Guid.NewGuid().ToString();

        }

       /// <summary>
        /// 壓縮文件返回壓縮檔名稱
       /// </summary>
       /// <returns>string 返回壓縮后的zip文件名稱</returns>
        public string CompressReturnFileName()
        {
            //if (this.ZipParameter.Zip_Name == "")
            //    ZipParameter.Zip_Name = "ZipFile" + Guid.NewGuid().ToString();

            //zipOper = ZipFile.Create(ZipParameter.Zip_Name);

            //zipOper.BeginUpdate();

            //if (ZipParameter.File != "")
            //    zipOper.Add(ZipParameter.File);

            //foreach (string filename in ZipParameter.FileList)
            //    zipOper.Add(filename);

            //zipOper.CommitUpdate();

            //if (ZipParameter.DirectoryName != "")
            //{
            //    fastzipOper = new FastZip();
            //    if (Directory.Exists(ZipParameter.DirectoryName))
            //        fastzipOper.CreateZip(ZipParameter.Zip_Name, ZipParameter.DirectoryName, true, "");
            //    else
            //        throw new DirectoryNotFoundException("找不到指定的壓縮目錄！");
            //}
            try
            {
                var path = Path.GetDirectoryName(ZipParameter.Zip_Name);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                Zip_File = File.Create(ZipParameter.Zip_Name);
                ZipStream = new ZipOutputStream(Zip_File);
                ZipStream.Password = ZipParameter.Password;
                ZipStream.SetLevel(ZipParameter.Level);
                foreach (string FileToZip in ZipParameter.FileList)
                {
                    Zip_File = File.OpenRead(FileToZip);
                    byte[] buffer = new byte[Zip_File.Length];
                    Zip_File.Read(buffer, 0, buffer.Length);
                    Zip_File.Close();
                    ZipEntry = new ZipEntry(Path.GetFileName(FileToZip));
                    ZipStream.PutNextEntry(ZipEntry);
                    ZipStream.Write(buffer, 0, buffer.Length);
                }
            }
            finally
            {
                if (ZipEntry != null)
                {
                    ZipEntry = null;
                }
                if (ZipStream != null)
                {
                    ZipStream.Finish();
                    ZipStream.Close();
                }
                if (Zip_File != null)
                {
                    Zip_File.Close();
                    Zip_File = null;
                }
                GC.Collect();
                GC.Collect(1);
            }


            return ZipParameter.Zip_Name;
        }

        /// <summary>
        /// 壓縮返回文件流
        /// </summary>
        /// <returns>Stream 返回壓縮后在文件流</returns>
        public Stream CompressReturnStream()
        {
            Stream stream = new FileStream(ZipParameter.Zip_Name,FileMode.Create);

            zipOper = ZipFile.Create(stream);

            zipOper.BeginUpdate();

            if (ZipParameter.File != "")
                zipOper.Add(ZipParameter.File);

            foreach (string filename in ZipParameter.FileList)
                zipOper.Add(filename);

            zipOper.CommitUpdate();

            if (ZipParameter.DirectoryName != "")
            {
                fastzipOper = new FastZip();
                if (Directory.Exists(ZipParameter.DirectoryName))
                    fastzipOper.CreateZip(stream, ZipParameter.DirectoryName, true, "","");
                else
                    throw new DirectoryNotFoundException("找不到指定的壓縮目錄！");
            }

            return stream;
        }
        public List<string> UnZipFile(Stream fs,string rootpath)
        {
            List<string> rtn = new List<string>();
            fs.Seek(0, SeekOrigin.Begin);
            using (ZipInputStream s = new ZipInputStream(fs))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    Console.WriteLine(theEntry.Name);

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string path = rootpath + directoryName;
                    string fileName = Path.GetFileName(theEntry.Name);
                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(path);
                    }
                    

                    if (fileName != String.Empty)
                    {
                        
                        using (FileStream streamWriter = File.Create(rootpath+theEntry.Name))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            rtn.Add(theEntry.Name);
                        }
                    }
                }
            }

            return rtn;
        }
        /// <summary>
        /// 壓縮文件參數
        /// </summary>
        public ZipParameter ZipParameter { get; set; }

        #region IResouceEntity 成員

        public void Release()
        {
            if (zipOper != null)
            {
                zipOper.Close();
            }
            fastzipOper = null;
        }

        #endregion
        /// <summary>
        /// GC回收机制，无须调用
        /// </summary>
        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

        public string ID
        {
            get { return id; }
        }
    }
}
