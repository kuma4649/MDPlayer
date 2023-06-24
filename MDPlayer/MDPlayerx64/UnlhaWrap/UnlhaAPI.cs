using System.Runtime.InteropServices;
using System.Text;

namespace MDPlayer.UnlhaWrap
{
    public class UnlhaAPI
    {

        //DLLモジュールをマップ
        [DllImport("kernel32", EntryPoint = "LoadLibrary", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false)]
        public extern static IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPTStr)] string lpFileName);
        //DLLモジュールの参照カウントを1つ減らす
        [DllImport("kernel32", EntryPoint = "FreeLibrary", SetLastError = true, ExactSpelling = true)]
        public extern static bool FreeLibrary(IntPtr hModule);
        //関数のアドレスを取得
        [DllImport("kernel32", EntryPoint = "GetProcAddress", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true)]
        public extern static IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);
        //ファイルを検索
        [DllImport("kernel32", EntryPoint = "SearchPath", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false)]
        public static extern uint SearchPath([MarshalAs(UnmanagedType.LPTStr)] string lpPath, [MarshalAs(UnmanagedType.LPTStr)] string lpFileName, [MarshalAs(UnmanagedType.LPTStr)] string lpExtension, uint nBufferLength, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpBuffer, out IntPtr lpFilePart);

        //DLL の版の取得
        public delegate ushort GetVersionDelegate();
        //DLL の実行状況の取得
        public delegate bool GetRunningDelegate();
        //書庫のチェック
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate bool CheckArchiveDelegate([MarshalAs(UnmanagedType.LPStr)] string _szFileName, int _iMode);
        //書庫操作一般
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate int ExecuteDelegate(IntPtr _hwnd, [MarshalAs(UnmanagedType.LPStr)] string _szCmdLine, [MarshalAs(UnmanagedType.LPStr)] StringBuilder _szOutput, uint _dwSize);
        //メモリーへの展開
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate int ExtractMemDelegate(IntPtr _hwnd, [MarshalAs(UnmanagedType.LPStr)] string _szCmdLine, [MarshalAs(UnmanagedType.LPArray)] byte[] _szBuffer, uint _dwSize, IntPtr _lpTime, IntPtr _lpwAttr, IntPtr _lpdwWriteSize);

        //書庫のオープン
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate IntPtr OpenArchiveDelegate(IntPtr _hwnd, [MarshalAs(UnmanagedType.LPStr)] string _szFileName, uint _dwMode);
        //書庫のクローズ
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate int CloseArchiveDelegate(IntPtr _harc);
        //格納ファイルの検索
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate int FindFirstDelegate(IntPtr _harc, [MarshalAs(UnmanagedType.LPStr)] string _szWildName, IntPtr _lpSubInfo);
        //格納ファイルの検索
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate int FindNextDelegate(IntPtr _harc, IntPtr _lpSubInfo);
        //格納ファイルのファイル名を得ます。
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate int GetFileNameDelegate(IntPtr _harc, [MarshalAs(UnmanagedType.LPStr)] StringBuilder _lpBuffer, uint _nSize);
        //格納ファイルのサイズを 64 ビット整数で得ます。
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        public delegate bool GetOriginalSizeExDelegate(IntPtr _harc, ref UInt64 _lpllSize);

    }
}
