using System;
using System.Collections.Generic;
using System.IO;
using EFFC.Frame.Net.Base.Interfaces.Core;
using System.IO.Compression;
using System.Text;

namespace EFFC.Frame.Net.Base.ResouceManage.Files
{
    public class CompressFile : IResourceEntity, IDisposable
    {
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
            ZipArchive zipArchive = null;
            var path = Path.GetDirectoryName(ZipParameter.Zip_Name);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            File.Delete(ZipParameter.Zip_Name);

            using (zipArchive = ZipFile.Open(ZipParameter.Zip_Name, ZipArchiveMode.Create))
            {
                zipArchive.CreateEntryFromFile(ZipParameter.File, Path.GetFileName(ZipParameter.File));

                foreach (var item in ZipParameter.FileList)
                {
                    zipArchive.CreateEntryFromFile(item, Path.GetFileName(item));
                }
            }

            return ZipParameter.Zip_Name;
        }

        /// <summary>
        /// 壓縮返回文件流
        /// </summary>
        /// <returns>Stream 返回壓縮后在文件流</returns>
        public Stream CompressReturnStream()
        {
            MemoryStream zipFileToOpen = new MemoryStream();

            using (ZipArchive archive = new ZipArchive(zipFileToOpen, ZipArchiveMode.Create, true))
            {
                archive.CreateEntryFromFile(ZipParameter.File, Path.GetFileName(ZipParameter.File));

                foreach (var item in ZipParameter.FileList)
                {
                    archive.CreateEntryFromFile(item, Path.GetFileName(item));
                }
            }

            return zipFileToOpen;
        }
        /// <summary>
        /// 解压文件到指定目录
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="rootpath"></param>
        /// <returns></returns>
        public List<string> UnZipFile(Stream fs,string rootpath)
        {
            List<string> rtn = new List<string>();
            fs.Seek(0, SeekOrigin.Begin);
            
            using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Read, false, Encoding.UTF8))
            {
                foreach (var entry in zip.Entries)
                {
                    var tpath = entry.FullName;
                    if (Path.GetFileName(entry.FullName) != "")
                    {
                        tpath = entry.FullName.Replace(Path.GetFileName(entry.FullName), "");
                    }
                    if (!Directory.Exists(Path.Combine(rootpath, tpath)))
                    {
                        Directory.CreateDirectory(Path.Combine(rootpath, tpath));
                    }
                    if (entry.FullName != tpath)
                    {
                        entry.ExtractToFile(Path.Combine(rootpath, entry.FullName), true);
                    }

                    rtn.Add(entry.FullName);
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
