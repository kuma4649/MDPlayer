namespace MDPlayer.form
{
    public partial class frmTPGet : Form
    {
        private Setting setting = null;
        private TonePallet tonePallet = null;

        public frmTPGet()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(Setting setting, TonePallet tonePallet)
        {
            this.setting = setting;
            this.tonePallet = tonePallet;

            return this.ShowDialog();
        }

        private void frmTPGet_Load(object sender, EventArgs e)
        {
            dgvTonePallet.Rows.Clear();
            if (tonePallet == null) tonePallet = new TonePallet();
            if (tonePallet.LstTone == null) tonePallet.LstTone = new List<Tone>(256);

            for (int i = 0; i < 256; i++)
            {
                string toneName = "";
                if (tonePallet.LstTone.Count < i + 1 || tonePallet.LstTone[i] == null)
                {
                    tonePallet.LstTone.Add(new Tone());
                }

                toneName = tonePallet.LstTone[i].name;

                dgvTonePallet.Rows.Add();
                dgvTonePallet.Rows[i].Cells["clmNo"].Value = i;
                dgvTonePallet.Rows[i].Cells["clmName"].Value = toneName;

            }
        }

        private void btChn_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cc = dgvTonePallet.SelectedCells;
            if (cc == null || cc.Count != 1) return;
            DataGridViewRow row = cc[0].OwningRow;
            if (row == null) return;

            string m = string.Format("to Ch.{0}", ((Button)sender).Tag);
            string n = row.Cells["clmSpacer"].Value == null ? "" : row.Cells["clmSpacer"].Value.ToString();
            row.Cells["clmSpacer"].Value = (m == n) ? "" : m;

            btApply.Enabled = true;

        }

        private void btApply_Click(object sender, EventArgs e)
        {
            updateTone();
            btApply.Enabled = false;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            updateTone();
            this.Close();
        }

        private void updateTone()
        {
            for (int i = 0; i < 256; i++)
            {
                object o = dgvTonePallet.Rows[i].Cells["clmSpacer"].Value;
                string n = o == null ? "" : o.ToString();
                if (n == "") continue;

                int ch = int.Parse(n.Replace("to Ch.", "")) - 1;

                CopyTonePalletToSettingTone(i, ch);

                dgvTonePallet.Rows[i].Cells["clmSpacer"].Value = "";
            }
        }

        private void CopyTonePalletToSettingTone(int ind, int ch)
        {
            for (int i = 0; i < 4; i++)
            {
                setting.midiKbd.Tones[ch].OPs[i].AR = tonePallet.LstTone[ind].OPs[i].AR;//AR
                setting.midiKbd.Tones[ch].OPs[i].KS = tonePallet.LstTone[ind].OPs[i].KS;//KS
                setting.midiKbd.Tones[ch].OPs[i].DR = tonePallet.LstTone[ind].OPs[i].DR;//DR
                setting.midiKbd.Tones[ch].OPs[i].AM = tonePallet.LstTone[ind].OPs[i].AM;//AM
                setting.midiKbd.Tones[ch].OPs[i].SR = tonePallet.LstTone[ind].OPs[i].SR;//SR
                setting.midiKbd.Tones[ch].OPs[i].RR = tonePallet.LstTone[ind].OPs[i].RR;//RR
                setting.midiKbd.Tones[ch].OPs[i].SL = tonePallet.LstTone[ind].OPs[i].SL;//SL
                setting.midiKbd.Tones[ch].OPs[i].TL = tonePallet.LstTone[ind].OPs[i].TL;//TL
                setting.midiKbd.Tones[ch].OPs[i].ML = tonePallet.LstTone[ind].OPs[i].ML;//ML
                setting.midiKbd.Tones[ch].OPs[i].DT = tonePallet.LstTone[ind].OPs[i].DT;//DT
                setting.midiKbd.Tones[ch].OPs[i].DT2 = tonePallet.LstTone[ind].OPs[i].DT2;//DT2 
            }

            setting.midiKbd.Tones[ch].AL = tonePallet.LstTone[ind].AL;//AL
            setting.midiKbd.Tones[ch].FB = tonePallet.LstTone[ind].FB;//FB
            setting.midiKbd.Tones[ch].AMS = tonePallet.LstTone[ind].AMS;//AMS
            setting.midiKbd.Tones[ch].PMS = tonePallet.LstTone[ind].PMS;//PMS
        }

    }
}
