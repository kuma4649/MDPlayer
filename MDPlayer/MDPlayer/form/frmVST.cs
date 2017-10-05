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
        public Jacobi.Vst.Core.Host.IVstPluginCommandStub PluginCommandStub { get; set; }

        /// <summary>
        /// Shows the custom plugin editor UI.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public new DialogResult ShowDialog(IWin32Window owner)
        {

            this.Text = PluginCommandStub.GetEffectName();

            if (PluginCommandStub.EditorGetRect(out wndRect))
            {
                this.Size = this.SizeFromClientSize(new Size(wndRect.Width, wndRect.Height));
                PluginCommandStub.EditorOpen(this.Handle);
            }

            return base.ShowDialog(owner);
        }

        public void Show(vstInfo2 vi)
        {

            this.Text = PluginCommandStub.GetEffectName();

            if (PluginCommandStub.EditorGetRect(out wndRect))
            {
                this.Size = this.SizeFromClientSize(new Size(wndRect.Width, wndRect.Height));
                PluginCommandStub.EditorOpen(this.Handle);
            }
            this.Location = new Point(vi.location.X, vi.location.Y);
            base.Show();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            if (e.Cancel == false)
            {
                PluginCommandStub.EditorClose();
            }
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            try
            {
                PluginCommandStub.EditorIdle();
                if (PluginCommandStub.EditorGetRect(out wndRect))
                {
                    this.Size = this.SizeFromClientSize(new Size(wndRect.Width, wndRect.Height));
                }
            }
            catch { }
        }
    }
}
