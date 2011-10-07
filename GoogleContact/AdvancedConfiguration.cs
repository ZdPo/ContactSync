using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GoogleContact
{
    public partial class AdvancedConfiguration : Form
    {
        public AdvancedConfiguration()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            SettingsProvider.Instance.IsUseOutlookCache = chUseCacheOutlook.Checked;
            SettingsProvider.Instance.IsUseGoogleCache = chUseCacheGoogle.Checked;
            SettingsProvider.Instance.Save();
            Close();
        }

        private void AdvancedConfiguration_Load(object sender, EventArgs e)
        {
            chUseCacheGoogle.Checked = SettingsProvider.Instance.IsUseGoogleCache;
            chUseCacheOutlook.Checked = SettingsProvider.Instance.IsUseOutlookCache;
        }
    }
}
