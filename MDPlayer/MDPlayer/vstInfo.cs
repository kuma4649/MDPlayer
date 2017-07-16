using Jacobi.Vst.Interop.Host;
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
        public string name = "";

    }

    public class vstInfo2 : vstInfo
    {
        public VstPluginContext vstPlugins = null;
        public frmVST vstPluginsForm = null;
    }
}
