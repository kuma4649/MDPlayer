using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class ChipInf
    {
        /// <summary>
        /// ID Primary:0 / Secondary:1
        /// </summary>
        public int ID = 0;

        /// <summary>
        /// ChipType2
        /// </summary>
        public EnmChip type = EnmChip.Unuse;

        /// <summary>
        /// model Virtual / Real
        /// </summary>
        public EnmModel model = EnmModel.VirtualModel;

        /// <summary>
        /// Real model type
        /// </summary>
        public EnmRealModel mType = EnmRealModel.unknown;
    }

}
