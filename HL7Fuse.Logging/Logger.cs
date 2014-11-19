using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Logging;

namespace HL7Fuse.Logging
{
    public class Logger
    {
        #region Private properties
        private static Logger instance=null;
        ILog logger;
        #endregion

        #region Public properties
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                    instance = new Logger();

                return instance;
            }
        }
        #endregion

        #region Constructor
        private Logger()
        {
            Log4NetLogFactory facto = new Log4NetLogFactory();
            // Get the default logger
            logger = facto.GetLog("HL7Fuse");
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Logs the debug message.
        /// 
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Debug(object message)
        {
            Instance.logger.Debug(message);
        }

        /// <summary>
        /// Logs the debug message.
        /// 
        /// </summary>
        /// <param name="message">The message.</param><param name="exception">The exception.</param>
        public static void Debug(object message, Exception exception)
        {
            Instance.logger.Debug(message, exception);
        }

        /// <summary>
        /// Logs the debug message.
        /// 
        /// </summary>
        /// <param name="format">The format.</param><param name="arg0">The arg0.</param>
        public static void DebugFormat(string format, object arg0)
        {
            Instance.logger.DebugFormat(format, arg0);
        }

        /// <summary>
        /// Logs the debug message.
        /// 
        /// </summary>
        /// <param name="format">The format.</param><param name="args">The args.</param>
        public static void DebugFormat(string format, params object[] args)
        {
            Instance.logger.DebugFormat(format, args);
        }

        /// <summary>
        /// Logs the debug message.
        /// 
        /// </summary>
        /// <param name="provider">The provider.</param><param name="format">The format.</param><param name="args">The args.</param>
        public static void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            Instance.logger.DebugFormat(provider, format, args);
        }

        /// <summary>
        /// Logs the error message.
        /// 
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Error(object message)
        {
            Instance.logger.Error(message);
        }

        /// <summary>
        /// Logs the error message.
        /// 
        /// </summary>
        /// <param name="message">The message.</param><param name="exception">The exception.</param>
        public static void Error(object message, Exception exception)
        {
            Instance.logger.Error(message, exception);
        }

        /// <summary>
        /// Logs the error message.
        /// 
        /// </summary>
        /// <param name="format">The format.</param><param name="args">The args.</param>
        public static void ErrorFormat(string format, params object[] args)
        {
            Instance.logger.ErrorFormat(format, args);
        }

        /// <summary>
        /// Logs the error message.
        /// 
        /// </summary>
        /// <param name="provider">The provider.</param><param name="format">The format.</param><param name="args">The args.</param>
        public static void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            Instance.logger.ErrorFormat(provider, format, args);
        }

        /// <summary>
        /// Logs the fatal error message.
        /// 
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Fatal(object message)
        {
            Instance.logger.Fatal(message);
        }

        /// <summary>
        /// Logs the fatal error message.
        /// 
        /// </summary>
        /// <param name="message">The message.</param><param name="exception">The exception.</param>
        public static void Fatal(object message, Exception exception)
        {
            Instance.logger.Fatal(message, exception);
        }

        /// <summary>
        /// Logs the fatal error message.
        /// 
        /// </summary>
        /// <param name="format">The format.</param><param name="arg0">The arg0.</param>
        public static void FatalFormat(string format, object arg0)
        {
            Instance.logger.FatalFormat(format, arg0);
        }

        /// <summary>
        /// Logs the fatal error message.
        /// 
        /// </summary>
        /// <param name="format">The format.</param><param name="args">The args.</param>
        public static void FatalFormat(string format, params object[] args)
        {
            Instance.logger.FatalFormat(format, args);
        }

        /// <summary>
        /// Logs the fatal error message.
        /// 
        /// </summary>
        /// <param name="provider">The provider.</param><param name="format">The format.</param><param name="args">The args.</param>
        public static void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            Instance.logger.FatalFormat(provider, format, args);
        }

        /// <summary>
        /// Logs the info message.
        /// 
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Info(object message)
        {
            Instance.logger.Info(message);
        }

        /// <summary>
        /// Logs the info message.
        /// 
        /// </summary>
        /// <param name="message">The message.</param><param name="exception">The exception.</param>
        public static void Info(object message, Exception exception)
        {
            Instance.logger.Info(message, exception);
        }

        /// <summary>
        /// Logs the info message.
        /// 
        /// </summary>
        /// <param name="format">The format.</param><param name="arg0">The arg0.</param>
        public static void InfoFormat(string format, object arg0)
        {
            Instance.logger.InfoFormat(format, arg0);
        }

        /// <summary>
        /// Logs the info message.
        /// 
        /// </summary>
        /// <param name="format">The format.</param><param name="args">The args.</param>
        public static void InfoFormat(string format, params object[] args)
        {
            Instance.logger.InfoFormat(format, args);
        }

        /// <summary>
        /// Logs the info message.
        /// 
        /// </summary>
        /// <param name="provider">The provider.</param><param name="format">The format.</param><param name="args">The args.</param>
        public static void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            Instance.logger.InfoFormat(provider, format, args);
        }

        /// <summary>
        /// Logs the warning message.
        /// 
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Warn(object message)
        {
            Instance.logger.Warn(message);
        }

        /// <summary>
        /// Logs the warning message.
        /// 
        /// </summary>
        /// <param name="message">The message.</param><param name="exception">The exception.</param>
        public static void Warn(object message, Exception exception)
        {
            Instance.logger.Warn(message, exception);
        }

        /// <summary>
        /// Logs the warning message.
        /// 
        /// </summary>
        /// <param name="format">The format.</param><param name="arg0">The arg0.</param>
        public static void WarnFormat(string format, object arg0)
        {
            Instance.logger.WarnFormat(format, arg0);
        }

        /// <summary>
        /// Logs the warning message.
        /// 
        /// </summary>
        /// <param name="format">The format.</param><param name="args">The args.</param>
        public static void WarnFormat(string format, params object[] args)
        {
            Instance.logger.WarnFormat(format, args);
        }

        /// <summary>
        /// Logs the warning message.
        /// 
        /// </summary>
        /// <param name="provider">The provider.</param><param name="format">The format.</param><param name="args">The args.</param>
        public static void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            Instance.logger.WarnFormat(provider, format, args);
        }
        #endregion
    }
}
