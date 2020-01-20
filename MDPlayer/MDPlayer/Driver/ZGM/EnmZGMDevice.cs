using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.ZGM
{
    public enum EnmZGMDevice : int
    {
        None = 0x0000_0000
        //VGM Chips(VGMで使用されるエミュレーションチップ定義)
        , SN76489 = 0x0000_000C
        , YM2413 = 0x0000_0010
        , YM2612 = 0x0000_002C
        , YM2151 = 0x0000_0030
        , SegaPCM = 0x0000_0038
        , RF5C68 = 0x0000_0040
        , YM2203 = 0x0000_0044
        , YM2608 = 0x0000_0048
        , YM2610 = 0x0000_004C
        , YM2610B = 0x0000_004C
        , YM3812 = 0x0000_0050
        , YM3526 = 0x0000_0054
        , Y8950 = 0x0000_0058
        , YMF262 = 0x0000_005C
        , YMF278B = 0x0000_0060
        , YMF271 = 0x0000_0064
        , YMZ280B = 0x0000_0068
        , RF5C164 = 0x0000_006C
        , PWM = 0x0000_0070
        , AY8910 = 0x0000_0074
        , GameBoyDMG = 0x0000_0080
        , NESAPU = 0x0000_0084
        , MultiPCM = 0x0000_0088
        , uPD7759 = 0x0000_008C
        , OKIM6258 = 0x0000_0090
        , OKIM6295 = 0x0000_0098
        , K051649 = 0x0000_009C
        , K054539 = 0x0000_00A0
        , HuC6280 = 0x0000_00A4
        , C140 = 0x0000_00A8
        , K053260 = 0x0000_00AC
        , Pokey = 0x0000_00B0
        , QSound = 0x0000_00B4
        , SCSP = 0x0000_00B8
        , WonderSwan = 0x0000_00C0
        , VirtualBoyVSU = 0x0000_00C4
        , SAA1099 = 0x0000_00C8
        , ES5503 = 0x0000_00CC
        , ES5505 = 0x0000_00D0
        , ES5506 = 0x0000_00D0
        , X1_010 = 0x0000_00D8
        , C352 = 0x0000_00DC
        , GA20 = 0x0000_00E0
        // Chips                
        , Conductor = 0x0001_0000
        // 妄想Chips            
        , OtherChips = 0x0002_0000
        , AY8910B = 0x0002_0000
        , YM2609 = 0x0002_0001
        // XG音源               
        , MIDIXG = 0x0003_0000
        , MU50 = 0x0003_0000
        // LA/GS音源            
        , MIDIGS = 0x0004_0000
        , MT32 = 0x0004_0000
        //GM                    
        , MIDIGM = 0x0005_0000
        //VSTi                  
        , MIDIVSTi = 0x0006_0000
        , VOPMex = 0x0006_0000
        //Wave                  
        , Wave = 0x0007_0000
        , RawWave = 0x0007_0000
    }
}
