using System.Drawing;
using System.Windows.Forms;

namespace MDPlayer.form
{
    public partial class frmVST : Form
    {
        Rectangle wndRect = new Rectangle();

        public frmVST()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the Plugin Command Stub.
        /// </summary>
        public object PluginCommandStub { get; set; }

        /// <summary>
        /// Shows the custom plugin editor UI.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public new DialogResult ShowDialog(IWin32Window owner)
        {
            return base.ShowDialog(owner);
        }

        public void Show(vstMng.vstInfo2 vi)
        {
            base.Show();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
        }
    }
}
