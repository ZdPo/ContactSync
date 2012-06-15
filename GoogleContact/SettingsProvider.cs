using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.IO;

namespace GoogleContact
{
    /// <summary>
    /// Read/write application settings
    /// </summary>
    class SettingsProvider
    {
        private bool _isSaved = true;
        private static SettingsProvider _instace;

        #region Creator / instance / Save /Reload
        private SettingsProvider()
        {
        }
        /// <summary>
        /// Singletone instance
        /// </summary>
        public static SettingsProvider Instance
        {
            get
            {
                if (_instace == null)
                    _instace = new SettingsProvider();
                return _instace;
            }
        }
        /// <summary>
        /// Save current settings
        /// </summary>
        public void Save()
        {
            if (!_isSaved)
                Properties.Settings.Default.Save();
            _isSaved = true;
        }
        /// <summary>
        /// Reload settings
        /// </summary>
        //public void Reload()
        //{
        //    _instace.Reload();
        //    _isSaved = true;
        //}
        #endregion

        #region property

        /// <summary>
        /// Is remember password
        /// </summary>
        public bool IsRemember
        {
            get { return false; }
        }
        /// <summary>
        /// Use local cache for google contact, it's for speedUp read when huge volume of contacts
        /// </summary>
        public bool IsUseGoogleCache
        {
            get { return Properties.Settings.Default.UseGoogleCache; }
            set
            {
                Properties.Settings.Default.UseGoogleCache = value;
                if (!value)
                    Utils.RemoveCacheFile(false);
                _isSaved = false;
            }
        }
        /// <summary>
        /// Use local cache for Outlook contact, it's for speedUp read when huge volume of contacts
        /// </summary>
        public bool IsUseOutlookCache
        {
            get { return Properties.Settings.Default.UseOulookCache; }
            set
            {
                Properties.Settings.Default.UseOulookCache = value;
                if (!value)
                    Utils.RemoveCacheFile(true); 
                _isSaved = false;
            }
        }
        /// <summary>
        /// Maximal time between two synchronization for using cache file.
        /// </summary>
        public int CacheTTL
        {
            get { return Properties.Settings.Default.CacheTTL; }
            set
            {
                Properties.Settings.Default.CacheTTL = value;
                _isSaved = false;
            }
        }
        /// <summary>
        /// Is first Time synchronization
        /// </summary>
        public bool IsFirstTime
        {
            get { return Properties.Settings.Default.IsFirstSetup; }
            set
            {
                Properties.Settings.Default.IsFirstSetup = value;
                _isSaved = false;
            }
        }
        /// <summary>
        /// Get setup for First time synchronization
        /// </summary>
        /// <returns></returns>
        public static Constants.FirstSetupSynchronize FirstSynchronizeGet()
        {

            Constants.FirstSetupSynchronize en = Constants.FirstSetupSynchronize.Outlook2Google;
            try
            {
                en = (Constants.FirstSetupSynchronize)Enum.Parse(typeof(Constants.FirstSetupSynchronize),
                    Properties.Settings.Default.FirstSetup.ToString());
            }
            catch (ArgumentNullException e)
            {
                LoggerProvider.Instance.Logger.Error("Problem in loading configuration");
                LoggerProvider.Instance.Logger.Error(e);
            }
            catch (ArgumentException ee)
            {
                LoggerProvider.Instance.Logger.Error("Problem in loading configuration");
                LoggerProvider.Instance.Logger.Error(ee);
            }
            catch (OverflowException eee)
            {
                LoggerProvider.Instance.Logger.Error("Problem in loading configuration");
                LoggerProvider.Instance.Logger.Error(eee);
            }
            return en;
        }
        /// <summary>
        /// Set value for First time synchronization
        /// </summary>
        /// <param name="setValue"></param>
        public void FirstSynchronizeSet(Constants.FirstSetupSynchronize setValue)
        {
            int v = Convert.ToInt32(setValue);
            Properties.Settings.Default.FirstSetup = v;
            _isSaved = false;
        }
        /// <summary>
        /// Get standard synchronization way
        /// </summary>
        /// <returns></returns>
        public static Constants.SetupSynchronize SynchronizeDirectionGet()
        {
            Constants.SetupSynchronize en = Constants.SetupSynchronize.Outlook2Google;
            try
            {
                en = (Constants.SetupSynchronize)Enum.Parse(typeof(Constants.SetupSynchronize), Properties.Settings.Default.SyncDirection.ToString());
            }
            catch (ArgumentNullException e)
            {
                LoggerProvider.Instance.Logger.Error("Problem in loading configuration");
                LoggerProvider.Instance.Logger.Error(e);
            }
            catch (ArgumentException ee)
            {
                LoggerProvider.Instance.Logger.Error("Problem in loading configuration");
                LoggerProvider.Instance.Logger.Error(ee);
            }
            catch (OverflowException eee)
            {
                LoggerProvider.Instance.Logger.Error("Problem in loading configuration");
                LoggerProvider.Instance.Logger.Error(eee);
            } 
            return en;
        }
        /// <summary>
        /// Set standard synchronization way
        /// </summary>
        /// <param name="setValue"></param>
        public void SynchronizeDirectionSet(Constants.SetupSynchronize setValue)
        {
            int v = Convert.ToInt32(setValue);
            Properties.Settings.Default.SyncDirection = v;
            _isSaved = false;
        }
        #endregion

