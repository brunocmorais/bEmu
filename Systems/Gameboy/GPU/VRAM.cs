namespace bEmu.Systems.Gameboy.GPU
{
    public class VRAM
    {
        public MMU MMU { get; }
        public byte[] Bank0 { get; }
        public byte[] Bank1 { get; }
        public ushort DMASourceAddr => (ushort) ((MMU[0xFF51] << 8) | (MMU[0xFF52] & 0xF0));
        public ushort DMADestinationAddr => (ushort) (((MMU[0xFF53] & 0xF) << 8) | (MMU[0xFF54] & 0xF0));
        private bool hblankDMA;
        private int transferLength;
        private int startAddr;

        public VRAM(MMU mmu)
        {
            MMU = mmu;
            Bank0 = new byte[8192];
            Bank1 = new byte[8192];
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
            get { return VBK ? Bank1[index] : Bank0[index]; }
            set 
            {
                if (VBK)
                    Bank1[index] = value;
                else
                    Bank0[index] = value;
            }
        }

        public void StartDMATransfer(byte value)
        {
            hblankDMA = (value & 0x80) == 0x80;
            transferLength = ((value & 0x7F) + 1) << 4;

            if (!hblankDMA)
            {
                for (int i = 0; i < transferLength; i++)
                    MMU[DMADestinationAddr - 0x8000 + i] = MMU[DMASourceAddr + i];
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

        public Background GetBackgroundPaletteType(int addr)
        {
            return new Background
            (
                (PaletteType)((Bank1[addr] & 0x7) + 3),
                (Bank1[addr] & 0x8) == 0x8 ? 1 : 0,
                (Bank1[addr] & 0x20) == 0x20,
                (Bank1[addr] & 0x40) == 0x40,
                (Bank1[addr] & 0x80) == 0x80
            );
        }
    }
}