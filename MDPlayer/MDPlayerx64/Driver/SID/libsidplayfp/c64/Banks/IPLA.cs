namespace Driver.libsidplayfp.c64.Banks
{
    public interface IPLA
    {
        void setCpuPort(byte state);
        byte getLastReadByte();
        Int64 getPhi2Time();
    }
}
