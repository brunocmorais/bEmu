namespace bEmu.Core.Systems.Gameboy.GPU
{
    public class VRAM
    {
        public MMU MMU { get; }
        private readonly byte[] bank0;
        private readonly byte[] bank1;
        public ushort DMASourceAddr => (ushort) ((MMU[0xFF51] << 8) | (MMU[0xFF52] & 0xF0));
        public ushort DMADestinationAddr => (ushort) (((MMU[0xFF53] & 0xF) << 8) | (MMU[0xFF54] & 0xF0));
        private bool hblankDMA;
        private int transferLength;
        private int startAddr;

        public VRAM(MMU mmu)
        {
            MMU = mmu;
            bank0 = new byte[8192];
            bank1 = new byte[8192];
            InitDMAConfig();
        }

        public byte HDMA5
        {
            get { return MMU[0xFF55]; }
            set { MMU[0xFF55] = value; }
        }

        public bool VBK 
        {
            get { return (MMU[0xFF4F] & 0x1) == 0x1; }
            set { MMU[0xFF4F] = (byte) (value ? 0x1 : 0x0); }
        }
        
        public byte this[int index]
        {
            get { return VBK ? bank1[index] : bank0[index]; }
            set 
            {
                if (VBK)
                    bank1[index] = value;
                else
                    bank0[index] = value;
            }
        }

        public void StartDMATransfer(byte value)
        {
            hblankDMA = (value & 0x80) == 0x80;
            transferLength = ((value & 0x7F) + 1) << 4;

            if (!hblankDMA)
            {
                for (int i = 0; i < transferLength; i++)
                    this[DMADestinationAddr - 0x8000 + i] = MMU[DMASourceAddr + i];
            }
        }

        public void ExecuteHBlankDMATransfer()
        {
            if (!hblankDMA)
                return;
            
            if (startAddr == 0)
                startAddr = DMASourceAddr;

            for (int i = 0; i < 0x10; i++)
                this[DMADestinationAddr - 0x8000 + i + startAddr] = MMU[DMASourceAddr + i + startAddr];

            startAddr += 0x10;

            HDMA5--;

            if (HDMA5 == 0xFF) // transferência acabou
                InitDMAConfig();
        }

        private void InitDMAConfig()
        {
            hblankDMA = false;
            startAddr = 0;
            transferLength = 0;
        }

        public PaletteType GetBackgroundPaletteType(int addr)
        {
            int value = bank1[addr] & 0x7;

            switch (value)
            {
                case 0: return PaletteType.BG0;
                case 1: return PaletteType.BG1;
                case 2: return PaletteType.BG2;
                case 3: return PaletteType.BG3;
                case 4: return PaletteType.BG4;
                case 5: return PaletteType.BG5;
                case 6: return PaletteType.BG6;
                case 7: return PaletteType.BG7;
                default: return 0;
            }
        }
    }
}