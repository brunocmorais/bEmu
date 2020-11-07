namespace bEmu.Systems.Gameboy.GPU
{
    public class VRAM
    {
        public MMU MMU { get; }
        public byte[] Bank0 { get; }
        public byte[] Bank1 { get; }
        public ushort DMASourceAddr => (ushort) ((MMU.IO[0x51] << 8) | (MMU.IO[0x52] & 0xF0));
        public ushort DMADestinationAddr => (ushort) (((MMU.IO[0x53] & 0xF) << 8) | (MMU.IO[0x54] & 0xF0));
        public byte HDMA5 
        { 
            get 
            {
                if (active) 
                    return (byte)(MMU.IO[0x55] & 0x7F); 
                else
                    return (byte)(MMU.IO[0x55] | 0x80);
            }
            set => MMU.IO[0x55] = value; 
        }
        private int startAddress = 0;
        private bool active = false;

        public VRAM(MMU mmu)
        {
            MMU = mmu;
            Bank0 = new byte[8192];
            Bank1 = new byte[8192];
            ResetDMAFlags();
        }

        private void ResetDMAFlags()
        {
            MMU.IO[0x51] = 0xFF;
            MMU.IO[0x52] = 0xFF;
            MMU.IO[0x53] = 0xFF;
            MMU.IO[0x54] = 0xFF;
            MMU.IO[0x55] = 0xFF;
        }

        private bool IsHBlankDMAActive => active && (HDMA5 & 0x80) != 0x80 && 
            MMU.IO[0x51] != 0xFF && MMU.IO[0x52] != 0xFF &&
            MMU.IO[0x53] != 0xFF && MMU.IO[0x54] != 0xFF;

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
            HDMA5 = value;

            int length = ((HDMA5 & 0x7F) + 1) << 4;
            bool hblankDMA = (value & 0x80) == 0x80;

            if (!hblankDMA)
            {
                if (active) // interromper DMA ativo
                {
                    active = false;
                    HDMA5 = ((byte)(HDMA5 & 0x7F));
                }
                else
                    StartGeneralDMATransfer(length);
            }
            else
                active = true;
        }

        private void StartGeneralDMATransfer(int length)
        {
            for (int i = 0; i < length; i++)
            {
                this[DMADestinationAddr + i] = MMU[DMASourceAddr + i];

                if (i % 0x10 == 0)
                    HDMA5 = (byte)((((length - i) >> 4) - 1) & 0x7F);
            }

            ResetDMAFlags();
        }

        public void ExecuteHBlankDMATransfer()
        {
            if (!IsHBlankDMAActive) // hblank não ativo
                return;
            
            if (startAddress == 0)
                startAddress = DMASourceAddr;

            int padding = (startAddress - DMASourceAddr);

            for (int i = 0; i < 0x10; i++)
                this[DMADestinationAddr + i + padding] = MMU[DMASourceAddr + i + padding];
            
            startAddress += 0x10;

            if ((HDMA5 & 0x7F) == 0)
            {
                active = false;
                ResetDMAFlags();
            }
            else
                HDMA5--;
        }
    }
}