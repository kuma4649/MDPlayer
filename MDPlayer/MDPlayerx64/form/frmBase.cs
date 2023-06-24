#if X64
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif

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
                parent.WindowsMessage(ref m);
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
