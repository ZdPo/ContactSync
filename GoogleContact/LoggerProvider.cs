using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NLog;
using NLog.Targets;
using NLog.Config;

namespace GoogleContact
{
    /// <summary>
    /// This class write logs file
    /// </summary>
    class LoggerProvider : IDisposable
    {
        private static LoggerProvider _instance;
        private string _LogDirectory = "";
        private Constants.LogLevel _Level = Constants.LogLevel.Fatal;
        private Logger _logger;
        FileTarget fileTarget;

        private LoggerProvider()
        {
            _LogDirectory = SettingsProvider.Instance.LogFile;
            _Level = SettingsProvider.LogLevelGet();

            LoggingConfiguration _LogConfiguration = new LoggingConfiguration();
            fileTarget = new FileTarget();
            _LogConfiguration.AddTarget("file", fileTarget);
            fileTarget.FileName = string.Format("{0}/{1}", _LogDirectory, "${shortdate}.log");
            fileTarget.Layout = "${longdate}|${level:uppercase=true}|${callsite:className=true:fileName=true:includeSourcePath=false:methodName=true}|${message}";
            LoggingRule rule2 = new LoggingRule("*", GetLevel(_Level), fileTarget);
            _LogConfiguration.LoggingRules.Add(rule2);
            LogManager.Configuration = _LogConfiguration;
            _logger = LogManager.GetCurrentClassLogger();
        }

        protected virtual void Dispose(bool disposing)
        {
            if ((disposing) && (fileTarget!=null))
            {
                // dispose managed resources
                fileTarget.Dispose();
                fileTarget = null;
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Convert my level to NLOG
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static LogLevel GetLevel(Constants.LogLevel level)
        {
            LogLevel ret = LogLevel.Fatal;
            switch (level)
            {
                case Constants.LogLevel.None:
                    ret = LogLevel.Off;
                    break;
                case Constants.LogLevel.Fatal:
                    ret = LogLevel.Fatal;
                    break;
                case Constants.LogLevel.Error:
                    ret = LogLevel.Error;
                    break;
                case Constants.LogLevel.Warning:
                    ret = LogLevel.Warn;
                    break;
                case Constants.LogLevel.Debug:
                    ret = LogLevel.Debug;
                    break;
                default:
                    ret = LogLevel.Fatal;
                    break;
            }
            return ret;
        }
        /// <summary>
        /// Get singletone class
        /// </summary>
        public static LoggerProvider Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LoggerProvider();
                return _instance;
            }
        }
        /// <summary>
        /// Reread settings
        /// </summary>
        public void ReloadSettings()
        {
            LoggingConfiguration _LogConfiguration = LogManager.Configuration;
            if (!string.Equals(_LogDirectory,SettingsProvider.Instance.LogFile))
            {
                _LogDirectory = SettingsProvider.Instance.LogFile;
                _LogConfiguration.RemoveTarget("file");
                fileTarget = new FileTarget();
                _LogConfiguration.AddTarget("file", fileTarget);

                //consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
                fileTarget.FileName = string.Format("{0}/${shortdate}.log", _LogDirectory);
                fileTarget.Layout = "${time}|${level:uppercase=true}|${callsite:className=true:fileName=true:includeSourcePath=true:methodName=true}|${message}";
            }
            if (!Enum.Equals(_Level, SettingsProvider.LogLevelGet()))
            {
                _Level = SettingsProvider.LogLevelGet();
                _LogConfiguration.LoggingRules.RemoveAt(0);
                LoggingRule rule2 = new LoggingRule("*", GetLevel(_Level), fileTarget);
                _LogConfiguration.LoggingRules.Add(rule2);
            }
            _logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Actual confugure logger
        /// </summary>
        public Logger Logger
        {
            get { return _logger; }
        }
    }
}
