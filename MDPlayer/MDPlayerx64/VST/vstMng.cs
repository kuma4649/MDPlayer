using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Host.Interop;
using MDPlayer.form;
using System.Diagnostics;

namespace MDPlayer
{
    public class vstMng
    {
        public static string[] chkFn = new string[]{
            //"Jacobi.Vst.Core.dll"
            //,"Jacobi.Vst.Interop.dll"
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
                    if (vstPluginsInst[0].vstPlugins.PluginCommandStub != null) vstPluginsInst[0].vstPlugins.PluginCommandStub.Commands.EditorClose();
                    vstPluginsInst[0].vstPluginsForm.timer1.Enabled = false;
                    vstPluginsInst[0].location = vstPluginsInst[0].vstPluginsForm.Location;
                    vstPluginsInst[0].vstPluginsForm.Close();
                    if (vstPluginsInst[0].vstPlugins.PluginCommandStub != null) vstPluginsInst[0].vstPlugins.PluginCommandStub.Commands.StopProcess();
                    if (vstPluginsInst[0].vstPlugins.PluginCommandStub != null) vstPluginsInst[0].vstPlugins.PluginCommandStub.Commands.MainsChanged(false);
                    vstPluginsInst[0].vstPlugins.Dispose();
                }

                vstPluginsInst.RemoveAt(0);
            }

