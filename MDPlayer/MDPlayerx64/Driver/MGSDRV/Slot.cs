namespace MDPlayer.Driver.MGSDRV
{
    public class Slot
    {
        public Cartridge[][] slots;

        public Cartridge[] pages = new Cartridge[4];
        public slotPos[] pagesSlotPos = new slotPos[4] { new slotPos(), new slotPos(), new slotPos(), new slotPos() };
        public int[] currentExtSlotPos = new int[4] { 0, 0, 0, 0 };

        public void SetPageFromSlot(int page, int basic)
        {
            pages[page] = slots[basic][currentExtSlotPos[basic]];
            pagesSlotPos[page].basic = basic;
            pagesSlotPos[page].extend = currentExtSlotPos[basic];
        }
        public void SetPageFromSlot(int page, int basic, int extend)
        {
            pages[page] = slots[basic][extend];
            pagesSlotPos[page].basic = basic;
            pagesSlotPos[page].extend = extend;
            currentExtSlotPos[basic] = extend;
        }
    }

    public class slotPos
    {
        public int basic;
        public int extend;
    }
}
