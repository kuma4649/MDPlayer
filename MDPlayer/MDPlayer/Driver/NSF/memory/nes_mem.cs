using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDSound.np;

namespace MDPlayer.NSF
{
    public class nes_mem : IDevice
    {
        //# include <assert.h>
        //# include "nes_mem.h"
        protected byte[] image = new byte[0x10000];
        protected bool fds_enable;

        //  public:
        //NES_MEM();
        //  ~NES_MEM();
        //  void Reset();
        //  bool Read(UINT32 adr, UINT32 & val, UINT32 id = 0);
        //  bool Write(UINT32 adr, UINT32 val, UINT32 id = 0);
        //  bool SetImage(UINT8* data, UINT32 offset, UINT32 size);
        //  void SetFDSMode(bool);

        //namespace xgm
        //    {
        public nes_mem()
        {
            fds_enable = true;
        }

        ~nes_mem()
        {
        }

        public override void Reset()
        {
            for (int i = 0; i < 0x800; i++) image[i] = 0;
            //memset(image, 0, 0x800);
            //memset (image + 0x6000, 0, 0x2000); // 分かっててあえて初期化してません。
        }

        public bool SetImage(byte[] data, UInt32 offset, UInt32 size)
        {
            for (int i = 0; i < 0x10000; i++) image[i] = 0;
            //memset(image, 0, 0x10000);
            if (offset + size < 0x10000)
            {
                for (int i = 0; i < size; i++) image[offset + i] = data[i];
                //memcpy(image + offset, data, size);
            }
            else
            {
                for (int i = 0; i < 0x10000 - offset; i++) image[offset + i] = data[i];
                //memcpy(image + offset, data, 0x10000 - offset);
            }
            return true;
        }

        public override bool Write(UInt32 adr, UInt32 val, UInt32 id)
        {
            if (0x0000 <= adr && adr < 0x2000)
            {
                image[adr & 0x7ff] = (byte)val;
                return true;
            }
            if (0x6000 <= adr && adr < 0x8000)
            {
                image[adr] = (byte)val;
                return true;
            }
            if (0x4100 <= adr && adr < 0x4110)
            {
                image[adr] = (byte)val;
                return true;
            }
            if (fds_enable && 0x8000 <= adr && adr < 0xe000)
            {
                image[adr] = (byte)val;
            }
            return false;
        }

        public override bool Read(UInt32 adr, ref UInt32 val, UInt32 id)
        {
            if (0x0000 <= adr && adr < 0x2000)
            {
                val = image[adr & 0x7ff];
                return true;
            }
            if (0x4100 <= adr && adr < 0x4110)
            {
                val = image[adr];
                return true;
            }
            if (0x6000 <= adr && adr < 0x10000)
            {
                val = image[adr];
                return true;
            }
            val = 0;
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