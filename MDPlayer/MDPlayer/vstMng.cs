using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using System.Diagnostics;
using System.Threading;
using MDPlayer.form;

namespace MDPlayer
{
    public class vstMng
    {
        public static string[] chkFn = new string[]{
            "Jacobi.Vst.Core.dll"
            ,"Jacobi.Vst.Interop.dll"
        };

        public Setting setting = null;
        public MIDIParam[] midiParams = null;//ChipRegisterからインスタンスをもらう

        private List<vstInfo2> vstPlugins = new List<vstInfo2>();
        private List<vstInfo2> vstPluginsInst = new List<vstInfo2>();
        public List<vstInfo2> vstMidiOuts = new List<vstInfo2>();
        public List<int> vstMidiOutsType = new List<int>();




        public void vstparse()
        {
            while (vstPluginsInst.Count > 0)
            {
                if (vstPluginsInst[0] != null)
                {
                    if (vstPluginsInst[0].vstPlugins.PluginCommandStub != null) vstPluginsInst[0].vstPlugins.PluginCommandStub.EditorClose();
                    vstPluginsInst[0].vstPluginsForm.timer1.Enabled = false;
                    vstPluginsInst[0].location = vstPluginsInst[0].vstPluginsForm.Location;
                    vstPluginsInst[0].vstPluginsForm.Close();
                    if (vstPluginsInst[0].vstPlugins.PluginCommandStub != null) vstPluginsInst[0].vstPlugins.PluginCommandStub.StopProcess();
                    if (vstPluginsInst[0].vstPlugins.PluginCommandStub != null) vstPluginsInst[0].vstPlugins.PluginCommandStub.MainsChanged(false);
                    vstPluginsInst[0].vstPlugins.Dispose();
                }

                vstPluginsInst.RemoveAt(0);
            }

            while (vstPlugins.Count > 0)
            {
                if (vstPlugins[0] != null)
                {
                    if (vstPlugins[0].vstPlugins.PluginCommandStub != null) vstPlugins[0].vstPlugins.PluginCommandStub.EditorClose();
                    vstPlugins[0].vstPluginsForm.timer1.Enabled = false;
                    vstPlugins[0].location = vstPlugins[0].vstPluginsForm.Location;
                    vstPlugins[0].vstPluginsForm.Close();
                    if (vstPlugins[0].vstPlugins.PluginCommandStub != null) vstPlugins[0].vstPlugins.PluginCommandStub.StopProcess();
                    if (vstPlugins[0].vstPlugins.PluginCommandStub != null) vstPlugins[0].vstPlugins.PluginCommandStub.MainsChanged(false);
                    vstPlugins[0].vstPlugins.Dispose();
                }

                vstPlugins.RemoveAt(0);
            }
        }

        public void SetUpVstInstrument(KeyValuePair<string,int> kv)
        {
            VstPluginContext ctx = OpenPlugin(kv.Key);
            if (ctx == null) return;

            vstInfo2 vi = new vstInfo2();
            vi.key = DateTime.Now.Ticks.ToString();
            Thread.Sleep(1);
            vi.vstPlugins = ctx;
            vi.fileName = kv.Key;
            vi.isInstrument = true;

            ctx.PluginCommandStub.SetBlockSize(512);
            ctx.PluginCommandStub.SetSampleRate(Common.SampleRate);
            ctx.PluginCommandStub.MainsChanged(true);
            ctx.PluginCommandStub.StartProcess();
            vi.effectName = ctx.PluginCommandStub.GetEffectName();
            vi.editor = true;

            if (vi.editor)
            {
                frmVST dlg = new frmVST();
                dlg.PluginCommandStub = ctx.PluginCommandStub;
                dlg.Show(vi);
                vi.vstPluginsForm = dlg;
            }

            vstPluginsInst.Add(vi);
        }

