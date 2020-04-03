using bEmu.Core.CPUs.LR35902;

namespace bEmu.Core.Systems.Gameboy
{
    public class LCD
    {
        private readonly MMU mmu;

        public LCD(MMU mmu)
        {
            this.mmu = mmu;
        }

        public byte LCDC
        {
            get { return mmu[0xFF40]; }
            set { mmu[0xFF40] = value; }
        }

        public byte STAT
        {
            get { return mmu[0xFF41]; }
            set { mmu[0xFF41] = value; }
        }

        public byte SCY
        {
            get { return mmu[0xFF42]; }
            set { mmu[0xFF42] = value; }
        }

        public byte SCX
        {
            get { return mmu[0xFF43]; }
            set { mmu[0xFF43] = value; }
        }

        public byte LY
        {
            get { return mmu[0xFF44]; }
            set { mmu[0xFF44] = value; }
        }

        public byte LYC
        {
            get { return mmu[0xFF45]; }
            set { mmu[0xFF45] = value; }
        }

        public byte BGP => mmu[0xFF47];
        public byte OBP0 => mmu[0xFF48];
        public byte OBP1 => mmu[0xFF49];

        public bool GetLCDCFlag(LCDC option)
        {
            int option1 = (int)option;
            return ((LCDC & (0x1 << option1)) >> option1) == 1;
        }

        public int GetSTATFlag(STAT option)
        {
            int op = (int) option;

            if (op < 1)
                return STAT & 0x4;

            return ((STAT & (0x1 << (int)option)) >> (int) option);
        }

        public void SetSTATFlag(STAT option, bool value)
        {
            if ((int) option < 3)
                return; 

            byte mask = (byte) (0x1 << (int) option);

            if (value)
            {
                mask = (byte) ~mask;
                STAT &= mask;
            }
            else
                STAT |= mask;
        }

        public void SetSTATMode(int number)
        {
            byte value = (byte) (number | 0xFC);
            STAT &= value;
        }

        public void SetLCDCFlag(LCDC option, bool value)
        {
            byte mask = (byte) (0x1 << (int) option);

            if (value)
            {
                mask = (byte) ~mask;
                LCDC &= mask;
            }
            else
                LCDC |= mask;
        }
    }

    public enum STAT
    {
        ModeFlag = 0,
        CoincidenceFlag = 2,
        Mode0HBlankInterrupt = 3,
        Mode1VBlankInterrupt = 4,
        Mode2OAMInterrupt = 5,
        LYCoincidenceInterrupt = 6

    }

    public enum LCDC
    {
        BGDisplay = 0,
        SpriteDisplayEnable = 1,
        SpriteSize = 2,
        BGTileMapDisplaySelect = 3,
        BGWindowTileDataSelect = 4,
        WindowDisplayEnable = 5,
        WindowTileMapDisplaySelect = 6,
        LCDDisplayEnable = 7
    }
}