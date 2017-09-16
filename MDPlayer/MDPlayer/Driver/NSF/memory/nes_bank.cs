using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDSound.np;

namespace MDPlayer.NSF
{
    public class nes_bank : IDevice
    {
        //# include <stdio.h>
        //# include <stdlib.h>
        //# include "nes_bank.h"
        /**
         * 4KB*16バンクのバンク空間
         **/
        //      class NES_BANK : public IDevice
        //{
        protected byte[] null_bank = new byte[0x1000];
        protected Int32[] bank = new Int32[256]; //ptr
        protected byte[] image;//ptr
        protected Int32[] bankswitch = new Int32[16];
        protected Int32[] bankdefault = new Int32[16];
        protected bool fds_enable;
        protected Int32 bankmax;

        //  public:
        //NES_BANK();
        //  ~NES_BANK();
        //  void Reset();
        //  bool Read(UINT32 adr, UINT32 & val, UINT32 id = 0);
        //  bool Write(UINT32 adr, UINT32 val, UINT32 id = 0);
        //  void SetBankDefault(UINT8 bank, int value);
        //  bool SetImage(UINT8* data, UINT32 offset, UINT32 size);
        //  void SetFDSMode(bool);
        //};

        // this workaround solves a problem with mirrored FDS RAM writes
        // when the same bank is used twice; some NSF rips reuse bank 00
        // in "unused" banks that actually get written to.
        // it is preferred to fix the NSFs and leave this disabled.
        //#define FDS_MEMCPY 0

        // for detecting mirrored writes in FDS NSFs
        //#define DETECT_FDS_MIRROR 0

        //#if FDS_MEMCPY
        //    static UINT8* fds_image = NULL;
        //#endif

        //namespace xgm
        //{

        public nes_bank()
        {
            image = null;
            fds_enable = false;
        }

        ~nes_bank()
        {
            //if (image!=0) delete[] image;

            //#if FDS_MEMCPY
            //        if (fds_image)
            //            delete[] fds_image;
            //#endif
        }

        public void SetBankDefault(byte bank, Int32 value)
        {
            bankdefault[bank] = value;
        }

        public bool SetImage(byte[] data, UInt32 offset, UInt32 size)
        {
            Int32 i;

            // バンクスイッチの初期値は全て「バンク無効」
            for (i = 0; i < 16; i++)
                bankdefault[i] = -1; // -1 is special empty bank

            bankmax = (Int32)((((offset & 0xfff) + size) / 0x1000) + 1);
            if (bankmax > 256)
                return false;

            //if (image!=0)            delete[] image;
            image = new byte[0x1000 * bankmax];
            for (i = 0; i < image.Length; i++) image[i] = 0;
            //memset(image, 0, 0x1000 * bankmax);
            for (i = 0; i < size; i++) image[(offset & 0xfff) + i] = data[i];
            //memcpy(image + (offset & 0xfff), data, size);

            //#if FDS_MEMCPY
            //        if (fds_image)
            //            delete[] fds_image;
            //        fds_image = new UINT8[0x10000];
            //        memset(fds_image, 0, 0x10000);
            //        for (i = 0; i < 16; i++)
            //            bank[i] = fds_image + 0x1000 * i;
            //#else
            for (i = 0; i < bankmax; i++)
                bank[i] = 0x1000 * i;
            for (i = bankmax; i < 256; i++)
                bank[i] = -1;// null_bank;
            //#endif

            return true;
        }

        public override void Reset()
        {
            for (int i = 0; i < 0x1000; i++) null_bank[i] = 0;
            //memset(null_bank, 0, 0x1000);
            for (int i = 0; i < 16; i++)
            {
                bankswitch[i] = bankdefault[i];

                //#if FDS_MEMCPY
                //            bankswitch[i] = i;
                //            if (bankdefault[i] == -1 || bankdefault[i] >= bankmax)
                //                memset(bank[i], 0, 0x1000);
                //            else
                //                memcpy(bank[i], image + (bankdefault[i] * 0x1000), 0x1000);
                //#endif
            }
        }

        public override bool Write(UInt32 adr, UInt32 val, UInt32 id)
        {
            //#if FDS_MEMCPY
            //        if (!fds_enable)
            //#endif
            if (0x5ff8 <= adr && adr < 0x6000)
            {
                bankswitch[(adr & 7) + 8] = (Int32)(val & 0xff);
                return true;
            }

            if (fds_enable)
            {
                //#if FDS_MEMCPY
                //            if (0x5ff6 <= adr && adr < 0x6000)
                //            {
                //                int b = adr - 0x5ff0;
                //                if (int(val) >= bankmax)
                //                    memset(bank[b], 0, 0x1000);
                //                else
                //                    memcpy(bank[b], image + (val * 0x1000), 0x1000);
                //                return true;
                //            }
                //#else
                if (0x5ff6 <= adr && adr < 0x5ff8)
                {
                    bankswitch[adr & 7] = (Int32)(val & 0xff);
                    return true;
                }
                //#endif

                if (0 <= bankswitch[adr >> 12] && 0x6000 <= adr && adr < 0xe000)
                {
                    // for detecting FDS ROMs with improper mirrored writes
                    //#if DETECT_FDS_MIRROR
                    //                for (int i = 0; i < 14; ++i)
                    //                {
                    //                    int b = adr >> 12;
                    //                    if (i != b && bankswitch[i] == bankswitch[b])
                    //                    {
                    //                        DEBUG_OUT("[%04X] write mirrored to [%04X] = %02X\n",
                    //                          adr, (i * 0x1000) | (adr & 0x0fff), val);
                    //                    }
                    //                }
                    //#endif

                    image[bank[bankswitch[adr >> 12]] + (adr & 0x0fff)] = (byte)val;
                    return true;
                }
            }

            return false;
        }

        public override bool Read(UInt32 adr, ref UInt32 val, UInt32 id)
        {
            if (0x5ff8 <= adr && adr < 0x5fff)
            {
                val = (UInt32)bankswitch[(adr & 7) + 8];
                return true;
            }

            if (0 <= bankswitch[adr >> 12] && 0x8000 <= adr && adr < 0x10000)
            {
                val = image[bank[bankswitch[adr >> 12]] + (adr & 0xfff)];
                return true;
            }

            if (fds_enable)
            {
                if (0x5ff6 <= adr && adr < 0x5ff8)
                {
                    val = (UInt32)bankswitch[adr & 7];
                    return true;
                }

                if (0 <= bankswitch[adr >> 12] && 0x6000 <= adr && adr < 0x8000)
                {
                    val = image[bank[bankswitch[adr >> 12]] + (adr & 0xfff)];
                    return true;
                }
            }

            return false;
        }

        public void SetFDSMode(bool t)
        {
            fds_enable = t;
        }

        public override void SetOption(int id, int val)
        {
            throw new NotImplementedException();
        }
    }                               // namespace
}