        public void SetUpVstEffect()
        {
            for (int i = 0; i < setting.vst.VSTInfo.Length; i++)
            {
                if (setting.vst.VSTInfo[i] == null) continue;
                VstPluginContext ctx = OpenPlugin(setting.vst.VSTInfo[i].fileName);
                if (ctx == null) continue;

                vstInfo2 vi = new vstInfo2();
                vi.vstPlugins = ctx;
                vi.fileName = setting.vst.VSTInfo[i].fileName;
                vi.key = setting.vst.VSTInfo[i].key;

                ctx.PluginCommandStub.SetBlockSize(512);
                ctx.PluginCommandStub.SetSampleRate(Common.SampleRate / 1000.0f);
                ctx.PluginCommandStub.MainsChanged(true);
                ctx.PluginCommandStub.StartProcess();
                vi.effectName = ctx.PluginCommandStub.GetEffectName();
                vi.power = setting.vst.VSTInfo[i].power;
                vi.editor = setting.vst.VSTInfo[i].editor;
                vi.location = setting.vst.VSTInfo[i].location;
                vi.param = setting.vst.VSTInfo[i].param;

                if (vi.editor)
                {
                    frmVST dlg = new frmVST();
                    dlg.PluginCommandStub = ctx.PluginCommandStub;
                    dlg.Show(vi);
                    vi.vstPluginsForm = dlg;
                }

                if (vi.param != null)
                {
                    for (int p = 0; p < vi.param.Length; p++)
                    {
                        ctx.PluginCommandStub.SetParameter(p, vi.param[p]);
                    }
                }

                vstPlugins.Add(vi);
            }
        }

        public void SetupVstMidiOut(midiOutInfo mi)
        {
            int vn = -1;
            int vt = 0;
            vstInfo2 vmo = null;

            for (int j = 0; j < vstPluginsInst.Count; j++)
            {
                if (!vstPluginsInst[j].isInstrument || mi.fileName != vstPluginsInst[j].fileName) continue;
                bool k = false;
                foreach (vstInfo2 v in vstMidiOuts) if (v == vstPluginsInst[j]) { k = true; break; }
                if (k) continue;
                vn = j;
                vt = mi.type;
                break;
            }

            if (vn != -1)
            {
                try
                {
                    vmo = vstPluginsInst[vn];
                }
                catch
                {
                    vmo = null;
                }
            }

            if (vmo != null)
            {
                vstMidiOuts.Add(vmo);
                vstMidiOutsType.Add(vt);
            }

        }

        public void ReleaseAllMIDIout()
        {
            if (vstMidiOuts != null && vstMidiOuts.Count > 0)
            {
                vstMidiOuts.Clear();
                vstMidiOutsType.Clear();
            }
        }

        private void ReleaseAllPlugins()
        {
            foreach (vstInfo2 ctx in vstPlugins)
            {
                // dispose of all (unmanaged) resources
                ctx.vstPlugins.Dispose();
            }

            vstPlugins.Clear();
        }


