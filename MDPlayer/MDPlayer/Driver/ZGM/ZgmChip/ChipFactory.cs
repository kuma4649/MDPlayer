using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.ZGM.ZgmChip
{
    public class ChipFactory
    {
        public ZgmChip Create(uint chipIdentNo, ChipRegister chipRegister, Setting setting, byte[] vgmBuf)
        {
            switch (chipIdentNo)
            {
                case 0x0000_000C: return null;// new SN76489(chipRegister, setting, vgmBuf);
                case 0x0000_0010: return null;// new YM2413(chipRegister, setting, vgmBuf);
                case 0x0000_002c: return null;// new YM2612(chipRegister, setting, vgmBuf);
                case 0x0000_0030: return null;// new YM2151(chipRegister, setting, vgmBuf);
                case 0x0000_0038: return null;// new SEGAPCM(chipRegister, setting, vgmBuf);
                case 0x0000_0040: return null;// RF5C68           
                case 0x0000_0044: return null;// new YM2203(chipRegister, setting, vgmBuf);
                case 0x0000_0048: return null;// new YM2608(chipRegister, setting, vgmBuf);
                case 0x0000_004C: return null;// new YM2610(chipRegister, setting, vgmBuf);
                case 0x0000_0050: return null;// YM3812           
                case 0x0000_0054: return null;// YM3526           
                case 0x0000_0058: return null;// Y8950            
                case 0x0000_005C: return null;// YMF262           
                case 0x0000_0060: return null;// YMF278B          
                case 0x0000_0064: return null;// YMF271           
                case 0x0000_0068: return null;// YMZ280B          
                case 0x0000_006C: return null;// new RF5C164(chipRegister, setting, vgmBuf);
                case 0x0000_0070: return null;// PWM              
                case 0x0000_0074: return null;// AY8910           
                case 0x0000_0080: return null;// GameBoy DMG      
                case 0x0000_0084: return null;// NES APU          
                case 0x0000_0088: return null;// MultiPCM         
                case 0x0000_008C: return null;// uPD7759          
                case 0x0000_0090: return null;// OKIM6258         
                case 0x0000_0098: return null;// OKIM6295         
                case 0x0000_009C: return null;// new K051649(chipRegister, setting, vgmBuf);
                case 0x0000_00A0: return null;// K054539          
                case 0x0000_00A4: return null;// new HuC6280(chipRegister, setting, vgmBuf);
                case 0x0000_00A8: return null;// new C140(chipRegister, setting, vgmBuf);
                case 0x0000_00AC: return null;// new K053260(chipRegister, setting, vgmBuf);
                case 0x0000_00B0: return null;// Pokey            
                case 0x0000_00B4: return null;// new QSound(chipRegister, setting, vgmBuf);
                case 0x0000_00B8: return null;// SCSP             
                case 0x0000_00C0: return null;// WonderSwan       
                case 0x0000_00C4: return null;// Virtual Boy VSU  
                case 0x0000_00C8: return null;// SAA1099          
                case 0x0000_00CC: return null;// ES5503           
                case 0x0000_00D0: return null;// ES5505/ES5506    
                case 0x0000_00D8: return null;// X1-010           
                case 0x0000_00DC: return null;// C352             
                case 0x0000_00E0: return null;// GA20             
                case 0x0001_0000: return new Conductor(chipRegister, setting, vgmBuf);
                case 0x0002_0001: return new YM2609(chipRegister, setting, vgmBuf);
                case 0x0003_0000: return null;// XG MU50             
                case 0x0003_0001: return null;// XG MU100            
                case 0x0003_0002: return null;// XG MU128            
                case 0x0003_0003: return null;// XG MU1000           
                case 0x0003_0004: return null;// XG MU2000           
                case 0x0003_0005: return null;// XG MU1000EX         
                case 0x0003_0006: return null;// XG MU2000EX         
                case 0x0004_0000: return null;// GS MT-32 LA           
                case 0x0004_0001: return null;// GS CM-64 LA           
                case 0x0004_0002: return null;// GS SC-55            
                case 0x0004_0003: return null;// GS SC-55mkII        
                case 0x0004_0004: return null;// GS SC-88            
                case 0x0004_0005: return null;// GS SC-88Pro         
                case 0x0004_0006: return null;// GS SC-8820          
                case 0x0004_0007: return null;// GS SC-8850          
                case 0x0004_0008: return null;// GS SD-90            
                case 0x0004_0009: return null;// GS Integra-7        
                case 0x0005_0000: return null;// new MidiGM(chipRegister, setting, vgmBuf);
                case 0x0006_0000: return null;// CSTi General          
                case 0x0007_0000: return null;// Wave General          
                default: throw new ArgumentException();
            }
        }
    }
}
