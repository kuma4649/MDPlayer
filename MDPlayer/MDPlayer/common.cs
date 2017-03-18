using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer
{
    public class common
    {
    }

    public enum enmModel
    {
        VirtualModel
        , RealModel
    }

    public enum enmUseChip : int
    {
        Unuse = 0
        , SN76489 = 1
        , YM2612 = 2
        , YM2612Ch6 = 4
        , RF5C164 = 8
        , PWM = 16
        , C140 = 32
        , OKIM6258 = 64
        , OKIM6295 = 128
        , SEGAPCM = 256
        , YM2151 = 512
        , YM2608 = 1024
        , YM2203 = 2048
        , YM2610 = 4096
        , AY8910 = 9192
            , HuC6280 = 18384
    }

    public enum enmScciChipType : int
    {
        YM2608 = 1
        , YM2151 = 2
        , YM2610 = 3
        , YM2203 = 4
        , YM2612 = 5
        , SN76489 = 7
    }

    public enum enmInstFormat : int
    {
        FMP7 = 0,
        MDX = 1,
        TFI = 2,
        MUSICLALF = 3,
        MUSICLALF2 = 4,
        MML2VGM = 5,
        NRTDRV = 6
    }

    public enum enmFileFormat : int
    {
        unknown = 0,
        VGM = 1,
        NRTDRV = 2
    }


}
