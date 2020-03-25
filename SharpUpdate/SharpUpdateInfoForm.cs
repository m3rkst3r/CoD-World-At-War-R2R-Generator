using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpUpdate
{

    internal partial class SharpUpdateInfoForm : Form
    {

        internal SharpUpdateInfoForm(SharpUpdateLocalAppInfo applicationInfo, SharpUpdateXml updateInfo)
        {
            InitializeComponent();
            if (applicationInfo.ApplicationIcon != null)
                this.Icon = applicationInfo.ApplicationIcon;
            this.Text = applicationInfo.ApplicationName + " - Update Info";
            this.lblVersions.Text = updateInfo.Tag == JobType.UPDATE ?
                string.Format("Current Version: {0}\nUpdate version: {1}", applicationInfo.Version.ToString(), updateInfo.Version.ToString()) :
                (updateInfo.Tag == JobType.ADD ? string.Format("Version: {0}", updateInfo.Version.ToString()) :
                "");
            this.txtDescription.Text = updateInfo.Description;
        }

            private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            // Only allow Cntrl - C to copy text
            if (!(e.Control && e.KeyCode == Keys.C))
                e.SuppressKeyPress = true;
        }
    }
}
