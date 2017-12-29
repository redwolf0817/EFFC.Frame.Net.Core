using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Text;

namespace Frame.Net.Base.Common
{
    public class EFFCAssemblyInfo
    {
        static string effc_version = "";
        static string effc_product_version = "";
        /// <summary>
        /// 程式版本号
        /// </summary>
        public static string Version
        {
            get
            {
                if(effc_version == "")
                {
                    effc_version = typeof(EFFCAssemblyInfo).GetTypeInfo().Assembly.GetName().Version.ToString();
                }
                return effc_version;
            }
        }
        /// <summary>
        /// 包版本号
        /// </summary>
        public static string ProductVersion
        {
            get
            {
                if (effc_product_version == "")
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(typeof(EFFCAssemblyInfo).GetTypeInfo().Assembly.Location);
                    effc_product_version = fvi.ProductVersion;
                }
                return effc_product_version;
            }
        }

    }
}
