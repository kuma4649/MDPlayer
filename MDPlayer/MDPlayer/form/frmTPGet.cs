using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDPlayer
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
            if (tonePallet.lstTone == null) tonePallet.lstTone = new List<Tone>(256);

            for (int i = 0; i < 256; i++)
            {
                string toneName = "";
                if (tonePallet.lstTone.Count < i + 1 || tonePallet.lstTone[i] == null)
                {
                    tonePallet.lstTone.Add(new Tone());
                }

                toneName = tonePallet.lstTone[i].name;

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
                setting.other.Tones[ch].OPs[i].AR = tonePallet.lstTone[ind].OPs[i].AR;//AR
                setting.other.Tones[ch].OPs[i].KS = tonePallet.lstTone[ind].OPs[i].KS;//KS
                setting.other.Tones[ch].OPs[i].DR = tonePallet.lstTone[ind].OPs[i].DR;//DR
                setting.other.Tones[ch].OPs[i].AM = tonePallet.lstTone[ind].OPs[i].AM;//AM
                setting.other.Tones[ch].OPs[i].SR = tonePallet.lstTone[ind].OPs[i].SR;//SR
                setting.other.Tones[ch].OPs[i].RR = tonePallet.lstTone[ind].OPs[i].RR;//RR
                setting.other.Tones[ch].OPs[i].SL = tonePallet.lstTone[ind].OPs[i].SL;//SL
                setting.other.Tones[ch].OPs[i].TL = tonePallet.lstTone[ind].OPs[i].TL;//TL
                setting.other.Tones[ch].OPs[i].ML = tonePallet.lstTone[ind].OPs[i].ML;//ML
                setting.other.Tones[ch].OPs[i].DT = tonePallet.lstTone[ind].OPs[i].DT;//DT
                setting.other.Tones[ch].OPs[i].DT2 = tonePallet.lstTone[ind].OPs[i].DT2;//DT2 
            }

            setting.other.Tones[ch].AL = tonePallet.lstTone[ind].AL;//AL
            setting.other.Tones[ch].FB = tonePallet.lstTone[ind].FB;//FB
            setting.other.Tones[ch].AMS = tonePallet.lstTone[ind].AMS;//AMS
            setting.other.Tones[ch].PMS = tonePallet.lstTone[ind].PMS;//PMS
        }

    }
}
