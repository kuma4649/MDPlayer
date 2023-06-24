using MDPlayer;

namespace MDPlayerx64.form.SYS
{
    public partial class frmConsole : Form
    {

        public void LogWrite(string message)
        {
            if (!this.IsHandleCreated) return;
            this.BeginInvoke((Action<string>)lw, message);
        }

        private void lw(string message)
        {
            tbLog.SelectionStart = tbLog.Text.Length;
            tbLog.SelectionLength = 0;
            tbLog.SelectedText = string.Format("{0}\r\n", message);
        }

        public frmConsole(Setting setting)
        {
            InitializeComponent();
            log.SetLogger(LogWrite);
        }

        private void tsmiTrace_Click(object sender, EventArgs e)
        {
            log.logLevel = LogLevel.Trace;
            updateLoglevel();
        }

        private void tsmiDebug_Click(object sender, EventArgs e)
        {
            log.logLevel = LogLevel.Debug;
            updateLoglevel();
        }

        private void tsmiError_Click(object sender, EventArgs e)
        {
            log.logLevel = LogLevel.Error;
            updateLoglevel();
        }

        private void tsmiWarning_Click(object sender, EventArgs e)
        {
            log.logLevel = LogLevel.Warning;
            updateLoglevel();
        }

        private void tsmiInformation_Click(object sender, EventArgs e)
        {
            log.logLevel = LogLevel.Information;
            updateLoglevel();
        }

        private void tsmiClear_Click(object sender, EventArgs e)
        {
            tbLog.Clear();
        }

        private void frmConsole_Load(object sender, EventArgs e)
        {

        }

        private void frmConsole_Shown(object sender, EventArgs e)
        {
            updateLoglevel();
        }

        private void updateLoglevel()
        {
            tsmiTrace.Checked = log.logLevel == LogLevel.Trace;
            tsmiDebug.Checked = log.logLevel == LogLevel.Debug;
            tsmiError.Checked = log.logLevel == LogLevel.Error;
            tsmiWarning.Checked = log.logLevel == LogLevel.Warning;
            tsmiInformation.Checked = log.logLevel == LogLevel.Information;
        }

        private void frmConsole_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
