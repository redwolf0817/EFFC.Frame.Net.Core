using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.Data.Base;

namespace EFFC.Frame.Net.Tag.Core
{
    public class TagParameter:ParameterStd
    {
        public TagParameter()
        {
            SetValue("__bind_object", FrameDLRObject.CreateInstance());
        }
        /// <summary>
        /// 待解析文本
        /// </summary>
        public string Text
        {
            get
            {
                return GetValue<string>("__Text");
            }
            set
            {
                SetValue("__Text", value);
            }
        }
        /// <summary>
        /// rootpath对应的物理路径
        /// </summary>
        public string RootPath
        {
            get
            {
                return GetValue<string>("__rootpath");
            }
            set
            {
                SetValue("__rootpath", value);
            }
        }
        /// <summary>
        /// 公共库的物理路径
        /// </summary>
        public string CommonLibPath
        {
            get
            {
                return GetValue<string>("__commonpath");
            }
            set
            {
                SetValue("__commonpath", value);
            }
        }
        /// <summary>
        /// 运行时库的物理路径
        /// </summary>
        public string RunTimeLibPath
        {
            get
            {
                return GetValue<string>("__runtimepath");
            }
            set
            {
                SetValue("__runtimepath", value);
            }
        }
        /// <summary>
        ///  待绑定的对象
        /// </summary>
        public FrameDLRObject BindObject
        {
            get
            {
                return (FrameDLRObject)GetValue("__bind_object");
            }
        }
    }
}
