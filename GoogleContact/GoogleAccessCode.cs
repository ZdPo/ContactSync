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
    public partial class GoogleAccessCode : Form
    {
        public GoogleAccessCode()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAccessCode.Text))
                MessageBox.Show("There is no Access Code eneterd", "Google Access Code Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearGoogleAccessCode();
            this.Close();
        }
        public string ActualGoogleAccessCode
        {
            get { return txtAccessCode.Text; }
        }
        public void CreateRequestOnGoole(string url)
        {
            System.Diagnostics.Process.Start(url);
        }
        public void ClearGoogleAccessCode()
        {
            txtAccessCode.Text = "";
        }

    }
}