        public void Close()
        {
            setting.vst.VSTInfo = null;
            List<vstInfo> vstlst = new List<vstInfo>();

            for (int i = 0; i < vstPlugins.Count; i++)
            {
                try
                {
                    vstPlugins[i].vstPluginsForm.timer1.Enabled = false;
                    vstPlugins[i].location = vstPlugins[i].vstPluginsForm.Location;
                    vstPlugins[i].vstPluginsForm.Close();
                }
                catch { }

                try
                {
                    if (vstPlugins[i].vstPlugins != null)
                    {
                        vstPlugins[i].vstPlugins.PluginCommandStub.EditorClose();
                        vstPlugins[i].vstPlugins.PluginCommandStub.StopProcess();
                        vstPlugins[i].vstPlugins.PluginCommandStub.MainsChanged(false);
                        int pc = vstPlugins[i].vstPlugins.PluginInfo.ParameterCount;
                        List<float> plst = new List<float>();
                        for (int p = 0; p < pc; p++)
                        {
                            float v = vstPlugins[i].vstPlugins.PluginCommandStub.GetParameter(p);
                            plst.Add(v);
                        }
                        vstPlugins[i].param = plst.ToArray();
                        vstPlugins[i].vstPlugins.Dispose();
                    }
                }
                catch { }

                vstInfo vi = new vstInfo();
                vi.editor = vstPlugins[i].editor;
                vi.fileName = vstPlugins[i].fileName;
                vi.key = vstPlugins[i].key;
                vi.effectName = vstPlugins[i].effectName;
                vi.power = vstPlugins[i].power;
                vi.location = vstPlugins[i].location;
                vi.param = vstPlugins[i].param;

                if (!vstPlugins[i].isInstrument) vstlst.Add(vi);
            }
            setting.vst.VSTInfo = vstlst.ToArray();


            for (int i = 0; i < vstPluginsInst.Count; i++)
            {
                try
                {
                    vstPluginsInst[i].vstPluginsForm.timer1.Enabled = false;
                    vstPluginsInst[i].location = vstPluginsInst[i].vstPluginsForm.Location;
                    vstPluginsInst[i].vstPluginsForm.Close();
                }
                catch { }

                try
                {
                    if (vstPluginsInst[i].vstPlugins != null)
                    {
                        vstPluginsInst[i].vstPlugins.PluginCommandStub.EditorClose();
                        vstPluginsInst[i].vstPlugins.PluginCommandStub.StopProcess();
                        vstPluginsInst[i].vstPlugins.PluginCommandStub.MainsChanged(false);
                        int pc = vstPluginsInst[i].vstPlugins.PluginInfo.ParameterCount;
                        List<float> plst = new List<float>();
                        for (int p = 0; p < pc; p++)
                        {
                            float v = vstPluginsInst[i].vstPlugins.PluginCommandStub.GetParameter(p);
                            plst.Add(v);
                        }
                        vstPluginsInst[i].param = plst.ToArray();
                        vstPluginsInst[i].vstPlugins.Dispose();
                    }
                }
                catch { }

                vstInfo vi = new vstInfo();
                vi.editor = vstPluginsInst[i].editor;
                vi.fileName = vstPluginsInst[i].fileName;
                vi.key = vstPluginsInst[i].key;
                vi.effectName = vstPluginsInst[i].effectName;
                vi.power = vstPluginsInst[i].power;
                vi.location = vstPluginsInst[i].location;
                vi.param = vstPluginsInst[i].param;

            }

        }

