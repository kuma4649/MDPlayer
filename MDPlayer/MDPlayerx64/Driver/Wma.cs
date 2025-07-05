using MDPlayer;
using MDPlayer.Driver.ZMS.nise68;
using System.Text;

namespace MDPlayerx64.Driver
{
    public class Wma : baseDriver
    {

        public System.Drawing.Image img = null;

        public override GD3 getGD3Info(byte[] buf, uint vgmGd3)
        {
            GD3 ret = new GD3();
            return ret;
        }

        public override bool init(byte[] vgmBuf, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException();
        }

        public override bool init(byte[] vgmBuf, int fileType, ChipRegister chipRegister, EnmModel model, EnmChip[] useChip, uint latency, uint waitTime)
        {
            throw new NotImplementedException();
        }

        public override void oneFrameProc()
        {
            throw new NotImplementedException();
        }


    }
}


