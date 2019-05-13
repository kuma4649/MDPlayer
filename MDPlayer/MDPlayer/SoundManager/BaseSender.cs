using MDPlayer;

namespace SoundManager
{
    public class BaseSender : BaseMakerSender
    {
        protected long Counter = 0;
        protected Chip Chip = new Chip();
        protected EnmDataType Type = 0;
        protected int Address = 0;
        protected int Data = 0;
        protected object ExData = null;

        protected int ringBufferSize;
    }

}
