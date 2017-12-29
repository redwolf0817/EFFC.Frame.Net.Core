using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Interfaces.System;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.WebGo.Log
{
    /// <summary>
    /// Log4Net为框架提供一个基础的log实现
    /// </summary>
    public class Log4Net : ILogger
    {
        ILoggerRepository repository = null;
        log4net.ILog logInfo = null;
        log4net.ILog logDebug = null;
        log4net.ILog logError = null;
        log4net.ILog logFatal = null;
        log4net.ILog logWarn = null;

        public Log4Net() : this("log4net.config")
        {
        }

        public Log4Net(string configpath)
        {
            repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo(configpath));
        }

        private void Init()
        {
            logInfo = log4net.LogManager.GetLogger(repository.Name, "InfoLog");
            logDebug = log4net.LogManager.GetLogger(repository.Name, "DebugLog");
            logError = log4net.LogManager.GetLogger(repository.Name, "ErrorLog");
            logFatal = log4net.LogManager.GetLogger(repository.Name, "FatalLog");
            logWarn = log4net.LogManager.GetLogger(repository.Name, "WarnLog");
        }
        /// <summary>
        /// 写入Log
        /// </summary>
        /// <param name="msg"></param>
        public void WriteLog(string msg)
        {
            Init();
            if (logError.IsErrorEnabled)
            {
                logError.Error(msg);
            }
            logError = null;

        }

        public void debug(string message)
        {
            Init();
            if (logDebug.IsDebugEnabled)
            {
                logDebug.Debug(message);
            }
            logDebug = null;
        }
        public void error(string message)
        {
            Init();
            if (logError.IsErrorEnabled)
            {
                logError.Error(message);
            }
            logError = null;
        }
        public void fatal(string message)
        {
            Init();
            if (logFatal.IsFatalEnabled)
            {
                logFatal.Fatal(message);
            }
            logFatal = null;
        }
        public void info(string message)
        {
            Init();
            if (logInfo.IsInfoEnabled)
            {
                logInfo.Info(message);
            }
            logInfo = null;
        }
        public void warn(string message)
        {
            Init();
            if (logWarn.IsWarnEnabled)
            {
                logWarn.Warn(message);
            }
            logWarn = null;
        }

        #region ILogger 成員

        public void WriteLog(LoggerLevel level, string message)
        {
            switch (level)
            {
                case LoggerLevel.DEBUG:
                    this.debug(message);
                    break;
                case LoggerLevel.ERROR:
                    this.error(message);
                    break;
                case LoggerLevel.FATAL:
                    this.fatal(message);
                    break;
                case LoggerLevel.INFO:
                    this.info(message);
                    break;
                case LoggerLevel.WARN:
                    this.warn(message);
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
