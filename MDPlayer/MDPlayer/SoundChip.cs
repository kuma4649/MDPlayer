using System;

namespace MDPlayer
{
    public class SoundChip
    {
        //public NScci.NScci nScci;
        private NScci.NSoundChip nSoundChip=null;
        private NScci.NSoundInterface nSoundInterface=null;
        private NScci.NSoundInterfaceManager nSoundInterfaceManager=null;

        public void setRegister(int v1, int v2)
        {
            nSoundChip.setRegister(v1, v2);
        }

        public void sendData()
        {
            nSoundInterfaceManager.sendData();
        }

        public bool isBufferEmpty()
        {
            return nSoundInterfaceManager.isBufferEmpty();
        }

        public void init()
        {
            nSoundChip.init();
        }
    }
}
