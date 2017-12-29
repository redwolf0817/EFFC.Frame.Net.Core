using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Interfaces.System;

namespace EFFC.Frame.Net.Web.Log
{
    public class Log4Net : ILogger
    {
        log4net.ILog logInfo = log4net.LogManager.GetLogger("InfoLog");
        log4net.ILog logDebug = log4net.LogManager.GetLogger("DebugLog");
        log4net.ILog logError = log4net.LogManager.GetLogger("ErrorLog");
        log4net.ILog logFatal = log4net.LogManager.GetLogger("FatalLog");
        log4net.ILog logWarn = log4net.LogManager.GetLogger("WarnLog");

        public Log4Net()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(HttpContext.Current.Server.MapPath("Log4Net.config")));
        }

        public Log4Net(string configpath)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(configpath));
        }

        private void Init()
        {
            logInfo = log4net.LogManager.GetLogger("InfoLog");
            logDebug = log4net.LogManager.GetLogger("DebugLog");
            logError = log4net.LogManager.GetLogger("ErrorLog");
            logFatal = log4net.LogManager.GetLogger("FatalLog");
            logWarn = log4net.LogManager.GetLogger("WarnLog");
        }
        /// <summary>
        /// ?庹
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
