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
    class LoggerProvider
    {
        private static LoggerProvider _instance = null;
        private string _LogDirectory = "";
        private Constants.LogLevels _Level = Constants.LogLevels.Fatal;
        private Logger _logger = null;
        FileTarget fileTarget = null;

        private LoggerProvider()
        {
            _LogDirectory = SettingsProvider.Instance.LogFile;
            _Level = SettingsProvider.Instance.LogLevelGet();

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
        /// <summary>
        /// Convert my level to NLOG
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private LogLevel GetLevel(Constants.LogLevels level)
        {
            LogLevel ret = LogLevel.Fatal;
            switch (level)
            {
                case Constants.LogLevels.None:
                    ret = LogLevel.Off;
                    break;
                case Constants.LogLevels.Fatal:
                    ret = LogLevel.Fatal;
                    break;
                case Constants.LogLevels.Error:
                    ret = LogLevel.Error;
                    break;
                case Constants.LogLevels.Warning:
                    ret = LogLevel.Warn;
                    break;
                case Constants.LogLevels.Debug:
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
            if (!Enum.Equals(_Level, SettingsProvider.Instance.LogLevelGet()))
            {
                _Level = SettingsProvider.Instance.LogLevelGet();
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
