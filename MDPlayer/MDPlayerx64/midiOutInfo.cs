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
        public int type = 0;//GM / XG / GS / LA / GS(SC - 55_1) / GS(SC - 55_2)

        public int beforeSendType = 0;//None / GM Reset / XG Reset / GS Reset / Custom
        public bool isVST = false;
        public string fileName = "";
        public string vendor = "";

    }
}
