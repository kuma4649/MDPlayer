using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.UnlhaWrap
{
    public class UnlhaCmd
    {
        private const string dllName = "unlha32.dll";

        private IntPtr LoadDll(string archiveFile)
        {
            //DLLの存在を確認
            IntPtr pathPtr;
            if (UnlhaAPI.SearchPath(null, dllName, null, 0, null, out pathPtr) == 0)
            {
                throw new ApplicationException(
                    dllName + "が見つかりません。");
            }

            //DLLをロード
            IntPtr hmod = UnlhaAPI.LoadLibrary(dllName);
            if (hmod == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            try
            {
                IntPtr funcAddr;

                //DLLのチェック
                funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaGetVersion");
                if (funcAddr == IntPtr.Zero)
                {
                    throw new ApplicationException(
                        dllName + "がインストールされていません。");
                }
                UnlhaAPI.GetVersionDelegate getVersion =
                    (UnlhaAPI.GetVersionDelegate)Marshal.GetDelegateForFunctionPointer(
                    funcAddr, typeof(UnlhaAPI.GetVersionDelegate));
                ushort ver = getVersion();
                if (ver < 300)
                {
                    throw new ApplicationException(string.Format(dllName + "はバージョン3.00を要求。Ver{0:0.00}", ver / 100f));
                }

                //動作中かチェック
                funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaGetRunning");
                if (funcAddr != IntPtr.Zero)
                {
                    UnlhaAPI.GetRunningDelegate getRunning =
                        (UnlhaAPI.GetRunningDelegate)Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(UnlhaAPI.GetRunningDelegate));
                    if (getRunning())
                    {
                        throw new ApplicationException(
                            dllName + "が現在動作中です。");
                    }
                }

                //展開できるかチェック
                funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaCheckArchive");
                if (funcAddr == IntPtr.Zero)
                {
                    throw new ApplicationException(
                       "UnlhaCheckArchiveのアドレスを取得できませんでした。");
                }
                UnlhaAPI.CheckArchiveDelegate checkArchive =
                    (UnlhaAPI.CheckArchiveDelegate)Marshal.GetDelegateForFunctionPointer(
                    funcAddr, typeof(UnlhaAPI.CheckArchiveDelegate));
                if (!checkArchive(archiveFile, 0))
                {
                    throw new ApplicationException(
                        dllName + "では展開できません。");
                }

            }
            catch
            {
                //開放する
                UnlhaAPI.FreeLibrary(hmod);
                throw;
            }

            return hmod;
        }

        public List<Tuple<string,UInt64>> GetFileList(string archiveFile,string wildCard)
        {

            IntPtr hmod = LoadDll(archiveFile);
            IntPtr funcAddr;

            try
            {
                //open
                funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaOpenArchive");
                if (funcAddr == IntPtr.Zero)
                {
                    throw new ApplicationException(
                       "Unlhaのアドレスを取得できませんでした。");
                }
                UnlhaAPI.OpenArchiveDelegate openArchive =
                    (UnlhaAPI.OpenArchiveDelegate)Marshal.GetDelegateForFunctionPointer(
                    funcAddr, typeof(UnlhaAPI.OpenArchiveDelegate));
                IntPtr harc = openArchive(IntPtr.Zero, archiveFile, 0);

                try
                {
                    //findFirst
                    funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaFindFirst");
                    if (funcAddr == IntPtr.Zero)
                    {
                        throw new ApplicationException(
                           "Unlhaのアドレスを取得できませんでした。");
                    }
                    UnlhaAPI.FindFirstDelegate findFirst =
                        (UnlhaAPI.FindFirstDelegate)Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(UnlhaAPI.FindFirstDelegate));
                    int ret = findFirst(harc, wildCard, IntPtr.Zero);

                    List<Tuple<string, UInt64>> result = new List<Tuple<string, ulong>>();

                    do
                    {

                        //UnlhaGetFileName
                        funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaGetFileName");
                        if (funcAddr == IntPtr.Zero)
                        {
                            throw new ApplicationException(
                               "Unlhaのアドレスを取得できませんでした。");
                        }
                        UnlhaAPI.GetFileNameDelegate getFileName =
                            (UnlhaAPI.GetFileNameDelegate)Marshal.GetDelegateForFunctionPointer(
                            funcAddr, typeof(UnlhaAPI.GetFileNameDelegate));
                        StringBuilder fn = new StringBuilder(1024);
                        ret = getFileName(harc, fn, 1024);

                        //UnlhaGetOriginalSizeEx
                        funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaGetOriginalSizeEx");
                        if (funcAddr == IntPtr.Zero)
                        {
                            throw new ApplicationException(
                               "Unlhaのアドレスを取得できませんでした。");
                        }
                        UnlhaAPI.GetOriginalSizeExDelegate getOriginalSizeEx =
                            (UnlhaAPI.GetOriginalSizeExDelegate)Marshal.GetDelegateForFunctionPointer(
                            funcAddr, typeof(UnlhaAPI.GetOriginalSizeExDelegate));
                        UInt64 size = 0;
                        bool res = getOriginalSizeEx(harc, ref size);

                        Tuple<string, UInt64> item = new Tuple<string, ulong>(fn.ToString(), size);
                        result.Add(item);

                        //nextFirst
                        funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaFindNext");
                        if (funcAddr == IntPtr.Zero)
                        {
                            throw new ApplicationException(
                               "Unlhaのアドレスを取得できませんでした。");
                        }
                        UnlhaAPI.FindNextDelegate findNext =
                            (UnlhaAPI.FindNextDelegate)Marshal.GetDelegateForFunctionPointer(
                            funcAddr, typeof(UnlhaAPI.FindNextDelegate));
                        ret = findNext(harc, IntPtr.Zero);

                    } while (ret == 0);

                    return result;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    //close
                    funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaCloseArchive");
                    if (funcAddr == IntPtr.Zero)
                    {
                        throw new ApplicationException(
                           "Unlhaのアドレスを取得できませんでした。");
                    }
                    UnlhaAPI.CloseArchiveDelegate closeArchive =
                        (UnlhaAPI.CloseArchiveDelegate)Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(UnlhaAPI.CloseArchiveDelegate));
                    closeArchive(harc);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                UnlhaAPI.FreeLibrary(hmod);
            }

            return null;
        }

        public byte[] GetFileByte(string archiveFile, string fileName)
        {

            IntPtr hmod = LoadDll(archiveFile);
            IntPtr funcAddr;
            int ret;

            try
            {

                //open
                funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaOpenArchive");
                if (funcAddr == IntPtr.Zero)
                {
                    throw new ApplicationException(
                       "Unlhaのアドレスを取得できませんでした。");
                }
                UnlhaAPI.OpenArchiveDelegate openArchive =
                    (UnlhaAPI.OpenArchiveDelegate)Marshal.GetDelegateForFunctionPointer(
                    funcAddr, typeof(UnlhaAPI.OpenArchiveDelegate));
                IntPtr harc = openArchive(IntPtr.Zero, archiveFile, 0);
                Tuple<string, UInt64> item;
                try
                {
                    //findFirst
                    funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaFindFirst");
                    if (funcAddr == IntPtr.Zero)
                    {
                        throw new ApplicationException(
                           "Unlhaのアドレスを取得できませんでした。");
                    }
                    UnlhaAPI.FindFirstDelegate findFirst =
                        (UnlhaAPI.FindFirstDelegate)Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(UnlhaAPI.FindFirstDelegate));
                    ret = findFirst(harc, fileName, IntPtr.Zero);

                    //UnlhaGetFileName
                    funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaGetFileName");
                    if (funcAddr == IntPtr.Zero)
                    {
                        throw new ApplicationException(
                           "Unlhaのアドレスを取得できませんでした。");
                    }
                    UnlhaAPI.GetFileNameDelegate getFileName =
                        (UnlhaAPI.GetFileNameDelegate)Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(UnlhaAPI.GetFileNameDelegate));
                    StringBuilder fn = new StringBuilder(1024);
                    ret = getFileName(harc, fn, 1024);

                    //UnlhaGetOriginalSizeEx
                    funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaGetOriginalSizeEx");
                    if (funcAddr == IntPtr.Zero)
                    {
                        throw new ApplicationException(
                           "Unlhaのアドレスを取得できませんでした。");
                    }
                    UnlhaAPI.GetOriginalSizeExDelegate getOriginalSizeEx =
                        (UnlhaAPI.GetOriginalSizeExDelegate)Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(UnlhaAPI.GetOriginalSizeExDelegate));
                    UInt64 size = 0;
                    bool res = getOriginalSizeEx(harc, ref size);

                    item = new Tuple<string, ulong>(fn.ToString(), size);

                }
                catch
                {
                    throw;
                }
                finally
                {
                    //close
                    funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaCloseArchive");
                    if (funcAddr == IntPtr.Zero)
                    {
                        throw new ApplicationException(
                           "Unlhaのアドレスを取得できませんでした。");
                    }
                    UnlhaAPI.CloseArchiveDelegate closeArchive =
                        (UnlhaAPI.CloseArchiveDelegate)Marshal.GetDelegateForFunctionPointer(
                        funcAddr, typeof(UnlhaAPI.CloseArchiveDelegate));
                    closeArchive(harc);
                }


                //展開する
                funcAddr = UnlhaAPI.GetProcAddress(hmod, "UnlhaExtractMem");
                if (funcAddr == IntPtr.Zero)
                {
                    throw new ApplicationException(
                       "Unlhaのアドレスを取得できませんでした。");
                }
                UnlhaAPI.ExtractMemDelegate extractMem =
                    (UnlhaAPI.ExtractMemDelegate)Marshal.GetDelegateForFunctionPointer(
                    funcAddr, typeof(UnlhaAPI.ExtractMemDelegate));
                byte[] buf = new byte[item.Item2];
                ret = extractMem(IntPtr.Zero,
                    //string.Format(command, archiveFile, baseDir),
                    string.Format("-l1 -n1 {0} c: {1}", archiveFile,fileName),
                    buf,
                    (uint)buf.Length,
                    IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                return buf;
            }
            catch
            {

            }
            finally
            {
                UnlhaAPI.FreeLibrary(hmod);
            }

            return null;
        }
    }
}