        public void VST_Update(short[] buffer, int offset, int sampleCount)
        {
            if (vstPlugins.Count < 1 && vstPluginsInst.Count < 1) return;
            if (buffer == null || buffer.Length < 1 || sampleCount == 0) return;

            try
            {
                //if (trdStopped) return;

                int blockSize = sampleCount / 2;

                for (int i = 0; i < vstPluginsInst.Count; i++)
                {
                    vstInfo2 info2 = vstPluginsInst[i];
                    VstPluginContext PluginContext = info2.vstPlugins;
                    if (PluginContext == null) continue;
                    if (PluginContext.PluginCommandStub == null) continue;


                    int inputCount = info2.vstPlugins.PluginInfo.AudioInputCount;
                    int outputCount = info2.vstPlugins.PluginInfo.AudioOutputCount;

                    using (VstAudioBufferManager inputMgr = new VstAudioBufferManager(inputCount, blockSize))
                    {
                        using (VstAudioBufferManager outputMgr = new VstAudioBufferManager(outputCount, blockSize))
                        {
                            VstAudioBuffer[] inputBuffers = inputMgr.ToArray();
                            VstAudioBuffer[] outputBuffers = outputMgr.ToArray();

                            if (inputCount != 0)
                            {
                                inputMgr.ClearBuffer(inputBuffers[0]);
                                inputMgr.ClearBuffer(inputBuffers[1]);

                                for (int j = 0; j < blockSize; j++)
                                {
                                    // generate a value between -1.0 and 1.0
                                    inputBuffers[0][j] = buffer[j * 2 + offset + 0] / (float)short.MaxValue;
                                    inputBuffers[1][j] = buffer[j * 2 + offset + 1] / (float)short.MaxValue;
                                }
                            }

                            outputMgr.ClearBuffer(outputBuffers[0]);
                            outputMgr.ClearBuffer(outputBuffers[1]);

                            PluginContext.PluginCommandStub.ProcessEvents(info2.lstEvent.ToArray());
                            info2.lstEvent.Clear();


                            PluginContext.PluginCommandStub.ProcessReplacing(inputBuffers, outputBuffers);

                            for (int j = 0; j < blockSize; j++)
                            {
                                // generate a value between -1.0 and 1.0
                                if (inputCount == 0)
                                {
                                    buffer[j * 2 + offset + 0] += (short)(outputBuffers[0][j] * short.MaxValue);
                                    buffer[j * 2 + offset + 1] += (short)(outputBuffers[1][j] * short.MaxValue);
                                }
                                else
                                {
                                    buffer[j * 2 + offset + 0] = (short)(outputBuffers[0][j] * short.MaxValue);
                                    buffer[j * 2 + offset + 1] = (short)(outputBuffers[1][j] * short.MaxValue);
                                }
                            }

                        }
                    }
                }

                for (int i = 0; i < vstPlugins.Count; i++)
                {
                    vstInfo2 info2 = vstPlugins[i];
                    VstPluginContext PluginContext = info2.vstPlugins;
                    if (PluginContext == null) continue;
                    if (PluginContext.PluginCommandStub == null) continue;


                    int inputCount = info2.vstPlugins.PluginInfo.AudioInputCount;
                    int outputCount = info2.vstPlugins.PluginInfo.AudioOutputCount;

                    using (VstAudioBufferManager inputMgr = new VstAudioBufferManager(inputCount, blockSize))
                    {
                        using (VstAudioBufferManager outputMgr = new VstAudioBufferManager(outputCount, blockSize))
                        {
                            VstAudioBuffer[] inputBuffers = inputMgr.ToArray();
                            VstAudioBuffer[] outputBuffers = outputMgr.ToArray();

                            if (inputCount != 0)
                            {
                                inputMgr.ClearBuffer(inputBuffers[0]);
                                inputMgr.ClearBuffer(inputBuffers[1]);

                                for (int j = 0; j < blockSize; j++)
                                {
                                    // generate a value between -1.0 and 1.0
                                    inputBuffers[0][j] = buffer[j * 2 + offset + 0] / (float)short.MaxValue;
                                    inputBuffers[1][j] = buffer[j * 2 + offset + 1] / (float)short.MaxValue;
                                }
                            }

                            outputMgr.ClearBuffer(outputBuffers[0]);
                            outputMgr.ClearBuffer(outputBuffers[1]);

                            PluginContext.PluginCommandStub.ProcessReplacing(inputBuffers, outputBuffers);

                            for (int j = 0; j < blockSize; j++)
                            {
                                // generate a value between -1.0 and 1.0
                                if (inputCount == 0)
                                {
                                    buffer[j * 2 + offset + 0] += (short)(outputBuffers[0][j] * short.MaxValue);
                                    buffer[j * 2 + offset + 1] += (short)(outputBuffers[1][j] * short.MaxValue);
                                }
                                else
                                {
                                    buffer[j * 2 + offset + 0] = (short)(outputBuffers[0][j] * short.MaxValue);
                                    buffer[j * 2 + offset + 1] = (short)(outputBuffers[1][j] * short.MaxValue);
                                }
                            }

                        }
                    }
                }

            }
            catch { }
        }



