using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MDPlayerx64
{
    class EcoQos
    {
        private static bool isEcoMode = false;
        private static DateTime lastActiveTime = DateTime.Now;
        private static System.Windows.Forms.Timer timer = new();

        public static void StartEcoQos()
        {
            lastActiveTime = DateTime.Now;
            isEcoMode = false;

            timer.Interval = 1000;

            timer.Tick += (s, e) =>
            {
                if (!isEcoMode && (DateTime.Now - lastActiveTime).TotalSeconds >= 10)
                {
                    EnterEfficiencyMode();   // Chrome式
                    SetThreadEcoQoS(true);   // EcoQoS API（失敗してもOK）
                    GC.Collect();
                    isEcoMode = true;
                }
            };
            timer.Start();
        }

        public static void ExitEcoMode()
        {
            timer.Stop();
            ExitEfficiencyMode();      // Chrome式
            SetThreadEcoQoS(false);    // EcoQoS API（失敗してもOK）
            isEcoMode = false;
            lastActiveTime = DateTime.Now;
        }

        // ---- Chrome式効率モード ----
        private static void EnterEfficiencyMode()
        {
            var p = Process.GetCurrentProcess();
            p.PriorityClass = ProcessPriorityClass.BelowNormal;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
        }

        private static void ExitEfficiencyMode()
        {
            var p = Process.GetCurrentProcess();
            p.PriorityClass = ProcessPriorityClass.Normal;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        // ---- EcoQoS API（失敗してもOK） ----

        [StructLayout(LayoutKind.Sequential)]
        struct THREAD_POWER_THROTTLING_STATE
        {
            public uint Version;
            public uint Reserved;
            public uint ControlMask;
            public uint StateMask;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetThreadInformation(
            IntPtr hThread,
            int ThreadInformationClass,
            ref THREAD_POWER_THROTTLING_STATE ThreadInformation,
            int ThreadInformationSize);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

        private static void SetThreadEcoQoS(bool enable)
        {
            const int ThreadPowerThrottling = 3;
            const uint EXECUTION_SPEED = 0x1;

            var state = new THREAD_POWER_THROTTLING_STATE
            {
                Version = 0,
                Reserved = 0,
                ControlMask = EXECUTION_SPEED,
                StateMask = enable ? EXECUTION_SPEED : 0
            };

            IntPtr hThread = GetCurrentThread();

            bool ok = SetThreadInformation(
                hThread,
                ThreadPowerThrottling,
                ref state,
                Marshal.SizeOf<THREAD_POWER_THROTTLING_STATE>());

            if (!ok)
            {
                Console.WriteLine("EcoQoS API Failed: " + Marshal.GetLastWin32Error());
            }
        }
    }
}
