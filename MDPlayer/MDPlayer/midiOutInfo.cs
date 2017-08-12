using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    [Serializable]
    public class midiOutInfo
    {
        public int id = 0;
        public int manufacturer = -1;
        public string name = "";
        public int type = 0;//0:GM 1:XG 2:GS

    }
}
