namespace Driver.libsidplayfp.c64.Banks
{
    public interface IBank
    {
        void poke(UInt16 address, byte value);
        byte peek(UInt16 address);
    }
}
