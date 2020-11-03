namespace bEmu.Systems.Gameboy.GPU
{
    public class Tile
    {
        private readonly BackgroundMap backgroundMap;
        private int Map => backgroundMap.Window ? backgroundMap.WindowMapSelect : backgroundMap.BackgroundMapSelect;
        private VRAM VRAM => backgroundMap.MMU.VRAM;
        public int X { get; }
        public int Y { get; }
        public int TileNumber
        {
            get
            {
                if (TileVRAMBankNumber == 1 && backgroundMap.TileStartAddress > 0) // TODO: review
                    return VRAM.Bank1[MapAddress];
                else
                    return VRAM.Bank0[MapAddress];
            }  
        }
        public ushort MapAddress => (ushort)(Map + X + (Y * 0x20));
        public ushort TileAddress
        {
            get
            {
                if (backgroundMap.TileStartAddress == 0) // unsigned
                    return (ushort)(backgroundMap.TileStartAddress + (TileNumber << 4));
                else // signed
                    return (ushort)(((TileNumber & 0x80) == 0x80 ? 0x800 : 0x1000) + ((TileNumber & 0x7F) << 4));
            }   
        }
        public byte Attribute => VRAM.Bank1[MapAddress];
        public PaletteType BackgroundPaletteNumber => (PaletteType)((Attribute & 0x7) + 3);
        public int TileVRAMBankNumber => (Attribute & 0x8) == 0x8 ? 1 : 0;
        public bool HorizontalFlip => (Attribute & 0x20) == 0x20;
        public bool VerticalFlip => (Attribute & 0x40) == 0x40;
        public bool BGToOAMPriority => (Attribute & 0x80) == 0x80;

        public Tile(BackgroundMap backgroundMap, int x, int y)
        {
            this.backgroundMap = backgroundMap;
            X = x;
            Y = y;
        }
    }
}