        #region Rigt way for work by setup data
        /// <summary>
        /// Is now setup for adding data to Google
        /// </summary>
        public bool IsAddToGoogle
        {
            get { return ToGoogle; }
        }
        /// <summary>
        /// Is now setup for adding data to Outlook
        /// </summary>
        public bool IsAddToOutlook
        {
            get { return ToOutlook; }
        }
        /// <summary>
        /// Is in this Settings relevant delete contact from Google
        /// </summary>
        public bool IsDeleteFromGoogle
        {
            get { return ToGoogle; }
        }
        /// <summary>
        /// Is in this Settings relevant delete contact from Outlook
        /// </summary>
        public bool IsDeleteFromOutlook
        {
            get { return ToOutlook; }
        }
        /// <summary>
        /// Update data on google
        /// </summary>
        public bool IsUpdateToGoogle
        {
            get { return ToGoogle; }
        }
        /// <summary>
        /// Update data on Outlook
        /// </summary>
        public bool IsUpdateToOutlook
        {
            get { return ToOutlook; }
        }

        #region private function
        private bool ToGoogle
        {
            get
            {
                bool ret = true;
                if (IsFirstTime)
                    switch (FirstSynchronizeGet())
                    {
                        case Constants.FirstSetupSynchronize.Google2Outlook:
                            ret = false;
                            break;
                        case Constants.FirstSetupSynchronize.Outlook2Google:
                        case Constants.FirstSetupSynchronize.SynchronizeBoth:
                        default:
                            ret = true;
                            break;
                    }
                else
                {
                    switch (SynchronizeDirectionGet())
                    {
                        case Constants.SetupSynchronize.Google2Outlook:
                            ret = false;
                            break;
                        case Constants.SetupSynchronize.Outlook2Google:
                        case Constants.SetupSynchronize.SynchronizeBothSide:
                        default:
                            ret = true;
                            break;
                    }
                }
                return ret;
            }
        }
        
        private bool ToOutlook
        {
            get
            {
                bool ret = false;
                if (IsFirstTime)
                    switch (FirstSynchronizeGet())
                    {
                        case Constants.FirstSetupSynchronize.Outlook2Google:
                            ret = false;
                            break;
                        case Constants.FirstSetupSynchronize.Google2Outlook:
                        case Constants.FirstSetupSynchronize.SynchronizeBoth:
                        default:
                            ret = true;
                            break;
                    }
                else
                {
                    switch (SynchronizeDirectionGet())
                    {
                        case Constants.SetupSynchronize.Outlook2Google:
                            ret = false;
                            break;
                        case Constants.SetupSynchronize.Google2Outlook:
                        case Constants.SetupSynchronize.SynchronizeBothSide:
                        default:
                            ret = true;
                            break;
                    }
                }
                return ret;
            }
        }
        #endregion

        #endregion

        #region Logging setup
        /// <summary>
        /// Where save Log files
        /// </summary>
        public string LogFile
        {
            get
            {
                if (string.IsNullOrEmpty(Properties.Settings.Default.LogFile))
                    Properties.Settings.Default.LogFile = Path.GetTempPath();
                        //Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Properties.Settings.Default.LogFile;
            }
            set
            {
                Properties.Settings.Default.LogFile = value;
                _isSaved = false;
            }
        }
        /// <summary>
        /// Get current Log level
        /// </summary>
        /// <returns></returns>
        public static Constants.LogLevel LogLevelGet()
        {
            Constants.LogLevel en = Constants.LogLevel.Fatal;
            try
            {
                en = (Constants.LogLevel)Enum.Parse(typeof(Constants.LogLevel), Properties.Settings.Default.LogLevel.ToString());
            }
            catch (ArgumentNullException e)
            {
                LoggerProvider.Instance.Logger.Error("Problem in loading configuration");
                LoggerProvider.Instance.Logger.Error(e);
            }
            catch (ArgumentException ee)
            {
                LoggerProvider.Instance.Logger.Error("Problem in loading configuration");
                LoggerProvider.Instance.Logger.Error(ee);
            }
            catch (OverflowException eee)
            {
                LoggerProvider.Instance.Logger.Error("Problem in loading configuration");
                LoggerProvider.Instance.Logger.Error(eee);
            }
            return en;
        }
        /// <summary>
        /// Save new loglevel value
        /// </summary>
        /// <param name="setValue"></param>
        public void LogLevelSet(Constants.LogLevel setValue)
        {
            int v = Convert.ToInt32(setValue);
            Properties.Settings.Default.LogLevel = v;
            _isSaved = false;
        }
        #endregion
    }
}
