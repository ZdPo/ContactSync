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
    public partial class SyncInfo : Form
    {
        private int _maxComplete = 100;
        private int _maxWork = 100;
        private int _ouContacts=0;
        private int _goContact=0;
        private int _onWork = 0;
        //private int _onStep = 0;

        public SyncInfo(int MaxComplete)
        {
            InitializeComponent();
            _maxComplete = MaxComplete;
            LoggerProvider.Instance.Logger.Debug("Class SyncInfo created");
        }

        private void SyncInfo_Load(object sender, EventArgs e)
        {
            pbWork.Step = 1;
            pbSum.Step = 1;
            pbSum.Maximum = _maxComplete;
            LoggerProvider.Instance.Logger.Debug("Class SyncInfo Loaded");
        }

        #region Parameters
        /// <summary>
        /// Change text for actual step
        /// </summary>
        public string ActualStep
        {
            set { lbStep.Text = value; }
            get { return lbStep.Text; }
        }

        /// <summary>
        /// Set number of google contacts
        /// </summary>
        public int GoogleContacs
        {
            set
            {
                _goContact = value;
                ShowActualData();
            }
        }
        /// <summary>
        /// Set number of outlook contacts
        /// </summary>
        public int OutlookContacts
        {
            set
            {
                _ouContacts = value;
                ShowActualData();
            }
        }
        /// <summary>
        /// Now work on record
        /// </summary>
        public int WorkOn
        {
            set
            {
                _onWork = value;
                ShowActualData();
            }
        }

        public int WorkOnMax
        {
            set
            {
                _maxWork = value;
                _onWork = 0;
                pbWork.Value = _onWork;
                pbWork.Maximum = _maxWork;
                pbWork.Update();
                ShowActualData();
            }
        }
        #endregion
        
        #region Metody
        /// <summary>
        /// Poskoci o 1 na OnWork progress baru
        /// </summary>
        public void WorkOnNextStep()
        {
            pbWork.PerformStep();
        }
        public void ActualNextStep()
        {
            pbSum.PerformStep();
        }
        #endregion

        #region Private rutines
        private void ShowActualData()
        {
            if (!this.Visible)
                return;
            lbContacts.Text = string.Format(Constants.FormatSyncAll, _ouContacts, _goContact);
            lbWorkOn.Text = string.Format(Constants.FormatSyncActual, _onWork);
            this.Refresh();
        }
        #endregion
    }
}
