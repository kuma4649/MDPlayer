using musicDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver
{
    public class MucomDotNET : baseDriver
    {
        private InstanceMarker im = null;

        public MucomDotNET(InstanceMarker mucomDotNET_Im)
        {
            im = mucomDotNET_Im;
        }

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            throw new NotImplementedException();
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            musicDriverInterface.iCompiler mucomCompiler = im.GetCompiler("mucomDotNET.Compiler.Compiler");
            musicDriverInterface.iDriver mucomDriver = im.GetDriver("mucomDotNET.Driver.Driver");

            this.vgmBuf = vgmBuf;
            this.chipRegister = chipRegister;
            this.model = model;
            this.useChip = useChip;
            this.latency = latency;
            this.waitTime = waitTime;

            throw new NotImplementedException();
        }

        public override void oneFrameProc()
        {
            throw new NotImplementedException();
        }
    }
}
