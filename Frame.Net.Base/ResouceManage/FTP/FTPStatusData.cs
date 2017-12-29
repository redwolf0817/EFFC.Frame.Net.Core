using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.ResouceManage.FTP
{
    public class FTPStatusData:DataCollection
    {
        /// <summary>
        /// 传输的总字节数
        /// </summary>
        public long TotalByteLength
        {
            get
            {
                return this["TotalByteLength"] != null ? (long)this["TotalByteLength"] : 0;
            }
            set
            {
                this["TotalByteLength"] = value;
            }
        }
        /// <summary>
        /// 本次传输的字节数
        /// </summary>
        public long CurrentByteLength
        {
            get
            {
                return this["CurrentByteLength"] != null ? (long)this["CurrentByteLength"] : 0;
            }
            set
            {
                this["CurrentByteLength"] = value;
            }
        }
        /// <summary>
        /// 已传输字节数
        /// </summary>
        public long TransferedByteLength
        {
            get
            {
                return this["TransferedByteLength"] != null ? (long)this["TransferedByteLength"] : 0;
            }
            set
            {
                this["TransferedByteLength"] = value;
            }
        }
        /// <summary>
        /// 传输文件的名称
        /// </summary>
        public string FileName
        {
            get
            {
                return ComFunc.nvl(this["FileName"]);
            }
            set
            {
                this["FileName"] = value;
            }
        }
        /// <summary>
        /// 传输速度
        /// </summary>
        public double Speed
        {
            get
            {
                return this["Speed"] != null ? (double)this["Speed"] : 0;
            }
            set
            {
                this["Speed"] = value;
            }
        }
        /// <summary>
        /// 已经消耗的时间
        /// </summary>
        public TimeSpan CostTime
        {
            get
            {
                return this["CostTime"] != null ? (TimeSpan)this["CostTime"] : TimeSpan.Zero;
            }
            set
            {
                this["CostTime"] = value;
            }
        }
        /// <summary>
        /// 调用传入的保留对象
        /// </summary>
        public object ReserveObject
        {
            get
            {
                return this["ReserveObject"];
            }
            set
            {
                this["ReserveObject"] = value;
            }
        }
        /// <summary>
        /// 当前状态
        /// </summary>
        public FtpStaus CurrentStatus
        {
            get
            {
                return (FtpStaus)this["CurrentStatus"];
            }
            set
            {
                this["CurrentStatus"] = value;
            }
        }

        public enum FtpStaus
        {
            /// <summary>
            /// 空状态
            /// </summary>
            None,
            /// <summary>
            /// 正在传输中
            /// </summary>
            Processing,
            /// <summary>
            /// 传输完毕
            /// </summary>
            End
        }
    }
}
