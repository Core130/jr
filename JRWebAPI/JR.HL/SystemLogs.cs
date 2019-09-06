using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.HL
{
    /// <summary>
    /// 系统日志
    /// 各级别的固定配置项为(不能配错);
    /// Info->InfoLogFile
    /// Fatal->FatalLogFile
    /// Error->ErrorLogFile
    /// Debug->DebugLogFile
    /// Warn->WarnLogFile
    /// </summary>
    public class SystemLogs
    {
        private static string strInfoLogConfig = "InfoLogFile";
       // private static string strFatalLogConfig = "FatalLogFile";
        private static string strErrorLogConfig = "ErrorLogFile";
       // private static string strDebugLogConfig = "DebugLogFile";
       // private static string strWarnLogConfig = "WarnLogFile";

        /// <summary>
        /// 初始化log4net
        /// </summary>
        static SystemLogs()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private static ILog getLog(string strConfig)
        {
            return LogManager.GetLogger(strConfig);
        }

        /// <summary>
        /// 将提示信息输出到文本日志文件中
        /// </summary>
        /// <param name="message"></param>
        public static void WriteInfo(object message)
        {
            getLog(strInfoLogConfig).Info(message);
        }
        /// <summary>
        /// 将提示信息输出到文本日志文件中
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void WriteInfo(object message, Exception exception)
        {
            getLog(strInfoLogConfig).Info(message, exception);
        }
        /// <summary>
        /// 将错误信息输出到文本日志文件中
        /// </summary>
        /// <param name="message"></param>
        public static void WriteError(object message)
        {
            getLog(strErrorLogConfig).Error(message);
        }
        /// <summary>
        /// 将错误信息输出到文本日志文件中
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void WriteError(object message, Exception exception)
        {
            getLog(strErrorLogConfig).Error(message, exception);
        }
    }
}
