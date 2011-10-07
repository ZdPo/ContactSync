using System;
using System.Windows.Forms;

namespace GoogleContact
{
    public partial class ConnectionSettings : Form
    {
        #region private variable
        SettingsProvider _sp = null;
        #endregion

        public ConnectionSettings()
        {
            InitializeComponent();
            //_isSave = false;
            //_Password = "";
            _sp = SettingsProvider.Instance;
        }

        private void ConnectionSettings_Load(object sender, EventArgs e)
        {
            gUserName.Text = _sp.UserName;
            //if (_isRemeber)
            if (_sp.IsRemember)
                gPassword.Text = _sp.UserPassword;
            else
            {
                //_Password = "";
                gPassword.Text = "";
            }
            gRemember.Checked = _sp.IsRemember;
            chFirstSync.Checked = _sp.IsFirstTime; ;
            switch (SettingsProvider.FirstSynchronizeGet())
            {
                case Constants.FirstSetupSynchronize.Outlook2Google:
                    rbOu2Go.Checked = true;
                    rbGo2Ou.Checked = false;
                    break;
                case Constants.FirstSetupSynchronize.Google2Outlook:
                    rbOu2Go.Checked = false;
                    rbGo2Ou.Checked = true;
                    break;
                case Constants.FirstSetupSynchronize.SynchronizeBoth:
                    rbOu2Go.Checked = false;
                    rbGo2Ou.Checked = false;
                    break;
                default:
                    break;
            }
            switch (SettingsProvider.SynchronizeDirectionGet())
            {
                case Constants.SetupSynchronize.Outlook2Google:
                    rbOut2GoSync.Checked = true;
                    rbGo2OuSync.Checked = false;
                    rbBothSync.Checked = false;
                    break;
                case Constants.SetupSynchronize.Google2Outlook:
                    rbOut2GoSync.Checked = false;
                    rbGo2OuSync.Checked = true;
                    rbBothSync.Checked = false;
                    break;
                case Constants.SetupSynchronize.SynchronizeBothSide:
                    rbOut2GoSync.Checked = false;
                    rbGo2OuSync.Checked = false;
                    rbBothSync.Checked = true;
                    break;
                default:
                    break;
            }
            txDirectory.Text = _sp.LogFile;
            ddLogLevel.SelectedIndex = (int)SettingsProvider.LogLevelGet();
        }

        #region On Button
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(gUserName.Text) || string.IsNullOrEmpty(gPassword.Text))
            {
                LoggerProvider.Instance.Logger.Debug("Close settings without change because Usernameor Password is blank");
                this.Close();
            }
            _sp.UserName = gUserName.Text;
            _sp.UserPassword = gPassword.Text;
            _sp.IsRemember = gRemember.Checked;
            //_isSave = true;
            _sp.IsFirstTime = chFirstSync.Checked;
            if (rbOu2Go.Checked)
                _sp.FirstSynchronizeSet(Constants.FirstSetupSynchronize.Outlook2Google);
            else if (rbGo2Ou.Checked)
                _sp.FirstSynchronizeSet(Constants.FirstSetupSynchronize.Google2Outlook);

            if (rbOut2GoSync.Checked)
                _sp.SynchronizeDirectionSet(Constants.SetupSynchronize.Outlook2Google);
            else if (rbGo2OuSync.Checked)
                _sp.SynchronizeDirectionSet(Constants.SetupSynchronize.Google2Outlook);
            else if (rbBothSync.Checked)
                _sp.SynchronizeDirectionSet(Constants.SetupSynchronize.SynchronizeBothSide);
            _sp.LogFile = txDirectory.Text;
            _sp.LogLevelSet((Constants.LogLevel)Enum.Parse(typeof(Constants.LogLevel),ddLogLevel.SelectedIndex.ToString()));
            LoggerProvider.Instance.Logger.Debug("New configuration saved");
            _sp.Save();
            LoggerProvider.Instance.ReloadSettings();
            LoggerProvider.Instance.Logger.Debug("Logger instance reload");
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoggerProvider.Instance.Logger.Debug("Connection Settings Close without on Click button Close");
            this.Close();
        }
        #endregion

        private void btnSelectDirectory_Click(object sender, EventArgs e)
        {
            fbSelectLogDirectory.SelectedPath = _sp.LogFile;
            DialogResult dr = fbSelectLogDirectory.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _sp.LogFile = fbSelectLogDirectory.SelectedPath;
                LoggerProvider.Instance.Logger.Debug("Logger file directory changet to {0}", _sp.LogFile);
            }
            txDirectory.Text = _sp.LogFile;
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            using (AdvancedConfiguration ac = new AdvancedConfiguration())
                ac.ShowDialog();
        }
    }
}
