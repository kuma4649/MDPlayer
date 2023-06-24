namespace MDPlayer.form
{
    public partial class frmTPPut : Form
    {
        private Setting setting = null;
        private TonePallet tonePallet = null;

        public frmTPPut()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(Setting setting, TonePallet tonePallet)
        {
            this.setting = setting;
            this.tonePallet = tonePallet;

            return this.ShowDialog();
        }

        private void frmTPPut_Load(object sender, EventArgs e)
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

        private void dgvTonePallet_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvTonePallet.Rows[e.RowIndex].Cells[0].Value = e.RowIndex + "*";
            btApply.Enabled = true;
        }

        private void btChn_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cc = dgvTonePallet.SelectedCells;
            if (cc == null || cc.Count != 1) return;
            DataGridViewRow row = cc[0].OwningRow;
            if (row == null) return;

            string m = string.Format("from Ch.{0}", ((Button)sender).Tag);
            string n = row.Cells["clmSpacer"].Value == null ? "" : row.Cells["clmSpacer"].Value.ToString();
            row.Cells["clmSpacer"].Value = (m == n) ? "" : m;

            btApply.Enabled = true;
        }

        private void btApply_Click(object sender, EventArgs e)
        {
            updateToneNames();
            updateTone();
            btApply.Enabled = false;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            updateToneNames();
            updateTone();
            this.Close();
        }

        private void updateToneNames()
        {
            for (int i = 0; i < 256; i++)
            {
                dgvTonePallet.Rows[i].Cells["clmNo"].Value = i;
                tonePallet.LstTone[i].name = dgvTonePallet.Rows[i].Cells["clmName"].Value.ToString();
            }
        }

        private void updateTone()
        {
            for (int i = 0; i < 256; i++)
            {
                object o = dgvTonePallet.Rows[i].Cells["clmSpacer"].Value;
                string n = o == null ? "" : o.ToString();
                if (n == "") continue;

                int ch = int.Parse(n.Replace("from Ch.", "")) - 1;

                CopySettingToneToTonePallet(ch, i);

                dgvTonePallet.Rows[i].Cells["clmSpacer"].Value = "";
            }
        }

        private void CopySettingToneToTonePallet(int ch, int ind)
        {
            for (int i = 0; i < 4; i++)
            {
                tonePallet.LstTone[ind].OPs[i].AR = setting.midiKbd.Tones[ch].OPs[i].AR;//AR
                tonePallet.LstTone[ind].OPs[i].KS = setting.midiKbd.Tones[ch].OPs[i].KS;//KS
                tonePallet.LstTone[ind].OPs[i].DR = setting.midiKbd.Tones[ch].OPs[i].DR;//DR
                tonePallet.LstTone[ind].OPs[i].AM = setting.midiKbd.Tones[ch].OPs[i].AM;//AM
                tonePallet.LstTone[ind].OPs[i].SR = setting.midiKbd.Tones[ch].OPs[i].SR;//SR
                tonePallet.LstTone[ind].OPs[i].RR = setting.midiKbd.Tones[ch].OPs[i].RR;//RR
                tonePallet.LstTone[ind].OPs[i].SL = setting.midiKbd.Tones[ch].OPs[i].SL;//SL
                tonePallet.LstTone[ind].OPs[i].TL = setting.midiKbd.Tones[ch].OPs[i].TL;//TL
                tonePallet.LstTone[ind].OPs[i].ML = setting.midiKbd.Tones[ch].OPs[i].ML;//ML
                tonePallet.LstTone[ind].OPs[i].DT = setting.midiKbd.Tones[ch].OPs[i].DT;//DT
                tonePallet.LstTone[ind].OPs[i].DT2 = setting.midiKbd.Tones[ch].OPs[i].DT2;//DT2 
            }

            tonePallet.LstTone[ind].AL = setting.midiKbd.Tones[ch].AL;//AL
            tonePallet.LstTone[ind].FB = setting.midiKbd.Tones[ch].FB;//FB
            tonePallet.LstTone[ind].AMS = setting.midiKbd.Tones[ch].AMS;
            tonePallet.LstTone[ind].PMS = setting.midiKbd.Tones[ch].PMS;

        }

    }
}
