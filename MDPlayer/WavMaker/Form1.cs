using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace WavMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnRef_Click(object sender, EventArgs e)
        {
            string fn = fileOpen();
            if (fn != null) tbFileName.Text = fn;
        }

        private void btnMake_Click(object sender, EventArgs e)
        {
            if (make(tbFileName.Text))
            {
                MessageBox.Show("処理完了");
            }
            else
            {
                MessageBox.Show("処理失敗");
            }
        }

        private string fileOpen()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "すべてのファイル(*.*)|*.*";
                ofd.Title = "ファイルを選択してください";
                ofd.RestoreDirectory = true;
                ofd.CheckPathExists = true;
                ofd.Multiselect = false;

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return null;
                }

                return ofd.FileNames[0];
            }
            catch { }
            return null;
        }

        private bool make(string fn)
        {
            try
            {
                byte[] src = File.ReadAllBytes(fn);
                string dFn = Path.ChangeExtension(fn,".wav");
                List<byte> des=new List<byte>();

                // 'RIFF'
                des.Add((byte)'R'); des.Add((byte)'I'); des.Add((byte)'F'); des.Add((byte)'F');
                // サイズ
                int fsize = src.Length + 36;
                des.Add((byte)((fsize & 0xff) >> 0));
                des.Add((byte)((fsize & 0xff00) >> 8));
                des.Add((byte)((fsize & 0xff0000) >> 16));
                des.Add((byte)((fsize & 0xff000000) >> 24));
                // 'WAVE'
                des.Add((byte)'W'); des.Add((byte)'A'); des.Add((byte)'V'); des.Add((byte)'E');
                // 'fmt '
                des.Add((byte)'f'); des.Add((byte)'m'); des.Add((byte)'t'); des.Add((byte)' ');
                // サイズ(16)
                des.Add(0x10); des.Add(0); des.Add(0); des.Add(0);
                // フォーマット(1)
                des.Add(0x01); des.Add(0x00);
                // チャンネル数(mono)
                des.Add(0x01); des.Add(0x00);
                //サンプリング周波数(8KHz)
                des.Add(0x40); des.Add(0x1f); des.Add(0); des.Add(0);
                //平均データ割合(8K)
                des.Add(0x40); des.Add(0x1f); des.Add(0); des.Add(0);
                //ブロックサイズ(1)
                des.Add(0x01); des.Add(0x00);
                //ビット数(8bit)
                des.Add(0x08); des.Add(0x00);

                // 'data'
                des.Add((byte)'d'); des.Add((byte)'a'); des.Add((byte)'t'); des.Add((byte)'a');
                // サイズ(データサイズ)
                des.Add((byte)((src.Length & 0xff) >> 0));
                des.Add((byte)((src.Length & 0xff00) >> 8));
                des.Add((byte)((src.Length & 0xff0000) >> 16));
                des.Add((byte)((src.Length & 0xff000000) >> 24));

                foreach (byte d in src) des.Add(d);

                //出力
                File.WriteAllBytes(dFn, des.ToArray());

                return true;
            }
            catch
            {
            }
            return false;
        }
    }
}
