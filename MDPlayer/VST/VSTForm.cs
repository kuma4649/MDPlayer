using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace VST
{
    unsafe public partial class VSTForm : Form
    {
        public AEffect effect;
        private VST vst;

        public VSTForm(AEffect effect,VST vst)
        {
            InitializeComponent();
            this.effect = effect;
            this.vst = vst;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case 0x1: //WM_CREATE
                    SetTimer(m.HWnd, 1, 20, IntPtr.Zero);
                    effect.dispatcher(ref effect, (int)AEffectOpcodes.effEditOpen, 0, 0, m.HWnd, 0);
                    ERect rect = new ERect
                    {
                        left = 0,
                        top = 0,
                        right = 0,
                        bottom = 0
                    };
                    IntPtr rectIntPtr = IntPtr.Zero;
                    IntPtr rectIntPtr2 = IntPtr.Zero;
                    rectIntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
                    Marshal.StructureToPtr(rect, rectIntPtr, true);
                    rectIntPtr2 = Marshal.AllocHGlobal(Marshal.SizeOf(rectIntPtr));
                    Marshal.StructureToPtr(rectIntPtr, rectIntPtr2, true);
                    effect.dispatcher(ref effect, (int)AEffectOpcodes.effEditGetRect, 0, 0, rectIntPtr2, 0);
                    IntPtr rectIntPtr3 = (IntPtr)Marshal.PtrToStructure(rectIntPtr2, typeof(IntPtr));
                    rect = (ERect)Marshal.PtrToStructure(rectIntPtr3, typeof(ERect));
                    Marshal.FreeHGlobal(rectIntPtr);
                    Marshal.FreeHGlobal(rectIntPtr2);
                    int width = rect.right - rect.left;
                    int height = rect.bottom - rect.top;
                    if (width < 100)
                        width = 100;
                    if (height < 100)
                        height = 100;
                    this.ClientSize = new Size(width, height);
                    break;
                case 0x113: //WM_TIMER
                    effect.dispatcher(ref effect, (int)AEffectOpcodes.effEditIdle, 0, 0, IntPtr.Zero, 0);
                    break;
                case 0x10: //WM_CLOSE
                    KillTimer(m.HWnd, 1);
                    effect.dispatcher(ref effect, (int)AEffectOpcodes.effEditClose, 0, 0, IntPtr.Zero, 0);
                    vst.IsOpenEditor = false;
                    break;
            }
        }

        private void VSTForm_Load(object sender, EventArgs e)
        {
        }

        [DllImport("user32.dll", ExactSpelling = true)]
        static extern IntPtr SetTimer(IntPtr hWnd, int nIDEvent, uint uElapse, IntPtr lpTimerFunc);

        [DllImport("user32.dll", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool KillTimer(IntPtr hWnd, int uIDEvent);
    }


}
