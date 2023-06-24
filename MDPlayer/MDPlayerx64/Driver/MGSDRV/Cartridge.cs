namespace MDPlayer.Driver.MGSDRV
{
    public abstract class Cartridge
    {
        public abstract byte this[ushort address] { get; set; }

        public const int PAGE_SIZE = 16 * 1024;

    }
}