            while (vstPlugins.Count > 0)
            {
                if (vstPlugins[0] != null)
                {
                    if (vstPlugins[0].vstPlugins.PluginCommandStub != null) vstPlugins[0].vstPlugins.PluginCommandStub.Commands.EditorClose();
                    vstPlugins[0].vstPluginsForm.timer1.Enabled = false;
                    vstPlugins[0].location = vstPlugins[0].vstPluginsForm.Location;
                    vstPlugins[0].vstPluginsForm.Close();
                    if (vstPlugins[0].vstPlugins.PluginCommandStub != null) vstPlugins[0].vstPlugins.PluginCommandStub.Commands.StopProcess();
                    if (vstPlugins[0].vstPlugins.PluginCommandStub != null) vstPlugins[0].vstPlugins.PluginCommandStub.Commands.MainsChanged(false);
                    vstPlugins[0].vstPlugins.Dispose();
                }

                vstPlugins.RemoveAt(0);
            }
        }

        public void SetUpVstInstrument(KeyValuePair<string, int> kv)
        {
            VstPluginContext ctx = OpenPlugin(kv.Key);
            if (ctx == null) return;

            vstInfo2 vi = new vstInfo2();
            vi.key = DateTime.Now.Ticks.ToString();
            Thread.Sleep(1);
            vi.vstPlugins = ctx;
            vi.fileName = kv.Key;
            vi.isInstrument = true;

            ctx.PluginCommandStub.Commands.SetBlockSize(512);
            ctx.PluginCommandStub.Commands.SetSampleRate(setting.outputDevice.SampleRate);
            ctx.PluginCommandStub.Commands.MainsChanged(true);
            ctx.PluginCommandStub.Commands.StartProcess();
            vi.effectName = ctx.PluginCommandStub.Commands.GetEffectName();
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

                ctx.PluginCommandStub.Commands.SetBlockSize(512);
                ctx.PluginCommandStub.Commands.SetSampleRate(setting.outputDevice.SampleRate / 1000.0f);
                ctx.PluginCommandStub.Commands.MainsChanged(true);
                ctx.PluginCommandStub.Commands.StartProcess();
                vi.effectName = ctx.PluginCommandStub.Commands.GetEffectName();
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
                        ctx.PluginCommandStub.Commands.SetParameter(p, vi.param[p]);
                    }
                }

                vstPlugins.Add(vi);
            }
        }

        public void SetupVstMidiOut(MidiOutInfo mi)
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
                        vstPlugins[i].vstPlugins.PluginCommandStub.Commands.EditorClose();
                        vstPlugins[i].vstPlugins.PluginCommandStub.Commands.StopProcess();
                        vstPlugins[i].vstPlugins.PluginCommandStub.Commands.MainsChanged(false);
                        int pc = vstPlugins[i].vstPlugins.PluginInfo.ParameterCount;
                        List<float> plst = new List<float>();
                        for (int p = 0; p < pc; p++)
                        {
                            float v = vstPlugins[i].vstPlugins.PluginCommandStub.Commands.GetParameter(p);
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
                        vstPluginsInst[i].vstPlugins.PluginCommandStub.Commands.EditorClose();
                        vstPluginsInst[i].vstPlugins.PluginCommandStub.Commands.StopProcess();
                        vstPluginsInst[i].vstPlugins.PluginCommandStub.Commands.MainsChanged(false);
                        int pc = vstPluginsInst[i].vstPlugins.PluginInfo.ParameterCount;
                        List<float> plst = new List<float>();
                        for (int p = 0; p < pc; p++)
                        {
                            float v = vstPluginsInst[i].vstPlugins.PluginCommandStub.Commands.GetParameter(p);
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
                            VstAudioBuffer[] inputBuffers = inputMgr.Buffers.ToArray();
                            VstAudioBuffer[] outputBuffers = outputMgr.Buffers.ToArray();

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

                            PluginContext.PluginCommandStub.Commands.ProcessEvents(info2.lstEvent.ToArray());
                            info2.lstEvent.Clear();


                            PluginContext.PluginCommandStub.Commands.ProcessReplacing(inputBuffers, outputBuffers);

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
                            VstAudioBuffer[] inputBuffers = inputMgr.Buffers.ToArray();
                            VstAudioBuffer[] outputBuffers = outputMgr.Buffers.ToArray();

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

                            PluginContext.PluginCommandStub.Commands.ProcessReplacing(inputBuffers, outputBuffers);

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
                HostCommandStub hostCmdStub = new HostCommandStub(setting);
                hostCmdStub.PluginCalled += new EventHandler<PluginCalledEventArgs>(HostCmdStub_PluginCalled);

                VstPluginContext ctx = VstPluginContext.Create(pluginPath, hostCmdStub);

                // add custom data to the context
                ctx.Set("PluginPath", pluginPath);
                ctx.Set("HostCmdStub", hostCmdStub);

                // actually open the plugin itself
                ctx.PluginCommandStub.Commands.Open();

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
            ret.effectName = ctx.PluginCommandStub.Commands.GetEffectName();
            ret.productName = ctx.PluginCommandStub.Commands.GetProductString();
            ret.vendorName = ctx.PluginCommandStub.Commands.GetVendorString();
            ret.programName = ctx.PluginCommandStub.Commands.GetProgramName();
            ret.fileName = filename;
            ret.midiInputChannels = ctx.PluginCommandStub.Commands.GetNumberOfMidiInputChannels();
            ret.midiOutputChannels = ctx.PluginCommandStub.Commands.GetNumberOfMidiOutputChannels();
            ctx.PluginCommandStub.Commands.Close();

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

            ctx.PluginCommandStub.Commands.SetBlockSize(512);
            ctx.PluginCommandStub.Commands.SetSampleRate(setting.outputDevice.SampleRate);
            ctx.PluginCommandStub.Commands.MainsChanged(true);
            ctx.PluginCommandStub.Commands.StartProcess();
            vi.effectName = ctx.PluginCommandStub.Commands.GetEffectName();
            vi.power = true;
            ctx.PluginCommandStub.Commands.GetParameterProperties(0);


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
                            vstPlugins[i].vstPlugins.PluginCommandStub.Commands.EditorClose();
                            vstPlugins[i].vstPlugins.PluginCommandStub.Commands.StopProcess();
                            vstPlugins[i].vstPlugins.PluginCommandStub.Commands.MainsChanged(false);
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
                            vstPlugins[ind].vstPlugins.PluginCommandStub.Commands.EditorClose();
                            vstPlugins[ind].vstPlugins.PluginCommandStub.Commands.StopProcess();
                            vstPlugins[ind].vstPlugins.PluginCommandStub.Commands.MainsChanged(false);
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
            private Setting setting;

            public HostCommandStub()
            {
                Commands = new HostCommands(this);
            }

            public HostCommandStub(Setting setting)
            {
                this.setting = setting;
                Commands = new HostCommands(this);
            }

            /// <summary>
            /// Raised when one of the methods is called.
            /// </summary>
            public event EventHandler<PluginCalledEventArgs> PluginCalled;

            private void RaisePluginCalled(string message)
            {
                PluginCalled?.Invoke(this, new PluginCalledEventArgs(message));
            }

            /// <summary>
            /// Attached to the EditorFrame for a plugin.
            /// </summary>
            public event EventHandler<SizeWindowEventArgs> SizeWindow;

            public void RaiseSizeWindow(int width, int height)
            {
                SizeWindow?.Invoke(this, new SizeWindowEventArgs(width, height));
            }

            #region IVstHostCommandsStub Members

            /// <inheritdoc />
            public IVstPluginContext PluginContext { get; set; }

            public IVstHostCommands20 Commands { get; private set; }

            #endregion

            //#region IVstHostCommands20 Members

            ///// <inheritdoc />
            //public bool BeginEdit(int index)
            //{
            //    RaisePluginCalled("BeginEdit(" + index + ")");

            //    return false;
            //}

            ///// <inheritdoc />
            //public VstCanDoResult CanDo(string cando)
            //{
            //    RaisePluginCalled("CanDo(" + cando + ")");
            //    return VstCanDoResult.Unknown;
            //}

            ///// <inheritdoc />
            //public bool CloseFileSelector(VstFileSelect fileSelect)
            //{
            //    RaisePluginCalled("CloseFileSelector(" + fileSelect.Command + ")");
            //    return false;
            //}

            ///// <inheritdoc />
            //public bool EndEdit(int index)
            //{
            //    RaisePluginCalled("EndEdit(" + index + ")");
            //    return false;
            //}

            ///// <inheritdoc />
            //public VstAutomationStates GetAutomationState()
            //{
            //    RaisePluginCalled("GetAutomationState()");
            //    return VstAutomationStates.Off;
            //}

            ///// <inheritdoc />
            //public int GetBlockSize()
            //{
            //    RaisePluginCalled("GetBlockSize()");
            //    return 1024;
            //}

            ///// <inheritdoc />
            //public string GetDirectory()
            //{
            //    RaisePluginCalled("GetDirectory()");
            //    return null;
            //}

            ///// <inheritdoc />
            //public int GetInputLatency()
            //{
            //    RaisePluginCalled("GetInputLatency()");
            //    return 0;
            //}

            ///// <inheritdoc />
            //public VstHostLanguage GetLanguage()
            //{
            //    RaisePluginCalled("GetLanguage()");
            //    return VstHostLanguage.NotSupported;
            //}

            ///// <inheritdoc />
            //public int GetOutputLatency()
            //{
            //    RaisePluginCalled("GetOutputLatency()");
            //    return 0;
            //}

            ///// <inheritdoc />
            //public VstProcessLevels GetProcessLevel()
            //{
            //    RaisePluginCalled("GetProcessLevel()");
            //    return VstProcessLevels.Unknown;
            //}

            ///// <inheritdoc />
            //public string GetProductString()
            //{
            //    RaisePluginCalled("GetProductString()");
            //    return "VST.NET";
            //}

            ///// <inheritdoc />
            //public float GetSampleRate()
            //{
            //    RaisePluginCalled("GetSampleRate()");
            //    return setting.outputDevice.SampleRate / 1000.0f;
            //}

            ///// <inheritdoc />
            //public VstTimeInfo GetTimeInfo(VstTimeInfoFlags filterFlags)
            //{
            //    //RaisePluginCalled("GetTimeInfo(" + filterFlags + ")");
            //    VstTimeInfo vti = new VstTimeInfo();
            //    vti.SamplePosition = 0;
            //    vti.SampleRate = setting.outputDevice.SampleRate / 1000.0f;
            //    vti.NanoSeconds = 0;
            //    vti.PpqPosition = 0;
            //    vti.Tempo = 120;
            //    vti.BarStartPosition = 0;
            //    vti.CycleStartPosition = 0;
            //    vti.CycleEndPosition = 0;
            //    vti.TimeSignatureNumerator = 4;
            //    vti.TimeSignatureDenominator = 4;
            //    vti.SmpteOffset = 0;
            //    vti.SmpteFrameRate = VstSmpteFrameRate.Smpte24fps;
            //    vti.SamplesToNearestClock = 0;
            //    vti.Flags = VstTimeInfoFlags.NanoSecondsValid
            //        | VstTimeInfoFlags.PpqPositionValid
            //        | VstTimeInfoFlags.TempoValid
            //        | VstTimeInfoFlags.TimeSignatureValid;
            //    return vti;
            //}

            ///// <inheritdoc />
            //public string GetVendorString()
            //{
            //    RaisePluginCalled("GetVendorString()");
            //    return "";
            //}

            ///// <inheritdoc />
            //public int GetVendorVersion()
            //{
            //    RaisePluginCalled("GetVendorVersion()");
            //    return 1000;
            //}

            ///// <inheritdoc />
            //public bool IoChanged()
            //{
            //    RaisePluginCalled("IoChanged()");
            //    return false;
            //}

            ///// <inheritdoc />
            //public bool OpenFileSelector(VstFileSelect fileSelect)
            //{
            //    RaisePluginCalled("OpenFileSelector(" + fileSelect.Command + ")");
            //    return false;
            //}

            ///// <inheritdoc />
            //public bool ProcessEvents(VstEvent[] events)
            //{
            //    RaisePluginCalled("ProcessEvents(" + events.Length + ")");
            //    return false;
            //}

            ///// <inheritdoc />
            //public bool SizeWindow(int width, int height)
            //{
            //    RaisePluginCalled("SizeWindow(" + width + ", " + height + ")");
            //    return false;
            //}

            ///// <inheritdoc />
            //public bool UpdateDisplay()
            //{
            //    RaisePluginCalled("UpdateDisplay()");
            //    return false;
            //}

            //#endregion

            //#region IVstHostCommands10 Members

            ///// <inheritdoc />
            //public int GetCurrentPluginID()
            //{
            //    RaisePluginCalled("GetCurrentPluginID()");
            //    return PluginContext.PluginInfo.PluginID;
            //}

            ///// <inheritdoc />
            //public int GetVersion()
            //{
            //    RaisePluginCalled("GetVersion()");
            //    return 1000;
            //}

            ///// <inheritdoc />
            //public void ProcessIdle()
            //{
            //    RaisePluginCalled("ProcessIdle()");
            //}

            ///// <inheritdoc />
            //public void SetParameterAutomated(int index, float value)
            //{
            //    RaisePluginCalled("SetParameterAutomated(" + index + ", " + value + ")");
            //}

            //#endregion
            private sealed class HostCommands : IVstHostCommands20
            {
                private readonly HostCommandStub _cmdStub;

                public HostCommands(HostCommandStub cmdStub)
                {
                    _cmdStub = cmdStub;
                }

                #region IVstHostCommands20 Members

                /// <inheritdoc />
                public bool BeginEdit(int index)
                {
                    _cmdStub.RaisePluginCalled("BeginEdit(" + index + ")");
                    return false;
                }

                /// <inheritdoc />
                public Jacobi.Vst.Core.VstCanDoResult CanDo(string cando)
                {
                    _cmdStub.RaisePluginCalled("CanDo(" + cando + ")");
                    return Jacobi.Vst.Core.VstCanDoResult.Unknown;
                }

                /// <inheritdoc />
                public bool CloseFileSelector(Jacobi.Vst.Core.VstFileSelect fileSelect)
                {
                    _cmdStub.RaisePluginCalled("CloseFileSelector(" + fileSelect.Command + ")");
                    return false;
                }

                /// <inheritdoc />
                public bool EndEdit(int index)
                {
                    _cmdStub.RaisePluginCalled("EndEdit(" + index + ")");
                    return false;
                }

                /// <inheritdoc />
                public Jacobi.Vst.Core.VstAutomationStates GetAutomationState()
                {
                    _cmdStub.RaisePluginCalled("GetAutomationState()");
                    return Jacobi.Vst.Core.VstAutomationStates.Off;
                }

                /// <inheritdoc />
                public int GetBlockSize()
                {
                    _cmdStub.RaisePluginCalled("GetBlockSize()");
                    return 1024;
                }

                /// <inheritdoc />
                public string GetDirectory()
                {
                    _cmdStub.RaisePluginCalled("GetDirectory()");
                    return null;
                }

                /// <inheritdoc />
                public int GetInputLatency()
                {
                    _cmdStub.RaisePluginCalled("GetInputLatency()");
                    return 0;
                }

                /// <inheritdoc />
                public Jacobi.Vst.Core.VstHostLanguage GetLanguage()
                {
                    _cmdStub.RaisePluginCalled("GetLanguage()");
                    return Jacobi.Vst.Core.VstHostLanguage.NotSupported;
                }

                /// <inheritdoc />
                public int GetOutputLatency()
                {
                    _cmdStub.RaisePluginCalled("GetOutputLatency()");
                    return 0;
                }

                /// <inheritdoc />
                public Jacobi.Vst.Core.VstProcessLevels GetProcessLevel()
                {
                    _cmdStub.RaisePluginCalled("GetProcessLevel()");
                    return Jacobi.Vst.Core.VstProcessLevels.Unknown;
                }

                /// <inheritdoc />
                public string GetProductString()
                {
                    _cmdStub.RaisePluginCalled("GetProductString()");
                    return "VST.NET";
                }

                /// <inheritdoc />
                public float GetSampleRate()
                {
                    _cmdStub.RaisePluginCalled("GetSampleRate()");
                    return 44.8f;
                }

                /// <inheritdoc />
                public Jacobi.Vst.Core.VstTimeInfo GetTimeInfo(Jacobi.Vst.Core.VstTimeInfoFlags filterFlags)
                {
                    _cmdStub.RaisePluginCalled("GetTimeInfo(" + filterFlags + ")");
                    return null;
                }

                /// <inheritdoc />
                public string GetVendorString()
                {
                    _cmdStub.RaisePluginCalled("GetVendorString()");
                    return "Jacobi Software";
                }

                /// <inheritdoc />
                public int GetVendorVersion()
                {
                    _cmdStub.RaisePluginCalled("GetVendorVersion()");
                    return 1000;
                }

                /// <inheritdoc />
                public bool IoChanged()
                {
                    _cmdStub.RaisePluginCalled("IoChanged()");
                    return false;
                }

                /// <inheritdoc />
                public bool OpenFileSelector(Jacobi.Vst.Core.VstFileSelect fileSelect)
                {
                    _cmdStub.RaisePluginCalled("OpenFileSelector(" + fileSelect.Command + ")");
                    return false;
                }

                /// <inheritdoc />
                public bool ProcessEvents(Jacobi.Vst.Core.VstEvent[] events)
                {
                    _cmdStub.RaisePluginCalled("ProcessEvents(" + events.Length + ")");
                    return false;
                }

                /// <inheritdoc />
                public bool SizeWindow(int width, int height)
                {
                    _cmdStub.RaisePluginCalled("SizeWindow(" + width + ", " + height + ")");
                    _cmdStub.RaiseSizeWindow(width, height);
                    return false;
                }

                /// <inheritdoc />
                public bool UpdateDisplay()
                {
                    _cmdStub.RaisePluginCalled("UpdateDisplay()");
                    return false;
                }

                #endregion

                #region IVstHostCommands10 Members

                /// <inheritdoc />
                public int GetCurrentPluginID()
                {
                    _cmdStub.RaisePluginCalled("GetCurrentPluginID()");
                    // this is the plugin Id the host wants to load
                    // for shell plugins (a plugin that hosts other plugins)
                    return 0;
                }

                /// <inheritdoc />
                public int GetVersion()
                {
                    _cmdStub.RaisePluginCalled("GetVersion()");
                    return 1000;
                }

                /// <inheritdoc />
                public void ProcessIdle()
                {
                    _cmdStub.RaisePluginCalled("ProcessIdle()");
                }

                /// <inheritdoc />
                public void SetParameterAutomated(int index, float value)
                {
                    _cmdStub.RaisePluginCalled("SetParameterAutomated(" + index + ", " + value + ")");
                }

                #endregion
            }
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

        /// <summary>
        /// Event arguments used when the SizeWindow method is called.
        /// </summary>
        public sealed class SizeWindowEventArgs : EventArgs
        {
            /// <summary>
            /// Constructs a new instance with a <paramref name="message"/>.
            /// </summary>
            /// <param name="message"></param>
            public SizeWindowEventArgs(int width, int height)
            {
                Width = width;
                Height = height;
            }

            public int Width { get; }
            public int Height { get; }
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
