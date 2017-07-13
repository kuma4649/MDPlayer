using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

namespace VST
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int audioMasterCallbackDelegate(IntPtr effect, int opcode, int index, int value, IntPtr ptr, float opt);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int AEffectDispatcherProcDelegate(ref AEffect effect, int opcode, int index, int value, IntPtr ptr, float opt);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AEffectProcessProcDelegate(ref AEffect effect, ref IntPtr inputs, ref IntPtr outputs, int sampleFrames);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AEffectProcessDoubleProcDelegate(ref AEffect effect, ref IntPtr inputs, ref IntPtr outputs, int sampleFrames);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AEffectSetParameterProcDelegate(ref AEffect effect, int index, float parameter);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate float AEffectGetParameterProcDelegate(ref AEffect effect, int index);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr PluginEntryProcDelegate(audioMasterCallbackDelegate audioMaster);

    public class VST : IDisposable
    {
        public AEffect effect;
        public string effectName
        {
            get
            {
                return _effectName;
            }
        }
        public string vendor
        {
            get
            {
                return _vendor;
            }
        }
        public string product
        {
            get
            {
                return _product;
            }
        }
        public VSTForm form = null;
        public bool IsOpenEditor = false;

        //[MarshalAs(UnmanagedType.LPArray, SizeConst = 2 * 512)]
        public float[] bufOutputs = null;
        //[MarshalAs(UnmanagedType.LPArray, SizeConst = 2 * 512)]
        public float[] bufInputs = null;

        private IntPtr outputsPtr = IntPtr.Zero;
        private IntPtr inputsPtr1 = IntPtr.Zero;
        private IntPtr inputsPtr2 = IntPtr.Zero;
        private IntPtr inputsPtr = IntPtr.Zero;
        IntPtr[] inputsPtrs = new IntPtr[2] { IntPtr.Zero, IntPtr.Zero };
        IntPtr[] outputsPtrs = new IntPtr[2] { IntPtr.Zero, IntPtr.Zero };
        private string _effectName = "";
        private string _vendor = "";
        private string _product = "";
        private IntPtr ipLib = IntPtr.Zero;
        private bool blnDisposed = false;
        private bool wasIdle = false;

        private PluginEntryProcDelegate mainEntry;
        private audioMasterCallbackDelegate hostCallBack;

        public VST()
        {
        }

        ~VST()
        {
            this.Dispose();
        }

        public int hostcallback(IntPtr effect, int opcode, int index, int value, IntPtr ptr, float opt)
        {
            int result = 0;

            // Filter idle calls...
            bool filtered = false;
            if (opcode == 3)//audioMasterIdle)
            {
                if (wasIdle)
                    filtered = true;
                else
                {
                    Console.WriteLine("(Future idle calls will not be displayed!)");
                    wasIdle = true;
                }
            }

            //if (!filtered)
                //Console.WriteLine(string.Format("PLUG> HostCallback (opcode {0})\n index = {1}, value = {2}, ptr = {3}, opt = {4}\n", opcode, index, value, ptr, opt));

            switch (opcode)
            {
                case 1://audioMasterVersion :
                    result = 2;// kVstVersion;
                    break;
            }
            return result;
        }

        public bool LoadVST(string filename)
        {
            string mainProcStr = "VSTPluginMain";
            ipLib = LoadLibrary(filename);
            var mainProc = GetProcAddress(ipLib, mainProcStr);
            if (mainProc == IntPtr.Zero)
            {
                mainProcStr = "main";
                mainProc = GetProcAddress(ipLib, mainProcStr);
            }
            if (mainProc == IntPtr.Zero) return false;

            mainEntry = Generate(mainProcStr, typeof(PluginEntryProcDelegate)) as PluginEntryProcDelegate;

            return true;
        }

        public bool InitVST()
        {
            hostCallBack = new audioMasterCallbackDelegate(hostcallback);
            IntPtr eff = mainEntry(hostCallBack);
            effect = (AEffect)Marshal.PtrToStructure(eff, typeof(AEffect));

            effect.dispatcher(ref effect, (int)AEffectOpcodes.effOpen, 0, 0, IntPtr.Zero, 0);
            effect.dispatcher(ref effect, (int)AEffectOpcodes.effSetSampleRate, 0, 0, IntPtr.Zero, 44100.0F);//kSampleRate);
            effect.dispatcher(ref effect, (int)AEffectOpcodes.effSetBlockSize, 0, 512, IntPtr.Zero, 0);//kBlockSize

            //checkEffectProperties(effect);
            IntPtr prop = Marshal.AllocHGlobal(256);
            effect.dispatcher(ref effect, (int)AEffectXOpcodes.effGetEffectName, 0, 0, prop, 0);
            _effectName = Marshal.PtrToStringAnsi(prop);
            effect.dispatcher(ref effect, (int)AEffectXOpcodes.effGetVendorString, 0, 0, prop, 0);
            _vendor = Marshal.PtrToStringAnsi(prop);
            effect.dispatcher(ref effect, (int)AEffectXOpcodes.effGetProductString, 0, 0, prop, 0);
            _product = Marshal.PtrToStringAnsi(prop);
            Marshal.FreeHGlobal(prop);

            // alloc buffer memory
            if (effect.numInputs != 2) return false;
            if (effect.numOutputs != 2) return false;

            bufInputs = new float[2*512];
            inputsPtr = Marshal.AllocCoTaskMem(sizeof(float) * 3 * 512);
            //            bufInputs = new float[2][] { new float[512], new float[512] };
            //            inputsPtr = Marshal.AllocCoTaskMem(sizeof(float) * 2 * 512 + sizeof(int) * 30);
            //            inputsPtrs[0] = Marshal.AllocCoTaskMem(sizeof(int));
            //            inputsPtrs[1] = Marshal.AllocCoTaskMem(sizeof(int));
            bufOutputs = new float[2*512];
            outputsPtr = Marshal.AllocCoTaskMem(sizeof(float) * 3 * 512);
            //            bufOutputs = new float[2][] { new float[512], new float[512] };
            //            outputsPtr = Marshal.AllocCoTaskMem(sizeof(float) * 2 * 512 + sizeof(int) * 30);
            //            outputsPtrs[0] = Marshal.AllocCoTaskMem(sizeof(int));
            //            outputsPtrs[1] = Marshal.AllocCoTaskMem(sizeof(int));

            return true;
        }

        public bool OpenEditor()
        {
            if ((effect.flags & 1) == 0)
            {
                Console.WriteLine("This plug does not have an editor!\n");
                return false;
            }

            form = new VSTForm(effect, this);
            form.Text = this.effectName;
            form.Show();

            IsOpenEditor = true;

            return true;
        }

        public bool CloseEditor()
        {
            if ((effect.flags & 1) == 0)
            {
                Console.WriteLine("This plug does not have an editor!\n");
                return false;
            }

            if (form == null) return false;
            form.Close();

            IsOpenEditor = false;

            return true;
        }

        public void effectResume()
        {
            Console.WriteLine("HOST> Resume effect...\n");
            effect.dispatcher(ref effect, (int)AEffectOpcodes.effMainsChanged, 0, 1, IntPtr.Zero, 0);
        }

        public void effectSuspend()
        {
            Console.WriteLine("HOST> Suspend effect...\n");
            effect.dispatcher(ref effect, (int)AEffectOpcodes.effMainsChanged, 0, 0, IntPtr.Zero, 0);
        }

        unsafe public void sendMidi()
        {
            VstMidiEvent midievent = new VstMidiEvent();
            midievent.type = (int)VstEventTypes.kVstMidiType;
            midievent.byteSize = sizeof(VstMidiEvent);
            midievent.deltaFrames = 0;
            midievent.midiData[0] = 0x90; //note on
            midievent.midiData[1] = 60; // note (C)
            midievent.midiData[2] = 127;//Velocity

            //iasiodll.vstProcessEventsMIDI(ref effect, ref midievent);
        }

        unsafe public void processReplacing(int sampleFrames)
        {
            //Console.WriteLine("HOST> Process Replacing...\n");

            if (effect.numInputs > 0)
            {
                Marshal.Copy(bufInputs, 0, inputsPtr, 1024);// sizeof(float) * 2 * 512);
                //Marshal.Copy(bufInputs[0], 0, inputsPtrs[0], sizeof(float)* 512);
                //Marshal.Copy(bufInputs[1], 0, inputsPtrs[1], sizeof(float) * 512);
                //Marshal.Copy(inputsPtrs, 0, inputsPtr, sizeof(int)*2);
            }
            if (effect.numOutputs > 0)
            {
                Marshal.Copy(bufOutputs, 0, outputsPtr, 1024);// sizeof(float) * 2 * 512);
                //Marshal.Copy(bufOutputs[0], 0, outputsPtrs[0], sizeof(float) * 512);
                //Marshal.Copy(bufOutputs[1], 0, outputsPtrs[1], sizeof(float) * 512);
                //Marshal.Copy(outputsPtrs, 0, outputsPtr, sizeof(int) * 2);
            }

            effect.processReplacing(ref effect, ref inputsPtr, ref outputsPtr, 0);// sampleFrames);
            //iasiodll.vstProcessReplacing(ref effect, inputsPtr, effect.numInputs, outputsPtr, effect.numOutputs, 512);

            if (effect.numInputs > 0)
            {
                Marshal.Copy(inputsPtr, bufInputs, 0, 2*512);
                //Marshal.Copy(inputsPtrs[0], bufInputs[0], 0, 512);
                //Marshal.Copy(inputsPtrs[1], bufInputs[1], 0, 512);
            }
            if (effect.numOutputs > 0)
            {
                Marshal.Copy(outputsPtr, bufOutputs, 0, 2*512);
                //Marshal.Copy(outputsPtrs[0], bufOutputs[0], 0, 512);
                //Marshal.Copy(outputsPtrs[1], bufOutputs[1], 0, 512);
            }
        }

        public void sendMIDIMessage()
        {
            effect.dispatcher(ref effect, (int)AEffectXOpcodes.effProcessEvents, 0, 0, IntPtr.Zero, 0);
        }

        public bool CloseVST()
        {
            effect.dispatcher(ref effect, (int)AEffectOpcodes.effClose, 0, 0, IntPtr.Zero, 0);
            if (form != null)
            {
                form.Close();
            }

            //delete buffer memory
            if (effect.numInputs > 0)
            {
                Marshal.FreeCoTaskMem(inputsPtr);
                Marshal.FreeCoTaskMem(inputsPtrs[0]);
                Marshal.FreeCoTaskMem(inputsPtrs[1]);
            }
            if (effect.numOutputs > 0)
            {
                Marshal.FreeCoTaskMem(outputsPtr);
                Marshal.FreeCoTaskMem(outputsPtrs[0]);
                Marshal.FreeCoTaskMem(outputsPtrs[1]);
            }

            return true;
        }

        public void Dispose()
        {
            if (!blnDisposed)
            {
                FreeLibrary(ipLib);
                blnDisposed = true;
            }
        }

        private void ThrowExceptionIfDisposed()
        {
            if (this.blnDisposed)
                throw new ObjectDisposedException(this.GetType().ToString());
        }

        /// <summary> 
        /// 指定されたデリゲートスタイルで動的にAPI関数実行メソッドを生成する 
        /// 元ねた：http://d.hatena.ne.jp/zecl/20090223/p1
        /// </summary> 
        /// <param name="methodType">デリゲートのタイプを指定</param> 
        /// <param name="methodName">DLL内に定義されている関数名</param> 
        /// <returns>関数実行メソッドに関連付けられたデリゲート</returns> 
        public MulticastDelegate Generate(string methodName, Type methodType)
        {
            this.ThrowExceptionIfDisposed();

            if (methodType.BaseType != typeof(MulticastDelegate))
                throw new ArgumentException("TypeはSystm.MulticastDelegate型である必要があります。");

            AssemblyName asmName = new AssemblyName("PlatformInvokeMethodGenerator");
            AssemblyBuilder asmb = Thread.GetDomain().DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            ModuleBuilder modb = asmb.DefineDynamicModule("PInvokeModule");
            TypeBuilder typb = modb.DefineType("PInvokeType", TypeAttributes.Public);

            MethodInfo mi = methodType.GetMethod("Invoke");
            var pi = mi.GetParameters();

            var pList = new List<Type>();
            foreach (var item in pi)
                pList.Add(item.ParameterType);
            var pTypes = pList.ToArray();

            MethodBuilder metb = typb.DefineMethod("Generated" + methodName,
                                                    MethodAttributes.Public,
                                                    mi.ReturnType, pTypes);

            ILGenerator ilg = metb.GetILGenerator();
            // 関数に送られるすべての引数を積む 
            for (int i = 1; i <= pTypes.Length; i++)
                ilg.Emit(OpCodes.Ldarg, i);

            // 関数ポインタを積む 
            ilg.Emit(OpCodes.Ldc_I4, (int)GetProcAddress(ipLib, methodName));
            ilg.EmitCalli(OpCodes.Calli, CallingConvention.Cdecl, mi.ReturnType, pTypes);
            ilg.Emit(OpCodes.Ret);

            var o = Activator.CreateInstance(typb.CreateType());
            return (MulticastDelegate)Delegate.CreateDelegate(methodType, o, "Generated" + methodName);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll")]
        private extern static bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private extern static IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct VstEvent
    {
        public int type;			///< @see VstEventTypes
	    public int byteSize;		///< size of this event, excl. type and byteSize
	    public int deltaFrames;	///< sample frames related to the current block start sample position
	    public int flags;			///< generic flags, none defined yet
	    public fixed byte data[16];			///< data size may vary, depending on event type
    };

    public enum VstEventTypes : int
    {
        kVstMidiType = 1,		///< MIDI event  @see VstMidiEvent
	    __kVstAudioTypeDeprecated,		///< \deprecated unused event type
	    __kVstVideoTypeDeprecated,		///< \deprecated unused event type
	    __kVstParameterTypeDeprecated,	///< \deprecated unused event type
        __kVstTriggerTypeDeprecated,	///< \deprecated unused event type
	    kVstSysExType			///< MIDI system exclusive  @see VstMidiSysexEvent
    };

    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct VstEvents
    {
        public int numEvents;		///< number of Events in array
	    public IntPtr reserved;		///< zero (Reserved for future use)
        public VstMidiEvent events1;	///< event pointer array, variable size
        public VstEvent events2;	///< event pointer array, variable size
    };

    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct VstMidiEvent
    {
        public int type;			///< #kVstMidiType
	    public int byteSize;		///< sizeof (VstMidiEvent)
	    public int deltaFrames;	///< sample frames related to the current block start sample position
	    public int flags;			///< @see VstMidiEventFlags
	    public int noteLength;	///< (in sample frames) of entire note, if available, else 0
	    public int noteOffset;	///< offset (in sample frames) into note from note start if available, else 0
    	public fixed byte midiData[4];		///< 1 to 3 MIDI bytes; midiData[3] is reserved (zero)
	    public byte detune;			///< -64 to +63 cents; for scales other than 'well-tempered' ('microtuning')
	    public byte noteOffVelocity;	///< Note Off Velocity [0, 127]
	    public byte reserved1;			///< zero (Reserved for future use)
	    public byte reserved2;			///< zero (Reserved for future use)
    };

    public enum VstMidiEventFlags : int
    {
        kVstMidiEventIsRealtime = 1 << 0	///< means that this event is played life (not in playback from a sequencer track).\n This allows the Plug-In to handle these flagged events with higher priority, especially when the Plug-In has a big latency (AEffect::initialDelay)
    };

    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct VstMidiSysexEvent
    {
        public int type;			///< #kVstSysexType
        public int byteSize;		///< sizeof (VstMidiSysexEvent)
        public int deltaFrames;	///< sample frames related to the current block start sample position
        public int flags;			///< none defined yet (should be zero)
        public int dumpBytes;		///< byte size of sysexDump
        public IntPtr resvd1;		///< zero (Reserved for future use)
        public byte* sysexDump;		///< sysex dump
        public IntPtr resvd2;		///< zero (Reserved for future use)
    };

    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct AEffect
    {
        public int magic;
        public AEffectDispatcherProcDelegate dispatcher;

        /** \deprecated Accumulating process mode is deprecated in VST 2.4! Use AEffect::processReplacing instead! */
        public AEffectProcessProcDelegate __precessDeprecated;

        /** Set new value of automatable parameter @see AudioEffect::setParameter */
        public AEffectSetParameterProcDelegate setParameter;

        /** Returns current value of automatable parameter @see AudioEffect::getParameter*/
        public AEffectGetParameterProcDelegate getParameter;

        public int numPrograms;   ///< number of programs
        public int numParams;		///< all programs are assumed to have numParams parameters
        public int numInputs;		///< number of audio inputs
        public int numOutputs;	///< number of audio outputs

        public int flags;			///< @see VstAEffectFlags

        public int resvd1;		///< reserved for Host, must be 0
        public int resvd2;		///< reserved for Host, must be 0

        public int initialDelay;	///< for algorithms which need input in the first place (Group delay or latency in Samples). This value should be initialized in a resume state.

        public int __realQualitiesDeprecated;	///< \deprecated unused member
        public int __offQualitiesDeprecated;		///< \deprecated unused member
        public float __ioRatioDeprecated;			///< \deprecated unused member

        public IntPtr Object;			///< #AudioEffect class pointer
        public IntPtr user;				///< user-defined pointer

        public int uniqueID;		///< registered unique identifier (register it at Steinberg 3rd party support Web). This is used to identify a plug-in during save+load of preset and project.
        public int version;		///< plug-in version (example 1100 for version 1.1.0.0)

        /** Process audio samples in replacing mode @see AudioEffect::processReplacing */
        public AEffectProcessProcDelegate processReplacing;

        /** Process double-precision audio samples in replacing mode @see AudioEffect::processDoubleReplacing */
        public AEffectProcessDoubleProcDelegate processDoubleReplacing;

        public fixed byte future[56];		///< reserved for future use (please zero)
    };

    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct ERect
    {
        public short top;
        public short left;
        public short bottom;
        public short right;
    };

    public enum AEffectOpcodes : int
    {
        effOpen = 0,		///< no arguments  @see AudioEffect::open
        effClose,			///< no arguments  @see AudioEffect::close

        effSetProgram,		///< [value]: new program number  @see AudioEffect::setProgram
        effGetProgram,		///< [return value]: current program number  @see AudioEffect::getProgram
        effSetProgramName,	///< [ptr]: char* with new program name, limited to #kVstMaxProgNameLen  @see AudioEffect::setProgramName
        effGetProgramName,	///< [ptr]: char buffer for current program name, limited to #kVstMaxProgNameLen  @see AudioEffect::getProgramName

        effGetParamLabel,	///< [ptr]: char buffer for parameter label, limited to #kVstMaxParamStrLen  @see AudioEffect::getParameterLabel
        effGetParamDisplay,	///< [ptr]: char buffer for parameter display, limited to #kVstMaxParamStrLen  @see AudioEffect::getParameterDisplay
        effGetParamName,	///< [ptr]: char buffer for parameter name, limited to #kVstMaxParamStrLen  @see AudioEffect::getParameterName

        __effGetVuDeprecated,	///< \deprecated deprecated in VST 2.4

        effSetSampleRate,	///< [opt]: new sample rate for audio processing  @see AudioEffect::setSampleRate
        effSetBlockSize,	///< [value]: new maximum block size for audio processing  @see AudioEffect::setBlockSize
        effMainsChanged,	///< [value]: 0 means "turn off", 1 means "turn on"  @see AudioEffect::suspend @see AudioEffect::resume

        effEditGetRect,		///< [ptr]: #ERect** receiving pointer to editor size  @see ERect @see AEffEditor::getRect
        effEditOpen,		///< [ptr]: system dependent Window pointer, e.g. HWND on Windows  @see AEffEditor::open
        effEditClose,		///< no arguments @see AEffEditor::close

        __effEditDrawDeprecated,	///< \deprecated deprecated in VST 2.4
        __effEditMouseDeprecated,	///< \deprecated deprecated in VST 2.4
        __effEditKeyDeprecated,	///< \deprecated deprecated in VST 2.4

        effEditIdle,		///< no arguments @see AEffEditor::idle

        __effEditTopDeprecated,	///< \deprecated deprecated in VST 2.4
        __effEditSleepDeprecated,	///< \deprecated deprecated in VST 2.4
        __effIdentifyDeprecated,	///< \deprecated deprecated in VST 2.4

        effGetChunk,		///< [ptr]: void** for chunk data address [index]: 0 for bank, 1 for program  @see AudioEffect::getChunk
        effSetChunk,		///< [ptr]: chunk data [value]: byte size [index]: 0 for bank, 1 for program  @see AudioEffect::setChunk

        effNumOpcodes
    };

    public enum AEffectXOpcodes : int
    {
        effProcessEvents = AEffectOpcodes.effSetChunk + 1		///< [ptr]: #VstEvents*  @see AudioEffectX::processEvents

        , effCanBeAutomated						///< [index]: parameter index [return value]: 1=true, 0=false  @see AudioEffectX::canParameterBeAutomated
            , effString2Parameter					///< [index]: parameter index [ptr]: parameter string [return value]: true for success  @see AudioEffectX::string2parameter

            , __effGetNumProgramCategoriesDeprecated	///< \deprecated deprecated in VST 2.4

            , effGetProgramNameIndexed				///< [index]: program index [ptr]: buffer for program name, limited to #kVstMaxProgNameLen [return value]: true for success  @see AudioEffectX::getProgramNameIndexed

            , __effCopyProgramDeprecated	///< \deprecated deprecated in VST 2.4
            , __effConnectInputDeprecated	///< \deprecated deprecated in VST 2.4
            , __effConnectOutputDeprecated	///< \deprecated deprecated in VST 2.4

            , effGetInputProperties					///< [index]: input index [ptr]: #VstPinProperties* [return value]: 1 if supported  @see AudioEffectX::getInputProperties
            , effGetOutputProperties				///< [index]: output index [ptr]: #VstPinProperties* [return value]: 1 if supported  @see AudioEffectX::getOutputProperties
            , effGetPlugCategory					///< [return value]: category  @see VstPlugCategory @see AudioEffectX::getPlugCategory

            , __effGetCurrentPositionDeprecated	///< \deprecated deprecated in VST 2.4
            , __effGetDestinationBuffer	///< \deprecated deprecated in VST 2.4

            , effOfflineNotify						///< [ptr]: #VstAudioFile array [value]: count [index]: start flag  @see AudioEffectX::offlineNotify
            , effOfflinePrepare						///< [ptr]: #VstOfflineTask array [value]: count  @see AudioEffectX::offlinePrepare
            , effOfflineRun							///< [ptr]: #VstOfflineTask array [value]: count  @see AudioEffectX::offlineRun

            , effProcessVarIo						///< [ptr]: #VstVariableIo*  @see AudioEffectX::processVariableIo
            , effSetSpeakerArrangement				///< [value]: input #VstSpeakerArrangement* [ptr]: output #VstSpeakerArrangement*  @see AudioEffectX::setSpeakerArrangement

            , __effSetBlockSizeAndSampleRateDeprecated	///< \deprecated deprecated in VST 2.4

            , effSetBypass							///< [value]: 1 = bypass, 0 = no bypass  @see AudioEffectX::setBypass
            , effGetEffectName						///< [ptr]: buffer for effect name, limited to #kVstMaxEffectNameLen  @see AudioEffectX::getEffectName

            , __effGetErrorTextDeprecated	///< \deprecated deprecated in VST 2.4

            , effGetVendorString					///< [ptr]: buffer for effect vendor string, limited to #kVstMaxVendorStrLen  @see AudioEffectX::getVendorString
            , effGetProductString					///< [ptr]: buffer for effect vendor string, limited to #kVstMaxProductStrLen  @see AudioEffectX::getProductString
            , effGetVendorVersion					///< [return value]: vendor-specific version  @see AudioEffectX::getVendorVersion
            , effVendorSpecific						///< no definition, vendor specific handling  @see AudioEffectX::vendorSpecific
            , effCanDo								///< [ptr]: "can do" string [return value]: 0: "don't know" -1: "no" 1: "yes"  @see AudioEffectX::canDo
            , effGetTailSize						///< [return value]: tail size (for example the reverb time of a reverb plug-in); 0 is default (return 1 for 'no tail')

            , __effIdleDeprecated				///< \deprecated deprecated in VST 2.4
            , __effGetIconDeprecated			///< \deprecated deprecated in VST 2.4
            , __effSetViewPositionDeprecated	///< \deprecated deprecated in VST 2.4

            , effGetParameterProperties				///< [index]: parameter index [ptr]: #VstParameterProperties* [return value]: 1 if supported  @see AudioEffectX::getParameterProperties

            , __effKeysRequiredDeprecated	///< \deprecated deprecated in VST 2.4

            , effGetVstVersion						///< [return value]: VST version  @see AudioEffectX::getVstVersion

            , effEditKeyDown						///< [index]: ASCII character [value]: virtual key [opt]: modifiers [return value]: 1 if key used  @see AEffEditor::onKeyDown
            , effEditKeyUp							///< [index]: ASCII character [value]: virtual key [opt]: modifiers [return value]: 1 if key used  @see AEffEditor::onKeyUp
            , effSetEditKnobMode					///< [value]: knob mode 0: circular, 1: circular relativ, 2: linear (CKnobMode in VSTGUI)  @see AEffEditor::setKnobMode

            , effGetMidiProgramName					///< [index]: MIDI channel [ptr]: #MidiProgramName* [return value]: number of used programs, 0 if unsupported  @see AudioEffectX::getMidiProgramName
            , effGetCurrentMidiProgram				///< [index]: MIDI channel [ptr]: #MidiProgramName* [return value]: index of current program  @see AudioEffectX::getCurrentMidiProgram
            , effGetMidiProgramCategory				///< [index]: MIDI channel [ptr]: #MidiProgramCategory* [return value]: number of used categories, 0 if unsupported  @see AudioEffectX::getMidiProgramCategory
            , effHasMidiProgramsChanged				///< [index]: MIDI channel [return value]: 1 if the #MidiProgramName(s) or #MidiKeyName(s) have changed  @see AudioEffectX::hasMidiProgramsChanged
            , effGetMidiKeyName						///< [index]: MIDI channel [ptr]: #MidiKeyName* [return value]: true if supported, false otherwise  @see AudioEffectX::getMidiKeyName

            , effBeginSetProgram					///< no arguments  @see AudioEffectX::beginSetProgram
            , effEndSetProgram						///< no arguments  @see AudioEffectX::endSetProgram
            , effGetSpeakerArrangement				///< [value]: input #VstSpeakerArrangement* [ptr]: output #VstSpeakerArrangement*  @see AudioEffectX::getSpeakerArrangement
            , effShellGetNextPlugin					///< [ptr]: buffer for plug-in name, limited to #kVstMaxProductStrLen [return value]: next plugin's uniqueID  @see AudioEffectX::getNextShellPlugin

            , effStartProcess						///< no arguments  @see AudioEffectX::startProcess
            , effStopProcess						///< no arguments  @see AudioEffectX::stopProcess
            , effSetTotalSampleToProcess		    ///< [value]: number of samples to process, offline only!  @see AudioEffectX::setTotalSampleToProcess
            , effSetPanLaw							///< [value]: pan law [opt]: gain  @see VstPanLawType @see AudioEffectX::setPanLaw

            , effBeginLoadBank						///< [ptr]: #VstPatchChunkInfo* [return value]: -1: bank can't be loaded, 1: bank can be loaded, 0: unsupported  @see AudioEffectX::beginLoadBank
            , effBeginLoadProgram					///< [ptr]: #VstPatchChunkInfo* [return value]: -1: prog can't be loaded, 1: prog can be loaded, 0: unsupported  @see AudioEffectX::beginLoadProgram
            , effSetProcessPrecision				///< [value]: @see VstProcessPrecision  @see AudioEffectX::setProcessPrecision
            , effGetNumMidiInputChannels			///< [return value]: number of used MIDI input channels (1-15)  @see AudioEffectX::getNumMidiInputChannels
            , effGetNumMidiOutputChannels			///< [return value]: number of used MIDI output channels (1-15)  @see AudioEffectX::getNumMidiOutputChannels
    };
}
