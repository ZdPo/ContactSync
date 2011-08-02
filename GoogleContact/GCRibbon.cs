using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.Windows.Forms;

using Google.GData.Client;

namespace GoogleContact
{
    public partial class GCRibbon
    {
        private Synchronizer synchr;
        /// <summary>
        /// Uses for last statistic
        /// </summary>
        private LastStatistic lastStatistic = new LastStatistic();
        
        /// <summary>
        /// Setup data when load ribbon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GCRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            LoggerProvider.Instance.Logger.Debug("GCRibbon_Load(object sender, RibbonUIEventArgs e)");
            if (SettingsProvider.Instance.UserName.Length > 0)
                btnSync.Enabled = true;
            else
                btnSync.Enabled = false;
        }

        #region Button click action
        /// <summary>
        /// Configure window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfigure_Click(object sender, RibbonControlEventArgs e)
        {
            using (ConnectionSettings c = new ConnectionSettings())
            {
                LoggerProvider.Instance.Logger.Debug("Click on ConnectionSettings");
                c.ShowDialog();
                lastStatistic.Clear();
            }
            if (SettingsProvider.Instance.UserName.Length > 0)
                btnSync.Enabled = true;
            else
                btnSync.Enabled = false;
        }

        /// <summary>
        /// Synchronize data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSync_Click(object sender, RibbonControlEventArgs e)
        {
            ///Synchronizer synchr = new Synchronizer(ref lastStatistic);
            if (synchr == null)
                synchr = new Synchronizer(ref lastStatistic);
            LoggerProvider.Instance.Logger.Debug("Click on Synchronize button");
            if ((SettingsProvider.Instance.UserPassword.Length == 0) || !SettingsProvider.Instance.IsRemember)
            {
                using (AuthenticateRequest ar = new AuthenticateRequest())
                {
                    ar.UserName = SettingsProvider.Instance.UserName;
                    ar.ShowDialog();
                    if (!ar.IsAccept)
                        return;
                    GoogleProvider.GetProvider.Logon(SettingsProvider.Instance.UserName, ar.Password);
                }
            }
            synchr.SetupSync();
            try
            {
                synchr.Synchronize();
            }
            ///TODO: Need change exception type to catch
            catch (GDataRequestException ge)
            {
                MessageBox.Show("Problem in connect to Google site. Detail data is in Log file.", "Synchronize to Google",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                LoggerProvider.Instance.Logger.Error(ge);
            }
            catch (CaptchaRequiredException ca)
            {
                MessageBox.Show("Problem in connect to Google site. Google required CAPTCHA autorization. Detail data is in Log file.", "Synchronize to Google",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                LoggerProvider.Instance.Logger.Error(ca);
            }
            catch (NullReferenceException ne)
            {
                MessageBox.Show("Problem in connect to Google site. Problem when read Google.Contacts.Contact Feed. Detail data is in Log file.", "Synchronize to Google",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                LoggerProvider.Instance.Logger.Error(ne);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem in synchronize. Data is in Log file.", "Synchronize to Google",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                LoggerProvider.Instance.Logger.Error(ex);
            }
            LoggerProvider.Instance.Logger.Debug("Statistic after synchronize\r\n{0}", lastStatistic.StatisticString());
            /// in next release don't dispose because use memored Outlook and Google contacts for better speed in update
            synchr.Dispose();
            synchr = null;
            MessageBox.Show(lastStatistic.StatisticString(), "Statistic", 
                MessageBoxButtons.OK, MessageBoxIcon.Information,MessageBoxDefaultButton.Button1,MessageBoxOptions.ServiceNotification);
        }

        /// <summary>
        /// Show About windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbout_Click(object sender, RibbonControlEventArgs e)
        {
            using (About a = new About())
            {
                LoggerProvider.Instance.Logger.Debug("Click on About");
                a.ShowDialog();
            }
        }
        #endregion
    }
}
