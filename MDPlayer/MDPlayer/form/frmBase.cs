using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MDPlayer.Properties;

namespace MDPlayer.form
{
    public partial class frmBase : Form
    {
        public frmMain parent = null;

        public frmBase()
        {
            InitializeComponent();
        }
        public frmBase(frmMain frm)
        {
            parent = frm;
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            if (parent != null)
            {
                parent.windowsMessage(ref m);
            }

            try
            {
                int WM_NCLBUTTONDBLCLK = 0xA3;
                if (m.Msg == WM_NCLBUTTONDBLCLK)
                {
                    TopMost = !TopMost;
                    if (TopMost)
                        this.Icon = Resources.FeliTop;
                    else
                        this.Icon = Resources.Feli128;
                }
                base.WndProc(ref m);
            }
            catch (Exception ex)
            {
                log.ForcedWrite(ex);
            }
        }

    }
}
