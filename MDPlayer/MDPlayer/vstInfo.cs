using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    [Serializable]
    public class vstInfo
    {
        public string key = "";
        public string fileName = "";
        public bool power = false;
        public bool editor = false;
        public string effectName = "";
        public string productName = "";
        public string vendorName = "";
        public string programName = "";
        public System.Drawing.Point location = System.Drawing.Point.Empty;
        public float[] param = null;
        public int midiInputChannels = 0;
        public int midiOutputChannels = 0;

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
