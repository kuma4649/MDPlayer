using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MGSDRV
{
    internal interface iMapper
    {
        int GetSegmentNumberFromPageNumber(int pageNumber);
        void SetSegmentToPage(int segmentNumber, int pageNumber);
        bool Use(int segmentNumber);
        void ClearUseFlag();
    }

}
