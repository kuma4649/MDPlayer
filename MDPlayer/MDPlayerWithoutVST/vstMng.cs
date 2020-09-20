using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using MDPlayer.form;

namespace MDPlayer
{
    public class vstMng
    {
        public static string[] chkFn = new string[]{
        };

        public Setting setting = null;
        public MIDIParam[] midiParams = null;//ChipRegisterからインスタンスをもらう

        private List<vstInfo2> vstPlugins = new List<vstInfo2>();
        private List<vstInfo2> vstPluginsInst = new List<vstInfo2>();
        public List<vstInfo2> vstMidiOuts = new List<vstInfo2>();
        public List<int> vstMidiOutsType = new List<int>();




        public void vstparse()
        {
        }

        public void SetUpVstInstrument(KeyValuePair<string,int> kv)
        {
        }

        public void SetUpVstEffect()
        {
        }

        public void SetupVstMidiOut(midiOutInfo mi)
        {
        }

        public void ReleaseAllMIDIout()
        {
        }

        private void ReleaseAllPlugins()
        {
        }


        public void Close()
        {
        }

        public void VST_Update(short[] buffer, int offset, int sampleCount)
        {
        }



        public void sendMIDIout(EnmModel model, int num, byte cmd, byte prm1, byte prm2, int deltaFrames = 0)
        {
        }

        public void sendMIDIout(EnmModel model, int num, byte cmd, byte prm1, int deltaFrames = 0)
        {
        }

        public void sendMIDIout(EnmModel model, int num, byte[] data, int deltaFrames = 0)
        {
        }

        public void resetAllMIDIout(EnmModel model)
        {
        }



        public object OpenPlugin(string pluginPath)
        {
            return null;
        }

        private static void HostCmdStub_PluginCalled(object sender, PluginCalledEventArgs e)
        {
        }

        public List<vstInfo2> getVSTInfos()
        {
            return vstPlugins;
        }

        public vstInfo getVSTInfo(string filename)
        {
            return null;
        }

        public bool addVSTeffect(string fileName)
        {
            return true;
        }

        public bool delVSTeffect(string key)
        {
            return true;
        }




        /// <summary>
        /// The HostCommandStub class represents the part of the host that a plugin can call.
        /// </summary>
        public class HostCommandStub
        {
            /// <summary>
            /// Raised when one of the methods is called.
            /// </summary>
            public event EventHandler<PluginCalledEventArgs> PluginCalled;

            private void RaisePluginCalled(string message)
            {
            }

            #region IVstHostCommandsStub Members

            /// <inheritdoc />
            public object PluginContext { get; set; }

            #endregion

            #region IVstHostCommands20 Members

            /// <inheritdoc />
            public bool BeginEdit(int index)
            {
                return false;
            }

            /// <inheritdoc />
            public object CanDo(string cando)
            {
                return null;
            }

            /// <inheritdoc />
            public bool CloseFileSelector(object fileSelect)
            {
                return false;
            }

            /// <inheritdoc />
            public bool EndEdit(int index)
            {
                RaisePluginCalled("EndEdit(" + index + ")");
                return false;
            }

            /// <inheritdoc />
            public object GetAutomationState()
            {
                return null;
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
            public object GetLanguage()
            {
                return null;
            }

            /// <inheritdoc />
            public int GetOutputLatency()
            {
                RaisePluginCalled("GetOutputLatency()");
                return 0;
            }

            /// <inheritdoc />
            public object GetProcessLevel()
            {
                return null;
            }

            /// <inheritdoc />
            public string GetProductString()
            {
                return null;
            }

            /// <inheritdoc />
            public float GetSampleRate()
            {
                return Common.SampleRate / 1000.0f;
            }

            /// <inheritdoc />
            public object GetTimeInfo(object filterFlags)
            {
                return null;
            }

            /// <inheritdoc />
            public string GetVendorString()
            {
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
            public bool OpenFileSelector(object fileSelect)
            {
                return false;
            }

            /// <inheritdoc />
            public bool ProcessEvents(object events)
            {
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
                return 0;
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
            public dummyClass vstPlugins = null;
            public form.frmVST vstPluginsForm = null;

            //実際にVSTiかどうかは問わない
            public bool isInstrument = false;
            public List<object> lstEvent = new List<object>();

            public void AddMidiEvent(object evt)
            {
            }
        }

        public class dummyClass
        {
            public dummyClass PluginCommandStub { get; internal set; }

            public void SetBypass(bool v)
            {
            }
        }

    }
}
