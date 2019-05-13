using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MDPlayer;

namespace SoundManager
{
    public class PackData
    {
        /// <summary>
        /// Emuチップ / 実チップ
        /// </summary>
        public Chip Chip=new Chip();

        /// <summary>
        /// データの種類
        /// </summary>
        public EnmDataType Type;

        /// <summary>
        /// データのアドレス
        /// </summary>
        public int Address;

        /// <summary>
        /// データ
        /// </summary>
        public int Data;

        /// <summary>
        /// 複数データ
        /// </summary>
        public object ExData;

        public PackData()
        {

        }

        public PackData(Chip Chip, EnmDataType Type, int Address, int Data, object ExData)
        {
            this.Chip.Move(Chip);
            this.Type = Type;
            this.Address = Address;
            this.Data = Data;
            this.ExData = ExData;
        }

        public void Copy(PackData pack)
        {
            Chip.Move(pack.Chip);
            Type = pack.Type;
            Address = pack.Address;
            Data = pack.Data;
            ExData = pack.ExData;
        }

        public void Copy(Chip Chip, EnmDataType Type, int Address, int Data, object ExData)
        {
            this.Chip.Move(Chip);
            this.Type = Type;
            this.Address = Address;
            this.Data = Data;
            this.ExData = ExData;
        }
    }

    public class CntPackData
    {
        public CntPackData prev;
        public CntPackData next;

        public long Counter;

        public PackData pack = new PackData();
    }

}
