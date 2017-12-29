using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Base.Constants
{
    /// <summary>
    /// 序列化类型
    /// </summary>
    public enum SerializableType
    {
        /// <summary>
        /// 二进制序列化
        /// </summary>
        Binary,
        /// <summary>
        /// xml序列化
        /// </summary>
        Xml
    }
    
    /// <summary>
    /// Go请求下的响应数据类型
    /// </summary>
    public enum GoResponseDataType
    {
        /// <summary>
        /// 无指定类型，系统会将返回结果集直接response掉
        /// </summary>
        NONE,
        /// <summary>
        /// Json
        /// </summary>
        Json,
        /// <summary>
        /// Html
        /// </summary>
        HTML,
        /// <summary>
        /// 字符串
        /// </summary>
        String,
        /// <summary>
        /// Jpg图片
        /// </summary>
        Pic_Jpg,
        /// <summary>
        /// Bmp图片
        /// </summary>
        Pic_Bmp,
        /// <summary>
        /// Gif图片
        /// </summary>
        Pic_Gif,
        /// <summary>
        /// png图片
        /// </summary>
        Pic_png,
        /// <summary>
        /// Excel
        /// </summary>
        Excel,
        /// <summary>
        /// Word
        /// </summary>
        Word,
        /// <summary>
        /// PDF
        /// </summary>
        PDF,
        /// <summary>
        /// HostView
        /// </summary>
        HostView,
        /// <summary>
        /// RazorView
        /// </summary>
        RazorView
    }
    /// <summary>
    /// Log Level
    /// </summary>
    public enum LoggerLevel
    {
        /// <summary>
        /// Debug
        /// </summary>
        DEBUG,
        /// <summary>
        /// Info
        /// </summary>
        INFO,
        /// <summary>
        /// Warning
        /// </summary>
        WARN,
        /// <summary>
        /// Error
        /// </summary>
        ERROR,
        /// <summary>
        /// Fatal error
        /// </summary>
        FATAL
    }
    /// <summary>
    /// DB状态
    /// </summary>
    [Serializable]
    public enum DBStatus
    {
        /// <summary>
        /// 开启
        /// </summary>
        Open,
        /// <summary>
        /// 关闭
        /// </summary>
        Close,
        /// <summary>
        /// 开启事务
        /// </summary>
        Begin_Trans,
        /// <summary>
        /// 提交事务
        /// </summary>
        Commit_Trans,
        /// <summary>
        /// 回滚事务
        /// </summary>
        RollBack_Trans,
        /// <summary>
        /// 空
        /// </summary>
        Empty
    }
    /// <summary>
    /// 状态定义，Rule专用
    /// </summary>
    public enum StateDefine
    {
        /// <summary>
        /// 开始
        /// </summary>
        Start,
        /// <summary>
        /// 处理中
        /// </summary>
        Processing,
        /// <summary>
        /// 结束
        /// </summary>
        End
    }
    /// <summary>
    /// Frame动态对象控制标记
    /// </summary>
    [Flags]
    public enum FrameDLRFlags
    {
        /// <summary>
        /// None
        /// </summary>
        None = 1,
        /// <summary>
        /// 敏感大小写
        /// </summary>
        SensitiveCase = 2
        

    }

    /// <summary>
    /// Transaction隔离级别
    /// </summary>
    public enum FrameIsolationLevel
    {
        /// <summary>
        /// 默认级别，由各个事务自定决定
        /// </summary>
        Default = 0,
        /// <summary>
        /// 正在使用与指定隔离级别不同的隔离级别，但是无法确定该级别。
        /// </summary>
        Unspecified = -1,
        /// <summary>
        /// 无法覆盖隔离级别更高的事务中的挂起的更改。
        /// </summary>
        Chaos = 16,
        /// <summary>
        /// 可以进行脏读，意思是说，不发布共享锁，也不接受独占锁。
        /// </summary>
        ReadUncommitted = 256,
        /// <summary>
        /// 在正在读取数据时保持共享锁，以避免脏读，但是在事务结束之前可以更改数据，从而导致不可重复的读取或幻像数据。
        /// </summary>
        ReadCommitted = 4096,
        /// <summary>
        /// 在查询中使用的所有数据上放置锁，以防止其他用户更新这些数据。 防止不可重复的读取，但是仍可以有幻像行。
        /// </summary>
        RepeatableRead = 65536,
        /// <summary>
        /// 在 System.Data.DataSet 上放置范围锁，以防止在事务完成之前由其他用户更新行或向数据集中插入行。
        /// </summary>
        Serializable = 1048576,
        /// <summary>
        /// 通过在一个应用程序正在修改数据时存储另一个应用程序可以读取的相同数据版本来减少阻止。 表示您无法从一个事务中看到在其他事务中进行的更改，即便重新查询也是如此。
        /// </summary>
        Snapshot = 16777216,
    }
    /// <summary>
    /// 流程状态
    /// </summary>
    public enum FlowStateType
    {
        /// <summary>
        /// 无效状态
        /// </summary>
        None,
        /// <summary>
        /// 准备执行
        /// </summary>
        Ready,
        /// <summary>
        /// 中断，挂起
        /// </summary>
        Suspend,
        /// <summary>
        /// 结束
        /// </summary>
        End
    }
}
