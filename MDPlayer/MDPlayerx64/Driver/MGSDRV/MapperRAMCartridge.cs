using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MGSDRV
{
    public class MapperRAMCartridge : Cartridge, iMapper
    {
        private readonly byte[][] physicalMemory;
        private readonly bool[] useFlag;
        private readonly byte[][] visibleMemory;
        private readonly int[] visibleMemorySegmentNumber;

        public int segmentSize { get; internal set; }

        public MapperRAMCartridge(int segmentSize)
        {
            this.segmentSize = segmentSize;
            physicalMemory = new byte[segmentSize][];
            useFlag = new bool[segmentSize];
            visibleMemory = new byte[4][] { null, null, null, null };
            visibleMemorySegmentNumber = new int[4];

            for (int i = 0; i < segmentSize; i++)
            {
                physicalMemory[i] = new byte[PAGE_SIZE];
                if (i < 4) SetSegmentToPage(i, i);
            }

        }

        public override byte this[ushort address]
        {
            get
            {
                int page = address / PAGE_SIZE;
                if (page > visibleMemory.Length || visibleMemory[page] == null) return 0xff;

                return visibleMemory[page][address % PAGE_SIZE];
            }
            set
            {
                int page = address / PAGE_SIZE;
                if (page > visibleMemory.Length || visibleMemory[page] == null) return;

                visibleMemory[page][address % PAGE_SIZE] = value;
            }
        }

        public int GetSegmentNumberFromPageNumber(int pageNumber)
        {
            return visibleMemorySegmentNumber[pageNumber % 4];
        }

        public void SetSegmentToPage(int segmentNumber, int pageNumber)
        {
            visibleMemory[pageNumber % 4] = physicalMemory[segmentNumber % physicalMemory.Length];
            visibleMemorySegmentNumber[pageNumber % 4] = segmentNumber % physicalMemory.Length;
            Use(segmentNumber % physicalMemory.Length);
        }

        public void ClearUseFlag()
        {
            for (int i = 0; i < useFlag.Length; i++) useFlag[i] = false;
        }

        public bool Use(int segmentNumber)
        {
            return useFlag[segmentNumber % physicalMemory.Length];
        }
    }

}
