using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MDPlayer.form
{
    // Partialの相方（frmToast.Designer.cs）があるはずなので、
    // 重複を避けるためコンストラクタ周辺を整理します
    public partial class frmToast : Form
    {
        // Win32 API 定義
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(
            IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;

        // ShowWithoutActivation を true にする
        protected override bool ShowWithoutActivation => true;

        private System.Windows.Forms.Timer scrollTimer;
        private System.Windows.Forms.Timer closeTimer;
        private Label lblTitle;
        private Panel containerPanel;
        private bool isScrollFinished = false; // スクロールが終わったか
        private bool isTimeReached = false;    // 5秒経過したか

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // 最前面に表示するがフォーカスは奪わない
            SetWindowPos(
                this.Handle,
                HWND_TOPMOST,
                0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW
            );
        }

        public frmToast(string artist, string title)
        {
            //Width = 400;
            //Height = 200;
            //StartPosition = FormStartPosition.CenterScreen;
            //FormBorderStyle = FormBorderStyle.FixedToolWindow;


            // デザイナーを使わずコードのみで生成する場合の初期設定
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.Size = new Size(300, 80);
            this.DoubleBuffered = true; // ちらつき防止

            // 1. パネルの配置
            containerPanel = new Panel
            {
                Location = new Point(10, 15),
                Size = new Size(280, 25),
                BackColor = Color.Transparent
            };
            this.Controls.Add(containerPanel);

            // 2. タイトルラベル（パネルの中に入れる！）
            lblTitle = new Label
            {
                Text = title,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(0, 0),
                AutoSize = true
            };
            containerPanel.Controls.Add(lblTitle); 

            // アーティストラベル（こちらはフォームに直接置く）
            Label lblArtist = new Label
            {
                Text = artist,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 9),
                Location = new Point(10, 40),
                AutoSize = true
            };
            this.Controls.Add(lblArtist);

            // 5秒後に閉じるタイマーの設定
            closeTimer = new System.Windows.Forms.Timer { Interval = 5000 };
            closeTimer.Tick += async (s, e) =>
            {
                closeTimer.Stop();
                isTimeReached = true;
                CheckAndClose(); // 条件が揃っていれば閉じる
            };
            closeTimer.Start();

            this.Load += (s, e) => {
                // 表示位置の計算
                Rectangle screen = Screen.PrimaryScreen.WorkingArea;
                this.Location = new Point(screen.Right - this.Width - 10,
                                          screen.Bottom - this.Height - 10);

                AnimateWindow();

                // 3. スクロールが必要か判定（Load後に行うのが確実）
                if (lblTitle.Width > containerPanel.Width)
                {
                    scrollTimer = new System.Windows.Forms.Timer { Interval = 30 };
                    scrollTimer.Tick += (s, e) =>
                    {
                        lblTitle.Left -= 1;

                        // 文字が完全に左へ消え切った判定 (ループさせずに終了フラグを立てる)
                        if (lblTitle.Right < 0)
                        {
                            scrollTimer.Stop();
                            isScrollFinished = true;
                            CheckAndClose(); // 条件が揃っていれば閉じる
                        }
                    };
                    scrollTimer.Start();
                }
                else
                {
                    // スクロール不要な場合は最初から完了扱い
                    isScrollFinished = true;
                }
            };
        }

        private void StartScroll()
        {
            scrollTimer = new System.Windows.Forms.Timer { Interval = 30 };
            scrollTimer.Tick += (s, e) =>
            {
                lblTitle.Left -= 1;
                if (lblTitle.Right < 0)
                {
                    lblTitle.Left = containerPanel.Width;
                }
            };
            scrollTimer.Start();
        }

        private async void AnimateWindow()
        {
            try
            {
                this.Opacity = 0;
                while (this.Opacity < 1)
                {
                    await System.Threading.Tasks.Task.Delay(10);
                    this.Opacity += 0.05;
                }
            }
            catch
            {
                // フォームが閉じられた後にアニメーションが続行される場合の例外を無視
            }
        }

        private async System.Threading.Tasks.Task FadeOutAndClose()
        {
            try
            {
                // 徐々に不透明度を下げる
                while (this.Opacity > 0)
                {
                    await System.Threading.Tasks.Task.Delay(10);
                    this.Opacity -= 0.05;
                }
                this.Close(); // 完全に消えたら閉じる
            }
            catch
            {
                // フォームが既に閉じられている場合の例外を無視
            }
        }

        private async void CheckAndClose()
        {
            // 「5秒経過」かつ「スクロール完了」の両方を満たした時だけ閉じる
            if (isTimeReached && isScrollFinished)
            {
                await FadeOutAndClose();
            }
        }
    }
}
