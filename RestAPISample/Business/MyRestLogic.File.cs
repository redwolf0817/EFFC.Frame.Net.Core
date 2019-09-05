using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using EFFC.Frame.Net.Resource.Sqlite;
using EFFC.Frame.Net.Resource.SQLServer;
using EFFC.Frame.Net.Unit.DB;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static EFFC.Frame.Net.Module.Extend.EWRA.Logic.RestLogic;

namespace RestAPISample.Business
{
    public partial class MyRestLogic
    {
        MyFileHelper _file;
        /// <summary>
        /// db操作相关
        /// </summary>
		public MyFileHelper FileHelper
        {
            get
            {
                if (_file == null)
                    _file = new MyFileHelper(this);

                return _file;
            }
        }

        public class MyFileHelper
        {
            string upload_root_path = MyConfig.GetConfiguration("Upload", "File_Path");
            int upload_max_size = IntStd.IsNotIntThen(MyConfig.GetConfiguration("Upload", "Max_Size"), 10);
            MyRestLogic _logic = null;

            public MyFileHelper(MyRestLogic logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// 上传文件根目录
            /// </summary>
            public string Upload_Root_Path
            {
                get
                {
                    return upload_root_path;
                }
            }
            /// <summary>
            /// 上传文件最大Size
            /// </summary>
            public int Upload_Max_Size
            {
                get
                {
                    return upload_max_size;
                }
            }
            /// <summary>
            /// 上传文件
            /// </summary>
            /// <param name="file_name"></param>
            /// <param name="file_length"></param>
            /// <param name="file_content"></param>
            /// <param name="is_keep_filename"></param>
            /// <returns></returns>
            public object DoUploadFile(string file_name, long file_length, string file_content, bool is_keep_filename)
            {
                if (file_content == "")
                {
                    return new
                    {
                        code = "failed",
                        msg = "文件内容不可为空"
                    };
                }
                if (!ComFunc.IsBase64(file_content))
                {
                    return new
                    {
                        code = "failed",
                        msg = "文件内容格式不正确"
                    };
                }
                if (file_length > upload_max_size * 1024 * 1024)
                {
                    return new
                    {
                        code = "failed",
                        msg = $"上传文件不可超过{upload_max_size}MB"
                    };
                }
                var bytes = ComFunc.Base64DeCodeToByte(file_content);
                var save_file_name = $"{Path.GetFileNameWithoutExtension(file_name)}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}{Path.GetExtension(file_name)}";
                var relative_path = $"~/{DateTime.Now.ToString("yyyyMMdd")}/";
                var savepath = relative_path.Replace("~", upload_root_path);
                if (!Directory.Exists(savepath))
                {
                    Directory.CreateDirectory(savepath);
                }
                File.WriteAllBytes($"{savepath}/{save_file_name}", bytes);

                return new
                {
                    code = "success",
                    msg = "上传成功",
                    upload_path = $"{relative_path}/{save_file_name}".Replace("//", "/")
                };
            }
            /// <summary>
            /// 下载文件
            /// </summary>
            /// <param name="path"></param>
            /// <returns>{
            /// code:'success-成功，failed-失败',
            /// msg:'提示信息',
            /// filetype:'文件的content-type类型'
            /// filename:'文件名称',
            /// filelength:'文件长度',
            /// file:'文件内容，采用base64加密'
            /// }</returns>
            public object DoDownLoad(string path)
            {
                path = path.Replace("~", upload_root_path);
                if (!File.Exists(path))
                {
                    return new
                    {
                        code = "failed",
                        msg = "文件不存在"
                    };
                }
                var filecontent = File.ReadAllBytes(path);
                var width = "";
                var height = "";
                if(new string[] { "jpg", "png", "bmp", "gif","jpeg" }.Contains(Path.GetExtension(path).Replace(".", "").ToLower()))
                {
                    using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
                    {
                        width = image.Width.ToString();
                        height = image.Height.ToString();
                    }
                }
                return new
                {
                    code = "success",
                    msg = "",
                    filetype = ResponseHeader_ContentType.Map(Path.GetExtension(path).ToLower().Replace(".", "")),
                    filename = Path.GetFileName(path),
                    filelength = filecontent.Count(),
                    file = ComFunc.Base64Code(filecontent),
                    pic_width = width,
                    pic_heigth = height
                };
            }
            /// <summary>
            /// 删除上传的文件
            /// </summary>
            /// <param name="file_path"></param>
            /// <returns></returns>
            public bool DelUploadFile(string file_path)
            {
                if (string.IsNullOrEmpty(file_path)) return true;
                var path = file_path.Replace("~", upload_root_path);
                if (!File.Exists(path))
                {
                    return true;
                }

                File.Delete(path);
                return true;
            }
            /// <summary>
            /// 拷贝文件到指定目录
            /// </summary>
            /// <param name="from_file_path"></param>
            /// <param name="to_path"></param>
            /// <returns></returns>
            public string CopyUploadFileTo(string from_file_path, string to_path)
            {
                if (string.IsNullOrEmpty(from_file_path)) return "";
                var path = from_file_path.Replace("~", upload_root_path);
                if (!File.Exists(path))
                {
                    return "";
                }
                var to = to_path.Replace("~", upload_root_path);
                if (!Directory.Exists(Path.GetDirectoryName(to)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(to));
                }
                var destinypath = $"{to}/{Path.GetFileName(from_file_path)}".Replace("//", "/");
                if (!File.Exists(destinypath))
                {
                    File.Copy(path, destinypath, false);
                }

                return $"{to_path}/{Path.GetFileName(from_file_path)}".Replace("//", "/");
            }
            /// <summary>
            /// 上传文件是否存在
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public bool IsExists(string path)
            {
                if (string.IsNullOrEmpty(path)) return false;
                var tmp_path = path.Replace("~", upload_root_path);
                return File.Exists(tmp_path);
            }
        }
    }
}
