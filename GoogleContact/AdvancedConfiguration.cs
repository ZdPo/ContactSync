using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;


namespace GoogleContact
{
    public partial class AdvancedConfiguration : Form
    {
        private BackgroundWorker bw = new BackgroundWorker();

        public AdvancedConfiguration()
        {
            InitializeComponent();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = false;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
//            pBar.Maximum = 100;
//            pBar.Maximum = 0;
            pBar.Step = 1;
            pBar.Value = 1;
        }


        /// <summary>
        /// Save current settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            SettingsProvider.Instance.IsUseOutlookCache = chUseCacheOutlook.Checked;
            SettingsProvider.Instance.IsUseGoogleCache = chUseCacheGoogle.Checked;
            SettingsProvider.Instance.CacheTTL = Convert.ToInt32(nUpDown.Value);
            SettingsProvider.Instance.Save();
            Close();
        }
        /// <summary>
        /// Incialize data when load form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvancedConfiguration_Load(object sender, EventArgs e)
        {
            chUseCacheGoogle.Checked = SettingsProvider.Instance.IsUseGoogleCache;
            chUseCacheOutlook.Checked = SettingsProvider.Instance.IsUseOutlookCache;
            nUpDown.Value = SettingsProvider.Instance.CacheTTL;
        }
        /// <summary>
        /// Delete cached data for outlook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearOutlook_Click(object sender, EventArgs e)
        {
            LoggerProvider.Instance.Logger.Info("User request remove all cache data for Outlook.");
            Utils.RemoveCacheFile(true);
        }
        /// <summary>
        /// Delete cached data for Google
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearGoogle_Click(object sender, EventArgs e)
        {
            LoggerProvider.Instance.Logger.Info("User request remove all cache data for Google.");
            Utils.RemoveCacheFile(false);
        }
        /// <summary>
        /// Remove synchronize keys from Outlook contact. In same delete all cache data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOutlokSync_Click(object sender, EventArgs e)
        {
            LoggerProvider.Instance.Logger.Info("User request for clear synchronize keys in Outlook. I remove all cache data for Google and Outlook.");
            Utils.RemoveCacheFile(false);
            Utils.RemoveCacheFile(true);
            pBar.Value = 1;
            pBar.Update();
            pBar.Refresh();
            OutlookProvider op = OutlookProvider.Instance;
            Outlook.Items it = op.OutlookItems();
            int _ouMaxContacts = op.CountContact();
            object works = null;
            Outlook.ContactItem oci = null;
            int counter = 0;
            for (int i =0; i < _ouMaxContacts; i++)
            {
                pBar.Value = counter;
                pBar.PerformStep();
                pBar.Refresh();
                if (counter > pBar.Maximum)
                    counter = 0;
                if (i == 0)
                    works = it.GetFirst();
                else
                    works = it.GetNext();
                if (works is Outlook.DistListItem)
                    continue;
                oci = works as Outlook.ContactItem;
                if (works == null)
                    continue;
                if (!string.IsNullOrEmpty(oci.User3))
                {
                    oci.User3 = string.Empty;
                    oci.Save();
                }
            }
        }


        #region Remove Google sync keys
        /// <summary>
        /// Remove synchronization keys from Google contacts. This work asychnonies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGoogleSync_Click(object sender, EventArgs e)
        {
            LoggerProvider.Instance.Logger.Info("User request for clear synchronize keys in Google. I remove all cache data for Google and Outlook.");
            Utils.RemoveCacheFile(false);
            Utils.RemoveCacheFile(true);
            if (!bw.IsBusy)
            {
                LoggerProvider.Instance.Logger.Debug("Execute Background synchronize.");
                pBar.Value = 1;
                pBar.Update();
                pBar.Refresh();
                btnClose.Enabled = false;
                btnClose1.Enabled = false;

                bw.RunWorkerAsync();
            }
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //lblProgress.Text = "Progress : " + e.ProgressPercentage.ToString();
            if (pBar.Value < pBar.Maximum)
            {
                pBar.PerformStep();
                pBar.Refresh();
            }
            else
            {
                pBar.Value = 0;
                pBar.Refresh();
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Dictionary<string, Google.Contacts.Contact> goContact = new Dictionary<string, Google.Contacts.Contact>();
            LoggerProvider.Instance.Logger.Debug("Read Google contact data for clear synchronization keys.");
            //int ToCheck = GoogleProvider.GetProvider.CountContact();
            int counter = 0;

            foreach (Google.Contacts.Contact gc in GoogleProvider.GetProvider.ContactItems.Entries)
            {
                worker.ReportProgress(++counter);
                if (counter > pBar.Maximum)
                    counter = 0;
                for (int j = 0; j < gc.ExtendedProperties.Count; j++)
                    if (gc.ExtendedProperties[j].Name == Constants.NameGoogleExtendProperties)
                    {
                        gc.ExtendedProperties.RemoveAt(j);
                        goContact.Add(gc.Id, gc);
                    }
            }
            if (goContact.Count == 0)
            {
                LoggerProvider.Instance.Logger.Debug("No contact to clear synchronize keys.");
            }
            LoggerProvider.Instance.Logger.Debug("Start to clear symchronization keys.");
            foreach (string i in goContact.Keys)
            {
                worker.ReportProgress(++counter);
                if (counter > pBar.Maximum)
                    counter = 0;
                GoogleProvider.GetProvider.Update(goContact[i]);
            }

        }
        /// <summary>
        /// Alow close program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoggerProvider.Instance.Logger.Debug("Asynchronies erasing synchronizoation keys on Google ends.");
            btnClose.Enabled = true;
            btnClose1.Enabled = true;
            pBar.Value = 1;
            pBar.Refresh();
        }

        #endregion

        /// <summary>
        /// Need wait until background process doesn't ends
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvancedConfiguration_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bw.IsBusy)
            {
                //MessageBox.Show("Need wait until background process doesn't stop.", "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.UseWaitCursor = true;
                //while (bw.IsBusy)
                //     System.Threading.Thread.Sleep(500);
                //this.UseWaitCursor = false;
            }
        }
    }
}
