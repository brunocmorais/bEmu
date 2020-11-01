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
        private int startAddr;
        private bool isActive;
        private byte transferLength;

        public VRAM(MMU mmu)
        {
            MMU = mmu;
            Bank0 = new byte[8192];
            Bank1 = new byte[8192];
            isActive = false;
            transferLength = 0xFF;
            MMU[0xFF51] = 0xFF;
            MMU[0xFF52] = 0xFF;
            MMU[0xFF53] = 0xFF;
            MMU[0xFF54] = 0xFF;
            InitDMAConfig();
        }

        public byte HDMA5
        {
            get
            {
                byte value = 0;

                if (!isActive)
                    value |= 0x80;

                value |= transferLength;

                return value;
            }
            set
            {
                if (isActive)
                {
                    if ((value & 0x80) == 0)
                    {
                        isActive = false;
                    }
                }
                else
                {
                    transferLength = (byte)(value & 0x7F);
                    hblankDMA = (value & 0x80) == 0x80;

                    StartDMATransfer(value);
                }
            }
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

        private void StartDMATransfer(byte value)
        {
            int length = ((transferLength) + 1) << 4;
            isActive = true;

            if (!hblankDMA)
            {
                for (int i = 0; i < length; i++)
                {
                    this[DMADestinationAddr + i] = MMU[DMASourceAddr + i];

                    if (!isActive)
                        break;

                    if (i % 0x10 == 0)
                        transferLength--;
                }

                InitDMAConfig();
            }
        }

        public void ExecuteHBlankDMATransfer()
        {
            if (!hblankDMA)
                return;
            
            if (startAddr == 0)
                startAddr = DMASourceAddr;

            for (int i = 0; i < 0x10; i++)
                this[DMADestinationAddr + i + (startAddr - DMASourceAddr)] = MMU[DMASourceAddr + i + startAddr];

            transferLength--;
            startAddr += 0x10;

            if (HDMA5 == 0xFF) // transferência acabou
                InitDMAConfig();
        }

        private void InitDMAConfig()
        {
            // HDMA5 = 0x80;
            hblankDMA = false;
            startAddr = 0;
            isActive = false;
            // transferLength = 0;
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