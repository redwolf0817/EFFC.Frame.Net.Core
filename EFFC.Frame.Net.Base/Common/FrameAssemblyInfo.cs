using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace Frame.Net.Base.Common
{
    /// <summary>
    /// 获取框架的版本信息
    /// </summary>
    public class FrameAssemblyInfo
    {
        Type _t = null;

        string effc_version = "";
        string effc_product_version = "";
        /// <summary>
        /// 创建一个框架版本信息的对象
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static FrameAssemblyInfo From(Type t)
        {
            return new FrameAssemblyInfo(t);
        }
        public FrameAssemblyInfo(Type t)
        {
            _t = t;
        }
        /// <summary>
        /// 程式版本号
        /// </summary>
        public string Version
        {
            get
            {
                if(effc_version == "")
                {
                    effc_version = _t.GetTypeInfo().Assembly.GetName().Version.ToString();
                }
                return effc_version;
            }
        }
        /// <summary>
        /// 包版本号
        /// </summary>
        public string ProductVersion
        {
            get
            {
                if (effc_product_version == "")
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(_t.GetTypeInfo().Assembly.Location);
                    effc_product_version = fvi.ProductVersion;
                }
                return effc_product_version;
            }
        }

    }
}
