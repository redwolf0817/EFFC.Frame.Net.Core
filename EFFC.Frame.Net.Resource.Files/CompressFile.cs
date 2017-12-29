using EFFC.Frame.Net.Base.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace EFFC.Frame.Net.Resource.Others
{
    /// <summary>
    /// 压缩文件资源处理器
    /// </summary>
    public class CompressFile : IResourceEntity, IDisposable
    {
        string id = "";
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
        /// <param name="toZipFilePath">生成的压缩文件路径</param>
        /// <param name="fromFilePath">来源文件</param>
        /// <param name="otherFiles">其他来源文件</param>
        /// <returns></returns>
        public string CompressReturnFileName(string toZipFilePath,string fromFilePath,params string[] otherFiles)
        {
            ZipArchive zipArchive = null;
            var path = Path.GetDirectoryName(toZipFilePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            File.Delete(toZipFilePath);

            using (zipArchive = ZipFile.Open(toZipFilePath, ZipArchiveMode.Create))
            {
                zipArchive.CreateEntryFromFile(toZipFilePath, Path.GetFileName(fromFilePath));

                foreach (var item in otherFiles)
                {
                    zipArchive.CreateEntryFromFile(item, Path.GetFileName(item));
                }
            }

            return toZipFilePath;
        }

        /// <summary>
        /// 壓縮返回文件流
        /// </summary>
        /// <returns>Stream 返回壓縮后在文件流</returns>
        public Stream CompressReturnStream(string fromFilePath, params string[] otherFiles)
        {
            MemoryStream zipFileToOpen = new MemoryStream();

            using (ZipArchive archive = new ZipArchive(zipFileToOpen, ZipArchiveMode.Create, true))
            {
                archive.CreateEntryFromFile(fromFilePath, Path.GetFileName(fromFilePath));

                foreach (var item in otherFiles)
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
        public List<string> UnZipFile(Stream fs, string rootpath)
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

        #region IResouceEntity 成員
        /// <summary>
        /// 资源释放
        /// </summary>
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
        }

        public string ID
        {
            get { return id; }
        }
    }
}
