using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MDPlayer.form
{
    // Partialの相方（frmToast.Designer.cs）があるはずなので、
    // 重複を避けるためコンストラクタ周辺を整理します
    public partial class frmToast : Form
    {
        // フォームが閉じられたかのフラグ（frmMain で参照される）
        public bool isClosed = false;
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

        //private System.Windows.Forms.Timer scrollTimer;
        //private System.Windows.Forms.Timer closeTimer;
        private Label lblTitle;
        private Panel containerPanel;
        private FrameBuffer frameBuffer;
        private PictureBox pbScreen;
        private string artistText = "";
        private Bitmap cachedTextBitmap = null;
        private string cachedTitle = null;
        private string cachedArtist = null;
        private Bitmap cachedTitleBitmap = null;
        private Bitmap cachedArtistBitmap = null;
        private int titleOffset = 0;
        private int titleWidth = 0;
        private int titleHeight = 0;
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

        // ANSI エスケープシーケンスや制御文字を取り除いたプレーンテキストを返す
        private static string StripAnsi(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            try
            {
                // CSI シーケンス (ESC [ ... letter)
                input = Regex.Replace(input, "\u001B\\[[0-9;?]*[ -/]*[@-~]", "");
                // その他の制御文字を除去 (0x00-0x1F, DEL)
                input = Regex.Replace(input, "[\x00-\x1F\x7F]+", "");
            }
            catch { }

            return input;
        }

        private void PrepareCachedTextBitmaps()
        {
            try
            {
                string title = lblTitle?.Text ?? "";
                string artist = artistText ?? "";

                // title bitmap
                // title bitmap (ANSI sequences stripped - plain white text)
                if (cachedTitleBitmap == null || title != cachedTitle)
                {
                    cachedTitleBitmap?.Dispose();
                    cachedTitleBitmap = null;
                    // strip ANSI/control sequences for plain rendering
                    string plainTitle = StripAnsi(title);
                    cachedTitle = title;
                    if (!string.IsNullOrEmpty(plainTitle))
                    {
                        using (var f = new Font("Segoe UI", 10, FontStyle.Bold))
                        {
                            var sz = TextRenderer.MeasureText(plainTitle, f, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                            titleWidth = Math.Max(1, sz.Width);
                            titleHeight = Math.Max(1, sz.Height);
                        }

                        cachedTitleBitmap = new Bitmap(titleWidth, titleHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        using (var g2 = Graphics.FromImage(cachedTitleBitmap))
                        using (var f2 = new Font("Segoe UI", 10, FontStyle.Bold))
                        {
                            g2.Clear(Color.Transparent);
                            g2.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                            TextRenderer.DrawText(g2, plainTitle, f2, new Point(0, 0), Color.White, TextFormatFlags.NoPadding);
                        }
                    }
                    else
                    {
                        titleWidth = 0;
                        titleHeight = 0;
                    }
                }

                // artist bitmap
                if (cachedArtistBitmap == null || artist != cachedArtist)
                {
                    cachedArtistBitmap?.Dispose();
                    cachedArtistBitmap = null;
                    cachedArtist = artist;
                    if (!string.IsNullOrEmpty(artist))
                    {
                        using (var tmp = new Bitmap(1, 1))
                        using (var g = Graphics.FromImage(tmp))
                        using (var f = new Font("Segoe UI", 9))
                        {
                            var sz = g.MeasureString(artist, f);
                            int w = Math.Max(1, (int)Math.Ceiling(sz.Width));
                            int h = Math.Max(1, (int)Math.Ceiling(sz.Height));
                            cachedArtistBitmap = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                            using (var g2 = Graphics.FromImage(cachedArtistBitmap))
                            using (var f2 = new Font("Segoe UI", 9))
                            using (var b = new SolidBrush(Color.LightGray))
                            {
                                g2.Clear(Color.Transparent);
                                g2.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                                g2.DrawString(artist, f2, b, new PointF(0f, 0f));
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void RenderTextToFrameBuffer()
        {
            try
            {
                if (frameBuffer == null) return;

                // Ensure text bitmaps prepared
                PrepareCachedTextBitmaps();

                // Clear framebuffer
                frameBuffer.clearScreen();

                int fw = frameBuffer.bmpPlaneW;
                int fh = frameBuffer.bmpPlaneH;
                if (fw <= 0) fw = Math.Max(1, this.ClientSize.Width);
                if (fh <= 0) fh = Math.Max(1, this.ClientSize.Height);

                using (Bitmap tmp = new Bitmap(fw, fh, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(tmp))
                    {
                        g.Clear(this.BackColor);
                        // draw title (scrolling) using cachedTitleBitmap
                        if (cachedTitleBitmap != null)
                        {
                            g.DrawImageUnscaled(cachedTitleBitmap, titleOffset, 8);
                        }
                        // draw artist static
                        if (cachedArtistBitmap != null)
                        {
                            g.DrawImageUnscaled(cachedArtistBitmap, 10, 24);
                        }
                    }

                    try
                    {
                        System.Drawing.Imaging.BitmapData bd = tmp.LockBits(new Rectangle(0, 0, tmp.Width, tmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, tmp.PixelFormat);
                        try
                        {
                            int[] src = new int[bd.Stride / 4 * bd.Height];
                            System.Runtime.InteropServices.Marshal.Copy(bd.Scan0, src, 0, src.Length);
                            frameBuffer.drawIntArray(0, 0, src, bd.Stride / 4, 0, 0, tmp.Width, tmp.Height);
                        }
                        finally
                        {
                            tmp.UnlockBits(bd);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        public frmToast(string artist, string title)//,frmMain frm)
        {
            InitializeComponent();

            // 閉じたフラグを管理
            this.FormClosed += (s, e) => { isClosed = true; };

            // デザイナーを使わずコードのみで生成する場合の初期設定
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.Size = new Size(300, 80);
            this.DoubleBuffered = true; // ちらつき防止

            // 1. PictureBox + FrameBuffer で描画（Label を使わない）
            pbScreen = new PictureBox();
            pbScreen.Location = new Point(0, 0);
            pbScreen.Size = this.ClientSize;
            pbScreen.BackColor = Color.Transparent;
            pbScreen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.Controls.Add(pbScreen);

            frameBuffer = new FrameBuffer();
            // 初期イメージ: 背景色で塗った bitmap
            Bitmap init = new Bitmap(Math.Max(1, this.ClientSize.Width), Math.Max(1, this.ClientSize.Height));
            using (Graphics g = Graphics.FromImage(init))
            {
                g.Clear(this.BackColor);
            }
            frameBuffer.Add(pbScreen, init, null, 1);
            init.Dispose();

            // 保存用ラベルテキストはプロパティとして保管
            lblTitle = new Label();
            lblTitle.Text = title;
            artistText = artist;
            containerPanel = new Panel();
            // ラベル/パネルは描画に使わないがテキスト保存用に残す
            this.Controls.Remove(containerPanel);


        }

        // フェード制御（frmMain のループから毎フレーム呼ばれる想定）
        private bool fadingIn = true;
        private bool fadingOut = false;
        private float fadeSpeed = 5.0f; // opacity change per second

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // frmToast
            // 
            ClientSize = new Size(284, 261);
            Name = "frmToast";
            Load += frmToast_Load;
            ResumeLayout(false);

        }
        private Stopwatch lifeWatch;
        private long lastUpdateMs;
        private float scrollSpeed = 60f; // pixels per second
        private bool scrollingActive = false;
        private bool needsPaint = false;
        private double lastOpacity = 0.0;
        private frmMain parent;

        // timers removed; main form will call screenDrawParams/update at ~60fps

        private void frmToast_Load(object sender, EventArgs e)
        {
            // 表示位置の計算
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Right - this.Width - 10,
                                      screen.Bottom - this.Height - 10);

            // 初期不透明度とフェードイン開始
            this.Opacity = 0.0;
            fadingIn = true;

            // 初期タイマー／スクロール状態
            lifeWatch = Stopwatch.StartNew();
            lastUpdateMs = lifeWatch.ElapsedMilliseconds;
            // prepare cached text bitmaps and initial offsets
            PrepareCachedTextBitmaps();
            int viewW = frameBuffer?.bmpPlaneW ?? this.ClientSize.Width;
            titleOffset = viewW; // start from right edge
            scrollingActive = (titleWidth > viewW - 20);
            isScrollFinished = !scrollingActive;

            // 初回描画パラメータ設定
            //screenDrawParams();

        }

        // frmMain の screenDrawParamsForms から呼ばれる想定のメソッド
        public void screenDrawParams()
        {
            try
            {
                // 位置を作業領域の右下に合わせる
                Rectangle screen = Screen.PrimaryScreen.WorkingArea;
                this.Location = new Point(screen.Right - this.Width - 10,
                                          screen.Bottom - this.Height - 10);
                // 初回または毎フレームの更新（スクロール・経過時間）
                if (lifeWatch == null)
                {
                    lifeWatch = Stopwatch.StartNew();
                    lastUpdateMs = lifeWatch.ElapsedMilliseconds;
                }

                long now = lifeWatch.ElapsedMilliseconds;
                long delta = now - lastUpdateMs;
                if (delta < 0) delta = 0;
                lastUpdateMs = now;

                // スクロール判定 / 更新
                try
                {
                    PrepareCachedTextBitmaps();
                    int viewW = frameBuffer?.bmpPlaneW ?? this.ClientSize.Width;
                    if (titleWidth > viewW - 20)
                    {
                        scrollingActive = true;
                    }
                    else
                    {
                        scrollingActive = false;
                        isScrollFinished = true;
                    }

                    if (scrollingActive)
                    {
                        float dx = scrollSpeed * (delta / 1000f);
                        // move titleOffset left by dx
                        int newOffset = titleOffset - Math.Max(1, (int)Math.Ceiling(dx));
                        if (newOffset != titleOffset)
                        {
                            titleOffset = newOffset;
                            RenderTextToFrameBuffer();
                            needsPaint = true;
                        }
                        // fully scrolled out
                        if (titleOffset + titleWidth < 0)
                        {
                            scrollingActive = false;
                            isScrollFinished = true;
                            RenderTextToFrameBuffer();
                            needsPaint = true;
                        }
                    }
                    else
                    {
                        // static: ensure title positioned at left margin
                        int desired = 10;
                        if (titleOffset != desired)
                        {
                            titleOffset = desired;
                            RenderTextToFrameBuffer();
                            needsPaint = true;
                        }
                    }
                }
                catch { }

                // 経過時間で閉じる判定（5秒）。スクロール完了と両方満たしたらフェードアウト開始
                if (!isTimeReached && lifeWatch.ElapsedMilliseconds >= 5000)
                {
                    isTimeReached = true;
                }

                if (isTimeReached && isScrollFinished && !fadingOut)
                {
                    fadingOut = true;
                }

                // フェードイン/アウト処理
                if (fadingIn)
                {
                    float dop = fadeSpeed * (delta / 1000f);
                    double newOp = Math.Min(1.0, this.Opacity + dop);
                    if (Math.Abs(newOp - this.Opacity) > 0.001)
                    {
                        this.Opacity = newOp;
                        RenderTextToFrameBuffer();
                        needsPaint = true;
                    }
                    if (this.Opacity >= 1.0)
                    {
                        fadingIn = false;
                    }
                }
                else if (fadingOut)
                {
                    float dop = fadeSpeed * (delta / 1000f);
                    double newOp = Math.Max(0.0, this.Opacity - dop);
                    if (Math.Abs(newOp - this.Opacity) > 0.001)
                    {
                        this.Opacity = newOp;
                        RenderTextToFrameBuffer();
                        needsPaint = true;
                    }
                    if (this.Opacity <= 0.0)
                    {
                        this.Close();
                    }
                }
            }
            catch { }
        }

        // frmMain から呼ばれる更新メソッド
        public void update()
        {
            try
            {
                if (!this.IsDisposed && this.IsHandleCreated)
                {
                    // 描画が必要なときは frameBuffer に非同期描画を要求する（UI スレッドをブロックしない）
                    if (needsPaint && frameBuffer != null)
                    {
                        frameBuffer.RefreshAsync(null);
                        needsPaint = false;
                    }
                }
            }
            catch { }
        }
    }
}
