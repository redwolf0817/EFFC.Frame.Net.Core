using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Parameter;

namespace EFFC.Frame.Net.Base.ResouceManage.Files
{
    public class ZipParameter : ParameterStd
    {
        public ZipParameter()
        {
            Zip_Name = "";
            DirectoryName= "";
            File="";
            FileList = new List<string>();
            Password="";
            Level=6;
        }
        /// <summary>
        /// 壓縮后的文件名稱
        /// </summary>
        public string Zip_Name
        {
            get { return GetValue<string>("Zip_Name"); }
            set { SetValue("Zip_Name", value); }
        }

        /// <summary>
        /// 被壓縮的目錄名稱
        /// </summary>
        public string DirectoryName
        {
            get { return GetValue<string>("DirectoryName"); }
            set { SetValue("DirectoryName", value); }
        }

        /// <summary>
        /// 單個文件
        /// </summary>
        public string File
        {
            get { return GetValue<string>("File"); }
            set { SetValue("File", value); }
        }

        /// <summary>
        /// 備壓縮的文件List
        /// </summary>
        public List<string> FileList
        {
            get { return GetValue<List<string>>("FileList"); }
            set { SetValue("FileList", value); }
        }

        /// <summary>
        /// 加密密碼
        /// </summary>
        public string Password
        {
            get { return GetValue<string>("Password"); }
            set { SetValue("Password", value); }
        }
        /// <summary>
        /// 壓縮比例等級，默認為6，0 - store only to 9 - means best compression
        /// </summary>
        public int Level
        {
            get { return GetValue<int>("Level"); }
            set { SetValue("Level", value); }
        }
    }
}
