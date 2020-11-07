namespace bEmu.Systems.Gameboy.GPU
{
    public class Sprite
    {
        public int X { get; }
        public int Y { get; }        
        public byte Address { get; }
        public byte Attr { get; }
        public int LineOffset { get; set; }
        public int Size { get; set; }
        public PaletteType PaletteType => (Attr & 0x10) == 0x10 ? PaletteType.OPB1 : PaletteType.OBP0;
        public bool Priority => (Attr & 0x80) == 0x80;
        public bool XFlip => (Attr & 0x20) == 0x20;
        public bool YFlip => (Attr & 0x40) == 0x40;
        public int TileVRAMBankNumber => (Attr & 0x8) == 0x8 ? 1 : 0;
        public PaletteType ColorPaletteType
        {
            get
            {
                int value = Attr & 0x7;

                switch (value)
                {
                    case 0: return PaletteType.OBJ0;
                    case 1: return PaletteType.OBJ1;
                    case 2: return PaletteType.OBJ2;
                    case 3: return PaletteType.OBJ3;
                    case 4: return PaletteType.OBJ4;
                    case 5: return PaletteType.OBJ5;
                    case 6: return PaletteType.OBJ6;
                    case 7: return PaletteType.OBJ7;
                    default: return 0;
                }
            }
        }

        public int PaletteAddress => (Address << 4) + (2 * ((LineOffset) % Size));

        public Sprite(int x, int y, byte address, byte attr)
        {
            X = x;
            Y = y;
            Address = address;
            Attr = attr;
        }
    }
}