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
    /// <summary>
    /// Rename by bad spelling
    /// </summary>
    public partial class AuthenticateRequest : Form
    {
        private bool _isAccept;
        public AuthenticateRequest()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _isAccept = false;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _isAccept = true;
            this.Close();
        }

        /// <summary>
        /// Set configure user name
        /// </summary>
        public string UserName
        {
            set { txtUsername.Text = value; }
            get { return txtUsername.Text; }
        }
        /// <summary>
        /// Get write password
        /// </summary>
        public string Password
        {
            get { return txtPassword.Text; }
        }
        /// <summary>
        /// User click on OK
        /// </summary>
        public bool IsAccept
        {
            get { return _isAccept; }
        }
    }
}
