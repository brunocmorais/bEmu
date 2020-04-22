namespace bEmu.Core.Systems.Gameboy
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

        public Sprite(int x, int y, byte address, byte attr)
        {
            X = x;
            Y = y;
            Address = address;
            Attr = attr;
        }
    }
}