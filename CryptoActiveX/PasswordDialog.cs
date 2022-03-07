using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CryptoActiveX
{
    public partial class PasswordDialog : Form
    {
        public PasswordDialog()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        public string GetPassword()
        {
            ShowDialog();
            if (DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                return txtPassword.Text;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