        public void sendMIDIout(EnmModel model, int num, byte cmd, byte prm1, byte prm2, int deltaFrames = 0)
        {
            if (model == EnmModel.RealModel) return;
            if (vstMidiOuts == null) return;
            if (num >= vstMidiOuts.Count) return;
            if (vstMidiOuts[num] == null) return;

            VstMidiEvent evt = new VstMidiEvent(
                deltaFrames
                , 0//noteLength
                , 0//noteOffset
                , new byte[] { cmd, prm1, prm2 }
                , 0//detune
                , 0//noteOffVelocity
                );
            vstMidiOuts[num].AddMidiEvent(evt);
            if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1, prm2 });
        }

        public void sendMIDIout(EnmModel model, int num, byte cmd, byte prm1, int deltaFrames = 0)
        {
            if (model == EnmModel.RealModel) return;
            if (vstMidiOuts == null) return;
            if (num >= vstMidiOuts.Count) return;
            if (vstMidiOuts[num] == null) return;

            Jacobi.Vst.Core.VstMidiEvent evt = new Jacobi.Vst.Core.VstMidiEvent(
                deltaFrames
                , 0//noteLength
                , 0//noteOffset
                , new byte[] { cmd, prm1 }
                , 0//detune
                , 0//noteOffVelocity
                );
            vstMidiOuts[num].AddMidiEvent(evt);
            if (num < midiParams.Length) midiParams[num].SendBuffer(new byte[] { cmd, prm1 });
        }

        public void sendMIDIout(EnmModel model, int num, byte[] data, int deltaFrames = 0)
        {
            if (model == EnmModel.RealModel) return;
            if (vstMidiOuts == null) return;
            if (num >= vstMidiOuts.Count) return;
            if (vstMidiOuts[num] == null) return;

            VstMidiEvent evt = new VstMidiEvent(
                deltaFrames
                , 0//noteLength
                , 0//noteOffset
                , data
                , 0//detune
                , 0//noteOffVelocity
                );
            vstMidiOuts[num].AddMidiEvent(evt);
            if (num < midiParams.Length) midiParams[num].SendBuffer(data);
        }

        public void resetAllMIDIout(EnmModel model)
        {
            if (model == EnmModel.RealModel) return;

            if (vstMidiOuts == null) return;
            for (int i = 0; i < vstMidiOuts.Count; i++)
            {
                if (vstMidiOuts[i] == null) continue;
                if (vstMidiOuts[i].vstPlugins == null) continue;
                if (vstMidiOuts[i].vstPlugins.PluginCommandStub == null) continue;

                try
                {
                    List<byte> dat = new List<byte>();
                    for (int ch = 0; ch < 16; ch++)
                    {
                        sendMIDIout(EnmModel.VirtualModel, i, new byte[] { (byte)(0xb0 + ch), 120, 0x00 }, 0);
                        sendMIDIout(EnmModel.VirtualModel, i, new byte[] { (byte)(0xb0 + ch), 64, 0x00 }, 0);
                    }

                }
                catch { }
            }
        }



        public VstPluginContext OpenPlugin(string pluginPath)
        {
            try
            {
                HostCommandStub hostCmdStub = new HostCommandStub();
                hostCmdStub.PluginCalled += new EventHandler<PluginCalledEventArgs>(HostCmdStub_PluginCalled);

                VstPluginContext ctx = VstPluginContext.Create(pluginPath, hostCmdStub);

                // add custom data to the context
                ctx.Set("PluginPath", pluginPath);
                ctx.Set("HostCmdStub", hostCmdStub);

                // actually open the plugin itself
                ctx.PluginCommandStub.Open();

                return ctx;
            }
            catch (Exception e)
            {
                log.ForcedWrite(e);
                //MessageBox.Show(this, e.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        private static void HostCmdStub_PluginCalled(object sender, PluginCalledEventArgs e)
        {
            HostCommandStub hostCmdStub = (HostCommandStub)sender;

            // can be null when called from inside the plugin main entry point.
            if (hostCmdStub.PluginContext.PluginInfo != null)
            {
                Debug.WriteLine("Plugin " + hostCmdStub.PluginContext.PluginInfo.PluginID + " called:" + e.Message);
            }
            else
            {
                Debug.WriteLine("The loading Plugin called:" + e.Message);
            }
        }

        public List<vstInfo2> getVSTInfos()
        {
            return vstPlugins;
        }

        public vstInfo getVSTInfo(string filename)
        {
            VstPluginContext ctx = OpenPlugin(filename);
            if (ctx == null) return null;

            vstInfo ret = new vstInfo();
            ret.effectName = ctx.PluginCommandStub.GetEffectName();
            ret.productName = ctx.PluginCommandStub.GetProductString();
            ret.vendorName = ctx.PluginCommandStub.GetVendorString();
            ret.programName = ctx.PluginCommandStub.GetProgramName();
            ret.fileName = filename;
            ret.midiInputChannels = ctx.PluginCommandStub.GetNumberOfMidiInputChannels();
            ret.midiOutputChannels = ctx.PluginCommandStub.GetNumberOfMidiOutputChannels();
            ctx.PluginCommandStub.Close();

            return ret;
        }

        public bool addVSTeffect(string fileName)
        {
            VstPluginContext ctx = OpenPlugin(fileName);
            if (ctx == null) return false;

            //Stop();

            vstInfo2 vi = new vstInfo2();
            vi.vstPlugins = ctx;
            vi.fileName = fileName;
            vi.key = DateTime.Now.Ticks.ToString();
            Thread.Sleep(1);

            ctx.PluginCommandStub.SetBlockSize(512);
            ctx.PluginCommandStub.SetSampleRate(Common.SampleRate);
            ctx.PluginCommandStub.MainsChanged(true);
            ctx.PluginCommandStub.StartProcess();
            vi.effectName = ctx.PluginCommandStub.GetEffectName();
            vi.power = true;
            ctx.PluginCommandStub.GetParameterProperties(0);


            frmVST dlg = new frmVST();
            dlg.PluginCommandStub = ctx.PluginCommandStub;
            dlg.Show(vi);
            vi.vstPluginsForm = dlg;
            vi.editor = true;

            vstPlugins.Add(vi);

            List<vstInfo> lvi = new List<vstInfo>();
            foreach (vstInfo2 vi2 in vstPlugins)
            {
                vstInfo v = new vstInfo();
                v.editor = vi.editor;
                v.effectName = vi.effectName;
                v.fileName = vi.fileName;
                v.key = vi.key;
                v.location = vi.location;
                v.midiInputChannels = vi.midiInputChannels;
                v.midiOutputChannels = vi.midiOutputChannels;
                v.param = vi.param;
                v.power = vi.power;
                v.productName = vi.productName;
                v.programName = vi.programName;
                v.vendorName = vi.vendorName;
                lvi.Add(v);
            }
            setting.vst.VSTInfo = lvi.ToArray();

            return true;
        }

        public bool delVSTeffect(string key)
        {
            if (key == "")
            {
                for (int i = 0; i < vstPlugins.Count; i++)
                {
                    try
                    {
                        if (vstPlugins[i].vstPlugins != null)
                        {
                            vstPlugins[i].vstPluginsForm.timer1.Enabled = false;
                            vstPlugins[i].location = vstPlugins[i].vstPluginsForm.Location;
                            vstPlugins[i].vstPluginsForm.Close();
                            vstPlugins[i].vstPlugins.PluginCommandStub.EditorClose();
                            vstPlugins[i].vstPlugins.PluginCommandStub.StopProcess();
                            vstPlugins[i].vstPlugins.PluginCommandStub.MainsChanged(false);
                            vstPlugins[i].vstPlugins.Dispose();
                        }
                    }
                    catch { }
                }
                vstPlugins.Clear();
                setting.vst.VSTInfo = new vstInfo[0];
            }
            else
            {
                int ind = -1;
                for (int i = 0; i < vstPlugins.Count; i++)
                {
                    //if (vstPlugins[i].fileName == fileName)
                    if (vstPlugins[i].key == key)
                    {
                        ind = i;
                        break;
                    }
                }

                if (ind != -1)
                {
                    try
                    {
                        if (vstPlugins[ind].vstPlugins != null)
                        {
                            vstPlugins[ind].vstPluginsForm.timer1.Enabled = false;
                            vstPlugins[ind].location = vstPlugins[ind].vstPluginsForm.Location;
                            vstPlugins[ind].vstPluginsForm.Close();
                            vstPlugins[ind].vstPlugins.PluginCommandStub.EditorClose();
                            vstPlugins[ind].vstPlugins.PluginCommandStub.StopProcess();
                            vstPlugins[ind].vstPlugins.PluginCommandStub.MainsChanged(false);
                            vstPlugins[ind].vstPlugins.Dispose();
                        }
                    }
                    catch { }
                    vstPlugins.RemoveAt(ind);
                }

                List<vstInfo> nvst = new List<vstInfo>();
                foreach (vstInfo vi in setting.vst.VSTInfo)
                {
                    if (vi.key == key) continue;
                    nvst.Add(vi);
                }
                setting.vst.VSTInfo = nvst.ToArray();
            }

            return true;
        }




        /// <summary>
        /// The HostCommandStub class represents the part of the host that a plugin can call.
        /// </summary>
        public class HostCommandStub : IVstHostCommandStub
        {
            /// <summary>
            /// Raised when one of the methods is called.
            /// </summary>
            public event EventHandler<PluginCalledEventArgs> PluginCalled;

            private void RaisePluginCalled(string message)
            {
                EventHandler<PluginCalledEventArgs> handler = PluginCalled;

                if (handler != null)
                {
                    handler(this, new PluginCalledEventArgs(message));
                }
            }

            #region IVstHostCommandsStub Members

            /// <inheritdoc />
            public IVstPluginContext PluginContext { get; set; }

            #endregion

            #region IVstHostCommands20 Members

            /// <inheritdoc />
            public bool BeginEdit(int index)
            {
                RaisePluginCalled("BeginEdit(" + index + ")");

                return false;
            }

            /// <inheritdoc />
            public VstCanDoResult CanDo(string cando)
            {
                RaisePluginCalled("CanDo(" + cando + ")");
                return VstCanDoResult.Unknown;
            }

            /// <inheritdoc />
            public bool CloseFileSelector(VstFileSelect fileSelect)
            {
                RaisePluginCalled("CloseFileSelector(" + fileSelect.Command + ")");
                return false;
            }

            /// <inheritdoc />
            public bool EndEdit(int index)
            {
                RaisePluginCalled("EndEdit(" + index + ")");
                return false;
            }

            /// <inheritdoc />
            public VstAutomationStates GetAutomationState()
            {
                RaisePluginCalled("GetAutomationState()");
                return VstAutomationStates.Off;
            }

            /// <inheritdoc />
            public int GetBlockSize()
            {
                RaisePluginCalled("GetBlockSize()");
                return 1024;
            }

            /// <inheritdoc />
            public string GetDirectory()
            {
                RaisePluginCalled("GetDirectory()");
                return null;
            }

            /// <inheritdoc />
            public int GetInputLatency()
            {
                RaisePluginCalled("GetInputLatency()");
                return 0;
            }

            /// <inheritdoc />
            public VstHostLanguage GetLanguage()
            {
                RaisePluginCalled("GetLanguage()");
                return VstHostLanguage.NotSupported;
            }

            /// <inheritdoc />
            public int GetOutputLatency()
            {
                RaisePluginCalled("GetOutputLatency()");
                return 0;
            }

            /// <inheritdoc />
            public VstProcessLevels GetProcessLevel()
            {
                RaisePluginCalled("GetProcessLevel()");
                return VstProcessLevels.Unknown;
            }

            /// <inheritdoc />
            public string GetProductString()
            {
                RaisePluginCalled("GetProductString()");
                return "VST.NET";
            }

            /// <inheritdoc />
            public float GetSampleRate()
            {
                RaisePluginCalled("GetSampleRate()");
                return Common.SampleRate / 1000.0f;
            }

            /// <inheritdoc />
            public VstTimeInfo GetTimeInfo(VstTimeInfoFlags filterFlags)
            {
                //RaisePluginCalled("GetTimeInfo(" + filterFlags + ")");
                VstTimeInfo vti = new VstTimeInfo();
                vti.SamplePosition = 0;
                vti.SampleRate = Common.SampleRate / 1000.0f;
                vti.NanoSeconds = 0;
                vti.PpqPosition = 0;
                vti.Tempo = 120;
                vti.BarStartPosition = 0;
                vti.CycleStartPosition = 0;
                vti.CycleEndPosition = 0;
                vti.TimeSignatureNumerator = 4;
                vti.TimeSignatureDenominator = 4;
                vti.SmpteOffset = 0;
                vti.SmpteFrameRate = VstSmpteFrameRate.Smpte24fps;
                vti.SamplesToNearestClock = 0;
                vti.Flags = VstTimeInfoFlags.NanoSecondsValid
                    | VstTimeInfoFlags.PpqPositionValid
                    | VstTimeInfoFlags.TempoValid
                    | VstTimeInfoFlags.TimeSignatureValid;
                return vti;
            }

            /// <inheritdoc />
            public string GetVendorString()
            {
                RaisePluginCalled("GetVendorString()");
                return "";
            }

            /// <inheritdoc />
            public int GetVendorVersion()
            {
                RaisePluginCalled("GetVendorVersion()");
                return 1000;
            }

            /// <inheritdoc />
            public bool IoChanged()
            {
                RaisePluginCalled("IoChanged()");
                return false;
            }

            /// <inheritdoc />
            public bool OpenFileSelector(VstFileSelect fileSelect)
            {
                RaisePluginCalled("OpenFileSelector(" + fileSelect.Command + ")");
                return false;
            }

            /// <inheritdoc />
            public bool ProcessEvents(VstEvent[] events)
            {
                RaisePluginCalled("ProcessEvents(" + events.Length + ")");
                return false;
            }

            /// <inheritdoc />
            public bool SizeWindow(int width, int height)
            {
                RaisePluginCalled("SizeWindow(" + width + ", " + height + ")");
                return false;
            }

            /// <inheritdoc />
            public bool UpdateDisplay()
            {
                RaisePluginCalled("UpdateDisplay()");
                return false;
            }

            #endregion

            #region IVstHostCommands10 Members

            /// <inheritdoc />
            public int GetCurrentPluginID()
            {
                RaisePluginCalled("GetCurrentPluginID()");
                return PluginContext.PluginInfo.PluginID;
            }

            /// <inheritdoc />
            public int GetVersion()
            {
                RaisePluginCalled("GetVersion()");
                return 1000;
            }

            /// <inheritdoc />
            public void ProcessIdle()
            {
                RaisePluginCalled("ProcessIdle()");
            }

            /// <inheritdoc />
            public void SetParameterAutomated(int index, float value)
            {
                RaisePluginCalled("SetParameterAutomated(" + index + ", " + value + ")");
            }

            #endregion
        }

        /// <summary>
        /// Event arguments used when one of the mehtods is called.
        /// </summary>
        public class PluginCalledEventArgs : EventArgs
        {
            /// <summary>
            /// Constructs a new instance with a <paramref name="message"/>.
            /// </summary>
            /// <param name="message"></param>
            public PluginCalledEventArgs(string message)
            {
                Message = message;
            }

            /// <summary>
            /// Gets the message.
            /// </summary>
            public string Message { get; private set; }
        }

        public class vstInfo2 : vstInfo
        {
            public VstPluginContext vstPlugins = null;
            public form.frmVST vstPluginsForm = null;

            //実際にVSTiかどうかは問わない
            public bool isInstrument = false;
            public List<VstMidiEvent> lstEvent = new List<VstMidiEvent>();

            public void AddMidiEvent(VstMidiEvent evt)
            {
                lstEvent.Add(evt);
            }
        }


    }
}
