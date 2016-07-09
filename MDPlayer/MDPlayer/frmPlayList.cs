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
    public partial class frmPlayList : Form
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;

        public frmPlayList()
        {
            InitializeComponent();
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        private void frmPlayList_FormClosed(object sender, FormClosedEventArgs e)
        {
            isClosed = true;
        }

        private void frmPlayList_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);
        }

        private void frmPlayList_Shown(object sender, EventArgs e)
        {
            dgvList.Rows.Add("Dummy", "dummy");
            dgvList.Rows.Add("まだ", "できてないよ");
        }
    }
}
