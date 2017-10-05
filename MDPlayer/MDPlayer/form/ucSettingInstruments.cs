using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDPlayer.form
{
    public partial class ucSettingInstruments : UserControl
    {
        public ucSettingInstruments()
        {
            InitializeComponent();
        }

        private void cbSendWait_CheckedChanged(object sender, EventArgs e)
        {
            cbTwice.Enabled = cbSendWait.Checked;
        }
    }
}
