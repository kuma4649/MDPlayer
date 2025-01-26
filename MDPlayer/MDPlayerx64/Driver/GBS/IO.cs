using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.GBS
{
    public class IO
    {
        // FF00-FF7F   I/O Ports
        private Action<byte>[] iow;
        private Func<byte>[] ior;
        private ChipRegister cr;
        private MDSound.gb gb;
        private EnmModel model;
        private long vgmFrameCounter;

        private const Int32 NR10 = 0x00;
        private const Int32 NR11 = 0x01;
        private const Int32 NR12 = 0x02;
        private const Int32 NR13 = 0x03;
        private const Int32 NR14 = 0x04;
        // 0x05
        private const Int32 NR21 = 0x06;
        private const Int32 NR22 = 0x07;
        private const Int32 NR23 = 0x08;
        private const Int32 NR24 = 0x09;
        private const Int32 NR30 = 0x0A;
        private const Int32 NR31 = 0x0B;
        private const Int32 NR32 = 0x0C;
        private const Int32 NR33 = 0x0D;
        private const Int32 NR34 = 0x0E;
        // 0x0F
        private const Int32 NR41 = 0x10;
        private const Int32 NR42 = 0x11;
        private const Int32 NR43 = 0x12;
        private const Int32 NR44 = 0x13;
        private const Int32 NR50 = 0x14;
        private const Int32 NR51 = 0x15;
        private const Int32 NR52 = 0x16;
        // 0x17 - 0x1F
        private const Int32 AUD3W0 = 0x20;
        private const Int32 AUD3W1 = 0x21;
        private const Int32 AUD3W2 = 0x22;
        private const Int32 AUD3W3 = 0x23;
        private const Int32 AUD3W4 = 0x24;
        private const Int32 AUD3W5 = 0x25;
        private const Int32 AUD3W6 = 0x26;
        private const Int32 AUD3W7 = 0x27;
        private const Int32 AUD3W8 = 0x28;
        private const Int32 AUD3W9 = 0x29;
        private const Int32 AUD3WA = 0x2A;
        private const Int32 AUD3WB = 0x2B;
        private const Int32 AUD3WC = 0x2C;
        private const Int32 AUD3WD = 0x2D;
        private const Int32 AUD3WE = 0x2E;
        private const Int32 AUD3WF = 0x2F;

        //http://bgb.bircd.org/pandocs.htm#soundcontroller

        public IO(ChipRegister cr, MDSound.gb gb,EnmModel model)
        {
            this.cr = cr;
            this.gb = gb;
            this.model = model;

            ior = new Func<byte>[]
            {
                //0x00
                null,null,null,null,
                null,null,null,null,
                //0x08
                null,null,null,null,
                null,null,null,null,
                //0x10
                R_FF10_NR10_Channel_1_Sweep_register,
                R_FF11_NR11_Channel_1_Sound_length_Wave_pattern_duty,
                R_FF12_NR12_Channel_1_Volume_Envelope,
                null,

                R_FF14_NR14_Channel_1_Frequency_hi,
                null,
                R_FF16_NR21_Channel_2_Sound_Length_Wave_Pattern_Duty,
                R_FF17_NR22_Channel_2_Volume_Envelope,
                //0x18
                null ,
                R_FF19_NR24_Channel_2_Frequency_hi_data,
                R_FF1A_NR30_Channel_3_Sound_on_off,
                R_FF1B_NR31_Channel_3_Sound_Length,

                R_FF1C_NR32_Channel_3_Select_output_level,
                null,
                R_FF1E_NR34_Channel_3_Frequencys_higher_data,
                null,
                //0x20
                R_FF20_NR41_Channel_4_Sound_Length,
                R_FF21_NR42_Channel_4_Volume_Envelope,
                R_FF22_NR43_Channel_4_Polynomial_Counter,
                R_FF23_NR44_Channel_4_Counter_consecutive_Inital,

                R_FF24_NR50_Channel_control_ON_OFF_Volume,
                R_FF25_NR51_SelectionOfSoundOutputTerminal,
                R_FF26_NR52_Sound_on_off,
                null,
                //0x28
                null,null,null,null,
                null,null,null,null,
                //0x30
                R_FF30_Wave_Pattern_RAM,
                R_FF31_Wave_Pattern_RAM,
                R_FF32_Wave_Pattern_RAM,
                R_FF33_Wave_Pattern_RAM,

                R_FF34_Wave_Pattern_RAM,
                R_FF35_Wave_Pattern_RAM,
                R_FF36_Wave_Pattern_RAM,
                R_FF37_Wave_Pattern_RAM,
                //0x38
                R_FF38_Wave_Pattern_RAM,
                R_FF39_Wave_Pattern_RAM,
                R_FF3A_Wave_Pattern_RAM,
                R_FF3B_Wave_Pattern_RAM,

                R_FF3C_Wave_Pattern_RAM,
                R_FF3D_Wave_Pattern_RAM,
                R_FF3E_Wave_Pattern_RAM,
                R_FF3F_Wave_Pattern_RAM,
                //0x40
                null,null,null,null,
                null,null,null,null,
                //0x48
                null,null,null,null,
                null,null,null,null,
                //0x50
                null,null,null,null,
                null,null,null,null,
                //0x58
                null,null,null,null,
                null,null,null,null,
                //0x60
                null,null,null,null,
                null,null,null,null,
                //0x68
                null,null,null,null,
                null,null,null,null,
                //0x70
                null,null,null,null,
                null,null,null,null,
                //0x78
                null,null,null,null,
                null,null,null,null,
            };

            iow = new Action<byte>[]
            {
                //0x00
                null,null,null,null,
                null,null,null,
                W_FF07_TAC_Timer_Control ,
                //0x08
                null,null,null,null,
                null,null,null,null,
                //0x10
                W_FF10_NR10_Channel_1_Sweep_register,
                W_FF11_NR11_Channel_1_Sound_length_Wave_pattern_duty,
                W_FF12_NR12_Channel_1_Volume_Envelope,
                W_FF13_NR13_Channel_1_Frequency_lo,

                W_FF14_NR14_Channel_1_Frequency_hi,
                null,
                W_FF16_NR21_Channel_2_Sound_length_Wave_pattern_duty,
                W_FF17_NR22_Channel_2_Volume_Envelope,
                //0x18
                W_FF18_NR23_Channel_2_Frequency_lo,
                W_FF19_NR24_Channel_2_Frequency_hi,
                W_FF1A_NR30_Channel_3_Sound_on_off,
                W_FF1B_NR31_Channel_3_Sound_Length,

                W_FF1C_NR32_Channel_3_Select_output_level ,
                W_FF1D_NR33_Channel_3_Frequencys_lower_data ,
                W_FF1E_NR34_Channel_3_Frequencys_higher_data ,
                null,
                //0x20
                W_FF20_NR41_Channel_4_Sound_Length ,
                W_FF21_NR42_Channel_4_Volume_Envelope,
                W_FF22_NR43_Channel_4_Polynomial_Counter,
                W_FF23_NR44_Channel_4_Counter_consecutive_Inital ,

                W_FF24_NR50_ChannelControl,
                W_FF25_NR51_SelectionOfSoundOutputTerminal,
                W_FF26_NR52_SoundOnOff,null,
                //0x28
                null,null,null,null,                null,null,null,null,
                //0x30
                W_FF30_Wave_Pattern_RAM,
                W_FF31_Wave_Pattern_RAM,
                W_FF32_Wave_Pattern_RAM,
                W_FF33_Wave_Pattern_RAM,

                W_FF34_Wave_Pattern_RAM,
                W_FF35_Wave_Pattern_RAM,
                W_FF36_Wave_Pattern_RAM,
                W_FF37_Wave_Pattern_RAM,
                //0x38
                W_FF38_Wave_Pattern_RAM,
                W_FF39_Wave_Pattern_RAM,
                W_FF3A_Wave_Pattern_RAM,
                W_FF3B_Wave_Pattern_RAM,

                W_FF3C_Wave_Pattern_RAM,
                W_FF3D_Wave_Pattern_RAM,
                W_FF3E_Wave_Pattern_RAM,
                W_FF3F_Wave_Pattern_RAM,
                //0x40
                null,null,null,null,                null,null,null,null,
                //0x48
                null,null,null,null,                null,null,null,null,
                //0x50
                null,null,null,null,                null,null,null,null,
                //0x58
                null,null,null,null,                null,null,null,null,
                //0x60
                null,null,null,null,                null,null,null,null,
                //0x68
                null,null,null,null,                null,null,null,null,
                //0x70
                null,null,null,null,                null,null,null,null,
                //0x78
                null,null,null,null,                null,null,null,null,
            };
        }

        public void Write(byte pc, byte dat,long vgmFrameCounter)
        {
            this.vgmFrameCounter = vgmFrameCounter;
            if (iow[(byte)pc] != null) iow[(byte)pc](dat);
            else
            {
#if DEBUG
                Console.WriteLine("Write unkown IO Address:${0:X04} dat:${1:X02}", pc + 0xff00, dat);
#endif
                throw new NotImplementedException();
            }
        }

        public byte Read(byte pc)
        {
            if (ior[(byte)pc] != null) return ior[(byte)pc]();
            else
            {
#if DEBUG
                Console.WriteLine("Read unkown IO Address:${0:X04}", pc + 0xff00);
#endif
                throw new NotImplementedException();
            }
        }




        //参考
        //http://bgb.bircd.org/pandocs.htm#soundcontroller

        private void W_FF07_TAC_Timer_Control(byte dat)
        {
#if DEBUG
            Console.WriteLine("Write FF07 - TAC - Timer Control (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF10_NR10_Channel_1_Sweep_register()
        {
            byte dat = gb.gb_sound_r(0, NR10);
#if DEBUG
            Console.WriteLine("Read FF10 - NR10 - Channel 1 Sweep register (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF10_NR10_Channel_1_Sweep_register(byte dat)
        {
            cr.setDMGRegister(0, NR10, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF10 - NR10 - Channel 1 Sweep register (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF11_NR11_Channel_1_Sound_length_Wave_pattern_duty()
        {
            byte dat = gb.gb_sound_r(0, NR11);
#if DEBUG
            Console.WriteLine("Read FF11 - NR11 - Channel 1 Sound length/Wave pattern duty (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF11_NR11_Channel_1_Sound_length_Wave_pattern_duty(byte dat)
        {
            cr.setDMGRegister(0, NR11, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF11 - NR11 - Channel 1 Sound length/Wave pattern duty (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF12_NR12_Channel_1_Volume_Envelope()
        {
            byte dat = gb.gb_sound_r(0, NR12);
#if DEBUG
            Console.WriteLine("Read FF12 - NR12 - Channel 1 Volume Envelope (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF12_NR12_Channel_1_Volume_Envelope(byte dat)
        {
            cr.setDMGRegister(0, NR12, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF12 - NR12 - Channel 1 Volume Envelope (R/W) ${0:X02}", dat);
#endif
        }

        private void W_FF13_NR13_Channel_1_Frequency_lo(byte dat)
        {
            cr.setDMGRegister(0, NR13, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF13 - NR13 - Channel 1 Frequency lo (W) ${0:X02}", dat);
#endif
        }

        private byte R_FF14_NR14_Channel_1_Frequency_hi()
        {
            byte dat = gb.gb_sound_r(0, NR14);
#if DEBUG
            Console.WriteLine("Read FF14 - NR14 - Channel 1 Frequency hi (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF14_NR14_Channel_1_Frequency_hi(byte dat)
        {
            cr.setDMGRegister(0, NR14, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF14 - NR14 - Channel 1 Frequency hi (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF16_NR21_Channel_2_Sound_Length_Wave_Pattern_Duty()
        {
            byte dat = gb.gb_sound_r(0, NR21);
#if DEBUG
            Console.WriteLine("Read FF16 - NR21 - Channel 2 Sound Length/Wave Pattern Duty (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF16_NR21_Channel_2_Sound_length_Wave_pattern_duty(byte dat)
        {
            cr.setDMGRegister(0, NR21, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF16 - NR21 - Channel 2 Sound Length/Wave Pattern Duty (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF17_NR22_Channel_2_Volume_Envelope()
        {
            byte dat = gb.gb_sound_r(0, NR22);
#if DEBUG
            Console.WriteLine("Read FF17 - NR22 - Channel 2 Volume Envelope (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF17_NR22_Channel_2_Volume_Envelope(byte dat)
        {
            cr.setDMGRegister(0, NR22, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF17 - NR22 - Channel 2 Volume Envelope (R/W) ${0:X02}", dat);
#endif
        }

        private void W_FF18_NR23_Channel_2_Frequency_lo(byte dat)
        {
            cr.setDMGRegister(0, NR23, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF18 - NR23 - Channel 2 Frequency lo data (W) ${0:X02}", dat);
#endif
        }

        private byte R_FF19_NR24_Channel_2_Frequency_hi_data()
        {
            byte dat = gb.gb_sound_r(0, NR24);
#if DEBUG
            Console.WriteLine("Read FF19 - NR24 - Channel 2 Frequency hi data (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF19_NR24_Channel_2_Frequency_hi(byte dat)
        {
            cr.setDMGRegister(0, NR24, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF19 - NR24 - Channel 2 Frequency hi data (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF1A_NR30_Channel_3_Sound_on_off()
        {
            byte dat = gb.gb_sound_r(0, NR30);
#if DEBUG
            Console.WriteLine("Read FF1A - NR30 - Channel 3 Sound on/off (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF1A_NR30_Channel_3_Sound_on_off(byte dat)
        {
            cr.setDMGRegister(0, NR30, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF1A - NR30 - Channel 3 Sound on/off (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF1B_NR31_Channel_3_Sound_Length()
        {
            byte dat = gb.gb_sound_r(0, NR31);
#if DEBUG
            Console.WriteLine("Read FF1B - NR31 - Channel 3 Sound Length ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF1B_NR31_Channel_3_Sound_Length(byte dat)
        {
            cr.setDMGRegister(0, NR31, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF1B - NR31 - Channel 3 Sound Length ${0:X02}", dat);
#endif
        }

        private byte R_FF1C_NR32_Channel_3_Select_output_level()
        {
            byte dat = gb.gb_sound_r(0, NR32);
#if DEBUG
            Console.WriteLine("Read FF1C - NR32 - Channel 3 Select output level (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF1C_NR32_Channel_3_Select_output_level(byte dat)
        {
            cr.setDMGRegister(0, NR32, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF1C - NR32 - Channel 3 Select output level (R/W) ${0:X02}", dat);
#endif
        }

        private void W_FF1D_NR33_Channel_3_Frequencys_lower_data(byte dat)
        {
            cr.setDMGRegister(0, NR33, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF1D - NR33 - Channel 3 Frequency's lower data (W) ${0:X02}", dat);
#endif
        }

        private byte R_FF1E_NR34_Channel_3_Frequencys_higher_data()
        {
            byte dat = gb.gb_sound_r(0, NR34);
#if DEBUG
            Console.WriteLine("Read FF1E - NR34 - Channel 3 Frequency's higher data (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF1E_NR34_Channel_3_Frequencys_higher_data(byte dat)
        {
            cr.setDMGRegister(0, NR34, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF1E - NR34 - Channel 3 Frequency's higher data (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF20_NR41_Channel_4_Sound_Length()
        {
            byte dat = gb.gb_sound_r(0, NR41);
#if DEBUG
            Console.WriteLine("Read FF20 - NR41 - Channel 4 Sound Length (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF20_NR41_Channel_4_Sound_Length(byte dat)
        {
            cr.setDMGRegister(0, NR41, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF20 - NR41 - Channel 4 Sound Length (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF21_NR42_Channel_4_Volume_Envelope()
        {
            byte dat = gb.gb_sound_r(0, NR42);
#if DEBUG
            Console.WriteLine("Read FF21 - NR42 - Channel 4 Volume Envelope (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF21_NR42_Channel_4_Volume_Envelope(byte dat)
        {
            cr.setDMGRegister(0, NR42, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF21 - NR42 - Channel 4 Volume Envelope (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF22_NR43_Channel_4_Polynomial_Counter()
        {
            byte dat = gb.gb_sound_r(0, NR43);
#if DEBUG
            Console.WriteLine("Read FF22 - NR43 - Channel 4 Polynomial Counter (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF22_NR43_Channel_4_Polynomial_Counter(byte dat)
        {
            cr.setDMGRegister(0, NR43, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF22 - NR43 - Channel 4 Polynomial Counter (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF23_NR44_Channel_4_Counter_consecutive_Inital()
        {
            byte dat = gb.gb_sound_r(0, NR44);
#if DEBUG
            Console.WriteLine("Read FF23 - NR44 - Channel 4 Counter/consecutive; Inital (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF23_NR44_Channel_4_Counter_consecutive_Inital(byte dat)
        {
            cr.setDMGRegister(0, NR44, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF23 - NR44 - Channel 4 Counter/consecutive; Inital (R/W) ${0:X02}", dat);
#endif
        }
        private byte R_FF24_NR50_Channel_control_ON_OFF_Volume()
        {
            byte dat = gb.gb_sound_r(0, NR50);
#if DEBUG
            Console.WriteLine("Read FF24 - NR50 - Channel control / ON-OFF / Volume (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF24_NR50_ChannelControl(byte dat)
        {
            cr.setDMGRegister(0, NR50, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF24 - NR50 - Channel control / ON-OFF / Volume (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF25_NR51_SelectionOfSoundOutputTerminal()
        {
            byte dat = gb.gb_sound_r(0, NR51);
#if DEBUG
            Console.WriteLine("Read FF25 - NR51 - Selection of Sound output terminal (R/W) ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF25_NR51_SelectionOfSoundOutputTerminal(byte dat)
        {
            cr.setDMGRegister(0, NR51, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF25 - NR51 - Selection of Sound output terminal (R/W) ${0:X02}", dat);
#endif
        }

        private byte R_FF26_NR52_Sound_on_off()
        {
            byte dat = gb.gb_sound_r(0, NR52);
#if DEBUG
            Console.WriteLine("Read FF26 - NR52 - Sound on/off ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF26_NR52_SoundOnOff(byte dat)
        {
            cr.setDMGRegister(0, NR52, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF26 - NR52 - Sound on/off ${0:X02}", dat);
#endif
        }

        private byte R_FF30_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3W0);
#if DEBUG
            Console.WriteLine("Read FF30 - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF30_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3W0, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF30 - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF31_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3W1);
#if DEBUG
            Console.WriteLine("Read FF31 - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF31_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3W1, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF31 - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF32_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3W2);
#if DEBUG
            Console.WriteLine("Read FF32 - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF32_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3W2, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF32 - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF33_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3W3);
#if DEBUG
            Console.WriteLine("Read FF33 - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF33_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3W3, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF33 - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF34_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3W4);
#if DEBUG
            Console.WriteLine("Read FF34 - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF34_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3W4, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF34 - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF35_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3W5);
#if DEBUG
            Console.WriteLine("Read FF35 - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF35_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3W5, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF35 - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF36_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3W6);
#if DEBUG
            Console.WriteLine("Read FF36 - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF36_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3W6, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF36 - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF37_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3W7);
#if DEBUG
            Console.WriteLine("Read FF37 - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF37_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3W7, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF37 - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF38_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3W8);
#if DEBUG
            Console.WriteLine("Read FF38 - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF38_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3W8, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF38 - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF39_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3W9);
#if DEBUG
            Console.WriteLine("Read FF39 - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF39_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3W9, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF39 - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF3A_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3WA);
#if DEBUG
            Console.WriteLine("Read FF3A - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF3A_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3WA, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF3A - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF3B_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3WB);
#if DEBUG
            Console.WriteLine("Read FF3B - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF3B_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3WB, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF3B - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF3C_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3WC);
#if DEBUG
            Console.WriteLine("Read FF3C - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF3C_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3WC, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF3C - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF3D_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3WD);
#if DEBUG
            Console.WriteLine("Read FF3D - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF3D_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3WD, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF3D - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF3E_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3WE);
#if DEBUG
            Console.WriteLine("Read FF3E - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF3E_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3WE, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF3E - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

        private byte R_FF3F_Wave_Pattern_RAM()
        {
            byte dat = gb.gb_sound_r(0, AUD3WF);
#if DEBUG
            Console.WriteLine("Read FF3F - Wave Pattern RAM ${0:X02}", dat);
#endif
            return dat;
        }

        private void W_FF3F_Wave_Pattern_RAM(byte dat)
        {
            cr.setDMGRegister(0, AUD3WF, dat, model, vgmFrameCounter);
#if DEBUG
            Console.WriteLine("Write FF3F - Wave Pattern RAM ${0:X02}", dat);
#endif
        }

    }
}
